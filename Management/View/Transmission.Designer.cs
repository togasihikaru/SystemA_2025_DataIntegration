namespace Management
{
    public partial class Transmission
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
        private void InitializeComponent() // ここが private であることを確認
        {
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            _salesRecord = new DataGridView();
            chkCategoryD = new CheckBox();
            chkCategoryC = new CheckBox();
            chkCategoryB = new CheckBox();
            chkCategoryA = new CheckBox();
            txtStartDate = new TextBox();
            txtEndDate = new TextBox();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            checkBox1 = new CheckBox();
            label4 = new Label();
            ((System.ComponentModel.ISupportInitialize)_salesRecord).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(597, 113);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "検索";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(688, 113);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 1;
            button2.Text = "条件クリア";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(597, 426);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 2;
            button3.Text = "更新";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Location = new Point(688, 426);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 3;
            button4.Text = "閉じる";
            button4.UseVisualStyleBackColor = true;
            // 
            // _salesRecord
            // 
            _salesRecord.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _salesRecord.Location = new Point(12, 174);
            _salesRecord.Name = "_salesRecord";
            _salesRecord.Size = new Size(760, 246);
            _salesRecord.TabIndex = 15;
            // 
            // chkCategoryD
            // 
            chkCategoryD.AutoSize = true;
            chkCategoryD.Location = new Point(173, 112);
            chkCategoryD.Name = "chkCategoryD";
            chkCategoryD.Size = new Size(71, 19);
            chkCategoryD.TabIndex = 24;
            chkCategoryD.Text = "再送待ち";
            chkCategoryD.UseVisualStyleBackColor = true;
            // 
            // chkCategoryC
            // 
            chkCategoryC.AutoSize = true;
            chkCategoryC.Location = new Point(105, 113);
            chkCategoryC.Name = "chkCategoryC";
            chkCategoryC.Size = new Size(73, 19);
            chkCategoryC.TabIndex = 23;
            chkCategoryC.Text = "処理済み";
            chkCategoryC.UseVisualStyleBackColor = true;
            // 
            // chkCategoryB
            // 
            chkCategoryB.AutoSize = true;
            chkCategoryB.Location = new Point(173, 82);
            chkCategoryB.Name = "chkCategoryB";
            chkCategoryB.Size = new Size(50, 19);
            chkCategoryB.TabIndex = 22;
            chkCategoryB.Text = "受信";
            chkCategoryB.UseVisualStyleBackColor = true;
            // 
            // chkCategoryA
            // 
            chkCategoryA.AutoSize = true;
            chkCategoryA.Location = new Point(105, 82);
            chkCategoryA.Name = "chkCategoryA";
            chkCategoryA.Size = new Size(50, 19);
            chkCategoryA.TabIndex = 21;
            chkCategoryA.Text = "送信";
            chkCategoryA.UseVisualStyleBackColor = true;
            // 
            // txtStartDate
            // 
            txtStartDate.Location = new Point(105, 39);
            txtStartDate.Name = "txtStartDate";
            txtStartDate.Size = new Size(79, 23);
            txtStartDate.TabIndex = 19;
            // 
            // txtEndDate
            // 
            txtEndDate.Location = new Point(211, 39);
            txtEndDate.Name = "txtEndDate";
            txtEndDate.Size = new Size(87, 23);
            txtEndDate.TabIndex = 20;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(38, 113);
            label3.Name = "label3";
            label3.Size = new Size(51, 15);
            label3.TabIndex = 18;
            label3.Text = "ステータス";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(38, 82);
            label2.Name = "label2";
            label2.Size = new Size(31, 15);
            label2.TabIndex = 17;
            label2.Text = "分類";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(38, 42);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 16;
            label1.Text = "期間";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(260, 112);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(50, 19);
            checkBox1.TabIndex = 25;
            checkBox1.Text = "異常";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(38, 9);
            label4.Name = "label4";
            label4.Size = new Size(55, 15);
            label4.TabIndex = 26;
            label4.Text = "検索条件";
            // 
            // Transmission
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 461);
            Controls.Add(label4);
            Controls.Add(checkBox1);
            Controls.Add(chkCategoryD);
            Controls.Add(chkCategoryC);
            Controls.Add(chkCategoryB);
            Controls.Add(chkCategoryA);
            Controls.Add(txtStartDate);
            Controls.Add(txtEndDate);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(_salesRecord);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Transmission";
            Text = "送受信管理";
            ((System.ComponentModel.ISupportInitialize)_salesRecord).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private DataGridView _salesRecord;
        private CheckBox chkCategoryD;
        private CheckBox chkCategoryC;
        private CheckBox chkCategoryB;
        private CheckBox chkCategoryA;
        private TextBox txtStartDate;
        private TextBox txtEndDate;
        private Label label3;
        private Label label2;
        private Label label1;
        private CheckBox checkBox1;
        private Label label4;
    }
}