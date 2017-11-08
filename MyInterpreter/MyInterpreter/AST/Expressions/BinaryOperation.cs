using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal enum BinaryOperationType
    {
        Plus,
        Minus,
        Multiply,
        Divide,
        Degree,
        LogicalAnd,
        LogicalOr
    }

    internal class BinaryOperation : Expression
    {
        public readonly Expression LeftOperand;
        public readonly Expression RightOperand;
        public readonly BinaryOperationType Operation;

        public BinaryOperation(Expression leftOperand, BinaryOperationType operation, Expression rightOperand, Position position)
            : base(position)
        {
            if (leftOperand == null || rightOperand == null)
            {
                throw new ArgumentNullException();
            }
            LeftOperand = leftOperand;
            Operation = operation;
            RightOperand = rightOperand;
        }

        internal override Value Calculate(IVariableToValue<string, Pair<Value, Variable.Type>> variableToValue, params Value[] values)
        {
            if (values.Length != 2)
            {
                throw new ArgumentException();
            }

            Value leftValue = values[0];
            Value rightValue = values[1];

            try
            {
                switch (Operation)
                {
                    case BinaryOperationType.Plus:
                        return leftValue.Add(rightValue);
                    case BinaryOperationType.Minus:
                        return leftValue.Subtract(rightValue);
                    case BinaryOperationType.Multiply:
                        return leftValue.Multiply(rightValue);
                    case BinaryOperationType.Divide:
                        return leftValue.Divide(rightValue);
                    case BinaryOperationType.Degree:
                        return leftValue.Power(rightValue);
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
