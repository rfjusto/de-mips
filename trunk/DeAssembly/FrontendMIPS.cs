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
    class FrontendMIPS : IFrontend
    {
        const string VARIABLE_MARKER = "$";

        /// <summary>
        /// Translates a line of MIPS assembly into the intermediate format and returns
        /// the result.
        /// </summary>
        /// <param name="assembly">MIPS assembly.</param>
        /// <param name="parentBlock">ProgramBlock containing previously translated code
        /// and associated information</param>
        /// <returns>Translated chunk.</returns>
        public IProgramChunk TranslateLine(string assembly, ProgramBlock parentBlock)
        {
            IProgramChunk translatedChunk = null;

            //***LABELS***
            if (assembly.Contains(":"))
            {
                string labelName = assembly.Substring(0, assembly.IndexOf(":"));

                //label does not exist as an orphan
                if(parentBlock.GetOrphanJumpTargetByLabel(labelName) == null)
                    translatedChunk = new ProgramChunkJumpTarget(labelName);
                //label does exist as an orphan.
                else
                {
                    translatedChunk = parentBlock.GetOrphanJumpTargetByLabel(labelName);
                    parentBlock.RemoveOrphanJumpTarget(parentBlock.GetOrphanJumpTargetByLabel(labelName));
                }
            }
            //**KEYWORDS**
            else
            {
                string lineKeyword = assembly.Split(' ')[0];
                string[] lineParameters = assembly.Substring(assembly.IndexOf(' ') + 1).Split(' ');
                
                switch (lineKeyword)
                {
                    #region J - Unconditional Jump
                    case "j":
                    {
                        ProgramChunkJumpTarget target = parentBlock.GetJumpTargetByLabel(lineParameters[0]);

                        //label does not exist but should be defined later. create an orphan placeholder.
                        if (target == null)
                        {
                            target = new ProgramChunkJumpTarget(lineParameters[0]);
                            parentBlock.AddOrphanJumpTarget(target);
                        }

                        translatedChunk = new ProgramChunkJumpUnconditional(target);
                    }
                    break;
                    #endregion

                    #region Form: (command) product, multiplicand, multiplier

                    case "addi":
                    case "add":
                    case "sllv"://SLL - Shift Left Logical (variable)
                    case "sll": //SLL - Shift Left Logical (constant)
                    case "sub":
                    case "subi":
                        //Form: (command) product, multiplicand, multiplier
                        //see below (or above i guess) to see acceptable commands

                        //Addition: 'add', and 'addi' (default)
                        Operand commandOperand = Operand.ADDITION;

                        //Multiplication: 'sll', and 'sllv'
                        if(lineKeyword.Equals("sll") || lineKeyword.Equals("sllv"))
                            commandOperand = Operand.MULTIPLICATION;

                        //Subtraction: 'sub', and 'subi'
                        if(lineKeyword.Equals("sub") || lineKeyword.Equals("subi"))
                            commandOperand = Operand.SUBTRACTION;

                        BlockVariable termAssignee;  //must be variable
                        ProgramChunkExpressionTerm termMultiplicand;//constant or variable? I think the doc only support var's
                        ProgramChunkExpressionTerm termMultiplier; //constant or variable

                        #region 1) find assignee

                        if (IsVariable(lineParameters[0]))
                        {
                            ProcessVariable(ref lineParameters[0]);

                            BlockVariable variable = parentBlock.GetVariableByNameForced(lineParameters[0]);

                            termAssignee = variable;
                        }
                        else
                            throw new Exception("FrontendMIPS - Detected attempt to assign expression to constant.");

                        #endregion

                        #region 2) find multiplicand

                        if (IsVariable(lineParameters[1]))
                        {
                            ProcessVariable(ref lineParameters[1]);

                            BlockVariable variable = parentBlock.GetVariableByNameForced(lineParameters[1]);

                            termMultiplicand = variable;
                        }
                        else //The first (real) term is a constant
                        {
                            //CHECKME: attempt to parse hex numbers, does it work this way?
                            int term = int.Parse(lineParameters[1], System.Globalization.NumberStyles.HexNumber);

                            termMultiplicand = new BlockConstant(term);
                        }

                        #endregion
                        
                        #region 3) find multiplier

                        if (IsVariable(lineParameters[2]))
                        {
                            ProcessVariable(ref lineParameters[2]);

                            BlockVariable variable = parentBlock.GetVariableByNameForced(lineParameters[2]);

                            termMultiplier = variable;
                        }
                        else//constant
                        {
                            //TODO: support hex numbers
                            if(lineParameters[2].Contains("x"))
                                throw new Exception("FrontendMIPS - Cannot parse hexidecimal numbers.");

                            int term = int.Parse(lineParameters[2]);

                            //deal with bit shifts
                            if (lineKeyword.Equals("sll"))
                                term *= 2;
                            else if (lineKeyword.Equals("sllv"))
                                throw new Exception("FrontendMIPS - Cannot bit shift by variable without subexpression support.");


                            termMultiplier = new BlockConstant(term);
                        }

                        #endregion

                        ProgramChunkExpression expression = new ProgramChunkExpression(termMultiplicand, commandOperand, termMultiplier);

                        translatedChunk = new ProgramChunkAssignment(termAssignee, expression);
                        break;
                    #endregion

                    #region Branchs (BNE, BEQ)

                    //Form: if (var equality var) goto label.
                    //e.g. "beq $a1 0 finish"
                    case "bne":
                    case "beq":
                    {
                        Equality equality;
                        ProgramChunkExpressionTerm first;
                        ProgramChunkExpressionTerm second;
                        ProgramChunkJumpTarget target;

                        #region 1) determine equality

                        if (lineKeyword.Equals("bne"))
                            equality = Equality.NOT_EQUAL;
                        else if (lineKeyword.Equals("beq"))
                            equality = Equality.EQUAL;
                        else
                            throw new Exception("FrontendMIPS - Unimplemented equality found.");

                        #endregion

                        #region 2) find first item to evaluate

                        if (IsVariable(lineParameters[0]))
                        {
                            ProcessVariable(ref lineParameters[0]);
                            first = parentBlock.GetVariableByNameForced(lineParameters[0]);
                        }
                        else //constant
                        {
                            //TODO: support hex numbers
                            if (lineParameters[0].Contains("x"))
                                throw new Exception("FrontendMIPS - Cannot parse hexidecimal numbers.");

                            first = new BlockConstant(int.Parse(lineParameters[0]));
                        }

                        #endregion

                        #region 3) find second item to evaluate

                        if (IsVariable(lineParameters[1]))
                        {
                            ProcessVariable(ref lineParameters[1]);
                            second = parentBlock.GetVariableByNameForced(lineParameters[1]);
                        }
                        else //constant
                        {
                            //TODO: support hex numbers
                            if (lineParameters[1].Contains("x"))
                                throw new Exception("FrontendMIPS - Cannot parse hexidecimal numbers.");

                            second = new BlockConstant(int.Parse(lineParameters[1]));
                        }

                        #endregion

                        #region 4) generate jump for true

                        target = parentBlock.GetJumpTargetByLabel(lineParameters[2]);

                        //label does not exist but should be defined later. create an orphan placeholder.
                        if (target == null)
                        {
                            target = new ProgramChunkJumpTarget(lineParameters[2]);
                            parentBlock.AddOrphanJumpTarget(target);
                        }

                        #endregion

                        translatedChunk = new ProgramChunkBranch(equality, first, new ProgramChunkJumpUnconditional(target), second, new ProgramChunkNoOperation());
                    }
                    break;

                    #endregion
                }
            }

            if (translatedChunk == null)
            {
                translatedChunk = new ProgramChunkNoOperation();
                UtilDebugConsole.AddException(new Exception("FrontendMIPS - Unidentified assembly encountered during translation."));
            }

            return translatedChunk;
        }

        /// <summary>
        /// Tests if a given token of text is a variable name. In most cases, when this
        /// returns false, the possible variable is actually a constant.
        /// </summary>
        /// <param name="variable">Possible variable name.</param>
        /// <returns>True if it is a variable.</returns>
        private bool IsVariable(string variable)
        {
            if (!(variable is string))
                throw new Exception("FrontendMIPS - Unexpected input in IsVariable.");

            return variable.Contains(VARIABLE_MARKER);
        }

        private void ProcessVariable(ref string variable)
        {
            variable = variable.TrimStart('$');
        }

        /// <summary>
        /// Processes source code by removing non-coding regions,cleaning non-uniform
        /// whitespace, removing spare characters. TODO: check for malformed input and
        /// labels existing on same line as keyword.
        /// </summary>
        /// <param name="file">Code to process.</param>
        public void Preprocess(string[] file)
        {
            //TODO: split labels from code
            UtilPreprocessor.PreprocessComments(file, "#");
            UtilPreprocessor.PreprocessWhiteSpace(file);
            PreprocessByToken(file);
        }

        /// <summary>
        /// Cleans up tokens in source code. Tokens are defined by blocks of characters
        /// between spaces. Tokens will have tail ','s removed and space between tokens
        /// will be normalize to a single space.
        /// </summary>
        /// <param name="code">Array of strings containing source code.</param>
        private void PreprocessByToken(string[] code)
        {
            for (int i = 0; i < code.Length; i++)
            {
                string[] lineTokens = code[i].Split(' ');
                string processedLine = "";

                foreach (string token in lineTokens)
                {
                    if (token.TrimEnd(',').Equals("$zero"))
                        processedLine += "0 ";
                    else
                        processedLine += token.TrimEnd(',') + " ";
                }

                code[i] = processedLine.Trim(); //HACK: the above leaves a trailing space.
            }
        }
    }

    /// <summary>
    /// Interface for the Frontend that will convert assembly to the intermediate
    /// format. The main purpose of this is preparation for supporting more than MIPS.
    /// </summary>
    interface IFrontend
    {
        IProgramChunk TranslateLine(string assembly, ProgramBlock parentBlock);
        void Preprocess(string[] file);
    }
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
