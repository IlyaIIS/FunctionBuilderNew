using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace FunctionBuilder.Desktop
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            Drawer.SetControls(this);
            Drawer.CanvasSizeChenged();
        }


        /////////////////////////////////////////////////// —Œ¡€“»ﬂ //////////////////////////////////////////////////////
        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            var tbInfo = this.Find<TextBlock>("tbInfo");
            var tbExpression = this.FindControl<TextBox>("tbExpression");

            string expression = tbExpression.Text;
            string exceptionText;

            if (Rpn.IsExpressionCorrectly(expression, out exceptionText))
            {
                Drawer.Expression = expression;
                Drawer.RedrawCanvas();
            }
            else
            {
                tbInfo.Text = exceptionText;
            }
        }

        private void tbCheckFormula_KeyUp(object sender, KeyEventArgs e)
        {
            var btnCalculate = this.Find<Button>("btnCalculate");
            var btnTable = this.Find<Button>("btnTable");
            var tbExpression = (TextBox)sender;
            string exception;

            if (Rpn.IsExpressionCorrectly(tbExpression.Text, out exception))
            {
                btnCalculate.Background = Avalonia.Media.Brush.Parse("#d5e0dd");
                btnTable.Background = Avalonia.Media.Brush.Parse("#d5e0dd");
            }
            else
            {
                btnCalculate.Background = Avalonia.Media.Brush.Parse("#dcd6dd");
                btnTable.Background = Avalonia.Media.Brush.Parse("#dcd6dd");
            }
        }

        private void tbStep_KeyUp(object sender, KeyEventArgs e)
        {
            var tbStep = (TextBox)sender;
            if (Double.TryParse(tbStep.Text, out double result) && result > 0) Drawer.Step = result;
        }

        private void MainWindow_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Width" || e.Property.Name == "Height") Drawer.CanvasSizeChenged();
        }

        private void btnTable_Click(object sender, RoutedEventArgs e)
        {
            var tbInfo = this.Find<TextBlock>("tbInfo");
            var tbExpression = this.FindControl<TextBox>("tbExpression");

            string expression = tbExpression.Text;
            string exceptionText;

            if (Rpn.IsExpressionCorrectly(expression, out exceptionText))
            {
                var table = new Table(expression, new Rpn(expression));
                table.Show();
            }
            else
            {
                tbInfo.Text = exceptionText;
            }
        }

        private void btnStep_Click(object sender, RoutedEventArgs e)
        {
            var tbStep = this.FindControl<TextBox>("tbStep");
            tbStep.Text = "default";

            Drawer.SetStepDefault();
        }
    }
}
