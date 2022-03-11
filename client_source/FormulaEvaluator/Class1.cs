using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

/* This Program is designed to evaluate in-fix expressions for the spreadsheet program. Relies on a Lookup delegate to process cell value information.
 * 
 * Author: Nicholas Vaskelis
 * Date: 9/3/20
 */
//
// File was copied over from Nick's spreadsheet project as a base for CS3505

namespace FormulaEvaluator
{
    public delegate int Lookup(string v);

    /// <summary>
    /// Holds methods for stacks. Mainly checking stacks along with math.
    /// </summary>
    public static class StackExtensions
    {
        /// <summary>
        /// Finds whether or not the variable c exists on the top of the stack s.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool OnTop<T>(this Stack<T> s, T c)
        {
            if (s.Count < 1)
            {
                return false;
            }

            else
            {
                if (s.Peek().Equals(c))
                {
                    s.Pop();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Adds two numbers in a stack. Throws argument exception.
        /// </summary>
        /// <param name="s"></param>
        public static void Add(this Stack<int> s)
        {
            if (s.Count < 2)
            {
                throw new ArgumentException("Can't add something and nothing.");
            }
            int b = s.Pop();
            int a = s.Pop();
            s.Push(a + b);
        }

        /// <summary>
        /// Subtracts two numbers in a stack. Throws argument exception.
        /// </summary>
        /// <param name="s"></param>
        public static void Sub(this Stack<int> s)
        {
            if (s.Count < 2)
            {
                throw new ArgumentException("Can't subtract something from nothing.");
            }
            int b = s.Pop();
            int a = s.Pop();
            s.Push(a - b);
        }

        /// <summary>
        /// Multiplies two numbers in a stack. Throws argument exception.
        /// </summary>
        /// <param name="s"></param>
        public static void Mult(this Stack<int> s)
        {
            if (s.Count < 2)
            {
                throw new ArgumentException("Can't multiply something and nothing.");
            }
            int b = s.Pop();
            int a = s.Pop();
            s.Push(a * b);
        }

        /// <summary>
        /// Divides two numbers in a stack. Throws argument exception.
        /// </summary>
        /// <param name="s"></param>
        public static void Div(this Stack<int> s)
        {
            if (s.Count < 2)
            {
                throw new ArgumentException("Can't divide something from nothing.");
            }
            int b = s.Pop();
            int a = s.Pop();
            if (b == 0)
            {
                throw new ArgumentException("Can't divide by zero.");
            }
            s.Push(a / b);
        }
    }

    

    /// <summary>
    /// Contains evaluation methods.
    /// </summary>
    public static class Evaluator
    {
        private static Stack<int> vs = new Stack<int>();
        private static Stack<char> os = new Stack<char>();

        /// <summary>
        /// Checks if a string is the right format for a variable.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static bool isVar(string s)
        {
            bool fl = false;
            bool fd = false;

            int i;
            for(i = 0; i < s.Length; i++)
            {
                if (Char.IsLetter(s[i]))
                    fl = true;
                else
                    break;
            }

            for (; i< s.Length; i++)
            {
                if (Char.IsDigit(s[i]))
                    fd = true;
                else
                    break;
            }

            return fl && fd;
        }

        /// <summary>
        /// Evaluation operations done for integers and variables. Throws argument exception.
        /// </summary>
        /// <param name="x"></param>
        static void IntOp(int x)
        {
            if (os.OnTop('*'))
            {
                if (vs.Count == 0)
                    throw new ArgumentException("Can't multiply nothing.");
                vs.Push(vs.Pop() * x);
            }
            else if (os.OnTop('/'))
            {
                if (vs.Count == 0)
                    throw new ArgumentException("Can't divide nothing.");
                if (x == 0)
                    throw new ArgumentException("Can't divide by zero.");
                vs.Push(vs.Pop() / x);
            }
            else
            {
                vs.Push(x);
            }
        }

        /// <summary>
        /// Evaluates an infix expression and returns an int answer. Throws argument exception.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        public static int Evaluate(string exp, Lookup variableEvaluator)
        {
            if(string.IsNullOrWhiteSpace(exp))
            {
                throw new ArgumentException("Input string cannot be null or whitespace.");
            }

            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            foreach (String x in substrings)
            {
                string t = x.Trim();

                //Empty token
                if (t.Length < 1)
                {
                    continue;
                }

                //Operations if token is a variable
                else if (isVar(t))
                {
                    int v = variableEvaluator(t);
                    IntOp(v);
                }

                //Operations if token is an integer
                else if (int.TryParse(t, out int n))
                {
                    IntOp(n);
                }

                //Operations if token is an operator
                else if (char.TryParse(t, out char c))
                {

                    if (c.Equals('*') || c.Equals('/'))
                    {
                        os.Push(c);
                    }

                    else if (c.Equals('+') || c.Equals('-'))
                    {
                        if (os.OnTop('+'))
                        {
                            vs.Add();
                        }
                        else if (os.OnTop('-'))
                        {
                            vs.Sub();
                        }
                        os.Push(c);
                    }

                    else if (c.Equals('('))
                    {
                        os.Push(c);
                    }

                    else if (c.Equals(')'))
                    {
                        if (os.OnTop('+'))
                        {
                            vs.Add();
                        }
                        else if (os.OnTop('-'))
                        {
                            vs.Sub();
                        }

                        if (os.Count != 0 && os.Peek().Equals('('))
                            os.Pop();
                        else
                            throw new ArgumentException("Missing left parenthesis.");

                        if (os.OnTop('*'))
                        {
                            vs.Mult();
                        }
                        else if (os.OnTop('/'))
                        {
                            vs.Div();
                        }

                    }

                    //If token is any of the above
                    else
                    {
                        throw new ArgumentException("Bad character in input.");
                    }

                }

                else
                {
                    throw new ArgumentException("Invalid variable in input.");
                }

            }

            

            //After all tokens have been processed
            if(os.Count == 0)
            {
                if(vs.Count != 1)
                {
                    throw new ArgumentException("Missing operator in input.");
                }

                return vs.Pop();
            }

            else
            {
                if (os.Count != 1)
                {
                    throw new ArgumentException("Overlapping operators in input.");
                }

                if (vs.Count != 2)
                {
                    throw new ArgumentException("Missing operator in input.");
                }

                if (os.Peek().Equals('+'))
                {
                    os.Pop();
                    int b = vs.Pop();
                    int a = vs.Pop();
                    return a + b;
                }
                else if (os.Peek().Equals('-'))
                {
                    os.Pop();
                    int b = vs.Pop();
                    int a = vs.Pop();
                    return a - b;
                }
                else
                {
                    throw new ArgumentException("Invalid operator placement.");
                }
            }

            throw new ArgumentException("Something went wrong when finalizing answer.");
        }

    }
}
