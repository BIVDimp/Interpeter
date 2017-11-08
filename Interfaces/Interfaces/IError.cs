using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IError
    {
        int GetStart();
        int GetLength();
        int GetLine();
        int GetColumn();
        string GetMessage();
    }
}
