﻿// <auto-generated />
using System;
using BatteryCommander.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BatteryCommander.Web.Migrations
{
    [DbContext(typeof(Database))]
    [Migration("20200208012613_SUTA_Signatures")]
    partial class SUTA_Signatures
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1");

            modelBuilder.Entity("BatteryCommander.Web.Models.ABCP", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<decimal>("Height")
                        .HasColumnType("TEXT");

                    b.Property<string>("MeasurementsJson")
                        .HasColumnType("TEXT");

                    b.Property<int>("SoldierId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Weight")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SoldierId");

                    b.ToTable("ABCPs");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.ACFT", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<bool>("ForRecord")
                        .HasColumnType("INTEGER");

                    b.Property<int>("HandReleasePushups")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LegTucks")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SoldierId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SprintDragCarrySeconds")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StandingPowerThrow")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ThreeRepMaximumDeadlifts")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TwoMileRunSeconds")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SoldierId");

                    b.ToTable("ACFTs");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.APFT", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte>("AerobicEvent")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<int>("PushUps")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RunSeconds")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SitUps")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SoldierId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SoldierId");

                    b.ToTable("APFTs");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Evaluation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("EvaluationId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RateeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RaterId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ReviewerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SeniorRaterId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("date");

                    b.Property<byte>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ThruDate")
                        .HasColumnType("date");

                    b.Property<byte>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RateeId");

                    b.HasIndex("RaterId");

                    b.HasIndex("ReviewerId");

                    b.HasIndex("SeniorRaterId");

                    b.ToTable("Evaluations");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Evaluation+Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Author")
                        .HasColumnType("TEXT");

                    b.Property<int>("EvaluationId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Message")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EvaluationId");

                    b.ToTable("Event");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.SUTA", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CommanderSignature")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CommanderSignedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstSergeantSignature")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("FirstSergeantSignedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("MitigationPlan")
                        .HasColumnType("TEXT");

                    b.Property<string>("Reasoning")
                        .HasColumnType("TEXT");

                    b.Property<int>("SoldierId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SupervisorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SupervisorSignature")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("SupervisorSignedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SoldierId");

                    b.HasIndex("SupervisorId");

                    b.ToTable("SUTAs");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.SUTA+Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Author")
                        .HasColumnType("TEXT");

                    b.Property<string>("Message")
                        .HasColumnType("TEXT");

                    b.Property<int>("SUTAId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SUTAId");

                    b.ToTable("SUTA_Events");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Soldier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CanLogin")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CivilianEmail")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("ClsQualificationDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<DateTime?>("DateOfRank")
                        .HasColumnType("date");

                    b.Property<string>("DoDId")
                        .HasColumnType("TEXT")
                        .HasMaxLength(12);

                    b.Property<DateTime?>("DscaQualificationDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ETSDate")
                        .HasColumnType("date");

                    b.Property<byte>("EducationLevel")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<byte>("Gender")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("IwqQualificationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("MiddleName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("MilitaryEmail")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<byte>("Rank")
                        .HasColumnType("INTEGER");

                    b.Property<byte>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SupervisorId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UnitId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("DoDId")
                        .IsUnique();

                    b.HasIndex("SupervisorId");

                    b.HasIndex("UnitId");

                    b.HasIndex("FirstName", "MiddleName", "LastName")
                        .IsUnique();

                    b.ToTable("Soldiers");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Soldier+SSDSnapshot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("AsOf")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("PerecentComplete")
                        .HasColumnType("TEXT");

                    b.Property<byte>("SSD")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SoldierId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SoldierId");

                    b.ToTable("SSDSnapshot");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Unit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IgnoreForReports")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("ReportSettingsJson")
                        .HasColumnType("TEXT");

                    b.Property<string>("UIC")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UIC")
                        .IsUnique();

                    b.ToTable("Units");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Vehicle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("A_DriverId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Bumper")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(10);

                    b.Property<byte>("Chalk")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DriverId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HasFuelCard")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HasJBCP")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HasTowBar")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LIN")
                        .HasColumnType("TEXT")
                        .HasMaxLength(10);

                    b.Property<DateTimeOffset>("LastUpdate")
                        .HasColumnType("TEXT");

                    b.Property<byte>("Location")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Nomenclature")
                        .HasColumnType("TEXT")
                        .HasMaxLength(20);

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<int>("OrderOfMarch")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Registration")
                        .HasColumnType("TEXT")
                        .HasMaxLength(10);

                    b.Property<int>("Seats")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Serial")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<byte>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TroopCapacity")
                        .HasColumnType("INTEGER");

                    b.Property<byte>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UnitId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("A_DriverId");

                    b.HasIndex("DriverId");

                    b.HasIndex("Registration")
                        .IsUnique();

                    b.HasIndex("Serial")
                        .IsUnique();

                    b.HasIndex("UnitId", "Bumper")
                        .IsUnique();

                    b.ToTable("Vehicles");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Vehicle+Passenger", b =>
                {
                    b.Property<int>("VehicleId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SoldierId")
                        .HasColumnType("INTEGER");

                    b.HasKey("VehicleId", "SoldierId");

                    b.HasIndex("SoldierId");

                    b.ToTable("Passengers");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Weapon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AdminNumber")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(10);

                    b.Property<int?>("AssignedId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("OpticSerial")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<byte>("OpticType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Serial")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<byte>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UnitId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AssignedId");

                    b.HasIndex("UnitId");

                    b.HasIndex("Serial", "Type")
                        .IsUnique();

                    b.ToTable("Weapons");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FriendlyName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Xml")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DataProtectionKeys");
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.ABCP", b =>
                {
                    b.HasOne("BatteryCommander.Web.Models.Soldier", "Soldier")
                        .WithMany("ABCPs")
                        .HasForeignKey("SoldierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.ACFT", b =>
                {
                    b.HasOne("BatteryCommander.Web.Models.Soldier", "Soldier")
                        .WithMany("ACFTs")
                        .HasForeignKey("SoldierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.APFT", b =>
                {
                    b.HasOne("BatteryCommander.Web.Models.Soldier", "Soldier")
                        .WithMany("APFTs")
                        .HasForeignKey("SoldierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Evaluation", b =>
                {
                    b.HasOne("BatteryCommander.Web.Models.Soldier", "Ratee")
                        .WithMany()
                        .HasForeignKey("RateeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BatteryCommander.Web.Models.Soldier", "Rater")
                        .WithMany()
                        .HasForeignKey("RaterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BatteryCommander.Web.Models.Soldier", "Reviewer")
                        .WithMany()
                        .HasForeignKey("ReviewerId");

                    b.HasOne("BatteryCommander.Web.Models.Soldier", "SeniorRater")
                        .WithMany()
                        .HasForeignKey("SeniorRaterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Evaluation+Event", b =>
                {
                    b.HasOne("BatteryCommander.Web.Models.Evaluation", "Evaluation")
                        .WithMany("Events")
                        .HasForeignKey("EvaluationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.SUTA", b =>
                {
                    b.HasOne("BatteryCommander.Web.Models.Soldier", "Soldier")
                        .WithMany()
                        .HasForeignKey("SoldierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BatteryCommander.Web.Models.Soldier", "Supervisor")
                        .WithMany()
                        .HasForeignKey("SupervisorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.SUTA+Event", b =>
                {
                    b.HasOne("BatteryCommander.Web.Models.SUTA", "SUTA")
                        .WithMany("Events")
                        .HasForeignKey("SUTAId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Soldier", b =>
                {
                    b.HasOne("BatteryCommander.Web.Models.Soldier", "Supervisor")
                        .WithMany()
                        .HasForeignKey("SupervisorId");

                    b.HasOne("BatteryCommander.Web.Models.Unit", "Unit")
                        .WithMany("Soldiers")
                        .HasForeignKey("UnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Soldier+SSDSnapshot", b =>
                {
                    b.HasOne("BatteryCommander.Web.Models.Soldier", "Soldier")
                        .WithMany("SSDSnapshots")
                        .HasForeignKey("SoldierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Vehicle", b =>
                {
                    b.HasOne("BatteryCommander.Web.Models.Soldier", "A_Driver")
                        .WithMany()
                        .HasForeignKey("A_DriverId");

                    b.HasOne("BatteryCommander.Web.Models.Soldier", "Driver")
                        .WithMany()
                        .HasForeignKey("DriverId");

                    b.HasOne("BatteryCommander.Web.Models.Unit", "Unit")
                        .WithMany("Vehicles")
                        .HasForeignKey("UnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Vehicle+Passenger", b =>
                {
                    b.HasOne("BatteryCommander.Web.Models.Soldier", "Soldier")
                        .WithMany()
                        .HasForeignKey("SoldierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BatteryCommander.Web.Models.Vehicle", "Vehicle")
                        .WithMany("Passengers")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BatteryCommander.Web.Models.Weapon", b =>
                {
                    b.HasOne("BatteryCommander.Web.Models.Soldier", "Assigned")
                        .WithMany()
                        .HasForeignKey("AssignedId");

                    b.HasOne("BatteryCommander.Web.Models.Unit", "Unit")
                        .WithMany()
                        .HasForeignKey("UnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}