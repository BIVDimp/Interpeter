using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal class FunctionCallExpr : Expression
    {
        public readonly FunctionCall FunctionCall;

        public FunctionCallExpr(FunctionCall functionCall, Position position)
            : base(position)
        {
            if (functionCall == null)
            {
                throw new ArgumentNullException();
            }
            this.FunctionCall = functionCall;
        }

        internal override Value Calculate(IVariableToValue<string, Pair<Value, Variable.Type>> variableToValue, params Value[] values)
        {
            throw new NotSupportedException();
        }
    }
}
