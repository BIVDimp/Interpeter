using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public enum BreakpointType
    {
        Stop,
        Hit,
        Condition
    }

    public interface IBreakpoint
    {
        BreakpointType GetBreakpointType();
        int GetStart();
        int GetLength();
    }
}
