using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IStorableSettings
    {
        IMemento GetMemento();
        bool SetMemento(IMemento memento);
        void Clear();
        Type GetMementoType();
    }
}
