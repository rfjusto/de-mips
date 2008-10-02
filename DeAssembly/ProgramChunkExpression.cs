/////////////////////////////////////////////////////////////////////////
//                                                                     //
//    DeMIPS - A MIPS decompiler                                       //
//                                                                     //
//        Copyright (c) 2008 by Ruben Acuna and Michael Bradley        //
//                                                                     //
// This file is part of DeMIPS.                                        //
//                                                                     //
// DeMIPS is free software; you can redistribute it and/or             //
// modify it under the terms of the GNU Lesser General Public          //
// License as published by the Free Software Foundation; either        //
// version 3 of the License, or (at your option) any later version.    //
//                                                                     //
// This library is distributed in the hope that it will be useful,     //
// but WITHOUT ANY WARRANTY; without even the implied warranty of      //
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the       //
// GNU Lesser General Public License for more details.                 //
//                                                                     //
// You should have received a copy of the GNU Lesser General Public    //
// License along with this library; if not, write to the Free Software //
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA           //
// 02111-1307, USA, or contact the author(s):                          //
//                                                                     //
// Ruben Acuna <flyingfowlsoftware@earthlink.net>                      //
//                                                                     //
/////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

//Useful link: http://www.sssoftware.com/docs/wnadoc/glossary.html

namespace DeMIPS
{
    //NOTE: this class is designed to be recursive in nature.
    class ProgramChunkExpression : IProgramChunk
    {
        #region variables

        private Queue<ProgramChunkExpressionTerm> terms;

        #endregion

        #region properties

        public Queue<ProgramChunkExpressionTerm> Terms
        {
            get { return terms; }
            set { terms = value; }
        }

        #endregion

        #region constructor

        public ProgramChunkExpression(ProgramChunkExpressionTerm startingTerm)
        {
            Terms = new Queue<ProgramChunkExpressionTerm>();
           
            Terms.Enqueue(startingTerm);
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

    class ProgramChunkTermConstant : ProgramChunkExpressionTerm
    {
        #region variables

        int constant;
        Operand op;

        #endregion

        #region properties

        public int Constant
        {
            get { return constant; }
            set { constant = value; }
        }

        public Operand Operator
        {
            get { return op; }
            set { op = value; }
        }

        #endregion

        #region constructor

        public ProgramChunkTermConstant(int constant) : this(Operand.ADDITION, constant)
        {
        }

        public ProgramChunkTermConstant(Operand op, int constant)
        {
            Operator = op;
            Constant = Math.Abs(constant);
        }

        //TODO: toString? Or should that reside in the backend?

        #endregion

        #region interface methods

        public bool UsesVariable(BlockVariable variable)
        {
            return false;
        }

        #endregion
    }

    interface ProgramChunkExpressionTerm
    {
        //NOTE: Don't extend IProgramChunk, do that would mean
        //we accept any valid ProgramChunk not just terms.
        bool UsesVariable(BlockVariable variable);

        Operand Operator
        {
            get;
            set;
        }
    }

    enum Operand
    {
        ADDITION,
        SUBTRACTION,
        MULTIPLICATION,
        DIVISION,
        EQUAL
    }
}
