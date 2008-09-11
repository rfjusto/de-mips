using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeAssembly
{
    class MathParser
    {
        //assume only two values and one operand (+, -, *, or /). Only removes zeros right now.
        static public string SimplifyArithmetic(string operand, string first, string second)
        {
            string finalExpression = "";

            //0 OP 0
            if (first.Equals("0") && second.Equals("0"))
            {
                switch (operand)
                {
                    case "+":
                        finalExpression = "0";
                        break;
                    default:
                        throw new Exception("Invalid operand detected in SimplifyExpression!");
                }
            }
            //A OP 0
            else if ((!first.Equals("0")) && second.Equals("0"))
            {
                switch (operand)
                {
                    case "+":
                        finalExpression = first;
                        break;
                    default:
                        throw new Exception("Invalid operand detected in SimplifyExpression!");
                }
            }
            //0 OP A
            else if (first.Equals("0") && (!second.Equals("0")))
            {
                switch (operand)
                {
                    case "+":
                        finalExpression = second;
                        break;
                    default:
                        throw new Exception("Invalid operand detected in SimplifyExpression!");
                }
            }
            //A OP B
            else
                finalExpression = first + " " + operand + " " + second;


            return finalExpression;
        }

        static public string SimplifyEqual(string left, string right)
        {
            string finalEquals = "";
            string[] rightTokens = right.ToLower().Split(' ');

            //has two terms, FYI third token is for operand.
            if (rightTokens.Length > 1 && rightTokens.Length < 4)
            {
                // +=, -=, *=, /=
                if (left.Equals(rightTokens[0]))
                    finalEquals = left + " " + rightTokens[1] + "= " + rightTokens[2];
            }

            if (rightTokens.Length == 3 && left.Equals(rightTokens[0]) && rightTokens[2].Equals("1"))
                if (rightTokens[1].Equals("-"))
                    finalEquals = left + "--";
                else
                    finalEquals = left + "++";
            //(rightTokens[1].Equals("-" || rightTokens[1].Equals("+")) && rightTokens[2].Equals("1")
            {

            }

            //failed to simplify
            if (finalEquals.Equals(""))
                finalEquals = left + " = " + right;

            return finalEquals;
        }
    }
}
