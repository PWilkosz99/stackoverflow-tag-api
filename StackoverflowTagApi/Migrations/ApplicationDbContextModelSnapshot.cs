﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StackoverflowTagApi.Data;

#nullable disable

namespace StackoverflowTagApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.3");

            modelBuilder.Entity("StackoverflowTagApi.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Count")
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Relational:JsonPropertyName", "count");

                    b.Property<bool>("IsModeratorOnly")
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Relational:JsonPropertyName", "is_moderator_only");

                    b.Property<bool>("IsRequired")
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Relational:JsonPropertyName", "is_required");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "name");

                    b.Property<string>("User_id")
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "user_id");

                    b.Property<bool>("has_synonyms")
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Relational:JsonPropertyName", "has_synonyms");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}
