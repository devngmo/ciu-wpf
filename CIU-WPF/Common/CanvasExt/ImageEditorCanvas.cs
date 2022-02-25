using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CIU_WPF.Common.CanvasExt
{
    public class ImageEditorCanvas : ImageViewCanvas
    {
        List<DrawingLayerInterface> layers = new List<DrawingLayerInterface>();

        int selectedLayerIndex = -1;
        public ImageEditorCanvas():base()
        {
            displaySourceImage = false; 
        }
        public void AddLayer(DrawingLayerInterface dv)
        {
            layers.Add(dv);
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            foreach (var layer in layers)
            {
                layer.Render(dc);
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Point curPos = e.GetPosition(this);
            int clickedLayerIndex = getLayerAt(curPos);
            deselect();
            if (clickedLayerIndex >= 0)
                selectLayer(clickedLayerIndex);
        }

        private void selectLayer(int index)
        {
            selectedLayerIndex = index;
            layers[selectedLayerIndex].Selected = false;
            beforeTranslateOffset = layers[selectedLayerIndex].Offset;
            InvalidateVisual();
        }

        private void deselect()
        {
            if (selectedLayerIndex >= 0)
                layers[selectedLayerIndex].Selected = false;
            selectedLayerIndex = -1;
        }

        private int getLayerAt(Point pos)
        {
            for (int i = layers.Count-1; i>=0; i--)
            {
                if (layers[i].ContainsPoint(pos)) return i;
            }
            return -1;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isMoveViewport && e.LeftButton == MouseButtonState.Pressed)
            {   
                Point curPos = e.GetPosition(this);
                if (selectedLayerIndex == -1)
                {
                    ViewportOffset = new Point(
                    beforeTranslateOffset.X - (curPos.X - mouseDownPos.X),
                    beforeTranslateOffset.Y - (curPos.Y - mouseDownPos.Y));
                }
                else
                {
                    DrawingLayerInterface selectedLayer = layers[selectedLayerIndex];
                    selectedLayer.Offset = new Point(
                    beforeTranslateOffset.X - (curPos.X - mouseDownPos.X),
                    beforeTranslateOffset.Y - (curPos.Y - mouseDownPos.Y));
                }
            }
            base.OnMouseMove(e);
        }

        protected override void DrawSnapLines(DrawingContext dc)
        {
            int xc = GetClientWidth() / 2;
            int yc = GetClientHeight() / 2;

            Pen pen = new Pen(new SolidColorBrush(Color.FromRgb(32, 32, 32)), 1);
            pen.DashStyle = DashStyles.Dash;
            SolidColorBrush snapLabelBrush = new SolidColorBrush(Color.FromRgb(128, 128, 128));

            dc.DrawLine(pen, new Point((int)(-ViewportOffset.X), 0), new Point((int)(- ViewportOffset.X), GetClientHeight()));
            DrawText(dc, "Y", new Point(5-(int)ViewportOffset.X, GetClientHeight()-15), 14, snapLabelBrush);

            dc.DrawLine(pen, new Point(0, (int)(-ViewportOffset.Y)), new Point(GetClientWidth(), (int)(- ViewportOffset.Y)));
            DrawText(dc, "X", new Point(GetClientWidth()-20, -20-(int)ViewportOffset.Y), 14, snapLabelBrush);
            if (selectedLayerIndex != -1)
            {
                DrawingLayerInterface layer = layers[selectedLayerIndex];
                dc.DrawLine(pen, new Point((int)(layer.Width-ViewportOffset.X), 0), new Point((int)(layer.Width- ViewportOffset.X), GetClientHeight()));
                dc.DrawLine(pen, new Point(0, (int)(layer.Height-ViewportOffset.Y)), new Point(GetClientWidth(), (int)(layer.Height - ViewportOffset.Y)));
            }
        }
    }
}
