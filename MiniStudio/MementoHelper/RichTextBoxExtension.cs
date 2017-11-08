using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using Interfaces;

namespace MiniStudio
{
    public sealed class RichTextBoxExtension : IStorableSettings
    {
        private RichTextBox box;

        public string FilePath { get; set; }

        public RichTextBoxExtension(RichTextBox box)
        {
            if (box == null)
            {
                throw new ArgumentNullException();
            }
            this.box = box;
            FilePath = null;
        }

        #region API Stuff
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll")]
        public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll", EntryPoint = "PostMessageA")]
        private static extern int PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        private const int SB_HORZ = 0x0;
        private const int SB_VERT = 0x1;
        #endregion

        public IMemento GetMemento()
        {
            return new RichTextBoxMemento(FilePath, box.SelectionStart, box.SelectionLength, GetScrollPos(box.Handle, SB_VERT));
        }

        public bool SetMemento(IMemento memento)
        {
            if (memento == null || !(memento is RichTextBoxMemento))
            {
                return false;
            }

            RichTextBoxMemento rtbMemento = memento as RichTextBoxMemento;

            if (!string.Equals(FilePath, rtbMemento.FilePath))
            {
                LoadFile(rtbMemento.FilePath);
                FilePath = rtbMemento.FilePath;
            }
            box.SelectionStart = rtbMemento.SelectionStart;
            box.SelectionLength = rtbMemento.SelectionLength;
            PostMessage(box.Handle, 0x115, 4 + 0x10000 * rtbMemento.VertScrollPos, 0);

            return true;
        }

        public void Clear()
        {
            box.Clear();
            FilePath = null;
        }

        public Type GetMementoType()
        {
            return typeof(RichTextBoxMemento);
        }

        private void LoadFile(string filePath)
        {
            if (filePath == null)
            {
                return;
            }
            using (StreamReader codeReader = new StreamReader(filePath))
            {
                box.LoadFile(codeReader.BaseStream, RichTextBoxStreamType.PlainText);
            }
        }
    }
}
