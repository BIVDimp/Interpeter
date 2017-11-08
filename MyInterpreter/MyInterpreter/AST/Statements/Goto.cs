using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter.AST
{
    internal sealed class Goto : JumpStatement
    {
        internal override Statement NextStatement
        {
            get
            {
                return base.NextStatement;
            }
            set
            {
                if (value.Labels.Exists(label => string.Equals(label.Name, GotoLabel.Name)))
                {
                    base.NextStatement = value;
                }
            }
        }

        public readonly Label GotoLabel;

        public Goto(Label gotoLabel, Position position)
            : base(position)
        {
            if (gotoLabel == null)
            {
                throw new ArgumentNullException();
            }
            this.GotoLabel = gotoLabel;
        }
    }
}
