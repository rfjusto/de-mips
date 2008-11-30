//This is the class used for loops (based off of ProgramChunkJumpTarget.cs)

using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class ProgramChunkLoop : IProgramChunk
    {
        #region variables

        private IProgramChunk loopLabel;
        private LinkedList<IProgramChunk> loopCode;

        #endregion

        #region properties

        public IProgramChunk LoopLabel
        {
            get { return loopLabel; }
            set { loopLabel = value; }
        }

        #endregion

        #region constructor

        public ProgramChunkLoop(IProgramChunk LoopLabel2)
        {
            LoopLabel = LoopLabel2;
        }

        #endregion


        #region interface methods

        //TODO: Implement me!
        public bool UsesVariable(BlockVariable variable)
        {
            return false;
        }

        #endregion
    }
}