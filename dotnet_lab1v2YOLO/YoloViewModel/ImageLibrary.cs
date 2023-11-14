using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace YoloViewModel
{
    public class StoredImage
    {
        public int Id { get; set; }
        public string sourcePath { get; set; }
        public byte[] ImageData { get; set; }
        public byte[] SourceImageData { get; set; }
        public string detectedClass { get; set; }
        public double confidence { get; set; }
        public int sizesH { get; set; }
        public int sizesW { get; set; }
        public int sourceSizesH { get; set; }
        public int sourceSizesW { get; set; }
        public StoredImage(Image<Rgb24> i, Image<Rgb24> sourceImg, string p, string c, double d)
        {
            ImageData = new byte[i.Width * i.Height * Unsafe.SizeOf<Rgb24>()];
            i.CopyPixelDataTo(ImageData);
            (sizesH, sizesW) = (i.Height, i.Width);

            SourceImageData = new byte[sourceImg.Width * sourceImg.Height * Unsafe.SizeOf<Rgb24>()];
            sourceImg.CopyPixelDataTo(SourceImageData);
            (sourceSizesH, sourceSizesW) = (sourceImg.Height, sourceImg.Width);

            (sourcePath, detectedClass, confidence) = (p, c, d);
        }
        public StoredImage() { }
    }

    public class LoadedImage
    {
        public int Id { get; set; }
        public string sourcePath { get; set; }
        public ImageSharpImageSource<Rgb24> Image { get; init; }
        public ImageSharpImageSource<Rgb24> SourceImage { get; init; }
        public string detectedClass { get; set; }
        public double confidence { get; set; }
        public LoadedImage(StoredImage storedImg) => (Id, sourcePath, Image, SourceImage, detectedClass, confidence) = (storedImg.Id, storedImg.sourcePath, new ImageSharpImageSource<Rgb24>(ByteArrayToImage(storedImg.ImageData, storedImg.sizesH, storedImg.sizesW)), new ImageSharpImageSource<Rgb24>(ByteArrayToImage(storedImg.SourceImageData, storedImg.sourceSizesH, storedImg.sourceSizesW)), storedImg.detectedClass, storedImg.confidence);
        public Image<Rgb24> ByteArrayToImage(byte[] byteArrayIn, int height, int width) => Image<Rgb24>.LoadPixelData<Rgb24>(byteArrayIn, width, height);
    }

    class ImageLibrary : DbContext
    {
        public DbSet<StoredImage> Images { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder o) => o.UseSqlite("Data Source=C:\\Users\\bikmi\\Personal Files\\study\\МГУ\\Учебные материалы\\IV курс\\С# dotnet\\dotnet_YOLO\\dotnet_lab1v2YOLO\\YoloViewModel\\library.db");
        public async Task AddImage(StoredImage img) => await Images.AddAsync(img);
    }
}
