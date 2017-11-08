using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyInterpreter.AST;
using Interfaces;
using ValueType = MyInterpreter.ValueType;

namespace MyInterpreter
{
    public sealed class Interpreter
    {
        public bool IsCorrectBuild
        {
            get
            {
                return parser.IsCorrect;
            }
        }

        public bool IsDebugging { get; private set; }

        public Statement CurrentStatement
        {
            get
            {
                if (statementInfoStack.Count > 0)
                {
                    return statementInfoStack.Peek().First;
                }
                else
                {
                    return null;
                }
            }
        }

        public BaseInterpreterStatus Status { get; private set; }

        private List<Error> errors = new List<Error>();

        private readonly string inputCode;
        private readonly IWriter writer;

        private bool isRelease = false;
        private bool isBreakpointTriggered = false;
        private Value lastReturn = null;
        private Parser parser;
        public Program Program { get; private set; }

        private Stack<Triplet<Statement, ExpressionCalculator, DictionaryList<string, Pair<Value, Variable.Type>>>> statementInfoStack =
            new Stack<Triplet<Statement, ExpressionCalculator, DictionaryList<string, Pair<Value, Variable.Type>>>>();
        private Dictionary<Triplet<Statement, ExpressionCalculator, DictionaryList<string, Pair<Value, Variable.Type>>>, bool> interpretedStatement =
            new Dictionary<Triplet<Statement, ExpressionCalculator, DictionaryList<string, Pair<Value, Variable.Type>>>, bool>();
        private Dictionary<Triplet<Statement, ExpressionCalculator, DictionaryList<string, Pair<Value, Variable.Type>>>, bool> interpretedBreakpoint =
            new Dictionary<Triplet<Statement, ExpressionCalculator, DictionaryList<string, Pair<Value, Variable.Type>>>, bool>();
        private Dictionary<Triplet<Statement, ExpressionCalculator, DictionaryList<string, Pair<Value, Variable.Type>>>, bool> calculateStatement =
            new Dictionary<Triplet<Statement, ExpressionCalculator, DictionaryList<string, Pair<Value, Variable.Type>>>, bool>();
        private Dictionary<HitCountBreakpoint, int> breakpointHits = new Dictionary<HitCountBreakpoint, int>();

        public Interpreter(string inputCode, IWriter writer = null)
        {
            if (inputCode == null)
            {
                throw new ArgumentNullException();
            }

            this.inputCode = inputCode;
            this.writer = writer;
            this.IsDebugging = false;
            this.parser = new Parser(inputCode);
            this.Program = parser.ParseProgram();
        }

        public BaseInterpreterStatus BuildSolution()
        {
            if (IsDebugging)
            {
                return null;
            }

            if (IsCorrectBuild)
            {
                Status = new CorrectBuildingStatus();
            }
            else
            {
                Status = new HaveErrorsStatus(parser.GetErrors());
            }

            return Status;
        }

        public BaseInterpreterStatus RunSolution()
        {
            if (IsDebugging)
            {
                return null;
            }

            if (!IsCorrectBuild)
            {
                return BuildSolution();
            }

            isRelease = true;

            try
            {
                Interpret();
            }
            catch (BaseException exc)
            {
                isRelease = false;
                return new HaveErrorsStatus(Error.CreateError(exc));
            }

            isRelease = false;
            return new CorrectRunningStatus();
        }

        private void Interpret()
        {
            PreInterpret();

            while (statementInfoStack.Count > 0)
            {
                MakeStep();
            }
        }

        private void PreInterpret()
        {
            DictionaryList<string, Pair<Value, Variable.Type>> emptyList = new DictionaryList<string, Pair<Value, Variable.Type>>();
            emptyList.AddDictionary();

            DictionaryList<string, Pair<Value, Variable.Type>> dictionaryForEntryFunction =
                new DictionaryList<string, Pair<Value, Variable.Type>>(emptyList.Dictionaries[0]);
            dictionaryForEntryFunction.AddDictionary();
            PushInStack(Program.MainFunction.Block, dictionaryForEntryFunction);

            for (int i = Program.NodeList.Count - 1; i >= 0; i--)
            {
                if (Program.NodeList[i] is Declaration)
                {
                    PushInStack(Program.NodeList[i] as Declaration, emptyList);
                }
            }
        }

