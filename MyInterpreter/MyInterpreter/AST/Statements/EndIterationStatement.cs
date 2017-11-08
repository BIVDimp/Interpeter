using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal class EndIterationStatement : Invisible
    {
        public readonly IterationStatement IterationStatement;

        public EndIterationStatement(IterationStatement iteration, Position position)
            : base(position)
        {
            if (iteration == null)
            {
                throw new ArgumentNullException();
            }
            this.IterationStatement = iteration;
        }
    }
}
