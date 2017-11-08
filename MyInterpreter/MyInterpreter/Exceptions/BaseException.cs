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
    public abstract class BaseException : Exception, ISerializable
    {
        protected const string outputMessage = "{0}: {1}.";

        public readonly Position Position;

        protected BaseException(string message, Position position)
            : base(message)
        {
            Position = position;
        }

        public abstract string GetInfo();

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
        }
    }
}
