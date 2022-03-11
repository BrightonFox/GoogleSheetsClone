//Class defining the members and functions of a formula.

#ifndef FORMULA_H
#define FORMULA_H

//Libraries:

#include <string>
#include <vector>
#include <regex>
#include <unordered_set>
#include <bits/stdc++.h> 
#include <stack>

namespace ttspreadsheet
{
    class Formula
    {
        private:
        std::string FinalFormula = "";
        std::unordered_set<std::string> *FormulaVariables = new std::unordered_set<std::string>();
        std::vector<std::string> Tokens;

        bool isNumber(std::string s);

        public:
        Formula(std::string formula);

        private:
        std::string FindTokenSpec(std::string token);
        bool IsVar(std::string s);

        public:
        Formula(std::string formula, std::string (*normalize)(std::string), bool (*isValid)(std::string));
        
        template <typename T>
        T Evaluate(double (*lookup)(std::string));
        
        std::unordered_set<std::string> GetVariables();
        std::string ToString();
        
        template <typename T>
        bool Equals(const T obj) const;

        bool operator==(const Formula &f2) const;
        bool operator!=(const Formula &f2) const;

        int GetHashCode();

        private:
        const std::vector<std::string> GetTokens(std::string formula);

        public:
        template <typename T>
        bool StackOnTop(std::stack<T> *s, T c);

        void StackAdd(std::stack<double> *s);

        void StackSub(std::stack<double> *s);

    };

    class FormulaFormatException : std::exception
    {
        private:
        std::string message;

        public:
        /// Constructs a FormulaFormatException containing the explanatory message.
        FormulaFormatException(std::string m);

        std::string getMessage();
    };

    struct FormulaError
    {
        std::string Reason;

        public:

        /// Constructs a FormulaError containing the explanatory reason.
        FormulaError(std::string reason);

        ///  The reason why this FormulaError was created.
        std::string getReason();
        
        private:
        void setReason(std::string reason);
    };

}

#endif
