using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal enum CompareOperator
    {
        None,
        IsEqual,
        IsNotEqual,
        IsLess,
        IsLessOrEqual,
        IsGreater,
        IsGreaterOrEqual
    }

    public class Condition : Expression
    {
        internal readonly Expression LeftOperand;
        internal readonly CompareOperator Operator;
        internal readonly Expression RightOperand;

        internal Condition(Position position)
            : this(null, CompareOperator.None, null, position)
        {
        }

        internal Condition(Expression leftOperand, CompareOperator comparer, Expression rightOperand, Position position)
            : base(position)
        {
            if (leftOperand == null && (comparer != CompareOperator.None || rightOperand != null))
            {
                throw new ArgumentNullException();
            }

            LeftOperand = leftOperand;
            Operator = comparer;
            RightOperand = rightOperand;
        }

        internal override Value Calculate(IVariableToValue<string, Pair<Value, Variable.Type>> variableToValue, params Value[] values)
        {
            if (!(values.Length == 0 && IsEmptyCondition()) && !(values.Length == 2 && !IsEmptyCondition()))
            {
                throw new ArgumentException();
            }

            if (IsEmptyCondition())
            {
                return new Bool(true);
            }

            Value leftValue = values[0];
            Value rightValue = values[1];

            try
            {
                switch (Operator)
                {
                    case CompareOperator.IsEqual:
                        return leftValue.IsEqual(rightValue);
                    case CompareOperator.IsNotEqual:
                        return leftValue.IsNotEqual(rightValue);
                    case CompareOperator.IsLess:
                        return leftValue.IsLess(rightValue);
                    case CompareOperator.IsLessOrEqual:
                        return leftValue.IsLessOrEqual(rightValue);
                    case CompareOperator.IsGreater:
                        return leftValue.IsGreater(rightValue);
                    case CompareOperator.IsGreaterOrEqual:
                        return leftValue.IsGreaterOrEqual(rightValue);
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (ValueException exception)
            {
                throw InterpretException.BuildException(exception, Position);
            }
        }

        private bool IsEmptyCondition()
        {
            return LeftOperand == null;
        }
    }
}
