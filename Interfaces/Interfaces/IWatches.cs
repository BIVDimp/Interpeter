using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IWatches
    {
        void AddResult(string name, string value);
        void ClearResults();
    }
}
