namespace CeilingDesigner
{
    partial class OrderForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderForm));
            this.good_viewDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.good_viewBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.ceilingDataSet = new CeilingDesigner.CeilingDataSet();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.reportToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.statisticToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveFileToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveDBToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.orderInfo = new CeilingDesigner.OrderInfo();
            ((System.ComponentModel.ISupportInitialize)(this.good_viewDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.good_viewBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceilingDataSet)).BeginInit();
            //((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // good_viewDataGridView
            // 
            this.good_viewDataGridView.AllowUserToAddRows = false;
            this.good_viewDataGridView.AllowUserToDeleteRows = false;
            this.good_viewDataGridView.AutoGenerateColumns = false;
            this.good_viewDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.good_viewDataGridView.BackgroundColor = System.Drawing.Color.White;
            this.good_viewDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.good_viewDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn10,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn13,
            this.dataGridViewTextBoxColumn9,
            this.dataGridViewTextBoxColumn8,
            this.dataGridViewTextBoxColumn11,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn12});
            this.good_viewDataGridView.DataSource = this.good_viewBindingSource;
            this.good_viewDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.good_viewDataGridView.GridColor = System.Drawing.Color.LightGray;
            this.good_viewDataGridView.Location = new System.Drawing.Point(0, 0);
            this.good_viewDataGridView.Name = "good_viewDataGridView";
            this.good_viewDataGridView.RowTemplate.Height = 23;
            this.good_viewDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.good_viewDataGridView.Size = new System.Drawing.Size(872, 313);
            this.good_viewDataGridView.TabIndex = 1;
            this.good_viewDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.good_viewDataGridView_CellValueChanged);
            this.good_viewDataGridView.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.good_viewDataGridView_SortCompare);
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "order_id";
            this.dataGridViewTextBoxColumn2.HeaderText = "订单ID";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Visible = false;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "name";
            this.dataGridViewTextBoxColumn3.FillWeight = 36.24366F;
            this.dataGridViewTextBoxColumn3.HeaderText = "产品名称";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "category";
            this.dataGridViewTextBoxColumn5.FillWeight = 18.12183F;
            this.dataGridViewTextBoxColumn5.HeaderText = "类别";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "product_id";
            this.dataGridViewTextBoxColumn4.FillWeight = 43.14721F;
            this.dataGridViewTextBoxColumn4.HeaderText = "产品ID";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Visible = false;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.DataPropertyName = "model";
            this.dataGridViewTextBoxColumn10.FillWeight = 18.12183F;
            this.dataGridViewTextBoxColumn10.HeaderText = "规格";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.DataPropertyName = "pattern";
            this.dataGridViewTextBoxColumn7.HeaderText = "型号";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.Visible = false;
            // 
            // dataGridViewTextBoxColumn13
            // 
            this.dataGridViewTextBoxColumn13.DataPropertyName = "unit";
            this.dataGridViewTextBoxColumn13.HeaderText = "单位";
            this.dataGridViewTextBoxColumn13.Name = "dataGridViewTextBoxColumn13";
            this.dataGridViewTextBoxColumn13.Visible = false;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.DataPropertyName = "amount";
            this.dataGridViewTextBoxColumn9.FillWeight = 18.12183F;
            this.dataGridViewTextBoxColumn9.HeaderText = "数量";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.DataPropertyName = "price";
            this.dataGridViewTextBoxColumn8.FillWeight = 18.12183F;
            this.dataGridViewTextBoxColumn8.HeaderText = "单价(元)";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn11
            // 
            this.dataGridViewTextBoxColumn11.DataPropertyName = "total";
            this.dataGridViewTextBoxColumn11.FillWeight = 18.12183F;
            this.dataGridViewTextBoxColumn11.HeaderText = "小计(元)";
            this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            this.dataGridViewTextBoxColumn11.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.DataPropertyName = "color";
            this.dataGridViewTextBoxColumn6.HeaderText = "颜色";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.Visible = false;
            // 
            // dataGridViewTextBoxColumn12
            // 
            this.dataGridViewTextBoxColumn12.DataPropertyName = "remark";
            this.dataGridViewTextBoxColumn12.HeaderText = "备注";
            this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            this.dataGridViewTextBoxColumn12.Visible = false;
            // 
            // good_viewBindingSource
            // 
            this.good_viewBindingSource.DataMember = "good_view";
            this.good_viewBindingSource.DataSource = this.ceilingDataSet;
            // 
            // ceilingDataSet
            // 
            this.ceilingDataSet.DataSetName = "palaceDataSet";
            this.ceilingDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.good_viewDataGridView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(872, 626);
            this.splitContainer1.SplitterDistance = 313;
            this.splitContainer1.TabIndex = 19;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(402, 309);
            this.tabControl1.TabIndex = 18;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Location = new System.Drawing.Point(4, 21);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(394, 284);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Controls.Add(this.orderInfo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(402, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(470, 309);
            this.panel1.TabIndex = 19;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reportToolStripButton,
            this.statisticToolStripButton,
            this.saveFileToolStripButton,
            this.saveDBToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(40, 240);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(282, 39);
            this.toolStrip1.TabIndex = 31;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // reportToolStripButton
            // 
            this.reportToolStripButton.Image = global::CeilingDesigner.Properties.Resources.report;
            this.reportToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reportToolStripButton.Name = "reportToolStripButton";
            this.reportToolStripButton.Size = new System.Drawing.Size(89, 36);
            this.reportToolStripButton.Text = "生成报表";
            this.reportToolStripButton.Click += new System.EventHandler(this.ReportButton_Click);
            // 
            // statisticToolStripButton
            // 
            this.statisticToolStripButton.Image = global::CeilingDesigner.Properties.Resources.stats2;
            this.statisticToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.statisticToolStripButton.Name = "statisticToolStripButton";
            this.statisticToolStripButton.Size = new System.Drawing.Size(89, 36);
            this.statisticToolStripButton.Text = "重新统计";
            this.statisticToolStripButton.Click += new System.EventHandler(this.statisticToolStripButton_Click);
            // 
            // saveFileToolStripButton
            // 
            this.saveFileToolStripButton.Image = global::CeilingDesigner.Properties.Resources.document_notes;
            this.saveFileToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveFileToolStripButton.Name = "saveFileToolStripButton";
            this.saveFileToolStripButton.Size = new System.Drawing.Size(101, 36);
            this.saveFileToolStripButton.Text = "保存在本地";
            this.saveFileToolStripButton.Click += new System.EventHandler(this.saveFileToolStripButton_Click);
            // 
            // saveDBToolStripButton
            // 
            this.saveDBToolStripButton.Image = global::CeilingDesigner.Properties.Resources.database_active;
            this.saveDBToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveDBToolStripButton.Name = "saveDBToolStripButton";
            this.saveDBToolStripButton.Size = new System.Drawing.Size(113, 36);
            this.saveDBToolStripButton.Text = "保存到服务器";
            this.saveDBToolStripButton.Visible = false;
            this.saveDBToolStripButton.Click += new System.EventHandler(this.saveDBToolStripButton_Click);
            // 
            // orderInfo
            // 
            this.orderInfo.CustomerBox = null;
            this.orderInfo.Location = new System.Drawing.Point(3, -1);
            this.orderInfo.Name = "orderInfo";
            this.orderInfo.OrderAdapter = null;
            this.orderInfo.OrderRow = null;
            this.orderInfo.Size = new System.Drawing.Size(450, 200);
            this.orderInfo.TabIndex = 0;
            // 
            // OrderForm1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(872, 626);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OrderForm1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "订单信息";
            this.Load += new System.EventHandler(this.OrderForm1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.good_viewDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.good_viewBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceilingDataSet)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView good_viewDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn13;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private System.Windows.Forms.BindingSource good_viewBindingSource;
        private CeilingDataSet ceilingDataSet;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton reportToolStripButton;
        private System.Windows.Forms.ToolStripButton statisticToolStripButton;
        private System.Windows.Forms.ToolStripButton saveFileToolStripButton;
        private System.Windows.Forms.ToolStripButton saveDBToolStripButton;
        private OrderInfo orderInfo;

    }
}