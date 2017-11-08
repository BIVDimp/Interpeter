using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueType = MyInterpreter.ValueType;

namespace MyInterpreter
{
    internal class Double : Value
    {
        private const double floatingComparisonThreshold = 0.0000001;

        public double DoubleValue { get; private set; }

        public Double()
            : base(ValueType.Double)
        {
            DoubleValue = 0;
        }

        public Double(double value)
            : base(ValueType.Double)
        {
            DoubleValue = value;
        }

        public Double(Double value)
            : base(ValueType.Double)
        {
            DoubleValue = value.DoubleValue;
        }

        public override Value Set(Value value)
        {
            value = TryConvertTypeToDouble(value);
            Double valueAsDouble = value as Double;

            if (valueAsDouble == null)
            {
                throw new ValueException(ValueException.ErrorType.ImplicitConversion,
                    new string[] { value.Type.ToString(), Type.ToString() });
            }

            this.DoubleValue = valueAsDouble.DoubleValue;
            return new Double(this);
        }

        public override Value Add()
        {
            return new Double(DoubleValue);
        }

        public override Value Subtract()
        {
            return new Double(-DoubleValue);
        }

        public override Value Add(Value value)
        {
            CheckForNull(value);
            value = TryConvertTypeToDouble(value);

            Double rightValue = value as Double;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.BinaryPlus);
            }

            double result = DoubleValue + rightValue.DoubleValue;

            if (double.IsInfinity(result))
            {
                throw new ValueException(ValueException.ErrorType.Overflow, Type.ToString());
            }

            return new Double(result);
        }

        public override Value Subtract(Value value)
        {
            CheckForNull(value);
            value = TryConvertTypeToDouble(value);

            Double rightValue = value as Double;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.BinaryMinus);
            }

            double result = DoubleValue - rightValue.DoubleValue;

            if (double.IsInfinity(result))
            {
                throw new ValueException(ValueException.ErrorType.Overflow, Type.ToString());
            }

            return new Double(result);
        }

        public override Value Multiply(Value value)
        {
            CheckForNull(value);
            value = TryConvertTypeToDouble(value);

            Double rightValue = value as Double;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.Multiply);
            }

            double result = DoubleValue * rightValue.DoubleValue;

            if (double.IsInfinity(result))
            {
                throw new ValueException(ValueException.ErrorType.Overflow, Type.ToString());
            }

            return new Double(result);
        }

        public override Value Divide(Value value)
        {
            CheckForNull(value);
            value = TryConvertTypeToDouble(value);

            Double rightValue = value as Double;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.Divide);
            }

            if (AreEqualToZero(rightValue.DoubleValue))
            {
                throw new ValueException(ValueException.ErrorType.DivisionByZero);
            }

            double result = DoubleValue / rightValue.DoubleValue;

            if (double.IsInfinity(result))
            {
                throw new ValueException(ValueException.ErrorType.Overflow, Type.ToString());
            }

            return new Double(result);
        }

        public override Value Power(Value value)
        {
            CheckForNull(value);
            value = TryConvertTypeToDouble(value);

            Double rightValue = value as Double;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.Degree);
            }

            if (AreEqualToZero(DoubleValue) && rightValue.DoubleValue < 0)
            {
                throw new ValueException(ValueException.ErrorType.DivisionByZero);
            }

            double result = Math.Pow(DoubleValue, rightValue.DoubleValue);

            if (double.IsInfinity(result))
            {
                throw new ValueException(ValueException.ErrorType.Overflow, Type.ToString());
            }

            return new Double(result);
        }

        public override Value IsEqual(Value value)
        {
            value = TryConvertTypeToDouble(value);
            Double rightValue = value as Double;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.Equal);
            }

            return IsEqual(rightValue);
        }

        public override Value IsNotEqual(Value value)
        {
            value = TryConvertTypeToDouble(value);
            Double rightValue = value as Double;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.NotEqual);
            }

            return IsNotEqual(rightValue);
        }

        public override Value IsLess(Value value)
        {
            value = TryConvertTypeToDouble(value);
            Double rightValue = value as Double;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.Less);
            }

            return IsLess(rightValue);
        }

        public override Value IsLessOrEqual(Value value)
        {
            value = TryConvertTypeToDouble(value);
            Double rightValue = value as Double;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.LessOrEqual);
            }

            return IsLessOrEqual(rightValue);
        }

        public override Value IsGreater(Value value)
        {
            value = TryConvertTypeToDouble(value);
            Double rightValue = value as Double;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.Greater);
            }

            return IsGreater(rightValue);
        }

        public override Value IsGreaterOrEqual(Value value)
        {
            value = TryConvertTypeToDouble(value);
            Double rightValue = value as Double;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.GreaterOrEqual);
            }

            return IsGreaterOrEqual(rightValue);
        }

        private Bool IsEqual(Double value)
        {
            return new Bool(AreEqual(DoubleValue, value.DoubleValue));
        }

        private Bool IsNotEqual(Double value)
        {
            return new Bool(!AreEqual(DoubleValue, value.DoubleValue));
        }

        private Bool IsLess(Double value)
        {
            return new Bool(value.DoubleValue - DoubleValue > floatingComparisonThreshold);
        }

        private Bool IsLessOrEqual(Double value)
        {
            return IsLess(value).LogicalOr(IsEqual(value));
        }

        private Bool IsGreater(Double value)
        {
            return new Bool(DoubleValue - value.DoubleValue > floatingComparisonThreshold);
        }

        private Bool IsGreaterOrEqual(Double value)
        {
            return IsGreater(value).LogicalOr(IsEqual(value));
        }

        private static bool AreEqual(double first, double second)
        {
            return Math.Abs(first - second) < floatingComparisonThreshold;
        }

        private static bool AreEqualToZero(double value)
        {
            return Math.Abs(value) < floatingComparisonThreshold;
        }

        private Value TryConvertTypeToDouble(Value value)
        {
            if (value is Int)
            {
                return (value as Int).ToDoubleValue();
            }
            return value;
        }

        public override bool Equals(Value value)
        {
            Double rightValue = value as Double;
            return rightValue != null && AreEqual(DoubleValue, rightValue.DoubleValue);
        }

        public override string ToString()
        {
            return DoubleValue.ToString();
        }

        public static Double Sin(Double value)
        {
            return new Double(Math.Sin(value.DoubleValue));
        }

        public static Double Cos(Double value)
        {
            return new Double(Math.Cos(value.DoubleValue));
        }
    }
}
