﻿// <auto-generated />
using Blog.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Blog.Migrations
{
    [DbContext(typeof(BlogContext))]
    partial class BlogContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ApplicationCore.Entities.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activce");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<string>("Name");

                    b.Property<string>("PS");

                    b.Property<int>("PermissionId");

                    b.Property<bool>("Removed");

                    b.Property<string>("UpdatedBy");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("AppUsers");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Permisson", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AppUserId");

                    b.Property<string>("Name");

                    b.Property<bool>("Removed");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.ToTable("Permissons");
                });

            modelBuilder.Entity("Blog.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Code");

                    b.Property<string>("Name");

                    b.Property<int>("Order");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Blog.Models.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime>("Date");

                    b.Property<int>("DisplayOrder");

                    b.Property<bool>("Down");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<int>("Month");

                    b.Property<string>("Number");

                    b.Property<bool>("Removed");

                    b.Property<string>("Summary");

                    b.Property<string>("TermNumber");

                    b.Property<string>("Title");

                    b.Property<bool>("Top");

                    b.Property<string>("UpdatedBy");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Blog.Models.PostCategory", b =>
                {
                    b.Property<int>("PostId");

                    b.Property<int>("CategoryId");

                    b.HasKey("PostId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("PostsCategories");
                });

            modelBuilder.Entity("Blog.Models.UploadFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("Height");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<string>("Name");

                    b.Property<int>("Order");

                    b.Property<string>("PS");

                    b.Property<string>("Path");

                    b.Property<int>("PostId");

                    b.Property<string>("PreviewPath");

                    b.Property<string>("Title");

                    b.Property<string>("Type");

                    b.Property<string>("UpdatedBy");

                    b.Property<int>("Width");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("UploadFiles");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Permisson", b =>
                {
                    b.HasOne("ApplicationCore.Entities.AppUser")
                        .WithMany("Permissions")
                        .HasForeignKey("AppUserId");
                });

            modelBuilder.Entity("Blog.Models.PostCategory", b =>
                {
                    b.HasOne("Blog.Models.Category", "Category")
                        .WithMany("PostCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Blog.Models.Post", "Post")
                        .WithMany("PostCategories")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Blog.Models.UploadFile", b =>
                {
                    b.HasOne("Blog.Models.Post", "Post")
                        .WithMany("Attachments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
