using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyInterpreter.AST;
using ValueType = MyInterpreter.ValueType;

namespace MyInterpreter
{
    public sealed partial class Parser
    {
        #region Initialize Dictionaries

        private static readonly Dictionary<string, MethodType> stringToFunctionType =
            new Dictionary<string, MethodType>()
            {
                {"Sin", MethodType.Sin},
                {"Cos", MethodType.Cos}
            };

        private static readonly Dictionary<string, ValueType> stringToValueType =
            new Dictionary<string, ValueType>()
            {
                {"double", ValueType.Double},
                {"string", ValueType.String},
                {"int", ValueType.Int},
                {"void", ValueType.Void}
            };

        private static readonly Dictionary<TokenType, BinaryOperationType> tokenTypeToBinaryOperationType =
            new Dictionary<TokenType, BinaryOperationType>()
            {
                {TokenType.Plus, BinaryOperationType.Plus},
                {TokenType.Minus, BinaryOperationType.Minus},
                {TokenType.Multiply, BinaryOperationType.Multiply},
                {TokenType.Divide, BinaryOperationType.Divide},
                {TokenType.Degree, BinaryOperationType.Degree}
            };

        private static readonly Dictionary<TokenType, UnaryOperationType> tokenTypeToUnaryOperationType =
            new Dictionary<TokenType, UnaryOperationType>()
            {
                {TokenType.Plus, UnaryOperationType.Plus},
                {TokenType.Minus, UnaryOperationType.Minus}
            };

        private static readonly Dictionary<TokenType, CompareOperator> tokenTypeToCompareOperator =
            new Dictionary<TokenType, CompareOperator>()
            {
                {TokenType.Equal, CompareOperator.IsEqual},
                {TokenType.NotEqual, CompareOperator.IsNotEqual},
                {TokenType.Less, CompareOperator.IsLess},
                {TokenType.LessOrEqual, CompareOperator.IsLessOrEqual},
                {TokenType.Greater, CompareOperator.IsGreater},
                {TokenType.GreaterOrEqual, CompareOperator.IsGreaterOrEqual}
            };

        #endregion

        public const string MainFunction = "Main";
        private const string callPrint = "Print";
        private const int charLength = 1;

        public bool IsCorrect
        {
            get
            {
                if (!isRun)
                {
                    throw new InvalidOperationException();
                }
                return errors.Count <= 0;
            }
        }

        private bool isRun = false;
        private readonly Lexer lexer;
        private readonly List<Error> errors = new List<Error>();
        private Program program = null;

        private readonly List<Function> functionList = new List<Function>();
        private readonly List<Statement> simpleStatementList = new List<Statement>();
        private readonly Statement fakeStatement = null;

        public Parser(string inputText)
        {
            if (inputText == null)
            {
                throw new ArgumentNullException();
            }
            this.lexer = new Lexer(inputText);
        }

        public Parser(Lexer lexer)
        {
            if (lexer == null)
            {
                throw new ArgumentNullException();
            }
            this.lexer = lexer;
        }

        public static bool IsValueType(string indentifier)
        {
            return stringToValueType.ContainsKey(indentifier);
        }

        public static bool IsFunction(string identifier)
        {
            return callPrint.Equals(identifier, StringComparison.InvariantCulture)
                   || stringToFunctionType.ContainsKey(identifier);
        }

        public Program ParseProgram()
        {
            isRun = true;
            program = EatProgram();
            HandleJumpStatements();
            HandleFunctionCallAndCheckType();
            if (lexer.HaveErrors)
            {
                errors.AddRange(lexer.GetErrors());
            }
            return program;
        }

        public Condition ParseCondition()
        {
            Condition condition;
            try
            {
                condition = EatCondition();
            }
            catch (Exception)
            {
                return null;
            }
            if (lexer.Peek().Type == TokenType.EOF)
            {
                return condition;
            }
            else
            {
                return null;
            }
        }

        private Program EatProgram()
        {
            Position startPosition = lexer.Peek().Position;
            List<Node> nodeList = new List<Node>();
            Node node;
            bool skipToNextNode = false;

            while (lexer.Peek().Type != TokenType.EOF)
            {
                skipToNextNode = false;
                try
                {
                    if ((node = EatFunction()) != null)
                    {
                        nodeList.Add(node);
                    }
                    else if ((node = EatDeclaration()) != null)
                    {
                        if (lexer.Peek().Type != TokenType.Semicolon)
                        {
                            errors.Add(Error.CreateError(
                                new ParserException(ParserException.ErrorType.MissingSemicolon, lexer.GetLastTokenPosition())));
                        }
                        else
                        {
                            lexer.Pop();
                        }
                        nodeList.Add(node);
                    }
                    else
                    {
                        skipToNextNode = true;
                    }
                }
                catch (ParserException parserExc)
                {
                    skipToNextNode = true;
                    errors.Add(Error.CreateError(parserExc));
                }
                if (skipToNextNode)
                {
                    Position start = lexer.Peek().Position;
                    lexer.Pop();
                    lexer.SkipToNextToken(new TokenType[] { TokenType.Semicolon, TokenType.CloseCurlyBrace });
                    lexer.Pop();
                    Position end = lexer.GetLastTokenPosition();
                    errors.Add(Error.CreateError(
                        new ParserException(ParserException.ErrorType.UnidentifiedProgramPart, GetNodePosition(start, end))));
                }
            }

            Position endPosition = lexer.GetLastTokenPosition();
            Function main = null, temp;

            for (int i = 0; i < nodeList.Count; i++)
            {
                if ((temp = nodeList[i] as Function) != null &&
                    temp.Signature.Name.Equals(MainFunction) &&
                    temp.Signature.Parameters.Count == 0)
                {
                    main = temp;
                    break;
                }
            }

            if (main == null)
            {
                errors.Add(Error.CreateError(
                    new ParserException(ParserException.ErrorType.DoesNotContainMainMethod, MainFunction, GetNextCharPosition())));
            }

            Program program = new Program(nodeList, main, GetNodePosition(startPosition, endPosition));
            program.StatementList = simpleStatementList;
            return program;
        }

