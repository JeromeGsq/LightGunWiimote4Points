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

        public static Position[] Sort(List<Position> points)
        {
            Position top = new Position(0, 0);
            Position bottom = new Position(0, 0);
            Position left = new Position(0, 0);
            Position right = new Position(0, 0);

            if (points.Count >= 3)
            {
                points = points.OrderBy(w => w.X).ToList();
                left = points.First();
                right = points.Last();

                points = points.OrderBy(w => w.Y).ToList();
                top = points.First();
                bottom = points.Last();

                if (points.Count == 3)
                {
                    if (left.X == top.X || left.X == bottom.X)
                    {
                        left = new Position(top.X - (right.X - top.X), right.Y);
                    }

                    if (right.X == top.X || right.X == bottom.X)
                    {
                        right = new Position(top.X - (left.X - top.X), left.Y);
                    }
                }
            }

            return new Position[4] { top, bottom, left, right };
        }

        public static Position[] Sort(IRSensor[] rect)
        {
            Position top = new Position(0, 0);
            Position bottom = new Position(0, 0);
            Position left = new Position(0, 0);
            Position right = new Position(0, 0);

            List<Position> points = new List<Position>();
            for (int i = 0; i < rect.Length; i++)
            {
                if (rect[i].Found)
                {
                    points.Add(new Position(rect[i].Position.X, rect[i].Position.Y));
                }
            }

            if (points.Count >= 3)
            {
                points = points.OrderBy(w => w.X).ToList();
                left = points.First();
                right = points.Last();

                points = points.OrderBy(w => w.Y).ToList();
                top = points.First();
                bottom = points.Last();

                if (points.Count == 3)
                {
                    if (left.X == top.X || left.X == bottom.X)
                    {
                        left = new Position(top.X - (right.X - top.X), right.Y);
                    }

                    if (right.X == top.X || right.X == bottom.X)
                    {
                        right = new Position(top.X - (left.X - top.X), left.Y);
                    }
                }
            }

            return new Position[4] { top, bottom, left, right };
        }
    }

}
