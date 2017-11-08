using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal class Print : Statement
    {
        public readonly List<Expression> Parameters;

        public Print(List<Expression> parameters, Position position)
            : base(position)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException();
            }

            foreach (Expression parameter in parameters)
            {
                if (parameter == null)
                {
                    throw new ArgumentNullException();
                }
            }

            Parameters = parameters;
        }
    }
}
