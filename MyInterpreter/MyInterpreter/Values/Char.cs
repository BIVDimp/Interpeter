using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueType = MyInterpreter.ValueType;

namespace MyInterpreter
{
    internal class Char : Value
    {
        public char CharValue { get; private set; }

        public Char()
            : base(ValueType.Char)
        {
            CharValue = default(char);
        }

        public Char(char value)
            : base(ValueType.Char)
        {
            CharValue = value;
        }

        public Char(Char value)
            : base(ValueType.Char)
        {
            CharValue = value.CharValue;
        }

        public override Value Set(Value value)
        {
            Char valueAsChar = value as Char;

            if (valueAsChar == null)
            {
                throw new ValueException(ValueException.ErrorType.ImplicitConversion,
                    new string[] { value.Type.ToString(), Type.ToString() });
            }

            this.CharValue = valueAsChar.CharValue;
            return new Char(this);
        }

        public override Value IsEqual(Value value)
        {
            CheckForNull(value);

            Char rightValue = value as Char;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.Equal);
            }

            return new Bool(CharValue == rightValue.CharValue);
        }

        public override Value IsNotEqual(Value value)
        {
            CheckForNull(value);

            Char rightValue = value as Char;

            if (rightValue == null)
            {
                throw BuildInvalidBinaryException(this, value, Operator.NotEqual);
            }

            return new Bool(CharValue != rightValue.CharValue);
        }

        public override bool Equals(Value value)
        {
            if (value == null)
            {
                return false;
            }

            return value.Type == Type && CharValue == ((Char)value).CharValue;
        }

        public override string ToString()
        {
            return CharValue.ToString();
        }

        public String ToStringValue()
        {
            return new String(CharValue.ToString());
        }
    }
}
