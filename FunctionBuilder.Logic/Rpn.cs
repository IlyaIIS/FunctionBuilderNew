using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FunctionBuilder
{
    public class Rpn
    {
        private IToken mainToken;
        static readonly string[] signArr = new string[] { "+", "-", "*", "/", "^", "(", ")", "," };
        static readonly string[] zeroOperatorArr = { "-", "+", "*", "/", "^" };
        static readonly string[] operatorArr = { "+", "-", "*", "/", "^", "(", ")", "sin", "log", "!", "round" };
        static Dictionary<string, int> operandsNumList = new Dictionary<string, int>
        {
            {"+", 2},
            {"-", 2},
            {"*", 2},
            {"/", 2},
            {"^", 2},
            {"sin", 1},
            {"log", 2},
            {"!", 1},
            {"round", 1}
        };

        public Rpn(string input)
        {
            input = input.Replace(" ", "");

            string[] pInput = ParseInput(input);

            List<IToken> tokenList = FormTokensList(pInput);

            tokenList = FormOrder(tokenList);

            mainToken = GetMainToken(tokenList);
        }

        private List<IToken> FormTokensList(string[] pInput)
        {
            List<IToken> output = new List<IToken>();

            foreach (string el in pInput)
            {
                if (Double.TryParse(el, out double result)) output.Add(new TokenDigit(result));
                else if (el == "(" || el == ")") output.Add(new TokenSign(el));
                else if (el == "x") output.Add(new TokenDigit(Double.NaN));
                else output.Add(GetOperatorFromName(el));
            }

            return output;
        }

        private List<IToken> FormOrder(List<IToken> pInput)
        {
            List<IToken> firstList = new List<IToken>();
            List<IToken> secondList = new List<IToken>();
            bool isDebugEnabled = false;

            for (int i = 0; i < pInput.Count; i++)
            {
                if (pInput[i].Type == TokenType.Digit) firstList.Add(pInput[i]);
                else if (pInput[i].Type == TokenType.Sign)
                {
                    if ((string)pInput[i].Content == "(")
                    {
                        secondList.Add(pInput[i]);
                    }
                    else if ((string)pInput[i].Content == ")")
                    {
                        for (int ii = secondList.Count - 1; ii >= 0; ii--)
                        {
                            if (secondList[ii].Type == TokenType.Operator && (string)secondList[ii].Content == "(")
                            {
                                secondList.RemoveAt(secondList.Count - 1);
                                break;
                            }
                            else
                            {
                                firstList.Add(secondList[secondList.Count - 1]);
                                secondList.RemoveAt(secondList.Count - 1);
                            }
                        }
                    }
                }
                else
                {
                    if (secondList.Count >= 1)
                    {
                        if (((IHaveOrder)secondList[secondList.Count - 1]).Order >= ((IHaveOrder)pInput[i]).Order)
                        {
                            firstList.Add(secondList[secondList.Count - 1]);
                            secondList.RemoveAt(secondList.Count - 1);
                        }
                    }
                    secondList.Add(pInput[i]);
                }

                if (isDebugEnabled)
                {
                    for (int ii = 0; ii < pInput.Count; ii++)
                    {
                        Debug.Write(pInput[ii].Content + " ");
                    }
                    Debug.WriteLine(' ');
                    Debug.WriteLine(' ');

                    string local = "";
                    for (int ii = 0; ii < firstList.Count; ii++) local += firstList[ii] + " ";
                    Debug.WriteLine("Первая строка: " + local);
                    local = "";
                    for (int ii = 0; ii < secondList.Count; ii++) local += secondList[ii] + " ";
                    Debug.WriteLine("Вторая строка: " + local);
                }
            }
            for (int i = secondList.Count - 1; i >= 0; i--) firstList.Add(secondList[i]);

            return firstList;
        }
        private IToken GetMainToken(List<IToken> tokenList)
        {
            IToken lastOperator = null;

            for (int i = 0; i < tokenList.Count; i++)
            {
                if (tokenList[i].Type == TokenType.Operator)
                {
                    ((TokenOperator)tokenList[i]).SetOperands(tokenList, i);
                    lastOperator = tokenList[i];
                }
            }

            return lastOperator;
        }

        public Rpn(Rpn input)
        {
            mainToken = input.mainToken;
        }

        private static string[] ParseInput(string input)
        {
            var preOutput = new List<string>();
            string[] output;

            RudeParce(input, preOutput);
            preOutput.RemoveAll(x => x == ",");
            InsertMulSigns(preOutput);

            output = new string[preOutput.Count];
            
            return preOutput.ToArray();
        }
        private static void RudeParce(string input, List<string> preOutput)
        {
            string token = string.Empty;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == 'x' || signArr.Contains(input[i].ToString()))
                {
                    if (token.Length != 0)
                    {
                        preOutput.Add(token);
                        token = string.Empty;
                    }
                    preOutput.Add(input[i].ToString());
                }
                else
                {
                    token += input[i];
                }
            }
            if (token.Length != 0) preOutput.Add(token);
        }
        private static void InsertMulSigns(List<string> preOutput)
        {
            for (int i = 1; i < preOutput.Count - 1; i++)
            {
                if (preOutput[i] == "(" && !operatorArr.Contains(preOutput[i - 1]))
                    preOutput.Insert(i, "*");
                if (preOutput[i] == ")" && !zeroOperatorArr.Contains(preOutput[i + 1]))
                    preOutput.Insert(i + 1, "*");
            }

            for (int i = 0; i < preOutput.Count; i++)
            {
                if (preOutput[i] == "x")
                    if (i > 0 && !operatorArr.Contains(preOutput[i - 1]))
                        preOutput.Insert(i, "*");
                    else if (i + 1 < preOutput.Count && !zeroOperatorArr.Contains(preOutput[i + 1]) && preOutput[i + 1] != ")")
                        preOutput.Insert(i + 1, "*");
            }
        }

        public double Calculate()
        {
            return mainToken.GetNumber();
        }

        private void FindAndSetVariable(IToken token, double digit)
        {
            if (token.Type == TokenType.Variable)
                token = new TokenDigit(digit);
            else if (token.Type == TokenType.Operator)
                foreach (IToken operand in ((TokenOperator)token).OperandsList)
                    FindAndSetVariable(operand, digit);
        }

        public Rpn GetNewRpnWithSetVariable(double digit)
        {
            Rpn output = new Rpn(this);

            output.FindAndSetVariable(mainToken, digit);

            return output;
        }

        static public bool IsExpressionCorrectly(string formula, out string errorText)
        {
            errorText = String.Empty;

            if (formula == null || formula.Length == 0)
            {
                errorText = "Формула не введина";
                return false;
            }

            formula = formula.Replace(" ", "");

            if (formula.Length == 1 && formula[0] != 'x')
            {
                errorText = "Формула слишком коротка";
                return false;
            }

            int bracketNum = 0;
            foreach (char letter in formula)
            {
                if (letter == '(') bracketNum++;
                if (letter == ')') bracketNum--;
                if (bracketNum < 0)
                {
                    errorText = "Ошибка в расставлении скобок";
                    return false;
                }
            }
            if (bracketNum > 0)
            {
                errorText = "Не все скобки закрыты";
                return false;
            }

            var rpn = ParseInput(formula);

            foreach(string el in rpn)
                if (!Double.TryParse(el, out double digit) && !operatorArr.Contains(el) && el != "x")
                {
                    errorText = "Неизвестный символ в формуле";
                    return false;
                }

            int needOperandNum = 0;
            int operandNum = 0;
            foreach (string el in rpn)
                if (operandsNumList.ContainsKey(el))
                {
                    operandNum++;
                    needOperandNum += operandsNumList[el];
                }else
                {
                    if (Double.TryParse(el, out double d) || el == "x")
                        operandNum++;
                }
            
            if (needOperandNum != operandNum - 1)
            {
                errorText = "Формула некорректна";
                return false;
            }

            return true;
        }

        private TokenOperator GetOperatorFromName(string input)
        {
            switch (input)
            {
                case "+": return new TokenOperatorAdd();
                case "-": return new TokenOperatorSub();
                case "*": return new TokenOperatorMul();
                case "/": return new TokenOperatorDiv();
                case "^": return new TokenOperatorExp();
                case "sin": return new TokenOperatorSin();
                case "log": return new TokenOperatorLog();
                case "!": return new TokenOperatorFactorial();
                case "round": return new TokenOperatorRound();
                default: throw new Exception("Не верное название оператора");
            }
        }
    }

    enum TokenType
    {
        Digit,
        Operator,
        Sign,
        Variable
    }

    interface IToken
    {
        public TokenType Type { get; }
        public object Content { get; }
        public double GetNumber();
    }

    class TokenDigit : IToken
    {
        public TokenType Type { get; private set; }
        public object Content { get; private set; }
        public TokenDigit(double digit)
        {
            Content = digit;
            if (double.IsNaN(digit))
                Type = TokenType.Variable;
            else
                Type = TokenType.Digit;
        }
        public double GetNumber()
        {
            return (double)Content;
        }
    }

    abstract class TokenOperator : IToken, IHaveOrder
    {
        public virtual TokenType Type { get; protected set; } = TokenType.Operator;
        public virtual Object Content { get; protected set; }
        public virtual List<IToken> OperandsList { get; protected set; } = new List<IToken>();
        public abstract int Order { get; protected set; }
        public abstract int OperandsNum { get; protected set; }
        public TokenOperator()
        {
            Type = TokenType.Operator;
        }
        public double GetNumber() 
        {
            throw new Exception("Попытка получить число от неопределённого оператора");
        }
        public void SetOperands(List<IToken> tokens, int pos)
        {
            for (int i = 1; i <= OperandsNum; i++)
            {
                OperandsList.Add(tokens[pos - i]);
            }
        }
    }

    class TokenOperatorAdd : TokenOperator
    {
        public override int Order { get; protected set; } = 1;
        public override int OperandsNum { get; protected set; } = 2;
        public new double GetNumber()
        {
            return OperandsList[0].GetNumber() + OperandsList[1].GetNumber();
        }
    }
    class TokenOperatorSub : TokenOperator
    {
        public override int Order { get; protected set; } = 1;
        public override int OperandsNum { get; protected set; } = 2;
        public new double GetNumber()
        {
            return OperandsList[0].GetNumber() - OperandsList[1].GetNumber();
        }
    }
    class TokenOperatorMul : TokenOperator
    {
        public override int Order { get; protected set; } = 2;
        public override int OperandsNum { get; protected set; } = 2;
        public new double GetNumber()
        {
            return OperandsList[0].GetNumber() * OperandsList[1].GetNumber();
        }
    }
    class TokenOperatorDiv : TokenOperator
    {
        public override int Order { get; protected set; } = 2;
        public override int OperandsNum { get; protected set; } = 2;
        public new double GetNumber()
        {
            return OperandsList[0].GetNumber() / OperandsList[1].GetNumber();
        }
    }
    class TokenOperatorExp : TokenOperator
    {
        public override int Order { get; protected set; } = 3;
        public override int OperandsNum { get; protected set; } = 2;
        public new double GetNumber()
        {
            return Math.Pow(OperandsList[0].GetNumber(), OperandsList[1].GetNumber());
        }
    }
    class TokenOperatorSin : TokenOperator
    {
        public override int Order { get; protected set; } = 10;
        public override int OperandsNum { get; protected set; } = 1;
        public new double GetNumber()
        {
            return Math.Sin(OperandsList[0].GetNumber());
        }
    }
    class TokenOperatorLog : TokenOperator
    {
        public override int Order { get; protected set; } = 10;
        public override int OperandsNum { get; protected set; } = 2;
        public new double GetNumber()
        {
            return Math.Log10(OperandsList[0].GetNumber()) / Math.Log10(OperandsList[1].GetNumber());
        }
    }
    class TokenOperatorFactorial : TokenOperator
    {
        public override int Order { get; protected set; } = 10;
        public override int OperandsNum { get; protected set; } = 1;
        public new double GetNumber()
        {
            double x = 1;
            for (int ii = 2; ii <= OperandsList[0].GetNumber(); ii++) x *= ii;
            return x;
        }
    }
    class TokenOperatorRound : TokenOperator
    {
        public override int Order { get; protected set; } = 10;
        public override int OperandsNum { get; protected set; } = 1;
        public new double GetNumber()
        {
            return Math.Round(OperandsList[0].GetNumber());
        }
    }

    class TokenSign : IToken, IHaveOrder
    {
        public TokenType Type { get; private set; } = TokenType.Sign;
        public object Content { get; private set; }
        public int Order { get; private set; } = 0;
        public TokenSign(string content)
        {
            Content = content;
        }
        public double GetNumber()
        {
            throw new Exception("Попытка использования знака в качестве оператора");
        }
    }

    interface IHaveOrder
    {
        public int Order { get; }
    }

    delegate double GetNumberDel(List<IToken> operands);
}
