using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class ProgramLine
    {
        private string assembly;
        private string highlevel;

        #region properties

        public string Assembly
        {
            get { return assembly; }
            set { assembly = value; }
        }

        public string Highlevel
        {
            get { return highlevel; }
            set { highlevel = value; }
        }

        #endregion

        #region methods

        public ProgramLine(string asm)
        {
            Assembly = asm;
        }

        #endregion
    }
}
