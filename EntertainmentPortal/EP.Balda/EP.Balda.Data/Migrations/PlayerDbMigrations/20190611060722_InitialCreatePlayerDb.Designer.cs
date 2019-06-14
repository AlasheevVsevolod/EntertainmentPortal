﻿// <auto-generated />
using EP.Balda.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EP.Balda.Data.Migrations.PlayerDbMigrations
{
    [DbContext(typeof(PlayerDbContext))]
    [Migration("20190611060722_InitialCreatePlayerDb")]
    partial class InitialCreatePlayerDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("EP.Balda.Data.Models.PlayerDb", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Login");

                    b.Property<string>("NickName");

                    b.Property<string>("Password");

                    b.Property<int>("Score");

                    b.HasKey("Id");

                    b.ToTable("Players");

                });
#pragma warning restore 612, 618
        }
    }
}
