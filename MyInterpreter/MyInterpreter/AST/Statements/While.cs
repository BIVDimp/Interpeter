using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal class While : IterationStatement
    {
        internal override Statement NextStatement
        {
            get
            {
                return LoopCondition;
            }
            set
            {
                End.NextStatement = value;
            }
        }

        public readonly Fork LoopCondition;

        public While(Fork loopCondition, Statement body, Position position)
            : base(body, position)
        {
            if (loopCondition == null)
            {
                throw new ArgumentNullException();
            }

            LoopCondition = loopCondition;
            PutLinks();
        }

        public While(Condition condition, Statement body, Position position)
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
            LoopCondition.NextStatement = Body;
            Body.NextStatement = LoopCondition;
            LoopCondition.FalseStatement = End;
        }
    }
}