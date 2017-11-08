using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyInterpreter.AST;

namespace MyInterpreter
{
    public enum CalculatorState
    {
        Done,
        Calculated,
        ExpectedValue
    }

    public sealed class ExpressionCalculator
    {
        public Value ExpectationValue
        {
            set
            {
                if (value == default(Value))
                {
                    throw new ArgumentNullException("Калькулятору передано значение null");
                }
                if (isExpectationValueSet)
                {
                    throw new InvalidOperationException();
                }
                isExpectationValueSet = true;
                State = CalculatorState.Calculated;
                valueList[index] = value;
                index--;
            }
        }
        public Statement NextStatement { get; private set; }
        public Value[] Result { get; private set; }

        //exprList in reverse notation, calculation begins with the end
        private readonly List<Pair<Expression, int>> exprList = new List<Pair<Expression, int>>();
        private readonly List<Value> valueList = new List<Value>();

        private int index;
        public CalculatorState State { get; private set; }
        private bool isExpectationValueSet = false;

        internal ExpressionCalculator(Expression[] expressions)
        {
            if (expressions == null)
            {
                throw new ArgumentNullException();
            }

            for (int i = expressions.Length - 1; i >= 0; i--)
            {
                this.exprList.Add(new Pair<Expression, int>(expressions[i], 0));
                this.valueList.Add(default(Value));
            }

            ExpandExpressionList();
            index = this.exprList.Count - 1;
            State = CalculatorState.Calculated;
            isExpectationValueSet = true;
            NextStatement = null;
            Result = new Value[expressions.Length];
        }

        internal CalculatorState Calculate(IVariableToValue<string, Pair<Value, Variable.Type>> variableToValue)
        {
            switch (State)
            {
                case CalculatorState.Done:
                    return CalculatorState.Done;
                case CalculatorState.ExpectedValue:
                    return CalculatorState.ExpectedValue;
                case CalculatorState.Calculated:
                    isExpectationValueSet = false;
                    NextStatement = null;
                    break;
                default:
                    throw new NotSupportedException();
            }

            for (; index >= 0; index--)
            {
                if (exprList[index].First is FunctionCallExpr)
                {
                    NextStatement = (exprList[index].First as FunctionCallExpr).FunctionCall;
                    return State = CalculatorState.ExpectedValue;
                }
                int internalExpressionsNumber = GetInternalExpressionsNumber(exprList[index].First);
                Value[] parameters = new Value[internalExpressionsNumber];

                for (int i = 0; i < internalExpressionsNumber; i++)
                {
                    parameters[i] = valueList[exprList[index].Second + internalExpressionsNumber - 1 - i];
                }

                valueList[index] = exprList[index].First.Calculate(variableToValue, parameters);
            }

            for (int i = 0; i < Result.Length; i++)
            {
                Result[i] = valueList[Result.Length - 1 - i];
            }

            return State = CalculatorState.Done;
        }

        private void ExpandExpressionList()
        {
            Expression[] internalExpressions;
            int length;

            for (int i = 0; i < exprList.Count; i++)
            {
                internalExpressions = GetInternalExpressions(exprList[i].First);
                exprList[i].Second = exprList.Count;
                length = internalExpressions.Length;

                for (int j = 0; j < length; j++)
                {
                    exprList.Add(new Pair<Expression, int>(internalExpressions[length - 1 - j], 0));
                    valueList.Add(default(Value));
                }
            }
        }

        private Expression[] GetInternalExpressions(Expression expr)
        {
            if (expr is ArrayCreator)
            {
                ArrayCreator temp = expr as ArrayCreator;
                return temp.Sizes.ToArray();
            }
            else if (expr is BinaryOperation)
            {
                BinaryOperation temp = expr as BinaryOperation;
                return new Expression[] { temp.LeftOperand, temp.RightOperand };
            }
            else if (expr is Condition)
            {
                Condition temp = expr as Condition;
                return new Expression[] { temp.LeftOperand, temp.RightOperand };
            }
            else if (expr is Constant)
            {
                return new Expression[] { };
            }
            else if (expr is FunctionCallExpr)
            {
                return new Expression[] { };
            }
            else if (expr is Method)
            {
                Method temp = expr as Method;
                return new Expression[] { temp.Expr };
            }
            else if (expr is Slice)
            {
                Slice temp = expr as Slice;
                return new Expression[] { temp.Collection, temp.Indexer };
            }
            else if (expr is UnaryOperation)
            {
                UnaryOperation temp = expr as UnaryOperation;
                return new Expression[] { temp.Expr };
            }
            else if (expr is Variable)
            {
                return new Expression[] { };
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private int GetInternalExpressionsNumber(Expression expr)
        {
            return GetInternalExpressions(expr).Length;
        }
    }
}
