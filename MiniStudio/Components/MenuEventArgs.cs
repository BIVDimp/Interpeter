using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniStudio
{
    public class MenuEventArgs : EventArgs
    {
        public bool Cancel { get; set; }

        public MenuEventArgs()
        {
            Cancel = false;
        }
    }
}
