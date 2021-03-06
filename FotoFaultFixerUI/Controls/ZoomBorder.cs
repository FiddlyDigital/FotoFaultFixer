﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace FotoFaultFixerUI.Controls
{
    // PG: Source: https://stackoverflow.com/a/6782715
    public class ZoomBorder : Border
    {
        private const double _zoomFactor = 0.2;

        private UIElement _child = null;
        private Point _origin;
        private Point _start;

        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is ScaleTransform);
        }

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != this.Child)
                {
                    this.Initialize(value);
                }

                base.Child = value;
            }
        }

        public void Initialize(UIElement element)
        {
            this._child = element;
            if (_child != null)
            {
                TransformGroup group = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                group.Children.Add(st);

                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(tt);

                _child.RenderTransform = group;
                _child.RenderTransformOrigin = new Point(0.0, 0.0);

                this.MouseWheel += child_MouseWheel;
                this.MouseLeftButtonDown += child_MouseLeftButtonDown;
                this.MouseLeftButtonUp += child_MouseLeftButtonUp;
                this.MouseMove += child_MouseMove;
                this.PreviewMouseRightButtonDown += new MouseButtonEventHandler(child_PreviewMouseRightButtonDown);
            }
        }

        public void Reset()
        {
            if (_child != null)
            {
                // reset zoom
                var st = GetScaleTransform(_child);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                // reset pan
                var tt = GetTranslateTransform(_child);
                tt.X = 0.0;
                tt.Y = 0.0;
            }
        }

        public void ZoomIn()
        {
            if (_child != null)
            {
                var st = GetScaleTransform(_child);
                st.ScaleX += _zoomFactor;
                st.ScaleY += _zoomFactor;
            }
        }

        public void ZoomOut()
        {
            if (_child != null)
            {
                var st = GetScaleTransform(_child);
                st.ScaleX -= _zoomFactor;
                st.ScaleY -= _zoomFactor;
            }
        }

        #region Child Events

        private void child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (_child != null)
            {
                var st = GetScaleTransform(_child);
                var tt = GetTranslateTransform(_child);

                double zoom = e.Delta > 0 ? _zoomFactor : -_zoomFactor;
                if (!(e.Delta > 0) && (st.ScaleX < (_zoomFactor*2) || st.ScaleY < (_zoomFactor*2)))
                {
                    return;
                }

                Point relative = e.GetPosition(_child);
                double absoluteX = (relative.X * st.ScaleX + tt.X);
                double absoluteY = (relative.Y * st.ScaleY + tt.Y);

                st.ScaleX += zoom;
                st.ScaleY += zoom;

                tt.X = (absoluteX - relative.X * st.ScaleX);
                tt.Y = (absoluteY - relative.Y * st.ScaleY);
            }
        }

        private void child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (_child != null)
            {
                var tt = GetTranslateTransform(_child);
                _start = e.GetPosition(this);
                _origin = new Point(tt.X, tt.Y);

                this.Cursor = Cursors.Hand;
                _child.CaptureMouse();
            }
        }

        private void child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (_child != null)
            {
                _child.ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;
            }
        }

        void child_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            this.Reset();
        }

        private void child_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            if (_child != null)
            {
                if (_child.IsMouseCaptured)
                {
                    var tt = GetTranslateTransform(_child);
                    Vector v = _start - e.GetPosition(this);
                    tt.X = _origin.X - v.X;
                    tt.Y = _origin.Y - v.Y;
                }
            }
        }

        #endregion
    }
}
