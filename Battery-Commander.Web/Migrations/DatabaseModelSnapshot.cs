﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using BatteryCommander.Web.Models;

namespace BatteryCommander.Web.Migrations
{
    [DbContext(typeof(Database))]
    partial class DatabaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("BatteryCommander.Web.Models.APFT", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<int>("PushUps");

                    b.Property<int>("RunSeconds");

                    b.Property<int>("SitUps");

                    b.Property<int>("SoldierId");

                    b.HasKey("Id");

                    b.HasIndex("SoldierId");

                    b.ToTable("APFTs");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Evaluation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("RateeId");

                    b.Property<int>("RaterId");

                    b.Property<int>("SeniorRaterId");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("date");

                    b.Property<byte>("Status");

                    b.Property<DateTime>("ThruDate")
                        .HasColumnType("date");

                    b.Property<byte>("Type");

                    b.HasKey("Id");

                    b.HasIndex("RateeId");

                    b.HasIndex("RaterId");

                    b.HasIndex("SeniorRaterId");

                    b.ToTable("Evaluations");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Evaluation+Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<int?>("EvaluationId");

                    b.Property<string>("Message");

                    b.Property<DateTimeOffset>("Timestamp");

                    b.HasKey("Id");

                    b.HasIndex("EvaluationId");

                    b.ToTable("Event");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Soldier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CivilianEmail")
                        .HasMaxLength(50);

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<string>("DoDId")
                        .HasMaxLength(12);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<byte>("Gender");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("MilitaryEmail")
                        .HasMaxLength(50);

                    b.Property<byte>("Rank");

                    b.HasKey("Id");

                    b.ToTable("Soldiers");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.APFT", b =>
                {
                    b.HasOne("BatteryCommander.Web.Models.Soldier", "Soldier")
                        .WithMany("APFTs")
                        .HasForeignKey("SoldierId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Evaluation", b =>
                {
                    b.HasOne("BatteryCommander.Web.Models.Soldier", "Ratee")
                        .WithMany()
                        .HasForeignKey("RateeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BatteryCommander.Web.Models.Soldier", "Rater")
                        .WithMany()
                        .HasForeignKey("RaterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BatteryCommander.Web.Models.Soldier", "SeniorRater")
                        .WithMany()
                        .HasForeignKey("SeniorRaterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Evaluation+Event", b =>
                {
                    b.HasOne("BatteryCommander.Web.Models.Evaluation")
                        .WithMany("Events")
                        .HasForeignKey("EvaluationId");
                });
        }
    }
}
