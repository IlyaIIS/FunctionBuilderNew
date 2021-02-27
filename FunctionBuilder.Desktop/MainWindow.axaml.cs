using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace FunctionBuilder.Desktop
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var tbExpression = this.FindControl<TextBox>("tbExpression");
            var bBorder = this.Find<Border>("bBotder");
        }

        private void btCalculate_Click(object sender, RoutedEventArgs e)
        {
            var gResult = this.Find<Grid>("gResult");
            var tbResult = this.Find<TextBlock>("tbResult");
            var tbExpression = this.FindControl<TextBox>("tbExpression");

            string expression = tbExpression.Text;
            string exceptionText;

            gResult.IsVisible = true;
            if (OPZ.IsExpressionCorrectly(expression, out exceptionText))
                tbResult.Text = OPZ.Calculate(OPZ.ParseExpression(expression)).ToString();
            else
                tbResult.Text = exceptionText;
        }
    }
}
