using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueType = MyInterpreter.ValueType;

namespace MyInterpreter
{
    internal class Array : Value
    {
        public Value[] ArrayValue { get; private set; }

        public Array(ValueType internalType)
            : base(ValueType.Array(internalType))
        {
        }

        public Array(ValueType internalType, int size)
            : base(ValueType.Array(internalType))
        {
            ArrayValue = new Value[size];

            for (int i = 0; i < size; i++)
            {
                ArrayValue[i] = Type.InternalType.CreateValue();
            }
        }

        public override Value Set(Value value)
        {
            Array arrayValue = value as Array;
            if (arrayValue == null || !ValueType.Equals(Type, arrayValue.Type))
            {
                throw new ValueException(ValueException.ErrorType.ImplicitConversion,
                    new string[] { value.Type.ToString(), Type.ToString() });
            }
            this.ArrayValue = arrayValue.ArrayValue;
            return this;
        }

        public override Value this[Value indexValue]
        {
            get
            {
                int index = GetIndex(indexValue);
                if (index < 0 || index >= ArrayValue.Length)
                {
                    throw new ValueException(ValueException.ErrorType.IndexOutOfRange);
                }
                return ArrayValue[index];
            }
            set
            {
                int index = GetIndex(indexValue);
                if (index < 0 || index >= ArrayValue.Length)
                {
                    throw new ValueException(ValueException.ErrorType.IndexOutOfRange);
                }
                ArrayValue[index].Set(value);
            }
        }

        private int GetIndex(Value indexValue)
        {
            Int number = indexValue as Int;
            if (number == null)
            {
                throw new ValueException(ValueException.ErrorType.ImplicitConversion,
                    new string[] { indexValue.Type.ToString(), ValueType.Int.ToString() });
            }
            return number.IntValue;
        }

        public override string ToString()
        {
            if (ArrayValue == null)
            {
                return "null";
            }

            StringBuilder answer = new StringBuilder();

            answer.Append("{ ");
            for (int i = 0; i < ArrayValue.Length; i++)
            {
                if (i != 0)
                {
                    answer.Append(", ");
                }
                answer.Append(ArrayValue[i].ToString());
            }
            answer.Append(" }");

            return answer.ToString();
        }
    }
}
