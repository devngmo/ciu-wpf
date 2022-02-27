using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIU_WPF.AppLayer
{
    internal class ThumbnailCacheManager
    {
        public class ThumbnailCacheManagerSettings
        {
            public string CacheFolder { get; set; }
        }

        private ThumbnailCacheManagerSettings _settings;

        private static ThumbnailCacheManager s_ins = new ThumbnailCacheManager();
        public static ThumbnailCacheManager Ins => s_ins;

        public class CreateThumbnailArgs : EventArgs
        {
            public string originFilePath { get; set; }
            public string thumbnailID { get; set; }
        }
        public EventHandler<CreateThumbnailArgs> ThumbnailCreatedEventHandler;
        internal string createThumbnail(string filePath, int maxWidth, int maxHeight)
        {
            const int quality = 75;

            using (var input = File.OpenRead(filePath))
            {
                using (var inputStream = new SKManagedStream(input))
                {
                    using (var original = SKBitmap.Decode(inputStream))
                    {
                        float dx = maxWidth * 1.0f / original.Width;
                        float dy = maxHeight * 1.0f / original.Height;
                        float d = dx;
                        if (d > dy) d = dy;

                        int width = (int)(original.Width * d);
                        int height = (int)(original.Height * d);

                        using (var resized = original
                               .Resize(new SKImageInfo(width, height), SKBitmapResizeMethod.Lanczos3))
                        {
                            if (resized == null) return null;
                            return ThumbnailCacheStorage.Ins.save(resized, filePath, 75);
                        }
                    }
                }
            }
        }

        public void Init(ThumbnailCacheManagerSettings settings)
        {
            _settings = settings;
            ThumbnailCacheStorage.Ins.Init(new ThumbnailCacheStorage.ThumbnailCacheStorageSettings { 
                CacheFolder = settings.CacheFolder
            });
        }

        internal void CreateThumbnailsFromFiles(List<string> imageFiles, int maxWidth, int maxHeight)
        {
            foreach (var imageFile in imageFiles)
            {
                string thumbnailID = ThumbnailCacheStorage.Ins.getThumbnailID(imageFile);
                if (thumbnailID == null)
                    thumbnailID = createThumbnail(imageFile, maxWidth, maxHeight);

                if (ThumbnailCreatedEventHandler  != null)
                    ThumbnailCreatedEventHandler.Invoke(this, new CreateThumbnailArgs { 
                        originFilePath = imageFile,
                        thumbnailID = thumbnailID
                    });
            }
        }

        internal void CreateThumbnailsFromFolder(string curFolderPath)
        {

        }
    }
}
