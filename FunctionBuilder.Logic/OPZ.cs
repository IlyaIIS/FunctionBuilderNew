﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FunctionBuilder
{
    static public class OPZ
    {
        //Константы
        static string[] signsList = { "+", "-", "*", "/", "^", "(", ")", "sin", "log", "!", "round" };

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

        //Получить польскую запсь
        static public List<string> ParseExpression(string input)
        {
            string[] pInput = ParseInput(input);
            List<string> firstList = new List<string>();
            List<string> secondList = new List<string>();
            bool isDebugEnabled = false;

            for (int i = 0; i < pInput.Length; i++)
            {
                if (Char.IsDigit(pInput[i][0])) firstList.Add(pInput[i]);
                else
                if (pInput[i][0] == '(') secondList.Add(pInput[i]);
                else
                if (pInput[i][0] == ')')
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
                if (signsList.Contains(pInput[i]))
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

        //Выделить в строке цифры и знаки
        static string[] ParseInput(string input)
        {
            List<string> preOutput = new List<string>();
            string[] output;
            input += "  ";

            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsDigit(input[i]))
                {
                    preOutput.Add("");
                    for (; i < input.Length; i++)
                    {
                        if (Char.IsDigit(input[i]) || input[i] == '.')
                            
                        {
                            preOutput[preOutput.Count - 1] += input[i];
                        }
                        else
                        {
                            if (input[i] == 'E' && (input[i + 1] == '+' || input[i + 1] == '-'))
                            {
                                preOutput[preOutput.Count - 1] += input[i];
                                preOutput[preOutput.Count - 1] += input[i + 1];
                                i++;
                            }
                            else
                            {
                                i--;
                                break;
                            }
                        }
                    }
                }
                else
                if (signsList.Contains(Convert.ToString(input[i])))
                {
                    if (input[i] == '-')
                        if (i > 0)
                        {
                            if (!Char.IsDigit(input[i - 1]) && (input[i - 1] == '(' || input[i - 1] != ')'))
                                preOutput.Add("0");
                        }
                        else
                            preOutput.Add("0");
                    preOutput.Add(Convert.ToString(input[i]));
                }
                else
                if (input[i] != ' ' && input[i] != ',' && input[i] != ';')
                {
                    string local = "";
                    for (;; i++)
                    {
                        local += input[i];
                        if (signsList.Contains(Convert.ToString(local))) break;
                    }
                    preOutput.Add(local);
                }
            }

            output = new string[preOutput.Count];
            for (int i = 0; i < output.Length; i++) output[i] = preOutput[i];

            return output;
        }
        
        //Калькулятор
        static public double Calculate(List<string> input)
        {
            for (int i = 0; i < input.Count; i++)
            {
                if (signsList.Contains(input[i]))
                {
                    switch (input[i])
                    {
                        case "+":
                            input[i] = Convert.ToString(Convert.ToDouble(input[i - 2]) + Convert.ToDouble(input[i - 1]));
                            input.RemoveAt(i - 1);
                            input.RemoveAt(i - 2);
                            i -= 2;
                            break;
                        case "-":
                            input[i] = Convert.ToString(Convert.ToDouble(input[i - 2]) - Convert.ToDouble(input[i - 1]));
                            input.RemoveAt(i - 1);
                            input.RemoveAt(i - 2);
                            i -= 2;
                            break;
                        case "*":
                            input[i] = Convert.ToString(Convert.ToDouble(input[i - 2]) * Convert.ToDouble(input[i - 1]));
                            input.RemoveAt(i - 1);
                            input.RemoveAt(i - 2);
                            i -= 2;
                            break;
                        case "/":
                            input[i] = Convert.ToString(Convert.ToDouble(input[i - 2]) / Convert.ToDouble(input[i - 1]));
                            input.RemoveAt(i - 1);
                            input.RemoveAt(i - 2);
                            i -= 2;
                            break;
                        case "^":
                            input[i] = Convert.ToString(Math.Pow(Convert.ToDouble(input[i - 2]), Convert.ToDouble(input[i - 1])));
                            input.RemoveAt(i - 1);
                            input.RemoveAt(i - 2);
                            i -= 2;
                            break;
                        case "sin":
                            input[i] = Convert.ToString(Math.Sin(Convert.ToDouble(input[i - 1])));
                            input.RemoveAt(i - 1);
                            i -= 1;
                            break;
                        case "log":
                            input[i] = Convert.ToString(Math.Log10(Convert.ToDouble(input[i - 2])) / Math.Log10((Convert.ToDouble(input[i - 1]))));
                            input.RemoveAt(i - 1);
                            input.RemoveAt(i - 2);
                            i -= 2;
                            break;
                        case "!":
                            double x = 1;
                            for (int ii = 2; ii <= Convert.ToInt32(input[i - 1]); ii++) x *= ii;
                            input[i] = Convert.ToString(x);
                            input.RemoveAt(i - 1);
                            i -= 1;
                            break;
                        case "round":
                            input[i] = Convert.ToString(Math.Round(Convert.ToDouble(input[i - 1])));
                            input.RemoveAt(i - 1);
                            i -= 1;
                            break;
                    }
                }
            }

            return Convert.ToDouble(input[0]);
        }

        static public bool IsExpressionCorrectly(string formula, out string errorText)
        {
            errorText = String.Empty;

            if (formula == null || formula.Length == 0)
            {
                errorText = "Формула не введина";
                return false;
            }

            if (formula.Length == 1)
            {
                errorText = "Формула слишком коротка";
                return false;
            }

            int bracketNum = 0;
            foreach (char letter in formula)
            {
                if (letter == '(') bracketNum++;
                if (letter == ')') bracketNum--;
            }
            if (bracketNum > 0)
            {
                errorText = "Не все скобки закрыты";
                return false;
            }
            if (bracketNum < 0)
            {
                errorText = "Закрывающих скобок больше, чем открывающих";
                return false;
            }

            for (int i = 0; i < formula.Length; i++)
            {
                char letter = formula[i];
                if (Char.IsDigit(letter) || letter == 'x')
                {
                    var neigList = new List<char>();
                    if (i == 0)
                        neigList.Add(formula[i + 1]);
                    else
                        if (i == formula.Length - 1)
                        neigList.Add(formula[i - 1]);
                        else
                            neigList = new List<char> { formula[i - 1], formula[i + 1] };

                    foreach (char neig in neigList)
                    {
                        char[] signArr = new char[] { ',', '.', '(', ')', '+', '-', '*', '/', '^', '!' };
                        if (!(Char.IsDigit(neig) || signArr.Contains(neig)))
                        {
                            errorText = "Неизвестный символ в формуле";
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
