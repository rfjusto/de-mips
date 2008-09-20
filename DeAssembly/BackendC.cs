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
        //static string[] EmitBlock() { }

        static string EmitChunk(IProgramChunk chunk)
        {
            string programLine;

            if (chunk is ProgramChunkJumpTarget)
            {
                ProgramChunkJumpTarget thisChunk = (ProgramChunkJumpTarget)chunk;

                programLine = thisChunk.Label + ":";
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
        //string EmitChunk(IProgramChunk chunk);
    }
}