        private Block EatBlock(bool isIndependent = true)
        {
            Token token = lexer.Peek();
            Position startPosition = token.Position;

            if (lexer.Peek().Type != TokenType.OpenCurlyBrace)
            {
                return null;
            }
            lexer.Pop();

            int reserveIndex = ReservePlaceInSimpleStatementList();

            List<Statement> statementList = new List<Statement>();
            Statement statement;

            while (true)
            {
                try
                {
                    if ((statement = EatStatement()) == null)
                    {
                        break;
                    }
                    statementList.Add(statement);
                }
                catch (ParserException parseExc)
                {
                    errors.Add(Error.CreateError(parseExc));
                    lexer.SkipToNextToken(new TokenType[] { TokenType.Semicolon, TokenType.CloseCurlyBrace,
                        TokenType.OpenCurlyBrace, TokenType.If, TokenType.While });
                    if (lexer.Peek().Type == TokenType.CloseCurlyBrace)
                    {
                        break;
                    }
                    if (lexer.Peek().Type == TokenType.Semicolon)
                    {
                        lexer.Pop();
                    }
                }
            }

            token = lexer.Peek();
            Position endPosition = token.Position;
            Block block = new Block(statementList, isIndependent, startPosition);
            simpleStatementList[reserveIndex] = block;
            BlockEnd end;

            if (token.Type == TokenType.CloseCurlyBrace)
            {
                lexer.Pop();
                end = new BlockEnd(block, endPosition.Resize(1));
                SaveStatement(end);
            }
            else
            {
                errors.Add(Error.CreateError(new ParserException(
                    ParserException.ErrorType.MissingCloseCurlyBrace,
                    GetNextCharPosition())));
                end = new BlockEnd(block, endPosition.Resize(0));
            }
            block.BlockEnd = end;
            return block;
        }

        private int ReservePlaceInSimpleStatementList()
        {
            simpleStatementList.Add(fakeStatement);
            return simpleStatementList.Count - 1;
        }

        private Statement EatStatement()
        {
            Statement statement;
            Token token;
            List<Label> labels = EatLabels();

            if ((statement = EatIterationStatement()) != null)
            {
            }
            else if ((statement = EatJumpStatement()) != null)
            {
                token = lexer.Peek();
                if (token.Type != TokenType.Semicolon)
                {
                    throw new ParserException(ParserException.ErrorType.MissingSemicolon, GetNextCharPosition());
                }
                lexer.Pop();
            }
            else if ((statement = EatPrint()) != null)
            {
                token = lexer.Peek();
                if (token.Type != TokenType.Semicolon)
                {
                    throw new ParserException(ParserException.ErrorType.MissingSemicolon, GetNextCharPosition());
                }
                lexer.Pop();
            }
            else if ((statement = EatFunctionCall()) != null)
            {
                token = lexer.Peek();
                if (token.Type != TokenType.Semicolon)
                {
                    throw new ParserException(ParserException.ErrorType.MissingSemicolon, GetNextCharPosition());
                }
                lexer.Pop();
            }
            else if ((statement = EatAssignment()) != null)
            {
                token = lexer.Peek();
                if (token.Type != TokenType.Semicolon)
                {
                    throw new ParserException(ParserException.ErrorType.MissingSemicolon, GetNextCharPosition());
                }
                lexer.Pop();
            }
            else if ((statement = EatDeclaration()) != null)
            {
                token = lexer.Peek();
                if (token.Type != TokenType.Semicolon)
                {
                    throw new ParserException(ParserException.ErrorType.MissingSemicolon, GetNextCharPosition());
                }
                lexer.Pop();
            }
            else if ((statement = EatIf()) != null)
            {
            }
            else if ((statement = EatBlock()) != null)
            {
            }
            else if (lexer.Peek().Type == TokenType.Semicolon)
            {
                token = lexer.Pop();
                statement = new Statement(token.Position.Resize(1));
                SaveStatement(statement);
            }
            else
            {
                return null;
            }

            statement.Labels = labels;
            return statement;
        }

        private Statement EatIterationStatement()
        {
            Statement statement;
            if ((statement = EatWhile()) != null)
            {
                return statement;
            }
            if ((statement = EatDoWhile()) != null)
            {
                return statement;
            }
            if ((statement = EatFor()) != null)
            {
                return statement;
            }
            return null;
        }

