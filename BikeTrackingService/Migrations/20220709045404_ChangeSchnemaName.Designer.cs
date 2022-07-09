﻿// <auto-generated />
using System;
using BikeTrackingService.BikeTrackingServiceDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BikeTrackingService.Migrations
{
    [DbContext(typeof(BikeTrackingDbContext))]
    [Migration("20220709045404_ChangeSchnemaName")]
    partial class ChangeSchnemaName
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BikeTrackingService.Models.Account", b =>
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

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("phone_number");

                    b.Property<double>("Point")
                        .HasColumnType("double precision")
                        .HasColumnName("point");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_account");

                    b.ToTable("account", (string)null);
                });

            modelBuilder.Entity("BikeTrackingService.Models.Bike", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    b.Property<int?>("BikeStationId")
                        .HasColumnType("integer")
                        .HasColumnName("bike_station_id");

                    b.Property<string>("BikeStationName")
                        .HasColumnType("text")
                        .HasColumnName("bike_station_name");

                    b.Property<string>("Color")
                        .HasColumnType("text")
                        .HasColumnName("color");

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
                        .HasName("pk_bikes");

                    b.ToTable("bikes", (string)null);
                });

            modelBuilder.Entity("BikeTrackingService.Models.BikeLocationTracking", b =>
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

            modelBuilder.Entity("BikeTrackingService.Models.BikeLocationTrackingHistory", b =>
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

            modelBuilder.Entity("BikeTrackingService.Models.BikeRentalTracking", b =>
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

            modelBuilder.Entity("BikeTrackingService.Models.BikeLocationTracking", b =>
                {
                    b.HasOne("BikeTrackingService.Models.Bike", "Bike")
                        .WithMany("BikeLocationTrackings")
                        .HasForeignKey("BikeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_location_tracking_bikes_bike_id");

                    b.Navigation("Bike");
                });

            modelBuilder.Entity("BikeTrackingService.Models.BikeLocationTrackingHistory", b =>
                {
                    b.HasOne("BikeTrackingService.Models.Bike", "Bike")
                        .WithMany()
                        .HasForeignKey("BikeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_location_tracking_history_bikes_bike_id");

                    b.HasOne("BikeTrackingService.Models.BikeRentalTracking", "BikeRentalTracking")
                        .WithMany()
                        .HasForeignKey("BikeRentalTrackingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_location_tracking_history_bike_rental_booking_bike_ren");

                    b.Navigation("Bike");

                    b.Navigation("BikeRentalTracking");
                });

            modelBuilder.Entity("BikeTrackingService.Models.BikeRentalTracking", b =>
                {
                    b.HasOne("BikeTrackingService.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_rental_booking_account_account_id");

                    b.HasOne("BikeTrackingService.Models.Bike", "Bike")
                        .WithMany()
                        .HasForeignKey("BikeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_rental_booking_bikes_bike_id");

                    b.Navigation("Account");

                    b.Navigation("Bike");
                });

            modelBuilder.Entity("BikeTrackingService.Models.Bike", b =>
                {
                    b.Navigation("BikeLocationTrackings");
                });
#pragma warning restore 612, 618
        }
    }
}
