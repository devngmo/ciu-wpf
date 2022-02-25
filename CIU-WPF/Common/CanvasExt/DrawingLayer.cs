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
    public interface DrawingLayerInterface
    {
        public Point Offset { get; set; }
        bool Selected { get; set; }

        public int Width { get; }
        public int Height { get; }

        void Render(DrawingContext dc);
        bool ContainsPoint(Point pos);
    }
    public class DrawingLayerImageSource : DrawingLayerInterface
    {
        Point _Offset = new Point(0, 0);
        public ImageSource image;

        public Point Offset { get => _Offset; set => _Offset = value; }
        bool _Selected;
        public bool Selected { get => _Selected; set => _Selected = value; }

        public int Width
        {
            get
            {
                if (image == null) return 0;
                return (int)image.Width;
            }
        }

        public int Height
        {
            get
            {
                if (image == null) return 0;
                return (int)image.Height;
            }
        }

        public DrawingLayerImageSource(ImageSource source)
        {
            image = source;
        }

        public void Render(DrawingContext dc)
        {
            if (image == null) return;
            dc.DrawImage(image, new Rect(-Offset.X, -Offset.Y, image.Width, image.Height));
        }

        public bool ContainsPoint(Point pos)
        {
            //TODO: check transparency pixel
            if (image == null) return false;
            return Offset.X <= pos.X && Offset.X + image.Width >= pos.X &&
                Offset.Y <= pos.Y && Offset.Y + image.Height >= pos.Y;
        }
    }
}
