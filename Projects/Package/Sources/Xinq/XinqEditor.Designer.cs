namespace Xinq {
    partial class XinqEditor {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XinqEditor));
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.tsAddQuery = new System.Windows.Forms.ToolStripButton();
            this.tsRemoveQuery = new System.Windows.Forms.ToolStripButton();
            this.SplitContainer = new System.Windows.Forms.SplitContainer();
            this.dgQueries = new System.Windows.Forms.DataGridView();
            this.dgcName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcComment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtQuery = new Xinq.QueryTextBox();
            this.ToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
            this.SplitContainer.Panel1.SuspendLayout();
            this.SplitContainer.Panel2.SuspendLayout();
            this.SplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgQueries)).BeginInit();
            this.SuspendLayout();
            // 
            // ToolStrip
            // 
            this.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsAddQuery,
            this.tsRemoveQuery});
            this.ToolStrip.Location = new System.Drawing.Point(0, 0);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.ToolStrip.Size = new System.Drawing.Size(869, 29);
            this.ToolStrip.TabIndex = 0;
            // 
            // tsAddQuery
            // 
            this.tsAddQuery.AutoSize = false;
            this.tsAddQuery.Image = ((System.Drawing.Image)(resources.GetObject("tsAddQuery.Image")));
            this.tsAddQuery.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsAddQuery.Name = "tsAddQuery";
            this.tsAddQuery.Size = new System.Drawing.Size(82, 22);
            this.tsAddQuery.Text = "Add &Query";
            this.tsAddQuery.Click += new System.EventHandler(this.tsAddQuery_Click);
            // 
            // tsRemoveQuery
            // 
            this.tsRemoveQuery.AutoSize = false;
            this.tsRemoveQuery.Image = ((System.Drawing.Image)(resources.GetObject("tsRemoveQuery.Image")));
            this.tsRemoveQuery.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRemoveQuery.Name = "tsRemoveQuery";
            this.tsRemoveQuery.Size = new System.Drawing.Size(101, 22);
            this.tsRemoveQuery.Text = "Re&move Query";
            this.tsRemoveQuery.Click += new System.EventHandler(this.tsRemoveQuery_Click);
            // 
            // SplitContainer
            // 
            this.SplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.SplitContainer.Location = new System.Drawing.Point(14, 49);
            this.SplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.SplitContainer.Name = "SplitContainer";
            this.SplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer.Panel1
            // 
            this.SplitContainer.Panel1.Controls.Add(this.dgQueries);
            // 
            // SplitContainer.Panel2
            // 
            this.SplitContainer.Panel2.Controls.Add(this.txtQuery);
            this.SplitContainer.Size = new System.Drawing.Size(841, 619);
            this.SplitContainer.SplitterDistance = 209;
            this.SplitContainer.SplitterWidth = 10;
            this.SplitContainer.TabIndex = 3;
            this.SplitContainer.TabStop = false;
            // 
            // dgQueries
            // 
            this.dgQueries.AllowUserToAddRows = false;
            this.dgQueries.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgQueries.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgQueries.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgQueries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgQueries.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgcName,
            this.dgcComment});
            this.dgQueries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgQueries.Location = new System.Drawing.Point(0, 0);
            this.dgQueries.MultiSelect = false;
            this.dgQueries.Name = "dgQueries";
            this.dgQueries.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgQueries.Size = new System.Drawing.Size(841, 209);
            this.dgQueries.StandardTab = true;
            this.dgQueries.TabIndex = 0;
            this.dgQueries.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgQueries_CellBeginEdit);
            this.dgQueries.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgQueries_CellEndEdit);
            this.dgQueries.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgQueries_CellValidating);
            this.dgQueries.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgQueries_RowsAdded);
            this.dgQueries.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgQueries_RowsRemoved);
            this.dgQueries.SelectionChanged += new System.EventHandler(this.dgQueries_SelectionChanged);
            this.dgQueries.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgQueries_UserDeletingRow);
            // 
            // dgcName
            // 
            this.dgcName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcName.DataPropertyName = "Name";
            this.dgcName.FillWeight = 157.8854F;
            this.dgcName.Frozen = true;
            this.dgcName.HeaderText = "Name";
            this.dgcName.MinimumWidth = 150;
            this.dgcName.Name = "dgcName";
            this.dgcName.Width = 150;
            // 
            // dgcComment
            // 
            this.dgcComment.DataPropertyName = "Comment";
            this.dgcComment.FillWeight = 2.013084F;
            this.dgcComment.HeaderText = "Comment";
            this.dgcComment.MinimumWidth = 100;
            this.dgcComment.Name = "dgcComment";
            // 
            // txtQuery
            // 
            this.txtQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtQuery.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQuery.Location = new System.Drawing.Point(0, 0);
            this.txtQuery.Multiline = true;
            this.txtQuery.Name = "txtQuery";
            this.txtQuery.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtQuery.Size = new System.Drawing.Size(841, 400);
            this.txtQuery.TabIndex = 2;
            this.txtQuery.TextChanging += new System.ComponentModel.CancelEventHandler(this.txtQuery_TextChanging);
            this.txtQuery.TextChanged += new System.EventHandler(this.txtQuery_TextChanged);
            // 
            // XinqEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SplitContainer);
            this.Controls.Add(this.ToolStrip);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "XinqEditor";
            this.Size = new System.Drawing.Size(869, 681);
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.SplitContainer.Panel1.ResumeLayout(false);
            this.SplitContainer.Panel2.ResumeLayout(false);
            this.SplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).EndInit();
            this.SplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgQueries)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip ToolStrip;
        private System.Windows.Forms.ToolStripButton tsAddQuery;
        private QueryTextBox txtQuery;
        private System.Windows.Forms.SplitContainer SplitContainer;
        private System.Windows.Forms.DataGridView dgQueries;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcComment;
        private System.Windows.Forms.ToolStripButton tsRemoveQuery;
    }
}
