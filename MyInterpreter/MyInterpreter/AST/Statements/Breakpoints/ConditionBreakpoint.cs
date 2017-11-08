using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace MyInterpreter.AST
{
    public sealed class ConditionBreakpoint : Breakpoint
    {
        internal readonly Condition Condition;

        public ConditionBreakpoint(Condition condition, Position position)
            : base(position)
        {
            if (condition == null)
            {
                throw new ArgumentNullException();
            }
            this.Condition = condition;
        }

        public override BreakpointType GetBreakpointType()
        {
            return BreakpointType.Condition;
        }
    }
}
