using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    interface IProgramChunk
    {
        string Assembly//HACK: just getting it to build
        {
            get;
            set;
        }

        string Highlevel//HACK: just getting it to build
        {
            get;
            set;
        }
    }
}
