using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeAssembly
{
    class ProgramLine
    {
        private string assembly;
        private string cpp;
        private bool isDecompiled;

        #region properties

        public string Assembly
        {
            get { return assembly; }
            set { assembly = value; }
        }

        public string CPP
        {
            get { return cpp; }
            set { cpp = value; }
        }

        public bool IsDecompiled
        {
            get { return isDecompiled; }
            set { isDecompiled = value; }
        }

        #endregion

        #region methods

        public ProgramLine(string asm)
        {
            Assembly = asm;
            isDecompiled = false;
        }

        #endregion
    }
}
