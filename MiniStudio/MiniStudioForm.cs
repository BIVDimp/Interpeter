using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.IO;
using System.Runtime.CompilerServices;
using Interfaces;
using Managers;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Serialization;

namespace MiniStudio
{
    public partial class MiniStudioForm : Form, IStorableSettings, IErrorDisplay, IWatches, IBreakpointDisplay
    {
        private const string serializePath = "UserSettings.xml";
        private const string fileFilter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
        private const string saveChangesIn = "Save changes in {0}?";
        private const string unnamed = "Unname";
        private const string positionFormat = "Line {0}, Column {1}";
        private const string defaultText = "void Main()\n{\n}";

        private static Dictionary<Keys, bool> allowedKeyData = new Dictionary<Keys, bool>()
        {
            {Keys.Control | Keys.C, true},
            {Keys.Control | Keys.A, true}
        };

        private static Dictionary<Keys, bool> allowedKeyCode = new Dictionary<Keys, bool>()
        {
            {Keys.Up, true},
            {Keys.Down, true},
            {Keys.Left, true},
            {Keys.Right, true}
        };

        private List<IError> lastErrors = new List<IError>();
        private IEnumerable<KeyValuePair<string, string>> lastVariableValues = new Dictionary<string, string>();
        private OpenFileDialog openFileDialog = new OpenFileDialog();
        private SaveFileDialog saveFileDialog = new SaveFileDialog();
        private UndoRedoManager undoRedoManager = new UndoRedoManager();
        private RichTextBoxExtension mainRtbExtension;
        private SyntaxHighlighter syntaxHighlighter;
        private Debugger debugger;
        private bool textIsSaved = true;
        private BoxPainter boxPainter;
        private BoxHighlighter boxHighlighter;
        private XmlCaretaker formCaretaker;

        public delegate void TextChangedEvent(TextChangedEventArgs args);

        public event TextChangedEvent PreviewBoxtextEdited;
        public event TextChangedEvent BoxTextEdited;
        public event TextChangedEvent BoxTextUndoRedo;

        public MiniStudioForm()
        {
            InitializeComponent();
            mainRichTextBox.ContextMenuStrip = mainContextMenuStrip;
            BoxTextEdited += new TextChangedEvent(undoRedoManager.OnTextChanged);
            mainRtbExtension = new RichTextBoxExtension(mainRichTextBox);
            boxPainter = new BoxPainter(mainRichTextBox);
            boxHighlighter = new BoxHighlighter(mainRichTextBox);
            formCaretaker = new XmlCaretaker(serializePath,
                undoRedoManager,
                mainRtbExtension,
                this);
            formCaretaker.LoadState();

            syntaxHighlighter = new SyntaxHighlighter(boxPainter);
            BoxTextEdited += syntaxHighlighter.OnTextChanged;
            BoxTextUndoRedo += syntaxHighlighter.OnTextChanged;

            debugger = new Debugger(mainRichTextBox.Text,
                new OutputWriter(outputRichTextBox),
                boxHighlighter,
                this,
                this,
                new LabelRenderer(debugLabel),
                this);

            BoxTextEdited += debugger.OnTextChanged;
            BoxTextUndoRedo += debugger.OnTextChanged;
            PreviewBoxtextEdited += debugger.OnPreviewTextChanged;

            ReinitializeComponents();
            textIsSaved = true;
        }

        private void OnMiniStudioFormClosing(object sender, FormClosingEventArgs e)
        {
            MenuEventArgs menuEventArgs = new MenuEventArgs();
            bool isSaved = TrySaveOldText(menuEventArgs);

            if (menuEventArgs.Cancel)
            {
                e.Cancel = true;
            }

            if (!isSaved)
            {
                ClearMainTextBoxComponents();
            }

            formCaretaker.SaveState();
        }

        private void ReinitializeComponents()
        {
            string temp = mainRichTextBox.Text.Replace("\r", string.Empty);
            syntaxHighlighter.Clear(temp);
        }

