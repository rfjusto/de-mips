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

//FIXME: right now the operand is kept track of with the
//block variable, the problem is that the variable may be
//used in different mathimatcal ways depending the expression
//that it is used in.

namespace DeMIPS
{
    class FrontendMIPS : IFrontend
    {
        const string VARIABLE_MARKER = "$";

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

                    #region SLL - Shift Left Logical (constant or variable)
                    
                    //TODO: this code can probably be mapped more generically...
                    case "addi":
                    case "add":
                    case "sllv":
                    case "sll":
                        //Form: (command) product, multiplicand, multiplier
                        //see below (or above i guess) to see acceptable commands

                        //Addition: 'add', and 'addi'
                        Operand commandOperand = Operand.ADDITION;

                        //Multiplication: 'sll', and 'sllv'
                        if(lineKeyword.Equals("sll") || lineKeyword.Equals("sllv"))
                            commandOperand = Operand.MULTIPLICATION;

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

                            termMultiplicand = new ProgramChunkTermConstant(term);
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
                            //CHECKME: attempt to parse hex numbers, does it work this way?
                            int term = int.Parse(lineParameters[2], System.Globalization.NumberStyles.HexNumber);

                            //deal with bit shifts
                            if (lineKeyword.Equals("sll"))
                                term *= 2;
                            else if (lineKeyword.Equals("sllv"))
                                throw new Exception("FrontendMIPS - Cannot bit shift by variable without subexpression support.");


                            termMultiplier = new ProgramChunkTermConstant(int.Parse(lineParameters[2]) * 2);
                        }

                        termMultiplier.Operator = commandOperand;

                        #endregion

                        ProgramChunkExpression expression = new ProgramChunkExpression(termMultiplicand);
                        expression.Terms.Enqueue(termMultiplier);

                        translatedChunk = new ProgramChunkAssignment(termAssignee, expression);
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

        //logically we could assume that when this returns false it the string is a constant and that is the case here.
        //
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
        /// TODO: full preprocessor. checks for: malformed input, labels existing on same line as keyword, etc.
        /// </summary>
        /// <param name="file">Code to process.</param>
        public void Preprocess(string[] file)
        {
            //TODO: split labels from code
            UtilPreprocessor.PreprocessComments(file, "#");
            UtilPreprocessor.PreprocessWhiteSpace(file);
            PreprocessByToken(file);
        }

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

    //FUTURE: Setting up for supporting more than MIPS.
    interface IFrontend
    {
        IProgramChunk TranslateLine(string assembly, ProgramBlock parentBlock);
        void Preprocess(string[] file);
    }
}
