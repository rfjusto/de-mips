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
// Michael Bradley <mbradley1372@cox.net>                              //
//                                                                     //
/////////////////////////////////////////////////////////////////////////

//This is the class used for loops (based off of ProgramChunkJumpTarget.cs)

using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class ProgramChunkLoop : IProgramChunk
    {
        #region variables

        private IProgramChunk loopLabel;
        private LinkedList<IProgramChunk> loopCode;

        #endregion

        #region properties

        public IProgramChunk LoopLabel
        {
            get { return loopLabel; }
            set { loopLabel = value; }
        }

        #endregion

        #region constructor

        public ProgramChunkLoop(IProgramChunk LoopLabel2)
        {
            LoopLabel = LoopLabel2;
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
}