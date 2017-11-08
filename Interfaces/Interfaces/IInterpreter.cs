using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IInterpreter
    {
        bool IsCorrect { get; }
        bool BuildSolution();
        void RunSolution();
        List<IError> GetErrors();
        IEnumerable<KeyValuePair<string, string>> GetVariables();
    }
}
