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
    public sealed class ValueException : Exception, ISerializable
    {
        public enum ErrorType
        {
            ImplicitConversion,
            InvalidUnaryOperator,
            InvalidBinaryOperator,
            DivisionByZero,
            Overflow,
            InvalidIndexer,
            IndexOutOfRange,
            ReadOnlyIndexer
        }

        private static readonly Dictionary<ErrorType, string> errorMessage =
            new Dictionary<ErrorType, string>
            {
                { ErrorType.IndexOutOfRange, "Index out of range" },
                { ErrorType.ImplicitConversion, "Can not implicitly convert type \"{0}\" to \"{1}\"" },
                { ErrorType.InvalidUnaryOperator, "Operator \"{0}\" cannot be applied to operand of type \"{1}\"" },
                { ErrorType.InvalidBinaryOperator, "Operator \"{0}\" cannot be applied to operands of type \"{1}\" and \"{2}\"" },
                { ErrorType.DivisionByZero, "Division by zero" },
                { ErrorType.Overflow, "{0} overflow" },
                { ErrorType.InvalidIndexer, "Cannot apply indexing with [] to an expression of type \"{0}\"" },
                { ErrorType.ReadOnlyIndexer, "Property or indexer '{0}' cannot be assigned to -- it is read only" }
            };

        public static readonly Dictionary<Value.Operator, string> OperatorToString =
            new Dictionary<Value.Operator, string>
            {
                { Value.Operator.UnaryPlus, "+" },
                { Value.Operator.UnaryMinus, "-" },
                { Value.Operator.BinaryPlus, "+" },
                { Value.Operator.BinaryMinus, "-" },
                { Value.Operator.Multiply, "*" },
                { Value.Operator.Divide, "/" },
                { Value.Operator.Degree, "**" },
                { Value.Operator.Equal, "==" },
                { Value.Operator.NotEqual, "!=" },
                { Value.Operator.Less, "<" },
                { Value.Operator.LessOrEqual, "<=" },
                { Value.Operator.Greater, ">" },
                { Value.Operator.GreaterOrEqual, ">=" },
                { Value.Operator.LogicalAnd, "&&" },
                { Value.Operator.LogicalOr, "||" },
                { Value.Operator.Set, "=" }
            };

        public readonly ErrorType Error;
        public readonly string[] ErrorParameters;

        public ValueException(ErrorType error, string errorParameter = null)
            : this(error, new string[] { errorParameter })
        {
        }

        public ValueException(ErrorType error, Value.Operator valueOperator, string firstType)
            : this(error, new string[] { OperatorToString[valueOperator], firstType })
        {
        }

        public ValueException(ErrorType error, Value.Operator valueOperator, string firstType, string secondType)
            : this(error, new string[] { OperatorToString[valueOperator], firstType, secondType })
        {
        }

        public ValueException(ErrorType error, string[] errorParameters)
            : base(FormMessage(error, errorParameters))
        {
            Error = error;
            ErrorParameters = errorParameters;
        }

        public override string ToString()
        {
            return FormMessage(Error, ErrorParameters);
        }

        private static string FormMessage(ErrorType error, string[] errorParameters)
        {
            return string.Format(errorMessage[error], errorParameters);
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
            info.AddValue("Type", Error);
            info.AddValue("Parameters", ErrorParameters);
        }
    }
}
