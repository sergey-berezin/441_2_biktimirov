using AsyncCommand;
using System.Collections.Concurrent;
using System.Diagnostics;
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

    public class MainViewModel : ViewModelBase
    {
        public AsyncRelayCommand ChooseDirCmd { get; private set; }
        public AsyncRelayCommand CancelDetectingCmd { get; private set; }
        public AsyncRelayCommand ClearDatabaseCmd { get; private set; }
        private List<string> storedImagesHash => imgLib.Images.Select(x => x.sourcePath + x.detectedClass + $"{x.confidence:N2}").ToList();
        private List<string> storedImagesSourcePaths => imgLib.Images.Select(x => x.sourcePath).Distinct().ToList();
        public List<LoadedImage> LoadedImages => Detections.Select(x => new LoadedImage(x)).OrderBy(x => x.detectedClass).ThenBy(y => y.confidence).ToList();
        private ConcurrentStack<StoredImage> detections;
        public List<StoredImage> Detections { 
            get {
                if (detections.Any() == true)
                {
                    while(detections.TryPop(out var det) == true)
                        if (!storedImagesHash.Contains(GetImageHash(det)))
                        {
                            Debug.Print($"Added {GetImageHash(det)}, {detections.Count} left");
                            imgLib.Images.Add(det);
                        }
                    imgLib.SaveChanges();
                }
                return imgLib.Images.ToList(); 
            } 
        }
        private LoadedImage? selectedLoad;
        public LoadedImage? SelectedLoadedImg
        {
            get => selectedLoad;
            set { selectedLoad = value; RaisePropertyChanged(nameof(SelectedLoadedImg)); }
        }
        private YOLOlib.Detector detector;
        private CancellationTokenSource? tokenSource;
        private ImageLibrary imgLib = new ImageLibrary();
        private string GetImageHash(StoredImage img) => img.sourcePath + img.detectedClass + $"{img.confidence:N2}";
        public MainViewModel(YOLOlib.IFileServices fs, IDirectoryServices ds)
        {
            detector = new YOLOlib.Detector(fs);
            detections = new ConcurrentStack<StoredImage>();

            ChooseDirCmd = new AsyncRelayCommand(async _ =>
            {
                try
                {
                    List<string> selectedFiles = ds.ChooseFolder();
                    selectedFiles.RemoveAll(x => x.Split(".").LastOrDefault() != "jpg" || x.EndsWith("Detected.jpg") || storedImagesSourcePaths.Contains(x));

                    tokenSource = new CancellationTokenSource();
                    var tasks = Enumerable.Range(0, selectedFiles.Count).Select(i =>
                    {
                        return detector.Analyze(selectedFiles[(int)i], tokenSource.Token);
                    }).ToArray();

                    await Task.WhenAll(tasks);

                    for (int i = 0; i < selectedFiles.Count; ++i)
                    {
                        tasks[i].Result.detections.ForEach(x =>
                        {
                            var detectedResult = new StoredImage(tasks[i].Result.detectedImage, Image.Load<Rgb24>(selectedFiles[i]), selectedFiles[i], x.Item1, x.Item2);
                            if (!detections.Any(x => GetImageHash(x) == GetImageHash(detectedResult)))
                                detections.Push(detectedResult);
                        });
                    }
                    RaisePropertyChanged(nameof(LoadedImages));
                    tokenSource = null;
                }
                catch (OperationCanceledException)
                {
                    ds.Print("Detection cancelled!");
                }
            });
            CancelDetectingCmd = new AsyncRelayCommand(_ => { tokenSource?.Cancel(); return Task.CompletedTask; }, _ => tokenSource != null);
            ClearDatabaseCmd = new AsyncRelayCommand(_ => { imgLib.Images.RemoveRange(imgLib.Images); imgLib.SaveChanges(); Debug.Print("Clearing db..."); RaisePropertyChanged(nameof(LoadedImages)); return Task.CompletedTask; }, _ => imgLib.Images.Any());
        }
    }
}