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
            Assert.AreEqual(new List<string> { "5", "1.3", "*", "round", "2", "^" }, OPZ.ParseExpression(expression));
        }

        [TestCase("sinx", ExpectedResult = false)]
        [TestCase("((x+1)*2)", ExpectedResult = true)]
        public bool FormulaCorrectlyTest(string formula)
        {
            return OPZ.IsExpressionCorrectly(formula, out string s);
        }

        [TestCaseSource(nameof(TestCases))]
        public void CalculatorTests(double result, List<string> expression)
        {
            Assert.AreEqual(result, OPZ.Calculate(expression));
        }
        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(3, new List<string> { "1", "2", "+" });
                yield return new TestCaseData(9, new List<string> { "3", "1", "2", "+", "*" });
            }
        }
    }
}