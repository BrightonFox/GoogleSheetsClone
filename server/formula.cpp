//Class implementing the members and functions of a formula.

#include <string>
#include <vector>
#include <boost/regex.hpp>
#include <unordered_set>
#include <bits/stdc++.h> 
#include <stack>
#include <boost/functional/hash.hpp>
#include <typeinfo> 
#include "formula.h"

namespace ttspreadsheet
{
        bool Formula::isNumber(std::string s)
        {
            for (int i = 0; i < s.length(); i++)
                if (isdigit(s[i]) == false && s[i] != '.')
                    return false;

            return true;
        }

        Formula::Formula(std::string formula)
        {
            Formula(formula, [](std::string s) { return s; }, [](std::string s) { return true; });
        }

        std::string Formula::FindTokenSpec(std::string token)
        {
            if (token == "(")
            {
                return "(";
            }

            else if (token == ")")
            {
                return ")";
            }

            else if (token == "+" || token == "-" || token == "/" || token == "*")
            {
                return "Operator";
            }

            else if (isNumber(token))
            {
                return "Number";
            }

            else
            {
                return "Variable";
            }
        }

        bool Formula::IsVar(std::string s)
        {
            boost::regex varPattern("^[a-zA-Z_](?:[a-zA-Z_]|\\d)*$");
            return boost::regex_match(s, varPattern);
        }

        Formula::Formula(std::string formula, std::string (*normalize)(std::string), bool (*isValid)(std::string))
        {
            // TokenSpec holds the type of the current token.
            // LastTokenSpec holds the type of the last token evaluated.
            // The types are used to shorten if statements by further classification.
            int OpeningingParentheses = 0;
            int ClosingParentheses = 0;
            std::string LastTokenSpec = "";

            if (formula.empty() || std::all_of(formula.begin(),formula.end(),isspace))
            {
                throw FormulaFormatException("Missing Formula. Please add a valid formula.");
            }

            Tokens = GetTokens(formula);

            //Makes sure the ends are valid.
            std::string FirstTokenSpec = FindTokenSpec(Tokens[0]);
            if (FirstTokenSpec == ")" || FirstTokenSpec == "Operator")
            {
                throw FormulaFormatException("Formula must start with a number, a variable, or an opening parenthesis. Try removing the first character of the formula.");
            }

            std::string FinalTokenSpec = FindTokenSpec(Tokens[Tokens.size() - 1]);
            if (FinalTokenSpec == "(" || FinalTokenSpec == "Operator")
            {
                throw FormulaFormatException("Formula must end with a number, a variable, or an closing parenthesis. Try removing the last character of the formula.");
            }

            //Actually iterates through the formula after checking the ends.
            for (int i = 0; i<Tokens.size(); i++)
            {
                std::string TokenSpec = FindTokenSpec(Tokens[i]);

                //Balance of parentheses.
                if (TokenSpec == "(")
                {
                    OpeningingParentheses++;
                }

                if (TokenSpec == ")")
                {
                    ClosingParentheses++;
                    if (ClosingParentheses > OpeningingParentheses)
                    {
                        throw FormulaFormatException("Closing Parenthesis is missing accompaning opening parenthesis.");
                    }
                }

                //Proper Order.
                if ((LastTokenSpec == "(" || LastTokenSpec == "Operator") && !(TokenSpec == "(" || TokenSpec == "Number" || TokenSpec == "Variable"))
                {
                    throw FormulaFormatException("Opening parentheses and operators must be followed by a number, variable, or opening parenthesis.");
                }

                if ((LastTokenSpec == ")" || LastTokenSpec == "Number" || LastTokenSpec == "Variable") && !(TokenSpec == ")" || TokenSpec == "Operator"))
                {
                    throw FormulaFormatException("Numbers, variables, and opening parentheses must be followed by a closing parenthesis or operator");
                }

                //if the token is a valid variable, adds it to the final formula string as well as the set FormulaVariables to keep track of.
                if (TokenSpec == "Variable")
                {
                    if (!IsVar(normalize(Tokens[i])))
                    {
                        throw FormulaFormatException("Formula contains invalid variable after normalization.");
                    }
                    else if (!isValid(normalize(Tokens[i])))
                    {
                        throw FormulaFormatException("Formula contains invalid variable based on the validator after normalization.");
                    }
                    else
                    {
                        Tokens[i] = normalize(Tokens[i]);
                        FormulaVariables->insert(Tokens[i]);
                    }
                }

                //If the token is a number, normalize and add to stuff.
                if(TokenSpec == "Number")
                {
                    Tokens[i] = std::to_string(std::stod(Tokens[i]));
                }

                //Every other token gets added.
                LastTokenSpec = TokenSpec;
                FinalFormula += Tokens[i];
            }

            //If parentheses balance is not met.
            if(OpeningingParentheses != ClosingParentheses)
            {
                throw FormulaFormatException("Opening Parenthesis is missing accompaning closing parenthesis.");
            }
        }
        
