using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace MyInterpreter
{
    public sealed class HaveErrorsStatus : BaseInterpreterStatus
    {
        public readonly List<IError> ErrorList;

        internal HaveErrorsStatus(List<Error> errorList)
        {
            if (errorList == null)
            {
                throw new ArgumentNullException();
            }

            this.ErrorList = errorList.ToList<IError>();
        }

        internal HaveErrorsStatus(Error error)
        {
            if (error == null)
            {
                throw new ArgumentNullException();
            }

            this.ErrorList = new List<IError>();
            this.ErrorList.Add(error);
        }
    }
}
