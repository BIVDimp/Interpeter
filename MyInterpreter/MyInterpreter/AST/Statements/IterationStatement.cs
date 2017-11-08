using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal abstract class IterationStatement : Invisible
    {
        public readonly Statement Body;
        public readonly EndIterationStatement End;

        protected IterationStatement(Statement body, Position position)
            : base(position)
        {
            if (body == null)
            {
                throw new ArgumentNullException();
            }
            this.Body = body;
            this.End = new EndIterationStatement(this, position.Resize());
        }
    }
}
