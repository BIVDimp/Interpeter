using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    public class Program : Node
    {
        public List<Statement> StatementList { get; internal set; }

        internal Function MainFunction { get; set; }

        public readonly List<Node> NodeList = new List<Node>();

        internal Program(List<Node> nodeList, Function main, Position position)
            : base(position)
        {
            if (nodeList == null)
            {
                throw new ArgumentNullException();
            }
            this.NodeList = nodeList;
            this.MainFunction = main;
        }
    }
}
