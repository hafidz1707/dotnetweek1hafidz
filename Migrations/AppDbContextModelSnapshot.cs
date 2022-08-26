﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WeekOneApi.Infrastructure.Data;

#nullable disable

namespace WeekOneApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

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

            modelBuilder.Entity("WeekOneApi.Infrastructure.Shared.ComplaintView", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("circle_check_header_id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("notes")
                        .HasColumnType("TEXT");

                    b.Property<int>("service_registration_id")
                        .HasColumnType("INTEGER");

                    b.HasKey("id");

                    b.HasIndex("circle_check_header_id")
                        .IsUnique();

                    b.ToTable("ComplaintViews");
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

                    b.HasIndex("circle_check_header_id");

                    b.ToTable("ExteriorViews");
                });

            modelBuilder.Entity("WeekOneApi.Infrastructure.Shared.InteriorView", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("circle_check_header_id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("fuel_gauge")
                        .HasColumnType("INTEGER");

                    b.Property<string>("interior_photo_1")
                        .HasColumnType("TEXT");

                    b.Property<string>("interior_photo_2")
                        .HasColumnType("TEXT");

                    b.Property<string>("interior_photo_3")
                        .HasColumnType("TEXT");

                    b.Property<int>("other_stuff")
                        .HasColumnType("INTEGER");

                    b.Property<string>("other_stuff_notes")
                        .HasColumnType("TEXT");

                    b.Property<int>("safety_kit")
                        .HasColumnType("INTEGER");

                    b.Property<int>("service_booklet")
                        .HasColumnType("INTEGER");

                    b.Property<int>("service_registration_id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("spare_tire")
                        .HasColumnType("INTEGER");

                    b.Property<int>("stnk")
                        .HasColumnType("INTEGER");

                    b.HasKey("id");

                    b.HasIndex("circle_check_header_id")
                        .IsUnique();

                    b.ToTable("InteriorViews");
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
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("back_left")
                        .HasColumnType("TEXT");

                    b.Property<string>("back_left_photo_1")
                        .HasColumnType("TEXT");

                    b.Property<string>("back_left_photo_2")
                        .HasColumnType("TEXT");

                    b.Property<string>("back_left_photo_3")
                        .HasColumnType("TEXT");

                    b.Property<string>("back_right")
                        .HasColumnType("TEXT");

                    b.Property<string>("back_right_photo_1")
                        .HasColumnType("TEXT");

                    b.Property<string>("back_right_photo_2")
                        .HasColumnType("TEXT");

                    b.Property<string>("back_right_photo_3")
                        .HasColumnType("TEXT");

                    b.Property<int>("circle_check_header_id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("front_left")
                        .HasColumnType("TEXT");

                    b.Property<string>("front_left_photo_1")
                        .HasColumnType("TEXT");

                    b.Property<string>("front_left_photo_2")
                        .HasColumnType("TEXT");

                    b.Property<string>("front_left_photo_3")
                        .HasColumnType("TEXT");

                    b.Property<string>("front_right")
                        .HasColumnType("TEXT");

                    b.Property<string>("front_right_photo_1")
                        .HasColumnType("TEXT");

                    b.Property<string>("front_right_photo_2")
                        .HasColumnType("TEXT");

                    b.Property<string>("front_right_photo_3")
                        .HasColumnType("TEXT");

                    b.Property<int>("service_registration_id")
                        .HasColumnType("INTEGER");

                    b.HasKey("id");

                    b.HasIndex("circle_check_header_id")
                        .IsUnique();

                    b.ToTable("TireViews");
                });

            modelBuilder.Entity("WeekOneApi.Infrastructure.Shared.ComplaintView", b =>
                {
                    b.HasOne("WeekOneApi.Infrastructure.Shared.CircleCheck", null)
                        .WithOne("complaint_notes_view")
                        .HasForeignKey("WeekOneApi.Infrastructure.Shared.ComplaintView", "circle_check_header_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WeekOneApi.Infrastructure.Shared.ExteriorView", b =>
                {
                    b.HasOne("WeekOneApi.Infrastructure.Shared.CircleCheck", null)
                        .WithMany("exterior_view")
                        .HasForeignKey("circle_check_header_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WeekOneApi.Infrastructure.Shared.InteriorView", b =>
                {
                    b.HasOne("WeekOneApi.Infrastructure.Shared.CircleCheck", null)
                        .WithOne("interior_view")
                        .HasForeignKey("WeekOneApi.Infrastructure.Shared.InteriorView", "circle_check_header_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WeekOneApi.Infrastructure.Shared.TireView", b =>
                {
                    b.HasOne("WeekOneApi.Infrastructure.Shared.CircleCheck", null)
                        .WithOne("tire_view")
                        .HasForeignKey("WeekOneApi.Infrastructure.Shared.TireView", "circle_check_header_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WeekOneApi.Infrastructure.Shared.CircleCheck", b =>
                {
                    b.Navigation("complaint_notes_view");

                    b.Navigation("exterior_view");

                    b.Navigation("interior_view");

                    b.Navigation("tire_view");
                });
#pragma warning restore 612, 618
        }
    }
}
