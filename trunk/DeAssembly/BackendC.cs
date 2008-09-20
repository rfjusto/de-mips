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
        //TODO: Implement me!
        //public string[] EmitBlock() { }

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

                programLine = "goto " + activeChunk.Target + ";";
            }
            else
            {
                throw new Exception("BackendC: Unknown chunk in EmitChunk.");
            }

            return programLine;
        }
    }

    //FUTURE: Setting up for supporting more than C.
    interface IBackend
    {
        string EmitChunk(IProgramChunk chunk);
    }
}
