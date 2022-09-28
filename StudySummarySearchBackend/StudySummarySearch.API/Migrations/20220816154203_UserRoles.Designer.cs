﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using StudySummarySearch.API.Data;

#nullable disable

namespace StudySummarySearch.API.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20220816154203_UserRoles")]
    partial class UserRoles
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("KeywordSummary", b =>
                {
                    b.Property<int>("KeywordsId")
                        .HasColumnType("integer");

                    b.Property<int>("SummariesId")
                        .HasColumnType("integer");

                    b.HasKey("KeywordsId", "SummariesId");

                    b.HasIndex("SummariesId");

                    b.ToTable("KeywordSummary");
                });

            modelBuilder.Entity("StudySummarySearch.API.Models.Keyword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Keywords");
                });

            modelBuilder.Entity("StudySummarySearch.API.Models.Semester", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Value")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Semesters");
                });

            modelBuilder.Entity("StudySummarySearch.API.Models.Subject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("StudySummarySearch.API.Models.Summary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SemesterId")
                        .HasColumnType("integer");

                    b.Property<int>("SubjectId")
                        .HasColumnType("integer");

                    b.Property<string>("URL")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("SemesterId");

                    b.HasIndex("SubjectId");

                    b.ToTable("Summaries");
                });

            modelBuilder.Entity("StudySummarySearch.API.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("DropboxAccessToken")
                        .HasColumnType("text");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            PasswordHash = new byte[] { 38, 182, 45, 11, 166, 57, 149, 161, 45, 25, 223, 182, 150, 194, 17, 88, 25, 233, 17, 117, 203, 138, 203, 154, 252, 22, 142, 39, 108, 119, 39, 183, 244, 2, 119, 225, 193, 73, 116, 193, 99, 192, 215, 191, 242, 224, 176, 61, 84, 193, 13, 98, 63, 16, 50, 250, 243, 77, 205, 64, 58, 198, 255, 205 },
                            PasswordSalt = new byte[] { 135, 51, 50, 60, 57, 100, 68, 185, 91, 72, 100, 100, 82, 236, 22, 133, 253, 135, 74, 63, 143, 18, 98, 36, 253, 80, 18, 105, 113, 124, 134, 97, 25, 228, 214, 177, 193, 232, 104, 72, 81, 174, 41, 108, 145, 195, 80, 119, 231, 236, 60, 174, 21, 193, 244, 191, 24, 140, 231, 83, 175, 166, 47, 25, 226, 20, 11, 57, 151, 65, 151, 103, 51, 46, 75, 247, 223, 0, 186, 215, 172, 34, 25, 252, 93, 214, 147, 60, 47, 229, 94, 173, 89, 98, 88, 159, 146, 118, 99, 210, 32, 201, 62, 241, 224, 94, 220, 53, 130, 232, 2, 106, 52, 202, 225, 196, 55, 67, 78, 214, 194, 102, 13, 62, 227, 162, 70, 7 },
                            Role = "SuperUser",
                            Username = "AdminGrafJ"
                        });
                });

            modelBuilder.Entity("KeywordSummary", b =>
                {
                    b.HasOne("StudySummarySearch.API.Models.Keyword", null)
                        .WithMany()
                        .HasForeignKey("KeywordsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StudySummarySearch.API.Models.Summary", null)
                        .WithMany()
                        .HasForeignKey("SummariesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("StudySummarySearch.API.Models.Summary", b =>
                {
                    b.HasOne("StudySummarySearch.API.Models.Semester", "Semester")
                        .WithMany("Summaries")
                        .HasForeignKey("SemesterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StudySummarySearch.API.Models.Subject", "Subject")
                        .WithMany("Summaries")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Semester");

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("StudySummarySearch.API.Models.Semester", b =>
                {
                    b.Navigation("Summaries");
                });

            modelBuilder.Entity("StudySummarySearch.API.Models.Subject", b =>
                {
                    b.Navigation("Summaries");
                });
#pragma warning restore 612, 618
        }
    }
}
