using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CIU_WPF.Common.CanvasExt
{
    public class BaseDrawingCanvas : Canvas
    {
        public BaseDrawingCanvas() : base()
        {
            ClipToBounds = true;
        }

        public int GetClientWidth()
        {
            if (Width is double.NaN) return (int)ActualWidth;
            return (int)Width;
        }

        public int GetClientHeight()
        {
            if (Height is double.NaN) return (int)ActualHeight;
            return (int)Height;
        }
    }
}
