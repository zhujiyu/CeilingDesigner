using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CeilingDesigner
{
    public partial class OrderForm : Form
    {
        PalaceForm palaceForm = null;
        Order order = null;

        public OrderForm(PalaceForm pfrm, Order _order, TextBox customer)
        {
            InitializeComponent();

            this.order = _order;
            this.palaceForm = pfrm;
            this.orderInfo.CustomerBox = customer;

            this.good_viewBindingSource.DataSource = ShareData.CeilingDataSet;
            this.good_viewBindingSource.DataMember = "good_view";
        }

        private void DisplayGraph(OrderGraph graph)
        {
            TabPage page = new TabPage();
            page.Text = graph.Ceiling.Name;
            page.Tag = graph;
            page.Paint += new PaintEventHandler(TabPage_Paint);
            page.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tabControl1.TabPages.Add(page);
        }

        private void TabPage_Paint(object sender, PaintEventArgs e)
        {
            TabPage page = sender as TabPage;
            OrderGraph graph = page.Tag as OrderGraph;
            graph.DisplayGraph(e.Graphics, page.ClientRectangle, 80);
        }

        public void SetData(CeilingDataSet.ordersRow ordersRow)
        {
            List<OrderGraph> graphs = order.OrderGraphs;

            this.tabControl1.Controls.Clear();
            for (int i = 0; i < graphs.Count; i++)
            {
                this.DisplayGraph(graphs[i]);
            }

            this.orderInfo.InitInfo(ordersRow);
        }

        private void good_viewDataGridView_CellValueChanged(object sender, 
            DataGridViewCellEventArgs e)
        {
            DataGridViewCell current = this.good_viewDataGridView.CurrentCell;
            if (current == null)
                return;

            try
            {
                if (current.ColumnIndex == e.ColumnIndex
                    && current.RowIndex == e.RowIndex)
                    this.order.Change();
                if (e.ColumnIndex == 7 || e.ColumnIndex == 8)
                    this.order.SumGood();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private void good_viewDataGridView_SortCompare(object sender, 
            DataGridViewSortCompareEventArgs e)
        {
            DataGridViewRow r1 = this.good_viewDataGridView.Rows[e.RowIndex1];
            DataGridViewRow r2 = this.good_viewDataGridView.Rows[e.RowIndex2];

            if (!r1.IsNewRow && (int)r1.Cells["dataGridViewTextBoxColumn4"].Value == 0)
                r1.Frozen = true;
            if (!r2.IsNewRow && (int)r2.Cells["dataGridViewTextBoxColumn4"].Value == 0)
                r2.Frozen = true;
        }

        private void ReportButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.palaceForm.Report();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        // 保存在本地
        private void saveFileToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.order.SaveToFile())
                    this.palaceForm.Saved();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        // 保存到服务器
        private void saveDBToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.order.SaveToDB())
                    this.palaceForm.Saved();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        // 重新统计
        private void statisticToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                //palaceForm.Statistic();
                this.order.Statistic();
                this.SetData(this.order.OrderRow);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private void OrderForm1_Load(object sender, EventArgs e)
        {
            this.Text = "订单信息 - " + (this.order.OrderRow.IscustomerNull() ?
                "客户未命名" : this.order.OrderRow.customer) + " | " + ShareData.AppName;
            DataGridViewColumn dgvc = this.good_viewDataGridView.Columns["dataGridViewTextBoxColumn4"];
            this.good_viewDataGridView.Sort(dgvc, ListSortDirection.Descending);
        }
    }
}

//this.order.Statistic();
//ShareData.CeilingDataSet.orders.ordersRowChanged +=
//    new CeilingDataSet.ordersRowChangeEventHandler(orders_ordersRowChanged);

//void orders_ordersRowChanged(object sender,
//    CeilingDataSet.ordersRowChangeEvent e)
//{
//    this.orderInfo.InitInfo(this.order.OrderRow);
//}

//if (current != null)
//{
//    if (current.ColumnIndex == e.ColumnIndex 
//        && current.RowIndex == e.RowIndex)
//        this.order.Change();
//    if (e.ColumnIndex == 7 || e.ColumnIndex == 8)
//        this.order.SumGood();
//}

//ShareData.CeilingDataSet.orders.ordersRowChanged +=
//    new CeilingDataSet.ordersRowChangeEventHandler(orders_ordersRowChanged);

//void orders_ordersRowChanged(object sender,
//    CeilingDataSet.ordersRowChangeEvent e)
//{
//    try
//    {
//        if (this.IsDisposed || e.Row.IscustomerNull())
//            return;
//        this.Text = e.Row.customer + " - 订单信息";
//    }
//    catch (Exception ex)
//    {
//        System.Diagnostics.Debug.Write(ex);
//        MessageBox.Show(ex.Message, ex.Message);
//    }
//}

//this.good_viewDataGridView.Sort(
//    this.good_viewDataGridView.Columns["dataGridViewTextBoxColumn4"],
//    ListSortDirection.Descending);
