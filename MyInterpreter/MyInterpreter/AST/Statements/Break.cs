using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal class Break : JumpStatement
    {
        internal override Statement NextStatement
        {
            get
            {
                return IterationStatement.End;
            }
            set
            {
            }
        }

        public IterationStatement IterationStatement { get; set; }

        public Break(Position position)
            : base(position)
        {
        }
    }
}
