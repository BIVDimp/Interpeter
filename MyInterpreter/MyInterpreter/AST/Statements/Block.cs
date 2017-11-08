using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal class Block : Statement
    {
        internal override Statement NextStatement
        {
            get
            {
                return StatementList.Count < 1 ?
                    BlockEnd : StatementList[0];
            }
            set
            {
                BlockEnd.NextStatement = value;
            }
        }

        public readonly List<Statement> StatementList = new List<Statement>();
        public readonly bool IsIndependent;
        private BlockEnd blockEnd = null;

        public BlockEnd BlockEnd
        {
            get
            {
                return blockEnd;
            }
            set
            {
                blockEnd = value;
                if (StatementList.Count > 0)
                {
                    StatementList.Last().NextStatement = blockEnd;
                }
            }
        }

        public Block(List<Statement> statementList, Position position)
            : this(statementList, true, position)
        {
        }

        public Block(List<Statement> statementList, bool isIndependent, Position position)
            : base(position)
        {
            if (statementList == null)
            {
                throw new ArgumentNullException();
            }
            this.StatementList = statementList;
            this.IsIndependent = isIndependent;
            PutLinksInList();
        }

        private void PutLinksInList()
        {
            for (int i = 0; i < StatementList.Count - 1; i++)
            {
                StatementList[i].NextStatement = StatementList[i + 1];
            }
        }
    }
}
