using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FunctionBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            string formula = Interface.AskFormula("Введите формулу: у=");
            double step = Interface.AskDoubleNum(1, "Введите шаг: ");
            double xStart = Interface.AskDoubleNum(2, "Введите начало: ");
            double xEnd = Interface.AskDoubleNum(3, "Введите конец: ");

            if (step > 0 == xStart < xEnd)
                Interface.WriteResult(formula, step, xStart, xEnd);
            else
                Interface.WriteResult(formula, step, xEnd, xStart);
        }
    }
}
