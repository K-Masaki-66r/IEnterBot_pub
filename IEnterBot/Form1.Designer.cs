namespace IEnterBot
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TimesColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.UidColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ChangeMasterButton = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.AddButton = new System.Windows.Forms.Button();
			this.RemoveButton = new System.Windows.Forms.Button();
			this.TweetButton = new System.Windows.Forms.Button();
			this.SaveButton = new System.Windows.Forms.Button();
			this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
			this.textBox2 = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AllowUserToResizeColumns = false;
			this.dataGridView1.AllowUserToResizeRows = false;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColumn,
            this.TimesColumn,
            this.UidColumn});
			this.dataGridView1.Location = new System.Drawing.Point(105, 44);
			this.dataGridView1.MultiSelect = false;
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.ReadOnly = true;
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.RowTemplate.Height = 25;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.dataGridView1.Size = new System.Drawing.Size(461, 246);
			this.dataGridView1.TabIndex = 0;
			this.dataGridView1.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGridView1_SortCompare);
			// 
			// NameColumn
			// 
			this.NameColumn.HeaderText = "名前";
			this.NameColumn.Name = "NameColumn";
			this.NameColumn.ReadOnly = true;
			this.NameColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.NameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.NameColumn.Width = 178;
			// 
			// TimesColumn
			// 
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.TimesColumn.DefaultCellStyle = dataGridViewCellStyle2;
			this.TimesColumn.HeaderText = "回数";
			this.TimesColumn.Name = "TimesColumn";
			this.TimesColumn.ReadOnly = true;
			this.TimesColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.TimesColumn.Width = 60;
			// 
			// UidColumn
			// 
			this.UidColumn.HeaderText = "IDm/UID";
			this.UidColumn.Name = "UidColumn";
			this.UidColumn.ReadOnly = true;
			this.UidColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.UidColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.UidColumn.Width = 220;
			// 
			// ChangeMasterButton
			// 
			this.ChangeMasterButton.Location = new System.Drawing.Point(5, 44);
			this.ChangeMasterButton.Name = "ChangeMasterButton";
			this.ChangeMasterButton.Size = new System.Drawing.Size(94, 39);
			this.ChangeMasterButton.TabIndex = 1;
			this.ChangeMasterButton.Text = "マスター変更";
			this.ChangeMasterButton.UseVisualStyleBackColor = true;
			this.ChangeMasterButton.Click += new System.EventHandler(this.changeMasterButton_Click);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(105, 296);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(461, 67);
			this.textBox1.TabIndex = 2;
			// 
			// AddButton
			// 
			this.AddButton.Location = new System.Drawing.Point(5, 101);
			this.AddButton.Name = "AddButton";
			this.AddButton.Size = new System.Drawing.Size(94, 39);
			this.AddButton.TabIndex = 3;
			this.AddButton.Text = "追加";
			this.AddButton.UseVisualStyleBackColor = true;
			this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
			// 
			// RemoveButton
			// 
			this.RemoveButton.Location = new System.Drawing.Point(5, 158);
			this.RemoveButton.Name = "RemoveButton";
			this.RemoveButton.Size = new System.Drawing.Size(94, 39);
			this.RemoveButton.TabIndex = 4;
			this.RemoveButton.Text = "削除";
			this.RemoveButton.UseVisualStyleBackColor = true;
			this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
			// 
			// TweetButton
			// 
			this.TweetButton.Location = new System.Drawing.Point(5, 215);
			this.TweetButton.Name = "TweetButton";
			this.TweetButton.Size = new System.Drawing.Size(94, 39);
			this.TweetButton.TabIndex = 5;
			this.TweetButton.Text = "投稿";
			this.TweetButton.UseVisualStyleBackColor = true;
			this.TweetButton.Click += new System.EventHandler(this.PostButton_Click);
			// 
			// SaveButton
			// 
			this.SaveButton.Location = new System.Drawing.Point(5, 324);
			this.SaveButton.Name = "SaveButton";
			this.SaveButton.Size = new System.Drawing.Size(94, 39);
			this.SaveButton.TabIndex = 6;
			this.SaveButton.Text = "保存";
			this.SaveButton.UseVisualStyleBackColor = true;
			this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
			// 
			// textBox2
			// 
			this.textBox2.Font = new System.Drawing.Font("Yu Gothic UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.textBox2.Location = new System.Drawing.Point(5, 2);
			this.textBox2.Name = "textBox2";
			this.textBox2.ReadOnly = true;
			this.textBox2.Size = new System.Drawing.Size(561, 36);
			this.textBox2.TabIndex = 7;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(578, 375);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.SaveButton);
			this.Controls.Add(this.TweetButton);
			this.Controls.Add(this.RemoveButton);
			this.Controls.Add(this.AddButton);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.ChangeMasterButton);
			this.Controls.Add(this.dataGridView1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.Margin = new System.Windows.Forms.Padding(2);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Form1";
			this.Text = "IEnterBot";
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		internal DataGridView dataGridView1;
		private Button ChangeMasterButton;
		private TextBox textBox1;
		private Button AddButton;
		private Button RemoveButton;
		private Button TweetButton;
		private Button SaveButton;
		private BindingSource bindingSource1;
		private TextBox textBox2;
		private DataGridViewTextBoxColumn NameColumn;
		private DataGridViewTextBoxColumn TimesColumn;
		private DataGridViewTextBoxColumn UidColumn;
	}
}