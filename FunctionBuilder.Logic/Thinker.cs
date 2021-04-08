using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionBuilder
{
    static public class Thinker
    {
        static public List<DoublePoint> GetPointsList(string expression, double xStart, double xEnd, double minY, double maxY, DoublePoint offset, double zoom = 1)
        {
            double step = (xEnd - xStart) / 339; // Не присваивать числа вида 1/х !

            List<string> rpn = OPZ.GetRPN(expression);
            var output = new List<DoublePoint>();

            double x = xStart - offset.X;
            do
            {
                var rpnList = new List<string>(rpn);
                for (int i = 0; i < rpnList.Count; i++)
                    if (rpnList[i] == "x")
                        rpnList[i] = (x*zoom).ToString();
                double y = OPZ.Calculate(rpnList) / zoom - offset.Y;

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
