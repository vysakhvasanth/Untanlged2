using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CardControlLibrary
{
    public class AwesomeProgressBar : Shape
    {

        static AwesomeProgressBar()
        {
            Brush Brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#373542"));
            Brush.Freeze();

            StrokeProperty.OverrideMetadata(
                typeof(AwesomeProgressBar),
                new FrameworkPropertyMetadata(Brush));
            FillProperty.OverrideMetadata(
                typeof(AwesomeProgressBar),
                new FrameworkPropertyMetadata(Brushes.Transparent));

            StrokeThicknessProperty.OverrideMetadata(
           typeof(AwesomeProgressBar),
           new FrameworkPropertyMetadata(5.0));
        }


        // Value (0-100)
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // DependencyProperty - Value (0 - 100)
        private static readonly FrameworkPropertyMetadata ValueMetadata =
                new FrameworkPropertyMetadata(
                    0.0,     // Default value
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    null,    // Property changed callback
                    new CoerceValueCallback(CoerceValue));   // Coerce value callback

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(AwesomeProgressBar), ValueMetadata);

        private static object CoerceValue(DependencyObject depObj, object baseVal)
        {
            var val = (double)baseVal;
            val = Math.Min(val, 99.999);
            val = Math.Max(val, 0.0);
            return val;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                double startAngle = 90.0;
                double endAngle = 90.0 - ((Value / 100.0) * 360.0);

                double maxWidth = Math.Max(0.0, RenderSize.Width - StrokeThickness);
                double maxHeight = Math.Max(0.0, RenderSize.Height - StrokeThickness);

                double xStart = maxWidth / 2.0 * Math.Cos(startAngle * Math.PI / 180.0);
                double yStart = maxHeight / 2.0 * Math.Sin(startAngle * Math.PI / 180.0);

                double xEnd = maxWidth / 2.0 * Math.Cos(endAngle * Math.PI / 180.0);
                double yEnd = maxHeight / 2.0 * Math.Sin(endAngle * Math.PI / 180.0);

                var geom = new StreamGeometry();
                using (StreamGeometryContext ctx = geom.Open())
                {
                    ctx.BeginFigure(
                        new Point((RenderSize.Width / 2.0) + xStart,
                                  (RenderSize.Height / 2.0) - yStart),
                        true,   // Filled
                        false);  // Closed
                    ctx.ArcTo(
                        new Point((RenderSize.Width / 2.0) + xEnd,
                                  (RenderSize.Height / 2.0) - yEnd),
                        new Size(maxWidth / 2.0, maxHeight / 2),
                        0.0,     // rotationAngle
                        (startAngle - endAngle) > 180,   // greater than 180 deg?
                        SweepDirection.Clockwise,
                        true,    // isStroked
                        false);
                    // ctx.LineTo(new Point((RenderSize.Width / 2.0), (RenderSize.Height / 2.0)), true, false);
                }

                return geom;
            }

        }

    }
}
