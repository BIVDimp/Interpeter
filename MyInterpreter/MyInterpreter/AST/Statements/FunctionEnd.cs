using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal class FunctionEnd : Invisible
    {
        internal override Statement NextStatement
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public readonly Function Function;

        public FunctionEnd(Function function, Position position)
            : base(position)
        {
            if (function == null)
            {
                throw new ArgumentNullException();
            }
            this.Function = function;
        }
    }
}
