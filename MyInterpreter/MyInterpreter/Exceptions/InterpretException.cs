using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.IO;

namespace MyInterpreter
{
    [Serializable]
    public sealed class InterpretException : BaseException
    {
        public enum ErrorType
        {
            NotDeclaredVariable,
            VariableAlreadyExists,
            VariableIsNotSet,
            WrongArraySize,
            FormatException,

            ImplicitConversion,
            InvalidUnaryOperator,
            InvalidBinaryOperator,
            DivisionByZero,
            Overflow,
            InvalidIndexer,
            IndexOutOfRange,
            ReadOnlyIndexer,
            FunctionDoesNotReturnValue
        }

        private static readonly Dictionary<ErrorType, string> errorToMessage =
            new Dictionary<ErrorType, string>
            {
                { ErrorType.NotDeclaredVariable, "\"{0}\" is not declared" },
                { ErrorType.VariableAlreadyExists, "A variable with name \"{0}\" already exists" },
                { ErrorType.ImplicitConversion, "Can not implicitly convert type \"{0}\" to \"{1}\"" },
                { ErrorType.VariableIsNotSet, "Variable \"{0}\" is not set" },
                { ErrorType.WrongArraySize, "The size of the array is equal to or less than zero" },
                { ErrorType.FormatException, "The index of a format item is less than zero, or greater than or equal to the number of arguments in the arguments list" },
                
                { ErrorType.InvalidUnaryOperator, "Operator \"{0}\" cannot be applied to operand of type \"{1}\"" },
                { ErrorType.InvalidBinaryOperator, "Operator \"{0}\" cannot be applied to operands of type \"{1}\" and \"{2}\"" },
                { ErrorType.DivisionByZero, "Division by zero" },
                { ErrorType.Overflow, "Double overflow" },
                { ErrorType.InvalidIndexer, "Cannot apply indexing with [] to an expression of type \"{0}\"" },
                { ErrorType.IndexOutOfRange, "Index out of range" },
                { ErrorType.ReadOnlyIndexer, "Property or indexer '{0}' cannot be assigned to -- it is read only" },
                { ErrorType.FunctionDoesNotReturnValue, "Function '{0}' must return type '{1}' but return '{2}'" }
            };

        private static readonly Dictionary<ValueException.ErrorType, ErrorType> valueErrorToInterpretError =
            new Dictionary<ValueException.ErrorType, ErrorType>
            {
                { ValueException.ErrorType.InvalidUnaryOperator, ErrorType.InvalidUnaryOperator },
                { ValueException.ErrorType.InvalidBinaryOperator, ErrorType.InvalidBinaryOperator },
                { ValueException.ErrorType.DivisionByZero, ErrorType.DivisionByZero },
                { ValueException.ErrorType.Overflow, ErrorType.Overflow },
                { ValueException.ErrorType.ImplicitConversion, ErrorType.ImplicitConversion },
                { ValueException.ErrorType.InvalidIndexer, ErrorType.InvalidIndexer },
                { ValueException.ErrorType.IndexOutOfRange, ErrorType.IndexOutOfRange },
                { ValueException.ErrorType.ReadOnlyIndexer, ErrorType.ReadOnlyIndexer }
            };

        public readonly ErrorType Error;
        public readonly string[] ErrorParameters;

        public InterpretException(ErrorType error, Position position)
            : this(error, errorParameters: new string[0], position: position)
        {
        }

        public InterpretException(ErrorType error, string errorParameter, Position position)
            : this(error, new string[] { errorParameter }, position)
        {
        }

        public InterpretException(ErrorType error, string[] errorParameters, Position position)
            : base(FormMessage(error, errorParameters, position), position)
        {
            Error = error;
            ErrorParameters = errorParameters;
        }

        public override string GetInfo()
        {
            return string.Format(errorToMessage[Error], ErrorParameters);
        }

        public override string ToString()
        {
            return FormMessage(Error, ErrorParameters, Position);
        }

        private static string FormMessage(ErrorType error, string[] errorParameters, Position position)
        {
            return string.Format(outputMessage, position, string.Format(errorToMessage[error], errorParameters));
        }

        public static InterpretException BuildException(ValueException exception, Position position)
        {
            return new InterpretException(valueErrorToInterpretError[exception.Error], exception.ErrorParameters, position);
        }

        [SecurityPermission(SecurityAction.LinkDemand,
            Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            if (info == null)
            {
                throw new System.ArgumentNullException("info");
            }
            info.AddValue("Position", Position);
            info.AddValue("Type", Error);
            info.AddValue("Parameters", ErrorParameters);
        }
    }
}
