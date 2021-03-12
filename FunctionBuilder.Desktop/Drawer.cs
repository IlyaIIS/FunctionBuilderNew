using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionBuilder.Desktop
{
    static class Drawer
    {
        static public void DrawCanvas(MainWindow theMainWindow)
        {
            Window mainWindow = theMainWindow.Find<Window>("wTheMainWindow");
            var cChart = theMainWindow.Find<Canvas>("cChart");

            double height = mainWindow.Height - (87 + cChart.Margin.Top * 2 + 4);
            double width = mainWindow.Width - cChart.Margin.Right * 2;

            cChart.Children.Clear();

            AddArrow(0, height / 2, width, height / 2, ref cChart);
            AddArrow(width / 2, height, width / 2, 0, ref cChart);
        }

        static private void AddArrow(double x1, double y1, double x2, double y2, ref Canvas canvas)
        {
            double width = 5;
            double length = 15;

            double d = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

            double X = x2 - x1;
            double Y = y2 - y1;

            double X3 = x2 - (X / d) * length;
            double Y3 = y2 - (Y / d) * length;

            double Xp = y2 - y1;
            double Yp = x1 - x2;

            double X4 = X3 + (Xp / d) * width;
            double Y4 = Y3 + (Yp / d) * width;
            double X5 = X3 - (Xp / d) * width;
            double Y5 = Y3 - (Yp / d) * width;

            Line line = new Line
            {
                Stroke = Brushes.Black,
                StartPoint = new Point(x1, y1),
                EndPoint = new Point(x2, y2)
            };
            canvas.Children.Add(line);

            line = new Line
            {
                Stroke = Brushes.Black,
                StartPoint = new Point(x2 - (X / d), y2 - (Y / d)),
                EndPoint = new Point(X4, Y4)
            };
            canvas.Children.Add(line);

            line = new Line
            {
                Stroke = Brushes.Black,
                StartPoint = new Point(x2 - (X / d), y2 - (Y / d)),
                EndPoint = new Point(X5, Y5)
            };
            canvas.Children.Add(line);
        }
    }
}
