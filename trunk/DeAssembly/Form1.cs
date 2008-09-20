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
// Michael Bradley                                                     //
//                                                                     //
/////////////////////////////////////////////////////////////////////////

#define ENABLE_V4300I_INSTRUCTIONS

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
        #region constants

        private const string DEFAULT_FILENAME = "2-29.asm";
        //private const string DEFAULT_FILENAME = "AlleyCat.asm";

        #endregion

        #region variables

        OpenFileDialog fileSelectionDialog;

        #endregion

        /// <summary>
        /// Initializes GUI and starts decompilation process.
        /// </summary>
        /// <param name="args">If args[0] exists, it will automatically be decompiled.</param>
        public Form1(string[] args)
        {
            InitializeComponent();

            fileSelectionDialog = new OpenFileDialog();
            fileSelectionDialog.Filter = "mips assembly (*.asm)|*.asm|All files (*.*)|*.*";
            
            if (args.Length >= 1)
                TextBoxFileName.Text = args[0];
            else
                TextBoxFileName.Text = DEFAULT_FILENAME;

            ButtonDecompile_Click(null, null);
        }

        /// <summary>
        /// Decompiles file. Very hacked up.
        /// </summary>
        /// <param name="filename">Name to decompile</param>
        public void Decompile(string filename) //TODO: move to proper file
        {
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            LinkedList<ProgramLine> assemblyFile = new LinkedList<ProgramLine>();
            string tempLine;
            
            while ((tempLine = file.ReadLine()) != null)
                assemblyFile.AddLast(new ProgramLine(tempLine.ToLower()));

            file.Close();

            FrontendMIPS.Preprocess(assemblyFile);

            string[] displayAsm = new string[assemblyFile.Count];
            string[] displayCode = new string[assemblyFile.Count];
            int i = 0;
            //for(int i = 0; i < decompiledFile.Count; i++)

            foreach(object objLine in assemblyFile)
            {
                ProgramLine line = (ProgramLine)objLine;
                displayAsm[i] = line.Assembly;

            #if RELEASE
                try
                {
                    DecompileLine(line);
                }

                catch (Exception err)
                {
                    MessageBox.Show(err.Message + "\n" + err.StackTrace, "An error has ocurred during decompilation!");
                }
            #else //DEBUG
                DecompileLine(line, new ProgramChunkJumpUnconditional());
            #endif

                displayCode[i] = line.Highlevel;
                i++;
            }

            if (displayAsm.Length != displayCode.Length)
                MessageBox.Show("WARNING: The number of input and output lines differ, GUI may function incorrectly.");

            TextBoxInput.Lines = displayAsm;
            TextBoxOutput.Lines = displayCode;
            TextBoxOutput.Select(0,0); //HACK: .NET automatically highlights text.
        }

        /// <summary>
        /// This decompiles a single line of MIPS to C. This will be requiring heavy
        /// reworking so I won't document much.
        /// </summary>
        /// <param name="line">Line to decompile.</param>
        private void DecompileLine(ProgramLine line, IProgramChunk lineChunk) //TODO: finish moving to FrontendMIPS
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
                    processedLine = "//Unidentified: \"" + line.Assembly + "\"";
                else if (!processedLine.Equals(""))
                    processedLine += ";";//TODO: move to C emitter or ProgramLine

                line.Highlevel = processedLine;
            }
        }

        #region GUI events

        private void ButtonSelectFile_Click(object sender, EventArgs e)
        {
            if (fileSelectionDialog.ShowDialog() == DialogResult.OK)
            {
                TextBoxFileName.Text = fileSelectionDialog.FileName;
                label5.Text = "Desynchronized";
            }
        }

        private void ButtonDecompile_Click(object sender, EventArgs e)
        {
            string filename = TextBoxFileName.Text;

            if (File.Exists(filename))
            {
                Decompile(filename);
                label5.Text = "Synchronized";
            }
            else
                MessageBox.Show("Cannot find: " + filename);
        }

        private void ButtonQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

    }
}
