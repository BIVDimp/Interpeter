using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Managers;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.Serialization;
using Interfaces;

namespace MiniStudio
{
    [DataContract]
    public sealed class MainFormMemento : IMemento
    {
        [DataMember]
        public readonly Point Location;
        [DataMember]
        public readonly Size Size;
        [DataMember]
        public readonly FormWindowState FormWindowState;
        [DataMember]
        public readonly FormBorderStyle FormBoarderStyle;

        public MainFormMemento(Point location, Size size,
            FormWindowState formWindowState, FormBorderStyle formBoarderStyle)
        {
            Location = location;
            Size = size;
            FormWindowState = formWindowState;
            FormBoarderStyle = formBoarderStyle;
        }
    }
}
