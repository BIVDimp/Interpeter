using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public enum HighlightType
    {
        Breakpoint,
        CurrentStatement,
        CurrentBreakpoint,
        None
    }

    public interface IStatementHighlighter
    {
        void HighlightText(HighlightType type, int startPosition, int length);
        void SuspendHighlightDisplay();
        void ResumeHighlightDisplay();
    }
}
