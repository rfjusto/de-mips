﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    //TODO: Implement me!
    class ProgramChunkBranch : IProgramChunk
    {
        #region constructor

        public ProgramChunkBranch(Equality equality, BlockVariable varFirst, IProgramChunk varFirstChunk,
                                                     BlockVariable varSecond, IProgramChunk varSecondChunk)
        {

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

    enum Equality
    {
        EQUAL,
        NOT_EQUAL,
        GREATER,
        GREATER_OREQUAL,
        LESSER,
        LESSER_OREQUAL
    }
}
