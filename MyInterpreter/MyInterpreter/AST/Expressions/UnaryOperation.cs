using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    public enum UnaryOperationType
    {
        Plus,
        Minus
    }

    internal class UnaryOperation : Expression
    {
        public readonly UnaryOperationType Operation;
        public readonly Expression Expr;

        public UnaryOperation(UnaryOperationType operation, Expression expression, Position position)
            : base(position)
        {
            if (expression == null)
            {
                throw new ArgumentNullException();
            }
            Operation = operation;
            Expr = expression;
        }

        internal override Value Calculate(IVariableToValue<string, Pair<Value, Variable.Type>> variableToValue, params Value[] values)
        {
            if (values.Length != 1)
            {
                throw new ArgumentException();
            }

            Value value = values[0];

            try
            {
                switch (Operation)
                {
                    case UnaryOperationType.Plus:
                        return value.Add();
                    case UnaryOperationType.Minus:
                        return value.Subtract();
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (ValueException exception)
            {
                throw InterpretException.BuildException(exception, Position);
            }
        }
    }
}
