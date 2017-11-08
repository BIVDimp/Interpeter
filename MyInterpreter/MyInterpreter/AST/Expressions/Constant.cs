using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal sealed class Constant : Expression
    {
        public Value ConstantValue;

        public Constant(Value value, Position position)
            : base(position)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            ConstantValue = value;
        }

        internal override Value Calculate(IVariableToValue<string, Pair<Value, Variable.Type>> variableToValue, params Value[] values)
        {
            if (values.Length != 0)
            {
                throw new ArgumentException();
            }
            return ConstantValue;
        }
    }
}
