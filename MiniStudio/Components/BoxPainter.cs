using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using System.Windows.Forms;
using System.Drawing;
using Managers;

namespace MiniStudio
{
    public sealed class BoxPainter : ITokenPainter, IDisposable
    {
        private RichTextBox mainBox;
        private RichTextBoxCaretaker boxCaretaker;
        private RichTextBox tempBox = null;
        bool paitingDisplay = true;

        public BoxPainter(RichTextBox box)
        {
            if (box == null)
            {
                throw new ArgumentNullException();
            }

            this.mainBox = box;
            boxCaretaker = new RichTextBoxCaretaker(box);
        }

        public void PaintText(ColoredTokenType type, int startPosition, int length)
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
                case ColoredTokenType.KeyWords:
                    currentBox.SelectionColor = Color.Blue;
                    break;
                case ColoredTokenType.StringConstant:
                    currentBox.SelectionColor = Color.Brown;
                    break;
                case ColoredTokenType.Comment:
                    currentBox.SelectionColor = Color.Green;
                    break;
                case ColoredTokenType.Function:
                    currentBox.SelectionColor = Color.DeepSkyBlue;
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

        public void SuspendPaitingDisplay()
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

        public void ResumePaintingDisplay()
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