        #region Iteration statements

        private While EatWhile()
        {
            Position startPosition = lexer.Peek().Position;

            if (lexer.Peek().Type != TokenType.While)
            {
                return null;
            }
            lexer.Pop();

            if (lexer.Peek().Type != TokenType.OpenParenthesis)
            {
                throw new ParserException(ParserException.ErrorType.MissingOpenParenthesis, GetNextCharPosition());
            }
            lexer.Pop();

            Condition condition;
            if ((condition = EatCondition()) == null)
            {
                condition = new Condition(lexer.Peek().Position.Resize());
            }
            Fork loopCondition = new Fork(condition);
            SaveStatement(loopCondition);

            if (lexer.Peek().Type != TokenType.CloseParenthesis)
            {
                throw new ParserException(ParserException.ErrorType.MissingCloseParenthesis, GetNextCharPosition());
            }
            lexer.Pop();

            Statement statement;
            if ((statement = EatStatement()) == null)
            {
                throw new ParserException(ParserException.ErrorType.MissingStatement, lexer.Peek().Position);
            }

            Position endPosition = lexer.GetLastTokenPosition();
            return new While(loopCondition, statement, GetNodePosition(startPosition, endPosition));
        }

        private DoWhile EatDoWhile()
        {
            Position startPosition = lexer.Peek().Position;

            if (lexer.Peek().Type != TokenType.Do)
            {
                return null;
            }
            lexer.Pop();

            Statement statement;
            if ((statement = EatStatement()) == null)
            {
                throw new ParserException(ParserException.ErrorType.MissingStatement, GetNextCharPosition());
            }

            if (lexer.Peek().Type != TokenType.While)
            {
                throw new ParserException(ParserException.ErrorType.MissingWhile, lexer.Peek().Position);
            }
            lexer.Pop();

            if (lexer.Peek().Type != TokenType.OpenParenthesis)
            {
                throw new ParserException(ParserException.ErrorType.MissingOpenParenthesis, GetNextCharPosition());
            }
            lexer.Pop();

            Condition condition;
            if ((condition = EatCondition()) == null)
            {
                condition = new Condition(lexer.Peek().Position.Resize());
            }
            Fork loopCondition = new Fork(condition);
            SaveStatement(loopCondition);

            if (lexer.Peek().Type != TokenType.CloseParenthesis)
            {
                throw new ParserException(ParserException.ErrorType.MissingCloseParenthesis, GetNextCharPosition());
            }
            lexer.Pop();

            if (lexer.Peek().Type != TokenType.Semicolon)
            {
                throw new ParserException(ParserException.ErrorType.MissingSemicolon, GetNextCharPosition());
            }
            lexer.Pop();

            Position endPosition = lexer.GetLastTokenPosition();
            return new DoWhile(statement, condition, GetNodePosition(startPosition, endPosition));
        }

        private For EatFor()
        {
            Position startPosition = lexer.Peek().Position;

            if (lexer.Peek().Type != TokenType.For)
            {
                return null;
            }
            lexer.Pop();

            if (lexer.Peek().Type != TokenType.OpenParenthesis)
            {
                throw new ParserException(ParserException.ErrorType.MissingOpenParenthesis, GetNextCharPosition());
            }
            lexer.Pop();

            Statement initializer;
            if ((initializer = EatAssignment()) == null &&
                (initializer = EatDeclaration()) == null)
            {
                initializer = new Statement(lexer.Peek().Position.Resize());
            }
            if (lexer.Peek().Type != TokenType.Semicolon)
            {
                throw new ParserException(ParserException.ErrorType.MissingAssignmentOrDeclaration, GetNextCharPosition());
            }
            lexer.Pop();

            Condition condition;
            if ((condition = EatCondition()) == null)
            {
                condition = new Condition(lexer.Peek().Position.Resize());
            }
            Fork loopCondition = new Fork(condition);
            SaveStatement(loopCondition);

            if (lexer.Peek().Type != TokenType.Semicolon)
            {
                throw new ParserException(ParserException.ErrorType.MissingCondition, GetNextCharPosition());
            }
            lexer.Pop();

            Statement iterator;
            if ((iterator = EatAssignment()) == null)
            {
                iterator = new Statement(lexer.Peek().Position.Resize());
            }
            if (lexer.Peek().Type != TokenType.CloseParenthesis)
            {
                throw new ParserException(ParserException.ErrorType.MissingCloseParenthesis, GetNextCharPosition());
            }
            lexer.Pop();

            Statement body;
            if ((body = EatStatement()) == null)
            {
                throw new ParserException(ParserException.ErrorType.MissingStatement, lexer.Peek().Position);
            }

            Position endPosition = lexer.GetLastTokenPosition();
            return new For(initializer, loopCondition, iterator, body, GetNodePosition(startPosition, endPosition));
        }

        #endregion

        private JumpStatement EatJumpStatement()
        {
            JumpStatement statement;
            if ((statement = EatGoto()) != null)
            {
                return statement;
            }
            else if ((statement = EatBreak()) != null)
            {
                return statement;
            }
            else if ((statement = EatReturn()) != null)
            {
                return statement;
            }
            else
            {
                return null;
            }
        }

