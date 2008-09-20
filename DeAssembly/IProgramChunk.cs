using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    //TODO: this should be used by three more classes:
    //  ProgramChunkAssignment, ProgramChunkCall, ProgramChunkReturn
    interface IProgramChunk
    {
        bool UsesVariable(BlockVariable variable);
    }
}
