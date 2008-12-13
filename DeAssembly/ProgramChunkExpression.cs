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
    class ProgramChunkExpression : IProgramChunk
    {
        #region variables

        ProgramChunkExpressionTerm firstTerm;
        ProgramChunkExpressionTerm secondTerm;
        Operand oper;

        #endregion

        #region properties

        public Operand Oper
        {
            get { return oper; }
            set { oper = value; }
        }

        public ProgramChunkExpressionTerm FirstTerm
        {
            get { return firstTerm; }
            set { firstTerm = value; }
        }

        public ProgramChunkExpressionTerm SecondTerm
        {
            get { return secondTerm; }
            set { secondTerm = value; }
        }

        #endregion

        #region constructor

        public ProgramChunkExpression(ProgramChunkExpressionTerm firstTerm, Operand oper, ProgramChunkExpressionTerm secondTerm)
        {
            FirstTerm = firstTerm;
            SecondTerm = secondTerm;
            Oper = oper;

            /*
            //HACK: simplify minus.
            if (oper == Operand.ADDITION && secondTerm is BlockConstant)
            {
                if (((BlockConstant)secondTerm).Constant < 0)
                {
                    ((BlockConstant)secondTerm).Constant = Math.Abs(((BlockConstant)secondTerm).Constant);
                    oper = Operand.SUBTRACTION;
                }
            }
            */
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

    class BlockConstant : ProgramChunkExpressionTerm
    {
        #region variables

        int constant;

        #endregion

        #region properties

        public int Constant
        {
            get { return constant; }
            set { constant = value; }
        }

        #endregion

        #region constructor

        public BlockConstant(int constant)
        {
            Constant = constant;
        }

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

    }

    enum Operand
    {
        ADDITION,
        SUBTRACTION,
        MULTIPLICATION,
        DIVISION,
        //EQUAL
    }
}
