using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter
{
    internal interface IVariableToValue<TKey, TValue>
    {
        bool TryGetValue(TKey key, out TValue value);
    }
}