        public BaseInterpreterStatus StartDebbuging()
        {
            BaseInterpreterStatus status = BuildSolution();

            if (!IsCorrectBuild)
            {
                IsDebugging = false;
                return status;
            }

            PreInterpret();
            IsDebugging = true;
            return new DebuggingProcessStatus(statementInfoStack.Peek().First, statementInfoStack.Peek().Third);
        }

        public BaseInterpreterStatus MakeStepOver()
        {
            if (!IsDebugging)
            {
                return null;
            }

            Triplet<Statement, ExpressionCalculator, DictionaryList<string, Pair<Value, Variable.Type>>> currentTriplet =
                statementInfoStack.Peek();
            try
            {
                do
                {
                    MakeStep();
                }
                while (statementInfoStack.Count > 0 &&
                    (statementInfoStack.Contains(currentTriplet) || statementInfoStack.Peek().First is Invisible ||
                    calculateStatement.ContainsKey(statementInfoStack.Peek())));
            }
            catch (BaseException exc)
            {
                IsDebugging = false;
                ClearIntermediateData();
                return new HaveErrorsStatus(Error.CreateError(exc));
            }

            if (statementInfoStack.Count <= 0)
            {
                IsDebugging = false;
                ClearIntermediateData();
                return new EndDebugStatus();
            }

            return new DebuggingProcessStatus(statementInfoStack.Peek().First, statementInfoStack.Peek().Third);
        }

        public BaseInterpreterStatus MakeStepInto()
        {
            if (!IsDebugging)
            {
                return null;
            }

            Triplet<Statement, ExpressionCalculator, DictionaryList<string, Pair<Value, Variable.Type>>> currentTriplet =
                statementInfoStack.Peek();
            try
            {
                do
                {
                    MakeStep();
                }
                while (statementInfoStack.Count > 0 &&
                    (statementInfoStack.Peek() == currentTriplet || statementInfoStack.Peek().First is Invisible ||
                    calculateStatement.ContainsKey(statementInfoStack.Peek())));
            }
            catch (BaseException exc)
            {
                IsDebugging = false;
                ClearIntermediateData();
                return new HaveErrorsStatus(Error.CreateError(exc));
            }

            if (statementInfoStack.Count <= 0)
            {
                IsDebugging = false;
                ClearIntermediateData();
                return new EndDebugStatus();
            }

            return new DebuggingProcessStatus(statementInfoStack.Peek().First, statementInfoStack.Peek().Third);
        }

        public BaseInterpreterStatus Debug()
        {
            if (!IsDebugging)
            {
                return null;
            }

            try
            {
                do
                {
                    MakeStep();
                }
                while (statementInfoStack.Count > 0 &&
                    !isBreakpointTriggered);
            }
            catch (BaseException exc)
            {
                IsDebugging = false;
                ClearIntermediateData();
                return new HaveErrorsStatus(Error.CreateError(exc));
            }

            if (statementInfoStack.Count <= 0)
            {
                IsDebugging = false;
                ClearIntermediateData();
                return new EndDebugStatus();
            }

            return new DebuggingProcessStatus(statementInfoStack.Peek().First, statementInfoStack.Peek().Third);
        }

        public BaseInterpreterStatus RunToCursor(int cursor)
        {
            if (!IsDebugging)
            {
                return null;
            }

            Triplet<Statement, ExpressionCalculator, DictionaryList<string, Pair<Value, Variable.Type>>> currentTriplet =
                statementInfoStack.Peek();
            try
            {
                while (statementInfoStack.Count > 0 &&
                    (statementInfoStack.Peek() == currentTriplet || statementInfoStack.Peek().First is Invisible ||
                    !PointInPosition(cursor, statementInfoStack.Peek().First.Position)))
                {
                    MakeStep();
                }
            }
            catch (BaseException exc)
            {
                IsDebugging = false;
                ClearIntermediateData();
                return new HaveErrorsStatus(Error.CreateError(exc));
            }

            if (statementInfoStack.Count <= 0)
            {
                IsDebugging = false;
                ClearIntermediateData();
                return new EndDebugStatus();
            }

            return new DebuggingProcessStatus(statementInfoStack.Peek().First, statementInfoStack.Peek().Third);
        }

