using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using System.Windows.Forms;

namespace MiniStudio
{
    public class OutputWriter : IWriter
    {
        private readonly RichTextBox outputTextBox;

        public OutputWriter(RichTextBox box)
        {
            if (box == null)
            {
                throw new ArgumentNullException();
            }
            outputTextBox = box;
        }

        public void Write(string output)
        {
            outputTextBox.Text += output;
        }

        public void ClearText()
        {
            outputTextBox.Text = string.Empty;
        }
    }
}