        template <typename T>
        T Formula::Evaluate(double (*lookup)(std::string))
        {
            std::stack<double> vs;
            std::stack<char> os;

            for (std::string t : Tokens)
            {
                std::string TokenSpec = FindTokenSpec(t);

                //Operations if token is a variable
                if (TokenSpec == "Variable")
                {
                    double v = 0;
                    try
                    {
                        v = lookup(t);
                    }
                    catch(int i)
                    {
                        return new FormulaError("#BADVAR!");
                    }
                    if (StackOnTop<char>(&os, '*'))
                    {
                        double temp = (vs.top() * v);
                        vs.pop();
                        vs.push(temp);
                    }
                    else if (StackOnTop<char>(&os, '/'))
                    {
                        if (v == 0)
                            return new FormulaError("#DIV/0!");
                        double temp = (vs.top() / v);
                        vs.pop();
                        vs.push(temp);
                    }
                    else
                    {
                        vs.push(v);
                    }
                }

                //Operations if token is a double
                else if (TokenSpec == "Number")
                {
                    double n = std::stod(t);
                    if (StackOnTop<char>(&os, '*'))
                    {
                        double temp = (vs.top() * n);
                        vs.pop();
                        vs.push(temp);
                        
                    }
                    else if (StackOnTop<char>(&os, '/'))
                    {
                        if (n == 0)
                            return new FormulaError("#DIV/0!");
                        double temp = (vs.top() / n);
                        vs.pop();
                        vs.push(temp);
                    }
                    else
                    {
                        vs.push(n);
                    }
                }

                //Operations if token is an operator
                else if (TokenSpec == "Operator" || TokenSpec == "(" || TokenSpec == ")")
                {
                    char c = t[0];

                    if (c == '*' || c == '/')
                    {
                        os.push(c);
                    }

                    else if (c == '+' || c == '-')
                    {
                        if (StackOnTop<char>(&os, '+'))
                        {
                            StackAdd(&vs);
                        }
                        else if (StackOnTop<char>(&os, '-'))
                        {
                            StackSub(&vs);
                        }
                        os.push(c);
                    }

                    else if (c == '(')
                    {
                        os.push(c);
                    }

                    else if (c == ')')
                    {
                        if (StackOnTop<char>(&os, '+'))
                        {
                            StackAdd(&vs);
                        }
                        else if (StackOnTop<char>(&os, '-'))
                        {
                            StackSub(&vs);
                        }

                        os.pop();

                        if (StackOnTop<char>(&os, '*'))
                        {
                            double b = vs.top();
                            vs.pop();
                            double a = vs.top();
                            vs.pop();
                            vs.push(a * b);
                        }
                        else if (StackOnTop<char>(&os, '/'))
                        {
                            double b = vs.top();
                            vs.pop();
                            double a = vs.top();
                            vs.pop();
                            if (b == 0)
                            {
                                return new FormulaError("#DIV/0!");
                            }
                            vs.push(a / b);
                        }

                    }

                }

            }

            //After all tokens have been processed
            if (os.size() == 0)
            {
                return vs.pop();
            }

            else
            {
                if (os.top() == '+')
                {
                    os.pop();
                    double b = vs.top();
                    vs.pop();
                    double a = vs.top();
                    vs.pop();
                    return a + b;
                }
                else
                {
                    os.pop();
                    double b = vs.top();
                    vs.pop();
                    double a = vs.top();
                    vs.pop();
                    return a - b;
                }
            }
        }
        
