namespace CeilingDesigner
{
    partial class WallEdit
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lenghTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.offsetTextBox = new System.Windows.Forms.TextBox();
            this.offsetLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.depthTextBox = new System.Windows.Forms.TextBox();
            this.remarkTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.BLabel = new System.Windows.Forms.Label();
            this.endTextBox = new System.Windows.Forms.TextBox();
            this.beginTextBox = new System.Windows.Forms.TextBox();
            this.ALabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lenghTextBox
            // 
            this.lenghTextBox.Font = new System.Drawing.Font("宋体", 9F);
            this.lenghTextBox.Location = new System.Drawing.Point(50, 20);
            this.lenghTextBox.Name = "lenghTextBox";
            this.lenghTextBox.Size = new System.Drawing.Size(64, 21);
            this.lenghTextBox.TabIndex = 10;
            this.lenghTextBox.Text = "0";
            this.lenghTextBox.Enter += new System.EventHandler(this.TextBox_Enter);
            this.lenghTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lenghTextBox_KeyPress);
            this.lenghTextBox.Leave += new System.EventHandler(this.lenghTextBox_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F);
            this.label1.Location = new System.Drawing.Point(10, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "长度：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.offsetTextBox);
            this.groupBox1.Controls.Add(this.offsetLabel);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lenghTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(300, 80);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "调整整边";
            // 
            // offsetTextBox
            // 
            this.offsetTextBox.Font = new System.Drawing.Font("宋体", 9F);
            this.offsetTextBox.Location = new System.Drawing.Point(210, 20);
            this.offsetTextBox.Name = "offsetTextBox";
            this.offsetTextBox.Size = new System.Drawing.Size(64, 21);
            this.offsetTextBox.TabIndex = 20;
            this.offsetTextBox.Text = "0";
            this.offsetTextBox.Enter += new System.EventHandler(this.TextBox_Enter);
            this.offsetTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OffsetTextBox_KeyPress);
            this.offsetTextBox.Leave += new System.EventHandler(this.OffsetTextBox_Leave);
            // 
            // offsetLabel
            // 
            this.offsetLabel.AutoSize = true;
            this.offsetLabel.Font = new System.Drawing.Font("宋体", 9F);
            this.offsetLabel.Location = new System.Drawing.Point(140, 24);
            this.offsetLabel.Name = "offsetLabel";
            this.offsetLabel.Size = new System.Drawing.Size(65, 12);
            this.offsetLabel.TabIndex = 19;
            this.offsetLabel.Text = "水平平移：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 12);
            this.label5.TabIndex = 18;
            this.label5.Text = "实际长度（毫米）";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.depthTextBox);
            this.groupBox3.Controls.Add(this.remarkTextBox);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox3.Location = new System.Drawing.Point(640, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(340, 80);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "其他属性";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 12);
            this.label3.TabIndex = 17;
            this.label3.Text = "龙骨的夹高（毫米）";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 9F);
            this.label4.Location = new System.Drawing.Point(10, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "夹高：";
            // 
            // depthTextBox
            // 
            this.depthTextBox.Font = new System.Drawing.Font("宋体", 9F);
            this.depthTextBox.Location = new System.Drawing.Point(50, 20);
            this.depthTextBox.Name = "depthTextBox";
            this.depthTextBox.Size = new System.Drawing.Size(64, 21);
            this.depthTextBox.TabIndex = 12;
            this.depthTextBox.Text = "280";
            this.depthTextBox.Enter += new System.EventHandler(this.TextBox_Enter);
            this.depthTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.depthTextBox_KeyPress);
            this.depthTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.depthTextBox_KeyUp);
            this.depthTextBox.Leave += new System.EventHandler(this.depthTextBox_Leave);
            // 
            // remarkTextBox
            // 
            this.remarkTextBox.Font = new System.Drawing.Font("宋体", 9F);
            this.remarkTextBox.Location = new System.Drawing.Point(180, 16);
            this.remarkTextBox.Multiline = true;
            this.remarkTextBox.Name = "remarkTextBox";
            this.remarkTextBox.Size = new System.Drawing.Size(148, 53);
            this.remarkTextBox.TabIndex = 15;
            this.remarkTextBox.Enter += new System.EventHandler(this.TextBox_Enter);
            this.remarkTextBox.Leave += new System.EventHandler(this.remarkTextBox_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 9F);
            this.label7.Location = new System.Drawing.Point(140, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "批注：";
            // 
            // BLabel
            // 
            this.BLabel.AutoSize = true;
            this.BLabel.Font = new System.Drawing.Font("宋体", 9F);
            this.BLabel.Location = new System.Drawing.Point(180, 24);
            this.BLabel.Name = "BLabel";
            this.BLabel.Size = new System.Drawing.Size(83, 12);
            this.BLabel.TabIndex = 14;
            this.BLabel.Text = "B(1000,1000):";
            // 
            // endTextBox
            // 
            this.endTextBox.Font = new System.Drawing.Font("宋体", 9F);
            this.endTextBox.Location = new System.Drawing.Point(266, 20);
            this.endTextBox.Name = "endTextBox";
            this.endTextBox.Size = new System.Drawing.Size(64, 21);
            this.endTextBox.TabIndex = 15;
            this.endTextBox.Text = "0, 0";
            this.endTextBox.Enter += new System.EventHandler(this.TextBox_Enter);
            this.endTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.endTextBox_KeyPress);
            this.endTextBox.Leave += new System.EventHandler(this.endTextBox_Leave);
            // 
            // beginTextBox
            // 
            this.beginTextBox.Font = new System.Drawing.Font("宋体", 9F);
            this.beginTextBox.Location = new System.Drawing.Point(96, 20);
            this.beginTextBox.Name = "beginTextBox";
            this.beginTextBox.Size = new System.Drawing.Size(64, 21);
            this.beginTextBox.TabIndex = 12;
            this.beginTextBox.Text = "0, 0";
            this.beginTextBox.Enter += new System.EventHandler(this.TextBox_Enter);
            this.beginTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.beginTextBox_KeyPress);
            this.beginTextBox.Leave += new System.EventHandler(this.beginTextBox_Leave);
            // 
            // ALabel
            // 
            this.ALabel.AutoSize = true;
            this.ALabel.Font = new System.Drawing.Font("宋体", 9F);
            this.ALabel.Location = new System.Drawing.Point(10, 24);
            this.ALabel.Name = "ALabel";
            this.ALabel.Size = new System.Drawing.Size(83, 12);
            this.ALabel.TabIndex = 11;
            this.ALabel.Text = "A(1000,1000):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 12);
            this.label2.TabIndex = 17;
            this.label2.Text = "端点实际坐标（单位毫米）";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.ALabel);
            this.groupBox2.Controls.Add(this.beginTextBox);
            this.groupBox2.Controls.Add(this.endTextBox);
            this.groupBox2.Controls.Add(this.BLabel);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox2.Location = new System.Drawing.Point(300, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(340, 80);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "调整端点";
            // 
            // WallEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "WallEdit";
            this.Size = new System.Drawing.Size(992, 80);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox lenghTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox offsetTextBox;
        private System.Windows.Forms.Label offsetLabel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox depthTextBox;
        private System.Windows.Forms.TextBox remarkTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label BLabel;
        private System.Windows.Forms.TextBox endTextBox;
        private System.Windows.Forms.TextBox beginTextBox;
        private System.Windows.Forms.Label ALabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}
