using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class ProgramChunkJumpUnconditional : IProgramChunk
    {
        #region interface methods

        //TODO: Implement me!
        public bool UsesVariable(BlockVariable variable)
        {
            return false;
        }

        #endregion
    }
}
