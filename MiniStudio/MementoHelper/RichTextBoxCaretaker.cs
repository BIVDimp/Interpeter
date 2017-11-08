using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interfaces;

namespace MiniStudio
{
    public sealed class RichTextBoxCaretaker
    {
        private RichTextBoxExtension boxOriginator;
        private IMemento memento;

        public RichTextBoxCaretaker(RichTextBox box)
        {
            if (box == null)
            {
                throw new ArgumentNullException();
            }

            boxOriginator = new RichTextBoxExtension(box);
        }

        public void SaveState()
        {
            memento = boxOriginator.GetMemento();
        }

        public void LoadState()
        {
            if (memento == null)
            {
                throw new InvalidCastException();
            }

            boxOriginator.SetMemento(memento);
        }
    }
}
