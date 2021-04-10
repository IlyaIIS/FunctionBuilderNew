using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionBuilder.Desktop
{
    static class Drawer
    {
        static public double GraphWidth { get; private set; }
        static public double GraphHeight { get; private set; }
        static public Canvas GraphCanvas { get; private set; }
        static public Window TheMainWindow { get; private set; }
        static public Point Offset { get; private set; }
        static private Point mousePastyPos;
        static private bool mousePressed = false;
        static public double Zoom { get; private set; } = 1;
        static public string Expression { get; set; }
        static public void SetControls(Window window)
        {
            TheMainWindow = window;
            GraphCanvas = window.Find<Canvas>("cGraphCanvas");
            GraphCanvas.PointerPressed += GraphCanvas_PointerPressed;
            GraphCanvas.PointerMoved += GraphCanvas_PointerMovedDrag;
            GraphCanvas.PointerReleased += GraphCanvas_PointerReleased;
            GraphCanvas.PointerWheelChanged += GraphCanvas_PointerWheelChanged;
            GraphCanvas.PointerMoved += GraphCanvas_PointerMoved;
        }

        private static void GraphCanvas_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (!mousePressed && Expression != null)
            {
                TextBlock tbXCoord = TheMainWindow.FindControl<TextBlock>("tbXCoord");
                TextBlock tbYCoord = TheMainWindow.FindControl<TextBlock>("tbYCoord");
                var canvas = (Canvas)sender;

                double x = (e.GetPosition(canvas).X - GraphWidth / 2 - Offset.X)*Zoom;
                var rpnList = new List<string>(OPZ.GetRPN(Expression));
                for (int i = 0; i < rpnList.Count; i++)
                    if (rpnList[i] == "x")
                        rpnList[i] = x.ToString();
                double y = OPZ.Calculate(rpnList);

                tbXCoord.Text = "x: " + Math.Round(x, 2).ToString();
                tbYCoord.Text = "f(x): " + Math.Round(y, 2).ToString();
            }    
        }

        private static void GraphCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var canvas = (Canvas)sender;
            mousePastyPos = e.GetPosition(canvas);
            mousePressed = true;
        }
        private static void GraphCanvas_PointerMovedDrag(object? sender, PointerEventArgs e)
        {
            var canvas = (Canvas)sender;
            Point mousePos = e.GetPosition(canvas);
            if (mousePressed)
            {
                Offset = new Point(mousePos.X - mousePastyPos.X + Offset.X, mousePos.Y - mousePastyPos.Y + Offset.Y);
                RedrawCanvas();
                mousePastyPos = mousePos;
            }
        }

        private static void GraphCanvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            var canvas = (Canvas)sender;
            Point mouseEndPos = e.GetPosition(canvas);
            Offset = new Point(mouseEndPos.X - mousePastyPos.X + Offset.X, mouseEndPos.Y - mousePastyPos.Y + Offset.Y);
            RedrawCanvas();
            mousePressed = false;
        }
        private static void GraphCanvas_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            Zoom = Math.Max(Zoom * (1 - 0.1*e.Delta.Y), 0.005);
            TheMainWindow.FindControl<TextBlock>("tbInfo").Text = "Zoom: " + Math.Round(Zoom, 3).ToString();

            RedrawCanvas();
        }

        static public void CanvasSizeChenged() 
        {
            FindGraphCanvasSize();
            RedrawCanvas();
        }

        static public void RedrawCanvas()
        {
            GraphCanvas.Children.Clear();

            AddArrow(0, GraphHeight / 2 + Offset.Y, GraphWidth, GraphHeight / 2 + Offset.Y, GraphCanvas);
            AddArrow(GraphWidth / 2 + Offset.X, GraphHeight, GraphWidth / 2 + Offset.X, 0, GraphCanvas);

            if (Expression != null)
                AddLinesOnGraphCanvas(Thinker.GetPointsList(Expression,
                    -GraphWidth / 2, GraphWidth / 2,
                    -GraphHeight / 2, GraphHeight / 2,
                    new DoublePoint(Offset.X, Offset.Y), Zoom));
        }

        static private void AddArrow(double x1, double y1, double x2, double y2, Canvas canvas)
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
            if (line.StartPoint.Y > 0)
                canvas.Children.Add(line);

            line = new Line
            {
                Stroke = Brushes.Black,
                StartPoint = new Point(x2 - (X / d), y2 - (Y / d)),
                EndPoint = new Point(X4, Y4)
            };
            if (line.StartPoint.Y > 0)
                canvas.Children.Add(line);

            line = new Line
            {
                Stroke = Brushes.Black,
                StartPoint = new Point(x2 - (X / d), y2 - (Y / d)),
                EndPoint = new Point(X5, Y5)
            };
            if (line.StartPoint.Y > 0)
                canvas.Children.Add(line);
        }

        static private void AddLinesOnGraphCanvas(List<DoublePoint> pointsList)
        {
            for (int i = 1; i < pointsList.Count; i++)
            {
                GraphCanvas.Children.Add(new Line()
                {
                    StartPoint = new Point(pointsList[i - 1].X + GraphWidth / 2, -pointsList[i - 1].Y + GraphHeight / 2),
                    EndPoint = new Point(pointsList[i].X + GraphWidth / 2, -pointsList[i].Y + GraphHeight / 2),
                    StrokeThickness = 1,
                    Stroke = Brushes.Black
                });
            }
        }

        static private void FindGraphCanvasSize()
        {
            GraphWidth = TheMainWindow.Width - GraphCanvas.Margin.Right * 2;
            GraphHeight = TheMainWindow.Height - (87 + GraphCanvas.Margin.Top * 2 + 4);
        }
    }
}
