using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueType = MyInterpreter.ValueType;

namespace MyInterpreter
{
    public class Void : Value
    {
        public Void()
            : base(ValueType.Void)
        {
        }

        public override Value Set(Value value)
        {
            throw (BuildInvalidUnaryException(this, Operator.Set));
        }
    }
}
