using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CeilingDesigner
{
    public partial class SelectOrderForm : Form
    {
        private int orderId = 0;

        public int OrderId
        {
            get { return orderId; }
        }

        public SelectOrderForm()
        {
            InitializeComponent();
        }

        private void ordersBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.ordersBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.palaceDataSet);
        }

        private void SelectOrderForm_Load(object sender, EventArgs e)
        {
            this.ordersTableAdapter.Connection = ShareData.Connection;
            // TODO: 这行代码将数据加载到表“palaceDataSet.orders”中。您可以根据需要移动或删除它。
            this.ordersTableAdapter.Fill(this.palaceDataSet.orders);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void QueryButton_Click(object sender, EventArgs e)
        {
            string queryString = "";
            if (this.numberTextBox4.Text != "")
                queryString += "(number like \"%" + this.numberTextBox4.Text + "%\") and ";
            if (this.customerTextBox1.Text != "")
                queryString += "(customer = \"%" + this.customerTextBox1.Text + "\") and ";
            if (this.saleManTextBox.Text != "")
                queryString += "(salesman = \"%" + this.saleManTextBox.Text + "\") and ";

            if (queryString.Length > 0)
            {
                queryString = "SELECT ID, number, customer, address, phone, salesman, sum_price, create_date, install_date FROM orders WHERE " + queryString;
                queryString = queryString.Substring(0, queryString.Length - 5);

                MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand();
                command.CommandText = queryString;// +"order by ID limit 0, 12";
                command.CommandType = CommandType.Text;
                command.Connection = ShareData.Connection;

                MySql.Data.MySqlClient.MySqlDataAdapter adapter = this.ordersTableAdapter.Adapter;
                this.palaceDataSet.orders.Clear();
                adapter.SelectCommand = command;
                adapter.Fill(this.palaceDataSet.orders);
            }
            else
                this.ordersTableAdapter.Fill(this.palaceDataSet.orders);
        }

        public void OpenOrder()
        {
            if (this.ordersDataGridView.SelectedRows.Count < 1)
                return;
            DataGridViewRow row = this.ordersDataGridView.SelectedRows[0];
            if (row == null)
                return;
            this.orderId = (int)row.Cells["IDDataGridViewTextBoxColumn"].Value;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            OpenOrder();
        }

        private void ordersDataGridView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            OpenOrder();
        }

        private void ordersDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            OpenOrder();
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = this.QueryButton;
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            this.AcceptButton = this.OKButton;
        }
    }
}

//int orderId = (int)row.Cells["IDDataGridViewTextBoxColumn"].Value;
//if (orderId < 1)
//    return;

//PalaceForm frm = this.ParentForm as PalaceForm;
//if (!frm.OrderData.CloseOrder())
//    return;
//frm.OrderData.LoadFromDB(orderId);
//frm.OpenOrder();
