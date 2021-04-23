using System;
using System.Collections.Generic;

namespace FunctionBuilder
{
    public static class Thinker
    {
        public static List<DoublePoint> GetPointsList(Rpn rpn, double xStart, double xEnd, double step, double zoom)
        {
            var output = new List<DoublePoint>();

            Rpn localRpn;
            double x = xStart;
            do
            {
                double y = rpn.GetNewRpnWithSetVariable(x * zoom).Calculate() / zoom;

                output.Add(new DoublePoint(x, y));

                x += step;
            } while (x <= xEnd);

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
