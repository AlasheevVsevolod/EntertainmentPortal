﻿using AutoMapper;
using EP.Sudoku.Data.Context;
using EP.Sudoku.Logic.Models;
using EP.Sudoku.Logic.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EP.Sudoku.Logic.Handlers
{
    public class GetPlayerByIdHandler : IRequestHandler<GetPlayerById, Player>
    {
        private readonly SudokuDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public GetPlayerByIdHandler(SudokuDbContext context, IMapper mapper, IMemoryCache memoryCache)
        {
            _context = context;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<Player> Handle(GetPlayerById request, CancellationToken cancellationToken)
        {
            Player chosenPlayer = null;
            if (!_memoryCache.TryGetValue(request.Id, out chosenPlayer)) //caching "One"
            {
                chosenPlayer = _context.Players
                .Include(p => p.IconDb)
                .Include(p => p.GameSessionsDb)
                .Where(x => x.Id == request.Id)
                .Select(b => _mapper.Map<Player>(b)).FirstOrDefault();

                if (chosenPlayer != null)
                {
                    _memoryCache.Set(chosenPlayer.Id, chosenPlayer,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(60)));
                }
            }            

            return await Task.FromResult(chosenPlayer);
        }
    }
}
