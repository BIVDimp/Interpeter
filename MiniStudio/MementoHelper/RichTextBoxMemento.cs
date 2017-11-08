using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using Interfaces;

namespace MiniStudio
{
    [DataContract]
    public sealed class RichTextBoxMemento : IMemento
    {
        [DataMember]
        public readonly string FilePath;
        [DataMember]
        public readonly int SelectionStart;
        [DataMember]
        public readonly int SelectionLength;
        [DataMember]
        public readonly int VertScrollPos;

        public RichTextBoxMemento(string filePath, int selectionStart, int selectionLength, int vertScrollPos)
        {
            FilePath = filePath;
            SelectionStart = selectionStart;
            SelectionLength = selectionLength;
            VertScrollPos = vertScrollPos;
        }
    }
}
