namespace CeilingDesigner
{
    partial class ProductManage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductManage));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.添加NToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除DToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.重命名SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.添加NToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.粘贴PToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.新建NToolStripButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.添加类别ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.添加产品ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.编辑toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.删除toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.剪切UToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.复制CToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.粘贴PToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.帮助LToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.保存SToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStrip4 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.重命名MToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除DToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.复制CToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.剪切XToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            //((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.contextMenuStrip3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip4.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 27);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer1.Panel2.ContextMenuStrip = this.contextMenuStrip3;
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2.Click += new System.EventHandler(this.panel1_Click);
            this.splitContainer1.Size = new System.Drawing.Size(865, 639);
            this.splitContainer1.SplitterDistance = 235;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.ContextMenuStrip = this.contextMenuStrip2;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.FullRowSelect = true;
            this.treeView1.LabelEdit = true;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(863, 233);
            this.treeView1.TabIndex = 0;
            this.treeView1.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_BeforeLabelEdit);
            this.treeView1.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_AfterLabelEdit);
            this.treeView1.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterExpand);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.添加NToolStripMenuItem,
            this.删除DToolStripMenuItem1,
            this.重命名SToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(125, 70);
            // 
            // 添加NToolStripMenuItem
            // 
            this.添加NToolStripMenuItem.Name = "添加NToolStripMenuItem";
            this.添加NToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.添加NToolStripMenuItem.Text = "添加(&N)";
            this.添加NToolStripMenuItem.Click += new System.EventHandler(this.添加分组ToolStripMenuItem_Click);
            // 
            // 删除DToolStripMenuItem1
            // 
            this.删除DToolStripMenuItem1.Name = "删除DToolStripMenuItem1";
            this.删除DToolStripMenuItem1.Size = new System.Drawing.Size(124, 22);
            this.删除DToolStripMenuItem1.Text = "删除(&D)";
            this.删除DToolStripMenuItem1.Click += new System.EventHandler(this.删除toolStripButton2_Click);
            // 
            // 重命名SToolStripMenuItem
            // 
            this.重命名SToolStripMenuItem.Name = "重命名SToolStripMenuItem";
            this.重命名SToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.重命名SToolStripMenuItem.Text = "重命名(&M)";
            this.重命名SToolStripMenuItem.Click += new System.EventHandler(this.编辑toolStripButton1_Click);
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.添加NToolStripMenuItem1,
            this.粘贴PToolStripMenuItem1});
            this.contextMenuStrip3.Name = "contextMenuStrip3";
            this.contextMenuStrip3.Size = new System.Drawing.Size(113, 48);
            // 
            // 添加NToolStripMenuItem1
            // 
            this.添加NToolStripMenuItem1.Name = "添加NToolStripMenuItem1";
            this.添加NToolStripMenuItem1.Size = new System.Drawing.Size(112, 22);
            this.添加NToolStripMenuItem1.Text = "添加(&N)";
            this.添加NToolStripMenuItem1.Click += new System.EventHandler(this.添加产品ToolStripMenuItem_Click);
            // 
            // 粘贴PToolStripMenuItem1
            // 
            this.粘贴PToolStripMenuItem1.Name = "粘贴PToolStripMenuItem1";
            this.粘贴PToolStripMenuItem1.Size = new System.Drawing.Size(112, 22);
            this.粘贴PToolStripMenuItem1.Text = "粘贴(&P)";
            this.粘贴PToolStripMenuItem1.Visible = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.progressBar1);
            this.panel2.Location = new System.Drawing.Point(367, 60);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(335, 107);
            this.panel2.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(99, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "正在保存，请稍候...";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(14, 25);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(303, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.ContextMenuStrip = this.contextMenuStrip3;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(332, 398);
            this.panel1.TabIndex = 1;
            this.panel1.Click += new System.EventHandler(this.panel1_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建NToolStripButton,
            this.编辑toolStripButton1,
            this.删除toolStripButton2,
            this.剪切UToolStripButton,
            this.复制CToolStripButton,
            this.粘贴PToolStripButton,
            this.toolStripSeparator1,
            this.帮助LToolStripButton,
            this.保存SToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(865, 27);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // 新建NToolStripButton
            // 
            this.新建NToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.新建NToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.添加类别ToolStripMenuItem,
            this.添加产品ToolStripMenuItem});
            this.新建NToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("新建NToolStripButton.Image")));
            this.新建NToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.新建NToolStripButton.Name = "新建NToolStripButton";
            this.新建NToolStripButton.Size = new System.Drawing.Size(33, 24);
            this.新建NToolStripButton.Text = "新建";
            this.新建NToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // 添加类别ToolStripMenuItem
            // 
            this.添加类别ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("添加类别ToolStripMenuItem.Image")));
            this.添加类别ToolStripMenuItem.Name = "添加类别ToolStripMenuItem";
            this.添加类别ToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.添加类别ToolStripMenuItem.Text = "添加分组";
            this.添加类别ToolStripMenuItem.Click += new System.EventHandler(this.添加分组ToolStripMenuItem_Click);
            // 
            // 添加产品ToolStripMenuItem
            // 
            this.添加产品ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("添加产品ToolStripMenuItem.Image")));
            this.添加产品ToolStripMenuItem.Name = "添加产品ToolStripMenuItem";
            this.添加产品ToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.添加产品ToolStripMenuItem.Text = "添加产品";
            this.添加产品ToolStripMenuItem.Click += new System.EventHandler(this.添加产品ToolStripMenuItem_Click);
            // 
            // 编辑toolStripButton1
            // 
            this.编辑toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.编辑toolStripButton1.Image = global::CeilingDesigner.Properties.Resources.graphic_design;
            this.编辑toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.编辑toolStripButton1.Name = "编辑toolStripButton1";
            this.编辑toolStripButton1.Size = new System.Drawing.Size(24, 24);
            this.编辑toolStripButton1.Text = "toolStripButton1";
            this.编辑toolStripButton1.Click += new System.EventHandler(this.编辑toolStripButton1_Click);
            // 
            // 删除toolStripButton2
            // 
            this.删除toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.删除toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("删除toolStripButton2.Image")));
            this.删除toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.删除toolStripButton2.Name = "删除toolStripButton2";
            this.删除toolStripButton2.Size = new System.Drawing.Size(24, 24);
            this.删除toolStripButton2.Text = "删除";
            this.删除toolStripButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.删除toolStripButton2.Click += new System.EventHandler(this.删除toolStripButton2_Click);
            // 
            // 剪切UToolStripButton
            // 
            this.剪切UToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.剪切UToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("剪切UToolStripButton.Image")));
            this.剪切UToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.剪切UToolStripButton.Name = "剪切UToolStripButton";
            this.剪切UToolStripButton.Size = new System.Drawing.Size(24, 24);
            this.剪切UToolStripButton.Text = "剪切";
            this.剪切UToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.剪切UToolStripButton.Visible = false;
            this.剪切UToolStripButton.Click += new System.EventHandler(this.剪切UToolStripButton_Click);
            // 
            // 复制CToolStripButton
            // 
            this.复制CToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.复制CToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("复制CToolStripButton.Image")));
            this.复制CToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.复制CToolStripButton.Name = "复制CToolStripButton";
            this.复制CToolStripButton.Size = new System.Drawing.Size(24, 24);
            this.复制CToolStripButton.Text = "复制";
            this.复制CToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.复制CToolStripButton.Visible = false;
            this.复制CToolStripButton.Click += new System.EventHandler(this.复制CToolStripButton_Click);
            // 
            // 粘贴PToolStripButton
            // 
            this.粘贴PToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.粘贴PToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("粘贴PToolStripButton.Image")));
            this.粘贴PToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.粘贴PToolStripButton.Name = "粘贴PToolStripButton";
            this.粘贴PToolStripButton.Size = new System.Drawing.Size(24, 24);
            this.粘贴PToolStripButton.Text = "粘贴";
            this.粘贴PToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.粘贴PToolStripButton.Visible = false;
            this.粘贴PToolStripButton.Click += new System.EventHandler(this.粘贴PToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            this.toolStripSeparator1.Visible = false;
            // 
            // 帮助LToolStripButton
            // 
            this.帮助LToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.帮助LToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("帮助LToolStripButton.Image")));
            this.帮助LToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.帮助LToolStripButton.Name = "帮助LToolStripButton";
            this.帮助LToolStripButton.Size = new System.Drawing.Size(24, 24);
            this.帮助LToolStripButton.Text = "帮助(&L)";
            this.帮助LToolStripButton.Visible = false;
            // 
            // 保存SToolStripButton
            // 
            this.保存SToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.保存SToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("保存SToolStripButton.Image")));
            this.保存SToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.保存SToolStripButton.Name = "保存SToolStripButton";
            this.保存SToolStripButton.Size = new System.Drawing.Size(24, 24);
            this.保存SToolStripButton.Text = "保存";
            this.保存SToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.保存SToolStripButton.Click += new System.EventHandler(this.保存SToolStripButton_Click);
            // 
            // contextMenuStrip4
            // 
            this.contextMenuStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.重命名MToolStripMenuItem,
            this.删除DToolStripMenuItem2,
            this.复制CToolStripMenuItem1,
            this.剪切XToolStripMenuItem1});
            this.contextMenuStrip4.Name = "contextMenuStrip4";
            this.contextMenuStrip4.Size = new System.Drawing.Size(113, 92);
            // 
            // 重命名MToolStripMenuItem
            // 
            this.重命名MToolStripMenuItem.Name = "重命名MToolStripMenuItem";
            this.重命名MToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.重命名MToolStripMenuItem.Text = "编辑(&E)";
            this.重命名MToolStripMenuItem.Click += new System.EventHandler(this.编辑toolStripButton1_Click);
            // 
            // 删除DToolStripMenuItem2
            // 
            this.删除DToolStripMenuItem2.Name = "删除DToolStripMenuItem2";
            this.删除DToolStripMenuItem2.Size = new System.Drawing.Size(112, 22);
            this.删除DToolStripMenuItem2.Text = "删除(&D)";
            this.删除DToolStripMenuItem2.Click += new System.EventHandler(this.删除toolStripButton2_Click);
            // 
            // 复制CToolStripMenuItem1
            // 
            this.复制CToolStripMenuItem1.Name = "复制CToolStripMenuItem1";
            this.复制CToolStripMenuItem1.Size = new System.Drawing.Size(112, 22);
            this.复制CToolStripMenuItem1.Text = "复制(&C)";
            this.复制CToolStripMenuItem1.Visible = false;
            // 
            // 剪切XToolStripMenuItem1
            // 
            this.剪切XToolStripMenuItem1.Name = "剪切XToolStripMenuItem1";
            this.剪切XToolStripMenuItem1.Size = new System.Drawing.Size(112, 22);
            this.剪切XToolStripMenuItem1.Text = "剪切(&X)";
            this.剪切XToolStripMenuItem1.Visible = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "图片文件(*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|所有文件(*.*)|*.*" +
                "";
            this.openFileDialog1.Title = "打开产品图片";
            // 
            // ProductManage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(865, 666);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProductManage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "产品数据管理 | 奥普1+N浴顶设计";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.contextMenuStrip3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton 删除toolStripButton2;
        private System.Windows.Forms.ToolStripButton 剪切UToolStripButton;
        private System.Windows.Forms.ToolStripButton 复制CToolStripButton;
        private System.Windows.Forms.ToolStripButton 粘贴PToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton 帮助LToolStripButton;
        private System.Windows.Forms.ToolStripDropDownButton 新建NToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem 添加类别ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 添加产品ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem 添加NToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除DToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ToolStripMenuItem 添加NToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 粘贴PToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip4;
        private System.Windows.Forms.ToolStripMenuItem 删除DToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem 复制CToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 剪切XToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 重命名SToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 重命名MToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton 编辑toolStripButton1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripButton 保存SToolStripButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
    }
}