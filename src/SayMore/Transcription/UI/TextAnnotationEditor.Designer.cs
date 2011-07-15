namespace SayMore.Transcription.UI
{
	partial class TextAnnotationEditor
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._tableLayout = new System.Windows.Forms.TableLayoutPanel();
			this._splitter = new System.Windows.Forms.SplitContainer();
			this._buttonExport = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._comboPlaybackSpeed = new System.Windows.Forms.ComboBox();
			this._labelPlaybackSpeed = new System.Windows.Forms.Label();
			this._tableLayout.SuspendLayout();
			this._splitter.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tableLayout
			// 
			this._tableLayout.BackColor = System.Drawing.Color.Transparent;
			this._tableLayout.ColumnCount = 2;
			this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayout.Controls.Add(this._splitter, 0, 1);
			this._tableLayout.Controls.Add(this._buttonExport, 1, 0);
			this._tableLayout.Controls.Add(this.tableLayoutPanel1, 0, 0);
			this._tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayout.Location = new System.Drawing.Point(12, 12);
			this._tableLayout.Name = "_tableLayout";
			this._tableLayout.RowCount = 2;
			this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayout.Size = new System.Drawing.Size(425, 255);
			this._tableLayout.TabIndex = 0;
			// 
			// _splitter
			// 
			this._splitter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._tableLayout.SetColumnSpan(this._splitter, 2);
			this._splitter.Location = new System.Drawing.Point(0, 34);
			this._splitter.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
			this._splitter.Name = "_splitter";
			this._splitter.Size = new System.Drawing.Size(425, 221);
			this._splitter.SplitterDistance = 139;
			this._splitter.SplitterWidth = 8;
			this._splitter.TabIndex = 2;
			// 
			// _buttonExport
			// 
			this._buttonExport.AutoSize = true;
			this._buttonExport.Image = global::SayMore.Properties.Resources.InterlinearExport;
			this._buttonExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonExport.Location = new System.Drawing.Point(328, 0);
			this._buttonExport.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this._buttonExport.Name = "_buttonExport";
			this._buttonExport.Size = new System.Drawing.Size(97, 26);
			this._buttonExport.TabIndex = 3;
			this._buttonExport.Text = "Export...";
			this._buttonExport.UseVisualStyleBackColor = true;
			this._buttonExport.Click += new System.EventHandler(this.HandleExportButtonClick);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this._comboPlaybackSpeed, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._labelPlaybackSpeed, 0, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 2);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(191, 21);
			this.tableLayoutPanel1.TabIndex = 4;
			// 
			// _comboPlaybackSpeed
			// 
			this._comboPlaybackSpeed.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._comboPlaybackSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._comboPlaybackSpeed.FormattingEnabled = true;
			this._comboPlaybackSpeed.Location = new System.Drawing.Point(94, 0);
			this._comboPlaybackSpeed.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this._comboPlaybackSpeed.Name = "_comboPlaybackSpeed";
			this._comboPlaybackSpeed.Size = new System.Drawing.Size(97, 21);
			this._comboPlaybackSpeed.TabIndex = 2;
			// 
			// _labelPlaybackSpeed
			// 
			this._labelPlaybackSpeed.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelPlaybackSpeed.AutoSize = true;
			this._labelPlaybackSpeed.Location = new System.Drawing.Point(0, 4);
			this._labelPlaybackSpeed.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this._labelPlaybackSpeed.Name = "_labelPlaybackSpeed";
			this._labelPlaybackSpeed.Size = new System.Drawing.Size(88, 13);
			this._labelPlaybackSpeed.TabIndex = 1;
			this._labelPlaybackSpeed.Text = "Playback &Speed:";
			// 
			// TextAnnotationEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._tableLayout);
			this.Name = "TextAnnotationEditor";
			this.Padding = new System.Windows.Forms.Padding(12);
			this.Size = new System.Drawing.Size(449, 279);
			this._tableLayout.ResumeLayout(false);
			this._tableLayout.PerformLayout();
			this._splitter.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.TableLayoutPanel _tableLayout;
		private System.Windows.Forms.SplitContainer _splitter;
		private System.Windows.Forms.Button _buttonExport;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox _comboPlaybackSpeed;
        private System.Windows.Forms.Label _labelPlaybackSpeed;


	}
}
