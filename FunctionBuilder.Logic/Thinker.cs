using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionBuilder
{
    static public class Thinker
    {
        static public List<DoublePoint> GetPointsList(Rpn rpn, double xStart, double xEnd, double minY, double maxY, DoublePoint offset, double zoom = 1)
        {
            double step = (xEnd - xStart) / 339; // Не присваивать числа вида 1/х !

            var output = new List<DoublePoint>();

            double x = xStart - offset.X;
            do
            {
                Rpn localRpn = new Rpn(rpn);
                localRpn.SetVariable(x * zoom);
                double y = localRpn.Calculate() / zoom - offset.Y;

                if (y > maxY ) y = maxY ;
                if (y < minY ) y = minY ;

                output.Add(new DoublePoint(x + offset.X, y ));

                x += step;
            } while (x <= xEnd - offset.X);

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
