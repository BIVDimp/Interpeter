using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyInterpreter;

namespace Managers
{
    public static class HighlighterHelper
    {
        public const int WithoutCorrection = 0;

        public static Token CreateTokenWithShiftPosition(Token token, int shift)
        {
            return new Token(token.Type, token.Name, new Position(0, 0, token.Position.BeginIndex + shift, token.Position.Length));
        }
    }
}
