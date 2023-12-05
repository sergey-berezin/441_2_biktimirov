using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Net;
using YoloView;

namespace YoloMVC.Models
{
    public class DetectedResult
    {
        public string Img64 { get; set; }
        public string Class { get; set; }
        public double Confidence { get; set; }
        public DetectedResult(string img64, string @class, double confidence) => (Img64, Class, Confidence) = (img64, @class, confidence);
    }
    public class Detector : IDetector
    {
        private YOLOlib.Detector detector;
        public Detector() => detector = new YOLOlib.Detector(new FileService());
        public async Task<List<DetectedResult>> ProcessImages(byte[] img)
        {
            var res = new List<DetectedResult>();
            var task = detector.Analyze(img, new CancellationToken());
            await Task.WhenAll(task);

            task.Result.detections.ForEach(x => res.Add(new DetectedResult(ToBase64String(task.Result.detectedImage), x.Item1, x.Item2)));

            return res;
        }

        public static string ToBase64String(Image<Rgb24> img)
        {
            var imgData = new byte[img.Width * img.Height * System.Runtime.CompilerServices.Unsafe.SizeOf<Rgb24>()];
            img.CopyPixelDataTo(imgData);

            string base64img = Convert.ToBase64String(imgData);
            string base64img1 = "empty64str";

            using (MemoryStream stream = new MemoryStream())
            {
                img.SaveAsJpeg(stream);
                base64img1 = Convert.ToBase64String(stream.ToArray());
            }

            return base64img1;
        }
    }
}
