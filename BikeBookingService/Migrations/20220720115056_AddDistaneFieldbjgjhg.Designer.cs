﻿// <auto-generated />
using System;
using BikeBookingService.BikeTrackingServiceDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BikeBookingService.Migrations
{
    [DbContext(typeof(BikeTrackingDbContext))]
    [Migration("20220720115056_AddDistaneFieldbjgjhg")]
    partial class AddDistaneFieldbjgjhg
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BikeBookingService.Models.Account", b =>
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

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("phone_number");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_account");

                    b.ToTable("account", (string)null);
                });

            modelBuilder.Entity("BikeBookingService.Models.Bike", b =>
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
                        .HasColumnType("timestamp without time zone")
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

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_bikes");

                    b.ToTable("bikes", (string)null);
                });

            modelBuilder.Entity("BikeBookingService.Models.BikeLocationTracking", b =>
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
                        .HasColumnType("timestamp without time zone")
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
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_bike_location_tracking");

                    b.HasIndex("BikeId")
                        .HasDatabaseName("ix_bike_location_tracking_bike_id");

                    b.ToTable("bike_location_tracking", (string)null);
                });

            modelBuilder.Entity("BikeBookingService.Models.BikeLocationTrackingHistory", b =>
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
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_on");

                    b.Property<double>("DistanceFromPreviousLocation")
                        .HasColumnType("double precision")
                        .HasColumnName("distance_from_previous_location");

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
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_bike_location_tracking_history");

                    b.HasIndex("BikeId")
                        .HasDatabaseName("ix_bike_location_tracking_history_bike_id");

                    b.HasIndex("BikeRentalTrackingId")
                        .HasDatabaseName("ix_bike_location_tracking_history_bike_rental_tracking_id");

                    b.ToTable("bike_location_tracking_history", (string)null);
                });

            modelBuilder.Entity("BikeBookingService.Models.BikeRentalBooking", b =>
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
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("checkin_on");

                    b.Property<DateTime?>("CheckoutOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("checkout_on");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_on");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("PaymentStatus")
                        .HasColumnType("text")
                        .HasColumnName("payment_status");

                    b.Property<double>("TotalPoint")
                        .HasColumnType("double precision")
                        .HasColumnName("total_point");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_bike_rental_booking");

                    b.HasIndex("AccountId")
                        .HasDatabaseName("ix_bike_rental_booking_account_id");

                    b.HasIndex("BikeId")
                        .HasDatabaseName("ix_bike_rental_booking_bike_id");

                    b.ToTable("bike_rental_booking", (string)null);
                });

            modelBuilder.Entity("BikeBookingService.Models.BikeLocationTracking", b =>
                {
                    b.HasOne("BikeBookingService.Models.Bike", "Bike")
                        .WithMany("BikeLocationTrackings")
                        .HasForeignKey("BikeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_location_tracking_bikes_bike_id");

                    b.Navigation("Bike");
                });

            modelBuilder.Entity("BikeBookingService.Models.BikeLocationTrackingHistory", b =>
                {
                    b.HasOne("BikeBookingService.Models.Bike", "Bike")
                        .WithMany()
                        .HasForeignKey("BikeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_location_tracking_history_bikes_bike_id");

                    b.HasOne("BikeBookingService.Models.BikeRentalBooking", "BikeRentalBooking")
                        .WithMany()
                        .HasForeignKey("BikeRentalTrackingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_location_tracking_history_bike_rental_booking_bike_ren");

                    b.Navigation("Bike");

                    b.Navigation("BikeRentalBooking");
                });

            modelBuilder.Entity("BikeBookingService.Models.BikeRentalBooking", b =>
                {
                    b.HasOne("BikeBookingService.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_rental_booking_account_account_id");

                    b.HasOne("BikeBookingService.Models.Bike", "Bike")
                        .WithMany()
                        .HasForeignKey("BikeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bike_rental_booking_bikes_bike_id");

                    b.Navigation("Account");

                    b.Navigation("Bike");
                });

            modelBuilder.Entity("BikeBookingService.Models.Bike", b =>
                {
                    b.Navigation("BikeLocationTrackings");
                });
#pragma warning restore 612, 618
        }
    }
}
