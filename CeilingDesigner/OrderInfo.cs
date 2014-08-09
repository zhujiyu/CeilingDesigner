using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CeilingDesigner
{
    public partial class OrderInfo : UserControl
    {
        private TextBox customerBox = null;

        public TextBox CustomerBox
        {
            get { return customerBox; }
            set { customerBox = value; }
        }
        
        private CeilingDataSet.ordersRow orderRow = null;

        public CeilingDataSet.ordersRow OrderRow
        {
            get { return orderRow; }
            set { orderRow = value; }
        }

        private CeilingDataSetTableAdapters.ordersTableAdapter orderAdapter = null;

        public CeilingDataSetTableAdapters.ordersTableAdapter OrderAdapter
        {
            get { return orderAdapter; }
            set { orderAdapter = value; }
        }

        public OrderInfo()
        {
            InitializeComponent();
        }

        public void InitInfo(CeilingDataSet.ordersRow row)
        {
            this.orderRow = null;
            if (!row.IsnumberNull())
                this.numberTextBox.Text = row.number;
            else
                this.numberTextBox.Text = "";
            if (!row.Isinstall_dateNull())
                this.installTextBox.Text = row.install_date.ToShortDateString();
            else
                this.installTextBox.Text = "";
            if (!row.IsaddressNull())
                this.addressTextBox.Text = row.address;
            else
                this.addressTextBox.Text = "北京市";
            if (!row.IscustomerNull())
                this.customerTextBox1.Text = row.customer;
            else
                this.customerTextBox1.Text = "";
            if (!row.IsphoneNull())
                this.phoneTextBox.Text = row.phone;
            else
                this.phoneTextBox.Text = "";
            if (!row.IssalesmanNull())
                this.salesmanTextBox.Text = row.salesman;
            else
                this.salesmanTextBox.Text = "";
            if (!row.IsremarkNull())
                this.remarkRichTextBox.Text = row.remark;
            else
                this.remarkRichTextBox.Text = "";
            this.orderRow = row;
        }

        public void ReadOrderData(CeilingDataSet.ordersRow row)
        {
            if (this.numberTextBox.Text.Length > 0 && (row.IsnumberNull()
                || row.number != this.numberTextBox.Text))
                row.number = this.numberTextBox.Text;
            if (this.customerTextBox1.Text.Length > 0 && (row.IscustomerNull()
                || row.customer != this.customerTextBox1.Text))
                row.customer = this.customerTextBox1.Text;
            if (this.phoneTextBox.Text.Length > 0 && (row.IsphoneNull()
                || row.phone != this.phoneTextBox.Text))
                row.phone = this.phoneTextBox.Text;
            if (this.addressTextBox.Text.Length > 0 && (row.IsaddressNull()
                || row.address != this.addressTextBox.Text))
                row.address = this.addressTextBox.Text;

            if (this.salesmanTextBox.Text.Length > 0 && (row.IssalesmanNull()
                || row.salesman != this.salesmanTextBox.Text))
                row.salesman = this.salesmanTextBox.Text;
            if (this.remarkRichTextBox.Text.Length > 0 && (row.IsremarkNull()
                || row.remark != this.remarkRichTextBox.Text))
                row.remark = this.remarkRichTextBox.Text;

            DateTime dt;
            if (this.installTextBox.Text.Length > 4 
                && DateTime.TryParse(this.installTextBox.Text, out dt))
                row.install_date = dt;
        }

        private void numberTextBox_Leave(object sender, EventArgs e)
        {
            if ((!this.orderRow.IsnumberNull() 
                && this.numberTextBox.Text == this.orderRow.number)
                || this.numberTextBox.Text.Length < 5)
                return;
            if (orderAdapter == null)
                return;
            
            int? amount = (int?)orderAdapter.ScalarNumber(this.numberTextBox.Text);
            if (amount.HasValue && amount > 0)
                MessageBox.Show("该订单编号已经存在！");
        }

        private void numberTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.orderRow == null)
                return;
            if (this.orderRow.IsnumberNull() 
                || this.orderRow.number != this.numberTextBox.Text)
                this.orderRow.number = this.numberTextBox.Text;
        }

        private void customerTextBox_TextChanged(object sender, EventArgs e)
        {
            if (customerBox.Text.CompareTo(this.customerTextBox1.Text) != 0)
                customerBox.Text = this.customerTextBox1.Text;
        }

        private void phoneTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.orderRow == null)
                return;
            if (this.orderRow.IsphoneNull() 
                || this.orderRow.phone != this.phoneTextBox.Text)
                this.orderRow.phone = this.phoneTextBox.Text;
        }

        private void addressTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.orderRow == null)
                return;
            if (this.orderRow.IsaddressNull() 
                || this.orderRow.address != this.addressTextBox.Text)
                this.orderRow.address = this.addressTextBox.Text;
        }

        private void installTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.orderRow == null)
                return;
            try
            {
                if (this.installTextBox.Text == "")
                    this.orderRow.Setinstall_dateNull();
                else
                {
                    DateTime date = DateTime.Parse(this.installTextBox.Text);
                    if (this.orderRow.Isinstall_dateNull() 
                        || this.orderRow.install_date != date)
                        this.orderRow.install_date = date;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void salesmanTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.orderRow == null)
                return;
            if (this.orderRow.IssalesmanNull() 
                || this.orderRow.salesman != this.salesmanTextBox.Text)
                this.orderRow.salesman = this.salesmanTextBox.Text;
        }

        private void remarkRichTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.orderRow == null)
                return;
            if (this.orderRow.IsremarkNull() 
                || this.orderRow.remark != this.remarkRichTextBox.Text)
                this.orderRow.remark = this.remarkRichTextBox.Text;
        }
    }
}
