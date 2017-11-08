using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Interfaces;

namespace Managers
{
    [DataContract]
    public sealed class UndoRedoMemento : IMemento
    {
        [DataMember]
        public readonly int CurrentIndex;
        [DataMember]
        public readonly TextChangedEventArgs[] ArgsArray;

        public UndoRedoMemento(int currentIndex, TextChangedEventArgs[] argsArray)
        {
            CurrentIndex = currentIndex;
            ArgsArray = argsArray;
        }
    }
}
