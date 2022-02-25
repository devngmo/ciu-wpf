using CIU_WPF.AppLayer;
using CIU_WPF.AppLayer.Dialogs;
using CIU_WPF.Common;
using CIU_WPF.Common.ImageProcessors;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ImageViewerWindow : Window
    {
        Serilog.ILogger _lg;
        FileInfo _sourceFile;
        public FileInfo SourceFile => _sourceFile;
        public ImageViewerWindow(Serilog.ILogger logger)
        {
            _lg = logger;
            InitializeComponent();
        }

        void SwitchToEditorWindow(object sender, RoutedEventArgs args)
        {
            Hide();
            AppLauncher.Instance.ShowEditor(SourceFile.FullName);
        }

        void OpenResizeDialog(object sender, RoutedEventArgs args)
        {
            ResizeSettingWindow dlg = new ResizeSettingWindow();
            dlg.SourceFile = SourceFile;
            dlg.ShowDialog();
        }

        void OpenConvertDialog(object sender, RoutedEventArgs args)
        {
            ResizeSettingWindow dlg = new ResizeSettingWindow();
            dlg.SourceFile = SourceFile;
            dlg.ShowDialog();
        }

        internal void ShowFile(string file)
        {
            _sourceFile = null;
            _lg.Information($"Show File: {file}");
            if (!FileUtils.IsImageFilePath(file))
            {
                Show();
                return;
            }
            try
            {
                FileInfo fi = new FileInfo(file);
                if (!fi.Exists)
                {
                    Show();
                    return;
                }
                _sourceFile = fi;
                RenderCanvas.Source = ImageIOUtils.CreateBitmapImageFromFile(file);
                Width = RenderCanvas.Source.Width + Padding.Left + Padding.Right + 50;
                Height = RenderCanvas.Source.Height + Padding.Top + Padding.Bottom + 50;

                _lg.Information($"Window Caption Height: {SystemParameters.CaptionHeight}");
                _lg.Information($"Image: {RenderCanvas.Source.Width} x {RenderCanvas.Source.Height}");
                _lg.Information($"Window: {Width} x {Height}");
            }
            catch
            {

            }
            Show();
        }
    }
}