        private bool PointInPosition(int point, Position position)
        {
            return point >= position.BeginIndex && point <= position.BeginIndex + position.Length;
        }

        private void ClearIntermediateData()
        {
            statementInfoStack.Clear();
            interpretedStatement.Clear();
            interpretedBreakpoint.Clear();
            calculateStatement.Clear();
            isBreakpointTriggered = false;
            lastReturn = null;
        }

        public void StopDebbuging()
        {
            IsDebugging = false;
            ClearIntermediateData();
        }

        private void MakeStep()
        {
            isBreakpointTriggered = false;
            Triplet<Statement, ExpressionCalculator, DictionaryList<string, Pair<Value, Variable.Type>>> currentTriplet =
                statementInfoStack.Peek();
            Statement statement = currentTriplet.First;
            ExpressionCalculator calculator = currentTriplet.Second;
            DictionaryList<string, Pair<Value, Variable.Type>> variableToValue = currentTriplet.Third;
            Statement nextStatement = null;

            if (!isRelease && statement.Breakpoint != null)
            {
                if (!interpretedBreakpoint.ContainsKey(currentTriplet))
                {
                    interpretedBreakpoint.Add(currentTriplet, true);
                    PushInStack(statement.Breakpoint, variableToValue);
                    MakeStep();
                    return;
                }
            }

            if (calculator.State == CalculatorState.ExpectedValue)
            {
                calculator.ExpectationValue = lastReturn;
            }

            switch (calculator.Calculate(variableToValue))
            {
                case CalculatorState.Done:
                    break;
                case CalculatorState.ExpectedValue:
                    PushInStack(calculator.NextStatement, variableToValue);
                    calculateStatement.Add(statementInfoStack.Peek(), true);
                    return;
                default:
                    throw new NotSupportedException();
            }

            if (!(statement is BlockEnd) && !(statement is FunctionEnd) && !(statement is FunctionCall) && !(statement is Breakpoint))
            {
                lastReturn = null;
            }

            #region INTERPRET STATEMENT
            if (statement is Assignment)
            {
                InterpretAssignment(statement as Assignment, variableToValue, calculator.Result);
            }
            else if (statement is Declaration)
            {
                InterpretDeclaration(statement as Declaration, variableToValue, calculator.Result);
            }
            else if (statement is Block)
            {
                InterpretBlock(statement as Block, variableToValue);
            }
            else if (statement is BlockEnd)
            {
                InterpretBlockEnd(statement as BlockEnd, variableToValue);
            }
            else if (statement is DoWhile)
            {
                InterpretDoWhile(statement as DoWhile, variableToValue);
            }
            else if (statement is EndIterationStatement)
            {
                InterpretEndIterationStatement(statement as EndIterationStatement, variableToValue);
            }
            else if (statement is For)
            {
                InterpretFor(statement as For, variableToValue);
            }
            else if (statement is Fork)
            {
                nextStatement = InterpretFork(statement as Fork, calculator.Result);
            }
            else if (statement is FunctionCall)
            {
                if (interpretedStatement.ContainsKey(currentTriplet))
                {
                    interpretedStatement.Remove(currentTriplet);
                }
                else
                {
                    InterpretFunctionCall(statement as FunctionCall, variableToValue, calculator.Result);
                    interpretedStatement.Add(currentTriplet, true);
                    return;
                }
            }
            else if (statement is FunctionEnd)
            {
                InterpretFunctionEnd(statement as FunctionEnd, variableToValue);
            }
            else if (statement is If)
            {
                InterpretIf(statement as If, variableToValue);
            }
            else if (statement is JumpStatement)
            {
                InterpretJumpStatement(statement as JumpStatement, variableToValue, calculator.Result);
            }
            else if (statement is Print)
            {
                InterpretPrint(statement as Print, variableToValue, calculator.Result);
            }
            else if (statement is While)
            {
                InterpretWhile(statement as While, variableToValue);
            }
            else if (statement is StopBreakpoint)
            {
                InterpretStopBreakpoint(statement as StopBreakpoint, variableToValue);
            }
            else if (statement.GetType() == typeof(Statement))
            {
            }
            else if (statement is ConditionBreakpoint)
            {
                InterpretConditionBreakpoint(statement as ConditionBreakpoint, variableToValue, calculator.Result);
            }
            else if (statement is HitCountBreakpoint)
            {
                InterpretHitCountBreakpoint(statement as HitCountBreakpoint, variableToValue);
            }
            else
            {
                throw new NotSupportedException();
            }

            #endregion

            statementInfoStack.Pop();
            nextStatement = nextStatement ?? statement.NextStatement;

            if (nextStatement != null)
            {
                PushInStack(nextStatement, variableToValue);
            }
        }

