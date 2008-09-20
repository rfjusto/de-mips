using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class ProgramChunkJumpUnconditional : IProgramChunk
    {
        #region variables

        private ProgramChunkJumpTarget target;

        #endregion

        #region properties

        public ProgramChunkJumpTarget Target
        {
            get { return target; }
            set { target = value; }
        }

        #endregion

        #region constructor

        public ProgramChunkJumpUnconditional(ProgramChunkJumpTarget myTarget)
        {
            Target = myTarget;
        }

        #endregion

        #region interface methods

        public bool UsesVariable(BlockVariable variable)
        {
            return false;
        }

        #endregion
    }
}
