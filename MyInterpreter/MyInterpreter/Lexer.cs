using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter
{
    public sealed class Lexer
    {
        private enum NumberState
        {
            Error,
            BeginNumberStart,
            AfterDigitFinal,
            AfterNumberDecimalSeparator,
            AfterDigitsWithSeparatorFinal,
            AfterExponentSymbol,
            AfterUnarySign,
            AfterExponentNumberFinal,
            End
        };

        private enum ErrorType
        {
            UnknownToken,
            UnterminatedStringLiteral,
            UnrecognizedEscapeSequence
        }

        public const int MaxMergedTokenNumber = 5;

        private const char plus = '+';
        private const char minus = '-';
        private const char multiply = '*';
        private const char divide = slash;
        private const char openParenthesis = '(';
        private const char closeParenthesis = ')';
        private const char openCurlyBrace = '{';
        private const char closeCurlyBrace = '}';
        private const char openBracket = '[';
        private const char closeBracket = ']';
        private const char semicolon = ';';
        private const char colon = ':';
        private const char underscore = '_';
        private const char assignment = '=';
        private const char less = '<';
        private const char greater = '>';
        private const char exclamationMark = '!';
        private const char quotationMark = '\"';
        private const char newLine = '\n';
        private const char backslash = '\\';
        private const char slash = '/';
        private const char comma = ',';
        private const char et = '@';
        public const char NumberDecimalSeparator = '.';
        private static readonly char[] exponent = new char[] { 'e', 'E' };
        private const int wrongTokenLength = 1;

        #region Initialize Dictionaries

        private static readonly Dictionary<string, TokenType> keyWordsToTokenType =
            new Dictionary<string, TokenType>
            {
                { "new", TokenType.New },
                { "if", TokenType.If },
                { "else", TokenType.Else },
                { "for", TokenType.For },
                { "while", TokenType.While },
                { "do", TokenType.Do },
                { "goto", TokenType.Goto },
                { "return", TokenType.Return },
                { "break", TokenType.Break }
            };

        private static readonly Dictionary<ErrorType, string> errorToMassage =
            new Dictionary<ErrorType, string>
            {
                { ErrorType.UnknownToken, "unknown token" },
                { ErrorType.UnterminatedStringLiteral, "unfinished string constant" },
                { ErrorType.UnrecognizedEscapeSequence, "unrecognized escape sequence" }
            };

        private static readonly Dictionary<TokenType, string> tokenTypeToKeyWords =
            keyWordsToTokenType.ToDictionary(pair => pair.Value, pair => pair.Key);

        private static readonly Dictionary<char, char> escapeSequenceToChar =
            new Dictionary<char, char>
            {
                { 'n', '\n' },
                { '\\', '\\' },
                { '\"', '\"' },
                { '\'', '\'' }
            };

        #endregion

        public bool HaveErrors
        {
            get
            {
                return errorsList.Count > 0;
            }
        }

        private bool considerInsignificantToken = false;
        private string inputCode;
        private int beginningOfNextToken = 0;
        private List<int> newLinePositionsList = new List<int>() { -1 };
        private List<Error> errorsList = new List<Error>();
        private List<Token> readTokensList;
        private int currentToken = 0;

        public Lexer(string inputCode, bool considerInsignificantToken = false)
        {
            if (inputCode == null)
            {
                throw new ArgumentNullException();
            }
            this.inputCode = inputCode;
            readTokensList = new List<Token>();
            this.considerInsignificantToken = considerInsignificantToken;
        }

        public Token Peek()
        {
            if (currentToken < readTokensList.Count)
            {
                return readTokensList[currentToken];
            }
            readTokensList.Add(GetNextToken());
            return readTokensList[currentToken];
        }

        public Token Pop()
        {
            if (currentToken < readTokensList.Count)
            {
                return readTokensList[currentToken++];
            }
            if (readTokensList.Count > 0 && readTokensList[readTokensList.Count - 1].Type == TokenType.EOF)
            {
                return readTokensList[readTokensList.Count - 1];
            }
            readTokensList.Add(GetNextToken());
            return readTokensList[currentToken++];
        }

        public Token Previous()
        {
            if (currentToken <= 0)
            {
                throw new InvalidOperationException();
            }
            currentToken--;
            return readTokensList[currentToken];
        }

        public void SkipToNextToken(TokenType desiredType)
        {
            Token token = Pop();

            while (token.Type != TokenType.EOF)
            {
                if (token.Type == desiredType)
                {
                    Previous();
                    return;
                }
                token = Pop();
            }
        }

        public void SkipToNextToken(TokenType[] desiredTypes)
        {
            if (desiredTypes == null || desiredTypes.Length <= 0)
            {
                throw new ArgumentException("Array of desired types is null or empty");
            }

            Token token = Pop();

            while (token.Type != TokenType.EOF)
            {
                if (desiredTypes.Contains(token.Type))
                {
                    Previous();
                    return;
                }
                token = Pop();
            }
        }

        public Position GetLastTokenPosition()
        {
            if (currentToken <= 0)
            {
                return new Position(0, 0, 0, 0);
            }
            return readTokensList[currentToken - 1].Position;
        }

        private Token GetNextToken()
        {
            Token token;
            while (true)
            {
                token = ParseNextToken();
                if (considerInsignificantToken)
                {
                    return token;
                }
                if (!(token.Type == TokenType.SingleLineComment) && !(token.Type == TokenType.WhiteSpace))
                {
                    return token;
                }
            }
        }

        private Token ParseNextToken()
        {
            if (EndOfInputCode(beginningOfNextToken))
            {
                return new Token(TokenType.EOF, GetCurrentTokenPosition());
            }

            Token token;
            if (EatWhiteSpaceToken(out token))
            {
                return token;
            }

            switch (inputCode[beginningOfNextToken++])
            {
                case plus:
                    return new Token(TokenType.Plus, GetCurrentTokenPosition(1));
                case minus:
                    return new Token(TokenType.Minus, GetCurrentTokenPosition(1));
                case multiply:
                    {
                        if (NextCharIs(multiply))
                        {
                            beginningOfNextToken++;
                            return new Token(TokenType.Degree, GetCurrentTokenPosition(2));
                        }
                        return new Token(TokenType.Multiply, GetCurrentTokenPosition(1));
                    }
                case divide:
                    {
                        if (NextCharIs(divide))
                        {
                            beginningOfNextToken--;
                            break;
                        }
                        return new Token(TokenType.Divide, GetCurrentTokenPosition(1));
                    }
                case less:
                    {
                        if (NextCharIs(assignment))
                        {
                            beginningOfNextToken++;
                            return new Token(TokenType.LessOrEqual, GetCurrentTokenPosition(2));
                        }
                        return new Token(TokenType.Less, GetCurrentTokenPosition(1));
                    }
                case greater:
                    {
                        if (NextCharIs(assignment))
                        {
                            beginningOfNextToken++;
                            return new Token(TokenType.GreaterOrEqual, GetCurrentTokenPosition(2));
                        }
                        return new Token(TokenType.Greater, GetCurrentTokenPosition(1));
                    }
                case assignment:
                    {
                        if (NextCharIs(assignment))
                        {
                            beginningOfNextToken++;
                            return new Token(TokenType.Equal, GetCurrentTokenPosition(2));
                        }
                        return new Token(TokenType.Assignment, GetCurrentTokenPosition(1));
                    }
                case exclamationMark:
                    {
                        if (NextCharIs(assignment))
                        {
                            beginningOfNextToken++;
                            return new Token(TokenType.NotEqual, GetCurrentTokenPosition(2));
                        }
                        beginningOfNextToken--;
                        break;
                    }
                case openParenthesis:
                    return new Token(TokenType.OpenParenthesis, GetCurrentTokenPosition(1));
                case closeParenthesis:
                    return new Token(TokenType.CloseParenthesis, GetCurrentTokenPosition(1));
                case openCurlyBrace:
                    return new Token(TokenType.OpenCurlyBrace, GetCurrentTokenPosition(1));
                case closeCurlyBrace:
                    return new Token(TokenType.CloseCurlyBrace, GetCurrentTokenPosition(1));
                case openBracket:
                    return new Token(TokenType.OpenBracket, GetCurrentTokenPosition(1));
                case closeBracket:
                    return new Token(TokenType.CloseBracket, GetCurrentTokenPosition(1));
                case semicolon:
                    return new Token(TokenType.Semicolon, GetCurrentTokenPosition(1));
                case comma:
                    return new Token(TokenType.Comma, GetCurrentTokenPosition(1));
                case colon:
                    return new Token(TokenType.Colon, GetCurrentTokenPosition(1));
                default:
                    beginningOfNextToken--;
                    break;
            }

            if (EatIdentifierOrKeyWord(out token))
            {
                return token;
            }

            if (EatRegularStringLiteral(out token))
            {
                return token;
            }

            if (EatVerbatimStringLiteral(out token))
            {
                return token;
            }

            string number;
            if ((number = ParseNumber(beginningOfNextToken)) != null)
            {
                beginningOfNextToken += number.Length;
                return new Token(TokenType.Number, number, GetCurrentTokenPosition(number.Length));
            }

            string comment;
            if ((comment = ParseSingleLineComment()) != null)
            {
                return new Token(TokenType.SingleLineComment, comment, GetCurrentTokenPosition(comment.Length + 2));
            }

            beginningOfNextToken += wrongTokenLength;
            AddErrorInErrorList(ErrorType.UnknownToken, GetCurrentTokenPosition(wrongTokenLength));
            return new Token(TokenType.Unknown,
                inputCode.Substring(beginningOfNextToken - wrongTokenLength, wrongTokenLength),
                GetCurrentTokenPosition(wrongTokenLength));
        }

        private bool EatWhiteSpaceToken(out Token token)
        {
            StringBuilder whiteSpaceBuilder = new StringBuilder();

            int index = beginningOfNextToken;
            while (!EndOfInputCode(index))
            {
                if (!char.IsWhiteSpace(inputCode, index))
                {
                    break;
                }
                if (inputCode[index] == newLine)
                {
                    NewLineAppeared(index);
                }
                whiteSpaceBuilder.Append(inputCode[index]);
                index++;
            }
            beginningOfNextToken = index;

            if (whiteSpaceBuilder.Length <= 0)
            {
                token = default(Token);
                return false;
            }

            token = new Token(TokenType.WhiteSpace, whiteSpaceBuilder.ToString(), GetCurrentTokenPosition(whiteSpaceBuilder.Length));
            return true;
        }

        /// <summary>
        /// USE ONLY in switch in ParseNextToken function, where beginningOfNextToken++
        /// </summary>
        private bool NextCharIs(char desiredChar)
        {
            return !EndOfInputCode(beginningOfNextToken) &&
                inputCode[beginningOfNextToken] == desiredChar;
        }

        private bool EatIdentifierOrKeyWord(out Token token)
        {
            int startIndex = beginningOfNextToken;
            int currentIndex = beginningOfNextToken;

            if (!IsAlphaChar(inputCode[currentIndex]))
            {
                token = default(Token);
                return false;
            }

            StringBuilder identifierBuilder = new StringBuilder();
            identifierBuilder.Append(inputCode[currentIndex]);
            currentIndex++;

            while (!EndOfInputCode(currentIndex) && IsOtherChar(inputCode[currentIndex]))
            {
                identifierBuilder.Append(inputCode[currentIndex]);
                currentIndex++;
            }

            beginningOfNextToken = currentIndex;
            string identifier = identifierBuilder.ToString();

            if (IsKeyWord(identifier))
            {
                token = new Token(keyWordsToTokenType[identifier], GetCurrentTokenPosition(identifier.Length));
            }
            else
            {
                token = new Token(TokenType.Identifier, identifier, GetCurrentTokenPosition(identifier.Length));
            }
            return true;
        }

        private bool EatRegularStringLiteral(out Token token)
        {
            if (inputCode[beginningOfNextToken] != quotationMark)
            {
                token = default(Token);
                return false;
            }

            int stringBeginningIndex = beginningOfNextToken;
            int currentIndex = beginningOfNextToken + 1;
            StringBuilder regularStringBuilder = new StringBuilder();
            char charInString;
            bool isCorrectRegularString = true;

            while (!EndOfInputCode(currentIndex))
            {
                if (inputCode[currentIndex] == quotationMark)
                {
                    beginningOfNextToken = currentIndex + 1;
                    token = CreateRegularStringLiteral(isCorrectRegularString, regularStringBuilder.ToString(),
                        stringBeginningIndex, beginningOfNextToken - stringBeginningIndex);
                    return true;
                }

                if (inputCode[currentIndex] == newLine)
                {
                    beginningOfNextToken = currentIndex;
                    AddErrorInErrorList(ErrorType.UnterminatedStringLiteral,
                        GetCurrentTokenPosition(beginningOfNextToken - stringBeginningIndex));
                    token = new Token(TokenType.WrongStringLiteral,
                            inputCode.Substring(stringBeginningIndex, beginningOfNextToken - stringBeginningIndex),
                            GetCurrentTokenPosition(beginningOfNextToken - stringBeginningIndex));
                    return true;
                }

                if (inputCode[currentIndex] == backslash)
                {
                    currentIndex++;
                    if (EndOfInputCode(currentIndex))
                    {
                        isCorrectRegularString = false;
                        AddErrorInErrorList(ErrorType.UnrecognizedEscapeSequence, GetPosition(currentIndex - 1, 1));
                        continue;
                    }

                    if (!escapeSequenceToChar.ContainsKey(inputCode[currentIndex]))
                    {
                        isCorrectRegularString = false;
                        AddErrorInErrorList(ErrorType.UnrecognizedEscapeSequence, GetPosition(currentIndex - 1, 2));
                        continue;
                    }
                    charInString = escapeSequenceToChar[inputCode[currentIndex]];
                }
                else
                {
                    charInString = inputCode[currentIndex];
                }

                regularStringBuilder.Append(charInString);
                currentIndex++;
            }

            beginningOfNextToken = currentIndex;
            AddErrorInErrorList(ErrorType.UnterminatedStringLiteral,
                GetCurrentTokenPosition(beginningOfNextToken - stringBeginningIndex));
            token = new Token(TokenType.WrongStringLiteral,
                    inputCode.Substring(stringBeginningIndex, beginningOfNextToken - stringBeginningIndex),
                    GetCurrentTokenPosition(beginningOfNextToken - stringBeginningIndex));
            return true;
        }

        private Token CreateRegularStringLiteral(bool isCorrectStringLiteral, string stringLiteral, int startIndex, int length)
        {
            return isCorrectStringLiteral ?
                new Token(TokenType.RegularStringLiteral, stringLiteral,
                    GetCurrentTokenPosition(length)) :
                new Token(TokenType.WrongStringLiteral,
                    inputCode.Substring(startIndex, length),
                    GetCurrentTokenPosition(length));
        }

        private bool EatVerbatimStringLiteral(out Token token)
        {
            if (inputCode[beginningOfNextToken] != et ||
                EndOfInputCode(beginningOfNextToken + 1) ||
                inputCode[beginningOfNextToken + 1] != quotationMark)
            {
                token = default(Token);
                return false;
            }

            int stringBeginningIndex = beginningOfNextToken;
            StringBuilder strBuilder = new StringBuilder();
            int currentIndex = beginningOfNextToken + 2;

            while (!EndOfInputCode(currentIndex))
            {
                if (inputCode[currentIndex] == quotationMark)
                {
                    if (EndOfInputCode(currentIndex + 1) ||
                        inputCode[currentIndex + 1] != quotationMark)
                    {
                        beginningOfNextToken = currentIndex + 1;
                        token = new Token(TokenType.VerbatimStringLiteral, strBuilder.ToString(),
                            GetCurrentTokenPosition(beginningOfNextToken - stringBeginningIndex));
                        return true;
                    }
                    strBuilder.Append(quotationMark);
                    currentIndex++;
                }
                else
                {
                    strBuilder.Append(inputCode[currentIndex]);
                    if (inputCode[currentIndex] == newLine)
                    {
                        NewLineAppeared(currentIndex);
                    }
                }
                currentIndex++;
            }

            beginningOfNextToken = currentIndex;
            AddErrorInErrorList(ErrorType.UnterminatedStringLiteral,
                GetCurrentTokenPosition(beginningOfNextToken - stringBeginningIndex));
            token = new Token(TokenType.WrongStringLiteral,
                inputCode.Substring(stringBeginningIndex, beginningOfNextToken - stringBeginningIndex),
                GetCurrentTokenPosition(beginningOfNextToken - stringBeginningIndex));
            return true;
        }

        private void AddErrorInErrorList(ErrorType type, Position position)
        {
            errorsList.Add(new Error(position, errorToMassage[type]));
        }

        private bool IsAlphaChar(char symbol)
        {
            return char.IsLetter(symbol) || char.Equals(symbol, underscore);
        }

        private bool IsOtherChar(char symbol)
        {
            return IsAlphaChar(symbol) || char.IsDigit(symbol);
        }

        private bool IsKeyWord(string identifier)
        {
            return keyWordsToTokenType.ContainsKey(identifier);
        }

        private string ParseNumber(int beginningIndex)
        {
            StringBuilder numberBuilder = new StringBuilder();
            int currentIndex = beginningIndex;
            NumberState previousState = NumberState.BeginNumberStart;
            NumberState currrentState = NumberState.BeginNumberStart;

            while (!EndOfInputCode(currentIndex))
            {
                currrentState = GetNextNumberState(currrentState, inputCode[currentIndex]);
                if (currrentState == NumberState.Error)
                {
                    break;
                }
                previousState = currrentState;
                numberBuilder.Append(inputCode[currentIndex]);
                currentIndex++;
            }
            if (previousState == NumberState.AfterDigitFinal
                || previousState == NumberState.AfterDigitsWithSeparatorFinal
                || previousState == NumberState.AfterExponentNumberFinal)
            {
                return numberBuilder.ToString();
            }
            return null;
        }

        private string ParseSingleLineComment()
        {
            if (inputCode[beginningOfNextToken] != slash ||
                EndOfInputCode(beginningOfNextToken + 1) ||
                inputCode[beginningOfNextToken + 1] != slash)
            {
                return null;
            }

            int stringBeginningIndex = beginningOfNextToken;
            StringBuilder commentBuilder = new StringBuilder();
            int currentIndex = beginningOfNextToken + 2;

            while (!EndOfInputCode(currentIndex) && inputCode[currentIndex] != newLine)
            {
                commentBuilder.Append(inputCode[currentIndex]);
                currentIndex++;
            }

            beginningOfNextToken = currentIndex;
            return commentBuilder.ToString();
        }

        private NumberState GetNextNumberState(NumberState currentState, char symbol)
        {
            switch (currentState)
            {
                case NumberState.BeginNumberStart:
                    {
                        if (char.IsDigit(symbol))
                        {
                            return NumberState.AfterDigitFinal;
                        }
                        if (symbol == NumberDecimalSeparator)
                        {
                            return NumberState.AfterNumberDecimalSeparator;
                        }
                        return NumberState.Error;
                    }
                case NumberState.AfterDigitFinal:
                    {
                        if (char.IsDigit(symbol))
                        {
                            return NumberState.AfterDigitFinal;
                        }
                        if (symbol == NumberDecimalSeparator)
                        {
                            return NumberState.AfterNumberDecimalSeparator;
                        }
                        if (exponent.Contains<char>(symbol))
                        {
                            return NumberState.AfterExponentSymbol;
                        }
                        return NumberState.Error;
                    }
                case NumberState.AfterNumberDecimalSeparator:
                    {
                        if (char.IsDigit(symbol))
                        {
                            return NumberState.AfterDigitsWithSeparatorFinal;
                        }
                        return NumberState.Error;
                    }
                case NumberState.AfterDigitsWithSeparatorFinal:
                    {
                        if (char.IsDigit(symbol))
                        {
                            return NumberState.AfterDigitsWithSeparatorFinal;
                        }
                        if (exponent.Contains<char>(symbol))
                        {
                            return NumberState.AfterExponentSymbol;
                        }
                        return NumberState.Error;
                    }
                case NumberState.AfterExponentSymbol:
                    {
                        if (symbol == plus || symbol == minus)
                        {
                            return NumberState.AfterUnarySign;
                        }
                        if (char.IsDigit(symbol))
                        {
                            return NumberState.AfterExponentNumberFinal;
                        }
                        return NumberState.Error;
                    }
                case NumberState.AfterUnarySign:
                    {
                        if (char.IsDigit(symbol))
                        {
                            return NumberState.AfterExponentNumberFinal;
                        }
                        return NumberState.Error;
                    }
                case NumberState.AfterExponentNumberFinal:
                    {
                        if (char.IsDigit(symbol))
                        {
                            return NumberState.AfterExponentNumberFinal;
                        }
                        return NumberState.Error;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        private Position GetCurrentTokenPosition(int tokenLength = 0)
        {
            int i;
            for (i = newLinePositionsList.Count - 1; i >= 0; i--)
            {
                if (beginningOfNextToken - 1 - tokenLength >= newLinePositionsList[i])
                {
                    break;
                }
            }
            return new Position(i, beginningOfNextToken - newLinePositionsList[i] - 1 - tokenLength,
                beginningOfNextToken - tokenLength, tokenLength);
        }

        private Position GetPosition(int startIndex, int length)
        {
            int i;
            for (i = newLinePositionsList.Count - 1; i >= 0; i--)
            {
                if (startIndex >= newLinePositionsList[i])
                {
                    break;
                }
            }
            return new Position(i, startIndex - newLinePositionsList[i] - 1, startIndex, length);
        }

        private bool EndOfInputCode(int currentPosition)
        {
            return currentPosition >= inputCode.Length;
        }

        private void NewLineAppeared(int index)
        {
            newLinePositionsList.Add(index);
        }

        public static List<Token> SplitIntoTokens(string text, bool considerInsignificantToken = false)
        {
            Lexer lexer = new Lexer(text, considerInsignificantToken);

            lexer.SkipToNextToken(TokenType.EOF);
            lexer.readTokensList.RemoveAt(lexer.readTokensList.Count - 1);
            return lexer.readTokensList;
        }

        public static string TokenToOriginalString(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Unknown:
                    return token.Name;
                case TokenType.WrongStringLiteral:
                    return token.Name;

                case TokenType.SingleLineComment:
                    return slash.ToString() + slash + token.Name;
                case TokenType.WhiteSpace:
                    return token.Name;

                case TokenType.Number:
                    return token.Name;
                case TokenType.RegularStringLiteral:
                    return quotationMark.ToString() + RegularStringToOriginalString(token.Name) + quotationMark;
                case TokenType.VerbatimStringLiteral:
                    return et.ToString() + quotationMark + token.Name + quotationMark;
                case TokenType.Identifier:
                    return token.Name;
                case TokenType.Plus:
                    return plus.ToString();
                case TokenType.Minus:
                    return minus.ToString();
                case TokenType.Multiply:
                    return multiply.ToString();
                case TokenType.Divide:
                    return divide.ToString();
                case TokenType.Degree:
                    return divide.ToString() + divide.ToString();
                case TokenType.Assignment:
                    return assignment.ToString();
                case TokenType.Equal:
                    return assignment.ToString() + assignment.ToString();
                case TokenType.NotEqual:
                    return exclamationMark.ToString() + assignment.ToString();
                case TokenType.Less:
                    return less.ToString();
                case TokenType.LessOrEqual:
                    return less.ToString() + assignment.ToString();
                case TokenType.Greater:
                    return greater.ToString();
                case TokenType.GreaterOrEqual:
                    return greater.ToString() + assignment.ToString();
                case TokenType.OpenParenthesis:
                    return openParenthesis.ToString();
                case TokenType.CloseParenthesis:
                    return closeParenthesis.ToString();
                case TokenType.OpenCurlyBrace:
                    return openCurlyBrace.ToString();
                case TokenType.CloseCurlyBrace:
                    return closeCurlyBrace.ToString();
                case TokenType.OpenBracket:
                    return openBracket.ToString();
                case TokenType.CloseBracket:
                    return closeBracket.ToString();
                case TokenType.Comma:
                    return comma.ToString();
                case TokenType.New:
                case TokenType.If:
                case TokenType.Else:
                case TokenType.For:
                case TokenType.While:
                case TokenType.Do:
                case TokenType.Goto:
                case TokenType.Break:
                case TokenType.Return:
                    return tokenTypeToKeyWords[token.Type];
                case TokenType.Semicolon:
                    return semicolon.ToString();
                case TokenType.Colon:
                    return colon.ToString();
                case TokenType.EOF:
                    return string.Empty;
                default:
                    throw new NotSupportedException("Невозможно преобразовать токен обратно в строку");
            }
        }

        private static string RegularStringToOriginalString(string text)
        {
            StringBuilder originalStringBuilder = new StringBuilder();

            foreach (char symbol in text)
            {
                if (escapeSequenceToChar.ContainsValue(symbol))
                {
                    originalStringBuilder.Append(slash);
                    originalStringBuilder.Append(
                    escapeSequenceToChar.FirstOrDefault(pair => pair.Value == symbol).Key);
                    continue;
                }
                originalStringBuilder.Append(symbol);
            }

            return originalStringBuilder.ToString();
        }

        internal List<Error> GetErrors()
        {
            if (!HaveErrors)
            {
                throw new InvalidOperationException();
            }
            return errorsList;
        }
    }
}
