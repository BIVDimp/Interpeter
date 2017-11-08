using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueType = MyInterpreter.ValueType;

namespace MyInterpreter
{
    public struct OperationProperties
    {
        public readonly ValueType ResultType;
        public readonly Value.Operator Operator;
        public readonly ValueType[] Parameters;

        public OperationProperties(ValueType resultType, Value.Operator valueOperator, params ValueType[] parameters)
        {
            if (resultType == null || parameters == null)
            {
                throw new ArgumentNullException();
            }
            if (parameters.Length <= 0)
            {
                throw new ArgumentException();
            }
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == null)
                {
                    throw new ArgumentNullException();
                }
            }

            this.ResultType = resultType;
            this.Operator = valueOperator;
            this.Parameters = parameters;
        }
    }
}
