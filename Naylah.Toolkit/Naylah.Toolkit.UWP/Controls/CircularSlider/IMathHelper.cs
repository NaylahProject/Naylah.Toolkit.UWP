using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Naylah.Toolkit.UWP.Controls.CircularSlider
{
    public interface IMathHelper
    {
        double AngleBetweenLine(Point centrePoint, Point line1End, Point line2End, double radius);
        Point ScaleUnitCirclePoint(Point origin, double angle, double radius);
    }

    public class MathHelper : IMathHelper
    {
        private const double Radians = Math.PI / 180;

        public Point ScaleUnitCirclePoint(Point origin, double angle, double radius)
        {
            return new Point(origin.X + Math.Sin(Radians * angle) * radius, origin.Y - Math.Cos(Radians * angle) * radius);
        }

        private double GetDistanceBetweenPoints(Point start, Point end)
        {
            var ret = Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));

            return ret;
        }

        public double AngleBetweenLine(Point centrePoint, Point line1End, Point line2End, double radius)
        {
            var a = GetDistanceBetweenPoints(centrePoint, line1End);
            var b = GetDistanceBetweenPoints(centrePoint, line2End);

            var c = GetDistanceBetweenPoints(line1End, line2End);

            double nom = (a * a) + (b * b) - (c * c);
            double denom = 2 * a * b;

            var radians = Math.Acos(nom / denom);
            var ret = radians * (180.0 / Math.PI);

            var z = IsAngleGreaterThan180(centrePoint, line1End, line2End, radians);

            if (z > 0)
                return ret;

            var retVal = (360.0 - ret);
            if (retVal >= 360)
                retVal = 0.0;

            return retVal;

        }

        /// <summary>
        ///  (v1 x v2).z = ||v1|| * ||v2|| * sin(a)
        /// </summary>
        /// <param name="centrePoint"></param>
        /// <param name="line1End"></param>
        /// <param name="line2End"></param>
        /// <param name="angleRadians"></param>
        /// <returns></returns>
        private double IsAngleGreaterThan180(Point centrePoint, Point line1End, Point line2End, double angleRadians)
        {
            var v1 = new Point(line1End.X - centrePoint.X, line1End.Y - centrePoint.Y);
            var v2 = new Point(line2End.X - centrePoint.X, line2End.Y - centrePoint.Y);

            var v1Length = Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y);
            var v2Length = Math.Sqrt(v2.X * v2.X + v2.Y * v2.Y);

            var nom2 = v1Length * v2Length * Math.Sin(angleRadians);

            //Cross Product
            var crossProd = (v1.X * v2.Y) - (v2.X * v1.Y);

            return nom2 / crossProd;
        }
    }
}
