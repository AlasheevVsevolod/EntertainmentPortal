﻿using EP.TicTacToe.Data.Context.Configuration;
using EP.TicTacToe.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EP.TicTacToe.Data.Context
{
    public class TicTacDbContext : IdentityDbContext
    {
        public DbSet<PlayerDb> Players { get; set; }
        public DbSet<GameDb> Games { get; set; }
        public DbSet<MapDb> Maps { get; set; }
        public DbSet<ChainDb> Chains { get; set; }
        public DbSet<CellDb> Steps { get; set; }

        public TicTacDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new PlayerConfiguration());
            modelBuilder.ApplyConfiguration(new GameConfiguration());
            modelBuilder.ApplyConfiguration(new MapConfiguration());
            modelBuilder.ApplyConfiguration(new ChainConfiguration());
            modelBuilder.ApplyConfiguration(new CellConfiguration());
        }
    }
}

// *******************************************
// * Example syntax for use migrations:
// * Add-Migration InitialCreateDb -OutputDir Migrations\GameDbMigrations -Context TicTacDbContext -Project EP.TicTacToe.Data -StartupProject EP.TicTacToe.Web
// * Update-Database InitialCreateDb -Context TicTacDbContext -Project EP.TicTacToe.Data -StartupProject EP.TicTacToe.Web
// *******************************************