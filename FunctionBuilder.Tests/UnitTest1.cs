using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;

namespace FunctionBuilder
{
    public class Tests
    {
        [TestCase]
        public void CalculatorTest()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var expression = "round(5*1.3)^x";
            Assert.AreEqual(new Rpn("round(5*1.3)^10").Calculate(), new Rpn(expression).GetNewRpnWithSetVariable(10).Calculate());
        }

        [TestCase("sinx", ExpectedResult = false)]
        [TestCase("((x+1)*2)log(8,2)3+1", ExpectedResult = true)]
        public bool FormulaCorrectlyTest(string formula)
        {
            return Rpn.IsExpressionCorrectly(formula, out string s);
        }

        [TestCaseSource(nameof(TestCases))]
        public void CalculatorTests(double result, string expression)
        {
            Assert.AreEqual(result, new Rpn(expression).Calculate());
        }
        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(3, "1+2");
                yield return new TestCaseData(9, "(1+2)*3");
            }
        }
    }
}