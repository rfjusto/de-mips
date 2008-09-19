using System;
using System.Collections.Generic;
using System.Text;

/*
 *TODO LIST:
 *Add { } to C language emitter.
 *
*/
namespace DeMIPS
{
    class ProgramBlock
    {
        #region variables

        private LinkedList<BlockVariable> variables;

        #endregion

        #region properties

        public LinkedList<BlockVariable> Variables
        {
            get { return variables; }
            set { variables = value; }
        }

        #endregion

        #region constructor

        public ProgramBlock()
        {
            Variables = new LinkedList<BlockVariable>();
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
