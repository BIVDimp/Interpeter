using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal class For : IterationStatement
    {
        internal override Statement NextStatement
        {
            get
            {
                return Initializer;
            }
            set
            {
                End.NextStatement = value;
            }
        }


        public readonly Statement Initializer;
        public readonly Fork LoopCondition;
        public readonly Statement Iterator;

        public For(Statement initializer, Fork loopCondition, Statement iterator, Statement body, Position position)
            : base(body, position)
        {
            if (initializer == null || loopCondition == null || iterator == null)
            {
                throw new ArgumentNullException();
            }
            Initializer = initializer;
            LoopCondition = loopCondition;
            Iterator = iterator;
            PutLinks();
        }

        public For(Statement initializer, Condition condition, Statement iterator, Statement body, Position position)
            : base(body, position)
        {
            if (initializer == null || condition == null || iterator == null)
            {
                throw new ArgumentNullException();
            }
            Initializer = initializer;
            LoopCondition = new Fork(condition);
            Iterator = iterator;
            PutLinks();
        }

        private void PutLinks()
        {
            Initializer.NextStatement = LoopCondition;
            LoopCondition.NextStatement = Body;
            Body.NextStatement = Iterator;
            Iterator.NextStatement = LoopCondition;
            LoopCondition.FalseStatement = End;
        }
    }
}
