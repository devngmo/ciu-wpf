using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CIU_WPF.AppLayer
{
    internal class ThumbnailCacheStorage
    {
        public class ThumbnailCacheStorageSettings
        {
            public string CacheFolder { get; set; }
        }
        private static ThumbnailCacheStorage s_ins = new ThumbnailCacheStorage();
        public static ThumbnailCacheStorage Ins => s_ins;

        ThumbnailCacheStorageSettings _settings;

        Dictionary<string, string> fileMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public void Init(ThumbnailCacheStorageSettings settings)
        {
            _settings = settings;
            if (!Directory.Exists(_settings.CacheFolder))
                Directory.CreateDirectory(_settings.CacheFolder);   
        }

        public void Load()
        {
            loadFileMap();
        }

        private void loadFileMap()
        {
            fileMap.Clear();
            string pathFileMap = Path.Combine(_settings.CacheFolder, "filemap.txt");
            if (!File.Exists(pathFileMap))
                return;

            string[] lines = File.ReadAllLines(pathFileMap);
            foreach (string line in lines)
            {
                try
                {
                    int idx = line.IndexOf(":");
                    if (idx >= 32)
                    {
                        string fileID = line.Substring(0, idx);
                        string filePath = line.Substring(idx + 1);
                        fileMap.Add(filePath, fileID);
                    }
                }
                catch { }
            }
        }

        public string save(SKBitmap bmp, string filePath, int quality)
        {
            string fileID = Guid.NewGuid().ToString("N");
            string thumbnailFilePath = Path.Combine(_settings.CacheFolder, fileID);
            using (var image = SKImage.FromBitmap(bmp))
            {
                using (var output =
                       File.OpenWrite(thumbnailFilePath))
                {
                    image.Encode(SKEncodedImageFormat.Png, quality)
                        .SaveTo(output);

                    fileMap.Add(filePath, fileID);
                }
            }
            return fileID;
        }

        public bool hasThumbnailOf(string filePath)
        {
            if (fileMap.ContainsKey(filePath))
            {
                string fileID = fileMap[filePath];
                string thumbnailFilePath = Path.Combine(_settings.CacheFolder, fileID);
                return File.Exists(thumbnailFilePath);
            }
            return false;
        }

        internal string? getThumbnailID(string originFilePath)
        {
            if (fileMap.ContainsKey(originFilePath))
                return fileMap[originFilePath];
            return null;
        }

        public BitmapImage? loadAsBitmapImage(string filePath)
        {
            if (fileMap.ContainsKey(filePath))
            {
                string fileID = fileMap[filePath];
                string thumbnailFilePath = Path.Combine(_settings.CacheFolder, fileID);
                if (File.Exists(thumbnailFilePath))
                    return new BitmapImage(new Uri(thumbnailFilePath));
            }
            return null;
        }
    }
}
