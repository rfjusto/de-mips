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
            //Note: The label/assignment checks and jump/branch checks are
            //put in seperate foreach statements because this way the lists
            //will be complete by the time it checks the jumps

            ProgramBlock codeToDelete = new ProgramBlock();
            ProgramChunkLoop newLoop = null;
            ProgramChunkJumpTarget startLabel = null;

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
            }

            foreach (IProgramChunk chunk in currentBlock.Program)
            {
                if (chunk is ProgramChunkJumpUnconditional) //Check all the jump statements
                {
                    foreach (IProgramChunk chunk2 in currentBlock.ProgramAllJumpTargets)
                    {
                        if (((ProgramChunkJumpUnconditional)chunk).Target == chunk2) //Continue only if a jump statement branches to something in the list of labels
                        {
                            //Label and jump to label detected; if label is before jump loop will be detected
                            //This check determines if the specified label occurs before the jump to it

                            foreach (IProgramChunk chunk3 in currentBlock.Program)
                            {
                                if (chunk3 is ProgramChunkJumpTarget)
                                {
                                    if (chunk3 == chunk2) //Branch label comes first, loop detected
                                    {
                                        UtilDebugConsole.AddMessage("Loop involving an unconditional jump found");

                                        /*This section of code will use a while loop to run through the code.
                                         Upon detecting chunk2 (the label), it will create a list of the
                                         loop code AS WELL as the code inside the loop.  Upon detecting
                                         chunk, the code will stop writing to both lists.  At this point, one
                                         list will hold the whole loop and one will hold the code within the
                                         loop.  A loop will then be created using chunk, chunk2, and the
                                         linked list of the code within the loop, with everything in the
                                         linked list of the whole loop being deleted from the linked list of
                                         the entire program.  */

                                        ProgramBlock loopInnerCode = new ProgramBlock();
                                        ProgramBlock loopOuterCode = new ProgramBlock();
                                        bool copyCode = false;

                                        foreach (IProgramChunk loopRearrange in currentBlock.Program)
                                        {

                                            if (loopRearrange is ProgramChunkJumpTarget)
                                            {
                                                if (loopRearrange == chunk2)
                                                {
                                                    loopOuterCode.Program.AddLast(loopRearrange);
                                                    copyCode = true;
                                                    //UtilDebugConsole.AddMessage("Added label to ProgramChunkLoop");

                                                }
                                            }
                                            else if (loopRearrange is ProgramChunkJumpUnconditional) //Upon detecting jump, stop writing to lists
                                            {
                                                if (loopRearrange == chunk)
                                                {
                                                    loopOuterCode.Program.AddLast(loopRearrange);
                                                    copyCode = false;
                                                    //UtilDebugConsole.AddMessage("Added jump to ProgramChunkLoop");
                                                }
                                            }
                                            if ((copyCode == true) && (loopRearrange != chunk2)) //The second part of this check is here because the way this is set up this will run upon detecting the label
                                            { //Write all code to inside of loop but only if it's detected the label
                                                loopOuterCode.Program.AddLast(loopRearrange);
                                                loopInnerCode.Program.AddLast(loopRearrange);
                                                //UtilDebugConsole.AddMessage("Added line to ProgramChunkLoop");
                                            }
                                                
                                        }
                                        newLoop = new ProgramChunkLoop(loopInnerCode);
                                        // // // Code to add ProgramChunkLoop before chunk2 goes here; I can't figure out how to change the input to fit the required function // // //
                                        //currentBlock.Program.AddBefore(currentBlock.Program.Last, newLoop);
                                        
                                        foreach (IProgramChunk loopDelete in loopOuterCode.Program) {
                                            codeToDelete.Program.AddLast(loopDelete);
                                        }
  
                                    }
                                }
                                else if (chunk3 is ProgramChunkJumpUnconditional)
                                {
                                    if (chunk3 == chunk) //Jump comes first, not a loop
                                        break;
                                }                                  
                            }

                        }
                    }
                }
            }

            //find label that code should be added before
            foreach (IProgramChunk chunk in codeToDelete.Program)
                if (chunk is ProgramChunkJumpTarget)
                {
                    startLabel = (ProgramChunkJumpTarget)chunk;
                    break;
                }

            //HACK:
            if (newLoop != null)
                currentBlock.Program.AddBefore(currentBlock.Program.Find(startLabel), newLoop);

            foreach (IProgramChunk loopDelete in codeToDelete.Program)
            {
                currentBlock.Program.Remove(loopDelete);
            }

            /* Code required for this isn't implemented yet, commenting this part out
            foreach (IProgramChunk chunk in currentBlock.Program)
            {
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
            */
        }
    }
}