        private void ClearMainTextBoxComponents()
        {
            mainRtbExtension.Clear();
            undoRedoManager.Clear();
        }

        #region Main menu File event handlers

        private void OnNewFileMainMenuClick(object sender, EventArgs e)
        {
            NewFile(new MenuEventArgs());
        }

        private void OnOpenFileMainMenuClick(object sender, EventArgs e)
        {
            OpenFile(new MenuEventArgs());
        }

        private void OnSaveFileMainMenuClick(object sender, EventArgs e)
        {
            SaveFile(new MenuEventArgs());
        }

        private void OnSaveAsFileMainMenuClick(object sender, EventArgs e)
        {
            SaveAsFile(new MenuEventArgs());
        }

        private void OnExitFileMainMenuClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void NewFile(MenuEventArgs e)
        {
            if (e.Cancel)
            {
                return;
            }

            TrySaveOldText(e);

            if (e.Cancel)
            {
                return;
            }

            ClearMainTextBoxComponents();
            ClearPreviousDataFromTabs();
            ReinitializeComponents();
            textIsSaved = false;
        }

        private void OpenFile(MenuEventArgs e)
        {
            if (e.Cancel)
            {
                return;
            }

            TrySaveOldText(e);

            if (e.Cancel)
            {
                return;
            }

            openFileDialog.Filter = fileFilter;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                e.Cancel = true;
                return;
            }

            using (StreamReader codeReader = new StreamReader(openFileDialog.FileName))
            {
                mainRichTextBox.LoadFile(codeReader.BaseStream, RichTextBoxStreamType.PlainText);
            }

            mainRtbExtension.FilePath = openFileDialog.FileName;
            ClearPreviousDataFromTabs();
            ReinitializeComponents();
            textIsSaved = true;
        }

        private void SaveFile(MenuEventArgs e)
        {
            if (e.Cancel || textIsSaved)
            {
                return;
            }

            if (mainRtbExtension.FilePath == null)
            {
                SaveAsFile(e);
                return;
            }

            SaveText(mainRtbExtension.FilePath);
        }

        private void SaveAsFile(MenuEventArgs e)
        {
            if (e.Cancel)
            {
                return;
            }

            saveFileDialog.Filter = fileFilter;
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            SaveText(saveFileDialog.FileName);
        }

        private bool TrySaveOldText(MenuEventArgs e)
        {
            if (e.Cancel)
            {
                return false;
            }

            if (textIsSaved)
            {
                return true;
            }

            DialogResult result;
            result = MessageBox.Show(
                string.Format(saveChangesIn, mainRtbExtension.FilePath ?? unnamed),
                string.Empty, MessageBoxButtons.YesNoCancel
            );

            switch (result)
            {
                case DialogResult.Yes:
                    SaveFile(e);
                    return true;
                case DialogResult.No:
                    return false;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    return false;
                default:
                    throw new NotSupportedException();
            }
        }

        private void SaveText(string path)
        {
            using (StreamWriter codeWriter = new StreamWriter(path))
            {
                mainRichTextBox.SaveFile(codeWriter.BaseStream, RichTextBoxStreamType.PlainText);
            }
            textIsSaved = true;
            mainRtbExtension.FilePath = path;
        }

        #endregion

        #region Main menu Build event handlers

        private void OnBuildBuildMainMenuClick(object sender, EventArgs e)
        {
            mainTabControl.SelectedTab = outputTab;
            debugger.BuildSolution();
            mainRichTextBox.Focus();
        }

        private void OnRunBuildMainMenuClick(object sender, EventArgs e)
        {
            mainTabControl.SelectedTab = outputTab;
            debugger.RunSolution();
            mainRichTextBox.Focus();
        }

        #endregion

        #region Main menu Debug event handlers

        private void OnStepIntoDebugMenuClick(object sender, EventArgs e)
        {
            debugger.MakeStepInto();
            mainRichTextBox.Focus();
        }

        private void OnStepOverDebugMenuClick(object sender, EventArgs e)
        {
            debugger.MakeStepOver();
            mainRichTextBox.Focus();
        }