        #region Jump statements

        private Goto EatGoto()
        {
            Token token = lexer.Peek();
            if (token.Type != TokenType.Goto)
            {
                return null;
            }

            Position startPosition = token.Position;
            lexer.Pop();
            token = lexer.Peek();

            if (!IsLabel(token))
            {
                throw new ParserException(ParserException.ErrorType.MissingLabel, token.Position);
            }
            lexer.Pop();

            Position endPosition = token.Position;
            return SaveStatement(new Goto(new Label(token.Name, token.Position), GetNodePosition(startPosition, endPosition)));
        }

        private Return EatReturn()
        {
            Token token = lexer.Peek();
            if (token.Type != TokenType.Return)
            {
                return null;
            }

            Position startPosition = token.Position;
            lexer.Pop();
            Expression expression;

            if ((expression = EatExpression()) == null)
            {
                expression = new Constant(ValueType.CreateValue(ValueType.Void), lexer.Peek().Position.Resize());
            }

            Position endPosition = lexer.GetLastTokenPosition();
            return SaveStatement(new Return(expression, GetNodePosition(startPosition, endPosition)));
        }

        private Break EatBreak()
        {
            if (lexer.Peek().Type != TokenType.Break)
            {
                return null;
            }

            return SaveStatement(new Break(lexer.Pop().Position));
        }

        #endregion

        private If EatIf()
        {
            Position startPosition = lexer.Peek().Position;
            if (lexer.Peek().Type != TokenType.If)
            {
                return null;
            }
            lexer.Pop();
            if (lexer.Peek().Type != TokenType.OpenParenthesis)
            {
                throw new ParserException(ParserException.ErrorType.MissingOpenParenthesis, GetNextCharPosition());
            }
            lexer.Pop();
            Condition condition;
            if ((condition = EatCondition()) == null)
            {
                condition = new Condition(lexer.Peek().Position.Resize());
            }
            Fork ifCondition = SaveStatement(new Fork(condition));
            if (lexer.Peek().Type != TokenType.CloseParenthesis)
            {
                throw new ParserException(ParserException.ErrorType.MissingCloseParenthesis, GetNextCharPosition());
            }
            lexer.Pop();
            Statement trueStatement, falseStatement;
            if ((trueStatement = EatStatement()) == null)
            {
                throw new ParserException(ParserException.ErrorType.MissingStatement, lexer.Peek().Position);
            }
            Position endPosition = lexer.GetLastTokenPosition();
            if (lexer.Peek().Type != TokenType.Else)
            {
                endPosition = lexer.GetLastTokenPosition();
                return new If(ifCondition, trueStatement, GetNodePosition(startPosition, endPosition));
            }
            lexer.Pop();
            if ((falseStatement = EatStatement()) == null)
            {
                throw new ParserException(ParserException.ErrorType.MissingStatement, lexer.Peek().Position);
            }
            endPosition = lexer.GetLastTokenPosition();
            return new If(condition, trueStatement, falseStatement, GetNodePosition(startPosition, endPosition));
        }

        private Condition EatCondition()
        {
            Position startPosition = lexer.Peek().Position;
            Expression leftOperand;
            if ((leftOperand = EatExpression()) == null)
            {
                return null;
            }
            Token token = lexer.Peek();
            if (!IsComparer(token))
            {
                throw new ParserException(ParserException.ErrorType.MissingCompareOperator, token.Position);
            }
            CompareOperator comparer = tokenTypeToCompareOperator[token.Type];
            lexer.Pop();
            Expression rightOperand;
            if ((rightOperand = EatExpression()) == null)
            {
                throw new ParserException(ParserException.ErrorType.MissingExpression, lexer.Peek().Position);
            }
            Position endPosition = lexer.GetLastTokenPosition();
            return new Condition(leftOperand, comparer, rightOperand, GetNodePosition(startPosition, endPosition));
        }

        private Assignment EatAssignment()
        {
            Position startPosition = lexer.Peek().Position;
            Expression assignVariable;

            if ((assignVariable = EatSlicedVariable()) == null)
            {
                return null;
            }

            if (lexer.Peek().Type != TokenType.Assignment)
            {
                throw new ParserException(ParserException.ErrorType.MissingAssignment, lexer.Peek().Position);
            }
            lexer.Pop();

            Expression expression;
            if ((expression = EatArrayCreator()) != null)
            {
            }
            else if ((expression = EatExpression()) == null)
            {
                throw new ParserException(ParserException.ErrorType.MissingExpression, lexer.Peek().Position);
            }
            Position endPosition = lexer.GetLastTokenPosition();

            return SaveStatement(new Assignment(assignVariable, expression, GetNodePosition(startPosition, endPosition)));
        }

