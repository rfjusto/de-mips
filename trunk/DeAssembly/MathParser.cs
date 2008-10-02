/////////////////////////////////////////////////////////////////////////
//                                                                     //
//    DeMIPS - A MIPS decompiler                                       //
//                                                                     //
//        Copyright (c) 2008 by Ruben Acuna and Michael Bradley        //
//                                                                     //
// This file is part of DeMIPS.                                        //
//                                                                     //
// DeMIPS is free software; you can redistribute it and/or             //
// modify it under the terms of the GNU Lesser General Public          //
// License as published by the Free Software Foundation; either        //
// version 3 of the License, or (at your option) any later version.    //
//                                                                     //
// This library is distributed in the hope that it will be useful,     //
// but WITHOUT ANY WARRANTY; without even the implied warranty of      //
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the       //
// GNU Lesser General Public License for more details.                 //
//                                                                     //
// You should have received a copy of the GNU Lesser General Public    //
// License along with this library; if not, write to the Free Software //
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA           //
// 02111-1307, USA, or contact the author(s):                          //
//                                                                     //
// Ruben Acuna <flyingfowlsoftware@earthlink.net>                      //
//                                                                     //
/////////////////////////////////////////////////////////////////////////

//DEBUG SETTINGS
#define ENABLE__SIMPLIFY_ARITHMETIC
#define ENABLE__SIMPLIFY_EQUAL

using System;
using System.Collections.Generic;
using System.Text;

//NOTE: this class is currently unused.
namespace DeMIPS
{
    class MathParser
    {
        //assume only two values and one operand (+, -, *, or /). Only removes zeros right now.
        static public string SimplifyArithmetic(string operand, string first, string second)
        {
            string finalExpression = first + " " + operand + " " + second; //default

            #if ENABLE__SIMPLIFY_ARITHMETIC

            //0 OP 0
            if (first.Equals("0") && second.Equals("0"))
            {
                switch (operand)
                {
                    case "+":
                        finalExpression = "0";
                        break;
                    default:
                        throw new Exception("Math Parser: Invalid operand in SimplifyExpression with (0 OP 0) with " + finalExpression);
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
                        throw new Exception("MathParser - Invalid operand in SimplifyExpression (A OP 0) with " + finalExpression);
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
                    case "*":
                        finalExpression = "0";
                        break;
                    default:
                        throw new Exception("MathParser - Invalid operand in SimplifyExpression (0 OP A) with " + finalExpression);
                }
            }
            //TODO: the final case, A OP B, cannot be simplified unless they are both constants.
            #endif

            return finalExpression;
        }

        static public string SimplifyEqual(string left, string right)
        {
            string finalEquals = left + " = " + right; //default

            #if ENABLE__SIMPLIFY_EQUAL
            string[] rightTokens = right.Split(' ');

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
            #endif

            return finalEquals;
        
        }
    }
}
