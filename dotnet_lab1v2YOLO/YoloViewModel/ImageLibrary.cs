using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace YoloViewModel
{
    public class StoredImage
    {
        public int Id { get; set; }
        public string sourcePath { get; set; }
        //public ImageSharpImageSource<Rgb24> img { get; init; }
        public byte[] ImageData { get; set; }
        public string detectedClass { get; set; }
        public double confidence { get; set; }
        public StoredImage(Image<Rgb24> i, string p, string c, double d)
        {
            using var ms = new MemoryStream();
            i.SaveAsJpeg(ms);
            ImageData = ms.ToArray();
            (sourcePath, detectedClass, confidence) = (p, c, d);
        }
        public StoredImage() { }
    }

    public class LoadedImage
    {
        public int Id { get; set; }
        public string sourcePath { get; set; }
        public ImageSharpImageSource<Rgb24> Image { get; init; }
        public string detectedClass { get; set; }
        public double confidence { get; set; }
        public LoadedImage(StoredImage storedImg) => (Id, sourcePath, Image, detectedClass, confidence) = (storedImg.Id, storedImg.sourcePath, new ImageSharpImageSource<Rgb24>(ByteArrayToImage(storedImg.ImageData)), storedImg.detectedClass, storedImg.confidence);
        public Image<Rgb24> ByteArrayToImage(byte[] byteArrayIn)
        {
            using (var ms = new MemoryStream(byteArrayIn))
            {
                return Image<Rgb24>.Load<Rgb24>(ms);
            }
        }
    }

    class ImageLibrary : DbContext
    {
        public DbSet<StoredImage> Images { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder o) => o.UseSqlite("Data Source=C:\\Users\\bikmish\\workspace\\cmc\\4 курс\\C# dotnet\\lab1\\dotnet_YOLO\\dotnet_lab1v2YOLO\\YoloViewModel\\library.db");
        //o.UseSqlite("Data Source=\"C:\\Users\\bikmish\\workspace\\cmc\\4 курс\\C# dotnet\\lab1\\dotnet_YOLO\\dotnet_lab1v2YOLO\\lib\\library.db\"");
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<StoredImage>()
        //        .HasKey(img => img.Id);
        //}
        public async Task AddImage(StoredImage img) => await Images.AddAsync(img);
    }
}