        private void OnStartDebuggingDebugMenuClick(object sender, EventArgs e)
        {
            debugger.Debug();
            mainRichTextBox.Focus();
        }

        private void OnNewBreakpointDebugMenuClick(object sender, EventArgs e)
        {
            RunPutBreakpoint();
        }

        private void OnRunToCursorDebugMenuClick(object sender, EventArgs e)
        {
            debugger.RunToCursor(mainRichTextBox.SelectionStart);
            mainRichTextBox.Focus();
        }

        #endregion

        private void OnMainRichTextBoxTextChanged(object sender, EventArgs e)
        {
            textIsSaved = false;
        }

        private void OnMainRichTextBoxPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
        }

        private void OnMainRichTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            HandleShortcuts(e);
        }

        private void OnMainRichTextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            char symbol = e.KeyChar;
            if (symbol == '\r')
            {
                e.Handled = true;
                return;
            }

            if (!char.IsLetterOrDigit(symbol) &&
                !char.IsPunctuation(symbol) &&
                !char.IsSeparator(symbol) &&
                !char.IsSymbol(symbol) &&
                !char.IsWhiteSpace(symbol))
            {
                e.Handled = true;
                return;
            }
            TextChangedEventArgs textChangedArgs;
            if (mainRichTextBox.SelectionLength == 0)
            {
                textChangedArgs = TextChangedEventArgs.CreateTextWrittenEventArgs(symbol, mainRichTextBox.SelectionStart);
            }
            else
            {
                textChangedArgs = TextChangedEventArgs.CreateTextWrittenEventArgs(
                    mainRichTextBox.SelectedText,
                    symbol,
                    mainRichTextBox.SelectionStart);
            }

            if (PreviewBoxtextEdited != null)
            {
                PreviewBoxtextEdited(textChangedArgs);
            }

            MainRichTextBoxCorrectText(textChangedArgs);
            e.Handled = true;

            if (BoxTextEdited != null)
            {
                BoxTextEdited(textChangedArgs);
            }
        }

        private void OnErrorsDataGridViewSelectionChanged(object sender, EventArgs e)
        {
            SelectErrors();
        }

        private void OnErrorsDataGridViewCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectErrors();
            mainRichTextBox.Focus();
        }

        private void HandleShortcuts(KeyEventArgs e)
        {
            if (allowedKeyCode.ContainsKey(e.KeyCode) || allowedKeyData.ContainsKey(e.KeyData))
            {
                return;
            }
            e.Handled = true;

            if (IsPaste(e))
            {
                RunPaste();
            }
            else if (IsUndo(e))
            {
                RunUndo();
            }
            else if (IsRedo(e))
            {
                RunRedo();
            }
            else if (IsBackPress(e))
            {
                RunBack();
            }
            else if (IsDeletePress(e))
            {
                RunDelete();
            }
            else if (IsEnterPress(e))
            {
                RunEnter();
            }
            else if (IsCut(e))
            {
                RunCut();
            }
            else if (IsStopDebug(e))
            {
                RunStopDebug();
            }
            else if (IsPutBreakpoint(e))
            {
                RunPutBreakpoint();
            }
            else if (IsRunToCursor(e))
            {
                StartRunToCursor();
            }
        }

        #region Shortcuts handlers

        private bool IsRunToCursor(KeyEventArgs e)
        {
            return e.KeyData == (Keys.Control | Keys.F10);
        }

        private void StartRunToCursor()
        {
            debugger.RunToCursor(mainRichTextBox.SelectionStart);
        }

        private bool IsPutBreakpoint(KeyEventArgs e)
        {
            return e.KeyData == Keys.F9;
        }

        private void RunPutBreakpoint()
        {
            if (debugger.BreakpointExists(mainRichTextBox.SelectionStart))
            {
                debugger.RemoveBreakpoint(mainRichTextBox.SelectionStart);
            }
            else
            {
                debugger.PutBreakpoint(mainRichTextBox.SelectionStart);
            }
        }

        private bool IsStopDebug(KeyEventArgs e)
        {
            return e.KeyData == (Keys.Shift | Keys.F5);
        }

        private void RunStopDebug()
        {
            debugger.StopDebug();
        }

        private bool IsPaste(KeyEventArgs e)
        {
            return e.KeyData == (Keys.Control | Keys.V);
        }

        private void RunPaste()
        {
            if (!Clipboard.ContainsText())
            {
                return;
            }

            TextChangedEventArgs textChangedArgs = TextChangedEventArgs.CreateTextReplacedEventArgs(
                mainRichTextBox.SelectedText,
                Clipboard.GetText().Replace("\r", string.Empty),
                mainRichTextBox.SelectionStart);

            if (PreviewBoxtextEdited != null)
            {
                PreviewBoxtextEdited(textChangedArgs);
            }

            MainRichTextBoxCorrectText(textChangedArgs);

            if (BoxTextEdited != null)
            {
                BoxTextEdited(textChangedArgs);
            }
        }

        private bool IsUndo(KeyEventArgs e)
        {
            return e.KeyData == (Keys.Control | Keys.Z);
        }

        private void RunUndo()
        {
            TextChangedEventArgs textChangedArgs = undoRedoManager.Undo();

            if (PreviewBoxtextEdited != null)
            {
                PreviewBoxtextEdited(textChangedArgs);
            }

            MainRichTextBoxCorrectText(textChangedArgs);

            if (BoxTextUndoRedo != null)
            {
                BoxTextUndoRedo(textChangedArgs);
            }
        }

        private bool IsRedo(KeyEventArgs e)
        {
            return e.KeyData == (Keys.Control | Keys.Y);
        }

        private void RunRedo()
        {
            TextChangedEventArgs textChangedArgs = undoRedoManager.Redo();

            if (PreviewBoxtextEdited != null)
            {
                PreviewBoxtextEdited(textChangedArgs);
            }

            MainRichTextBoxCorrectText(textChangedArgs);

            if (BoxTextUndoRedo != null)
            {
                BoxTextUndoRedo(textChangedArgs);
            }
        }

        private bool IsBackPress(KeyEventArgs e)
        {
            return e.KeyData == Keys.Back;
        }

        private void RunBack()
        {
            TextChangedEventArgs textChangedArgs;

            if (mainRichTextBox.SelectionLength == 0)
            {
                if (mainRichTextBox.SelectionStart < 1)
                {
                    return;
                }
                textChangedArgs = TextChangedEventArgs.CreateTextReplacedEventArgs(
                    mainRichTextBox.Text[mainRichTextBox.SelectionStart - 1].ToString(),
                    string.Empty,
                    mainRichTextBox.SelectionStart - 1);
            }
            else
            {
                textChangedArgs = TextChangedEventArgs.CreateTextReplacedEventArgs(
                    mainRichTextBox.SelectedText,
                    string.Empty,
                    mainRichTextBox.SelectionStart);
            }

            if (PreviewBoxtextEdited != null)
            {
                PreviewBoxtextEdited(textChangedArgs);
            }

            MainRichTextBoxCorrectText(textChangedArgs);

            if (BoxTextEdited != null)
            {
                BoxTextEdited(textChangedArgs);
            }
        }

        private bool IsDeletePress(KeyEventArgs e)
        {
            return e.KeyData == Keys.Delete;
        }

        private void RunDelete()
        {
            TextChangedEventArgs textChangedArgs;

            if (mainRichTextBox.SelectionLength == 0)
            {
                if (mainRichTextBox.SelectionStart >= mainRichTextBox.Text.Length)
                {
                    return;
                }
                textChangedArgs = TextChangedEventArgs.CreateTextReplacedEventArgs(
                    mainRichTextBox.Text[mainRichTextBox.SelectionStart].ToString(),
                    string.Empty,
                    mainRichTextBox.SelectionStart);
            }
            else
            {
                textChangedArgs = TextChangedEventArgs.CreateTextReplacedEventArgs(
                    mainRichTextBox.SelectedText,
                    string.Empty,
                    mainRichTextBox.SelectionStart);
            }

            if (PreviewBoxtextEdited != null)
            {
                PreviewBoxtextEdited(textChangedArgs);
            }

            MainRichTextBoxCorrectText(textChangedArgs);

            if (BoxTextEdited != null)
            {
                BoxTextEdited(textChangedArgs);
            }
        }

        private bool IsEnterPress(KeyEventArgs e)
        {
            return e.KeyData == Keys.Enter;
        }

        private void RunEnter()
        {
            TextChangedEventArgs textChangedArgs;

            textChangedArgs = TextChangedEventArgs.CreateTextWrittenEventArgs(
                mainRichTextBox.SelectedText,
                '\n',
                mainRichTextBox.SelectionStart);

            if (PreviewBoxtextEdited != null)
            {
                PreviewBoxtextEdited(textChangedArgs);
            }

            MainRichTextBoxCorrectText(textChangedArgs);

            if (BoxTextEdited != null)
            {
                BoxTextEdited(textChangedArgs);
            }
        }

        private bool IsCut(KeyEventArgs e)
        {
            return e.KeyData == (Keys.Control | Keys.X);
        }

        private void RunCut()
        {
            if (string.IsNullOrEmpty(mainRichTextBox.SelectedText))
            {
                return;
            }
            Clipboard.SetText(mainRichTextBox.SelectedText);

            TextChangedEventArgs textChangedArgs = TextChangedEventArgs.CreateTextReplacedEventArgs(
                mainRichTextBox.SelectedText,
                string.Empty,
                mainRichTextBox.SelectionStart);

            if (PreviewBoxtextEdited != null)
            {
                PreviewBoxtextEdited(textChangedArgs);
            }

            MainRichTextBoxCorrectText(textChangedArgs);

            if (BoxTextEdited != null)
            {
                BoxTextEdited(textChangedArgs);
            }
        }

        #endregion

        private void MainRichTextBoxCorrectText(TextChangedEventArgs args)
        {
            if (args.Type == TextChangedEventArgs.EventType.None)
            {
                return;
            }

            mainRichTextBox.SelectionStart = args.OldStartPosition;
            mainRichTextBox.SelectionLength = args.OldText.Length;
            mainRichTextBox.SelectedText = string.Empty;
            mainRichTextBox.SelectionStart = args.NewStartPosition;
            mainRichTextBox.SelectionLength = 0;
            mainRichTextBox.SelectedText = args.NewText;
            mainRichTextBox.SelectionStart = mainRichTextBox.SelectionStart + mainRichTextBox.SelectionLength;
            mainRichTextBox.SelectionLength = 0;
        }

        private void ClearErrorTab()
        {
            lastErrors.Clear();
            errorsDataGridView.Rows.Clear();
        }

        private void ClearResultTab()
        {
            lastVariableValues = new Dictionary<string, string>();
            watchesDataGridView.Rows.Clear();
        }

        public void AddError(IError error)
        {
            mainTabControl.SelectedTab = errorTab;
            lastErrors.Add(error);
            errorsDataGridView.Rows.Add(
                string.Format(positionFormat, error.GetLine() + 1, error.GetColumn() + 1),
                error.GetMessage());
            errorsDataGridView.SelectAll();
        }

        public void ClearErrors()
        {
            errorsDataGridView.Rows.Clear();
            lastErrors.Clear();
        }

        private void ClearPreviousDataFromTabs()
        {
            ClearErrorTab();
            ClearResultTab();
            ClearOutputRichTextBox();
        }

        private void ClearOutputRichTextBox()
        {
            outputRichTextBox.Clear();
        }

        private void SelectErrors()
        {
            for (int i = 0; i < errorsDataGridView.RowCount; i++)
            {
                if (!errorsDataGridView.Rows[i].Selected)
                {
                    continue;
                }
                SelectError(lastErrors[i]);
            }
        }

        private void SelectError(IError error)
        {
            mainRichTextBox.Select(error.GetStart(), error.GetLength());
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowCaret(IntPtr hWnd);

        private void OnMainRichTextBoxMouseUp(object sender, MouseEventArgs e)
        {
            CreateCaret(mainRichTextBox.Handle, IntPtr.Zero, 1, mainRichTextBox.Font.Height);
            ShowCaret(mainRichTextBox.Handle);
        }

        private void OnMainRichTextBoxSelectionChanged(object sender, EventArgs e)
        {
            CreateCaret(mainRichTextBox.Handle, IntPtr.Zero, 1, mainRichTextBox.Font.Height);
            ShowCaret(mainRichTextBox.Handle);
        }

        public IMemento GetMemento()
        {
            SuspendLayout();
            FormWindowState currentWindowState = WindowState;
            WindowState = FormWindowState.Normal;
            Point normalLocation = Location;
            Size normalSize = Size;
            WindowState = currentWindowState;
            this.ResumeLayout(false);
            this.PerformLayout();

            return new MainFormMemento(normalLocation, normalSize, WindowState, FormBorderStyle);
        }

        public bool SetMemento(IMemento memento)
        {
            if (memento == null || !(memento is MainFormMemento))
            {
                return false;
            }

            MainFormMemento formMemento = memento as MainFormMemento;

            Location = formMemento.Location;
            Size = formMemento.Size;
            WindowState = formMemento.FormWindowState;
            FormBorderStyle = formMemento.FormBoarderStyle;

            return true;
        }

        public void Clear()
        {
        }

        public Type GetMementoType()
        {
            return typeof(MainFormMemento);
        }

        public void AddResult(string name, string value)
        {
            (lastVariableValues as Dictionary<string, string>).Add(name, value);
            watchesDataGridView.Rows.Add(name, value);
            watchesDataGridView.ClearSelection();
            if (mainTabControl.SelectedTab == errorTab)
            {
                mainTabControl.SelectedTab = outputTab;
            }
        }

        public void ClearResults()
        {
            lastVariableValues = new Dictionary<string, string>();
            watchesDataGridView.Rows.Clear();
        }

        public void Display(IBreakpoint breakpoint, string information)
        {
            mainTabControl.SelectedTab = breakpointsTab;
            breakpointDataGridView.Rows.Add(breakpoint.GetStart(), breakpoint.GetLength(),
                breakpoint.GetBreakpointType(), information);
            mainRichTextBox.Focus();
        }

        public void Delete(IBreakpoint breakpoint)
        {
            for (int i = 0; i < breakpointDataGridView.RowCount; i++)
            {
                if ((int)breakpointDataGridView[0, i].Value == breakpoint.GetStart())
                {
                    breakpointDataGridView.Rows.RemoveAt(i);
                    break;
                }
            }
        }

        public void ClearBreakpoints()
        {
            breakpointDataGridView.Rows.Clear();
        }

        private void OnBreakpointDataGridViewCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(() =>
              {
                  debugger.ChangeBreakpointType((int)breakpointDataGridView[0, e.RowIndex].Value,
                      (string)breakpointDataGridView[e.ColumnIndex, e.RowIndex].Value ?? string.Empty);
              }));
        }

        private void OnBreakpointDataGridViewCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                return;
            }
            for (int i = 0; i < breakpointDataGridView.RowCount; i++)
            {
                if (breakpointDataGridView.Rows[i].Selected)
                {
                    mainRichTextBox.Select((int)breakpointDataGridView[0, i].Value,
                        (int)breakpointDataGridView[1, i].Value);
                    mainRichTextBox.Focus();
                    break;
                }
            }
        }

        private void OnRunToCursorMainToolStripMenuItemClick(object sender, EventArgs e)
        {
            StartRunToCursor();
        }

        private void OnPutBreakpointMainToolStripMenuItemClick(object sender, EventArgs e)
        {
            RunPutBreakpoint();
        }
    }
}