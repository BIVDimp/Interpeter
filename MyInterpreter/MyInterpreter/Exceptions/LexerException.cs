using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.IO;

namespace MyInterpreter
{
    [Serializable]
    public sealed class LexerException : BaseException, ISerializable
    {
        public enum ErrorType
        {
            UnknownToken,
            UnfinishedStringConstant
        }

        private static readonly Dictionary<ErrorType, string> errors =
            new Dictionary<ErrorType, string>
            {
                { ErrorType.UnknownToken, "Unknown token" },
                { ErrorType.UnfinishedStringConstant, "Unfinished string constant" }
            };

        public readonly ErrorType Error;

        public LexerException(ErrorType error, Position position)
            : base(FormMessage(error, position), position)
        {
            Error = error;
        }

        public override string GetInfo()
        {
            return errors[Error];
        }

        public override string ToString()
        {
            return FormMessage(Error, Position);
        }

        private static string FormMessage(ErrorType error, Position position)
        {
            return string.Format(outputMessage, position.ToString(), errors[error]);
        }

        [SecurityPermission(SecurityAction.LinkDemand,
            Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            if (info == null)
            {
                throw new System.ArgumentNullException("info");
            }
            info.AddValue("Position", Position);
            info.AddValue("Type", Error);
        }
    }
}
