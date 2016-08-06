using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace MapTest
{
    static class Geometry
    {


        public static bool isConvex(Geopath path)
        {
            var points = path.Positions;
            if (path.Positions.Count < 4)
                return true;
            bool sign = false;
            int n = path.Positions.Count;

            for (int i = 0; i < n; i++)
            {
                double dx1 = points[((i + 2) % n)].Longitude - points[(i + 1) % n].Longitude;
                double dy1 = points[(i + 2) % n].Latitude - points[(i + 1) % n].Latitude;
                double dx2 = points[i].Longitude - points[(i + 1) % n].Longitude;
                double dy2 = points[i].Latitude - points[(i + 1) % n].Latitude;
                double zcrossproduct = dx1 * dy2 - dy1 * dx2;
                if (i == 0)
                    sign = zcrossproduct > 0;
                else
                {
                    if (sign != (zcrossproduct > 0))
                        return false;
                }
            }
            return true;
        }

        private struct Line
        {
            public Point A;
            public Point B;
            //public double lengthX;
            //public double lengthY;
        }

        public struct Point
        {
            public double X;
            public double Y;
        }

        public static bool isSelfIntersecting(Geopath path)
        {

            var points = path.Positions;
            Line[] lines = new Line[path.Positions.Count - 1];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i].A.X = path.Positions[i].Longitude;
                lines[i].B.X = path.Positions[i + 1].Longitude;
                lines[i].A.Y = path.Positions[i].Latitude;
                lines[i].B.Y = path.Positions[i + 1].Latitude;
                //lines[i].lengthX = lines[i].B.X - lines[i].A.X;
                //lines[i].lengthY = lines[i].B.Y - lines[i].A.Y;
            }

            for (int o = 0; o < lines.Length; o++)
            {
                for (int t = o + 1; t < lines.Length; t++)
                {
                    var one = lines[o];
                    var two = lines[t];
                    double angle1 = ccw(one.A, one.B, two.A);
                    double angle2 = ccw(one.A, one.B, two.B);
                    double angle3 = ccw(two.A, two.B, one.A);
                    double angle4 = ccw(two.A, two.B, one.B);


                    if (angle1 * angle2 < 0)
                        if (angle3 * angle4 < 0)
                            return true;

                }
            }

            return false; 
        }

        public static double ccw(Point a, Point b, Point c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (c.X- a.X) * (b.Y - a.Y);
        }

        public static double CalculateArea(List<BasicGeoposition> locs)
        {
            double area = 0;
            for (var i = 0; i < locs.Count - 1; i++)
            {
                area += Math.Atan(
                    Math.Tan(Math.PI / 180 * (locs[i + 1].Longitude - locs[i].Longitude) / 2) *
                    Math.Sin(Math.PI / 180 * (locs[i + 1].Latitude + locs[i].Latitude) / 2) /
                    Math.Cos(Math.PI / 180 * (locs[i + 1].Latitude - locs[i].Latitude) / 2));
            }

            if (area < 0)
            {
                area *= -1;
            }

            return area * 2 * Math.Pow(6378137.0, 2);
        }
    }
}
