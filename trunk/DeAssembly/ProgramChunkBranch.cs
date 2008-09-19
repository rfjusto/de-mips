using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    public class ProgramChunkBranch : IProgramChunk
    {
        #region properties
        public string Assembly//HACK: just to get it compiling
        {
            get { return "unimp"; }
            set { }
        }

        public string Highlevel//HACK: just to get it compiling
        {
            get { return "unimp"; }
            set { }
        }
        #endregion
    }
}
