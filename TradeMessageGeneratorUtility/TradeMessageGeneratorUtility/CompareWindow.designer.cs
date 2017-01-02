namespace TradeMessageGenerator
{
    partial class CompareWindow
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstFirstFiles = new System.Windows.Forms.ListBox();
            this.lstSecondFiles = new System.Windows.Forms.ListBox();
            this.btnCompare = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.lblFileOne = new System.Windows.Forms.Label();
            this.lblFileTwo = new System.Windows.Forms.Label();
            this.lblDiff = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstFirstFiles);
            this.groupBox1.Controls.Add(this.lstSecondFiles);
            this.groupBox1.Controls.Add(this.btnCompare);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(860, 223);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log Files";
            // 
            // lstFirstFiles
            // 
            this.lstFirstFiles.FormattingEnabled = true;
            this.lstFirstFiles.Location = new System.Drawing.Point(16, 60);
            this.lstFirstFiles.Name = "lstFirstFiles";
            this.lstFirstFiles.Size = new System.Drawing.Size(400, 108);
            this.lstFirstFiles.TabIndex = 0;
            this.lstFirstFiles.SelectedIndexChanged += new System.EventHandler(this.lstFirstFiles_SelectedIndexChanged);
            // 
            // lstSecondFiles
            // 
            this.lstSecondFiles.Enabled = false;
            this.lstSecondFiles.FormattingEnabled = true;
            this.lstSecondFiles.Location = new System.Drawing.Point(434, 60);
            this.lstSecondFiles.Name = "lstSecondFiles";
            this.lstSecondFiles.Size = new System.Drawing.Size(400, 108);
            this.lstSecondFiles.TabIndex = 1;
            this.lstSecondFiles.SelectedIndexChanged += new System.EventHandler(this.lstSecondFiles_SelectedIndexChanged);
            // 
            // btnCompare
            // 
            this.btnCompare.Location = new System.Drawing.Point(759, 182);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(75, 23);
            this.btnCompare.TabIndex = 4;
            this.btnCompare.Text = "Compare";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(431, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Second File:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "First File:";
            // 
            // listView1
            // 
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.Location = new System.Drawing.Point(12, 315);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(860, 290);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // lblFileOne
            // 
            this.lblFileOne.AutoSize = true;
            this.lblFileOne.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileOne.Location = new System.Drawing.Point(16, 289);
            this.lblFileOne.Name = "lblFileOne";
            this.lblFileOne.Size = new System.Drawing.Size(41, 13);
            this.lblFileOne.TabIndex = 2;
            this.lblFileOne.Text = "label3";
            // 
            // lblFileTwo
            // 
            this.lblFileTwo.AutoSize = true;
            this.lblFileTwo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileTwo.Location = new System.Drawing.Point(443, 289);
            this.lblFileTwo.Name = "lblFileTwo";
            this.lblFileTwo.Size = new System.Drawing.Size(41, 13);
            this.lblFileTwo.TabIndex = 3;
            this.lblFileTwo.Text = "label4";
            // 
            // lblDiff
            // 
            this.lblDiff.AutoSize = true;
            this.lblDiff.Location = new System.Drawing.Point(17, 624);
            this.lblDiff.Name = "lblDiff";
            this.lblDiff.Size = new System.Drawing.Size(35, 13);
            this.lblDiff.TabIndex = 4;
            this.lblDiff.Text = "label3";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 661);
            this.Controls.Add(this.lblDiff);
            this.Controls.Add(this.lblFileTwo);
            this.Controls.Add(this.lblFileOne);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.groupBox1);
            this.MaximumSize = new System.Drawing.Size(900, 700);
            this.MinimumSize = new System.Drawing.Size(900, 700);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Log File Compare Window";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.ListBox lstFirstFiles;
        private System.Windows.Forms.ListBox lstSecondFiles;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label lblFileOne;
        private System.Windows.Forms.Label lblFileTwo;
        private System.Windows.Forms.Label lblDiff;
    }
}

