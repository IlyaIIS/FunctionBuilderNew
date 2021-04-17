using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionBuilder.Desktop
{
    static class Drawer
    {
        public static double GraphWidth { get; private set; }
        public static double GraphHeight { get; private set; }
        public static Canvas GraphCanvas { get; private set; }
        public static Window TheMainWindow { get; private set; }
        private static List<DoublePoint> pointsList = new List<DoublePoint>();
        public static double Step { 
            get
            {
                return step;
            }
            set
            {
                step = value;
                IsStepDefault = false;
            }
        }

        private const int pointNum = 339;

        private static double step = GraphWidth / pointNum;
        public static bool IsStepDefault { get; private set; } = true;
        public static Point Offset { get; private set; }
        private static Point mousePastyPos;
        private static bool mousePressed = false;
        public static double Zoom { get; private set; } = 1;
        public static string Expression 
        { 
            get
            {
                return expression;
            }
            set
            {
                expression = value;
                rpn = new Rpn(value);
            }
        }
        private static string expression;
        private static Rpn rpn;
        public static void SetControls(Window window)
        {
            TheMainWindow = window;
            GraphCanvas = window.Find<Canvas>("cGraphCanvas");
            GraphCanvas.PointerPressed += GraphCanvas_PointerPressed;
            GraphCanvas.PointerMoved += GraphCanvas_PointerMovedDrag;
            GraphCanvas.PointerReleased += GraphCanvas_PointerReleased;
            GraphCanvas.PointerWheelChanged += GraphCanvas_PointerWheelChanged;
            GraphCanvas.PointerMoved += GraphCanvas_PointerMoved;
        }

        //Изменение отображаемого значения графика функции в зависимости от позиции мыши
        private static void GraphCanvas_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (!mousePressed && expression != null)
            {
                TextBlock tbXCoord = TheMainWindow.FindControl<TextBlock>("tbXCoord");
                TextBlock tbYCoord = TheMainWindow.FindControl<TextBlock>("tbYCoord");
                var canvas = (Canvas)sender;

                double x = (e.GetPosition(canvas).X - GraphWidth / 2 - Offset.X)*Zoom;
                Rpn localRpn = new Rpn(rpn);
                localRpn.SetVariable(x);
                double y = localRpn.Calculate();

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
                Offset = new Point(Offset.X + mousePos.X - mousePastyPos.X, Offset.Y + mousePos.Y - mousePastyPos.Y);
                UpdateCanvas();
                mousePastyPos = mousePos;
            }
        }

        private static void GraphCanvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            var canvas = (Canvas)sender;
            Point mouseEndPos = e.GetPosition(canvas);
            Offset = new Point(mouseEndPos.X - mousePastyPos.X + Offset.X, mouseEndPos.Y - mousePastyPos.Y + Offset.Y);
            UpdateCanvas();
            mousePressed = false;
        }
        private static void GraphCanvas_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            Zoom = Math.Min(Math.Max(Zoom * (1 - 0.1 * e.Delta.Y), 0.001), 200);
            TheMainWindow.FindControl<TextBlock>("tbInfo").Text = "Zoom: " + Math.Round(Zoom, 3).ToString();

            UpdateCanvas(true);
        }

        public static void CanvasSizeChenged() 
        {
            FindGraphCanvasSize();
            UpdateCanvas(true);
            if (IsStepDefault) step = GraphWidth / 339;
        }

        public static void UpdateCanvas(bool clearPoints = false)
        {
            GraphCanvas.Children.Clear();

            AddArrow(0, GraphHeight / 2 + Offset.Y, GraphWidth, GraphHeight / 2 + Offset.Y, GraphCanvas);
            AddArrow(GraphWidth / 2 + Offset.X, GraphHeight, GraphWidth / 2 + Offset.X, 0, GraphCanvas);

            if (clearPoints) pointsList.Clear();

            if (expression != null)
            {
                UpdatePointsList();
                AddLinesOnGraphCanvas(GetPointsToGraph());
            }
        }

        private static double GetLeftPointXPos()
        {
            return Math.Round((-Offset.X - GraphWidth / 2) / Step) * Step;
        }
        private static double GetRightPointXPos()
        {
            return Math.Round((-Offset.X + GraphWidth / 2) / Step) * Step;
        }

        private static List<DoublePoint> GetPointsToGraph()
        {
            var graphPoinst = new List<DoublePoint>();

            double minY = -GraphHeight / 2 - 1;
            double maxY = GraphHeight / 2 + 1;

            double x = (-Offset.X - GraphWidth / 2);
            double y = new Rpn(rpn).GetNewRpnWithSetVariable(x*Zoom).Calculate()/Zoom - Offset.Y;
            graphPoinst.Add(new DoublePoint(x + Offset.X, y));

            foreach(DoublePoint el in pointsList)
            {
                if (el.X > GetLeftPointXPos() && el.X < GetRightPointXPos())
                {
                    var point = new DoublePoint(el.X, el.Y);

                    point.Y -= Offset.Y;
                    point.X += Offset.X;

                    if (point.Y > maxY) point.Y = maxY;
                    if (point.Y < minY) point.Y = minY;

                    graphPoinst.Add(point);
                }
            }

            for (int i = 1; i < graphPoinst.Count - 1; i++)
            {
                if (graphPoinst[i].Y == maxY)
                {
                    if (graphPoinst[i - 1].Y == maxY && graphPoinst[i + 1].Y == maxY)
                    {
                        graphPoinst.RemoveAt(i);
                        i--;
                    }
                }
                else if (graphPoinst[i].Y == minY)
                {
                    if (graphPoinst[i - 1].Y == minY && graphPoinst[i + 1].Y == minY)
                    {
                        graphPoinst.RemoveAt(i);
                        i--;
                    }
                }
            }

            x = (-Offset.X + GraphWidth / 2);
            y = new Rpn(rpn).GetNewRpnWithSetVariable(x*Zoom).Calculate() / Zoom - Offset.Y;
            graphPoinst.Add(new DoublePoint(x + Offset.X, y));

            return graphPoinst;
        }

        private static void UpdatePointsList()
        {
            if (pointsList.Count == 0)
                SetPoints();
            else if (pointsList[0].X - (-Offset.X - GraphWidth / 2) > Step)
                AddPointsOnLeft();
            else if ((-Offset.X + GraphWidth / 2) - pointsList[pointsList.Count - 1].X > Step)
                AddPointsOnRight();
        }

        private static void SetPoints()
        {
            double xStart = GetLeftPointXPos();
            double xEnd = GetRightPointXPos();
            pointsList = Thinker.GetPointsList(rpn, xStart, xEnd, Step, Zoom);
        }
        private static void AddPointsOnLeft()
        {
            double xStart = GetLeftPointXPos();
            var additionalPoints = Thinker.GetPointsList(rpn, xStart, pointsList[0].X - Step, Step,Zoom);
            pointsList = additionalPoints.Concat(pointsList).ToList();
        }
        private static void AddPointsOnRight()
        {
            double xEnd = GetRightPointXPos();
            var additionalPoints = Thinker.GetPointsList(rpn, pointsList[pointsList.Count - 1].X + Step, xEnd, Step,Zoom);
            pointsList = pointsList.Concat(additionalPoints).ToList();
        }

        private static void AddLinesOnGraphCanvas(List<DoublePoint> pointsList)
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

        private static void AddArrow(double x1, double y1, double x2, double y2, Canvas canvas)
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

        private static void FindGraphCanvasSize()
        {
            GraphWidth = TheMainWindow.Width - GraphCanvas.Margin.Right * 2;
            GraphHeight = TheMainWindow.Height - (120 + GraphCanvas.Margin.Top * 2 + 4);
        }

        public static void SetStepDefault()
        {
            step = GraphWidth / pointNum;
            IsStepDefault = true;
        }
    }
}
