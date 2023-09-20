using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Net;

namespace dotnet_lab1v2YOLO
{
    internal class Program
    {
        public class FileServices : YOLOlib.IFileServices
        {
            public void Download(string url, string fileName)
            {
                using (var client = new WebClient())
                {
                    while (true)
                    {
                        try
                        {
                            client.DownloadFile(url, fileName);
                        }
                        catch (WebException)
                        {
                            continue;
                        }
                        break;
                    }
                }
            }
            public bool Exists(string path) => File.Exists(path);
        }
        static void Main(string[] args)
        {
            var detector = new YOLOlib.Detector(new FileServices());
            var threads = Enumerable.Range(0, args.Length).Select(i =>
            {
                var thread = new Thread(o =>
                {
                    lock(detector)
                        SaveResults(detector.Analyze(args[(int)o]));
                });
                thread.Start(i);
                return thread;
            }).ToArray();

            foreach(var thread in threads)
                thread.Join();
        }

        private static void SaveResults((string fileName, string curCsv, Image<Rgb24> curImg) res)
        {
            if (!File.Exists("results.csv") || !File.ReadLines("results.csv").Any(s => String.CompareOrdinal(s + "\n", res.curCsv) == 0))
                File.AppendAllText("results.csv", res.curCsv);

            res.curImg.Save(res.fileName);
        }
    }
}