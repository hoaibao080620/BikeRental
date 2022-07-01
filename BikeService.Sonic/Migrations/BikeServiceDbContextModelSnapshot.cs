﻿// <auto-generated />
using System;
using BikeService.Sonic.BikeDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    [DbContext(typeof(BikeServiceDbContext))]
    partial class BikeServiceDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BikeService.Sonic.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone")
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

                    b.Property<double>("Point")
                        .HasColumnType("double precision")
                        .HasColumnName("point");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_user");

                    b.ToTable("user", (string)null);
                });

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
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("LicensePlate")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("license_plate");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_bike");

                    b.HasIndex("BikeStationId")
                        .HasDatabaseName("ix_bike_bike_station_id");

                    b.ToTable("bike", (string)null);
                });

            modelBuilder.Entity("BikeService.Sonic.Models.BikeLocationTracking", b =>
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

                    b.Property<int>("BikeId")
                        .HasColumnType("integer")
                        .HasColumnName("bike_id");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision")
                        .HasColumnName("latitude");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision")
                        .HasColumnName("longitude");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_bike_location_tracking");

                    b.HasIndex("BikeId")
                        .HasDatabaseName("ix_bike_location_tracking_bike_id");

                    b.ToTable("bike_location_tracking", (string)null);
                });

            modelBuilder.Entity("BikeService.Sonic.Models.BikeLocationTrackingHistory", b =>
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

                    b.Property<int>("BikeId")
                        .HasColumnType("integer")
                        .HasColumnName("bike_id");

                    b.Property<int>("BikeRentalTrackingId")
                        .HasColumnType("integer")
                        .HasColumnName("bike_rental_tracking_id");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision")
                        .HasColumnName("latitude");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision")
                        .HasColumnName("longitude");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_bike_location_tracking_history");

                    b.HasIndex("BikeId")
                        .HasDatabaseName("ix_bike_location_tracking_history_bike_id");

                    b.HasIndex("BikeRentalTrackingId")
                        .HasDatabaseName("ix_bike_location_tracking_history_bike_rental_tracking_id");

                    b.ToTable("bike_location_tracking_history", (string)null);
                });

            modelBuilder.Entity("BikeService.Sonic.Models.BikeRentalTracking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountId")
                        .HasColumnType("integer")
                        .HasColumnName("account_id");

                    b.Property<int>("BikeId")
                        .HasColumnType("integer")
                        .HasColumnName("bike_id");

                    b.Property<DateTime>("CheckinOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("checkin_on");

                    b.Property<DateTime?>("CheckoutOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("checkout_on");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<double>("TotalPoint")
                        .HasColumnType("double precision")
                        .HasColumnName("total_point");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_bike_rental_booking");

                    b.HasIndex("AccountId")
                        .HasDatabaseName("ix_bike_rental_booking_account_id");

                    b.HasIndex("BikeId")
                        .HasDatabaseName("ix_bike_rental_booking_bike_id");

                    b.ToTable("bike_rental_booking", (string)null);
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
                        .HasColumnType("timestamp with time zone")
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
                        .HasColumnType("timestamp with time zone")
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
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<int>("ManagerId")
                        .HasColumnType("integer")
                        .HasColumnName("manager_id");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone")
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
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<int>("ManagerId")
                        .HasColumnType("integer")
                        .HasColumnName("manager_id");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone")
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
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on");

                    b.Property<string>("Email")
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
                        .HasColumnType("timestamp with time zone")
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

            modelBuilder.Entity("BikeService.Sonic.Models.BikeLocationTracking", b =>
                {
                    b.HasOne("BikeService.Sonic.Models.Bike", "Bike")
                        .WithMany("BikeLocationTrackings")
                        .HasForeignKey("BikeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_location_tracking_bike_bike_id");

                    b.Navigation("Bike");
                });

            modelBuilder.Entity("BikeService.Sonic.Models.BikeLocationTrackingHistory", b =>
                {
                    b.HasOne("BikeService.Sonic.Models.Bike", "Bike")
                        .WithMany()
                        .HasForeignKey("BikeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_location_tracking_history_bike_bike_id");

                    b.HasOne("BikeService.Sonic.Models.BikeRentalTracking", "BikeRentalTracking")
                        .WithMany()
                        .HasForeignKey("BikeRentalTrackingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_location_tracking_history_bike_rental_booking_bike_ren");

                    b.Navigation("Bike");

                    b.Navigation("BikeRentalTracking");
                });

            modelBuilder.Entity("BikeService.Sonic.Models.BikeRentalTracking", b =>
                {
                    b.HasOne("BikeService.Sonic.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_rental_booking_user_account_id");

                    b.HasOne("BikeService.Sonic.Models.Bike", "Bike")
                        .WithMany()
                        .HasForeignKey("BikeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_rental_booking_bike_bike_id");

                    b.Navigation("Account");

                    b.Navigation("Bike");
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

            modelBuilder.Entity("BikeService.Sonic.Models.Bike", b =>
                {
                    b.Navigation("BikeLocationTrackings");
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
