using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            var gResult = this.Find<Grid>("gResult");
            var tbResult = this.Find<TextBlock>("tbResult");
            var tbExpression = this.FindControl<TextBox>("tbExpression");

            string expression = tbExpression.Text;
            string exceptionText;

            gResult.IsVisible = true;
            if (OPZ.IsExpressionCorrectly(expression, out exceptionText))
                try
                {
                    tbResult.Text = OPZ.Calculate(OPZ.ParseExpression(expression)).ToString();
                }
                catch
                {
                    tbResult.Text = "В формуле содержится ошибка";
                }
            else
                tbResult.Text = exceptionText;
        }

        private void tbCheckFormula_LostFocus(object sender, KeyEventArgs e)
        {
            var btnButton = this.Find<Button>("btnCalculate");
            var tbExpression = (TextBox)sender;
            string exception;

            if (OPZ.IsExpressionCorrectly(tbExpression.Text, out exception))
            {
                try
                {
                    OPZ.Calculate(OPZ.ParseExpression(this.FindControl<TextBox>("tbExpression").Text));
                    btnButton.Background = Avalonia.Media.Brush.Parse("#d5e0dd");
                }
                catch
                {
                    btnButton.Background = Avalonia.Media.Brush.Parse("#d5d6dd");
                }
            }else
            {
                btnButton.Background = Avalonia.Media.Brush.Parse("#d5d6dd");
            }
        }
    }
}
