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
// Michael Bradley <mbradley1372@cox.net>                              //
//                                                                     //
/////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class SimplifyStructures
    {
        public static void SimplifyProgramBlock(ProgramBlock workingBlock)
        {
            SimplifyLoops(workingBlock);
        }

        private static void SimplifyLoops(ProgramBlock currentBlock)
        {
            //Attempt at detecting loops; starting with a simple branch/j loop and moving on to variable initializations afterward
            foreach (IProgramChunk chunk in currentBlock.Program)
            {
                //This checks a program chunk in order to see if it is part of a loop.

                if (chunk is ProgramChunkJumpTarget)
                {
                    currentBlock.ProgramAllJumpTargets.AddLast(chunk); //Add branch label to list to check on a jump
                }
                if (chunk is ProgramChunkAssignment)
                {
                    currentBlock.ProgramAllIncrementers.AddLast(chunk); //Add incrementer to list to check on a jump
                }
                if (chunk is ProgramChunkJumpUnconditional)
                {
                    foreach (IProgramChunk chunk2 in currentBlock.ProgramAllJumpTargets)
                    {
                        if (((ProgramChunkJumpUnconditional)chunk).Target == chunk2)
                        {
                            //Loop detected; rearrange code to form loop

                            UtilDebugConsole.AddMessage("Possible loop involving an unconditional jump found");

                            /*This section of code will use a while loop to run through the code.
                             Upon detecting chunk2 (the label), it will create a list of the
                             loop code AS WELL as the code inside the loop.  Upon detecting
                             chunk, the code will stop writing to both lists.  At this point, one
                             list will hold the whole loop and one will hold the code within the
                             loop.  A loop will then be created using chunk, chunk2, and the
                             linked list of the code within the loop, with everything in the
                             linked list of the whole loop being deleted from the linked list of
                             the entire program.  This explanation is here because the code isn't
                             and I don't want to forget what the code will do.*/
                        }
                    }
                }

                //This code will supposedly work despite the lack of checking if
                //the incrementor is inside the loop; it assumes that the code
                //passed through it has been checked beforehand.  This check
                //should likely be added in though, as the code might not have
                //been checked manually.
                if (chunk is ProgramChunkBranch)
                {
                    foreach (IProgramChunk chunk2 in currentBlock.ProgramAllIncrementers)
                    {
                        //if (((ProgramChunkBranch)chunk).NOT IMPLEMENTED YET == chunk2 //Checks if variable associated with branch is same as incrementer
                        foreach (IProgramChunk chunk3 in currentBlock.ProgramAllJumpTargets)
                        {
                            if (((ProgramChunkJumpUnconditional)chunk).Target == chunk3) //Checks if target associated with jump is same as branch
                            {
                                UtilDebugConsole.AddMessage("Possible loop involving a branch found");
                            }
                        }
                        //}
                    }
                }
            }
        }
    }
}
