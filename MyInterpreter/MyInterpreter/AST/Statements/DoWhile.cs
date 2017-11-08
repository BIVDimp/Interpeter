using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal class DoWhile : IterationStatement
    {
        internal override Statement NextStatement
        {
            get
            {
                return Body;
            }
            set
            {
                End.NextStatement = value;
            }
        }

        public readonly Fork LoopCondition;

        public DoWhile(Statement body, Fork loopCondition, Position position)
            : base(body, position)
        {
            if (loopCondition == null)
            {
                throw new ArgumentNullException();
            }

            LoopCondition = loopCondition;
            PutLinks();
        }

        public DoWhile(Statement body, Condition condition, Position position)
            : base(body, position)
        {
            if (condition == null)
            {
                throw new ArgumentNullException();
            }

            LoopCondition = new Fork(condition);
            PutLinks();
        }

        private void PutLinks()
        {
            Body.NextStatement = LoopCondition;
            LoopCondition.NextStatement = Body;
            LoopCondition.FalseStatement = End;
        }
    }
}