using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueType = MyInterpreter.ValueType;
using Array = MyInterpreter.Array;

namespace MyInterpreter.AST
{
    internal class ArrayCreator : Expression
    {
        public readonly ValueType ArrayElementsType;
        public readonly List<Expression> Sizes;

        public ArrayCreator(ValueType arrayElementsType, List<Expression> sizes, Position position)
            : base(position)
        {
            if (arrayElementsType == null || sizes == null)
            {
                throw new ArgumentNullException();
            }
            if (sizes.Count <= 0)
            {
                throw new ArgumentException();
            }
            ArrayElementsType = arrayElementsType;
            Sizes = sizes;
        }

        internal override Value Calculate(IVariableToValue<string, Pair<Value, Variable.Type>> variableToValue, params Value[] values)
        {
            if (values.Length != Sizes.Count)
            {
                throw new ArgumentException();
            }

            List<int> sizes = new List<int>();
            for (int i = 0; i < values.Length; i++)
            {
                sizes.Add((values[i] as Int).IntValue);
            }

            return CreateArray(0, ValueType.Array(ArrayElementsType, values.Length).InternalType, sizes);
        }

        private Value CreateArray(int indexDimension, ValueType currentElementsType, List<int> sizes)
        {

            Array array = new Array(currentElementsType, sizes[indexDimension]);

            for (int i = 0; i < sizes[indexDimension]; i++)
            {
                if (indexDimension == sizes.Count - 1)
                {
                    array[new Int(i)].Set(currentElementsType.CreateValue());
                }
                else
                {
                    array[new Int(i)].Set(CreateArray(indexDimension + 1, currentElementsType.InternalType, sizes));
                }
            }

            return array;
        }

        private int SizeValueToInt(Value value, Expression expr)
        {
            Int number = value as Int;
            if (number == null)
            {
                throw new InterpretException(InterpretException.ErrorType.ImplicitConversion,
                    new string[] { value.Type.ToString(), ValueType.Int.ToString() }, expr.Position);
            }

            int size = number.IntValue;

            if (size <= 0)
            {
                throw new InterpretException(InterpretException.ErrorType.WrongArraySize, Position);
            }

            return size;
        }
    }
}
