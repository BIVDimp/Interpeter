using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueType = MyInterpreter.ValueType;

namespace MyInterpreter.AST
{
    internal sealed class FunctionSignature : Node
    {
        public readonly ValueType ReturnType;
        public readonly string Name;
        public readonly List<Declaration> Parameters;

        public FunctionSignature(ValueType returnType, string name, List<Declaration> parameters, Position position)
            : base(position)
        {
            if (returnType == null || name == null || parameters == null)
            {
                throw new ArgumentNullException();
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException();
            }
            CheckListForNull(parameters);

            this.ReturnType = returnType;
            this.Name = name;
            this.Parameters = parameters;
        }

        public bool Equals(FunctionSignature signature)
        {
            bool isEquals = string.Equals(Name, signature.Name);
            isEquals = isEquals && Parameters.Count == signature.Parameters.Count;

            if (isEquals == false)
            {
                return false;
            }

            for (int i = 0; i < Parameters.Count; i++)
            {
                if (!ValueType.Equals(Parameters[i].ValueType, signature.Parameters[i].ValueType))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
