using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    public abstract class Node
    {
        public Position Position { get; set; }

        protected Node(Position position)
        {
            Position = position;
        }

        protected void CheckListForNull<T>(List<T> list)
        {
            foreach (T temp in list)
            {
                if (temp == null)
                {
                    throw new ArgumentNullException();
                }
            }
        }
    }
}