        private Declaration EatDeclaration(bool isAssignmentAllowed = true)
        {
            Position startPosition = lexer.Peek().Position;
            ValueType valueType;

            if ((valueType = EatValueType()) == null)
            {
                return null;
            }
            if (ValueType.Equals(valueType, ValueType.Void))
            {
                LexerRollbackValueType(valueType);
                return null;
            }

            Variable variable;
            if ((variable = EatVariable(Variable.AccessorType.Set)) == null)
            {
                throw new ParserException(ParserException.ErrorType.MissingVariable, lexer.Peek().Position);
            }

            Position endPosition;
            if (!isAssignmentAllowed || lexer.Peek().Type != TokenType.Assignment)
            {
                endPosition = lexer.GetLastTokenPosition();
                if (isAssignmentAllowed)
                {
                    return SaveStatement(new Declaration(valueType, variable, null, GetNodePosition(startPosition, endPosition)));
                }
                else
                {
                    return new Declaration(valueType, variable, null, GetNodePosition(startPosition, endPosition));
                }
            }
            lexer.Pop();

            Expression expression;
            if ((expression = EatArrayCreator()) != null)
            {
            }
            else if ((expression = EatExpression()) == null)
            {
                throw new ParserException(ParserException.ErrorType.MissingExpression, lexer.Peek().Position);
            }

            endPosition = lexer.GetLastTokenPosition();
            return SaveStatement(new Declaration(valueType, variable, expression, GetNodePosition(startPosition, endPosition)));
        }

        private FunctionCall EatFunctionCall()
        {
            Position startPosition = lexer.Peek().Position;
            Token identifier = lexer.Peek();

            if (identifier.Type != TokenType.Identifier)
            {
                return null;
            }
            lexer.Pop();

            if (lexer.Peek().Type != TokenType.OpenParenthesis)
            {
                lexer.Previous();
                return null;
            }
            lexer.Pop();

            List<Expression> parameters = new List<Expression>();
            Expression expression;
            bool isCommaExist = false;

            while (true)
            {
                if ((expression = EatExpression()) == null)
                {
                    if (isCommaExist)
                    {
                        throw new ParserException(ParserException.ErrorType.MissingExpression, GetNextCharPosition());
                    }
                    else
                    {
                        break;
                    }
                }

                parameters.Add(expression);

                if (lexer.Peek().Type != TokenType.Comma)
                {
                    break;
                }

                isCommaExist = true;
                lexer.Pop();
            }

            if (lexer.Peek().Type != TokenType.CloseParenthesis)
            {
                throw new ParserException(ParserException.ErrorType.MissingCloseParenthesis, GetNextCharPosition());
            }
            lexer.Pop();

            Position endPosition = lexer.GetLastTokenPosition();
            return SaveStatement(new FunctionCall(identifier.Name, parameters, GetNodePosition(startPosition, endPosition)));
        }

        private Function EatFunction()
        {
            Position startPosition = lexer.Peek().Position;
            FunctionSignature signature;

            if ((signature = EatFunctionSignature()) == null)
            {
                return null;
            }

            Block block;
            if ((block = EatBlock(false)) == null)
            {
                throw new ParserException(ParserException.ErrorType.MissingBlock, lexer.Peek().Position);
            }

            Position endPosition = lexer.GetLastTokenPosition();
            Function function = new Function(signature, block, GetNodePosition(startPosition, endPosition));
            AddFunction(function);
            return function;
        }

        private FunctionSignature EatFunctionSignature()
        {
            Position startPosition = lexer.Peek().Position;
            ValueType functionValueType;

            if ((functionValueType = EatValueType()) == null)
            {
                return null;
            }

            Token identifierToken = lexer.Peek();

            if (identifierToken.Type != TokenType.Identifier)
            {
                LexerRollbackValueType(functionValueType);
                return null;
            }
            lexer.Pop();

            if (lexer.Peek().Type != TokenType.OpenParenthesis)
            {
                lexer.Previous();
                LexerRollbackValueType(functionValueType);
                return null;
                //throw new ParserException(ParserException.ErrorType.MissingOpenParenthesis, GetNextCharPosition());
            }
            lexer.Pop();

            List<Declaration> parameters = new List<Declaration>();
            Declaration declaration;
            bool isCommaExist = false;

            while (true)
            {
                if ((declaration = EatDeclaration(false)) == null)
                {
                    if (isCommaExist)
                    {
                        throw new ParserException(ParserException.ErrorType.MissingExpression, GetNextCharPosition());
                    }
                    else
                    {
                        break;
                    }
                }

                parameters.Add(declaration);

                if (lexer.Peek().Type != TokenType.Comma)
                {
                    break;
                }

                isCommaExist = true;
                lexer.Pop();
            }

            if (lexer.Peek().Type != TokenType.CloseParenthesis)
            {
                throw new ParserException(ParserException.ErrorType.MissingCloseParenthesis, GetNextCharPosition());
            }
            lexer.Pop();

            Position endPosition = lexer.GetLastTokenPosition();
            return new FunctionSignature(functionValueType, identifierToken.Name,
                parameters, GetNodePosition(startPosition, endPosition));
        }

        private ValueType EatValueType(ValueType simpleType = null)
        {
            Token token = lexer.Peek();
            ValueType valueType;

            if (simpleType == null)
            {
                if (!IsSimpleValueType(token))
                {
                    return null;
                }
                valueType = stringToValueType[token.Name];
                lexer.Pop();
            }
            else
            {
                valueType = simpleType;
            }


            //Внимание, формально массив создается не справо налево, а слева направо!!!
            //При реализации других слайсов(если вдруг), к примеру int[,][], функцию нужно изменить!!!
            while (lexer.Peek().Type == TokenType.OpenBracket)
            {
                if (valueType.SimpleType == ValueType.Type.Void)
                {
                    throw new ParserException(ParserException.ErrorType.VoidCannotBeUsed, lexer.Peek().Position);
                }
                lexer.Pop();
                if (lexer.Peek().Type != TokenType.CloseBracket)
                {
                    throw new ParserException(ParserException.ErrorType.MissingCloseBracket, lexer.Peek().Position);
                }
                lexer.Pop();
                valueType = ValueType.Array(valueType);
            }

            return valueType;
        }

