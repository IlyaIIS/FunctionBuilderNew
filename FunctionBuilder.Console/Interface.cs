using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionBuilder
{
    static class Interface
    {
        static public string AskFormula(string text)
        {
            string output;
            Console.Write("\r" + new string(' ', Console.WindowWidth - Console.CursorLeft) + "\r" + text);
            output = Console.ReadLine();
            while(!OPZ.IsExpressionCorrectly(output, out string errorText))
            {
                Console.SetCursorPosition(0, 0);
                Console.Write("\r" + new string(' ', Console.WindowWidth - Console.CursorLeft) + "\r" + errorText);
                Console.ReadKey(true);
                Console.Write("\r" + new string(' ', Console.WindowWidth - Console.CursorLeft) + "\r" + text);
                output = Console.ReadLine();
            }
            
            return output;
        }
        static public double AskDoubleNum(int line, string text)
        {
            double output;
            string input;

            do
            {
                Console.SetCursorPosition(0, line);
                Console.Write(new string(' ', Console.WindowWidth - Console.CursorLeft) + "\r" + text);
                input = Console.ReadLine();
            } while (!Double.TryParse(input, out output));

            return output;
        }

        static public void WriteResult(string formula, double step, double xStart, double xEnd)
        {
            double x, y;
            int maxSize = GetMaxSizeXY(formula, 2, xStart, step, xEnd);
            string text = "";

            //Отрисовка
            text = WriteBorder('─', '┌', '┬', '┐', maxSize, text);

            text = WriteMid(' ', '│', "X", "Y", (maxSize - 1) / 2, (maxSize - 1) / 2,
                                                            (int)Math.Ceiling((double)(maxSize - 1) / 2),
                                                            (int)Math.Ceiling((double)(maxSize - 1) / 2), text);

            text = WriteBorder('─', '├', '┼', '┤', maxSize, text);

            x = xStart;
            do
            {
                y = GetY(formula, x);

                text = WriteMid(' ', '│', Convert.ToString(x), Convert.ToString(y),
                    (maxSize - GetDigitSize(x)) / 2,
                    (maxSize - GetDigitSize(y)) / 2,
                    (int)Math.Ceiling((double)(maxSize - GetDigitSize(x)) / 2),
                    (int)Math.Ceiling((double)(maxSize - GetDigitSize(y)) / 2), text);

                x = Convert.ToDouble(Convert.ToDecimal(x) + Convert.ToDecimal(step));
            } while ((step > 0 && x <= xEnd) || (step < 0 && x >= xEnd));

            text = WriteBorder('─', '└', '┴', '┘', maxSize, text);

            Console.WriteLine(text);
        }
        static double GetY(string formula, double x)
        {
            return OPZ.Calculate(OPZ.GetRPN(formula.Replace("x", Convert.ToString(x))));
        }

        static string WriteBorder(char char0, char char1, char char2, char char3, int maxSize, string text)
        {
            return text + char1 + new string(char0, maxSize) + char2 + new string(char0, maxSize) + char3 + "\n";
        }
        static string WriteMid(char char0, char char1, string char2, string char3, int maxSize1, int maxSize2, int maxSize3, int maxSize4, string text)
        {
            return text + char1 + new string(char0, maxSize1) + char2 + new string(char0, maxSize3) + char1 +
                new string(char0, maxSize2) + char3 + new string(char0, maxSize4) + char1 + "\n";
        }

        static int GetMaxSizeXY(string formula, int maxSize, double x, double step, double xEnd)
        {
            double y;
            do
            {
                y = GetY(formula, x);
                if (GetDigitSize(y) > maxSize) { maxSize = GetDigitSize(y); }
                if (GetDigitSize(x) > maxSize) { maxSize = GetDigitSize(x); }
                x += step;
            } while ((step > 0 && x <= xEnd) || (step < 0 && x >= xEnd));

            return maxSize;
        }

        static int GetDigitSize(double d)            //Сколько символов в числе
        {
            return Convert.ToString(d).Length;
        }
    }
}
