﻿using AutoMapper;
using CSharpFunctionalExtensions;
using EP.Balda.Data.Context;
using EP.Balda.Data.Models;
using EP.Balda.Logic.Commands;
using EP.Balda.Logic.Handlers;
using EP.Balda.Logic.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace EP.Balda.Tests.EP.Balda.Handlers.Tests
{
    [TestFixture]
    public class AddLetterToCellHandler_Tests
    {
        IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _mapper = new Mock<IMapper>().Object;
        }

        [Test]
        public async Task TestIsAllowedCell_True()
        {
            var options = new DbContextOptionsBuilder<BaldaGameDbContext>()
                .UseInMemoryDatabase(databaseName: "TestIsAllowedCell_True")
                .Options;

            bool isAllowed;

            using (var context = new BaldaGameDbContext(options))
            {
                var service = new AddLetterToCellHandler(context, _mapper);
                var mapDb = new MapDb() { Id = 1, Size = 3 };
                await context.Cells.AddAsync(new CellDb() { MapId = 1, Map = mapDb, X = 0, Y = 0, Letter = 'g' });
                var cell = new CellDb() { MapId = 1, Map = mapDb, X = 0, Y = 1, Letter = null };
                await context.SaveChangesAsync();
                isAllowed = await service.IsAllowedCell(cell);
            }

            using (var context = new BaldaGameDbContext(options))
            {
                Assert.IsTrue(isAllowed);
            }
        }

        [Test]
        public async Task TestIsAllowedCell_False()
        {
            var options = new DbContextOptionsBuilder<BaldaGameDbContext>()
                .UseInMemoryDatabase(databaseName: "TestIsAllowedCell_False")
                .Options;

            bool isAllowed;

            using (var context = new BaldaGameDbContext(options))
            {
                var service = new AddLetterToCellHandler(context, _mapper);
                var mapDb = new MapDb() { Id = 1, Size = 3 };
                await context.Cells.AddAsync(new CellDb() { Id = 1, MapId = 1, Map = mapDb, X = 0, Y = 0, Letter = 'g' });
                var cell = new CellDb() { MapId = 1, Map = mapDb, X = 2, Y = 2, Letter = null };
                await context.SaveChangesAsync();
                isAllowed = await service.IsAllowedCell(cell);
            }

            using (var context = new BaldaGameDbContext(options))
            {
                Assert.IsFalse(isAllowed);
            }
        }

        [Test]
        public async Task TestAddLetterToCell_Handle_NormalData()
        {
            var options = new DbContextOptionsBuilder<BaldaGameDbContext>()
                .UseInMemoryDatabase(databaseName: "TestAddLetterToCell_Handle_NormalData")
                .Options;

            var request = new AddLetterToCellCommand()
            {
                Id = 1,
                Letter = 'c'
            };

            Result<Cell> result;

            using (var context = new BaldaGameDbContext(options))
            {
                var service = new AddLetterToCellHandler(context, _mapper);
                var mapDb = new MapDb() { Id = 1, Size = 3 };
                await context.Maps.AddAsync(mapDb);
                await context.SaveChangesAsync();
                var cellNearBy = new CellDb() { Id = 2, MapId = 1, Map = mapDb, X = 2, Y = 3, Letter = 'v' };
                var cell = new CellDb() { Id = 1, MapId = 1, Map = mapDb, X = 2, Y = 2, Letter = null };
                await context.Cells.AddAsync(cellNearBy);
                await context.Cells.AddAsync(cell);
                await context.SaveChangesAsync();
                result = await service.Handle(request, CancellationToken.None);
            }

            using (var context = new BaldaGameDbContext(options))
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual(2, await context.Cells.CountAsync());
            }
        }

        [Test]
        public async Task TestAddLetterToCell_Handle_CellDoesntExist()
        {
            var options = new DbContextOptionsBuilder<BaldaGameDbContext>()
                .UseInMemoryDatabase(databaseName: "TestAddLetterToCell_Handle_CellDoesntExist")
                .Options;

            var request = new AddLetterToCellCommand()
            {
                Id = 1,
                Letter = 'c'
            };

            Result<Cell> result;

            using (var context = new BaldaGameDbContext(options))
            {
                var service = new AddLetterToCellHandler(context, _mapper);
                result = await service.Handle(request, CancellationToken.None);
            }

            using (var context = new BaldaGameDbContext(options))
            {
                Assert.IsFalse(result.IsSuccess);
            }
        }

        [Test]
        public async Task TestAddLetterToCell_Handle_CellLetterNotNull()
        {
            var options = new DbContextOptionsBuilder<BaldaGameDbContext>()
                .UseInMemoryDatabase(databaseName: "TestAddLetterToCell_Handle_CellLetterNotNull")
                .Options;

            var request = new AddLetterToCellCommand()
            {
                Id = 2,
                Letter = 'c'
            };

            Result<Cell> result;

            using (var context = new BaldaGameDbContext(options))
            {
                var service = new AddLetterToCellHandler(context, _mapper);
                var mapDb = new MapDb() { Size = 3 };
                await context.Maps.AddAsync(mapDb);
                await context.SaveChangesAsync();
                var cellNearBy = new CellDb() { MapId = 1, Map = mapDb, X = 2, Y = 3, Letter = 'v' };
                var cell = new CellDb() { MapId = 1, Map = mapDb, X = 2, Y = 2, Letter = 'b' };
                await context.Cells.AddAsync(cellNearBy);
                await context.Cells.AddAsync(cell);
                await context.SaveChangesAsync();
                result = await service.Handle(request, CancellationToken.None);
            }

            using (var context = new BaldaGameDbContext(options))
            {
                var expected = new Result<Cell>() { };
                Assert.IsFalse(result.IsSuccess);
            }
        }
    }
}