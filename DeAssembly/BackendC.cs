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

            foreach (IProgramChunk chunk in block.Program)
            {
                blockList.AddLast(EmitChunk(chunk));
            }

            blockSource = new string[blockList.Count];
            blockList.CopyTo(blockSource, 0);

            return blockSource;
        }

        public string EmitChunk(IProgramChunk chunk)
        {
            string programLine;

            if (chunk is ProgramChunkJumpTarget)
            {
                ProgramChunkJumpTarget activeChunk = (ProgramChunkJumpTarget)chunk;

                programLine = activeChunk.Label + ":";
            }
            else if (chunk is ProgramChunkJumpUnconditional)
            {
                ProgramChunkJumpUnconditional activeChunk = (ProgramChunkJumpUnconditional)chunk;

                programLine = "goto " + activeChunk.Target.Label + ";";
            }
            else
            {
                UtilDebugConsole.AddException(new ExceptionWarning("BackendC: No operation IProgramChunk found."));

                programLine = "//NOP";
            }

            return programLine;
        }
    }

    //FUTURE: Setting up for supporting more than C.
    interface IBackend
    {
        string[] EmitBlock(ProgramBlock block);
        string EmitChunk(IProgramChunk chunk);
    }
}
