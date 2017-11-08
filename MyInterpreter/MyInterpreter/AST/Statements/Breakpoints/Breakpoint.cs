using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace MyInterpreter.AST
{
    public abstract class Breakpoint : Statement, IBreakpoint
    {
        protected Breakpoint(Position position)
            : base(position)
        {
        }

        public void ChangePosition(Position newPosition)
        {
            Position = newPosition;
        }

        public abstract BreakpointType GetBreakpointType();

        public int GetStart()
        {
            return Position.BeginIndex;
        }

        public int GetLength()
        {
            return Position.Length;
        }
    }
}
