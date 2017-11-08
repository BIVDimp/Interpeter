using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    public abstract class Expression : Node
    {
        protected Expression(Position position)
            : base(position)
        {
        }

        internal abstract Value Calculate(IVariableToValue<string, Pair<Value, Variable.Type>> variableToValue, params Value[] values);
    }
}
