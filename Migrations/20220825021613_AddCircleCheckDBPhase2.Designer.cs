﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WeekOneApi.Infrastructure.Data;

#nullable disable

namespace WeekOneApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20220825021613_AddCircleCheckDBPhase2")]
    partial class AddCircleCheckDBPhase2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.8");

            modelBuilder.Entity("WeekOneApi.Infrastructure.Data.Models.AuthToken", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int>("expiredAt")
                        .HasColumnType("INTEGER");

                    b.Property<string>("role")
                        .HasColumnType("TEXT");

                    b.Property<string>("token")
                        .HasColumnType("TEXT");

                    b.Property<int>("userId")
                        .HasColumnType("INTEGER");

                    b.HasKey("id");

                    b.ToTable("AuthTokens");
                });

            modelBuilder.Entity("WeekOneApi.Infrastructure.Data.Models.User", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("DeviceId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Version")
                        .HasColumnType("TEXT");

                    b.Property<string>("dealer_code")
                        .HasColumnType("TEXT");

                    b.Property<string>("dealer_name")
                        .HasColumnType("TEXT");

                    b.Property<string>("email")
                        .HasColumnType("TEXT");

                    b.Property<bool>("is_registered")
                        .HasColumnType("INTEGER");

                    b.Property<string>("name")
                        .HasColumnType("TEXT");

                    b.Property<string>("password")
                        .HasColumnType("TEXT");

                    b.Property<string>("phone")
                        .HasColumnType("TEXT");

                    b.Property<int?>("pin_otp")
                        .HasColumnType("INTEGER");

                    b.Property<string>("position_code")
                        .HasColumnType("TEXT");

                    b.Property<string>("position_name")
                        .HasColumnType("TEXT");

                    b.Property<string>("username")
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WeekOneApi.Infrastructure.Data.Models.UserChanger", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("email")
                        .HasColumnType("TEXT");

                    b.Property<string>("password")
                        .HasColumnType("TEXT");

                    b.Property<string>("phone")
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.ToTable("UsersChanger");
                });

            modelBuilder.Entity("WeekOneApi.Infrastructure.Shared.CircleCheck", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("customer_name")
                        .HasColumnType("TEXT");

                    b.Property<string>("dealer_service")
                        .HasColumnType("TEXT");

                    b.Property<string>("phone")
                        .HasColumnType("TEXT");

                    b.Property<string>("plate_number")
                        .HasColumnType("TEXT");

                    b.Property<string>("service_advisor")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("service_date")
                        .HasColumnType("TEXT");

                    b.Property<int>("service_registration_id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("signature")
                        .HasColumnType("TEXT");

                    b.Property<string>("vehicle_model")
                        .HasColumnType("TEXT");

                    b.Property<string>("vin")
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.ToTable("CircleChecks");
                });

            modelBuilder.Entity("WeekOneApi.Infrastructure.Shared.ExteriorView", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("circle_check_header_id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("data_type")
                        .HasColumnType("INTEGER");

                    b.Property<string>("data_type_text")
                        .HasColumnType("TEXT");

                    b.Property<string>("image_path")
                        .HasColumnType("TEXT");

                    b.Property<string>("notes")
                        .HasColumnType("TEXT");

                    b.Property<int>("service_registration_id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("vehicle_condition")
                        .HasColumnType("INTEGER");

                    b.HasKey("id");

                    b.ToTable("ExteriorViews");
                });

            modelBuilder.Entity("WeekOneApi.Infrastructure.Shared.ServiceList", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("booking_date_time")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("create_time")
                        .HasColumnType("TEXT");

                    b.Property<string>("estimated_service")
                        .HasColumnType("TEXT");

                    b.Property<string>("input_source")
                        .HasColumnType("TEXT");

                    b.Property<int>("input_source_id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("is_ontime")
                        .HasColumnType("TEXT");

                    b.Property<string>("is_rework")
                        .HasColumnType("TEXT");

                    b.Property<string>("is_vip")
                        .HasColumnType("TEXT");

                    b.Property<string>("name")
                        .HasColumnType("TEXT");

                    b.Property<string>("plate_number")
                        .HasColumnType("TEXT");

                    b.Property<string>("preferred_sa")
                        .HasColumnType("TEXT");

                    b.Property<string>("queue_number")
                        .HasColumnType("TEXT");

                    b.Property<string>("reg_number")
                        .HasColumnType("TEXT");

                    b.Property<string>("service_advisor")
                        .HasColumnType("TEXT");

                    b.Property<string>("status")
                        .HasColumnType("TEXT");

                    b.Property<int>("status_id")
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan?>("waiting_time")
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.ToTable("ServiceLists");
                });

            modelBuilder.Entity("WeekOneApi.Infrastructure.Shared.TireView", b =>
                {
                    b.Property<string>("back_left")
                        .HasColumnType("TEXT");

                    b.Property<string>("back_right")
                        .HasColumnType("TEXT");

                    b.Property<int>("circle_check_header_id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("front_left")
                        .HasColumnType("TEXT");

                    b.Property<string>("front_right")
                        .HasColumnType("TEXT");

                    b.Property<int>("id")
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int>("service_registration_id")
                        .HasColumnType("INTEGER");

                    b.ToTable("TireViews");
                });
#pragma warning restore 612, 618
        }
    }
}
