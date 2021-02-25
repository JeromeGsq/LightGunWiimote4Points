using System;
using System.Collections.Generic;
using System.Linq;
using WiimoteLib;
using LightGunWiimote4Points.Models;

namespace LightGunWiimote4Points.Utils
{
    public static class MathUtils
    {
        public static double GetDistance(Position p1, Position p2)
        {
            return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
        }

        public static Position[] Sort(PointF[] rect)
        {
            Position topLeft = new Position(0, 0);
            Position topRight = new Position(0, 0);
            Position bottomLeft = new Position(0, 0);
            Position bottomRight = new Position(0, 0);

            var leftMost = new Point[2];
            var rightMost = new Point[2];

            // Insert 4 corners into array
            List<Position> points = new List<Position>();
            for (int i = 0; i <= 3; i++)
            {
                points.Add(new Position(rect[i].X, rect[i].Y));
            }

            points = points.OrderBy(w => w.X).ToList();

            if (points[0].Y < points[1].Y)
            {
                topLeft = points[0];
                bottomLeft = points[1];
            }
            else
            {
                topLeft = points[1];
                bottomLeft = points[0];
            }

            if (points[2].Y < points[3].Y)
            {
                topRight = points[2];
                bottomRight = points[3];
            }
            else
            {
                topRight = points[3];
                bottomRight = points[2];
            }

            return new Position[4] { topLeft, topRight, bottomRight, bottomLeft };
        }
    }

}
