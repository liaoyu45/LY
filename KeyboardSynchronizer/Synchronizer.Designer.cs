namespace KeyboardSynchronizer {
    partial class Synchronizer {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Synchronizer));
            this.allWindows = new System.Windows.Forms.DataGridView();
            this.main = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.winName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.up = new System.Windows.Forms.DataGridViewButtonColumn();
            this.remove = new System.Windows.Forms.DataGridViewButtonColumn();
            this.run = new System.Windows.Forms.Button();
            this.tips = new System.Windows.Forms.Label();
            this.pick = new System.Windows.Forms.Button();
            this.quit = new System.Windows.Forms.Button();
            this.excludedKeys = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.allWindows)).BeginInit();
            this.SuspendLayout();
            // 
            // allWindows
            // 
            this.allWindows.AllowUserToAddRows = false;
            this.allWindows.AllowUserToResizeColumns = false;
            this.allWindows.AllowUserToResizeRows = false;
            this.allWindows.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.allWindows.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.allWindows.ColumnHeadersVisible = false;
            this.allWindows.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.main,
            this.winName,
            this.up,
            this.remove});
            this.allWindows.Location = new System.Drawing.Point(9, 53);
            this.allWindows.MultiSelect = false;
            this.allWindows.Name = "allWindows";
            this.allWindows.RowHeadersVisible = false;
            this.allWindows.RowTemplate.Height = 23;
            this.allWindows.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.allWindows.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.allWindows.Size = new System.Drawing.Size(283, 194);
            this.allWindows.TabIndex = 1;
            this.allWindows.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.allWindows_CellClick);
            // 
            // main
            // 
            this.main.FillWeight = 57.07589F;
            this.main.HeaderText = "";
            this.main.Name = "main";
            this.main.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // winName
            // 
            this.winName.FillWeight = 183.3091F;
            this.winName.HeaderText = "";
            this.winName.Name = "winName";
            this.winName.ReadOnly = true;
            // 
            // up
            // 
            this.up.FillWeight = 57.07589F;
            this.up.HeaderText = "";
            this.up.Name = "up";
            this.up.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // remove
            // 
            this.remove.FillWeight = 57.07589F;
            this.remove.HeaderText = "";
            this.remove.Name = "remove";
            this.remove.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // run
            // 
            this.run.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.run.AutoEllipsis = true;
            this.run.Location = new System.Drawing.Point(226, 252);
            this.run.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.run.Name = "run";
            this.run.Size = new System.Drawing.Size(67, 38);
            this.run.TabIndex = 4;
            this.run.Text = "run";
            this.run.UseVisualStyleBackColor = true;
            this.run.Click += new System.EventHandler(this.run_Click);
            // 
            // tips
            // 
            this.tips.AutoEllipsis = true;
            this.tips.BackColor = System.Drawing.Color.Transparent;
            this.tips.Location = new System.Drawing.Point(8, 9);
            this.tips.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.tips.Name = "tips";
            this.tips.Size = new System.Drawing.Size(212, 38);
            this.tips.TabIndex = 6;
            this.tips.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tips.MouseDown += new System.Windows.Forms.MouseEventHandler(this.moveWindow);
            // 
            // pick
            // 
            this.pick.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.pick.Location = new System.Drawing.Point(226, 9);
            this.pick.Name = "pick";
            this.pick.Size = new System.Drawing.Size(67, 38);
            this.pick.TabIndex = 0;
            this.pick.Text = "pick";
            this.pick.UseVisualStyleBackColor = true;
            this.pick.Click += new System.EventHandler(this.pick_Click);
            this.pick.Leave += new System.EventHandler(this.pick_Leave);
            // 
            // quit
            // 
            this.quit.Location = new System.Drawing.Point(155, 252);
            this.quit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.quit.Name = "quit";
            this.quit.Size = new System.Drawing.Size(67, 38);
            this.quit.TabIndex = 3;
            this.quit.Text = "quit";
            this.quit.UseVisualStyleBackColor = true;
            this.quit.Click += new System.EventHandler(this.quit_Click);
            // 
            // excludedKeys
            // 
            this.excludedKeys.Location = new System.Drawing.Point(9, 253);
            this.excludedKeys.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.excludedKeys.Multiline = true;
            this.excludedKeys.Name = "excludedKeys";
            this.excludedKeys.ReadOnly = true;
            this.excludedKeys.Size = new System.Drawing.Size(142, 35);
            this.excludedKeys.TabIndex = 2;
            this.excludedKeys.Text = "excluded keys";
            this.excludedKeys.KeyDown += new System.Windows.Forms.KeyEventHandler(this.excludedKeys_KeyDown);
            this.excludedKeys.MouseDown += new System.Windows.Forms.MouseEventHandler(this.moveWindow);
            // 
            // Synchronizer
            // 
            this.AcceptButton = this.run;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.ControlBox = false;
            this.Controls.Add(this.excludedKeys);
            this.Controls.Add(this.quit);
            this.Controls.Add(this.pick);
            this.Controls.Add(this.tips);
            this.Controls.Add(this.run);
            this.Controls.Add(this.allWindows);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Synchronizer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.Synchronizer_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Synchronizer_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.moveWindow);
            ((System.ComponentModel.ISupportInitialize)(this.allWindows)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.DataGridView allWindows;
        public System.Windows.Forms.Label tips;
        public System.Windows.Forms.Button pick;
        public System.Windows.Forms.DataGridViewCheckBoxColumn main;
        public System.Windows.Forms.DataGridViewTextBoxColumn winName;
        public System.Windows.Forms.DataGridViewButtonColumn up;
        public System.Windows.Forms.DataGridViewButtonColumn remove;
        public System.Windows.Forms.Button run;
        private System.Windows.Forms.Button quit;
        private System.Windows.Forms.TextBox excludedKeys;
    }
}

