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

using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class ProgramBlock : IProgramChunk
    {
        #region variables

        private LinkedList<BlockVariable> variables;
        private LinkedList<IProgramChunk> program;
        private LinkedList<IProgramChunk> programOrphanJumpTargets;
        private LinkedList<IProgramChunk> programAllJumpTargets;    //This is used so that the loop routine can perform a check on what branches are there
        private LinkedList<IProgramChunk> programAllIncrementers;    //This is used so that the loop routine can perform a check on what incrementers are there

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

        public LinkedList<IProgramChunk> ProgramAllJumpTargets
        {
            get { return programAllJumpTargets; }
            set { programAllJumpTargets = value; }
        }
        public LinkedList<IProgramChunk> ProgramAllIncrementers
        {
            get { return programAllIncrementers; }
            set { programAllIncrementers = value; }
        }

        #endregion

        #region constructor

        public ProgramBlock()
        {
            Variables = new LinkedList<BlockVariable>();
            Program = new LinkedList<IProgramChunk>();
            ProgramOrphanJumpTargets = new LinkedList<IProgramChunk>();
            ProgramAllJumpTargets = new LinkedList<IProgramChunk>();
            ProgramAllIncrementers = new LinkedList<IProgramChunk>();
        }

        #endregion

        #region general methods

        public bool IsSane()
        {
            if (ProgramOrphanJumpTargets.Count > 0)
                return false;

            return true;
        }

        //returns true if any chunks or subblocks use a variable.
        public bool UsesVariable(BlockVariable variable)
        {
            foreach(IProgramChunk chunk in program)
                if(chunk.UsesVariable(variable))
                    return true;

            return false;
        }

        //Returns a ProgramBlock from a given list of IProgramChunks.
        public ProgramBlock GenerateSubProgramBlock(LinkedList<IProgramChunk> chunks)
        {
            ProgramBlock subBlock = new ProgramBlock();

            //Move chunks to new ProgramBlock.
            foreach (IProgramChunk chunk in chunks)
                subBlock.Program.AddLast(chunk);

            //Remove those chunks from this ProgramBlock.
            foreach (IProgramChunk chunk in chunks)
                program.Remove(chunk);

            return subBlock;
        }

        #endregion

        #region loop methods

        /*
        // This piece of code was intended to be used, but was not deleted because it might have later use
        //This function returns true if every chunk in listToFind is present in listToSearch
        //As of now, used for detecting everything in a loop, can supposedly be used for all types of loops
        public bool FindAllChunks(LinkedList<IProgramChunk> listToSearch, LinkedList<IProgramChunk> listToFind)
        {
            int found = 0;
            foreach (IProgramChunk chunk in listToFind)
            {
                //Check if the list to search contains each entry in the list to find (one at a time using foreach loop)
                if (listToSearch.Contains(chunk) == true)
                {
                    found++;
                }
            }
            //Check if number of items found is the same as the number of items in the linked list
            if (found == listToFind.Count)
                return true;

            return false;
        }*/

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

        /// <summary>
        /// Searches for variable in the pool of local variables. 
        /// If it doesn't exist, create a new one, add it to the
        /// pool, and return it.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <returns>BlockVariable. Never null.</returns>
        public BlockVariable GetVariableByNameForced(string name)
        {
            BlockVariable variable = GetVariableByName(name);

            if (variable == null)
            {
                variable = new BlockVariable(name);
                AddVariable(variable);
            }

            return variable;
        }

        #endregion

        #region debug methods

        #endregion
    }

    //TODO: implement me!??!?!
    class BlockVariableTerm : BlockVariable, ProgramChunkExpressionTerm
    {
        public BlockVariableTerm(string name) : base (name)
        {

        }
    }

    class BlockVariable : ProgramChunkExpressionTerm
    {
        #region variables

        private string name;
        private VariableType type;
        private VariableSource source;

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

        public VariableSource Source
        {
            get { return source; }
            set { source = value; }
        }

        #endregion

        #region constructor

        public BlockVariable(string name)
            : this(name, VariableType.UNDEFINED, VariableSource.UNDEFINED)
        {
            //this function
        }

        public BlockVariable(string name, VariableType type, VariableSource source)
        {
            Name = name;
            Type = type;
            Source = source;
        }

        #endregion

        #region methods

        public bool Equals(BlockVariable variable)
        {
            if (this.Name.Equals(variable.Name))
            {
                if (this.Type.Equals(variable.Type) || !(variable.Type == VariableType.UNDEFINED))
                    if (this.Source.Equals(variable.Source) || !(variable.Source == VariableSource.UNDEFINED))
                        return true;
            }
            
            return false;
        }

        public bool UsesVariable(BlockVariable variable)
        {
            //equals shouldn't be used here but that would be more standard
            if (Name == variable.Name)
                return true;

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

    //NOTE: this actually doubles as scope.
    enum VariableSource
    {
        UNDEFINED,
        ARGUMENT,
        LOCAL,
        GLOBAL,
        RETURN
    }
}
