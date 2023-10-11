using AsyncCommand;
using System.Windows.Input;

namespace YoloViewModel
{
    public interface IDirectoryServices
    {
        /// <summary>
        /// Returns the list of file names in choosed directory.
        /// </summary>
        /// <returns></returns>
        List<string> ChooseFolder();
        void Print(string message);
    }
    public class Detection
    {
        public string sourcePath { get; set; }
        public string detectedClass { get; set; }
        public double confidence { get; set; }
        public Detection(string p, string c, double d) => (sourcePath,detectedClass,confidence) = (p,c,d);
    }
    public class MainViewModel : ViewModelBase
    {
        public AsyncRelayCommand ChooseDirCmd { get; private set; }
        public AsyncRelayCommand CancelDetectingCmd { get; private set; }
        public List<Detection> Dets { get; private set; }
        private Detection? selection;
        public Detection? SelectedImg { get => selection;
            set { selection = value; RaisePropertyChanged(nameof(imgPath)); } }
        public string imgPath { get => SelectedImg != null ? SelectedImg.sourcePath.Replace("." + SelectedImg.sourcePath.Split(".").LastOrDefault(), "") + "Detected.jpg" : Directory.GetCurrentDirectory()+"\\kittens.jpg"; }
        private YOLOlib.Detector detector;
        private CancellationTokenSource tokenSource;
        public MainViewModel(YOLOlib.IFileServices fs, IDirectoryServices ds)
        {
            detector = new YOLOlib.Detector(fs);
            Dets = new List<Detection>();
            ChooseDirCmd = new AsyncRelayCommand(async _ =>
            {
                try
                {
                    List<string> selectedFiles = ds.ChooseFolder();
                    selectedFiles.RemoveAll(x => x.Split(".").LastOrDefault() != "jpg" || x.EndsWith("Detected.jpg"));

                    tokenSource = new CancellationTokenSource();
                    var tasks = Enumerable.Range(0, selectedFiles.Count).Select(i =>
                    {
                        return detector.Analyze(selectedFiles[(int)i], tokenSource.Token);
                    }).ToArray();

                    await Task.WhenAll(tasks);

                    for (int i = 0; i < selectedFiles.Count; ++i)
                    {
                        tasks[i].Result.detections.ForEach(x => Dets.Add(new Detection(selectedFiles[i], x.Item1, x.Item2)));
                        tasks[i].Result.detectedImage.Save(selectedFiles[i].Replace("." + selectedFiles[i].Split(".").LastOrDefault(), "") + "Detected.jpg");
                    }
                    Dets = Dets.OrderBy(x => x.detectedClass).ThenBy(y => y.confidence).ToList();
                    RaisePropertyChanged(nameof(Dets));
                }
                catch (OperationCanceledException)
                {
                    ds.Print("Detection cancelled!");
                }
            });
            CancelDetectingCmd = new AsyncRelayCommand(_ => { tokenSource?.Cancel(); return Task.CompletedTask; });
        }
    }
}