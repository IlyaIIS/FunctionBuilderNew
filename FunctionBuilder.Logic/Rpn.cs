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
        static Dictionary<string, int> ordersList = new Dictionary<string, int>
        {
            {"(", 0},
            {"+", 1},
            {"-", 1},
            {"*", 2},
            {"/", 2},
            {"^", 3},
            {"sin", 10},
            {"log", 10},
            {"!", 10},
            {"round", 10}
        };
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

            List<string> stringList = FormStringList(pInput);

            tokens = TransformStringListToTokenList(stringList);
        }
        private List<string> FormStringList(string[] pInput)
        {
            List<string> firstList = new List<string>();
            List<string> secondList = new List<string>();
            bool isDebugEnabled = false;

            for (int i = 0; i < pInput.Length; i++)
            {
                if (Char.IsDigit(pInput[i][0]) || pInput[i] == "x") firstList.Add(pInput[i]);
                else if (pInput[i][0] == '(') secondList.Add(pInput[i]);
                else if (pInput[i][0] == ')')
                {
                    for (int ii = secondList.Count - 1; ii >= 0; ii--)
                    {
                        if (secondList[ii] == "(")
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
                else
                if (operatorArr.Contains(pInput[i]))
                {
                    if (secondList.Count >= 1)
                    {
                        if (ordersList[secondList[secondList.Count - 1]] >= ordersList[pInput[i]])
                        {
                            firstList.Add(secondList[secondList.Count - 1]);
                            secondList.RemoveAt(secondList.Count - 1);
                        }
                    }
                    secondList.Add(pInput[i]);
                }

                if (isDebugEnabled)
                {
                    for (int ii = 0; ii < pInput.Length; ii++)
                    {
                        Debug.Write(pInput[ii] + " ");
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
        private List<IToken> TransformStringListToTokenList(List<string> stringList)
        {
            List<IToken> output = new List<IToken>();

            foreach (string el in stringList)
            {
                if (Double.TryParse(el, out double result)) output.Add(new TokenDigit(result));
                else if (el == "(" || el == ")") output.Add(new TokenSign(el));
                else if (el == "x") output.Add(new TokenDigit(Double.NaN));
                else
                {
                    output.Add(new TokenOperator(el,, GetOperationDel(el)));
                }
            }

            return output;
        }

        public Rpn(Rpn input)
        {
            tokens = new List<Token>(input.tokens);
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

        public void SetVariable(double digit)
        {
            for (int i = 0; i < tokens.Count; i++)
                if (tokens[i].Type == TokenType.Variable)
                    tokens[i] = new Token(digit);
        }

        public Rpn GetNewRpnWithSetVariable(double digit)
        {
            Rpn output = new Rpn(this);

            for (int i = 0; i < tokens.Count; i++)
                if (tokens[i].Type == TokenType.Variable)
                    output.tokens[i] = new Token(digit);

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

        private GetNumberDel GetOperationDel(string input)
        {
            GetNumberDel output;
            switch (input)
            {
                case "+": return Add;
                case "-": return Sub;
                case "*": return Mul;
                case "/": return Div;
                case "^": return Exp;
                case "sin": return Sin;
                case "log": return Log;
                case "!": return Factorial;
                case "round": return Round;
                default: throw new Exception("Не верное название оператора");
            }
        }

        GetNumberDel Add = new GetNumberDel((List<IToken> operands) =>
        {
            return operands[0].GetNumber() + operands[1].GetNumber();
        });

        GetNumberDel Sub = new GetNumberDel((List<IToken> operands) =>
        {
            return operands[0].GetNumber() - operands[1].GetNumber();
        });

        GetNumberDel Mul = new GetNumberDel((List<IToken> operands) =>
        {
            return operands[0].GetNumber() * operands[1].GetNumber();
        });

        GetNumberDel Div = new GetNumberDel((List<IToken> operands) =>
        {
            return operands[0].GetNumber() / operands[1].GetNumber();
        });

        GetNumberDel Exp = new GetNumberDel((List<IToken> operands) =>
        {
            return Math.Pow(operands[0].GetNumber(), operands[1].GetNumber());
        });

        GetNumberDel Sin = new GetNumberDel((List<IToken> operands) =>
        {
            return Math.Sin(operands[0].GetNumber());
        });

        GetNumberDel Log = new GetNumberDel((List<IToken> operands) =>
        {
            return Math.Log10(operands[0].GetNumber()) / Math.Log10(operands[1].GetNumber());
        });

        GetNumberDel Factorial = new GetNumberDel((List<IToken> operands) =>
        {
            double x = 1;
            for (int ii = 2; ii <= operands[0].GetNumber(); ii++) x *= ii;
            return x;
        });

        GetNumberDel Round = new GetNumberDel((List<IToken> operands) =>
        {
            return Math.Round(operands[0].GetNumber());
        });
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
        public double GetNumber();
    }

    class TokenDigit : IToken
    {
        public TokenType Type { get; private set; }
        public double Content { get; private set; }
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
            return Content;
        }
    }

    /*interface TokenOperator : IToken
    {
        public string Content { get; }
        public List<IToken> OperandsList { get; }
    }*/

    abstract class TokenOperator : IToken
    {
        public virtual TokenType Type { get; protected set; } = TokenType.Operator;
        public virtual string Content { get; protected set; }
        public virtual List<IToken> OperandsList { get; protected set; }
        public abstract int Order { get; protected set; }
        public TokenOperator(string content, List<IToken> operands)
        {
            Type = TokenType.Operator;
            Content = content;
            OperandsList = operands;
        }
        public abstract double GetNumber();
    }

    class TokenOperatorAdd : TokenOperator
    {
        public override int Order { get; protected set; } = 1;
        public TokenOperatorAdd(string content, List<IToken> operands) : base(content, operands) { }
        public override double GetNumber()
        {
            return OperandsList[0].GetNumber() + OperandsList[1].GetNumber();
        }
    }
    class TokenOperatorSub : TokenOperator
    {
        public TokenOperatorSub(string content, List<IToken> operands) : base(content, operands) { }
        public override double GetNumber()
        {
            return OperandsList[0].GetNumber() - OperandsList[1].GetNumber();
        }
    }
    class TokenOperatorMul : TokenOperator
    {
        public TokenOperatorMul(string content, List<IToken> operands) : base(content, operands) { }
        public override double GetNumber()
        {
            return OperandsList[0].GetNumber() * OperandsList[1].GetNumber();
        }
    }
    class TokenOperatorDiv : TokenOperator
    {
        public TokenOperatorDiv(string content, List<IToken> operands) : base(content, operands) { }
        public override double GetNumber()
        {
            return OperandsList[0].GetNumber() / OperandsList[1].GetNumber();
        }
    }
    class TokenOperatorExp : TokenOperator
    {
        public TokenOperatorExp(string content, List<IToken> operands) : base(content, operands) { }
        public override double GetNumber()
        {
            return Math.Pow(OperandsList[0].GetNumber(), OperandsList[1].GetNumber());
        }
    }
    class TokenOperatorSin : TokenOperator
    {
        public TokenOperatorSin(string content, List<IToken> operands) : base(content, operands) { }
        public override double GetNumber()
        {
            return Math.Sin(OperandsList[0].GetNumber());
        }
    }
    class TokenOperatorLog : TokenOperator
    {
        public TokenOperatorLog(string content, List<IToken> operands) : base(content, operands) { }
        public override double GetNumber()
        {
            return Math.Log10(OperandsList[0].GetNumber()) / Math.Log10(OperandsList[1].GetNumber());
        }
    }
    class TokenOperatorFactorial : TokenOperator
    {
        public TokenOperatorFactorial(string content, List<IToken> operands) : base(content, operands) { }
        public override double GetNumber()
        {
            double x = 1;
            for (int ii = 2; ii <= OperandsList[0].GetNumber(); ii++) x *= ii;
            return x;
        }
    }
    class TokenOperatorRound : TokenOperator
    {
        public TokenOperatorRound(string content, List<IToken> operands) : base(content, operands) { }
        public override double GetNumber()
        {
            return Math.Round(OperandsList[0].GetNumber());
        }
    }

    class TokenSign : IToken
    {
        public TokenType Type { get; private set; } = TokenType.Sign;
        public string Content { get; private set; }
        public TokenSign(string content)
        {
            Content = content;
        }
        public double GetNumber()
        {
            throw new Exception("Попытка использования знака в качестве оператора");
        }
    }

    delegate double GetNumberDel(List<IToken> operands);
}
