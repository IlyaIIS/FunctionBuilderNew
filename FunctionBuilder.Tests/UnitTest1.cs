using NUnit.Framework;
using System.Collections.Generic;

namespace FunctionBuilder
{
    public class Tests
    {
        [TestCase]
        public void CalculatorTest()
        {
            var expression = "round(5*1.3)^2";
            Assert.AreEqual(OPZ.ParseExpression(expression), new List<string> {"5", "1.3", "*", "round", "2", "^" });
        }

        [TestCase("sinx", ExpectedResult = false)]
        [TestCase("((x+1)*2)", ExpectedResult = true)]
        public bool FormulaCorrectlyTest(string formula)
        {
            return OPZ.IsFormulaCorrectly(formula, out string s);
        }

        [TestCaseSource(nameof(TestCases))]
        public void CalculatorTests(List<string> expression, double result)
        {
            Assert.AreEqual(result, OPZ.Calculate(expression));
        }
        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(new List<string> { "1", "2", "+" }, 3);
                yield return new TestCaseData(new List<string> { "3", "1", "2", "+", "*" }, 9);
            }
        }

    }
}