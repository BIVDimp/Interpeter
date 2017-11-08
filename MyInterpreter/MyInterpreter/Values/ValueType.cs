using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter
{
    public sealed class ValueType
    {
        public enum Type
        {
            Bool,
            Double,
            Int,
            String,
            Char,
            Array,
            Void
        }

        public static readonly ValueType Bool = new ValueType(Type.Bool);
        public static readonly ValueType Double = new ValueType(Type.Double);
        public static readonly ValueType Int = new ValueType(Type.Int);
        public static readonly ValueType String = new ValueType(Type.String);
        public static readonly ValueType Char = new ValueType(Type.Char);
        public static readonly ValueType Void = new ValueType(Type.Void);

        public readonly Type SimpleType;
        private readonly ValueType internalType;

        public ValueType InternalType
        {
            get
            {
                if (SimpleType != Type.Array)
                {
                    throw new InvalidOperationException();
                }
                return internalType;
            }
        }

        private ValueType(Type type)
        {
            this.SimpleType = type;
            this.internalType = null;
        }

        private ValueType(Type type, ValueType internalType)
        {
            if (type != Type.Array)
            {
                throw new ArgumentException();
            }

            if (internalType == null)
            {
                throw new ArgumentNullException();
            }

            this.SimpleType = type;
            this.internalType = internalType;
        }

        public static ValueType Array(ValueType internalType)
        {
            return new ValueType(Type.Array, internalType);
        }

        public static ValueType Array(ValueType internalType, int dimension)
        {
            if (dimension <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            ValueType answer = internalType;

            while (dimension > 0)
            {
                answer = Array(answer);
                dimension--;
            }

            return answer;
        }

        public Value CreateValue()
        {
            return CreateValue(this);
        }

        public static Value CreateValue(ValueType valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException();
            }

            switch (valueType.SimpleType)
            {
                case Type.Bool:
                    return new Bool();
                case Type.Double:
                    return new Double();
                case Type.String:
                    return new String();
                case Type.Int:
                    return new Int();
                case Type.Array:
                    return new Array(valueType.InternalType);
                case Type.Void:
                    return new Void();
                default:
                    throw new NotImplementedException();
            }
        }

        public static bool Equals(ValueType first, ValueType second)
        {
            if (first == null && second == null)
            {
                return false;
            }

            while (first != null && second != null)
            {
                if (first.SimpleType != second.SimpleType)
                {
                    return false;
                }

                first = first.internalType;
                second = second.internalType;
            }

            return first == second;
        }

        public override string ToString()
        {
            string answer = string.Empty;
            ValueType currentValueType = this;

            while (currentValueType.SimpleType == Type.Array)
            {
                answer += "[]";
                currentValueType = currentValueType.internalType;
            }

            answer = currentValueType.SimpleType.ToString().ToLowerInvariant() + answer;
            return answer;
        }
    }
}