using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyInterpreter;
using MyInterpreter.AST;
using Interfaces;

namespace Managers
{
    public enum DebuggerMode
    {
        On,
        Off
    }

    public sealed class Debugger
    {
        public DebuggerMode Mode
        {
            get
            {
                return mode;
            }
            private set
            {
                switch (value)
                {
                    case DebuggerMode.On:
                        labelRenderer.ChangeText("Debug: On");
                        break;
                    case DebuggerMode.Off:
                        labelRenderer.ChangeText("Debug: Off");
                        break;
                    default:
                        throw new NotSupportedException();
                }
                mode = value;
            }
        }

        private string text;
        private List<Statement> statementList;
        private Interpreter interpreter;
        private List<KeyValuePair<Breakpoint, string>> breakpointList = new List<KeyValuePair<Breakpoint, string>>();

        private DebuggerMode mode;
        private readonly IWriter writer;
        private readonly IStatementHighlighter highlighter;
        private readonly IErrorDisplay errorDisplay;
        private readonly IWatches watches;
        private readonly ILabelRenderer labelRenderer;
        private readonly IBreakpointDisplay breakpointDisplay;

        public Debugger(string text, IWriter writer, IStatementHighlighter highlighter, IErrorDisplay errorDisplay,
            IWatches watches, ILabelRenderer labelRenderer, IBreakpointDisplay breakpointDisplay)
        {
            if (text == null || writer == null || highlighter == null || errorDisplay == null ||
                watches == null || labelRenderer == null || breakpointDisplay == null)
            {
                throw new ArgumentNullException();
            }
            this.text = text;
            Parser parser = new Parser(text);
            Program program = parser.ParseProgram();
            statementList = program.StatementList;

            this.writer = writer;
            this.highlighter = highlighter;
            this.errorDisplay = errorDisplay;
            this.watches = watches;
            this.labelRenderer = labelRenderer;
            this.breakpointDisplay = breakpointDisplay;
            Mode = DebuggerMode.Off;
        }

        public void MakeStepOver()
        {
            if (Mode == DebuggerMode.Off)
            {
                StartDebug();
                return;
            }

            TurnOffCurrentStatement();
            BaseInterpreterStatus status = interpreter.MakeStepOver();
            HandleStatus(status);
        }

        public void MakeStepInto()
        {
            if (Mode == DebuggerMode.Off)
            {
                StartDebug();
                return;
            }

            TurnOffCurrentStatement();
            BaseInterpreterStatus status = interpreter.MakeStepInto();
            HandleStatus(status);
        }

        public void RunToCursor(int cursor)
        {
            if (Mode == DebuggerMode.Off)
            {
                StartDebug();
                if (Mode == DebuggerMode.Off)
                {
                    return;
                }
                if (PointInPosition(cursor, interpreter.CurrentStatement.Position))
                {
                    return;
                }
            }

            TurnOffCurrentStatement();
            BaseInterpreterStatus status = interpreter.RunToCursor(cursor);
            HandleStatus(status);
        }

        public void Debug()
        {
            if (Mode == DebuggerMode.Off)
            {
                StartDebug();
                if (Mode == DebuggerMode.Off)
                {
                    return;
                }
            }

            TurnOffCurrentStatement();
            BaseInterpreterStatus status = interpreter.Debug();
            HandleStatus(status);
        }

        public void BuildSolution()
        {
            if (Mode == DebuggerMode.On)
            {
                return;
            }

            writer.ClearText();
            ReinitializeComponents();
            BaseInterpreterStatus status = interpreter.BuildSolution();
            HandleStatus(status);
        }

        public void RunSolution()
        {
            if (Mode == DebuggerMode.On)
            {
                return;
            }

            writer.ClearText();
            ReinitializeComponents();
            BaseInterpreterStatus status = interpreter.RunSolution();
            HandleStatus(status);
        }

