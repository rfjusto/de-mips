/////////////////////////////////////////////////////////////////////////
//                                                                     //
//    DeMIPS - A MIPS decompiler                                       //
//                                                                     //
//        Copyright (c) 2008 by Ruben Acuna and Michael Bradley        //
//                                                                     //
// This file is part of DeMIPS.
//
// DeMIPS is free software; you can redistribute it and/or             //
// modify it under the terms of the GNU Lesser General Public          //
// License as published by the Free Software Foundation; either        //
// version 3 of the License, or (at your option) any later version.  //
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
// Michael Bradley                                                     //
//                                                                     //
/////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DeMIPS
{
    /// <summary>
    /// Handles GUI events and components.
    /// </summary>
    public partial class Form1 : Form
    {
        //SETTINGS
        private const string DEFAULT_FILENAME = "2-29.asm";

        /// <summary>
        /// Initializes GUI and starts decompilation process.
        /// </summary>
        /// <param name="args">If args[0] exists, it will automatically be decompiled.</param>
        public Form1(string[] args)
        {
            string filename = DEFAULT_FILENAME;

            InitializeComponent();

            if (args.Length >= 1)
                filename = args[0];

            if (File.Exists(filename))
                Decompile(filename);
            else
                throw new Exception("Cannot load: " + filename + "!");//TODO: GUI work.
        }

        /// <summary>
        /// Decompiles file. Very hacked up.
        /// </summary>
        /// <param name="filename">Name to decompile</param>
        public void Decompile(string filename) //TODO: move to proper file
        {
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            LinkedList<ProgramLine> decompiledFile = new LinkedList<ProgramLine>();
            string tempLine;

            while ((tempLine = file.ReadLine()) != null)
                decompiledFile.AddLast(new ProgramLine(tempLine.ToLower()));

            file.Close();

            PreprocessFile(decompiledFile);

            string[] displayAsm = new string[decompiledFile.Count];
            string[] displayCode = new string[decompiledFile.Count];
            int i = 0;
            foreach(ProgramLine line in decompiledFile)
            {
                displayAsm[i] = line.Assembly;
                DecompileLine(line);
                displayCode[i] = line.Highlevel;
                i++;
            }

            TextBoxInput.Lines = displayAsm;
            TextBoxOutput.Lines = displayCode;
            TextBoxOutput.Select(0,0); //HACK: .NET automatically highlights text.
        }

        /// <summary>
        /// This decompiles a single line of MIPS to C. This will be requiring heavy
        /// reworking so I won't document much.
        /// </summary>
        /// <param name="line">Line to decompile.</param>
        private void DecompileLine(ProgramLine line) //TODO: move to proper file
        {
            //LABEL
            if (line.Assembly.Contains(":"))
                line.Highlevel = line.Assembly;//TODO: move to C emitter?
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
                    lineParameters[i] = lineParameters[i].TrimStart('$');//TODO: Dumb. Once MathParser matures, this should be removed.
                }

                //any $zero's will now be replaced. 
                //TODO: Once MathParser matures, this should be removed.
                for (int i = 0; i < lineParameters.Length; i++)
                    if (lineParameters[i].Equals("zero"))
                        lineParameters[i] = "0";

                switch (lineKeyword)
                {
                    case "addi": //fall through, MathParser will handle constants.
                    case "add":
                        processedLine = MathParser.SimplifyEqual(lineParameters[0], MathParser.SimplifyArithmetic("+", lineParameters[1], lineParameters[2]));
                        break;

                    case "beq":
                        processedLine = "if ( " + lineParameters[0] + " == " + lineParameters[1] + " ) goto " + lineParameters[2];
                        break;

                    case "bne":
                        processedLine = "if ( " + lineParameters[0] + " != " + lineParameters[1] + " ) goto " + lineParameters[2];
                        break;

                    case "sll":
                        processedLine = MathParser.SimplifyEqual(lineParameters[0], MathParser.SimplifyArithmetic("*", lineParameters[1], lineParameters[2]));
                        break;

                    case "subi": //fall through, MathParser will handle constants.
                    case "sub":
                        processedLine = MathParser.SimplifyEqual(lineParameters[0], MathParser.SimplifyArithmetic("-", lineParameters[1], lineParameters[2]));
                        break;

                    case "j":
                        processedLine = "goto " + lineParameters[0];
                        break;
                }

                if(processedLine.Equals("") && !line.Assembly.Equals(""))
                    processedLine = "//Unidentified: \"" + line.Assembly + "\"";
                else if (!processedLine.Equals(""))
                    processedLine += ";";//TODO: move to C emitter or ProgramLine

                line.Highlevel = processedLine;
            }
        }


        /// <summary>
        /// TODO: full preprocessor. checks for: malformed input, labels existing on same line as keyword, etc.
        /// </summary>
        /// <param name="code">Code to process.</param>
        private void PreprocessFile(LinkedList<ProgramLine> code)//TODO: move to proper file
        {
            PreprocessComments(code);
            PreprocessWhiteSpace(code);
        }

        /// <summary>
        /// Splits a lines into code and comment portions.
        /// </summary>
        /// <param name="code">Code to process.</param>
        private void PreprocessComments(LinkedList<ProgramLine> code) //TODO: move to proper file
        {
            foreach (ProgramLine line in code)
            {
                if (line.Assembly.Contains("#"))
                {
                    line.AssemblyComment = line.Assembly.Substring(line.Assembly.IndexOf('#') + 1); //strips # character
                    line.Assembly = line.Assembly.Substring(0, line.Assembly.IndexOf('#')).Trim();                 
                }
            }
        }

        /// <summary>
        /// Reconstructs each line to include only one space between tokens.
        /// </summary>
        /// <param name="code">Code to process.</param>
        private void PreprocessWhiteSpace(LinkedList<ProgramLine> code)
        {
            foreach (ProgramLine line in code)
            {
                string processedLine = "";
                string[] lineTokens = line.Assembly.Split(' ');

                foreach (string token in lineTokens)
                    if (!token.Trim().Equals(""))
                        processedLine += token + " ";

                processedLine = processedLine.Trim(); //HACK: the above line always leaves a trailing space.

                line.Assembly = processedLine;

            }

        }
    }
}
