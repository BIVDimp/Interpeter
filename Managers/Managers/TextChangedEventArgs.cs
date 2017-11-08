using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Managers
{
    [DataContract]
    public struct TextChangedEventArgs
    {
        public enum EventType
        {
            None,
            TextWritten,
            TextReplaced
        }

        [DataMember]
        public readonly EventType Type;
        [DataMember]
        public readonly string OldText;
        [DataMember]
        public readonly int OldStartPosition;
        [DataMember]
        public readonly string NewText;
        [DataMember]
        public readonly int NewStartPosition;

        public static readonly TextChangedEventArgs NoneEventArgs = new TextChangedEventArgs(EventType.None);

        public static TextChangedEventArgs CreateTextWrittenEventArgs(char symbol, int startPosition)
        {
            return CreateTextWrittenEventArgs(string.Empty, symbol, startPosition);
        }

        public static TextChangedEventArgs CreateTextWrittenEventArgs(string deletedText, char symbol, int startPosition)
        {
            return new TextChangedEventArgs(EventType.TextWritten, deletedText, symbol.ToString(), startPosition);
        }

        public static TextChangedEventArgs CreateTextWrittenEventArgs(string deletedText, string text, int startPosition)
        {
            return new TextChangedEventArgs(EventType.TextWritten, deletedText, text, startPosition);
        }

        public static TextChangedEventArgs CreateTextReplacedEventArgs(string oldText, string newText, int startPosition)
        {
            return new TextChangedEventArgs(EventType.TextReplaced, oldText, newText, startPosition);
        }

        //For EventType (.None) only
        private TextChangedEventArgs(EventType eventType)
            : this(eventType, default(string), default(string), default(int), default(int))
        {
        }

        //For EventType (.TextWritten, .TextReplaced)
        private TextChangedEventArgs(EventType eventType, string oldText, string newText, int startPosition)
            : this(eventType, oldText, newText, startPosition, startPosition)
        {
        }

        private TextChangedEventArgs(EventType eventType, string oldText, string newText,
            int oldTextStartPosition, int newTextStartPosition)
        {
            this.Type = eventType;
            this.OldStartPosition = oldTextStartPosition;
            this.OldText = oldText;
            this.NewStartPosition = newTextStartPosition;
            this.NewText = newText;
        }
    }
}
