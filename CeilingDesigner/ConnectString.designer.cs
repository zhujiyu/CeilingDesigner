namespace CeilingDesigner
{
    partial class ConnectString
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectString));
            this.dbNameTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.pwordTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.unameTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.hostTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CancelButton1 = new System.Windows.Forms.Button();
            this.OKbutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dbNameTextBox
            // 
            this.dbNameTextBox.Font = new System.Drawing.Font("宋体", 12F);
            this.dbNameTextBox.Location = new System.Drawing.Point(109, 137);
            this.dbNameTextBox.Name = "dbNameTextBox";
            this.dbNameTextBox.Size = new System.Drawing.Size(200, 26);
            this.dbNameTextBox.TabIndex = 1117;
            this.dbNameTextBox.Text = "palace";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 12F);
            this.label7.Location = new System.Drawing.Point(32, 141);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 16);
            this.label7.TabIndex = 1121;
            this.label7.Text = "数据库：";
            // 
            // pwordTextBox
            // 
            this.pwordTextBox.Font = new System.Drawing.Font("宋体", 12F);
            this.pwordTextBox.Location = new System.Drawing.Point(109, 102);
            this.pwordTextBox.Name = "pwordTextBox";
            this.pwordTextBox.PasswordChar = '*';
            this.pwordTextBox.Size = new System.Drawing.Size(200, 26);
            this.pwordTextBox.TabIndex = 1116;
            this.pwordTextBox.Text = "palacer";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 12F);
            this.label8.Location = new System.Drawing.Point(32, 106);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 16);
            this.label8.TabIndex = 1120;
            this.label8.Text = "密码：";
            // 
            // unameTextBox
            // 
            this.unameTextBox.Font = new System.Drawing.Font("宋体", 12F);
            this.unameTextBox.Location = new System.Drawing.Point(109, 68);
            this.unameTextBox.Name = "unameTextBox";
            this.unameTextBox.Size = new System.Drawing.Size(200, 26);
            this.unameTextBox.TabIndex = 1115;
            this.unameTextBox.Text = "palacer";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F);
            this.label4.Location = new System.Drawing.Point(32, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 16);
            this.label4.TabIndex = 1119;
            this.label4.Text = "用户名：";
            // 
            // hostTextBox
            // 
            this.hostTextBox.Font = new System.Drawing.Font("宋体", 12F);
            this.hostTextBox.Location = new System.Drawing.Point(109, 32);
            this.hostTextBox.Name = "hostTextBox";
            this.hostTextBox.Size = new System.Drawing.Size(200, 26);
            this.hostTextBox.TabIndex = 1114;
            this.hostTextBox.Text = "ud60187.hichina.com";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F);
            this.label2.Location = new System.Drawing.Point(32, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 1118;
            this.label2.Text = "服务器：";
            // 
            // CancelButton1
            // 
            this.CancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton1.Location = new System.Drawing.Point(196, 184);
            this.CancelButton1.Name = "CancelButton1";
            this.CancelButton1.Size = new System.Drawing.Size(75, 23);
            this.CancelButton1.TabIndex = 1123;
            this.CancelButton1.Text = "取消";
            this.CancelButton1.UseVisualStyleBackColor = true;
            this.CancelButton1.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // OKbutton
            // 
            this.OKbutton.Location = new System.Drawing.Point(90, 184);
            this.OKbutton.Name = "OKbutton";
            this.OKbutton.Size = new System.Drawing.Size(75, 23);
            this.OKbutton.TabIndex = 1122;
            this.OKbutton.Text = "确定";
            this.OKbutton.UseVisualStyleBackColor = true;
            this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
            // 
            // ConnectString
            // 
            this.AcceptButton = this.OKbutton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelButton1;
            this.ClientSize = new System.Drawing.Size(352, 236);
            this.Controls.Add(this.CancelButton1);
            this.Controls.Add(this.OKbutton);
            this.Controls.Add(this.dbNameTextBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.pwordTextBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.unameTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.hostTextBox);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectString";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "服务器连接";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox dbNameTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox pwordTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox unameTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox hostTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button CancelButton1;
        private System.Windows.Forms.Button OKbutton;
    }
}