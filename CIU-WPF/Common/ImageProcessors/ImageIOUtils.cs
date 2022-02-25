using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CIU_WPF.Common.ImageProcessors
{
    internal class ImageIOUtils
    {
        public static void Save(BitmapFrame frame, string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            bool isPng = fi.Extension.ToLower() == ".png";

            BitmapEncoder enc = null;
            using (FileStream fs = File.Create(filePath))
            {
                if (isPng)
                    enc = new PngBitmapEncoder();
                else
                    enc = new JpegBitmapEncoder();


                enc.Frames.Add(BitmapFrame.Create(frame));
                enc.Save(fs);
            }
        }


        public static BitmapImage CreateBitmapImageFromData(byte[] data)
        {
            using (var ms = new System.IO.MemoryStream(data))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
        public static BitmapImage CreateBitmapImageFromFile(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = fs;
                image.EndInit();
                return image;
            }
        }
    }
}