        std::unordered_set<std::string> Formula::GetVariables()
        {
            return *FormulaVariables;
        }

        std::string Formula::ToString()
        {
            return FinalFormula;
        }
        
        template <typename T>
        bool Formula::Equals(const T obj) const
        {
            bool typecheck = typeid(obj).name() == typeid(Formula).name();
            if(&obj == NULL || !typecheck)
            {
                return false;
            }

            Formula temp = (Formula)obj;

            //Both normalized strings should equal one another if all normalized tokens are equal.
            return FinalFormula == temp.ToString();

        }

        bool Formula::operator==(const Formula &f2) const
        {
            if (this == NULL && &f2 == NULL)
            {
                return true;
            }
            else if (this == NULL || &f2 == NULL)
            {
                return false;
            }
            else
            {
                return this->Equals(f2);
            }
        }

        bool Formula::operator!=(const Formula &f2) const
        {
            if (this == NULL && &f2 == NULL)
            {
                return false;
            }
            else if (this == NULL || &f2 == NULL)
            {
                return true;
            }
            else
            {
                return !(this->Equals(f2));
            }
        }

        int Formula::GetHashCode()
        {
            boost::hash<std::string> hash;
            return hash(FinalFormula);
        }

        const std::vector<std::string> Formula::GetTokens(std::string formula)
        {
            std::vector<std::string> output;

            // Patterns for individual tokens
            std::string lpPattern("\\(");
            std::string rpPattern("\\)");
            std::string opPattern("[\\+\\-*/]");
            std::string varPattern("[a-zA-Z_](?: [a-zA-Z_]|\\d)*");
            std::string doublePattern("(?: \\d+\\.\\d* | \\d*\\.\\d+ | \\d+ ) (?: [eE][\\+-]?\\d+)?");
            std::string spacePattern("\\s+");
            std::string combinedPattern = "("+lpPattern+") | ("+rpPattern+") | ("+opPattern+") | ("+varPattern+") | ("
            +doublePattern+") | ("+spacePattern+")";

            // Overall pattern
            boost::regex pattern(combinedPattern);

            boost::regex_token_iterator<std::string::iterator> iterator(formula.begin(), formula.end(), pattern);

            // Enumerate matching tokens that don't consist solely of white space.
            while (iterator != boost::regex_token_iterator<std::string::iterator>())
            {
                std::string s = iterator->str();
                if (!(s.empty() || std::all_of(s.begin(),s.end(),isspace)))
                {
                    output.push_back(s);
                }
            }

            return output;
        }

        template <typename T>
        bool Formula::StackOnTop(std::stack<T> *s, T c)
        {      
            if (s->size() < 1)
            {
                return false;
            }

            else
            {
                if (s->top() == c)
                {
                    s->pop();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        
        }

        void Formula::StackAdd(std::stack<double> *s)
        {
            double b = s->top();
            s->pop();
            double a = s->top();
            s->pop();
            s->push(a + b);
        }

        void Formula::StackSub(std::stack<double> *s)
        {
            double b = s->top();
            s->pop();
            double a = s->top();
            s->pop();
            s->push(a - b);
        }


    /// Constructs a FormulaFormatException containing the explanatory message.
    FormulaFormatException::FormulaFormatException(std::string m)
    {
        message = m;
    }

    std::string FormulaFormatException::getMessage()
    {
        return message;
    }

    /// Constructs a FormulaError containing the explanatory reason.
    FormulaError::FormulaError(std::string reason)
    {
        Reason = reason;
    }

    ///  The reason why this FormulaError was created.
    std::string FormulaError::getReason()
    {
        return Reason;
    }

    void FormulaError::setReason(std::string reason)
    {
        Reason = reason;
    }
    

}
