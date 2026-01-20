namespace Management
{
    partial class SalesData
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
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            txtStartDate = new TextBox();
            txtEndDate = new TextBox();
            label4 = new Label();
            chkCategoryA = new CheckBox();
            chkCategoryB = new CheckBox();
            chkCategoryC = new CheckBox();
            chkCategoryD = new CheckBox();
            txtProductName = new TextBox();
            txtProductId = new TextBox();
            btnSearch = new Button();
            btnClear = new Button();
            _salesRecord = new DataGridView();
            btnSendSalesData = new Button();
            btnRefresh = new Button();
            button2 = new Button();
            ((System.ComponentModel.ISupportInitialize)_salesRecord).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(38, 34);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 0;
            label1.Text = "期間";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(38, 61);
            label2.Name = "label2";
            label2.Size = new Size(55, 15);
            label2.TabIndex = 1;
            label2.Text = "商品分類";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(38, 89);
            label3.Name = "label3";
            label3.Size = new Size(55, 15);
            label3.TabIndex = 2;
            label3.Text = "商品番号";
            // 
            // txtStartDate
            // 
            txtStartDate.Location = new Point(105, 31);
            txtStartDate.Name = "txtStartDate";
            txtStartDate.Size = new Size(79, 23);
            txtStartDate.TabIndex = 3;
            // 
            // txtEndDate
            // 
            txtEndDate.Location = new Point(211, 31);
            txtEndDate.Name = "txtEndDate";
            txtEndDate.Size = new Size(87, 23);
            txtEndDate.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(38, 120);
            label4.Name = "label4";
            label4.Size = new Size(43, 15);
            label4.TabIndex = 5;
            label4.Text = "商品名";
            // 
            // chkCategoryA
            // 
            chkCategoryA.AutoSize = true;
            chkCategoryA.Location = new Point(105, 60);
            chkCategoryA.Name = "chkCategoryA";
            chkCategoryA.Size = new Size(62, 19);
            chkCategoryA.TabIndex = 6;
            chkCategoryA.Text = "食料品";
            chkCategoryA.UseVisualStyleBackColor = true;
            // 
            // chkCategoryB
            // 
            chkCategoryB.AutoSize = true;
            chkCategoryB.Location = new Point(173, 60);
            chkCategoryB.Name = "chkCategoryB";
            chkCategoryB.Size = new Size(50, 19);
            chkCategoryB.TabIndex = 7;
            chkCategoryB.Text = "機器";
            chkCategoryB.UseVisualStyleBackColor = true;
            // 
            // chkCategoryC
            // 
            chkCategoryC.AutoSize = true;
            chkCategoryC.Location = new Point(224, 60);
            chkCategoryC.Name = "chkCategoryC";
            chkCategoryC.Size = new Size(74, 19);
            chkCategoryC.TabIndex = 8;
            chkCategoryC.Text = "生活用品";
            chkCategoryC.UseVisualStyleBackColor = true;
            // 
            // chkCategoryD
            // 
            chkCategoryD.AutoSize = true;
            chkCategoryD.Location = new Point(304, 60);
            chkCategoryD.Name = "chkCategoryD";
            chkCategoryD.Size = new Size(81, 19);
            chkCategoryD.TabIndex = 9;
            chkCategoryD.Text = "その他用品";
            chkCategoryD.UseVisualStyleBackColor = true;
            // 
            // txtProductName
            // 
            txtProductName.Location = new Point(105, 120);
            txtProductName.Name = "txtProductName";
            txtProductName.Size = new Size(193, 23);
            txtProductName.TabIndex = 10;
            // 
            // txtProductId
            // 
            txtProductId.Location = new Point(105, 86);
            txtProductId.Name = "txtProductId";
            txtProductId.Size = new Size(193, 23);
            txtProductId.TabIndex = 11;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(439, 86);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(100, 49);
            btnSearch.TabIndex = 12;
            btnSearch.Text = "検索";
            btnSearch.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(601, 86);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(100, 49);
            btnClear.TabIndex = 13;
            btnClear.Text = "条件クリア";
            btnClear.UseVisualStyleBackColor = true;
            // 
            // _salesRecord
            // 
            _salesRecord.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _salesRecord.Location = new Point(12, 149);
            _salesRecord.Name = "_salesRecord";
            _salesRecord.Size = new Size(760, 246);
            _salesRecord.TabIndex = 14;
            // 
            // btnSendSalesData
            // 
            btnSendSalesData.Location = new Point(23, 404);
            btnSendSalesData.Name = "btnSendSalesData";
            btnSendSalesData.Size = new Size(172, 45);
            btnSendSalesData.TabIndex = 15;
            btnSendSalesData.Text = "売上データ送信ボタン";
            btnSendSalesData.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(439, 404);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(100, 45);
            btnRefresh.TabIndex = 16;
            btnRefresh.Text = "更新";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(601, 404);
            button2.Name = "button2";
            button2.Size = new Size(100, 45);
            button2.TabIndex = 17;
            button2.Text = "閉じる";
            button2.UseVisualStyleBackColor = true;
            // 
            // SalesData
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 461);
            Controls.Add(button2);
            Controls.Add(btnRefresh);
            Controls.Add(btnSendSalesData);
            Controls.Add(_salesRecord);
            Controls.Add(btnClear);
            Controls.Add(btnSearch);
            Controls.Add(txtProductId);
            Controls.Add(txtProductName);
            Controls.Add(chkCategoryD);
            Controls.Add(chkCategoryC);
            Controls.Add(chkCategoryB);
            Controls.Add(chkCategoryA);
            Controls.Add(label4);
            Controls.Add(txtStartDate);
            Controls.Add(txtEndDate);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "SalesData";
            Text = "売上管理";
            Load += SalesData_Load;
            ((System.ComponentModel.ISupportInitialize)_salesRecord).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox txtStartDate;
        private TextBox txtEndDate;
        private Label label4;
        private CheckBox chkCategoryA;
        private CheckBox chkCategoryB;
        private CheckBox chkCategoryC;
        private CheckBox chkCategoryD;
        private TextBox txtProductName;
        private TextBox txtProductId;
        private Button btnSearch;
        private Button btnClear;
        private DataGridView _salesRecord;
        private Button btnSendSalesData;
        private Button btnRefresh;
        private Button button2;
        private DataGridViewTextBoxColumn 販売日時;
    }
}