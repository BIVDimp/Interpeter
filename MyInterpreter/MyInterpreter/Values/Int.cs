using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueType = MyInterpreter.ValueType;

namespace MyInterpreter
{
    internal class Int : Value
    {
        public int IntValue { get; private set; }

        public Int()
            : base(ValueType.Int)
        {
            IntValue = 0;
        }

        public Int(int value)
            : base(ValueType.Int)
        {
            IntValue = value;
        }

        public Int(Int value)
            : base(ValueType.Int)
        {
            IntValue = value.IntValue;
        }

        public override Value Set(Value value)
        {
            Int valueAsInt = value as Int;

            if (valueAsInt == null)
            {
                throw new ValueException(ValueException.ErrorType.ImplicitConversion,
                    new string[] { value.Type.ToString(), Type.ToString() });
            }

            this.IntValue = valueAsInt.IntValue;
            return new Int(this);
        }

        public override Value Add()
        {
            return new Int(IntValue);
        }

        public override Value Subtract()
        {
            return new Int(-IntValue);
        }

        public override Value Add(Value value)
        {
            CheckForNull(value);

            if (value is Double)
            {
                return this.ToDoubleValue().Add(value);
            }

            Int rightValue = value as Int;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.BinaryPlus);
            }

            int result = IntValue + rightValue.IntValue;

            return new Int(result);
        }

        public override Value Subtract(Value value)
        {
            CheckForNull(value);

            if (value is Double)
            {
                return this.ToDoubleValue().Subtract(value);
            }

            Int rightValue = value as Int;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.BinaryMinus);
            }

            int result = IntValue - rightValue.IntValue;

            return new Int(result);
        }

        public override Value Multiply(Value value)
        {
            CheckForNull(value);

            if (value is Double)
            {
                return this.ToDoubleValue().Multiply(value);
            }

            Int rightValue = value as Int;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.Multiply);
            }

            int result = IntValue * rightValue.IntValue;

            return new Int(result);
        }

        public override Value Divide(Value value)
        {
            CheckForNull(value);

            if (value is Double)
            {
                return this.ToDoubleValue().Divide(value);
            }

            Int rightValue = value as Int;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.Divide);
            }

            if (rightValue.IntValue == 0)
            {
                throw new ValueException(ValueException.ErrorType.DivisionByZero);
            }

            int result = IntValue / rightValue.IntValue;

            return new Int(result);
        }

        public override Value IsEqual(Value value)
        {
            CheckForNull(value);

            if (value is Double)
            {
                return this.ToDoubleValue().IsEqual(value);
            }

            Int rightValue = value as Int;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.Equal);
            }

            return IsEqual(rightValue);
        }

        public override Value IsNotEqual(Value value)
        {
            CheckForNull(value);

            if (value is Double)
            {
                return this.ToDoubleValue().IsNotEqual(value);
            }

            Int rightValue = value as Int;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.NotEqual);
            }

            return IsNotEqual(rightValue);
        }

        public override Value IsLess(Value value)
        {
            CheckForNull(value);

            if (value is Double)
            {
                return this.ToDoubleValue().IsLess(value);
            }

            Int rightValue = value as Int;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.Less);
            }

            return IsLess(rightValue);
        }

        public override Value IsLessOrEqual(Value value)
        {
            CheckForNull(value);

            if (value is Double)
            {
                return this.ToDoubleValue().IsLessOrEqual(value);
            }

            Int rightValue = value as Int;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.LessOrEqual);
            }

            return IsLessOrEqual(rightValue);
        }

        public override Value IsGreater(Value value)
        {
            CheckForNull(value);

            if (value is Double)
            {
                return this.ToDoubleValue().IsGreater(value);
            }

            Int rightValue = value as Int;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.Greater);
            }

            return IsGreater(rightValue);
        }

        public override Value IsGreaterOrEqual(Value value)
        {
            CheckForNull(value);

            if (value is Double)
            {
                return this.ToDoubleValue().IsGreaterOrEqual(value);
            }

            Int rightValue = value as Int;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.GreaterOrEqual);
            }

            return IsGreaterOrEqual(rightValue);
        }

        private Bool IsEqual(Int value)
        {
            return new Bool(IntValue == value.IntValue);
        }

        private Bool IsNotEqual(Int value)
        {
            return new Bool(IntValue != value.IntValue);
        }

        private Bool IsLess(Int value)
        {
            return new Bool(IntValue < value.IntValue);
        }

        private Bool IsLessOrEqual(Int value)
        {
            return new Bool(IntValue <= value.IntValue);
        }

        private Bool IsGreater(Int value)
        {
            return new Bool(IntValue > value.IntValue);
        }

        private Bool IsGreaterOrEqual(Int value)
        {
            return new Bool(IntValue >= value.IntValue);
        }

        public override bool Equals(Value value)
        {
            Int rightValue = value as Int;
            return rightValue != null && IntValue == rightValue.IntValue;
        }

        public override string ToString()
        {
            return IntValue.ToString();
        }

        public Double ToDoubleValue()
        {
            return new Double((double)IntValue);
        }
    }
}