        private List<Label> EatLabels()
        {
            List<Label> labels = new List<Label>();
            Token token;

            while (true)
            {
                token = lexer.Peek();

                if (!IsLabel(token))
                {
                    break;
                }
                lexer.Pop();

                if (lexer.Peek().Type != TokenType.Colon)
                {
                    lexer.Previous();
                    break;
                }
                labels.Add(new Label(token.Name, token.Position));
                lexer.Pop();
            }

            return labels;
        }

        private Print EatPrint()
        {
            Position startPosition = lexer.Peek().Position;
            List<Expression> printParameters = new List<Expression>();
            Expression expression;

            if (!IsPrint(lexer.Peek()))
            {
                return null;
            }
            lexer.Pop();

            if (lexer.Peek().Type != TokenType.OpenParenthesis)
            {
                throw new ParserException(ParserException.ErrorType.MissingOpenParenthesis, lexer.Peek().Position);
            }
            lexer.Pop();

            while (true)
            {
                if ((expression = EatExpression()) == null)
                {
                    if (printParameters.Count <= 0)
                    {
                        break;
                    }
                    throw new ParserException(ParserException.ErrorType.MissingExpression, lexer.Peek().Position);
                }

                printParameters.Add(expression);

                if (lexer.Peek().Type != TokenType.Comma)
                {
                    break;
                }
                lexer.Pop();
            }

            if (lexer.Peek().Type != TokenType.CloseParenthesis)
            {
                throw new ParserException(ParserException.ErrorType.MissingCloseParenthesis, lexer.Peek().Position);
            }
            lexer.Pop();

            Position endPosition = lexer.GetLastTokenPosition();
            return SaveStatement(new Print(printParameters, GetNodePosition(startPosition, endPosition)));
        }

        private Expression EatExpression()
        {
            Token token = lexer.Peek();
            Position startPosition = lexer.Peek().Position;
            Position endPosition;
            Expression expression;

            if ((expression = EatTerm()) == null)
            {
                return null;
            }

            BinaryOperationType operation;
            Expression rightOperand;

            while (true)
            {
                token = lexer.Peek();
                if (!IsAdditionOrSubstraction(token))
                {
                    return expression;
                }
                operation = tokenTypeToBinaryOperationType[token.Type];
                lexer.Pop();
                if ((rightOperand = EatTerm()) == null)
                {
                    throw new ParserException(ParserException.ErrorType.MissingExpression, lexer.Peek().Position);
                }
                endPosition = lexer.GetLastTokenPosition();
                expression = AddExpression(
                    new BinaryOperation(expression, operation, rightOperand, GetNodePosition(startPosition, endPosition)));
            }
        }

        private Expression EatTerm()
        {
            Position startPosition = lexer.Peek().Position;
            Position endPosition;
            Expression expression;

            if ((expression = EatDegree()) == null)
            {
                return null;
            }

            Token token;
            BinaryOperationType operation;
            Expression rightOperand;

            while (true)
            {
                token = lexer.Peek();
                if (!IsMultiplicationOrDivision(token))
                {
                    return expression;
                }
                operation = tokenTypeToBinaryOperationType[token.Type];
                lexer.Pop();
                if ((rightOperand = EatDegree()) == null)
                {
                    throw new ParserException(ParserException.ErrorType.MissingExpression, lexer.Peek().Position);
                }
                endPosition = lexer.GetLastTokenPosition();
                expression = AddExpression(
                    new BinaryOperation(expression, operation, rightOperand, GetNodePosition(startPosition, endPosition)));
            }
        }

        private Expression EatDegree()
        {
            Position startPosition = lexer.Peek().Position;
            Expression expression;

            if ((expression = EatUnaryOperation()) == null)
            {
                return null;
            }

            Token token;
            BinaryOperationType operation;
            Expression rightOperand;

            token = lexer.Peek();
            if (!IsInvolution(token))
            {
                return expression;
            }
            operation = tokenTypeToBinaryOperationType[token.Type];
            lexer.Pop();

            if ((rightOperand = EatDegree()) == null)
            {
                throw new ParserException(ParserException.ErrorType.MissingExpression, lexer.Peek().Position);
            }

            Position endPosition = lexer.GetLastTokenPosition();
            return AddExpression(new BinaryOperation(expression, operation, rightOperand, GetNodePosition(startPosition, endPosition)));
        }

