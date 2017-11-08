using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal class Function : Node
    {
        public readonly FunctionSignature Signature;
        public readonly Block Block;
        public readonly FunctionEnd FunctionEnd;

        public Function(FunctionSignature signature, Block block, Position position)
            : base(position)
        {
            if (signature == null || block == null)
            {
                throw new ArgumentNullException();
            }

            this.Signature = signature;
            this.Block = block;
            this.FunctionEnd = new FunctionEnd(this, block.BlockEnd.Position);

            this.Block.NextStatement = this.FunctionEnd;
        }
    }
}
