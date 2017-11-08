using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace MyInterpreter.AST
{
    public sealed class HitCountBreakpoint : Breakpoint
    {
        public int HitCount { get; private set; }

        public HitCountBreakpoint(int hitCount, Position position)
            : base(position)
        {
            if (hitCount <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.HitCount = hitCount;
        }

        public override BreakpointType GetBreakpointType()
        {
            return BreakpointType.Hit;
        }

        public void ChangeHitCount(int newHits)
        {
            if (newHits <= 0)
            {
                throw new ArgumentException();
            }
            HitCount = newHits;
        }
    }
}
