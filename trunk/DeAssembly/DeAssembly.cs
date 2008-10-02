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
using System.Windows.Forms;

namespace DeMIPS
{
    class DeAssembly
    {
        #region variables

        IFrontend activeFrontend;
        IBackend activeBackend;

        #endregion

        #region constuctor

        public DeAssembly()
        {
            activeFrontend = new FrontendMIPS();
            activeBackend = new BackendC();
        }

        #endregion

        #region methods

        public string[] LoadAssemblyFile(string filename)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            LinkedList<string> tempFile = new LinkedList<string>();
            string tempLine;
            string[] assemblyFile;

            while ((tempLine = file.ReadLine()) != null)
                tempFile.AddLast(tempLine.Trim().ToLower());

            file.Close();

            //convert linked list to string array
            assemblyFile = new string[tempFile.Count];
            tempFile.CopyTo(assemblyFile, 0);

            return assemblyFile;
        }

        /// <summary>
        /// Decompiles file. Very hacked up.
        /// </summary>
        /// <param name="assemblyFile">array of strings containting code that will be decompiled.</param>
        /// <param name="TextBoxOutput">GUI TextBox that results will be copied to.</param>
        public void DecompileAssembly(string[] assemblyFile, TextBox TextBoxOutput) //HACK: there should be a better way.
        {
            //string[] assemblyFile = LoadAssemblyFile(filename);
            ProgramBlock newBlock = new ProgramBlock();

            activeFrontend.Preprocess(assemblyFile);
            
            foreach(string line in assemblyFile)
            {
                try
                {
                    newBlock.Program.AddLast(activeFrontend.TranslateLine(line, newBlock));
                }
                catch (Exception err)
                {
                    UtilDebugConsole.AddException(err);
                }
            }

            string[] sourceFile = activeBackend.EmitBlock(newBlock);

            if(!newBlock.IsSane())
                UtilDebugConsole.AddException(new ExceptionWarning(""));


            if (assemblyFile.Length != sourceFile.Length)
                UtilDebugConsole.AddException(new ExceptionWarning("The number of input and output lines differ, GUI may function incorrectly."));

            //update GUI
            TextBoxOutput.Lines = sourceFile;
            TextBoxOutput.Select(0, 0); //HACK: .NET automatically highlights text.
        }
/*
        /// <summary>
        /// This decompiles a single line of MIPS to C. This will be requiring heavy
        /// reworking so I won't document much.
        /// </summary>
        /// <param name="line">Line to decompile.</param>
        private void DecompileLine(ProgramLine line) //TODO: finish moving to FrontendMIPS
        {
            //LABEL
            if (line.Assembly.Contains(":"))
            {
                line.Highlevel = line.Assembly;//TODO: move to C emitter?
            }
            //COMMAND
            else
            {
                string processedLine = ""; //TODO: we should be using an intermediate format before emitting in a highlevel language.

                string lineKeyword = line.Assembly.Split(' ')[0];
                string[] lineParameters = line.Assembly.Substring(line.Assembly.IndexOf(' ') + 1).Split(' ');

                //strip out any tailing ',' characters or starting '$' (var's)
                for (int i = 0; i < lineParameters.Length; i++)
                {
                    lineParameters[i] = lineParameters[i].TrimEnd(',');
                    //lineParameters[i] = lineParameters[i].TrimStart('$');//TODO: Dumb. Once MathParser matures, this should be removed.
                }

                //any $zero's will now be replaced. 
                //TODO: Once MathParser matures, this should be removed.
                for (int i = 0; i < lineParameters.Length; i++)
                    if (lineParameters[i].Equals("$zero"))
                        lineParameters[i] = "0";

                switch (lineKeyword)
                {
                    case "addi": //fall through, MathParser will handle constants.
                    case "add":
                        processedLine = MathParser.SimplifyEqual(lineParameters[0], MathParser.SimplifyArithmetic("+", lineParameters[1], lineParameters[2]));
                        break;

                    case "beql": //fall through. this is suppose to insert NOP before J. V4300i extention.
                    case "beq":
                        processedLine = "if ( " + lineParameters[0] + " == " + lineParameters[1] + " ) goto " + lineParameters[2];
                        break;

                    case "bne":
                        processedLine = "if ( " + lineParameters[0] + " != " + lineParameters[1] + " ) goto " + lineParameters[2];
                        break;

                    case "sllv": //fall through, MathParser will still simplify it.  V4300i extention.
                    case "sll":
                        if (lineParameters[2].Contains("$"))
                            throw new Exception("Unexpected variable.");
                        //HACK: at this point, we know para[2] is a constant. Since this is a bit shift we need to multiple that value by 2.
                        //      the program is that at this point var's and constants are stored as strings. :(
                        //      also, this problem may or may not bork the other v/i opcodes.
                        int tmp = int.Parse(lineParameters[2]) * 2;
                        lineParameters[2] = "" + tmp;
                        processedLine = MathParser.SimplifyEqual(lineParameters[0], MathParser.SimplifyArithmetic("*", lineParameters[1], lineParameters[2]));
                        break;

                    case "subi": //fall through, MathParser will handle constants.
                    case "sub":
                        processedLine = MathParser.SimplifyEqual(lineParameters[0], MathParser.SimplifyArithmetic("-", lineParameters[1], lineParameters[2]));
                        break;

                    case "j":
                        processedLine = "goto " + lineParameters[0];
                        break;

                    case "andi":
                        processedLine = lineParameters[0] + " = " + lineParameters[1] + " & 0x" + lineParameters[2];//assuming immediate is in hex.
                        break;

                    case "ori":
                        processedLine = lineParameters[0] + " = " + lineParameters[1] + " | 0x" + lineParameters[2];//assuming immediate is in hex.
                        break;

#if ENABLE_V4300I_INSTRUCTIONS

                    //FYI: these will actually fall through to the next case.
                    case "sync": // we don't care about sync'ing memory.
                    case "cop0": // command involving coprocessor 0.

#endif //ENABLE_V4300I_INSTRUCTIONS

                    case "syscall": //fall through - we don't need this.
                    case "nop": // fall through - no opcode
                    case "???": //fall through - we don't need this. Disassembler specific.
                        processedLine = "//" + lineKeyword;
                        break;

                }

                if (processedLine.Equals("") && !line.Assembly.Equals(""))
                    //processedLine = "//Unidentified: \"" + line.Assembly + "\"";
                    throw new Exception ("//Unidentified: \"" + line.Assembly + "\"");
                else if (!processedLine.Equals(""))
                    processedLine += ";";//TODO: move to C emitter or ProgramLine

                line.Highlevel = processedLine;
            }
        }
*/

        #endregion

        
    }
}
