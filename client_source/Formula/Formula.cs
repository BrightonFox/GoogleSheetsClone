// Formula Class for storing forumlas and computing them by Nicholas Vaskelis
// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens
//
// File was copied over from Nick's spreadsheet project as a base for CS3505


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private readonly string FinalFormula = "";
        private readonly HashSet<string> FormulaVariables = new HashSet<string>();
        private readonly string[] Tokens;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Finds the specific type of a string for classification purposes.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private string FindTokenSpec(string token)
        {
            if (token.Equals("("))
            {
                return "(";
            }

            else if (token.Equals(")"))
            {
                return ")";
            }

            else if (token.Equals("+") || token.Equals("-") || token.Equals("/") || token.Equals("*"))
            {
                return "Operator";
            }

            else if (Double.TryParse(token, out double num))
            {
                return "Number";
            }

            else
            {
                return "Variable";
            }
        }

        /// <summary>
        /// Returns whether or not the given string is a variable of proper format.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool IsVar(string s)
        {
            String varPattern = @"^[a-zA-Z_](?:[a-zA-Z_]|\d)*$";
            return Regex.IsMatch(s, varPattern);
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            // TokenSpec holds the type of the current token.
            // LastTokenSpec holds the type of the last token evaluated.
            // The types are used to shorten if statements by further classification.
            int OpeningingParentheses = 0;
            int ClosingParentheses = 0;
            string LastTokenSpec = "";

            if (string.IsNullOrWhiteSpace(formula))
            {
                throw new FormulaFormatException("Missing Formula. Please add a valid formula.");
            }

            Tokens = GetTokens(formula).ToArray();

            //Makes sure the ends are valid.
            string FirstTokenSpec = FindTokenSpec(Tokens[0]);
            if (FirstTokenSpec.Equals(")") || FirstTokenSpec.Equals("Operator"))
            {
                throw new FormulaFormatException("Formula must start with a number, a variable, or an opening parenthesis. Try removing the first character of the formula.");
            }

            string FinalTokenSpec = FindTokenSpec(Tokens[Tokens.Length - 1]);
            if (FinalTokenSpec.Equals("(") || FinalTokenSpec.Equals("Operator"))
            {
                throw new FormulaFormatException("Formula must end with a number, a variable, or an closing parenthesis. Try removing the last character of the formula.");
            }

            //Actually iterates through the formula after checking the ends.
            for (int i = 0; i<Tokens.Length; i++)
            {
                string TokenSpec = FindTokenSpec(Tokens[i]);

                //Balance of parentheses.
                if (TokenSpec.Equals("("))
                {
                    OpeningingParentheses++;
                }

                if (TokenSpec.Equals(")"))
                {
                    ClosingParentheses++;
                    if (ClosingParentheses > OpeningingParentheses)
                    {
                        throw new FormulaFormatException("Closing Parenthesis is missing accompaning opening parenthesis.");
                    }
                }

                //Proper Order.
                if ((LastTokenSpec.Equals("(") || LastTokenSpec.Equals("Operator")) && !(TokenSpec.Equals("(") || TokenSpec.Equals("Number") || TokenSpec.Equals("Variable")))
                {
                    throw new FormulaFormatException("Opening parentheses and operators must be followed by a number, variable, or opening parenthesis.");
                }

                if ((LastTokenSpec.Equals(")") || LastTokenSpec.Equals("Number") || LastTokenSpec.Equals("Variable")) && !(TokenSpec.Equals(")") || TokenSpec.Equals("Operator")))
                {
                    throw new FormulaFormatException("Numbers, variables, and opening parentheses must be followed by a closing parenthesis or operator");
                }

                //if the token is a valid variable, adds it to the final formula string as well as the set FormulaVariables to keep track of.
                if (TokenSpec.Equals("Variable"))
                {
                    if (!IsVar(normalize(Tokens[i])))
                    {
                        throw new FormulaFormatException("Formula contains invalid variable after normalization.");
                    }
                    else if (!isValid(normalize(Tokens[i])))
                    {
                        throw new FormulaFormatException("Formula contains invalid variable based on the validator after normalization.");
                    }
                    else
                    {
                        Tokens[i] = normalize(Tokens[i]);
                        FormulaVariables.Add(Tokens[i]);
                    }
                }

                //If the token is a number, normalize and add to stuff.
                if(TokenSpec.Equals("Number"))
                {
                    Tokens[i] = Double.Parse(Tokens[i]).ToString();
                }

                //Every other token gets added.
                LastTokenSpec = TokenSpec;
                FinalFormula += Tokens[i];
            }

            //If parentheses balance is not met.
            if(OpeningingParentheses != ClosingParentheses)
            {
                throw new FormulaFormatException("Opening Parenthesis is missing accompaning closing parenthesis.");
            }

        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<double> vs = new Stack<double>();
            Stack<char> os = new Stack<char>();

            foreach (String t in Tokens)
            {
                string TokenSpec = FindTokenSpec(t);

                //Operations if token is a variable
                if (TokenSpec.Equals("Variable"))
                {
                    double v = 0;
                    try
                    {
                        v = lookup(t);
                    }
                    catch
                    {
                        return new FormulaError("#BADVAR!");
                    }
                    if (os.OnTop('*'))
                    {
                        vs.Push(vs.Pop() * v);
                    }
                    else if (os.OnTop('/'))
                    {
                        if (v == 0)
                            return new FormulaError("#DIV/0!");
                        vs.Push(vs.Pop() / v);
                    }
                    else
                    {
                        vs.Push(v);
                    }
                }

                //Operations if token is a double
                else if (TokenSpec.Equals("Number"))
                {
                    double n = double.Parse(t);
                    if (os.OnTop('*'))
                    {
                        vs.Push(vs.Pop() * n);
                    }
                    else if (os.OnTop('/'))
                    {
                        if (n == 0)
                            return new FormulaError("#DIV/0!");
                        vs.Push(vs.Pop() / n);
                    }
                    else
                    {
                        vs.Push(n);
                    }
                }

                //Operations if token is an operator
                else if (TokenSpec.Equals("Operator") || TokenSpec.Equals("(") || TokenSpec.Equals(")"))
                {
                    char c = char.Parse(t);

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

                        os.Pop();

                        if (os.OnTop('*'))
                        {
                            double b = vs.Pop();
                            double a = vs.Pop();
                            vs.Push(a * b);
                        }
                        else if (os.OnTop('/'))
                        {
                            double b = vs.Pop();
                            double a = vs.Pop();
                            if (b == 0)
                            {
                                return new FormulaError("#DIV/0!");
                            }
                            vs.Push(a / b);
                        }

                    }

                }

            }

            //After all tokens have been processed
            if (os.Count == 0)
            {
                return vs.Pop();
            }

            else
            {
                if (os.Peek().Equals('+'))
                {
                    os.Pop();
                    double b = vs.Pop();
                    double a = vs.Pop();
                    return a + b;
                }
                else
                {
                    os.Pop();
                    double b = vs.Pop();
                    double a = vs.Pop();
                    return a - b;
                }
            }

            

        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return FormulaVariables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return FinalFormula;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if ((obj is null) || !(obj is Formula))
            {
                return false;
            }

            Formula temp = (Formula)obj;

            //Both normalized strings should equal one another if all normalized tokens are equal.
            return FinalFormula.Equals(temp.ToString());
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1 is null && f2 is null)
            {
                return true;
            }
            else if (f1 is null || f2 is null)
            {
                return false;
            }
            else
            {
                return f1.Equals(f2);
            }
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if (f1 is null && f2 is null)
            {
                return false;
            }
            else if (f1 is null || f2 is null)
            {
                return true;
            }
            else
            {
                return !f1.Equals(f2);
            }
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            //2 normalized formulas are equal, therfore the string hashcode of both formulas is also equal.
            return FinalFormula.GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    public static class StackExtensions
    {
        /// <summary>
        /// Finds whether or not the variable c exists on the top of the stack s. Pops if true.
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
        /// Adds two numbers in a stack.
        /// </summary>
        /// <param name="s"></param>
        public static void Add(this Stack<double> s)
        {
            double b = s.Pop();
            double a = s.Pop();
            s.Push(a + b);
        }

        /// <summary>
        /// Subtracts two numbers in a stack.
        /// </summary>
        /// <param name="s"></param>
        public static void Sub(this Stack<double> s)
        {
            double b = s.Pop();
            double a = s.Pop();
            s.Push(a - b);
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}

