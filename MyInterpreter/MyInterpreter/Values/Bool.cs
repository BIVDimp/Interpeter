using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueType = MyInterpreter.ValueType;

namespace MyInterpreter
{
    public class Bool : Value
    {
        public bool BoolValue { get; private set; }

        public Bool()
            : base(ValueType.Bool)
        {
            BoolValue = false;
        }

        public Bool(bool value)
            : base(ValueType.Bool)
        {
            BoolValue = value;
        }

        public override Value Set(Value value)
        {
            Bool valueAsBool = value as Bool;

            if (valueAsBool == null)
            {
                throw new ValueException(ValueException.ErrorType.ImplicitConversion,
                    new string[] { value.Type.ToString(), Type.ToString() });
            }

            this.BoolValue = valueAsBool.BoolValue;
            return this;
        }

        public override Value IsEqual(Value value)
        {
            CheckForNull(value);

            Bool rightValue = value as Bool;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.Equal);
            }

            return new Bool(BoolValue == rightValue.BoolValue);
        }

        public override Value IsNotEqual(Value value)
        {
            CheckForNull(value);

            Bool rightValue = value as Bool;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.NotEqual);
            }

            return new Bool(BoolValue != rightValue.BoolValue);
        }

        public override Bool LogicalAnd(Value value)
        {
            CheckForNull(value);

            Bool rightValue = value as Bool;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.LogicalAnd);
            }

            return new Bool(BoolValue && rightValue.BoolValue);
        }

        public override Bool LogicalOr(Value value)
        {
            CheckForNull(value);

            Bool rightValue = value as Bool;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.LogicalOr);
            }

            return new Bool(BoolValue || rightValue.BoolValue);
        }

        public override string ToString()
        {
            return BoolValue.ToString();
        }

        public override bool Equals(Value value)
        {
            Bool rightValue = value as Bool;
            return rightValue != null && BoolValue == rightValue.BoolValue;
        }
    }
}
