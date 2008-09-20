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
// Michael Bradley                                                     //
//                                                                     //
/////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class ProgramBlock
    {
        #region variables

        private LinkedList<BlockVariable> variables;
        private LinkedList<IProgramChunk> program;
        private LinkedList<IProgramChunk> programOrphanJumpTargets;

        #endregion

        #region properties

        public LinkedList<BlockVariable> Variables
        {
            get { return variables; }
            set { variables = value; }
        }

        public LinkedList<IProgramChunk> Program
        {
            get { return program; }
            set { program = value; }
        }

        private LinkedList<IProgramChunk> ProgramOrphanJumpTargets
        {
            get { return programOrphanJumpTargets; }
            set { programOrphanJumpTargets = value; }
        }

        #endregion

        #region constructor

        public ProgramBlock()
        {
            Variables = new LinkedList<BlockVariable>();
            Program = new LinkedList<IProgramChunk>();
            ProgramOrphanJumpTargets = new LinkedList<IProgramChunk>();
        }

        #endregion

        #region general methods

        public bool IsSane()
        {
            if (ProgramOrphanJumpTargets.Count > 0)
                return false;

            return true;
        }

        #endregion

        #region jump and label methods

        public void AddOrphanJumpTarget(ProgramChunkJumpTarget target)
        {
            ProgramOrphanJumpTargets.AddLast(((IProgramChunk)target));
        }

        public ProgramChunkJumpTarget GetOrphanJumpTargetByLabel(string label)
        {
            return GetJumpTargetByLabel(label, ProgramOrphanJumpTargets);
        }

        public void RemoveOrphanJumpTarget(ProgramChunkJumpTarget target)
        {
            ProgramOrphanJumpTargets.Remove(target);
        }

        public ProgramChunkJumpTarget GetJumpTargetByLabel(string label)
        {
            return GetJumpTargetByLabel(label, Program);
        }

        private ProgramChunkJumpTarget GetJumpTargetByLabel(string label, LinkedList<IProgramChunk> code)
        {
            foreach (IProgramChunk chunk in code)
            {
                if ((chunk is ProgramChunkJumpTarget) && ((ProgramChunkJumpTarget)chunk).Label.Equals(label))
                {
                    return (ProgramChunkJumpTarget)chunk;
                }
            }

            return null;
        }

        #endregion

        #region variable methods

        public void AddVariable(BlockVariable newVariable)
        {
            Variables.AddLast(newVariable);
        }

        public bool HasVariable(BlockVariable variable)
        {
            if (GetVariable(variable) != null)
                return true;
            else
                return false;
        }

        public BlockVariable GetVariableByName(string name)
        {
            foreach (BlockVariable var in Variables)
                if (var.Name.Equals(name))
                    return var;

            return null;
        }

        public BlockVariable GetVariable(BlockVariable variable)
        {
            foreach (BlockVariable var in Variables)
                if (var.Equals(variable))
                    return var;

            return null;
        }

        #endregion
    }

    class BlockVariable
    {
        #region variables

        private string name;
        private VariableType type;

        #endregion

        #region properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public VariableType Type
        {
            get { return type; }
            set { type = value; }
        }

        #endregion

        #region constructor

        public BlockVariable(string myName, VariableType myType)
        {
            Name = myName;
            Type = myType;
        }

        #endregion

        #region methods

        public bool Equals(BlockVariable variable)
        {
            if (this.Name.Equals(variable.Name) && this.Type.Equals(variable.Type))
                return true;
            else
                return false;
        }

        #endregion

        #region IComparable

        public int CompareTo(object obj)
        {
            BlockVariable variable = (BlockVariable)obj;
            return Name.CompareTo(variable.Name);
        }

        #endregion
    }

    enum VariableType
    {
        UNDEFINED,  //?-bits
        BIT,        //1-bit (bool)
        BYTE,       //8-bits
        WORD,       //32-bits
        DOUBLE_WORD //64-bits
    }

    enum VariableSource
    {
        ARGUMENT,
        LOCAL,
        GLOBAL,
        RETURN
    }
}