        private Expression EatUnaryOperation()
        {
            Position startPosition = lexer.Peek().Position;
            Token token = lexer.Peek();
            Expression expression;

            if (IsUnaryOperation(token))
            {
                UnaryOperationType operation = tokenTypeToUnaryOperationType[token.Type];
                lexer.Pop();

                if ((expression = EatUnaryOperation()) == null)
                {
                    throw new ParserException(ParserException.ErrorType.MissingExpression, lexer.Peek().Position);
                }

                Position endPosition = lexer.GetLastTokenPosition();
                return AddExpression(new UnaryOperation(operation, expression, GetNodePosition(startPosition, endPosition)));
            }

            if ((expression = EatSlice()) != null)
            {
                return expression;
            }

            return null;
        }

        private Expression EatSlice(Expression slisedExpression = null)
        {
            Position startPosition = lexer.Peek().Position;
            Position endPosition;
            Expression expression;

            if (slisedExpression == null)
            {
                if ((expression = EatFactor()) == null)
                {
                    return null;
                }
            }
            else
            {
                expression = slisedExpression;
            }

            Expression indexer;
            while (true)
            {
                if (lexer.Peek().Type != TokenType.OpenBracket)
                {
                    break;
                }
                lexer.Pop();
                if ((indexer = EatExpression()) == null)
                {
                    throw new ParserException(ParserException.ErrorType.MissingExpression, lexer.Peek().Position);
                }
                if (lexer.Peek().Type != TokenType.CloseBracket)
                {
                    throw new ParserException(ParserException.ErrorType.MissingCloseBracket, GetNextCharPosition());
                }
                lexer.Pop();
                endPosition = lexer.GetLastTokenPosition();
                expression = AddExpression(new Slice(expression, indexer, GetNodePosition(startPosition, endPosition)));
            }

            return expression;
        }

        private Expression EatFactor()
        {
            Expression expression;

            if ((expression = EatConstant()) != null)
            {
            }
            else if ((expression = EatMethod()) != null)
            {
            }
            else if ((expression = EatFunctionCallExpr()) != null)
            {
            }
            else if ((expression = EatVariable(Variable.AccessorType.Get)) != null)
            {
            }
            else if (lexer.Peek().Type == TokenType.OpenParenthesis)
            {
                lexer.Pop();

                if ((expression = EatExpression()) == null)
                {
                    throw new ParserException(ParserException.ErrorType.MissingExpression, lexer.Peek().Position);
                }
                if ((lexer.Peek().Type != TokenType.CloseParenthesis))
                {
                    throw new ParserException(ParserException.ErrorType.MissingCloseParenthesis, lexer.Peek().Position);
                }
                lexer.Pop();
            }
            else
            {
                return null;
            }

            return expression;
        }

        private FunctionCallExpr EatFunctionCallExpr()
        {
            FunctionCall call;

            if ((call = EatFunctionCall()) == null)
            {
                return null;
            }
            simpleStatementList.RemoveAt(simpleStatementList.Count - 1);

            return new FunctionCallExpr(call, call.Position);
        }

        private Method EatMethod()
        {
            Token token = lexer.Peek();
            Position startPosition = token.Position;

            if (!IsMethod(token))
            {
                return null;
            }
            lexer.Pop();

            if (lexer.Peek().Type != TokenType.OpenParenthesis)
            {
                throw new ParserException(ParserException.ErrorType.MissingOpenParenthesis, lexer.Peek().Position);
            }
            lexer.Pop();

            Expression expression;
            if ((expression = EatExpression()) == null)
            {
                throw new ParserException(ParserException.ErrorType.MissingExpression, lexer.Peek().Position);
            }

            if ((lexer.Peek().Type != TokenType.CloseParenthesis))
            {
                throw new ParserException(ParserException.ErrorType.MissingCloseParenthesis, lexer.Peek().Position);
            }
            lexer.Pop();

            Position endPosition = lexer.GetLastTokenPosition();
            return AddExpression(new Method(stringToFunctionType[token.Name], expression, GetNodePosition(startPosition, endPosition)));
        }

        private Variable EatVariable(Variable.AccessorType accessorType)
        {
            Token token = lexer.Peek();

            if (!IsVariable(token))
            {
                return null;
            }
            lexer.Pop();

            return AddExpression(new Variable(token.Name, accessorType, token.Position));
        }

        //Only for Assignment
        private Expression EatSlicedVariable()
        {
            Variable variable;

            if ((variable = EatVariable(Variable.AccessorType.Set)) == null)
            {
                return null;
            }

            Expression temp = EatSlice(variable);

            if (temp is Slice)
            {
                variable.Accessor = Variable.AccessorType.Get;
            }
            return temp;
        }

        private Constant EatConstant()
        {
            Token token = lexer.Pop();

            switch (token.Type)
            {
                case TokenType.Number:
                    return AddExpression(new Constant(ParseNumber(token), token.Position));
                case TokenType.RegularStringLiteral:
                case TokenType.VerbatimStringLiteral:
                    return AddExpression(new Constant(new String(token.Name), token.Position));
                case TokenType.WrongStringLiteral:
                    return AddExpression(new Constant(new String(token.Name), token.Position));
                default:
                    lexer.Previous();
                    return null;
            }
        }

