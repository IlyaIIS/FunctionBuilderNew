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
            Drawer.DrawCanvas(this);
        }


        /////////////////////////////////////////////////// —Œ¡€“»ﬂ //////////////////////////////////////////////////////
        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            var tbResult = this.Find<TextBlock>("tbResult");
            var tbExpression = this.FindControl<TextBox>("tbExpression");

            string expression = tbExpression.Text;
            string exceptionText;

            if (OPZ.IsExpressionCorrectly(expression, out exceptionText))
            {
                tbResult.Text = OPZ.Calculate(OPZ.GetRPN(expression)).ToString();
            }
            else
            {
                tbResult.Text = exceptionText;
            }
        }

        private void tbCheckFormula_KeyUp(object sender, KeyEventArgs e)
        {
            var btnButton = this.Find<Button>("btnCalculate");
            var tbExpression = (TextBox)sender;
            string exception;

            if (OPZ.IsExpressionCorrectly(tbExpression.Text, out exception))
                btnButton.Background = Avalonia.Media.Brush.Parse("#d5e0dd");
            else
                btnButton.Background = Avalonia.Media.Brush.Parse("#dcd6dd");
        }

        private void MainWindow_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Width" || e.Property.Name == "Height") Drawer.DrawCanvas(this);
        }
    }
}
