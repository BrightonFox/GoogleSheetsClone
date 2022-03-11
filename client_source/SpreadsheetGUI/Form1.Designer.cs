// File was copied over from Nick's spreadsheet project as a base for CS3505
// File was modified by Clay Ankeny & Glorien Roque for CS3505


namespace SS
{
    partial class Window
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
            this.NameLabel = new System.Windows.Forms.Label();
            this.ValueLabel = new System.Windows.Forms.Label();
            this.ContentsLabel = new System.Windows.Forms.Label();
            this.NameBox = new System.Windows.Forms.TextBox();
            this.ValueBox = new System.Windows.Forms.TextBox();
            this.ContentsBox = new System.Windows.Forms.TextBox();
            this.SetButton = new System.Windows.Forms.Button();
            this.BGWorker = new System.ComponentModel.BackgroundWorker();
            this.SetProgress = new System.Windows.Forms.ProgressBar();
            this.FileMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuButton = new System.Windows.Forms.Button();
            this.HelpMenuButton = new System.Windows.Forms.Button();
            this.connectButton = new System.Windows.Forms.Button();
            this.usernameBox = new System.Windows.Forms.TextBox();
            this.addressBox = new System.Windows.Forms.TextBox();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.addressLabel = new System.Windows.Forms.Label();
            this.sheetNameBox = new System.Windows.Forms.TextBox();
            this.sheetNameLabel = new System.Windows.Forms.Label();
            this.retreiveButton = new System.Windows.Forms.Button();
            this.SPPanel = new SS.SpreadsheetPanel();
            this.undoButton = new System.Windows.Forms.Button();
            this.revertButton = new System.Windows.Forms.Button();
            this.FileMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameLabel.Location = new System.Drawing.Point(91, 15);
            this.NameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(51, 28);
            this.NameLabel.TabIndex = 1;
            this.NameLabel.Text = "Cell:";
            // 
            // ValueLabel
            // 
            this.ValueLabel.AutoSize = true;
            this.ValueLabel.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ValueLabel.Location = new System.Drawing.Point(213, 16);
            this.ValueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ValueLabel.Name = "ValueLabel";
            this.ValueLabel.Size = new System.Drawing.Size(67, 28);
            this.ValueLabel.TabIndex = 2;
            this.ValueLabel.Text = "Value:";
            // 
            // ContentsLabel
            // 
            this.ContentsLabel.AutoSize = true;
            this.ContentsLabel.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ContentsLabel.Location = new System.Drawing.Point(385, 14);
            this.ContentsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ContentsLabel.Name = "ContentsLabel";
            this.ContentsLabel.Size = new System.Drawing.Size(98, 28);
            this.ContentsLabel.TabIndex = 3;
            this.ContentsLabel.Text = "Contents:";
            // 
            // NameBox
            // 
            this.NameBox.Enabled = false;
            this.NameBox.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameBox.Location = new System.Drawing.Point(141, 9);
            this.NameBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(65, 41);
            this.NameBox.TabIndex = 4;
            // 
            // ValueBox
            // 
            this.ValueBox.Enabled = false;
            this.ValueBox.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ValueBox.Location = new System.Drawing.Point(280, 9);
            this.ValueBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ValueBox.Name = "ValueBox";
            this.ValueBox.Size = new System.Drawing.Size(96, 41);
            this.ValueBox.TabIndex = 5;
            // 
            // ContentsBox
            // 
            this.ContentsBox.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ContentsBox.Location = new System.Drawing.Point(483, 7);
            this.ContentsBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ContentsBox.Name = "ContentsBox";
            this.ContentsBox.Size = new System.Drawing.Size(188, 41);
            this.ContentsBox.TabIndex = 6;
            // 
            // SetButton
            // 
            this.SetButton.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SetButton.Location = new System.Drawing.Point(680, 7);
            this.SetButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SetButton.Name = "SetButton";
            this.SetButton.Size = new System.Drawing.Size(60, 42);
            this.SetButton.TabIndex = 7;
            this.SetButton.Text = "Set";
            this.SetButton.UseVisualStyleBackColor = true;
            this.SetButton.Click += new System.EventHandler(this.SetButton_Click);
            // 
            // BGWorker
            // 
            this.BGWorker.WorkerReportsProgress = true;
            this.BGWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BGWorker_DoWork);
            this.BGWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BGWorker_ProgressChanged);
            this.BGWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BGWorker_RunWorkerCompleted);
            // 
            // SetProgress
            // 
            this.SetProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SetProgress.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.SetProgress.Location = new System.Drawing.Point(1816, 15);
            this.SetProgress.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SetProgress.Name = "SetProgress";
            this.SetProgress.Size = new System.Drawing.Size(133, 28);
            this.SetProgress.TabIndex = 8;
            this.SetProgress.Visible = false;
            // 
            // FileMenu
            // 
            this.FileMenu.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FileMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.FileMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMenuItem,
            this.openMenuItem,
            this.saveMenuItem,
            this.closeMenuItem});
            this.FileMenu.Name = "FileMenu";
            this.FileMenu.Size = new System.Drawing.Size(232, 116);
            // 
            // newMenuItem
            // 
            this.newMenuItem.Name = "newMenuItem";
            this.newMenuItem.Size = new System.Drawing.Size(231, 28);
            // 
            // openMenuItem
            // 
            this.openMenuItem.Name = "openMenuItem";
            this.openMenuItem.Size = new System.Drawing.Size(231, 28);
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Name = "saveMenuItem";
            this.saveMenuItem.Size = new System.Drawing.Size(231, 28);
            // 
            // closeMenuItem
            // 
            this.closeMenuItem.Name = "closeMenuItem";
            this.closeMenuItem.Size = new System.Drawing.Size(231, 28);
            this.closeMenuItem.Text = "Close Spreadsheet";
            this.closeMenuItem.Click += new System.EventHandler(this.closeMenuItem_Click);
            // 
            // MenuButton
            // 
            this.MenuButton.Location = new System.Drawing.Point(0, 0);
            this.MenuButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MenuButton.Name = "MenuButton";
            this.MenuButton.Size = new System.Drawing.Size(75, 23);
            this.MenuButton.TabIndex = 22;
            this.MenuButton.Visible = false;
            // 
            // HelpMenuButton
            // 
            this.HelpMenuButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.HelpMenuButton.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HelpMenuButton.Location = new System.Drawing.Point(1957, 9);
            this.HelpMenuButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.HelpMenuButton.Name = "HelpMenuButton";
            this.HelpMenuButton.Size = new System.Drawing.Size(71, 37);
            this.HelpMenuButton.TabIndex = 11;
            this.HelpMenuButton.Text = "Help";
            this.HelpMenuButton.UseVisualStyleBackColor = true;
            this.HelpMenuButton.Click += new System.EventHandler(this.HelpMenuButton_Click);
            // 
            // connectButton
            // 
            this.connectButton.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectButton.Location = new System.Drawing.Point(1383, 6);
            this.connectButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(109, 42);
            this.connectButton.TabIndex = 12;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // usernameBox
            // 
            this.usernameBox.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameBox.Location = new System.Drawing.Point(1277, 6);
            this.usernameBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.usernameBox.Name = "usernameBox";
            this.usernameBox.Size = new System.Drawing.Size(96, 41);
            this.usernameBox.TabIndex = 16;
            // 
            // addressBox
            // 
            this.addressBox.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addressBox.Location = new System.Drawing.Point(1037, 6);
            this.addressBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.addressBox.Name = "addressBox";
            this.addressBox.Size = new System.Drawing.Size(164, 41);
            this.addressBox.TabIndex = 15;
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameLabel.Location = new System.Drawing.Point(1211, 14);
            this.usernameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(70, 28);
            this.usernameLabel.TabIndex = 14;
            this.usernameLabel.Text = "Name:";
            // 
            // addressLabel
            // 
            this.addressLabel.AutoSize = true;
            this.addressLabel.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addressLabel.Location = new System.Drawing.Point(941, 14);
            this.addressLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.addressLabel.Name = "addressLabel";
            this.addressLabel.Size = new System.Drawing.Size(97, 28);
            this.addressLabel.TabIndex = 13;
            this.addressLabel.Text = "Address:";
            // 
            // sheetNameBox
            // 
            this.sheetNameBox.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sheetNameBox.Location = new System.Drawing.Point(1584, 7);
            this.sheetNameBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.sheetNameBox.Name = "sheetNameBox";
            this.sheetNameBox.Size = new System.Drawing.Size(96, 41);
            this.sheetNameBox.TabIndex = 19;
            // 
            // sheetNameLabel
            // 
            this.sheetNameLabel.AutoSize = true;
            this.sheetNameLabel.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sheetNameLabel.Location = new System.Drawing.Point(1500, 11);
            this.sheetNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.sheetNameLabel.Name = "sheetNameLabel";
            this.sheetNameLabel.Size = new System.Drawing.Size(74, 28);
            this.sheetNameLabel.TabIndex = 18;
            this.sheetNameLabel.Text = "Sheet:";
            // 
            // retreiveButton
            // 
            this.retreiveButton.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.retreiveButton.Location = new System.Drawing.Point(1689, 7);
            this.retreiveButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.retreiveButton.Name = "retreiveButton";
            this.retreiveButton.Size = new System.Drawing.Size(109, 42);
            this.retreiveButton.TabIndex = 17;
            this.retreiveButton.Text = "Retreive";
            this.retreiveButton.UseVisualStyleBackColor = true;
            this.retreiveButton.Click += new System.EventHandler(this.retreiveButton_Click);
            // 
            // SPPanel
            // 
            this.SPPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SPPanel.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SPPanel.Location = new System.Drawing.Point(16, 57);
            this.SPPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.SPPanel.MinimumSize = new System.Drawing.Size(1, 1);
            this.SPPanel.Name = "SPPanel";
            this.SPPanel.Size = new System.Drawing.Size(2012, 767);
            this.SPPanel.TabIndex = 0;
            // 
            // undoButton
            // 
            this.undoButton.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.undoButton.Location = new System.Drawing.Point(748, 6);
            this.undoButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(75, 42);
            this.undoButton.TabIndex = 20;
            this.undoButton.Text = "Undo";
            this.undoButton.UseVisualStyleBackColor = true;
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // revertButton
            // 
            this.revertButton.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.revertButton.Location = new System.Drawing.Point(831, 5);
            this.revertButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.revertButton.Name = "revertButton";
            this.revertButton.Size = new System.Drawing.Size(103, 42);
            this.revertButton.TabIndex = 21;
            this.revertButton.Text = "Revert";
            this.revertButton.UseVisualStyleBackColor = true;
            this.revertButton.Click += new System.EventHandler(this.revertButton_Click);
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2044, 838);
            this.Controls.Add(this.revertButton);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.sheetNameBox);
            this.Controls.Add(this.sheetNameLabel);
            this.Controls.Add(this.retreiveButton);
            this.Controls.Add(this.usernameBox);
            this.Controls.Add(this.addressBox);
            this.Controls.Add(this.usernameLabel);
            this.Controls.Add(this.addressLabel);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.HelpMenuButton);
            this.Controls.Add(this.MenuButton);
            this.Controls.Add(this.SetProgress);
            this.Controls.Add(this.SetButton);
            this.Controls.Add(this.ContentsBox);
            this.Controls.Add(this.ValueBox);
            this.Controls.Add(this.NameBox);
            this.Controls.Add(this.ContentsLabel);
            this.Controls.Add(this.ValueLabel);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.SPPanel);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(1274, 207);
            this.Name = "Window";
            this.Text = "Bootleg Google Sheets";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Window_FormClosing);
            this.FileMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SS.SpreadsheetPanel SPPanel;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Label ValueLabel;
        private System.Windows.Forms.Label ContentsLabel;
        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.TextBox ValueBox;
        private System.Windows.Forms.TextBox ContentsBox;
        private System.Windows.Forms.Button SetButton;
        private System.ComponentModel.BackgroundWorker BGWorker;
        private System.Windows.Forms.ProgressBar SetProgress;
        private System.Windows.Forms.ContextMenuStrip FileMenu;
        private System.Windows.Forms.ToolStripMenuItem newMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.Button MenuButton;
        private System.Windows.Forms.ToolStripMenuItem closeMenuItem;
        private System.Windows.Forms.Button HelpMenuButton;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.TextBox usernameBox;
        private System.Windows.Forms.TextBox addressBox;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.Label addressLabel;
        private System.Windows.Forms.TextBox sheetNameBox;
        private System.Windows.Forms.Label sheetNameLabel;
        private System.Windows.Forms.Button retreiveButton;
        private System.Windows.Forms.Button undoButton;
        private System.Windows.Forms.Button revertButton;
    }
}

