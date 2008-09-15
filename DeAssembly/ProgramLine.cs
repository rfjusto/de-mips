using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class ProgramLine
    {
        private string assembly;
        private string assemblyComment; //TODO: This is loaded but the GUI doesn't use it.
        private string highlevel;
        private bool isDecompiled;

        #region properties

        public string Assembly
        {
            get { return assembly; }
            set { assembly = value; }
        }

        public string AssemblyComment
        {
            get { return assemblyComment; }
            set { assemblyComment = value; }
        }

        public string Highlevel
        {
            get { return highlevel; }
            set { highlevel = value; }
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
