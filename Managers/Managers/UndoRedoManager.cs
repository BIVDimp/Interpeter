using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace Managers
{
    public sealed class UndoRedoManager : IStorableSettings
    {
        private sealed class ChangeKeeper
        {
            public int CurrentIndex { get; private set; }
            public TextChangedEventArgs[] Changes
            {
                get
                {
                    return changes.ToArray();
                }
            }

            private const int minIndex = -1;

            private List<TextChangedEventArgs> changes = new List<TextChangedEventArgs>();

            public ChangeKeeper()
            {
                CurrentIndex = minIndex;
            }

            public ChangeKeeper(int currentIndex, TextChangedEventArgs[] changes)
            {
                if (currentIndex < minIndex || currentIndex >= changes.Length)
                {
                    throw new ArgumentException();
                }

                this.CurrentIndex = currentIndex;
                this.changes = changes.ToList();
            }

            public void Add(TextChangedEventArgs currentEvent)
            {
                CurrentIndex++;

                if (CurrentIndex >= 0 && CurrentIndex < changes.Count)
                {
                    changes[CurrentIndex] = currentEvent;
                    changes.RemoveRange(CurrentIndex + 1, changes.Count - CurrentIndex - 1);
                    return;
                }

                changes.Add(currentEvent);
            }

            public TextChangedEventArgs Peek()
            {
                if (CurrentIndex >= 0 && CurrentIndex < changes.Count)
                {
                    return changes[CurrentIndex];
                }

                return TextChangedEventArgs.NoneEventArgs;
            }

            public TextChangedEventArgs Pop()
            {
                if (CurrentIndex >= 0 && CurrentIndex < changes.Count)
                {
                    CurrentIndex--;
                    return changes[CurrentIndex + 1];
                }

                return TextChangedEventArgs.NoneEventArgs;
            }

            public TextChangedEventArgs Next()
            {
                if (CurrentIndex + 1 >= 0 && CurrentIndex + 1 < changes.Count)
                {
                    CurrentIndex++;
                    return changes[CurrentIndex];
                }

                return TextChangedEventArgs.NoneEventArgs;
            }
        }

        private ChangeKeeper changeContainer = new ChangeKeeper();

        public UndoRedoManager()
        {
        }

        public void OnTextChanged(TextChangedEventArgs currentEvent)
        {
            switch (currentEvent.Type)
            {
                case TextChangedEventArgs.EventType.None:
                    return;
                case TextChangedEventArgs.EventType.TextWritten:
                    TextWrittenHandler(currentEvent);
                    return;
                case TextChangedEventArgs.EventType.TextReplaced:
                    TextReplacedHandler(currentEvent);
                    return;
                default:
                    throw new NotSupportedException();
            }
        }

        private void TextWrittenHandler(TextChangedEventArgs currentEvent)
        {
            TextChangedEventArgs peekEventArgs = changeContainer.Peek();

            if (!string.Empty.Equals(currentEvent.OldText) ||
                peekEventArgs.Type != TextChangedEventArgs.EventType.TextWritten ||
                peekEventArgs.OldStartPosition + peekEventArgs.NewText.Length != currentEvent.OldStartPosition)
            {
                changeContainer.Add(currentEvent);
                return;
            }

            string temp = peekEventArgs.NewText + currentEvent.NewText;
            if (IsIdentifierOrNumber(temp) || IsWhiteSpase(temp))
            {
                changeContainer.Pop();
                changeContainer.Add(
                    TextChangedEventArgs.CreateTextWrittenEventArgs(peekEventArgs.OldText, temp, peekEventArgs.OldStartPosition));
                return;
            }

            changeContainer.Add(currentEvent);
        }

        private void TextReplacedHandler(TextChangedEventArgs currentEvent)
        {
            changeContainer.Add(currentEvent);
        }

        public TextChangedEventArgs Undo()
        {
            TextChangedEventArgs textEventArgs = changeContainer.Pop();
            switch (textEventArgs.Type)
            {
                case TextChangedEventArgs.EventType.None:
                    return textEventArgs;
                case TextChangedEventArgs.EventType.TextWritten:
                    return TextChangedEventArgs.CreateTextReplacedEventArgs(
                        textEventArgs.NewText, textEventArgs.OldText, textEventArgs.OldStartPosition);
                case TextChangedEventArgs.EventType.TextReplaced:
                    return TextChangedEventArgs.CreateTextReplacedEventArgs(
                        textEventArgs.NewText, textEventArgs.OldText, textEventArgs.OldStartPosition);
                default:
                    throw new NotSupportedException();
            }
        }

        public TextChangedEventArgs Redo()
        {
            return changeContainer.Next();
        }

        public void Clear()
        {
            changeContainer = new ChangeKeeper();
        }

        private bool IsIdentifierOrNumber(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            for (int i = 0; i < line.Length; i++)
            {
                if (!char.IsLetterOrDigit(line[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsWhiteSpase(string line)
        {
            return line != null && string.IsNullOrWhiteSpace(line);
        }

        public IMemento GetMemento()
        {
            return new UndoRedoMemento(changeContainer.CurrentIndex, changeContainer.Changes);
        }

        public bool SetMemento(IMemento memento)
        {
            if (memento == null || !(memento is UndoRedoMemento))
            {
                return false;
            }

            UndoRedoMemento undoRedoMemento = memento as UndoRedoMemento;

            changeContainer = new ChangeKeeper(undoRedoMemento.CurrentIndex, undoRedoMemento.ArgsArray);

            return true;
        }

        public Type GetMementoType()
        {
            return typeof(UndoRedoMemento);
        }
    }
}
