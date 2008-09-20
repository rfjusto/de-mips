using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class ProgramChunkNoOperation : IProgramChunk
    {
        #region interface methods

        public bool UsesVariable(BlockVariable variable)
        {
            return false;
        }

        #endregion
    }
}
