using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIU_WPF.Common
{
    public class FileUtils
    {
        static string[] IMAGE_FILE_EXTS = new string[] { ".bmp", ".png", ".gif" };
        public static bool IsImageFilePath(string path)
        {
            try
            {
                FileInfo fi = new FileInfo(path);
                string ext = fi.Extension.ToLower();
                return IMAGE_FILE_EXTS.Contains(ext);
            }
            catch
            {
                return false;
            }
        }
    }
}
