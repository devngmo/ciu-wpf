using CIU_WPF.Common.CanvasExt;
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
using System.Windows.Shapes;

namespace CIU_WPF.AppLayer.Dialogs
{
    /// <summary>
    /// Interaction logic for ResizeSettingWindow.xaml
    /// </summary>
    public partial class ResizeSettingWindow : Window
    {
        public class ResizeDataModel
        {
            public int NewWidth { get; set; }
            public int NewHeight { get; set; }
        }




        ResizeDataModel _ResizeDataModel = new ResizeDataModel();
        FileInfo _SourceFile;

        BitmapImage _originBitmap;
        public FileInfo SourceFile
        {
            get => _SourceFile;
            set
            {
                _SourceFile = value;
                _originBitmap = ImageIOUtils.CreateBitmapImageFromFile(value.FullName);
                ImagePreview.Source = _originBitmap;
                UpdateResizedImage();
                if (value != null)
                {
                    _ResizeDataModel = new ResizeDataModel
                    {
                        NewWidth = (int)_originBitmap.Width,
                        NewHeight = (int)_originBitmap.Height
                    };
                    DataContext = _ResizeDataModel;
                }
                InvalidateVisual();
            }
        }

        private void UpdateResizedImage()
        {
            ImageSourceConverter c = new ImageSourceConverter();
            previewLayer_resizedImage.image = _originBitmap.CloneCurrentValue();
        }

        DrawingLayerImageSource previewLayer_resizedImage = new DrawingLayerImageSource(null);
        public ResizeSettingWindow()
        {
            InitializeComponent();

            ImagePreview.AddLayer(previewLayer_resizedImage);
            ImagePreview.onViewportOffsetChanged = onViewportOffsetChangedEvent;
        }

        private void onViewportOffsetChangedEvent(object? sender, EventArgs e)
        {
            int xc = ImagePreview.GetClientWidth() / 2;
            int yc = ImagePreview.GetClientHeight() / 2;

            int left = xc - (int)((ImagePreview.Source.Width * ImagePreview.ZoomRatio) / 2 + ImagePreview.ViewportOffset.X);
            int top = yc - (int)((ImagePreview.Source.Height * ImagePreview.ZoomRatio) / 2 + ImagePreview.ViewportOffset.Y);

            previewLayer_resizedImage.Offset = new Point(
                ImagePreview.ViewportOffset.X,
                ImagePreview.ViewportOffset.Y
                );
        }

        public void Exit(object sender, EventArgs e)
        {
            Close();
        }

        public void Save(object sender, EventArgs e)
        {
            string fileNameNoExt = _SourceFile.Name.Substring(0, _SourceFile.Name.LastIndexOf("."));
            string newFileName = $"{fileNameNoExt}_{_ResizeDataModel.NewWidth}x{_ResizeDataModel.NewHeight}";
            string newFilePath = System.IO.Path.Combine(_SourceFile.DirectoryName, newFileName + _SourceFile.Extension);
            int autonum = 1;
            while(File.Exists(newFilePath))
            {
                newFilePath = System.IO.Path.Combine(_SourceFile.DirectoryName, newFileName + "_" + autonum++ + _SourceFile.Extension);
            }
            BitmapFrame output = ImageTransformUtils.CreateResizedImage(_originBitmap, _ResizeDataModel.NewWidth, _ResizeDataModel.NewHeight, 0);
            ImageIOUtils.Save(output, newFilePath);
            MessageBox.Show("Saved as: " + newFilePath, "Image resized successful");
        }

        void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            object oldDataContext = e.OldValue;
        }
    }
}
