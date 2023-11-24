using SixLabors.ImageSharp.PixelFormats;
using System.Net;
using YoloView;

namespace YoloMVC.Models
{
    public class DetectedResult
    {
        public byte[] ImgData { get; set; }
        public string Class { get; set; }
        public double Confidence { get; set; }
        public DetectedResult(byte[] imgData, string @class, double confidence) => (ImgData, Class, Confidence) = (imgData, @class, confidence);
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

            var imgData = new byte[task.Result.detectedImage.Width * task.Result.detectedImage.Height * System.Runtime.CompilerServices.Unsafe.SizeOf<Rgb24>()];
            task.Result.detectedImage.CopyPixelDataTo(imgData);
            task.Result.detections.ForEach(x => res.Add(new DetectedResult(imgData, x.Item1, x.Item2)));

            return res;
        }
    }
}
