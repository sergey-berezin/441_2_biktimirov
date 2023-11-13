﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using YoloViewModel;

#nullable disable

namespace YoloViewModel.Migrations
{
    [DbContext(typeof(ImageLibrary))]
    [Migration("20231113104326_LoadingImageFromBytesFix")]
    partial class LoadingImageFromBytesFix
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.12");

            modelBuilder.Entity("YoloViewModel.StoredImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("ImageData")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("SourceImageData")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<double>("confidence")
                        .HasColumnType("REAL");

                    b.Property<string>("detectedClass")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("sourcePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Images");
                });
#pragma warning restore 612, 618
        }
    }
}
