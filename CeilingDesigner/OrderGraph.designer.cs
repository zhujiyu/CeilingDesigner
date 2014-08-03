namespace CeilingDesigner
{
    partial class OrderGraph
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
            this.SuspendLayout();
            // 
            // OrderGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Name = "OrderGraph";
            this.Size = new System.Drawing.Size(800, 600);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Graph_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Graph_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Graph_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Graph_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Graph_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Graph_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion




    }
}
