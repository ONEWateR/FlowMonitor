using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace onewater.flowmonitor.res.events
{

    public class ImageButton : Button
    {
        public static readonly DependencyProperty MyMoverBrushProperty;
        public static readonly DependencyProperty MyEnterBrushProperty;
        public Brush MyMoverBrush
        {
            get
            {
                return base.GetValue(ImageButton.MyMoverBrushProperty) as Brush;
            }
            set
            {
                base.SetValue(ImageButton.MyMoverBrushProperty, value);
            }
        }
        public Brush MyEnterBrush
        {
            get
            {
                return base.GetValue(ImageButton.MyEnterBrushProperty) as Brush;
            }
            set
            {
                base.SetValue(ImageButton.MyEnterBrushProperty, value);
            }
        }
        static ImageButton()
        {
            ImageButton.MyMoverBrushProperty = DependencyProperty.Register("MyMoverBrush", typeof(Brush), typeof(ImageButton), new PropertyMetadata(null));
            ImageButton.MyEnterBrushProperty = DependencyProperty.Register("MyEnterBrush", typeof(Brush), typeof(ImageButton), new PropertyMetadata(null));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }
        public ImageButton()
        {
            base.Content = "";
        }
    }
}
