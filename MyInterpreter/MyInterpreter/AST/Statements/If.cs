using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal class If : Invisible
    {
        internal override Statement NextStatement
        {
            get
            {
                return IfCondition;
            }
            set
            {
                TrueStatement.NextStatement = value;

                if (FalseStatement != null)
                {
                    FalseStatement.NextStatement = value;
                }
                else
                {
                    IfCondition.FalseStatement = value;
                }
            }
        }

        public readonly Fork IfCondition;
        public readonly Statement TrueStatement;
        public readonly Statement FalseStatement;

        public If(Fork ifCondition, Statement trueStatement, Position position)
            : this(ifCondition, trueStatement, null, position)
        {
        }

        public If(Condition condition, Statement trueStatement, Position position)
            : this(condition, trueStatement, null, position)
        {
        }

        public If(Fork ifCondition, Statement trueStatement, Statement falseStatement, Position position)
            : base(position)
        {
            if (ifCondition == null || trueStatement == null)
            {
                throw new ArgumentNullException();
            }
            IfCondition = ifCondition;
            TrueStatement = trueStatement;
            FalseStatement = falseStatement;
            PutLinks();
        }

        public If(Condition condition, Statement trueStatement, Statement falseStatement, Position position)
            : base(position)
        {
            if (condition == null || trueStatement == null)
            {
                throw new ArgumentNullException();
            }
            IfCondition = new Fork(condition);
            TrueStatement = trueStatement;
            FalseStatement = falseStatement;
            PutLinks();
        }

        private void PutLinks()
        {
            IfCondition.NextStatement = TrueStatement;
            IfCondition.FalseStatement = FalseStatement;
        }
    }
}