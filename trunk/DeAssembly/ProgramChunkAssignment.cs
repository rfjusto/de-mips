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

namespace DeMIPS
{
    /// <summary>
    /// This stores an expression in the form of 
    /// variable = expression, where variable and expression
    /// are objects.
    /// </summary>
    class ProgramChunkAssignment : IProgramChunk
    {
        #region variables

        private BlockVariable variable;
        private ProgramChunkExpression expression;

        #endregion

        #region properties

        public BlockVariable Variable
        {
            get { return variable; }
            set { variable = value; }
        }

        public ProgramChunkExpression Expression
        {
            get { return expression; }
            set { expression = value; }
        }

        #endregion

        #region constructor

        //NOTE: we only allow the assignment to a variable, constants
        //      can't change value.
        public ProgramChunkAssignment(BlockVariable variable, ProgramChunkExpression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        #endregion

        #region interface methods

        public bool UsesVariable(BlockVariable variable)
        {
            if(Variable.Equals(variable))
                return true;

            if(Expression.Equals(variable))
                return true;

            return false;
        }

        #endregion
    }
}
