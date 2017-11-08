using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal class FunctionCall : Statement
    {
        public Function Function
        {
            get
            {
                return function;
            }
            set
            {
                if (isFunctionSet)
                {
                    throw new InvalidOperationException();
                }
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                function = value;
                isFunctionSet = true;
            }
        }
        public readonly string Name;
        public readonly List<Expression> Parameters;

        private Function function = null;
        private bool isFunctionSet = false;

        public FunctionCall(string name, List<Expression> parameters, Position position)
            : base(position)
        {
            if (name == null || parameters == null)
            {
                throw new ArgumentNullException();
            }
            CheckListForNull(parameters);

            this.Name = name;
            this.Parameters = parameters;
        }

        public FunctionCall(Function function, List<Expression> parameters, Position position)
            : base(position)
        {
            if (function == null || parameters == null)
            {
                throw new ArgumentNullException();
            }
            CheckListForNull(parameters);

            this.Function = function;
            this.Parameters = parameters;
        }
    }
}
