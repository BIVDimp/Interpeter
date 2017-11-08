using Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniStudio
{
    public sealed class BoxHighlighter : IStatementHighlighter, IDisposable
    {
        private RichTextBox mainBox;
        private RichTextBoxCaretaker boxCaretaker;
        private RichTextBox tempBox = null;
        bool paitingDisplay = true;

        public BoxHighlighter(RichTextBox box)
        {
            if (box == null)
            {
                throw new ArgumentNullException();
            }

            this.mainBox = box;
            boxCaretaker = new RichTextBoxCaretaker(box);
        }

        public void HighlightText(HighlightType type, int startPosition, int length)
        {
            if (length < 0 || mainBox.Text.Length < startPosition + length)
            {
                throw new ArgumentException();
            }

            RichTextBox currentBox;

            if (paitingDisplay)
            {
                currentBox = mainBox;
                boxCaretaker.SaveState();
            }
            else
            {
                currentBox = tempBox;
            }

            currentBox.SelectionStart = startPosition;
            currentBox.SelectionLength = length;

            switch (type)
            {
                case HighlightType.Breakpoint:
                    currentBox.SelectionBackColor = Color.Red;
                    break;
                case HighlightType.CurrentBreakpoint:
                    currentBox.SelectionBackColor = Color.DarkOrange;
                    break;
                case HighlightType.CurrentStatement:
                    currentBox.SelectionBackColor = Color.Yellow;
                    break;
                case HighlightType.None:
                    currentBox.SelectionBackColor = currentBox.BackColor;
                    break;
                default:
                    currentBox.SelectionColor = Color.Black;
                    break;
            }

            if (paitingDisplay)
            {
                boxCaretaker.LoadState();
            }
        }

        public void SuspendHighlightDisplay()
        {
            if (!paitingDisplay)
            {
                return;
            }
            tempBox = new RichTextBox();
            tempBox.Rtf = mainBox.Rtf;
            boxCaretaker.SaveState();
            paitingDisplay = false;
        }

        public void ResumeHighlightDisplay()
        {
            if (paitingDisplay)
            {
                return;
            }
            mainBox.Rtf = tempBox.Rtf;
            boxCaretaker.LoadState();
            tempBox = null;
            paitingDisplay = true;
        }

        public void Dispose()
        {
            if (mainBox != null)
            {
                mainBox.Dispose();
            }
            if (tempBox != null)
            {
                tempBox.Dispose();
            }
        }
    }
}
