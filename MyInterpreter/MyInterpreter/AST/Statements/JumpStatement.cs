using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal abstract class JumpStatement : Statement
    {
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                level = value;
            }
        }

        private int level = 0;

        protected JumpStatement(Position position)
            : base(position)
        {
        }
    }
}
