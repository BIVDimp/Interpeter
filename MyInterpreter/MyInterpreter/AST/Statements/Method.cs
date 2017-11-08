using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal enum MethodType
    {
        Sin,
        Cos
    }

    internal class Method : Expression
    {
        public readonly MethodType Function;
        public readonly Expression Expr;

        public Method(MethodType function, Expression expr, Position position)
            : base(position)
        {
            if (expr == null)
            {
                throw new ArgumentNullException();
            }
            Function = function;
            Expr = expr;
        }

        internal override Value Calculate(IVariableToValue<string, Pair<Value, Variable.Type>> variableToValue, params Value[] values)
        {
            if (values.Length != 1)
            {
                throw new ArgumentException();
            }

            Value value = values[0];
            if (value is Int)
            {
                value = (value as Int).ToDoubleValue();
            }
            Double valueAsDouble = value as Double;

            if (valueAsDouble == null)
            {
                throw new InterpretException(InterpretException.ErrorType.ImplicitConversion,
                    new string[] { value.Type.ToString(), ValueType.Double.ToString() },
                    Expr.Position);
            }

            try
            {
                switch (Function)
                {
                    case MethodType.Sin:
                        return Double.Sin(valueAsDouble);
                    case MethodType.Cos:
                        return Double.Cos(valueAsDouble);
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (ValueException exception)
            {
                throw InterpretException.BuildException(exception, Position);
            }
        }
    }
}
