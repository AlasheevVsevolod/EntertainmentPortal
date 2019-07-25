﻿using System.Collections.Generic;
using CSharpFunctionalExtensions;
using EP.Balda.Data.Models;
using MediatR;

namespace EP.Balda.Logic.Queries
{
    public class GetAllWords : IRequest<Maybe<IEnumerable<WordDb>>>
    {
    }
}