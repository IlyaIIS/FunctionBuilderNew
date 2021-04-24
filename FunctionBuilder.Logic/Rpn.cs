using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FunctionBuilder
{
    public class Rpn
    {
        private List<Token> tokens = new List<Token>();
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
        private List<Token> TransformStringListToTokenList(List<string> stringList)
        {
            List<Token> output = new List<Token>();

            foreach (string el in stringList)
            {
                if (Double.TryParse(el, out double result)) output.Add(new Token(result, TokenType.Digit));
                else if (el == "(" || el == ")") output.Add(new Token(el, TokenType.Sign));
                else if (el == "x") output.Add(new Token(el, TokenType.Variable));
                else output.Add(new Token(el, TokenType.Operator));
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
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type != TokenType.Digit && tokens[i].Type != TokenType.Variable)
                {
                    switch (tokens[i].Content)
                    {
                        case "+":
                            DoOperation(tokens, ref i, Operations.Add, (string)tokens[i].Content);
                            break;
                        case "-":
                            DoOperation(tokens, ref i, Operations.Sub, (string)tokens[i].Content);
                            break;
                        case "*":
                            DoOperation(tokens, ref i, Operations.Mul, (string)tokens[i].Content);
                            break;
                        case "/":
                            DoOperation(tokens, ref i, Operations.Div, (string)tokens[i].Content);
                            break;
                        case "^":
                            DoOperation(tokens, ref i, Operations.Exp, (string)tokens[i].Content);
                            break;
                        case "sin":
                            DoOperation(tokens, ref i, Operations.Sin, (string)tokens[i].Content);
                            break;
                        case "log":
                            DoOperation(tokens, ref i, Operations.Log, (string)tokens[i].Content);
                            break;
                        case "!":
                            DoOperation(tokens, ref i, Operations.Factorial, (string)tokens[i].Content);
                            break;
                        case "round":
                            DoOperation(tokens, ref i, Operations.Round, (string)tokens[i].Content);
                            break;
                    }
                }
            }

            return (Double)tokens[0].Content;
        }
        delegate Token OperationDel(List<Token> operands, int pos);
        private static void DoOperation(List<Token> tokens, ref int pos, OperationDel del, string content)
        {
            tokens[pos] = del(tokens, pos);

            for (int i = 1; i <= operandsNumList[content]; i++)
            {
                tokens.RemoveAt(pos - i);
            }
            pos -= operandsNumList[content];
        }
        static class Operations
        {
            public static OperationDel Add = new OperationDel((List<Token> tokens, int i) =>
            {
                return new Token((Double)tokens[i - 2].Content + (Double)tokens[i - 1].Content);
            });
            public static OperationDel Sub = new OperationDel((List < Token > tokens, int i) =>
            {
                return new Token((Double)tokens[i - 2].Content - (Double)tokens[i - 1].Content);
            });
            public static OperationDel Mul = new OperationDel((List<Token> tokens, int i) =>
            {
                return new Token((Double)tokens[i - 2].Content * (Double)tokens[i - 1].Content);
            });
            public static OperationDel Div = new OperationDel((List<Token> tokens, int i) =>
            {
                return new Token((Double)tokens[i - 2].Content / (Double)tokens[i - 1].Content);
            });
            public static OperationDel Exp = new OperationDel((List<Token> tokens, int i) =>
            {
                return new Token(Math.Pow((Double)tokens[i - 2].Content, (Double)tokens[i - 1].Content));
            });
            public static OperationDel Sin = new OperationDel((List<Token> tokens, int i) =>
            {
                return new Token(Math.Sin((Double)tokens[i - 1].Content));
            });
            public static OperationDel Log = new OperationDel((List<Token> tokens, int i) =>
            {
                return new Token(Math.Log10((Double)tokens[i - 2].Content) / Math.Log10((Double)tokens[i - 1].Content));
            });
            public static OperationDel Factorial = new OperationDel((List<Token> tokens, int i) =>
            {
                double x = 1;
                for (int ii = 2; ii <= (Double)tokens[i - 1].Content; ii++) x *= ii;
                return new Token(x);
            });
            public static OperationDel Round = new OperationDel((List<Token> tokens, int i) =>
            {
                return new Token(Math.Round((Double)tokens[i - 1].Content));
            });
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
    }

    class Token
    {
        public TokenType Type { get; private set; }
        public object Content { get; private set; }
        public Token(object content, TokenType type)
        {
            Content = content;
            Type = type;
        }
        public Token(double content)
        {
            Content = content;
            Type = TokenType.Digit;
        }
    }

    enum TokenType
    {
        Digit,
        Operator,
        Sign,
        Variable
    }
}
