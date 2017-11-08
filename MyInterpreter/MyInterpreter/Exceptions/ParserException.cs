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
    public sealed class ParserException : BaseException, ISerializable
    {
        public enum ErrorType
        {
            MissingCloseCurlyBrace,
            MissingSemicolon,
            MissingOpenParenthesis,
            MissingOpenBracket,
            MissingCondition,
            MissingCloseParenthesis,
            MissingCloseBracket,
            MissingBlock,
            MissingStatement,
            MissingCompareOperator,
            MissingVariable,
            MissingExpression,
            UnidentifiedProgramPart,
            MissingWhile,
            MissingAssignmentOrDeclaration,
            MissingAssignment,
            MissingLabel,
            LabelDoesNotExist,
            LabelDuplicate,
            NumberOutOfRange,
            MissingType,
            FunctionAlreadyDefines,
            ImplicitConversion,
            VariableAlreadyExists,
            InvalidBinaryOperator,
            InvalidCompareOperator,
            InvalidIndexer,
            NotDeclaredVariable,
            FunctionNotExist,
            InvalidSetIndexer,
            NoEnclosingLoop,
            VoidCannotBeUsed,
            DoesNotContainMainMethod
        }

        #region Initialize Dictionaries
        private static readonly Dictionary<ErrorType, string> errorToMessage =
            new Dictionary<ErrorType, string>
            {
                { ErrorType.MissingCloseCurlyBrace, "Missing close curly brace" },
                { ErrorType.MissingSemicolon, "Missing semicolon" },
                { ErrorType.MissingOpenParenthesis, "Missing open parenthesis" },
                { ErrorType.MissingOpenBracket, "Missing open bracket" },
                { ErrorType.MissingCondition, "Missing condition" },
                { ErrorType.MissingCloseParenthesis, "Missing close parenthesis" },
                { ErrorType.MissingCloseBracket, "Missing close bracket" },
                { ErrorType.MissingBlock, "Missing block" },
                { ErrorType.MissingStatement, "Missing statement" },
                { ErrorType.MissingCompareOperator, "Missing compare operator" },
                { ErrorType.MissingVariable, "Variable expected" },
                { ErrorType.MissingExpression, "Missing expression" },
                { ErrorType.UnidentifiedProgramPart, "Unidentified program part" },
                { ErrorType.MissingWhile, "Missing \"while\"" },
                { ErrorType.MissingAssignmentOrDeclaration, "Missing assignment or declaration" },
                { ErrorType.MissingAssignment, "Missing assignment" },
                { ErrorType.MissingLabel, "Missing label" },
                { ErrorType.LabelDoesNotExist, "Label does not exist" },
                { ErrorType.LabelDuplicate, "Label is a duplicate" },
                { ErrorType.NumberOutOfRange, "Number out of range" },
                { ErrorType.MissingType, "Missing type" },
                { ErrorType.FunctionAlreadyDefines, "Function is already defines with the same parameter types" },
                { ErrorType.ImplicitConversion, "Can not implicitly convert type \"{0}\" to \"{1}\"" },
                { ErrorType.VariableAlreadyExists, "A variable with name \"{0}\" already exists" },
                { ErrorType.InvalidBinaryOperator, "Operator \"{0}\" cannot be applied to operands of type \"{1}\" and \"{2}\"" },
                { ErrorType.InvalidCompareOperator, "Operator \"{0}\" cannot be applied to operands of type \"{1}\" and \"{2}\"" },
                { ErrorType.InvalidIndexer, "Cannot apply indexing with [{1}] to an expression of type \"{0}\"" },
                { ErrorType.NotDeclaredVariable, "\"{0}\" is not declared" },
                { ErrorType.FunctionNotExist, "A function with name \"{0}\" does not exists" },
                { ErrorType.InvalidSetIndexer, "{0}[{1}]={2} operation error" },
                { ErrorType.NoEnclosingLoop, "No enclosing loop" },
                { ErrorType.VoidCannotBeUsed, "Keyword 'void' cannot be used in this context" },
                { ErrorType.DoesNotContainMainMethod, "Does not contain a '{0}()' method suitable for an entry point" }
            };
        #endregion

        public readonly ErrorType Error;
        public readonly string[] ErrorParameters;

        public ParserException(ErrorType error, Position position)
            : this(error, new string[0], position)
        {
        }

        public ParserException(ErrorType error, string errorParameter, Position position)
            : this(error, new string[] { errorParameter }, position)
        {
        }

        public ParserException(ErrorType error, string[] errorParameters, Position position)
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
