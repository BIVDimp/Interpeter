using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal class BlockEnd : Statement
    {
        public readonly Block Block;

        public BlockEnd(Block block, Position position)
            : base(position)
        {
            if (block == null)
            {
                throw new ArgumentNullException();
            }

            this.Block = block;
        }
    }
}