        private ArrayCreator EatArrayCreator()
        {
            Position startPosition = lexer.Peek().Position;

            if (lexer.Peek().Type != TokenType.New)
            {
                return null;
            }
            lexer.Pop();

            ValueType arrayType;
            if (!IsSimpleValueType(lexer.Peek()))
            {
                throw new ParserException(ParserException.ErrorType.MissingType, lexer.Peek().Position);
            }
            arrayType = stringToValueType[lexer.Peek().Name];
            lexer.Pop();

            bool isFirstDimension = true;
            List<Expression> arrayDimensions = new List<Expression>();
            Expression size;

            while (true)
            {
                if (lexer.Peek().Type != TokenType.OpenBracket)
                {
                    if (isFirstDimension)
                    {
                        throw new ParserException(ParserException.ErrorType.MissingOpenBracket, lexer.Peek().Position);
                    }
                    else
                    {
                        break;
                    }
                }
                lexer.Pop();

                if ((size = EatExpression()) == null)
                {
                    if (isFirstDimension)
                    {
                        throw new ParserException(ParserException.ErrorType.MissingExpression, lexer.Peek().Position);
                    }
                    else
                    {
                        lexer.Previous();
                        break;
                    }
                }

                if (lexer.Peek().Type != TokenType.CloseBracket)
                {
                    throw new ParserException(ParserException.ErrorType.MissingCloseBracket, lexer.Peek().Position);
                }
                lexer.Pop();

                isFirstDimension = false;
                arrayDimensions.Add(size);
            }

            arrayType = EatValueType(arrayType);
            Position endPosition = lexer.GetLastTokenPosition();
            return AddExpression(new ArrayCreator(arrayType, arrayDimensions, GetNodePosition(startPosition, endPosition)));
        }

        private bool IsAdditionOrSubstraction(Token token)
        {
            return token.Type == TokenType.Plus || token.Type == TokenType.Minus;
        }

        private bool IsMultiplicationOrDivision(Token token)
        {
            return token.Type == TokenType.Multiply || token.Type == TokenType.Divide;
        }

        private bool IsInvolution(Token token)
        {
            return token.Type == TokenType.Degree;
        }

        private bool IsUnaryOperation(Token token)
        {
            return tokenTypeToUnaryOperationType.ContainsKey(token.Type);
        }

        private bool IsVariable(Token token)
        {
            if (token.Type != TokenType.Identifier)
            {
                return false;
            }

            return !(IsMethod(token) || IsSimpleValueType(token) || IsPrint(token));
        }

        private bool IsLabel(Token token)
        {
            if (token.Type != TokenType.Identifier)
            {
                return false;
            }

            return !(IsMethod(token) || IsSimpleValueType(token));
        }

        private bool IsMethod(Token token)
        {
            if (token.Type != TokenType.Identifier)
            {
                return false;
            }

            return stringToFunctionType.ContainsKey(token.Name);
        }

        private bool IsComparer(Token token)
        {
            return tokenTypeToCompareOperator.ContainsKey(token.Type);
        }

        private bool IsSimpleValueType(Token token)
        {
            if (token.Type != TokenType.Identifier)
            {
                return false;
            }
            return stringToValueType.ContainsKey(token.Name);
        }

        private bool IsPrint(Token token)
        {
            if (token.Type != TokenType.Identifier)
            {
                return false;
            }

            return callPrint.Equals(token.Name, StringComparison.InvariantCulture);
        }

        private Value ParseNumber(Token number)
        {
            int intValue;

            if (int.TryParse(number.Name, out intValue))
            {
                return new Int(intValue);
            }

            double doubleValue;

            if (double.TryParse(number.Name.Replace(Lexer.NumberDecimalSeparator, ','), out doubleValue))
            {
                return new Double(doubleValue);
            }

            throw new ParserException(ParserException.ErrorType.NumberOutOfRange, number.Position);
        }

        private void AddFunction(Function function)
        {
            for (int i = 0; i < functionList.Count; i++)
            {
                if (functionList[i].Signature.Equals(function.Signature))
                {
                    errors.Add(Error.CreateError(
                      new ParserException(ParserException.ErrorType.FunctionAlreadyDefines, function.Signature.Position)));
                    return;
                }
            }

            functionList.Add(function);
        }

        private T AddExpression<T>(T expression) where T : Expression
        {
            if (expression == null)
            {
                throw new ArgumentNullException();
            }
            return expression;
        }

        private T SaveStatement<T>(T statement) where T : Statement
        {
            simpleStatementList.Add(statement);
            return statement;
        }

        private void LexerRollbackValueType(ValueType valueType)
        {
            if (valueType == null)
            {
                return;
            }

            while (valueType.SimpleType == ValueType.Type.Array)
            {
                lexer.Previous();// ']'
                lexer.Previous();// '['
                valueType = valueType.InternalType;
            }

            lexer.Previous();// double, int...
        }

        private Position GetNodePosition(Position start, Position end)
        {
            return new Position(
                start.LineNumber,
                start.ColumnNumber,
                start.BeginIndex,
                end.BeginIndex - start.BeginIndex + end.Length
                );
        }

        private Position GetNextCharPosition()
        {
            Position position = lexer.GetLastTokenPosition();

            return new Position(position.LineNumber, position.ColumnNumber,
                position.BeginIndex + position.Length, charLength);
        }

        internal List<Error> GetErrors()
        {
            if (IsCorrect)
            {
                throw new InvalidOperationException();
            }
            return errors;
        }
    }
}
