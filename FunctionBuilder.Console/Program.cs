using System;
using System.IO;
using System.Linq;

namespace FunctionBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ry");
            if (IsFileCorrect())
            {
                string[] input = ReadFile();

                string formula = input[0];
                double step = Convert.ToDouble(input[1].Replace(',','.'));
                double xStart = Convert.ToDouble(input[2].Replace(',', '.'));
                double xEnd = Convert.ToDouble(input[3].Replace(',', '.'));

                if (step > 0 == xStart < xEnd)
                    WriteResult(formula, step, xStart, xEnd);
                else
                    WriteResult(formula, step, xEnd, xStart);
            }
        }

        /////////////////////////////////////////////////////РАБОТА С ФАЙЛАМИ/////////////////////////////////////////////
        static string[] ReadFile()
        {
            string[] output = new string[4];
            int i = 0;
            foreach (string element in File.ReadLines("../../../input.txt"))
            {
                output[i] = element;
                i++;
            }

            return output;
        }

        static bool IsFileCorrect()
        {
            try
            {
                if (File.ReadLines("../../../input.txt").Count() == 4) return true;
                else
                {
                    Console.WriteLine("Введите в файле 4 строки: формула, шаг, Х начальное, Х конечное.");
                    return false;
                }
            }
            catch
            {
                Console.WriteLine("Файл не обнаружен (поместите файл input.txt в папку с программой)");
                return false;
            }
        }
        
        /////////////////////////////////////////////////////ВЫВОД РЕЗУЛЬТАТА//////////////////////////////////////////////////
        static void WriteResult(string formula, double step, double xStart, double xEnd)    
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
                    (maxSize - DigitSize(x)) / 2,
                    (maxSize - DigitSize(y)) / 2,
                    (int)Math.Ceiling((double)(maxSize - DigitSize(x)) / 2), 
                    (int)Math.Ceiling((double)(maxSize - DigitSize(y)) / 2), text);

                x = Convert.ToDouble(Convert.ToDecimal(x) + Convert.ToDecimal(step));
            } while ((step > 0 && x <= xEnd) || (step < 0 && x >= xEnd));

            text = WriteBorder('─', '└', '┴', '┘', maxSize, text);

            File.WriteAllText("../../../output.txt", text);
        }
        static double GetY(string formula, double x)
        {
            return OPZ.Calculate(OPZ.ParseExpression(formula.Replace("x", Convert.ToString(x))));
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
                if (DigitSize(y) > maxSize) { maxSize = DigitSize(y); }
                if (DigitSize(x) > maxSize) { maxSize = DigitSize(x); }
                x += step;
            } while ((step > 0 && x <= xEnd) || (step < 0 && x >= xEnd));

            return maxSize;
        }

        static int DigitSize(double d)            //Сколько символов в числе
        {
            return Convert.ToString(d).Length;
        }
    }
}
