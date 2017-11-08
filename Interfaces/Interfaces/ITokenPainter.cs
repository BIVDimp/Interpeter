using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Interfaces
{
    public enum ColoredTokenType
    {
        NumberConstant,
        Function,
        KeyWords,
        StringConstant,
        Comment,
        WithoutIllumination
    }

    public interface ITokenPainter
    {
        void PaintText(ColoredTokenType type, int startPosition, int length);
        void SuspendPaitingDisplay();
        void ResumePaintingDisplay();
    }
}