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

/*
 *TODO LIST:
 *Add { } support.
 *
*/

namespace DeMIPS
{
    class BackendC : IBackend
    {

        public string[] EmitBlock(ProgramBlock block)
        {
            LinkedList<string> blockList = new LinkedList<string>();
            string[] blockSource;

            blockList.AddLast("{");

            foreach (IProgramChunk chunk in block.Program)
            {
                foreach(string line in EmitChunk(chunk))
                    blockList.AddLast(line);
            }

            blockList.AddLast("}");

            blockSource = new string[blockList.Count];
            blockList.CopyTo(blockSource, 0);

            return blockSource;
        }

        public string[] EmitChunk(IProgramChunk chunk)
        {
            string[] programLines;
            LinkedList<string> lines = new LinkedList<string>();

            if (chunk is ProgramChunkJumpTarget)
            {
                //Form: (label):
                ProgramChunkJumpTarget activeChunk = (ProgramChunkJumpTarget)chunk;

                lines.AddLast(activeChunk.Label + ":");
            }
            else if (chunk is ProgramChunkJumpUnconditional)
            {
                //Form: goto (label)
                ProgramChunkJumpUnconditional activeChunk = (ProgramChunkJumpUnconditional)chunk;

                lines.AddLast("goto " + activeChunk.Target.Label + ";");
            }
            else if (chunk is ProgramChunkAssignment)   
            {
                //Form: (Var) (assign) (term op term)
                ProgramChunkAssignment activeChunk = (ProgramChunkAssignment)chunk;

                //1) determine the variable that will recieve this assignment.
                //2) emit the code that will assign the expression to the variable.
                //3) determine the expression that will that variable will be assigned to.
                lines.AddLast(activeChunk.Variable.Name + " = " + EmitExpression(activeChunk.Expression) + ";");
            }
            else if (chunk is ProgramChunkLoop)
            {
                ProgramChunkLoop activeChunk = (ProgramChunkLoop)chunk;

                lines.AddLast("while (true)");
                foreach(string line in EmitBlock(activeChunk.InnerCode))
                    lines.AddLast(line);
            }
            else
            {
                UtilDebugConsole.AddException(new ExceptionWarning("BackendC - No operation IProgramChunk found, emitting NOP."));

                lines.AddLast("//NOP");
            }

            programLines = new string[lines.Count];
            lines.CopyTo(programLines, 0);

            return programLines;
        }

    private string EmitExpression(ProgramChunkExpression expression)
    {
        string finalExpression;

        if (expression.FirstTerm is BlockConstant)
            finalExpression = ((BlockConstant)expression.FirstTerm).Constant.ToString();
        else if (expression.FirstTerm is BlockVariable)
            finalExpression = ((BlockVariable)expression.FirstTerm).Name;
        else
            throw new Exception("Attempted to emit an expression with at term that was not a constant or variable.");

        if (expression.Oper == Operand.ADDITION)
            finalExpression += " + ";
        else if (expression.Oper == Operand.DIVISION)
            finalExpression += " / ";
        else if (expression.Oper == Operand.MULTIPLICATION)
            finalExpression += " * ";
        else if (expression.Oper == Operand.SUBTRACTION)
            finalExpression += " - ";
        else
            throw new Exception("Attempted to emit an expression with an operand that doesn't exist.");

        if (expression.SecondTerm is BlockConstant)
            finalExpression += ((BlockConstant)expression.SecondTerm).Constant.ToString();
        else if (expression.SecondTerm is BlockVariable)
            finalExpression += ((BlockVariable)expression.SecondTerm).Name;
        else
            throw new Exception("Attempted to emit an expression with at term that was not a constant or variable.");

        return finalExpression;
    }

    }

    //FUTURE: Setting up for supporting more than C.
    interface IBackend
    {
        string[] EmitBlock(ProgramBlock block);
        string[] EmitChunk(IProgramChunk chunk);
    }
}
