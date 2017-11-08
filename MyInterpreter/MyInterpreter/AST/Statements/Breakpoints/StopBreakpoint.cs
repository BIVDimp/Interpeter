using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace MyInterpreter.AST
{
    public sealed class StopBreakpoint : Breakpoint
    {
        public StopBreakpoint(Position position)
            : base(position)
        {
        }

        public override BreakpointType GetBreakpointType()
        {
            return BreakpointType.Stop;
        }
    }
}
