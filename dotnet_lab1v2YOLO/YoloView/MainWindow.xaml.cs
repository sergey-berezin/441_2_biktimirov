using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Forms;
using YoloViewModel;

namespace YoloView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(new FileService(), new DirectoryService());
        }
    }

    public class FileService : YOLOlib.IFileServices
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

        public void Print(string msg) => Console.WriteLine(msg);
    }

    public class DirectoryService : IDirectoryServices
    {
        public List<string> ChooseFolder()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Choose folder with source images.";
            var fileList = new List<string>();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in Directory.GetFiles(fbd.SelectedPath))
                    fileList.Add(file);
            }
            return fileList;
        }

        public void Print(string msg) => System.Windows.MessageBox.Show(msg);
    }
}
