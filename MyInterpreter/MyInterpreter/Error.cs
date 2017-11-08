using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace MyInterpreter
{
    internal sealed class Error : IError
    {

        public readonly Position Position;
        public readonly string Message;

        public Error(Position position, string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException();
            }
            Position = position;
            Message = message;
        }

        public int GetStart()
        {
            return Position.BeginIndex;
        }

        public int GetLength()
        {
            return Position.Length;
        }

        public int GetLine()
        {
            return Position.LineNumber;
        }

        public int GetColumn()
        {
            return Position.ColumnNumber;
        }

        public string GetMessage()
        {
            return Message;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Position.ToString(), Message);
        }

        public static Error CreateError(BaseException exception)
        {
            return new Error(exception.Position, exception.GetInfo());
        }
    }
}