        private void PushInStack(Statement statement, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue)
        {
            if (IsEmptyFork(statement))
            {
                if (statement.NextStatement != null)
                {
                    PushInStack(statement.NextStatement, variableToValue);
                }
                return;
            }
            Expression[] expressions = GetExpressions(statement);
            ExpressionCalculator calculator = new ExpressionCalculator(expressions);
            statementInfoStack.Push(new Triplet<Statement, ExpressionCalculator, DictionaryList<string, Pair<Value, Variable.Type>>>(
                statement, calculator, variableToValue));
        }

        private bool IsEmptyFork(Statement statement)
        {
            return statement is Fork
                && (statement as Fork).Condition.LeftOperand == null && (statement as Fork).Condition.RightOperand == null;
        }

        private void InterpretFunctionEnd(FunctionEnd functionEnd, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue)
        {
            variableToValue.RemoveDictionary();

            if (lastReturn == null)
            {
                lastReturn = ValueType.CreateValue(ValueType.Void);
            }
            if (!ValueType.Equals(functionEnd.Function.Signature.ReturnType, lastReturn.Type))
            {
                throw new InterpretException(InterpretException.ErrorType.FunctionDoesNotReturnValue,
                    new string[] { functionEnd.Function.Signature.Name, functionEnd.Function.Signature.ReturnType.ToString(), lastReturn.Type.ToString() },
                        functionEnd.Position);
            }
        }

        private void InterpretPrint(Print print, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue, Value[] values)
        {
            if (print.Parameters.Count != values.Length)
            {
                throw new ArgumentException();
            }

            if (values.Length == 0)
            {
                return;
            }

            string format = values[0].ToString();
            List<string> parametersStringForm = new List<string>();

            for (int i = 1; i < values.Length; i++)
            {
                parametersStringForm.Add(values[i].ToString());
            }

            string answer;

            if (parametersStringForm.Count <= 0)
            {
                answer = format;
            }
            else
            {
                try
                {
                    answer = string.Format(format, parametersStringForm.ToArray());
                }
                catch (FormatException)
                {
                    throw new InterpretException(InterpretException.ErrorType.FormatException, print.Position);
                }
            }

            writer.Write(answer);
        }

        private void InterpretFunctionCall(FunctionCall functionCall, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue, Value[] values)
        {
            if (values.Length != functionCall.Parameters.Count)
            {
                throw new ArgumentException();
            }

            DictionaryList<string, Pair<Value, Variable.Type>> newVariableToValue =
                new DictionaryList<string, Pair<Value, Variable.Type>>(variableToValue.Dictionaries[0]);
            newVariableToValue.AddDictionary();
            for (int i = 0; i < functionCall.Function.Signature.Parameters.Count; i++)
            {
                newVariableToValue.Add(functionCall.Function.Signature.Parameters[i].DeclVariable.Name,
                    new Pair<Value, Variable.Type>(values[i], Variable.Type.Assigned));
            }

            PushInStack(functionCall.Function.Block, newVariableToValue);
        }

        private void InterpretEndIterationStatement(EndIterationStatement endIterationStatement, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue)
        {
            variableToValue.RemoveDictionary();
        }

        private void InterpretJumpStatement(JumpStatement jump, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue, Value[] values)
        {
            if (jump is Break)
            {
            }
            else if (jump is Goto)
            {
            }
            else if (jump is Return)
            {
                InterpretReturn(jump as Return, variableToValue, values);
            }
            else
            {
                throw new NotSupportedException();
            }

            int level = jump.Level;
            while (level > 0)
            {
                variableToValue.RemoveDictionary();
                level--;
            }
        }

