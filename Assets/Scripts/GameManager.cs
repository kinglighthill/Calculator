using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using System.Linq;

public class GameManager : MonoBehaviour
{

    public TMP_Text inputText, answerText;

    private MathsParser parser;

    private string input;

    // Start is called before the first frame update
    void Start()
    {
        parser = new MathsParser();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Clear()
    {
        input = "";
        inputText.text = input;

    }

    public void PlusMinus()
    {

    }

    public void Percent()
    {
        input += "%";
        inputText.text = input;
    }

    public void Div()
    {
        input += "/";
        inputText.text = input;
    }

    public void Mul()
    {
        input += "*";
        inputText.text = input;
    }

    public void Minus()
    {
        input += "-";
        inputText.text = input;
    }

    public void Plus()
    {
        input += "+";
        inputText.text = input;
    }

    public void Zero()
    {
        input += "0";
        inputText.text = input;

    }

    public void One()
    {
        input += "1";
        inputText.text = input;
    }

    public void Two()
    {
        input += "2";
        inputText.text = input;
    }

    public void Three()
    {
        input += "3";
        inputText.text = input;
    }

    public void Four()
    {
        input += "4";
        inputText.text = input;
    }

    public void Five()
    {
        input += "5";
        inputText.text = input;
    }

    public void Six()
    {
        input += "6";
        inputText.text = input;
    }

    public void Seven()
    {
        input += "7";
        inputText.text = input;
    }

    public void Eight()
    {
        input += "8";
        inputText.text = input;
    }

    public void Nine()
    {
        input += "9";
        inputText.text = input;
    }

    public void Decimal()
    {
        input += ".";
        inputText.text = input;
    }

    public void EqualTo()
    {
        string answer = "";
        try
        {
            parser.Parse(input);
            answer = parser.Evaluate();
        } catch (Exception e)
        {
            Debug.Log(e.ToString());
        }

        answerText.text = answer;
    }


    class MathsParser
    {
        private string answer = "";

        private string[] FUNCTIONS = { "pm", "percent" };
        private const string OPERATORS = "+-*/";

        //private NumberForma

        private Stack<string> stackOperations = new Stack<string>();
        private Stack<string> stackRPN = new Stack<string>();
        private Stack<string> stackAnswer = new Stack<string>();

        public string Answer
        {
            get
            {
                return answer;
            }
            set
            {
                answer = value;
            }
        }

        public void Parse(string expression)
        {

            stackOperations.Clear();
            stackRPN.Clear();

            if (expression.Length > 0)
            {
                if (expression.ElementAt(0) == '-' || expression.ElementAt(0) == '+')
                {
                    expression = "0" + expression;
                }
            }
            string[] tokens = Regex.Split(expression, @"([*()\^\/]|(?<!E)[\+\-])");

            foreach (string token in tokens)
            {
                if (IsNumber(token))
                {
                    Debug.Log("Number: " + token);
                    stackRPN.Push(token);
                }
                else if (IsOperator(token))
                {
                    while (stackOperations.Count != 0 && IsOperator(stackOperations.Last()) && GetPrecedence(token) <=
                            GetPrecedence(stackOperations.Last()))
                    {
                        stackRPN.Push(stackOperations.Pop());
                    }
                    stackOperations.Push(token);
                }
                else if (IsFunction(token))
                {
                    stackOperations.Push(token);
                }
                else
                {
                    throw new Exception("Unrecognized token: " + token);
                }
            }
 
            while (stackOperations.Count != 0)
            {
                stackRPN.Push(stackOperations.Pop());
            }


            Stack<string> revStackRPN = new Stack<string>();
            while (stackRPN.Count != 0)
            {
                revStackRPN.Push(stackRPN.Pop());
            }

            stackRPN = revStackRPN;

            string log = "";
            foreach (string s in stackRPN)
            {
                log += s;
            }
            Debug.Log(log);
        }

        public string Evaluate()
        {
            if (stackRPN.Count == 0)
            {
                return "";
            }

            stackAnswer.Clear();

            Stack<string> stackRPNClone = stackRPN;

            while (stackRPNClone.Count != 0)
            {
                string token = stackRPNClone.Pop();

                if (IsNumber(token))
                {
                    stackAnswer.Push(token);
                }
                else if (IsOperator(token))
                {
                    Nullable<double> a = stackAnswer.Count == 0 ? null : Double.Parse(stackAnswer.Pop());
                    Nullable<double> b = stackAnswer.Count == 0 ? null : Double.Parse(stackAnswer.Pop());
                    if (b.HasValue)
                    {
                        double c;
                        switch (token)
                        {
                            case "+":
                                stackAnswer.Push((b + a).ToString());
                                break;
                            case "-":
                                stackAnswer.Push((b - a).ToString());
                                break;
                            case "*":
                                stackAnswer.Push((b * a).ToString());
                                break;
                            case "/":
                                c = b.Value / a.Value;
                                string s = (c).ToString();
                                stackAnswer.Push(s);
                                break;
                        }
                    }
                }
                else if (IsFunction(token))
                {
                    if (stackAnswer.Count > 0)
                    {
                        Double a = Double.Parse(stackAnswer.Pop());
                        switch (token)
                        {
                            case "percent":
                                stackAnswer.Push((a * 0.01).ToString());
                                break;
                        }
                    }
                }
            }

            int stackSize = stackAnswer.Count;

            
            if (stackSize > 0)
            {
                answer = stackAnswer.Pop();
                return answer;
            }
            else
            {
                return answer;
            }
        }

        private bool IsNumber(String token)
        {
            try
            {
                Double.Parse(token);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        private bool IsFunction(String token)
        {
            foreach (String item in FUNCTIONS)
            {
                if (item.Equals(token))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsOperator(String token)
        {
            return OPERATORS.Contains(token);
        }

        private byte GetPrecedence(String token)
        {
            if (token.Equals("+") || token.Equals("-"))
            {
                return 1;
            }
            else if (token.Equals("*") || token.Equals("/"))
            {
                return 2;
            }

            return 3;
        }

    }
}
