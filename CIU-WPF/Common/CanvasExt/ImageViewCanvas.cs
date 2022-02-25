using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CIU_WPF.Common.CanvasExt
{
    public class ImageViewCanvas : BaseDrawingCanvas
    {
        public int SnapSpacing
        {
            get => (int)GetValue(SnapSpacingProperty);
            set
            {
                SetValue(SnapSpacingProperty, value);
                InvalidateVisual();
            }
        }
        public bool UseBackgroundChecker
        {
            get => (bool)GetValue(UseBackgroundCheckerProperty);
            set
            {
                SetValue(UseBackgroundCheckerProperty, value);
                if (value)
                    Background = CreateBackgroundChecker(32);
                else
                    Background = new SolidColorBrush(Color.FromRgb(0,0,0));
                InvalidateVisual();
            }
        }

        private Brush CreateBackgroundChecker(int tileSize)
        {
            GeometryDrawing gd = new GeometryDrawing();
            gd.Geometry = Geometry.Parse("M0, 0 H1 V1 H2 V2 H1 V1 H0Z");
            gd.Brush = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            DrawingBrush db = new DrawingBrush(gd);
            db.TileMode = TileMode.Tile;
            db.Viewport = new Rect(0, 0, tileSize, tileSize);
            db.ViewportUnits = BrushMappingMode.Absolute;
            return db;
        }

        public EventHandler onViewportOffsetChanged;
        public Point ViewportOffset
        {
            get => (Point)GetValue(ViewportOffsetProperty);
            set
            {
                SetValue(ViewportOffsetProperty, value);
                if (onViewportOffsetChanged != null)
                    onViewportOffsetChanged.Invoke(this, new EventArgs());
                InvalidateVisual();
            }
        }

        public bool ShowSnapLines
        {
            get => (bool)GetValue(ShowSnapLinesProperty);
            set => SetValue(ShowSnapLinesProperty, value);
        }

        public bool ScrollToZoom
        {
            get => (bool)GetValue(ScrollToZoomProperty);
            set => SetValue(ScrollToZoomProperty, value);
        }
        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set
            {
                if (value != null)
                {
                    int minHalf = (int)value.Width / 2;
                    if (value.Width > value.Height)
                        minHalf = (int)value.Height/ 2;

                    if (SnapSpacing > minHalf)
                    {
                        if (minHalf < 16) SnapSpacing = 0;
                        else if (minHalf < 32) SnapSpacing = 16;
                        else if (minHalf < 64) SnapSpacing = 32;
                        else if (minHalf < 128) SnapSpacing = 64;
                        else if (minHalf < 256) SnapSpacing = 128;
                        else if (minHalf < 512) SnapSpacing = 256;
                        else SnapSpacing = 512;
                    }
                }
                SetValue(SourceProperty, value);
            }
        }

        public float ZoomRatio
        {
            get => (float)GetValue(ZoomRatioProperty);
            set => SetValue(ZoomRatioProperty, value);
        }

        public bool AllowToMoveViewport
        {
            get => (bool)GetValue(AllowToMoveViewportProperty);
            set => SetValue(AllowToMoveViewportProperty, value);
        }
        public static readonly DependencyProperty AllowToMoveViewportProperty = DependencyProperty.Register("AllowToMoveViewport",
            typeof(bool), typeof(ImageViewCanvas), new FrameworkPropertyMetadata(default(bool)));


        public static readonly DependencyProperty ViewportOffsetProperty = DependencyProperty.Register("ViewportOffset",
            typeof(Point), typeof(ImageViewCanvas), new FrameworkPropertyMetadata(default(Point)));

        public static readonly DependencyProperty SnapSpacingProperty = DependencyProperty.Register("SnapSpacing",
            typeof(int), typeof(ImageViewCanvas), new FrameworkPropertyMetadata(default(int)));

        public static readonly DependencyProperty ZoomRatioProperty = DependencyProperty.Register("ZoomRatio",
            typeof(float), typeof(ImageViewCanvas), new FrameworkPropertyMetadata(default(float)));

        public static readonly DependencyProperty ShowSnapLinesProperty = DependencyProperty.Register("ShowSnapLines",
            typeof(bool), typeof(ImageViewCanvas), new FrameworkPropertyMetadata(default(bool)));
        
        public static readonly DependencyProperty ScrollToZoomProperty = DependencyProperty.Register("ScrollToZoom",
            typeof(bool), typeof(ImageViewCanvas), new FrameworkPropertyMetadata(default(bool)));

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source",
            typeof(ImageSource), typeof(ImageViewCanvas), new FrameworkPropertyMetadata(default(ImageSource)));

        public static readonly DependencyProperty UseBackgroundCheckerProperty = DependencyProperty.Register("UseBackgroundChecker",
            typeof(bool), typeof(ImageViewCanvas), new FrameworkPropertyMetadata(default(bool)));


        protected bool displaySourceImage = true;
        public ImageViewCanvas():base()
        {
            ShowSnapLines = false;
            SnapSpacing = 128;
            ZoomRatio = 1;
            UseBackgroundChecker = true;
        }
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (Source == null) return;

            if (displaySourceImage)
                drawSourceImage(dc);
            
            if (ShowSnapLines)
                DrawSnapLines(dc);
        }

        protected virtual void drawSourceImage(DrawingContext dc)
        {
            int xc = GetClientWidth() / 2;
            int yc = GetClientHeight() / 2;

            int left = xc - (int)((Source.Width * ZoomRatio) / 2 + ViewportOffset.X);
            int top = yc - (int)((Source.Height * ZoomRatio) / 2 + ViewportOffset.Y);
            //dc.PushTransform(new ScaleTransform(ZoomRatio, ZoomRatio));
            dc.DrawImage(Source, new Rect(left, top, Source.Width * ZoomRatio, Source.Height * ZoomRatio));
        }

        protected virtual void DrawSnapLines(DrawingContext dc)
        {
            int xc = GetClientWidth() / 2;
            int yc = GetClientHeight() / 2;

            int left = xc - (int)((Source.Width * ZoomRatio) / 2 + ViewportOffset.X);
            int top = yc - (int)((Source.Height * ZoomRatio) / 2 + ViewportOffset.Y);
            int right = left + (int)(Source.Width * ZoomRatio);
            int bottom = top + (int)(Source.Height * ZoomRatio);


            Pen pen = new Pen(new SolidColorBrush(Color.FromRgb(32,32,32)), 1);
            pen.DashStyle = DashStyles.Dash;
            int col = 0;
            SolidColorBrush snapLabelBrush = new SolidColorBrush(Color.FromRgb(128,128,128));

            while (col < Source.Width)
            {
                int x = left + (int)(col * ZoomRatio);
                dc.DrawLine(pen, new Point(x, 0), new Point(x, GetClientHeight()));
                DrawText(dc, "" + col, new Point(x + 5, 5), 14, snapLabelBrush);
                col += SnapSpacing;
            }
            dc.DrawLine(pen, new Point(right, 0), new Point(right, GetClientHeight()));
            DrawText(dc, "" + right, new Point(right + 5, 5), 14, snapLabelBrush);

            int row = 0;
            while (row < Source.Width)
            {
                int y = top + (int)(row * ZoomRatio);
                dc.DrawLine(pen, new Point(0, y), new Point(GetClientWidth(), y));
                DrawText(dc, "" + row, new Point(5, y - 20), 14, snapLabelBrush);
                row += SnapSpacing;
            }
            dc.DrawLine(pen, new Point(0, bottom), new Point(GetClientWidth(), bottom));
            DrawText(dc, "" + bottom, new Point(5, bottom - 20), 14, snapLabelBrush);
        }

        protected void DrawText(DrawingContext dc, string text, Point at, int fontSize, Brush brush, string fontName = "Arial")
        {
            FormattedText xLabel = new FormattedText(text,
                    System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    new Typeface(fontName), fontSize, brush);
            dc.DrawText(xLabel, new Point(at.X, at.Y));
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!ScrollToZoom) return;

            float ZoomSmooth = 0.001f;
            ZoomRatio = Math.Clamp(ZoomRatio + e.Delta * ZoomSmooth, 0.1f, 100f);
            InvalidateVisual();
        }

        protected Point beforeTranslateOffset = new Point(0, 0);
        protected Point mouseDownPos = new Point();
        protected bool isMoveViewport = false;
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            beforeTranslateOffset = ViewportOffset;
            mouseDownPos = e.GetPosition(this);
            if (AllowToMoveViewport)
                isMoveViewport = true;
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isMoveViewport && e.LeftButton == MouseButtonState.Pressed)
            {
                int xc = GetClientWidth() / 2;
                int yc = GetClientHeight() / 2;

                int w2 = (int)((Source.Width * ZoomRatio) / 2);
                int h2 = (int)((Source.Height * ZoomRatio) / 2);
                int left = xc - (int)((Source.Width * ZoomRatio) / 2);
                int top = yc - (int)((Source.Height * ZoomRatio) / 2);
                int right = left + (int)(Source.Width * ZoomRatio);
                int bottom = top + (int)(Source.Height * ZoomRatio);
                Point vpMin = new Point(32-right, 32 - bottom);
                Point vpMax = new Point(xc + w2-32, yc + h2 - 32);
                Point curPos = e.GetPosition(this);
                ViewportOffset = new Point(
                    Math.Clamp(beforeTranslateOffset.X - (curPos.X - mouseDownPos.X), vpMin.X, vpMax.X),
                    Math.Clamp(beforeTranslateOffset.Y - (curPos.Y - mouseDownPos.Y), vpMin.Y, vpMax.Y));
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            isMoveViewport = false;
            base.OnMouseUp(e);
        }
    }
}
