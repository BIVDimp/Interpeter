using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IBreakpointDisplay
    {
        void Display(IBreakpoint breakpoint, string information);
        void Delete(IBreakpoint breakpoint);
        void ClearBreakpoints();
    }
}
