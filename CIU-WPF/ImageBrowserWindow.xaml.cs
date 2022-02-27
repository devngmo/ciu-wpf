using CIU_WPF.AppLayer;
using CIU_WPF.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CIU_WPF
{
    /// <summary>
    /// Interaction logic for ImageBrowserWindow.xaml
    /// </summary>
    public partial class ImageBrowserWindow : Window
    {
        string curFolderPath;
        List<string> imageFiles = new List<string>();
        int curImageIndex = -1;
        Thread threadUpdateThumbnailAsync;
        public ImageBrowserWindow()
        {
            InitializeComponent();
            threadUpdateThumbnailAsync = new Thread(new ThreadStart(_UpdateThumbnailAsyncProc));
            ThumbnailCacheManager.Ins.Init(new ThumbnailCacheManager.ThumbnailCacheManagerSettings {
                CacheFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ThumbnailCache")
            });

            ThumbnailCacheManager.Ins.ThumbnailCreatedEventHandler = onThumbnailCreated;
        }

        delegate void d_onThumbnailCreated(object? sender, ThumbnailCacheManager.CreateThumbnailArgs e);
        private void onThumbnailCreated(object? sender, ThumbnailCacheManager.CreateThumbnailArgs e)
        {

            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(new d_onThumbnailCreated(onThumbnailCreated), e);
                return;
            }
            Image img = new Image();
            img.Width = 100;
            img.Height = 100;
            img.Source = ThumbnailCacheStorage.Ins.loadAsBitmapImage(e.originFilePath);
            thumbnailsContainer.Children.Add(img);
        }

        internal void Browse(string dir)
        {
            curFolderPath = dir;
            Title = "Image Browser: " + dir;
            Reload();
            Show();
        }

        private void Reload()
        {
            imageView.Source = null;
            imageFiles = GetImages();
            if (imageFiles.Count == 0)
            {
                return;
            }

            ShowImageAt(0);
            UpdateThumbnails();
        }

        private void UpdateThumbnails()
        {
            ThumbnailCacheManager.Ins.CreateThumbnailsFromFiles(imageFiles, 100, 100);
        }

        private void _UpdateThumbnailAsyncProc()
        {
            for (int i = 0; i < imageFiles.Count; i++)
            {
                createThumbnailPreview(imageFiles[i]);
            }
        }

        private void createThumbnailPreview(string f)
        {
            ThumbnailCacheManager.Ins.createThumbnail(f, 100, 100);
        }

        private List<string> GetImages()
        {
            List<string> files = new List<string>();
            try
            {
                string[] allFiles = Directory.GetFiles(curFolderPath);
                
                if (allFiles != null)
                {
                    foreach (string file in allFiles)
                    {
                        if (FileUtils.IsImageFilePath(file))
                            files.Add(file);
                    }
                }
            }
            catch
            {

            }
            return files;
        }

        void windowPreviewKeyDown(object sender, KeyEventArgs key)
        {
            if (imageFiles.Count == 0) return;

            if (key.Key == Key.Left)
            {
                if (curImageIndex > 0) ShowImageAt(curImageIndex - 1);
            }
            else if (key.Key == Key.Right)
            {
                if (curImageIndex  + 1 < imageFiles.Count) ShowImageAt(curImageIndex + 1);
            }
            else if (key.Key == Key.F)
            {
                if (toolbar.Visibility == Visibility.Collapsed)
                    toolbar.Visibility = Visibility.Visible;
                else
                    toolbar.Visibility = Visibility.Collapsed;
            }
        }

        void showDialogResizeAll(object sender, RoutedEventArgs a)
        {

        }
        private void ShowImageAt(int index)
        {
            curImageIndex = index;
            imageView.Source = new BitmapImage(new Uri(imageFiles[index]));
            FileInfo fi = new FileInfo(imageFiles[index]);
            Title = $"Image Browser: {index + 1}/{imageFiles.Count}: {fi.Name} Size: {(int)imageView.Source.Width}x{(int)imageView.Source.Height}";
        }

        void Exit(object sender, EventArgs e)
        {
            Close();
        }
    }
}
