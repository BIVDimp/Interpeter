using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueType = MyInterpreter.ValueType;

namespace MyInterpreter
{
    internal class String : Value
    {
        public string StringValue { get; private set; }

        public String()
            : base(ValueType.String)
        {
            StringValue = "";
        }

        public String(string value)
            : base(ValueType.String)
        {
            StringValue = value;
        }

        public String(String value)
            : base(ValueType.String)
        {
            StringValue = value.StringValue;
        }

        public override Value Set(Value value)
        {
            String valueAsString = value as String;

            if (valueAsString == null)
            {
                throw new ValueException(ValueException.ErrorType.ImplicitConversion,
                    new string[] { value.Type.ToString(), Type.ToString() });
            }

            this.StringValue = valueAsString.StringValue;
            return new String(this);
        }

        public override Value this[Value indexValue]
        {
            get
            {
                Int number = indexValue as Int;
                if (number == null)
                {
                    throw new ValueException(ValueException.ErrorType.ImplicitConversion,
                        new string[] { indexValue.Type.ToString(), ValueType.Int.ToString() });
                }
                int index = number.IntValue;
                if (index < 0 || index >= StringValue.Length)
                {
                    throw new ValueException(ValueException.ErrorType.IndexOutOfRange);
                }
                return new Char(StringValue[index]);
            }
            set
            {
                throw new ValueException(ValueException.ErrorType.ReadOnlyIndexer, "String.this[int]");
            }
        }

        public override Value Add(Value value)
        {
            CheckForNull(value);

            if (value is Char)
            {
                value = (value as Char).ToStringValue();
            }

            String rightValue = value as String;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.BinaryPlus);
            }

            return new String(StringValue + rightValue.StringValue);
        }

        public override Value IsEqual(Value value)
        {
            CheckForNull(value);

            String rightValue = value as String;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.Equal);
            }

            return new Bool(StringValue == rightValue.StringValue);
        }

        public override Value IsNotEqual(Value value)
        {
            CheckForNull(value);

            String rightValue = value as String;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.NotEqual);
            }

            return new Bool(StringValue != rightValue.StringValue);
        }

        public override bool Equals(Value value)
        {
            if (value == null)
            {
                return false;
            }

            return value.Type == Type && string.Equals(StringValue, ((String)value).StringValue, StringComparison.InvariantCulture);
        }

        public override string ToString()
        {
            if (StringValue == null)
            {
                return "null";
            }
            return StringValue;
        }
    }
}
