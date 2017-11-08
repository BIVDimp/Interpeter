using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal sealed class Return : JumpStatement
    {
        internal override Statement NextStatement
        {
            get
            {
                return Function.Block.BlockEnd;
            }
            set
            {
            }
        }

        public Function Function { get; set; }
        public readonly Expression Expression;

        public Return(Expression expression, Position position)
            : base(position)
        {
            if (expression == null)
            {
                throw new ArgumentNullException();
            }
            this.Expression = expression;
        }
    }
}
