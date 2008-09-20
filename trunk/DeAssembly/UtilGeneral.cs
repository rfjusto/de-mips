using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class UtilGeneral
    {

        static public char[] hexTable = new char[] {'0', '1', '2', '3', '4',
                                                    '5', '6', '7', '8', '9',
                                                    'A', 'B', 'C', 'D', 'E',
                                                    'F'};

        /// <summary>
        /// Converts a integer to a string by returning its hexadecimal value
        /// as a string. The form will be "0xHHHH".
        /// </summary>
        /// <param name="number">Integer to convert.</param>
        /// <returns>String containing hexadecimal verions of input.</returns>
        static public string DecimalToHexadecimal(int number)
        {
                string hexadecimal = "0x";
                int digit;


                while (number > 0)
                {
                    digit = number % 16;
                    hexadecimal += hexTable[digit];
                    number = (number >> 4);
                }
            
                return hexadecimal;   
        }

        //TODO: Implement me!
        static public int HexadecimalToDecimal(string hexadecimal)
        {
            return 0;
        }
    }
}
