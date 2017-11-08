using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace MyInterpreter.AST
{
    public class Statement : Node
    {
        public Breakpoint Breakpoint { get; set; }

        private List<Label> labels = new List<Label>();

        internal List<Label> Labels
        {
            get
            {
                return labels;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                if (labels.Count > 0)
                {
                    throw new InvalidOperationException("Список лейблов уже существует");
                }
                labels = value;
            }
        }

        internal virtual Statement NextStatement { get; set; }

        internal Statement(Position position)
            : base(position)
        {
        }
    }
}