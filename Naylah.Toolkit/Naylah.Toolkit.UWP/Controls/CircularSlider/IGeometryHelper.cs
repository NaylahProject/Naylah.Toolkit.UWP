using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Naylah.Toolkit.UWP.Controls.CircularSlider
{
    public interface IGeometryHelper
    {
        Path GetCircleSegment(Point centrePoint, Point currentPoint, double radius, double stripWidth, double angle);
        void SetColours(SolidColorBrush strokeBrush);
    }

    public class GeometryHelper : IGeometryHelper
    {
        private readonly IMathHelper _mathHelper = new MathHelper();
        private SolidColorBrush _strokeBrush;

        public void SetColours(SolidColorBrush strokeBrush)
        {
            _strokeBrush = strokeBrush;
        }

        public Path GetCircleSegment(Point centrePoint, Point currentPoint, double radius, double stripWidth, double angle)
        {
            var path = new Path { Stroke = _strokeBrush };

            var pathGeometry = new PathGeometry();

            var circleStart = new Point(centrePoint.X, centrePoint.Y - radius);

            // Arc
            var arcSegment = new ArcSegment
            {
                IsLargeArc = angle > 180.0,
                Point = _mathHelper.ScaleUnitCirclePoint(centrePoint, angle, radius),
                Size = new Size(radius, radius),
                SweepDirection = SweepDirection.Clockwise
            };

            //The path figure includes first, the first line from the centre to line1End, then the arc
            var pathFigure = new PathFigure { StartPoint = circleStart, IsClosed = false };

            pathFigure.Segments.Add(arcSegment);
            pathGeometry.Figures.Add(pathFigure);

            path.Data = pathGeometry;
            path.StrokeThickness = stripWidth;

            return path;
        }
    }
}