        private void StartDebug()
        {
            writer.ClearText();
            ClearPreviousData();

            switch (Mode)
            {
                case DebuggerMode.On:
                    {
                        break;
                    }
                case DebuggerMode.Off:
                    {
                        ReinitializeComponents();
                        BaseInterpreterStatus status = interpreter.StartDebbuging();
                        HandleStatus(status);
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }

        public void StopDebug()
        {
            if (Mode == DebuggerMode.Off)
            {
                return;
            }

            Mode = DebuggerMode.Off;
            TurnOffCurrentStatement();
            interpreter.StopDebbuging();
            ClearPreviousData();
        }

        private void ReinitializeComponents()
        {
            interpreter = new Interpreter(text, writer);
            statementList = interpreter.Program.StatementList;
            DottBreakpoint();
        }

        private void HandleStatus(BaseInterpreterStatus status)
        {
            ClearPreviousData();
            if (status is HaveErrorsStatus)
            {
                Mode = DebuggerMode.Off;
                ShowErrors((status as HaveErrorsStatus).ErrorList);
            }
            else if (status is EndDebugStatus)
            {
                Mode = DebuggerMode.Off;
            }
            else if (status is DebuggingProcessStatus)
            {
                Mode = DebuggerMode.On;
                Highlight((status as DebuggingProcessStatus).CurrentStatement);
                DisplayVariables((status as DebuggingProcessStatus).Variables);
            }
            else if (status is CorrectBuildingStatus)
            {
                Mode = DebuggerMode.Off;
            }
            else if (status is CorrectRunningStatus)
            {
                Mode = DebuggerMode.Off;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private void ClearPreviousData()
        {
            errorDisplay.ClearErrors();
            watches.ClearResults();
        }

        private void ShowErrors(List<IError> errorList)
        {
            errorDisplay.ClearErrors();

            errorList.Sort(delegate(IError x, IError y)
            {
                return x.GetStart().CompareTo(y.GetStart());
            });

            foreach (var error in errorList)
            {
                errorDisplay.AddError(error);
            }
        }

        private void Highlight(Statement statement)
        {
            if (statement == null)
            {
                throw new NotSupportedException();
            }

            if (Mode == DebuggerMode.On && statement == interpreter.CurrentStatement)
            {
                if (statement.Breakpoint != null)
                {
                    highlighter.HighlightText(HighlightType.CurrentBreakpoint,
                        statement.Position.BeginIndex, statement.Position.Length);
                }
                else
                {
                    highlighter.HighlightText(HighlightType.CurrentStatement,
                        statement.Position.BeginIndex, statement.Position.Length);
                }
            }
            else
            {
                if (statement.Breakpoint != null)
                {
                    highlighter.HighlightText(HighlightType.Breakpoint,
                        statement.Position.BeginIndex, statement.Position.Length);
                }
                else
                {
                    highlighter.HighlightText(HighlightType.None,
                        statement.Position.BeginIndex, statement.Position.Length);
                }
            }
        }

        private void DisplayVariables(List<KeyValuePair<string, string>> watchesList)
        {
            watches.ClearResults();

            foreach (var watch in watchesList)
            {
                watches.AddResult(watch.Key, watch.Value);
            }
        }

        private void TurnOffCurrentStatement()
        {
            Statement statement = interpreter.CurrentStatement;

            if (statement == null)
            {
                return;
            }
            if (statement.Breakpoint != null)
            {
                highlighter.HighlightText(HighlightType.Breakpoint,
                    statement.Position.BeginIndex, statement.Position.Length);
            }
            else
            {
                highlighter.HighlightText(HighlightType.None,
                    statement.Position.BeginIndex, statement.Position.Length);
            }
        }

        private void DottBreakpoint()
        {
            SwitchOffBreakpoints();

            foreach (var breakpoint in breakpointList)
            {
                foreach (var statement in interpreter.Program.StatementList)
                {
                    if (statement.Position.BeginIndex == breakpoint.Key.Position.BeginIndex
                        && statement.Position.Length == breakpoint.Key.Position.Length)
                    {
                        statement.Breakpoint = breakpoint.Key;
                        Highlight(statement);
                        break;
                    }
                }
            }
        }

        private void SwitchOffBreakpoints()
        {
            foreach (var breakpoint in breakpointList)
            {
                highlighter.HighlightText(HighlightType.None, breakpoint.Key.Position.BeginIndex, breakpoint.Key.Position.Length);
            }
        }

        private void SwitchOnBreakpoints()
        {
            foreach (var breakpoint in breakpointList)
            {
                highlighter.HighlightText(HighlightType.Breakpoint, breakpoint.Key.Position.BeginIndex, breakpoint.Key.Position.Length);
            }
        }

        public bool PutBreakpoint(int position)
        {
            int index = GetIndexStatementOnPosition(position);
            if (index == -1 || statementList[index].Breakpoint != null)
            {
                return false;
            }
            Breakpoint newBreakpoint = new StopBreakpoint(statementList[index].Position);
            statementList[index].Breakpoint = newBreakpoint;
            breakpointList.Add(new KeyValuePair<Breakpoint, string>(newBreakpoint, string.Empty));

            breakpointDisplay.Display(newBreakpoint, breakpointList.Last().Value);
            Highlight(statementList[index]);
            return true;
        }

        public bool BreakpointExists(int position)
        {
            int index = GetIndexStatementOnPosition(position);
            return index != -1 && statementList[index].Breakpoint != null;
        }

        private int GetIndexStatementOnPosition(int position)
        {
            if (statementList == null)
            {
                return -1;
            }
            int i = 0, indent;

            for (; i < statementList.Count; i++)
            {
                indent = position - statementList[i].Position.BeginIndex;
                if (indent < 0)
                {
                    continue;
                }
                if (indent < statementList[i].Position.Length)
                {
                    return i;
                }
                if (indent > statementList[i].Position.Length + 1)
                {
                    continue;
                }
                if (i < statementList.Count - 1)
                {
                    if (position >= statementList[i + 1].Position.BeginIndex)
                    {
                        continue;
                    }
                    else
                    {
                        return i;
                    }
                }
                if (indent == statementList[i].Position.Length)
                {
                    return i;
                }
            }

            return -1;
        }

        public void ChangeBreakpointType(int startPosition, string conditionString)
        {
            Parser parser = new Parser(conditionString);
            Condition condition = parser.ParseCondition();
            int indexStatement = GetIndexStatementOnPosition(startPosition);
            int indexPoint;
            int value;
            for (indexPoint = 0; indexPoint < breakpointList.Count; indexPoint++)
            {
                if (breakpointList[indexPoint].Key == statementList[indexStatement].Breakpoint)
                {
                    break;
                }
            }

            if (condition != null)
            {
                condition.Position = new Position();
                breakpointDisplay.Delete(breakpointList[indexPoint].Key);
                breakpointList.RemoveAt(indexPoint);
                breakpointList.Add(new KeyValuePair<Breakpoint, string>(
                    new ConditionBreakpoint(condition, statementList[indexStatement].Position), conditionString));
                statementList[indexStatement].Breakpoint = breakpointList.Last().Key;
                breakpointDisplay.Display(breakpointList.Last().Key, breakpointList.Last().Value);
            }
            else if (int.TryParse(conditionString, out value) && value > 0)
            {
                if (breakpointList[indexPoint].Key is HitCountBreakpoint)
                {
                    (breakpointList[indexPoint].Key as HitCountBreakpoint).ChangeHitCount(value);
                    breakpointList[indexPoint] = new KeyValuePair<Breakpoint, string>(breakpointList[indexPoint].Key, conditionString);
                    breakpointDisplay.Delete(breakpointList[indexPoint].Key);
                    breakpointDisplay.Display(breakpointList[indexPoint].Key, breakpointList[indexPoint].Value);
                    return;
                }
                breakpointDisplay.Delete(breakpointList[indexPoint].Key);
                breakpointList.RemoveAt(indexPoint);
                breakpointList.Add(new KeyValuePair<Breakpoint, string>(
                    new HitCountBreakpoint(value, statementList[indexStatement].Position), conditionString));
                statementList[indexStatement].Breakpoint = breakpointList.Last().Key;
                breakpointDisplay.Display(breakpointList.Last().Key, breakpointList.Last().Value);
            }
            else
            {
                breakpointDisplay.Delete(breakpointList[indexPoint].Key);
                breakpointList.RemoveAt(indexPoint);
                breakpointList.Add(new KeyValuePair<Breakpoint, string>(
                    new StopBreakpoint(statementList[indexStatement].Position), string.Empty));
                statementList[indexStatement].Breakpoint = breakpointList.Last().Key;
                breakpointDisplay.Display(breakpointList.Last().Key, breakpointList.Last().Value);
            }
        }

        public bool RemoveBreakpoint(int position)
        {
            int index = GetIndexStatementOnPosition(position);
            if (index == -1 || statementList[index].Breakpoint == null)
            {
                return false;
            }
            breakpointDisplay.Delete(statementList[index].Breakpoint);
            breakpointList.RemoveAll(pair => pair.Key == statementList[index].Breakpoint);
            statementList[index].Breakpoint = null;
            Highlight(statementList[index]);
            return true;
        }

        private bool PointInPosition(int point, Position position)
        {
            return point >= position.BeginIndex && point <= position.BeginIndex + position.Length;
        }

        public void OnPreviewTextChanged(TextChangedEventArgs currentEvent)
        {
            switch (currentEvent.Type)
            {
                case TextChangedEventArgs.EventType.None:
                    break;
                case TextChangedEventArgs.EventType.TextWritten:
                case TextChangedEventArgs.EventType.TextReplaced:
                    SwitchOffBreakpoints();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public void OnTextChanged(TextChangedEventArgs currentEvent)
        {
            StopDebug();

            switch (currentEvent.Type)
            {
                case TextChangedEventArgs.EventType.None:
                    break;
                case TextChangedEventArgs.EventType.TextWritten:
                case TextChangedEventArgs.EventType.TextReplaced:
                    TextReplasedHandler(currentEvent);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        //Handler for TextWritten, TextReplased
        private void TextReplasedHandler(TextChangedEventArgs currentEvent)
        {
            string newText = text.Remove(currentEvent.OldStartPosition, currentEvent.OldText.Length);
            newText = newText.Insert(currentEvent.NewStartPosition, currentEvent.NewText);
            Parser parser = new Parser(new Lexer(newText));
            Program program = parser.ParseProgram();
            text = newText;
            statementList = program.StatementList;
            breakpointDisplay.ClearBreakpoints();

            List<KeyValuePair<Breakpoint, string>> newBreakpointList = new List<KeyValuePair<Breakpoint, string>>();
            for (int i = 0, index; i < breakpointList.Count; i++)
            {
                Position position = breakpointList[i].Key.Position;

                if (position.BeginIndex <= currentEvent.OldStartPosition)
                {
                    index = GetIndexStatementOnPosition(position.BeginIndex);
                    if (index != -1 && statementList[index].Breakpoint == null)
                    {
                        breakpointList[i].Key.ChangePosition(statementList[index].Position);
                        newBreakpointList.Add(breakpointList[i]);
                        breakpointDisplay.Display(newBreakpointList.Last().Key, newBreakpointList.Last().Value);
                        statementList[index].Breakpoint = breakpointList[i].Key;
                    }
                    continue;
                }
                if (position.BeginIndex + position.Length >= currentEvent.OldStartPosition + currentEvent.OldText.Length)
                {
                    int pos = Math.Max(currentEvent.OldStartPosition + currentEvent.NewText.Length,
                        position.BeginIndex - currentEvent.OldText.Length + currentEvent.NewText.Length);
                    index = GetIndexStatementOnPosition(pos);
                    if (index != -1 && statementList[index].Breakpoint == null)
                    {
                        breakpointList[i].Key.ChangePosition(statementList[index].Position);
                        newBreakpointList.Add(breakpointList[i]);
                        breakpointDisplay.Display(newBreakpointList.Last().Key, newBreakpointList.Last().Value);
                        statementList[index].Breakpoint = breakpointList[i].Key;
                    }
                    continue;
                }
            }

            breakpointList = newBreakpointList;
            SwitchOnBreakpoints();
        }
    }
}
