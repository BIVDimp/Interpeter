using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal class Label : Node
    {
        public readonly string Name;

        public Label(string name, Position position)
            : base(position)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Empty label.");
            }
            this.Name = name;
        }
    }
}
