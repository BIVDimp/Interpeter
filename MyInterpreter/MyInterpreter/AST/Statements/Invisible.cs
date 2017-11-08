using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal abstract class Invisible : Statement
    {
        protected Invisible(Position position)
            : base(position)
        {
        }
    }
}
