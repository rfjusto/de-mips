using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    //TODO: Implement me!
    class ProgramChunkBranch : IProgramChunk
    {
        Equality equality;
        ProgramChunkExpressionTerm termFirst;
        ProgramChunkExpressionTerm termSecond;
        IProgramChunk trueChunk;
        IProgramChunk falseChunk;

        public Equality EqualityChecked
        {
            get { return equality; }
            set { equality = value; }
        }

        public ProgramChunkExpressionTerm TermFirst
        {
            get { return termFirst; }
            set { termFirst = value; }
        }

        public ProgramChunkExpressionTerm TermSecond
        {
            get { return termSecond; }
            set { termSecond = value; }
        }

        public IProgramChunk TrueChunk
        {
            get { return trueChunk; }
            set { trueChunk = value; }
        }

        public IProgramChunk FalseChunk
        {
            get { return falseChunk; }
            set { falseChunk = value; }
        }

        #region constructor

        public ProgramChunkBranch(Equality equality, ProgramChunkExpressionTerm termFirst, IProgramChunk trueChunk,
                                                     ProgramChunkExpressionTerm termSecond, IProgramChunk falseChunk)
        {
            this.equality = equality;
            this.termFirst = termFirst;
            this.termSecond = termSecond;
            this.trueChunk = trueChunk;
            this.falseChunk = falseChunk;
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
