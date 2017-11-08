using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interfaces;

namespace MiniStudio
{
    public sealed class LabelRenderer : ILabelRenderer
    {
        private readonly Label label;

        public LabelRenderer(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException();
            }
            this.label = label;
        }

        public void ChangeText(string newText)
        {
            if (newText == null)
            {
                throw new ArgumentNullException();
            }
            label.Text = newText;
        }
    }
}
