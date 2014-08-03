namespace CeilingDesigner
{
    partial class AddCeiling
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
            CeilingDesigner.Ceiling ceiling1 = new CeilingDesigner.Ceiling();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddCeiling));
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.ceilingsamplesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.palaceDataSet = new CeilingDesigner.CeilingDataSet();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.NameTextBox1 = new System.Windows.Forms.TextBox();
            this.depthTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.CTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.BTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.ATextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.FTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.ETextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.DTextBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.ceilSample1 = new CeilingDesigner.CeilSample();
            this.OKbutton1 = new System.Windows.Forms.Button();
            this.CancelButton2 = new System.Windows.Forms.Button();
            this.HTextBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.GTextBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ceilingsamplesBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.palaceDataSet)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.DataSource = this.ceilingsamplesBindingSource;
            this.comboBox1.DisplayMember = "name";
            this.comboBox1.Font = new System.Drawing.Font("宋体", 12F);
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(145, 17);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(177, 24);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.ValueMember = "ID";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // ceilingsamplesBindingSource
            // 
            this.ceilingsamplesBindingSource.DataMember = "ceiling_samples";
            this.ceilingsamplesBindingSource.DataSource = this.palaceDataSet;
            // 
            // palaceDataSet
            // 
            this.palaceDataSet.DataSetName = "palaceDataSet";
            this.palaceDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F);
            this.label1.Location = new System.Drawing.Point(22, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 112;
            this.label1.Text = "选择房屋类型：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F);
            this.label2.Location = new System.Drawing.Point(340, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 113;
            this.label2.Text = "名称：";
            // 
            // NameTextBox1
            // 
            this.NameTextBox1.Font = new System.Drawing.Font("宋体", 12F);
            this.NameTextBox1.Location = new System.Drawing.Point(401, 17);
            this.NameTextBox1.Name = "NameTextBox1";
            this.NameTextBox1.Size = new System.Drawing.Size(133, 26);
            this.NameTextBox1.TabIndex = 1;
            this.NameTextBox1.Text = "房间一";
            this.NameTextBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ATextBox_KeyUp);
            // 
            // depthTextBox
            // 
            this.depthTextBox.Font = new System.Drawing.Font("宋体", 12F);
            this.depthTextBox.Location = new System.Drawing.Point(401, 53);
            this.depthTextBox.Name = "depthTextBox";
            this.depthTextBox.Size = new System.Drawing.Size(133, 26);
            this.depthTextBox.TabIndex = 2;
            this.depthTextBox.Text = "280";
            this.depthTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ATextBox_KeyUp);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F);
            this.label4.Location = new System.Drawing.Point(340, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 16);
            this.label4.TabIndex = 119;
            this.label4.Text = "夹高：";
            // 
            // CTextBox
            // 
            this.CTextBox.Enabled = false;
            this.CTextBox.Font = new System.Drawing.Font("宋体", 12F);
            this.CTextBox.Location = new System.Drawing.Point(401, 157);
            this.CTextBox.Name = "CTextBox";
            this.CTextBox.Size = new System.Drawing.Size(133, 26);
            this.CTextBox.TabIndex = 5;
            this.CTextBox.Text = "0";
            this.CTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ATextBox_KeyUp);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 12F);
            this.label6.Location = new System.Drawing.Point(340, 161);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 16);
            this.label6.TabIndex = 1115;
            this.label6.Text = "C边长：";
            // 
            // BTextBox
            // 
            this.BTextBox.Font = new System.Drawing.Font("宋体", 12F);
            this.BTextBox.Location = new System.Drawing.Point(401, 122);
            this.BTextBox.Name = "BTextBox";
            this.BTextBox.Size = new System.Drawing.Size(133, 26);
            this.BTextBox.TabIndex = 4;
            this.BTextBox.Text = "3000";
            this.BTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ATextBox_KeyUp);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 12F);
            this.label7.Location = new System.Drawing.Point(340, 126);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 16);
            this.label7.TabIndex = 1113;
            this.label7.Text = "B边长：";
            // 
            // ATextBox
            // 
            this.ATextBox.Font = new System.Drawing.Font("宋体", 12F);
            this.ATextBox.Location = new System.Drawing.Point(401, 87);
            this.ATextBox.Name = "ATextBox";
            this.ATextBox.Size = new System.Drawing.Size(133, 26);
            this.ATextBox.TabIndex = 3;
            this.ATextBox.Text = "2000";
            this.ATextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ATextBox_KeyUp);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 12F);
            this.label8.Location = new System.Drawing.Point(340, 91);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(64, 16);
            this.label8.TabIndex = 1111;
            this.label8.Text = "A边长：";
            // 
            // FTextBox
            // 
            this.FTextBox.Enabled = false;
            this.FTextBox.Font = new System.Drawing.Font("宋体", 12F);
            this.FTextBox.Location = new System.Drawing.Point(401, 261);
            this.FTextBox.Name = "FTextBox";
            this.FTextBox.Size = new System.Drawing.Size(133, 26);
            this.FTextBox.TabIndex = 8;
            this.FTextBox.Text = "0";
            this.FTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ATextBox_KeyUp);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 12F);
            this.label9.Location = new System.Drawing.Point(340, 265);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 16);
            this.label9.TabIndex = 1121;
            this.label9.Text = "F边长：";
            // 
            // ETextBox
            // 
            this.ETextBox.Enabled = false;
            this.ETextBox.Font = new System.Drawing.Font("宋体", 12F);
            this.ETextBox.Location = new System.Drawing.Point(401, 226);
            this.ETextBox.Name = "ETextBox";
            this.ETextBox.Size = new System.Drawing.Size(133, 26);
            this.ETextBox.TabIndex = 7;
            this.ETextBox.Text = "0";
            this.ETextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ATextBox_KeyUp);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 12F);
            this.label10.Location = new System.Drawing.Point(340, 230);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 16);
            this.label10.TabIndex = 1119;
            this.label10.Text = "E边长：";
            // 
            // DTextBox
            // 
            this.DTextBox.Enabled = false;
            this.DTextBox.Font = new System.Drawing.Font("宋体", 12F);
            this.DTextBox.Location = new System.Drawing.Point(401, 191);
            this.DTextBox.Name = "DTextBox";
            this.DTextBox.Size = new System.Drawing.Size(133, 26);
            this.DTextBox.TabIndex = 6;
            this.DTextBox.Text = "0";
            this.DTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ATextBox_KeyUp);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 12F);
            this.label11.Location = new System.Drawing.Point(340, 195);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(64, 16);
            this.label11.TabIndex = 1117;
            this.label11.Text = "D边长：";
            // 
            // ceilSample1
            // 
            this.ceilSample1.BackColor = System.Drawing.Color.White;
            this.ceilSample1.BackgroundImage = global::CeilingDesigner.Properties.Resources.gridback;
            ceiling1.Bottom = 0F;
            ceiling1.Depth = 30;
            ceiling1.ID = 0;
            ceiling1.Name = "未命名房间";
            ceiling1.OrderID = 0;
            ceiling1.Right = 0F;
            ceiling1.SelectedIndex = -1;
            ceiling1.SelectedWall = null;
            this.ceilSample1.Ceiling = ceiling1;
            this.ceilSample1.Location = new System.Drawing.Point(20, 54);
            this.ceilSample1.Name = "ceilSample1";
            this.ceilSample1.SampleName = null;
            this.ceilSample1.Size = new System.Drawing.Size(300, 300);
            this.ceilSample1.TabIndex = 23;
            this.ceilSample1.WallesTable = null;
            this.ceilSample1.Paint += new System.Windows.Forms.PaintEventHandler(this.ceilSample1_Paint);
            // 
            // OKbutton1
            // 
            this.OKbutton1.Location = new System.Drawing.Point(170, 374);
            this.OKbutton1.Name = "OKbutton1";
            this.OKbutton1.Size = new System.Drawing.Size(75, 23);
            this.OKbutton1.TabIndex = 11;
            this.OKbutton1.Text = "确定";
            this.OKbutton1.UseVisualStyleBackColor = true;
            this.OKbutton1.Click += new System.EventHandler(this.OKbutton1_Click);
            // 
            // CancelButton2
            // 
            this.CancelButton2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton2.Location = new System.Drawing.Point(276, 374);
            this.CancelButton2.Name = "CancelButton2";
            this.CancelButton2.Size = new System.Drawing.Size(75, 23);
            this.CancelButton2.TabIndex = 12;
            this.CancelButton2.Text = "取消";
            this.CancelButton2.UseVisualStyleBackColor = true;
            this.CancelButton2.Click += new System.EventHandler(this.CancelButton2_Click);
            // 
            // HTextBox
            // 
            this.HTextBox.Enabled = false;
            this.HTextBox.Font = new System.Drawing.Font("宋体", 12F);
            this.HTextBox.Location = new System.Drawing.Point(400, 331);
            this.HTextBox.Name = "HTextBox";
            this.HTextBox.Size = new System.Drawing.Size(133, 26);
            this.HTextBox.TabIndex = 10;
            this.HTextBox.Text = "0";
            this.HTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ATextBox_KeyUp);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 12F);
            this.label12.Location = new System.Drawing.Point(340, 335);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 16);
            this.label12.TabIndex = 1128;
            this.label12.Text = "H边长：";
            // 
            // GTextBox
            // 
            this.GTextBox.Enabled = false;
            this.GTextBox.Font = new System.Drawing.Font("宋体", 12F);
            this.GTextBox.Location = new System.Drawing.Point(401, 296);
            this.GTextBox.Name = "GTextBox";
            this.GTextBox.Size = new System.Drawing.Size(133, 26);
            this.GTextBox.TabIndex = 9;
            this.GTextBox.Text = "0";
            this.GTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ATextBox_KeyUp);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("宋体", 12F);
            this.label13.Location = new System.Drawing.Point(340, 300);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(64, 16);
            this.label13.TabIndex = 1126;
            this.label13.Text = "G边长：";
            // 
            // AddCeiling
            // 
            this.AcceptButton = this.OKbutton1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelButton2;
            this.ClientSize = new System.Drawing.Size(551, 411);
            this.Controls.Add(this.HTextBox);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.GTextBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.CancelButton2);
            this.Controls.Add(this.OKbutton1);
            this.Controls.Add(this.ceilSample1);
            this.Controls.Add(this.FTextBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.ETextBox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.DTextBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.CTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.BTextBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ATextBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.depthTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.NameTextBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddCeiling";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "添加平面图";
            this.Load += new System.EventHandler(this.AddCeiling_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ceilingsamplesBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.palaceDataSet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox NameTextBox1;
        private System.Windows.Forms.TextBox depthTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox CTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox BTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox ATextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox FTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox ETextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox DTextBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.BindingSource ceilingsamplesBindingSource;
        private CeilSample ceilSample1;
        private System.Windows.Forms.Button OKbutton1;
        private System.Windows.Forms.Button CancelButton2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox GTextBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox HTextBox;
        private CeilingDataSet palaceDataSet;
    }
}