namespace MiniStudio
{
    partial class MiniStudioForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MiniStudioForm));
            this.textTabSplitContainer = new System.Windows.Forms.SplitContainer();
            this.mainRichTextBox = new System.Windows.Forms.RichTextBox();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.watchesTab = new System.Windows.Forms.TabPage();
            this.watchesDataGridView = new System.Windows.Forms.DataGridView();
            this.variableColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorTab = new System.Windows.Forms.TabPage();
            this.errorsDataGridView = new System.Windows.Forms.DataGridView();
            this.positionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.messageColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.outputTab = new System.Windows.Forms.TabPage();
            this.outputRichTextBox = new System.Windows.Forms.RichTextBox();
            this.breakpointsTab = new System.Windows.Forms.TabPage();
            this.breakpointDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Information = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepOverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepIntoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startDebuggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newBreakpointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToCursorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugLabel = new System.Windows.Forms.Label();
            this.mainContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.runToCursorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.putBreakpointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.textTabSplitContainer)).BeginInit();
            this.textTabSplitContainer.Panel1.SuspendLayout();
            this.textTabSplitContainer.Panel2.SuspendLayout();
            this.textTabSplitContainer.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            this.watchesTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.watchesDataGridView)).BeginInit();
            this.errorTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorsDataGridView)).BeginInit();
            this.outputTab.SuspendLayout();
            this.breakpointsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.breakpointDataGridView)).BeginInit();
            this.mainMenuStrip.SuspendLayout();
            this.mainContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // textTabSplitContainer
            // 
            this.textTabSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textTabSplitContainer.Location = new System.Drawing.Point(0, 24);
            this.textTabSplitContainer.Name = "textTabSplitContainer";
            this.textTabSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // textTabSplitContainer.Panel1
            // 
            this.textTabSplitContainer.Panel1.Controls.Add(this.mainRichTextBox);
            // 
            // textTabSplitContainer.Panel2
            // 
            this.textTabSplitContainer.Panel2.Controls.Add(this.mainTabControl);
            this.textTabSplitContainer.Size = new System.Drawing.Size(697, 362);
            this.textTabSplitContainer.SplitterDistance = 231;
            this.textTabSplitContainer.TabIndex = 1;
            // 
            // mainRichTextBox
            // 
            this.mainRichTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.mainRichTextBox.DetectUrls = false;
            this.mainRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainRichTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.mainRichTextBox.Location = new System.Drawing.Point(0, 0);
            this.mainRichTextBox.Name = "mainRichTextBox";
            this.mainRichTextBox.Size = new System.Drawing.Size(697, 231);
            this.mainRichTextBox.TabIndex = 0;
            this.mainRichTextBox.Text = "";
            this.mainRichTextBox.SelectionChanged += new System.EventHandler(this.OnMainRichTextBoxSelectionChanged);
            this.mainRichTextBox.TextChanged += new System.EventHandler(this.OnMainRichTextBoxTextChanged);
            this.mainRichTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnMainRichTextBoxKeyDown);
            this.mainRichTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnMainRichTextBoxKeyPress);
            this.mainRichTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMainRichTextBoxMouseUp);
            this.mainRichTextBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.OnMainRichTextBoxPreviewKeyDown);
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.watchesTab);
            this.mainTabControl.Controls.Add(this.errorTab);
            this.mainTabControl.Controls.Add(this.outputTab);
            this.mainTabControl.Controls.Add(this.breakpointsTab);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 0);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(697, 127);
            this.mainTabControl.TabIndex = 0;
            // 
            // watchesTab
            // 
            this.watchesTab.Controls.Add(this.watchesDataGridView);
            this.watchesTab.Location = new System.Drawing.Point(4, 22);
            this.watchesTab.Name = "watchesTab";
            this.watchesTab.Padding = new System.Windows.Forms.Padding(3);
            this.watchesTab.Size = new System.Drawing.Size(689, 101);
            this.watchesTab.TabIndex = 1;
            this.watchesTab.Text = "Watches";
            this.watchesTab.UseVisualStyleBackColor = true;
            // 
            // watchesDataGridView
            // 
            this.watchesDataGridView.AllowUserToAddRows = false;
            this.watchesDataGridView.AllowUserToDeleteRows = false;
            this.watchesDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.watchesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.watchesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.variableColumn,
            this.valueColumn});
            this.watchesDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.watchesDataGridView.Location = new System.Drawing.Point(3, 3);
            this.watchesDataGridView.Name = "watchesDataGridView";
            this.watchesDataGridView.RowHeadersVisible = false;
            this.watchesDataGridView.Size = new System.Drawing.Size(683, 95);
            this.watchesDataGridView.TabIndex = 0;
            // 
            // variableColumn
            // 
            this.variableColumn.HeaderText = "Variable";
            this.variableColumn.Name = "variableColumn";
            this.variableColumn.ReadOnly = true;
            // 
            // valueColumn
            // 
            this.valueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.valueColumn.HeaderText = "Value";
            this.valueColumn.Name = "valueColumn";
            this.valueColumn.ReadOnly = true;
            // 
            // errorTab
            // 
            this.errorTab.Controls.Add(this.errorsDataGridView);
            this.errorTab.Location = new System.Drawing.Point(4, 22);
            this.errorTab.Name = "errorTab";
            this.errorTab.Padding = new System.Windows.Forms.Padding(3);
            this.errorTab.Size = new System.Drawing.Size(689, 101);
            this.errorTab.TabIndex = 0;
            this.errorTab.Text = "Errors";
            this.errorTab.UseVisualStyleBackColor = true;
            // 
            // errorsDataGridView
            // 
            this.errorsDataGridView.AllowUserToAddRows = false;
            this.errorsDataGridView.AllowUserToDeleteRows = false;
            this.errorsDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.errorsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.errorsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.positionColumn,
            this.messageColumn});
            this.errorsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorsDataGridView.Location = new System.Drawing.Point(3, 3);
            this.errorsDataGridView.Name = "errorsDataGridView";
            this.errorsDataGridView.RowHeadersVisible = false;
            this.errorsDataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.errorsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.errorsDataGridView.Size = new System.Drawing.Size(683, 95);
            this.errorsDataGridView.TabIndex = 0;
            this.errorsDataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnErrorsDataGridViewCellDoubleClick);
            this.errorsDataGridView.SelectionChanged += new System.EventHandler(this.OnErrorsDataGridViewSelectionChanged);
            // 
            // positionColumn
            // 
            this.positionColumn.HeaderText = "Position";
            this.positionColumn.Name = "positionColumn";
            this.positionColumn.ReadOnly = true;
            this.positionColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // messageColumn
            // 
            this.messageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.messageColumn.HeaderText = "Message";
            this.messageColumn.Name = "messageColumn";
            this.messageColumn.ReadOnly = true;
            // 
            // outputTab
            // 
            this.outputTab.Controls.Add(this.outputRichTextBox);
            this.outputTab.Location = new System.Drawing.Point(4, 22);
            this.outputTab.Name = "outputTab";
            this.outputTab.Padding = new System.Windows.Forms.Padding(3);
            this.outputTab.Size = new System.Drawing.Size(689, 101);
            this.outputTab.TabIndex = 2;
            this.outputTab.Text = "Output";
            this.outputTab.UseVisualStyleBackColor = true;
            // 
            // outputRichTextBox
            // 
            this.outputRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputRichTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.outputRichTextBox.Location = new System.Drawing.Point(3, 3);
            this.outputRichTextBox.Name = "outputRichTextBox";
            this.outputRichTextBox.ReadOnly = true;
            this.outputRichTextBox.Size = new System.Drawing.Size(683, 95);
            this.outputRichTextBox.TabIndex = 0;
            this.outputRichTextBox.Text = "";
            // 
            // breakpointsTab
            // 
            this.breakpointsTab.Controls.Add(this.breakpointDataGridView);
            this.breakpointsTab.Location = new System.Drawing.Point(4, 22);
            this.breakpointsTab.Name = "breakpointsTab";
            this.breakpointsTab.Padding = new System.Windows.Forms.Padding(3);
            this.breakpointsTab.Size = new System.Drawing.Size(689, 101);
            this.breakpointsTab.TabIndex = 3;
            this.breakpointsTab.Text = "Breakpoints";
            this.breakpointsTab.UseVisualStyleBackColor = true;
            // 
            // breakpointDataGridView
            // 
            this.breakpointDataGridView.AllowUserToAddRows = false;
            this.breakpointDataGridView.AllowUserToDeleteRows = false;
            this.breakpointDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.breakpointDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.breakpointDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.Type,
            this.Information});
            this.breakpointDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.breakpointDataGridView.Location = new System.Drawing.Point(3, 3);
            this.breakpointDataGridView.Name = "breakpointDataGridView";
            this.breakpointDataGridView.RowHeadersVisible = false;
            this.breakpointDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.breakpointDataGridView.Size = new System.Drawing.Size(683, 95);
            this.breakpointDataGridView.TabIndex = 1;
            this.breakpointDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnBreakpointDataGridViewCellEndEdit);
            this.breakpointDataGridView.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.OnBreakpointDataGridViewCellMouseClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.FillWeight = 50F;
            this.dataGridViewTextBoxColumn1.HeaderText = "Start";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 50;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn1.Width = 50;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.FillWeight = 50F;
            this.dataGridViewTextBoxColumn2.HeaderText = "Length";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 50;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn2.Width = 50;
            // 
            // Type
            // 
            this.Type.FillWeight = 75F;
            this.Type.HeaderText = "Type";
            this.Type.MinimumWidth = 75;
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            this.Type.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Type.Width = 75;
            // 
            // Information
            // 
            this.Information.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Information.HeaderText = "Condition";
            this.Information.Name = "Information";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripMenuItem.Image")));
            this.newToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.OnNewFileMainMenuClick);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OnOpenFileMainMenuClick);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(143, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.OnSaveFileMainMenuClick);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.OnSaveAsFileMainMenuClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(143, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.OnExitFileMainMenuClick);
            // 
            // buildToolStripMenuItem
            // 
            this.buildToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buildToolStripMenuItem1,
            this.runToolStripMenuItem});
            this.buildToolStripMenuItem.Name = "buildToolStripMenuItem";
            this.buildToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.buildToolStripMenuItem.Text = "Build";
            // 
            // buildToolStripMenuItem1
            // 
            this.buildToolStripMenuItem1.Name = "buildToolStripMenuItem1";
            this.buildToolStripMenuItem1.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.buildToolStripMenuItem1.Size = new System.Drawing.Size(141, 22);
            this.buildToolStripMenuItem1.Text = "Build";
            this.buildToolStripMenuItem1.Click += new System.EventHandler(this.OnBuildBuildMainMenuClick);
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.runToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.runToolStripMenuItem.Text = "Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.OnRunBuildMainMenuClick);
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.buildToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(697, 24);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stepOverToolStripMenuItem,
            this.stepIntoToolStripMenuItem,
            this.startDebuggingToolStripMenuItem,
            this.newBreakpointToolStripMenuItem,
            this.runToCursorToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // stepOverToolStripMenuItem
            // 
            this.stepOverToolStripMenuItem.Name = "stepOverToolStripMenuItem";
            this.stepOverToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.stepOverToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.stepOverToolStripMenuItem.Text = "Step Over";
            this.stepOverToolStripMenuItem.Click += new System.EventHandler(this.OnStepOverDebugMenuClick);
            // 
            // stepIntoToolStripMenuItem
            // 
            this.stepIntoToolStripMenuItem.Name = "stepIntoToolStripMenuItem";
            this.stepIntoToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.stepIntoToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.stepIntoToolStripMenuItem.Text = "Step Into";
            this.stepIntoToolStripMenuItem.Click += new System.EventHandler(this.OnStepIntoDebugMenuClick);
            // 
            // startDebuggingToolStripMenuItem
            // 
            this.startDebuggingToolStripMenuItem.Name = "startDebuggingToolStripMenuItem";
            this.startDebuggingToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.startDebuggingToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.startDebuggingToolStripMenuItem.Text = "Start Debugging";
            this.startDebuggingToolStripMenuItem.Click += new System.EventHandler(this.OnStartDebuggingDebugMenuClick);
            // 
            // newBreakpointToolStripMenuItem
            // 
            this.newBreakpointToolStripMenuItem.Name = "newBreakpointToolStripMenuItem";
            this.newBreakpointToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F9;
            this.newBreakpointToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.newBreakpointToolStripMenuItem.Text = "New Breakpoint";
            this.newBreakpointToolStripMenuItem.Click += new System.EventHandler(this.OnNewBreakpointDebugMenuClick);
            // 
            // runToCursorToolStripMenuItem
            // 
            this.runToCursorToolStripMenuItem.Name = "runToCursorToolStripMenuItem";
            this.runToCursorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F10)));
            this.runToCursorToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.runToCursorToolStripMenuItem.Text = "Run To Cursor";
            this.runToCursorToolStripMenuItem.Click += new System.EventHandler(this.OnRunToCursorDebugMenuClick);
            // 
            // debugLabel
            // 
            this.debugLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.debugLabel.AutoSize = true;
            this.debugLabel.Location = new System.Drawing.Point(526, 9);
            this.debugLabel.Name = "debugLabel";
            this.debugLabel.Size = new System.Drawing.Size(0, 13);
            this.debugLabel.TabIndex = 2;
            // 
            // mainContextMenuStrip
            // 
            this.mainContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToCursorToolStripMenuItem1,
            this.putBreakpointToolStripMenuItem});
            this.mainContextMenuStrip.Name = "mainContextMenuStrip";
            this.mainContextMenuStrip.Size = new System.Drawing.Size(153, 70);
            // 
            // runToCursorToolStripMenuItem1
            // 
            this.runToCursorToolStripMenuItem1.Name = "runToCursorToolStripMenuItem1";
            this.runToCursorToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.runToCursorToolStripMenuItem1.Text = "RunTo Cursor";
            this.runToCursorToolStripMenuItem1.Click += new System.EventHandler(this.OnRunToCursorMainToolStripMenuItemClick);
            // 
            // putBreakpointToolStripMenuItem
            // 
            this.putBreakpointToolStripMenuItem.Name = "putBreakpointToolStripMenuItem";
            this.putBreakpointToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.putBreakpointToolStripMenuItem.Text = "Put Breakpoint";
            this.putBreakpointToolStripMenuItem.Click += new System.EventHandler(this.OnPutBreakpointMainToolStripMenuItemClick);
            // 
            // MiniStudioForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 386);
            this.Controls.Add(this.debugLabel);
            this.Controls.Add(this.textTabSplitContainer);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "MiniStudioForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Mini Studio";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnMiniStudioFormClosing);
            this.textTabSplitContainer.Panel1.ResumeLayout(false);
            this.textTabSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.textTabSplitContainer)).EndInit();
            this.textTabSplitContainer.ResumeLayout(false);
            this.mainTabControl.ResumeLayout(false);
            this.watchesTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.watchesDataGridView)).EndInit();
            this.errorTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorsDataGridView)).EndInit();
            this.outputTab.ResumeLayout(false);
            this.breakpointsTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.breakpointDataGridView)).EndInit();
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.mainContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.SplitContainer textTabSplitContainer;
        private System.Windows.Forms.RichTextBox mainRichTextBox;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stepIntoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stepOverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startDebuggingToolStripMenuItem;
        private System.Windows.Forms.Label debugLabel;
        private System.Windows.Forms.ToolStripMenuItem newBreakpointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runToCursorToolStripMenuItem;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage watchesTab;
        private System.Windows.Forms.DataGridView watchesDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn variableColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn valueColumn;
        private System.Windows.Forms.TabPage errorTab;
        private System.Windows.Forms.DataGridView errorsDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn positionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn messageColumn;
        private System.Windows.Forms.TabPage outputTab;
        private System.Windows.Forms.RichTextBox outputRichTextBox;
        private System.Windows.Forms.TabPage breakpointsTab;
        private System.Windows.Forms.DataGridView breakpointDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Information;
        private System.Windows.Forms.ContextMenuStrip mainContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem runToCursorToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem putBreakpointToolStripMenuItem;
    }
}

