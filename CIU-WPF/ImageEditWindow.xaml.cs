using CIU_WPF.AppLayer;
using CIU_WPF.AppLayer.Dialogs;
using CIU_WPF.Common;
using CIU_WPF.Common.ImageProcessors;
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
    public partial class ImageEditWindow : Window
    {
        FileInfo _sourceFile;
        public FileInfo SourceFile => _sourceFile;
        public ImageEditWindow()
        {
            InitializeComponent();
        }

        internal void ShowFile(string file)
        {
            _sourceFile = null;
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
            }
            catch
            {

            }
            Show();
        }

        void ShowResizeDialog(object sender, RoutedEventArgs e)
        {
            ResizeSettingWindow wd = new ResizeSettingWindow();
            wd.SourceFile = SourceFile;
            wd.ShowDialog();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            AppLauncher.Instance.OnEditorClosed();
        }
    }
}
