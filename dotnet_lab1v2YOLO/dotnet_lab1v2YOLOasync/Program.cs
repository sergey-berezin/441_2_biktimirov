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
                            Console.WriteLine("Downloading...");
                            client.DownloadFile(url, fileName);
                            Console.WriteLine("Succesfully downloaded!");
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

            public void Print(string msg) => Console.WriteLine(msg);

        }
        static async Task Main(string[] args)
        {
            var detector = new YOLOlib.Detector(new FileServices());
            var tokenSource = new CancellationTokenSource();
            var tasks = Enumerable.Range(0, args.Length).Select(i =>
            {
                var t = detector.Analyze(args[(int)i], tokenSource.Token);
                Console.WriteLine($"{i}");
                return t;
            }).ToArray();

            Console.WriteLine("Press Enter to continue or type 'c' to cancel detection");
            if (Console.ReadLine() == "c")
                tokenSource.Cancel();

            try
            {
                await Task.WhenAll(tasks);
                foreach (var res in tasks)
                    SaveResults(res.Result);
            }
            catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
            {
                Console.WriteLine("Detecting cancelled!");
            }
        }

        private static void SaveResults((string fileName, string curCsv, Image<Rgb24> curImg) res)
        {
            if (!File.Exists("resultsAsync.csv") || !File.ReadLines("resultsAsync.csv").Any(s => String.CompareOrdinal(s + "\n", res.curCsv) == 0))
                File.AppendAllText("resultsAsync.csv", res.curCsv);

            res.curImg.Save(res.fileName);
        }
    }
}