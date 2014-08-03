namespace CeilingDesigner
{
    partial class HelpCtrl
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.KnowLabel = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(37, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(599, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "1.通过菜单 图纸->保存为图片，或者在平面图上单击鼠标右键，然后点导出图纸，可以将当前平面图导成图片。";
            // 
            // label2
            // 
            this.label2.AutoEllipsis = true;
            this.label2.Location = new System.Drawing.Point(37, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(610, 45);
            this.label2.TabIndex = 1;
            this.label2.Text = "3.如果图形中斜边（非水平方向，也非竖直方向），请先调整水平方向和竖直方向的边，当这些边调整好之后，通常斜边也就调整好了。";
            // 
            // KnowLabel
            // 
            this.KnowLabel.AutoSize = true;
            this.KnowLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.KnowLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.KnowLabel.Location = new System.Drawing.Point(499, 133);
            this.KnowLabel.Name = "KnowLabel";
            this.KnowLabel.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.KnowLabel.Size = new System.Drawing.Size(73, 22);
            this.KnowLabel.TabIndex = 2;
            this.KnowLabel.Text = "我知道了";
            this.KnowLabel.Click += new System.EventHandler(this.KnowLabel_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(116, 132);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(96, 16);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "以后不再显示";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(425, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "2.当平面图上因为移动、删除产品，留下空白时，刷新一下图形，将自动补全。";
            // 
            // HelpCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.KnowLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "HelpCtrl";
            this.Size = new System.Drawing.Size(680, 175);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label KnowLabel;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label3;
    }
}
