﻿// <auto-generated />
using System;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace api.Migrations
{
    [DbContext(typeof(BlogContext))]
    partial class BlogContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity("API.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Color")
                        .IsRequired();

                    b.Property<string>("EnglishName")
                        .IsRequired();

                    b.Property<string>("FrenchName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("API.Models.PostItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CategoryId");

                    b.Property<DateTime>("Creation");

                    b.Property<string>("EnglishContent");

                    b.Property<string>("EnglishTitle");

                    b.Property<bool>("Favorite");

                    b.Property<string>("FrenchContent");

                    b.Property<string>("FrenchTitle");

                    b.Property<string>("Image");

                    b.Property<int>("Media");

                    b.Property<int>("ReadingTime");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("PostItems");
                });

            modelBuilder.Entity("API.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("PostItemId");

                    b.Property<string>("language");

                    b.HasKey("Id");

                    b.HasIndex("PostItemId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("API.Models.PostItem", b =>
                {
                    b.HasOne("API.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("API.Models.Tag", b =>
                {
                    b.HasOne("API.Models.PostItem", "PostItem")
                        .WithMany("Tags")
                        .HasForeignKey("PostItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
