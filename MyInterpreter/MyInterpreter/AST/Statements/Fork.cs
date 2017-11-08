using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal sealed class Fork : Statement
    {
        public Statement FalseStatement { get; set; }

        public readonly Condition Condition;

        public Fork(Condition condition)
            : base(condition.Position)
        {
            if (condition == null)
            {
                throw new ArgumentNullException();
            }
            Condition = condition;
        }
    }
}