        private void InterpretReturn(Return returnStatement, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue, Value[] values)
        {
            if (values.Length != 1)
            {
                throw new ArgumentException();
            }

            lastReturn = values[0];
        }

        private void InterpretAssignment(Assignment assignment, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue, Value[] values)
        {
            if (assignment.UnaryExpr is Variable)
            {
                if (values.Length != 2)
                {
                    throw new ArgumentException();
                }
                try
                {
                    Pair<Value, Variable.Type> valuePair;
                    variableToValue.TryGetValue((assignment.UnaryExpr as Variable).Name, out valuePair);
                    valuePair.Second = Variable.Type.Assigned;
                    values[0].Set(values[1]);
                }
                catch (ValueException)
                {
                    throw new NotSupportedException();
                }
            }
            else if (assignment.UnaryExpr is Slice)
            {
                if (values.Length != 3)
                {
                    throw new ArgumentException();
                }
                try
                {
                    values[0][values[1]].Set(values[2]);
                }
                catch (ValueException exc)
                {
                    throw exc.Error == ValueException.ErrorType.IndexOutOfRange ?
                         (Exception)(InterpretException.BuildException(exc, (assignment.UnaryExpr as Slice).Indexer.Position)) :
                         (Exception)(new NotSupportedException());
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private void InterpretBlock(Block block, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue)
        {
            if (block.IsIndependent)
            {
                variableToValue.AddDictionary();
            }
        }

        private void InterpretBlockEnd(BlockEnd blockEnd, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue)
        {
            if (blockEnd.Block.IsIndependent)
            {
                variableToValue.RemoveDictionary();
            }
        }

        private void InterpretDeclaration(Declaration declaration, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue, Value[] values)
        {
            if (declaration.Expr == null)
            {
                if (values.Length != 0)
                {
                    throw new ArgumentException();
                }
                variableToValue.Add(declaration.DeclVariable.Name,
                    new Pair<Value, Variable.Type>(ValueType.CreateValue(declaration.ValueType), Variable.Type.Unassigned));
            }
            else
            {
                if (values.Length != 1)
                {
                    throw new ArgumentException();
                }
                variableToValue.Add(declaration.DeclVariable.Name,
                    new Pair<Value, Variable.Type>(ValueType.CreateValue(declaration.ValueType).Set(values[0]), Variable.Type.Assigned));
            }
        }

        private void InterpretDoWhile(DoWhile doWhile, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue)
        {
            variableToValue.AddDictionary();
        }

        private void InterpretFor(For forStatement, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue)
        {
            variableToValue.AddDictionary();
        }

        private void InterpretIf(If ifStatement, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue)
        {
            variableToValue.AddDictionary();
        }

        private void InterpretWhile(While whileStatement, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue)
        {
            variableToValue.AddDictionary();
        }

        private void InterpretStopBreakpoint(StopBreakpoint stopBreakpoint, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue)
        {
            isBreakpointTriggered = true;
        }

        private void InterpretHitCountBreakpoint(HitCountBreakpoint hitCountBreakpoint, DictionaryList<string, Pair<Value, Variable.Type>> variableToValue)
        {

            if (!breakpointHits.ContainsKey(hitCountBreakpoint))
            {
                breakpointHits.Add(hitCountBreakpoint, 1);
            }
            else
            {
                breakpointHits[hitCountBreakpoint] = breakpointHits[hitCountBreakpoint] + 1;
            }

            if (breakpointHits[hitCountBreakpoint] == hitCountBreakpoint.HitCount)
            {
                isBreakpointTriggered = true;
            }
        }

        private void InterpretConditionBreakpoint(ConditionBreakpoint conditionBreakpoint,
            DictionaryList<string, Pair<Value, Variable.Type>> variableToValue, Value[] values)
        {
            if (values.Length != 1)
            {
                throw new ArgumentException();
            }
            isBreakpointTriggered = (values[0] as Bool).BoolValue;
        }

        private Statement InterpretFork(Fork fork, Value[] values)
        {
            if (values.Length != 1)
            {
                throw new ArgumentException();
            }

            return (values[0] as Bool).BoolValue ? fork.NextStatement : fork.FalseStatement;
        }

        private Expression[] GetExpressions(Statement statement)
        {
            if (statement is Assignment)
            {
                return GetExpressionsAssignment(statement as Assignment);
            }
            else if (statement is Declaration)
            {
                return GetExpressionsDeclaration(statement as Declaration);
            }
            else if (statement is Block)
            {
                return new Expression[] { };
            }
            else if (statement is BlockEnd)
            {
                return new Expression[] { };
            }
            else if (statement is Break)
            {
                return new Expression[] { };
            }
            else if (statement is DoWhile)
            {
                return new Expression[] { };
            }
            else if (statement is For)
            {
                return new Expression[] { };
            }
            else if (statement is Fork)
            {
                return GetExpressionsFork(statement as Fork);
            }
            else if (statement is FunctionCall)
            {
                return GetExpressionsFunctionCall(statement as FunctionCall);
            }
            else if (statement is Goto)
            {
                return new Expression[] { };
            }
            else if (statement is If)
            {
                return new Expression[] { };
            }
            else if (statement is Print)
            {
                return GetExpressionsPrint(statement as Print);
            }
            else if (statement is Return)
            {
                return GetExpressionsReturn(statement as Return);
            }
            else if (statement is While)
            {
                return new Expression[] { };
            }
            else if (statement is FunctionEnd)
            {
                return new Expression[] { };
            }
            else if (statement is EndIterationStatement)
            {
                return new Expression[] { };
            }
            else if (statement is StopBreakpoint)
            {
                return new Expression[] { };
            }
            else if (statement.GetType() == typeof(Statement))
            {
                return new Expression[] { };
            }
            else if (statement is ConditionBreakpoint)
            {
                return GetExpressionsConditionBreakpoint(statement as ConditionBreakpoint);
            }
            else if (statement is HitCountBreakpoint)
            {
                return new Expression[] { };
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private Expression[] GetExpressionsPrint(Print print)
        {
            return print.Parameters.ToArray();
        }

        private Expression[] GetExpressionsFunctionCall(FunctionCall functionCall)
        {
            return functionCall.Parameters.ToArray();
        }

        private Expression[] GetExpressionsReturn(Return returnStatement)
        {
            return new Expression[] { returnStatement.Expression };
        }

        private Expression[] GetExpressionsFork(Fork fork)
        {
            return new Expression[] { fork.Condition };
        }

        private Expression[] GetExpressionsDeclaration(Declaration declaration)
        {
            if (declaration.Expr == null)
            {
                return new Expression[] { };
            }
            else
            {
                return new Expression[] { declaration.Expr };
            }
        }

        private Expression[] GetExpressionsAssignment(Assignment assignment)
        {
            if (assignment.UnaryExpr is Variable)
            {
                return new Expression[] { assignment.UnaryExpr, assignment.Expr };
            }
            else if (assignment.UnaryExpr is Slice)
            {
                Slice slice = assignment.UnaryExpr as Slice;
                return new Expression[] { slice.Collection, slice.Indexer, assignment.Expr };
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private Expression[] GetExpressionsConditionBreakpoint(ConditionBreakpoint breakpoint)
        {
            return new Expression[] { breakpoint.Condition };
        }

        private List<KeyValuePair<string, string>> ToList(DictionaryList<string, Pair<Value, Variable.Type>> variableToValue)
        {
            List<KeyValuePair<string, string>> answer = new List<KeyValuePair<string, string>>();
            string suffix;

            for (int i = 0; i < variableToValue.Dictionaries.Count; i++)
            {
                suffix = new string('.', i);
                foreach (KeyValuePair<string, Pair<Value, Variable.Type>> pair in variableToValue.Dictionaries[i])
                {
                    answer.Add(new KeyValuePair<string, string>(suffix + pair.Key,
                        pair.Value.Second == Variable.Type.Assigned ? pair.Value.First.ToString() : ""));
                }
            }

            return answer;
        }
    }
}
