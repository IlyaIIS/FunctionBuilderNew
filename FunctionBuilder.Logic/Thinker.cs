using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FunctionBuilder
{
    public static class Thinker
    {
        public static List<DoublePoint> GetPointsList(Rpn rpn, double xStart, double xEnd, double step)
        {
            var output = new List<DoublePoint>();

            Rpn localRpn;
            double x = xStart;
            do
            {
                localRpn = new Rpn(rpn);
                localRpn.SetVariable(x);
                double y = localRpn.Calculate();

                output.Add(new DoublePoint(x, y));

                x += step;
            } while (x <= xEnd);

            return output;
        }

        public static List<DoublePoint> GetPointsListOld(Rpn rpn, double xStart, double xEnd, double minY, double maxY, double step, DoublePoint offset, double zoom = 1)
        {
            if (Double.IsNaN(step)) step = (xEnd - xStart) / 339;

            var output = new List<DoublePoint>();

            Rpn localRpn;
            double x = xStart - offset.X - step;
            bool repeat = true;
            do
            {
                x += step;

                if (x >= xEnd - offset.X)
                {
                    repeat = false;
                    x = xEnd - offset.X;
                }

                localRpn = new Rpn(rpn);
                localRpn.SetVariable(x * zoom);
                double y = localRpn.Calculate() / zoom - offset.Y;

                if (y > maxY) y = maxY;
                if (y < minY) y = minY;

                output.Add(new DoublePoint(x + offset.X, y ));
            } while (repeat);

            for (int i = 1; i < output.Count-1; i++)
            {
                if (output[i].Y == maxY)
                {
                    if (output[i - 1].Y == maxY && output[i + 1].Y == maxY)
                    {
                        output.RemoveAt(i);
                        i--;
                    }
                }
                else if (output[i].Y == minY)
                {
                    if (output[i - 1].Y == minY && output[i + 1].Y == minY)
                    {
                        output.RemoveAt(i);
                        i--;
                    }
                }
            }

            return output;
        }
    }

    public struct DoublePoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public DoublePoint(double x, double y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return X.ToString() + " " + Y.ToString();
        }
    }
}
