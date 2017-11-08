using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal class Variable : Expression
    {
        public enum Type
        {
            Assigned,
            Unassigned
        }

        public enum AccessorType
        {
            Get,
            Set
        }

        public readonly string Name;
        public AccessorType Accessor;

        public Variable(string name, AccessorType accessor, Position position)
            : base(position)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException();
            }
            this.Name = name;
            this.Accessor = accessor;
        }

        internal override Value Calculate(IVariableToValue<string, Pair<Value, Variable.Type>> variableToValue, params Value[] values)
        {
            if (values.Length != 0)
            {
                throw new ArgumentException();
            }

            Pair<Value, Variable.Type> valuePair;

            if (!variableToValue.TryGetValue(Name, out valuePair))
            {
                throw new InterpretException(InterpretException.ErrorType.NotDeclaredVariable, Name, Position);
            }

            if (valuePair.Second == Type.Unassigned && Accessor == AccessorType.Get)
            {
                throw new InterpretException(InterpretException.ErrorType.VariableIsNotSet, Name, Position);
            }

            return valuePair.First;
        }
    }
}
