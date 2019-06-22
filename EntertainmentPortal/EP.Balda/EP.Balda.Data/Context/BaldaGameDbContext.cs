﻿using EP.Balda.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EP.Balda.Data.Context
{
    public class BaldaGameDbContext : DbContext
    {
        public BaldaGameDbContext(DbContextOptions<BaldaGameDbContext> options)
            : base(options: options)
        {
        }

        public DbSet<PlayerDb> Players { get; set; }

        public DbSet<GameDb> Games { get; set; }
        
        public DbSet<MapDb> Maps { get; set; }

        public DbSet<CellDb> Cells { get; set; }

        public DbSet<WordsSourceDb> Words { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var playerEntity = modelBuilder.Entity<PlayerDb>();
            playerEntity.Property(p => p.Login).IsRequired().HasMaxLength(30);
            playerEntity.Property(p => p.NickName).IsRequired().HasMaxLength(30);
            playerEntity.Property(p => p.Password).IsRequired();
            
            var gameEntity = modelBuilder.Entity<GameDb>();
            gameEntity.HasOne(g => g.Map).WithOne();

            var playerGameEntry = modelBuilder.Entity<PlayerGameDb>();
            playerGameEntry
            .HasKey(pg => new { pg.PlayerId, pg.GameId });
            modelBuilder.Entity<PlayerGameDb>()
                .HasOne(pg => pg.Player)
                .WithMany(pg => pg.PlayerGames)
                .HasForeignKey(pg => pg.PlayerId);
            modelBuilder.Entity<PlayerGameDb>()
                .HasOne(pg => pg.Game)
                .WithMany(pg => pg.PlayerGames)
                .HasForeignKey(pg => pg.GameId);

            var playerWordEntry = modelBuilder.Entity<PlayerWordDb>();
            playerWordEntry
            .HasKey(pw => new { pw.PlayerId, pw.WordId });
            modelBuilder.Entity<PlayerWordDb>()
                .HasOne(pw => pw.Player)
                .WithMany(pw => pw.PlayerWords)
                .HasForeignKey(pw => pw.PlayerId);
            modelBuilder.Entity<PlayerWordDb>()
                .HasOne(pw => pw.Word)
                .WithMany(pw => pw.PlayerWords)
                .HasForeignKey(pw => pw.WordId);

            var mapEntity = modelBuilder.Entity<MapDb>();
            mapEntity.HasKey(m => m.Id);
            mapEntity.HasMany(m => m.Cells).WithOne();

            var cellEntity = modelBuilder.Entity<CellDb>();
            cellEntity.HasKey(c => c.Id);
            cellEntity.Property(c => c.X).IsRequired();
            cellEntity.Property(c => c.Y).IsRequired();
            cellEntity.HasOne(c => c.Map).WithMany(c => c.Cells).HasForeignKey(c => c.MapId);

            var wordEntity = modelBuilder.Entity<WordsSourceDb>();
            wordEntity.HasKey(w => w.Id);
        }
    }
}