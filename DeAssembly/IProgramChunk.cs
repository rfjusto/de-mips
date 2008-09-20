using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    interface IProgramChunk
    {
        bool UsesVariable(BlockVariable variable);
    }
}
