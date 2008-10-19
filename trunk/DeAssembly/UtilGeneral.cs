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

using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class UtilGeneral
    {
        #region variables

        static public char[] hexTable = new char[] {'0', '1', '2', '3', '4',
                                                    '5', '6', '7', '8', '9',
                                                    'A', 'B', 'C', 'D', 'E',
                                                    'F'};

        #endregion

        #region methods

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
            int result = 0;

            while (hexadecimal.Length > 0)
            {
                string chunk = hexadecimal.Substring(hexadecimal.Length - 1);
                hexadecimal = hexadecimal.Substring(0, hexadecimal.Length - 1);

                //int.p
            }

            return result;
        }

        /// <summary>
        /// Loads the contents of a text file into an array of strings and returns it.
        /// </summary>
        /// <param name="filename">Text file.</param>
        /// <returns>Array of strings containting the text file.</returns>
        static public string[] LoadTextFile(string filename)
        {
            System.IO.StreamReader fileStream = new System.IO.StreamReader(filename);
            LinkedList<string> fileLines = new LinkedList<string>();
            string tempLine;
            string[] fileArray;

            while ((tempLine = fileStream.ReadLine()) != null)
                fileLines.AddLast(tempLine.Trim().ToLower());

            fileStream.Close();

            //convert linked list to string array
            fileArray = new string[fileLines.Count];
            fileLines.CopyTo(fileArray, 0);

            return fileArray;
        }

        #endregion
    }
}
