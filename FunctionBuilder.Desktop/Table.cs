using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionBuilder.Desktop
{
    class Table
    {
        private List<Vector> pointsList = new List<Vector>();
        private List<string> rpnList;
        private Window window;
        private TextBox tbStart;
        private TextBox tbEnd;
        private TextBox tbStep;
        private StackPanel spValues;
        public Table(string expression, List<string> rpnList)
        {
            this.rpnList = rpnList;

            window = new Window();
            window.MinWidth = 200;
            window.MaxWidth = 200;
            window.Height = 500;

            var sp = new StackPanel();
            window.Content = sp;

            var tbExpressint = new TextBlock() { 
                Text = expression,
                TextAlignment = Avalonia.Media.TextAlignment.Center
            };
            sp.Children.Add(tbExpressint);

            var dpCoords = new DockPanel();
            tbStart = new TextBox() { 
                InnerLeftContent = "start",
                Width = window.MinWidth / 2
            };
            tbEnd = new TextBox() { 
                InnerLeftContent = "end",
                Width = window.MinWidth / 2
            };
            dpCoords.Children.Add(tbStart);
            dpCoords.Children.Add(tbEnd);
            sp.Children.Add(dpCoords);

            tbStep = new TextBox() {
                InnerLeftContent = "step",
                Width = window.MinWidth
            };
            sp.Children.Add(tbStep);

            var btnCalculate = new Button() { Content = "РАССЧИТАТЬ" };
            btnCalculate.Width = window.MinWidth;
            btnCalculate.Click += BtnCalculate_Click;
            sp.Children.Add(btnCalculate);

            var dpXY = new DockPanel();
            dpXY.Children.Add(new TextBlock() { 
                Text = "x", 
                Width = window.MinWidth / 2,
                TextAlignment = Avalonia.Media.TextAlignment.Center
            });
            dpXY.Children.Add(new TextBlock() { 
                Text = "y",
                Width = window.MinWidth / 2,
                TextAlignment = Avalonia.Media.TextAlignment.Center
            });
            sp.Children.Add(dpXY);

            spValues = new StackPanel();
            sp.Children.Add(spValues);
        }

        public void Show()
        {
            window.Show();
        }

        private void BtnCalculate_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (double.TryParse(tbStart.Text, out double xStart) && 
                double.TryParse(tbEnd.Text,   out double xEnd) &&
                double.TryParse(tbStep.Text,  out double step) &&
                xStart < xEnd)
            {
                spValues.Children.Clear();
                double x = xStart;
                do
                {
                    var rpnListLocal = new List<string>(rpnList);
                    for (int i = 0; i < rpnListLocal.Count; i++)
                        if (rpnListLocal[i] == "x")
                            rpnListLocal[i] = x.ToString();

                    var dp = new DockPanel();
                    dp.Children.Add(new TextBlock()
                    {
                        Text = x.ToString(),
                        Width = window.MinWidth / 2,
                        TextAlignment = Avalonia.Media.TextAlignment.Center
                    });
                    dp.Children.Add(new TextBlock()
                    {
                        Text = Math.Round(OPZ.Calculate(rpnListLocal),3).ToString(),
                        Width = window.MinWidth / 2,
                        TextAlignment = Avalonia.Media.TextAlignment.Center
                    });
                    spValues.Children.Add(dp);

                    x = Math.Round(x + step, 3);
                } while (x <= xEnd);
            }
        }
    }
}
