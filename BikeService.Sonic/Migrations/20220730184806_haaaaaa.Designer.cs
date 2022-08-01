﻿// <auto-generated />
using System;
using BikeService.Sonic.BikeDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    [DbContext(typeof(BikeServiceDbContext))]
    [Migration("20220730184806_haaaaaa")]
    partial class haaaaaa
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BikeService.Sonic.Models.Bike", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("BikeStationId")
                        .HasColumnType("integer")
                        .HasColumnName("bike_station_id");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_on");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<bool>("IsLock")
                        .HasColumnType("boolean")
                        .HasColumnName("is_lock");

                    b.Property<string>("LicensePlate")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("license_plate");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_bike");

                    b.HasIndex("BikeStationId")
                        .HasDatabaseName("ix_bike_bike_station_id");

                    b.ToTable("bike", (string)null);
                });

            modelBuilder.Entity("BikeService.Sonic.Models.BikeReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AccountEmail")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("account_email");

                    b.Property<int>("AssignToId")
                        .HasColumnType("integer")
                        .HasColumnName("assign_to_id");

                    b.Property<int>("BikeId")
                        .HasColumnType("integer")
                        .HasColumnName("bike_id");

                    b.Property<DateTime?>("CompletedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("completed_on");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_on");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("ReportDescription")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("report_description");

                    b.Property<int>("ReportTypeId")
                        .HasColumnType("integer")
                        .HasColumnName("report_type_id");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_bike_report");

                    b.HasIndex("AssignToId")
                        .HasDatabaseName("ix_bike_report_assign_to_id");

                    b.HasIndex("BikeId")
                        .HasDatabaseName("ix_bike_report_bike_id");

                    b.HasIndex("ReportTypeId")
                        .HasDatabaseName("ix_bike_report_report_type_id");

                    b.ToTable("bike_report", (string)null);
                });

            modelBuilder.Entity("BikeService.Sonic.Models.BikeReportType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_on");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_bike_report_type");

                    b.ToTable("bike_report_type", (string)null);
                });

            modelBuilder.Entity("BikeService.Sonic.Models.BikeStation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("address");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_on");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision")
                        .HasColumnName("latitude");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision")
                        .HasColumnName("longitude");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int>("ParkingSpace")
                        .HasColumnType("integer")
                        .HasColumnName("parking_space");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_on");

                    b.Property<int>("UsedParkingSpace")
                        .HasColumnType("integer")
                        .HasColumnName("used_parking_space");

                    b.HasKey("Id")
                        .HasName("pk_bike_station");

                    b.ToTable("bike_station", (string)null);
                });

            modelBuilder.Entity("BikeService.Sonic.Models.BikeStationColor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BikeStationId")
                        .HasColumnType("integer")
                        .HasColumnName("bike_station_id");

                    b.Property<string>("Color")
                        .HasColumnType("text")
                        .HasColumnName("color");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_on");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<int>("ManagerId")
                        .HasColumnType("integer")
                        .HasColumnName("manager_id");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_bike_station_color");

                    b.HasIndex("BikeStationId")
                        .HasDatabaseName("ix_bike_station_color_bike_station_id");

                    b.HasIndex("ManagerId")
                        .HasDatabaseName("ix_bike_station_color_manager_id");

                    b.ToTable("bike_station_color", (string)null);
                });

            modelBuilder.Entity("BikeService.Sonic.Models.BikeStationManager", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BikeStationId")
                        .HasColumnType("integer")
                        .HasColumnName("bike_station_id");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_on");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<int>("ManagerId")
                        .HasColumnType("integer")
                        .HasColumnName("manager_id");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_bike_station_manager");

                    b.HasIndex("BikeStationId")
                        .HasDatabaseName("ix_bike_station_manager_bike_station_id");

                    b.HasIndex("ManagerId")
                        .HasDatabaseName("ix_bike_station_manager_manager_id");

                    b.ToTable("bike_station_manager", (string)null);
                });

            modelBuilder.Entity("BikeService.Sonic.Models.Manager", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_on");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("external_id");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<bool>("IsSuperManager")
                        .HasColumnType("boolean")
                        .HasColumnName("is_super_manager");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_manager");

                    b.ToTable("manager", (string)null);
                });

            modelBuilder.Entity("BikeService.Sonic.Models.Bike", b =>
                {
                    b.HasOne("BikeService.Sonic.Models.BikeStation", "BikeStation")
                        .WithMany("Bikes")
                        .HasForeignKey("BikeStationId")
                        .HasConstraintName("fk_bike_bike_station_bike_station_id");

                    b.Navigation("BikeStation");
                });

            modelBuilder.Entity("BikeService.Sonic.Models.BikeReport", b =>
                {
                    b.HasOne("BikeService.Sonic.Models.Manager", "AssignTo")
                        .WithMany()
                        .HasForeignKey("AssignToId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_report_manager_assign_to_id");

                    b.HasOne("BikeService.Sonic.Models.Bike", "Bike")
                        .WithMany()
                        .HasForeignKey("BikeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_report_bike_bike_id");

                    b.HasOne("BikeService.Sonic.Models.BikeReportType", "ReportType")
                        .WithMany()
                        .HasForeignKey("ReportTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_report_bike_report_type_report_type_id");

                    b.Navigation("AssignTo");

                    b.Navigation("Bike");

                    b.Navigation("ReportType");
                });

            modelBuilder.Entity("BikeService.Sonic.Models.BikeStationColor", b =>
                {
                    b.HasOne("BikeService.Sonic.Models.BikeStation", "BikeStation")
                        .WithMany("BikeStationColors")
                        .HasForeignKey("BikeStationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_station_color_bike_station_bike_station_id");

                    b.HasOne("BikeService.Sonic.Models.Manager", "Manager")
                        .WithMany("BikeStationColors")
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_station_color_manager_manager_id");

                    b.Navigation("BikeStation");

                    b.Navigation("Manager");
                });

            modelBuilder.Entity("BikeService.Sonic.Models.BikeStationManager", b =>
                {
                    b.HasOne("BikeService.Sonic.Models.BikeStation", "BikeStation")
                        .WithMany("BikeStationManagers")
                        .HasForeignKey("BikeStationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_station_manager_bike_station_bike_station_id");

                    b.HasOne("BikeService.Sonic.Models.Manager", "Manager")
                        .WithMany("BikeStationManagers")
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_station_manager_manager_manager_id");

                    b.Navigation("BikeStation");

                    b.Navigation("Manager");
                });

            modelBuilder.Entity("BikeService.Sonic.Models.BikeStation", b =>
                {
                    b.Navigation("BikeStationColors");

                    b.Navigation("BikeStationManagers");

                    b.Navigation("Bikes");
                });

            modelBuilder.Entity("BikeService.Sonic.Models.Manager", b =>
                {
                    b.Navigation("BikeStationColors");

                    b.Navigation("BikeStationManagers");
                });
#pragma warning restore 612, 618
        }
    }
}