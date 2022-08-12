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
    [Migration("20220811043557_UpdateProfileDatabase")]
    partial class UpdateProfileDatabase
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.7");

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

                    b.Property<string>("no_hp")
                        .HasColumnType("TEXT");

                    b.Property<string>("password")
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

                    b.Property<string>("no_hp")
                        .HasColumnType("TEXT");

                    b.Property<string>("password")
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.ToTable("UsersChanger");
                });
#pragma warning restore 612, 618
        }
    }
}
