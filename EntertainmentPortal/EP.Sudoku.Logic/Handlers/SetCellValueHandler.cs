﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using EP.Sudoku.Data.Context;
using EP.Sudoku.Logic.Commands;
using EP.Sudoku.Logic.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EP.Sudoku.Logic.Handlers
{
    public class SetCellValueHandler : IRequestHandler<SetCellValueCommand, Result<Session>>
    {
        private readonly SudokuDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidator<SetCellValueCommand> _validator;

        public SetCellValueHandler(SudokuDbContext context, IMapper mapper, IValidator<SetCellValueCommand> validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<Result<Session>> Handle(SetCellValueCommand request, CancellationToken cancellationToken)
        {
            var result = _validator.Validate(request, ruleSet: "IsValidSudokuGameSet");

            if (result.Errors.Count > 0)
            {
                return Result.Fail<Session>(result.Errors.First().ErrorMessage);
            }

            var session = _context.Sessions
                .Include(d => d.SquaresDb)
                .First(d => d.Id == request.SessionId);
            session.SquaresDb.First(x => x.Id == request.Id).Value = request.Value;
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok<Session>(_mapper.Map<Session>(session));
        }


    }
}