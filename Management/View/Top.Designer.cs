namespace Management
{
    partial class Top
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
            売り上げ管理 = new Button();
            button2 = new Button();
            label1 = new Label();
            listBox1 = new ListBox();
            SuspendLayout();
            // 
            // 売り上げ管理
            // 
            売り上げ管理.Location = new Point(219, 182);
            売り上げ管理.Name = "売り上げ管理";
            売り上げ管理.Size = new Size(113, 55);
            売り上げ管理.TabIndex = 0;
            売り上げ管理.Text = "売り上げ管理";
            売り上げ管理.UseVisualStyleBackColor = true;
            売り上げ管理.Click += 売り上げ管理_Click;
            // 
            // button2
            // 
            button2.Location = new Point(456, 182);
            button2.Name = "button2";
            button2.Size = new Size(113, 55);
            button2.TabIndex = 1;
            button2.Text = "送受信管理";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.Location = new Point(330, 136);
            label1.Margin = new Padding(3);
            label1.Name = "label1";
            label1.Size = new Size(125, 20);
            label1.TabIndex = 2;
            label1.Text = "売り上げ管理システム";
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(143, 86);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(520, 199);
            listBox1.TabIndex = 3;
            // 
            // Top
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 461);
            Controls.Add(label1);
            Controls.Add(button2);
            Controls.Add(売り上げ管理);
            Controls.Add(listBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Top";
            Text = "トップ画面";
            ResumeLayout(false);
        }

        #endregion

        private Button 売り上げ管理;
        private Button button2;
        private Label label1;
        private ListBox listBox1;
    }
}