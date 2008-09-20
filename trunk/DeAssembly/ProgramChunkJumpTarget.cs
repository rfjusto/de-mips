using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class ProgramChunkJumpTarget : IProgramChunk
    {
        #region variables

        private string label;

        #endregion

        #region properties

        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        #endregion

        #region constructor

        public ProgramChunkJumpTarget(string myLabel)
        {
            Label = myLabel;
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
