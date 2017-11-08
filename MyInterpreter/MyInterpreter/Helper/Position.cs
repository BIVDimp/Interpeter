using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter
{
    public struct Position
    {
        private const int minLineNumber = 0;
        private const int minColumnNumber = 0;
        private const int minBeginIndex = 0;
        private const int minLength = 0;
        private const string formatString = "Line {0}, Column {1}";

        public readonly int LineNumber;
        public readonly int ColumnNumber;
        public readonly int BeginIndex;
        public readonly int Length;


        public Position(int lineNumber, int columnNumber, int beginIndex, int length)
        {
            if (lineNumber < minLineNumber || columnNumber < minColumnNumber
                || beginIndex < minBeginIndex || length < minLength)
            {
                throw new ArgumentOutOfRangeException();
            }
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            BeginIndex = beginIndex;
            Length = length;
        }

        public Position Resize(int newLength = 0)
        {
            return new Position(
                LineNumber,
                ColumnNumber,
                BeginIndex,
                newLength);
        }

        public override string ToString()
        {
            return string.Format(formatString, LineNumber + 1, ColumnNumber + 1);
        }
    }
}
