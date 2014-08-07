using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace CeilingDesigner
{
    public class Order
    {
        private CeilingDataSet.ordersRow orderRow = null;

        public CeilingDataSet.ordersRow OrderRow
        {
            get { return orderRow; }
        }

        //private CeilingDataSet ceilingDataSet = null;

        //public CeilingDataSet CeilingDataSet
        //{
        //    get { return ceilingDataSet; }
        //}

        private List<OrderGraph> orderGraphs = new List<OrderGraph>();

        public List<OrderGraph> OrderGraphs
        {
            get { return orderGraphs; }
        }

        private List<OrderGraph> deleteGraphs = new List<OrderGraph>();

        public List<OrderGraph> DeleteGraphs
        {
            get { return deleteGraphs; }
        }

        private string orderFile = "";

        public string OrderFile
        {
            get { return orderFile; }
        }

        private OrderSource orderSource = OrderSource.Null;

        public OrderSource OrderSource
        {
            get { return orderSource; }
        }

        PalaceForm palaceForm = null;

        //public PalaceForm PalaceForm
        //{
        //    get { return palaceForm; }
        //    //set { palaceForm = value; }
        //}

        public int ID
        {
            get
            {
                if (this.orderRow != null)
                    return this.orderRow.ID;
                else
                    return 0;
            }
        }

        //public string Number
        //{
        //    get
        //    {
        //        if (this.orderRow != null && !this.orderRow.IsnumberNull()
        //            && this.orderRow.number != "")
        //            return this.orderRow.number;
        //        else
        //            return "新建订单";
        //    }
        //}

        public string Customer
        {
            get
            {
                if (this.orderRow != null && !this.orderRow.IscustomerNull())
                    return this.orderRow.customer;
                else
                    return "客户未命名";
            }
        }

        private bool editing = true;

        public bool Editing
        {
            get { return editing; }
            set { editing = value; }
        }

        private bool changed = false;

        public bool Changed
        {
            get
            {
                if (this.changed)
                    return changed;
                for (int i = 0; i < this.orderGraphs.Count; i++)
                {
                    if (this.orderGraphs[i].Changed)
                        return true;
                }
                return false;
            }
        }

        CeilingDataSetTableAdapters.goodsTableAdapter goodAdapter
            = new CeilingDataSetTableAdapters.goodsTableAdapter();
        CeilingDataSetTableAdapters.good_viewTableAdapter gvAdapter
            = new CeilingDataSetTableAdapters.good_viewTableAdapter();
        CeilingDataSetTableAdapters.ordersTableAdapter orderAdapter
            = new CeilingDataSetTableAdapters.ordersTableAdapter();
        CeilingDataSetTableAdapters.ceilingsTableAdapter ceilingAdapter
            = new CeilingDataSetTableAdapters.ceilingsTableAdapter();
        CeilingDataSetTableAdapters.ceiling_wallesTableAdapter cwAdapter
            = new CeilingDataSetTableAdapters.ceiling_wallesTableAdapter();

        public CeilingDataSetTableAdapters.ceiling_wallesTableAdapter CwAdapter
        {
            get { return cwAdapter; }
        }

        //OrderInfo orderInfo = new OrderInfo();

        public Order(PalaceForm frm)
        {
            this.palaceForm = frm;
            
            ShareData.CeilingDataSet.orders.ordersRowChanged +=
                new CeilingDataSet.ordersRowChangeEventHandler(orders_ordersRowChanged);
        }

        void orders_ordersRowChanged(object sender, 
            CeilingDataSet.ordersRowChangeEvent e)
        {
            if (!this.editing)
                return;
            this.Change();
            //palaceForm.Text = 
        }

        public void InitMysqlDB()
        {
            this.orderAdapter.Connection = ShareData.Connection;
            this.ceilingAdapter.Connection = ShareData.Connection;
            this.cwAdapter.Connection = ShareData.Connection;
            this.goodAdapter.Connection = ShareData.Connection;
            this.gvAdapter.Connection = ShareData.Connection;

            //this.ceilingAdapter.Adapter.UpdateCommand.CommandText = "UPDATE `ceilings` SET `order_id` = @order_id, `name` = @name, `lines` = @lines, `display_left` = @display_left, `display_top` = @display_top, `display_width` = @display_width, `display_height` = @display_height, `left` = @left, `top` = @top, `width` = @width, `height` = @height, `scale` = @scale, `products` = @products, `keels` = @keels, `paint_width` = @paint_width, `paint_height` = @paint_height, `rows` = @rows, `clomns` = @clomns WHERE (`ID` = @Original_ID)";
            //this.orderInfo.OrderAdapter = this.orderAdapter;
            this.ceilingAdapter.Adapter.UpdateCommand.CommandText = "UPDATE `ceilings` SET `order_id` = @order_id, `name` = @name, `lines` = @lines, `display_left` = @display_left, `display_top` = @display_top, `display_width` = @display_width, `display_height` = @display_height, `left` = @left, `top` = @top, `width` = @width, `height` = @height, `scale` = @scale, `paint_width` = @paint_width, `paint_height` = @paint_height, `rows` = @rows, `clomns` = @clomns, `products` = @products, `appendix` = @appendix, `keels` = @keels WHERE (`ID` = @Original_ID)";
            this.ceilingAdapter.Adapter.DeleteCommand.CommandText = "DELETE FROM ceilings WHERE (ID = @Original_ID) AND (@IsNull_order_id = 1 AND order_id IS NULL OR order_id = @Original_order_id) ";
        }

        public void SetConnection(MySql.Data.MySqlClient.MySqlConnection _conn)
        {
            ceilingAdapter.Connection = _conn;
            goodAdapter.Connection = _conn;
            gvAdapter.Connection = _conn;
            orderAdapter.Connection = _conn;
            cwAdapter.Connection = _conn;
        }

        public void SetOrderID(int id)
        {
            if (id < 1)
                return;

            for (int i = 0; i < ShareData.CeilingDataSet.good_view.Count; i++)
            {
                CeilingDataSet.good_viewRow row = ShareData.CeilingDataSet.good_view[i];
                row.BeginEdit();
                row.order_id = id;
                row.EndEdit();
            }
        }

        public void Change()
        {
            if (this.palaceForm != null)
                this.palaceForm.Change();
            this.changed = true;
        }

        public void SumGood()
        {
            CeilingDataSet ceilingDataSet = ShareData.CeilingDataSet;
            CeilingDataSet.good_viewRow row, sumrow = null;
            float sum = 0;

            for (int i = 0; i < ceilingDataSet.good_view.Count; i++)
            {
                row = ceilingDataSet.good_view[i];
                if (row.id == 0 || row.product_id == 0)
                {
                    sumrow = row;
                    continue;
                }
                row.BeginEdit();
                if (row.IspriceNull())
                    row.price = 0;
                row.total = row.price * row.amount;
                row.EndEdit();
                sum += row.total;
            }

            if (sumrow == null)
            {
                sumrow = ceilingDataSet.good_view.Newgood_viewRow();
                sumrow.id = 0;
                sumrow.product_id = 0;
                sumrow.name = "总计";
                ceilingDataSet.good_view.Addgood_viewRow(sumrow);
            }

            sumrow.total = sum;
        }

        //this.tabControl1.Controls.Clear();
        //this.orderGraphs[i].Statistic();
        //this.DisplayGraph(this.orderGraphs[i]);
        //ceilingDataSet.good_view.AcceptChanges();

        public void Statistic()
        {
            CeilingDataSet ceilingDataSet = ShareData.CeilingDataSet;
            CeilingDataSet.good_viewRow row;

            for (int i = 0; i < ceilingDataSet.good_view.Count; i++)
            {
                row = ceilingDataSet.good_view[i];
                row.BeginEdit();
                row.amount = 0;
                row.EndEdit();
            }

            for (int i = 0; i < this.orderGraphs.Count; i++)
            {
                if (this.orderGraphs[i].ProductSet != null)
                    this.orderGraphs[i].ProductSet.Statistic(ceilingDataSet.good_view);
            }

            for (int i = 0; i < ceilingDataSet.good_view.Count; i++)
            {
                row = ceilingDataSet.good_view[i];
                if (row.amount < 1)
                    row.Delete();
            }
            ceilingDataSet.good_view.AcceptChanges();
            this.SumGood();

            this.editing = false;
            if (orderRow == null)
            {
                this.orderRow = ceilingDataSet.orders.NewordersRow();
                ceilingDataSet.orders.AddordersRow(this.orderRow);
            }
            this.editing = true;
        }

        private void RefrushGoodData(CeilingDataSet.goodsDataTable _godds)
        {
            CeilingDataSet ceilingDataSet = ShareData.CeilingDataSet;
            CeilingDataSet.good_viewRow gvrow;
            CeilingDataSet.goodsRow _grow = null;

            for (int i = 0; i < ceilingDataSet.goods.Count; i++)
            {
                _grow = ceilingDataSet.goods[i];
                _grow.BeginEdit();
                _grow.amount = 0;
                _grow.EndEdit();
            }

            for (int i = 0; i < ceilingDataSet.good_view.Count; i++)
            {
                gvrow = ceilingDataSet.good_view[i];
                if (gvrow.id > 0)
                    _grow = _godds.FindByID(gvrow.id);
                else if (gvrow.id == 0)
                    continue;
                else
                    _grow = null;

                if (gvrow.id < 0 || _grow == null)
                {
                    _grow = _godds.NewgoodsRow();
                    _grow.BeginEdit();
                    _grow.order_id = gvrow.order_id;
                    _grow.product_id = gvrow.product_id;
                    _grow.amount = gvrow.amount;
                    _grow.price = gvrow.price;
                    _grow.total = gvrow.total;

                    if (!gvrow.IscategoryNull())
                        _grow.category = gvrow.category;
                    if (!gvrow.IsmodelNull())
                        _grow.model = gvrow.model;
                    if (!gvrow.IsremarkNull())
                        _grow.remark = gvrow.remark;
                    _grow.EndEdit();
                    _godds.AddgoodsRow(_grow);
                }
                else
                {
                    _grow.BeginEdit();
                    if (_grow.product_id != gvrow.product_id)
                        _grow.product_id = gvrow.product_id;
                    if (_grow.amount != gvrow.amount)
                        _grow.amount = gvrow.amount;
                    if (_grow.total != gvrow.total)
                        _grow.total = gvrow.total;

                    if (!gvrow.IscategoryNull() && gvrow.category != _grow.category)
                        _grow.category = gvrow.category;
                    if (!gvrow.IsmodelNull() && _grow.model != gvrow.model)
                        _grow.model = gvrow.model;
                    if (!gvrow.IsremarkNull() && _grow.remark != gvrow.remark)
                        _grow.remark = gvrow.remark;
                    _grow.EndEdit();
                }
            }

            for (int i = 0; i < ceilingDataSet.goods.Count; i++)
            {
                _grow = ceilingDataSet.goods[i];
                if (_grow.amount < 1)
                    _grow.Delete();
            }
        }

        public bool SaveToDB()
        {
            if (this.orderRow.IsnumberNull() && this.orderRow.IscustomerNull()
                && this.orderRow.IsphoneNull() && this.orderRow.IsaddressNull())
            {
                MessageBox.Show("请输入订单编号和客户信息，然后才能保存到服务器！",
                    ShareData.AppName);
                return false;
            }
            //else
            //    this.orderInfo.ReadOrderData(this.orderRow);

            if (!this.orderRow.IsnumberNull() 
                && this.orderSource == CeilingDesigner.OrderSource.XMLDoc
                && orderAdapter.ExistNumber(this.orderRow.number) > 0)
            {
                //"奥普1+N浴顶",
                if (MessageBox.Show("该订单在服务器上已经存在，你确实要覆盖吗？",
                    ShareData.AppName, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    return false;
                CeilingDataSet dataSet = new CeilingDataSet();

                this.orderAdapter.FillByNumber(dataSet.orders, this.orderRow.number);
                this.ceilingAdapter.FillByOrderId(dataSet.ceilings, 
                    dataSet.orders[0].ID);
                this.goodAdapter.FillByOrderId(dataSet.goods, dataSet.orders[0].ID);

                for (int i = 0; i < dataSet.ceilings.Count; i++)
                {
                    this.cwAdapter.FillByCeilingId(dataSet.ceiling_walles, 
                        dataSet.ceilings[i].ID);
                    for (int j = 0; j < dataSet.ceiling_walles.Count; j++)
                    {
                        dataSet.ceiling_walles[j].Delete();
                    }
                    this.cwAdapter.Update(dataSet.ceiling_walles);
                    dataSet.ceiling_walles.AcceptChanges();

                    dataSet.ceilings[i].Delete();
                }
                this.ceilingAdapter.Update(dataSet.ceilings);
                dataSet.ceilings.AcceptChanges();

                for (int i = 0; i < dataSet.goods.Count; i++)
                {
                    dataSet.goods[i].Delete();
                }
                this.goodAdapter.Update(dataSet.goods);
                dataSet.goods.AcceptChanges();

                for (int i = 0; i < dataSet.orders.Count; i++)
                {
                    dataSet.orders[i].Delete();
                }
                this.orderAdapter.Update(dataSet.orders);
                dataSet.orders.AcceptChanges();

                //this.RemoveOrderDB();
            }

            CeilingDataSet ceilingDataSet = ShareData.CeilingDataSet;
            if (this.changed || this.ID < 1)
            {
                this.orderRow.create_date = DateTime.Now;
                this.orderAdapter.Update(ceilingDataSet.orders);
                this.orderRow = ceilingDataSet.orders[0];

                if (this.orderRow.ID < 1)
                {
                    long _id = this.orderAdapter.Adapter.InsertCommand.LastInsertedId;
                    this.orderRow.BeginEdit();
                    this.orderRow.ID = (int)_id;
                    this.orderRow.EndEdit();
                    this.SetOrderID(this.orderRow.ID);
                }
                ceilingDataSet.orders.AcceptChanges();

                this.goodAdapter.FillByOrderId(ceilingDataSet.goods,
                    this.orderRow.ID);
                this.RefrushGoodData(ceilingDataSet.goods);

                if (ceilingDataSet.goods.GetChanges() != null)
                {
                    this.goodAdapter.Update(ceilingDataSet.goods);
                    ceilingDataSet.goods.AcceptChanges();
                }
            }

            // 先将删除某些房间的数据checkin
            if (this.deleteGraphs.Count > 0 && this.ID > 0)
            {
                OrderGraph graph;
                for (int i = 0; i < this.deleteGraphs.Count; i++)
                {
                    graph = this.deleteGraphs[i];
                    if (graph.Ceiling.ID < 1)
                        continue;
                    this.cwAdapter.Update(graph.Ceiling.CeilingWalles);
                    graph.Ceiling.CeilingWalles.AcceptChanges();
                }

                ceilingAdapter.Update(ceilingDataSet.ceilings);
                ceilingDataSet.ceilings.AcceptChanges();
                this.deleteGraphs.Clear();
            }

            ceilingAdapter.FillByOrderId(ceilingDataSet.ceilings, this.ID);
            for (int i = 0; i < this.orderGraphs.Count; i++)
            {
                this.orderGraphs[i].SaveDB(ceilingDataSet, ceilingAdapter);
            }

            try
            {
                ceilingAdapter.Update(ceilingDataSet.ceilings);
                ceilingDataSet.ceilings.AcceptChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }

            for (int i = 0; i < this.orderGraphs.Count; i++)
            {
                this.orderGraphs[i].Ceiling.SaveToDB(this.cwAdapter);
                this.orderGraphs[i].SaveToDB();
            }

            this.orderSource = CeilingDesigner.OrderSource.WebServer;
            this.changed = false;
            return true;
        }

        public string GetFileName()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.FileName = "aupu.xml";
            saveFileDialog.Filter = "xml 文件(*.xml)|*.xml|所有文件(*.*)|*.*";

            saveFileDialog.FileName = this.Customer + " - "
                + this.palaceForm.CurOrderGraph.Ceiling.Name + ".xml";

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return null;
            return saveFileDialog.FileName;
        }

        //if (this.Customer != null)
        //    saveFileDialog.FileName = this.Customer + " - "
        //        + this.palaceForm.CurOrderGraph.Ceiling.Name + ".xml";
        //else
        //    saveFileDialog.FileName = this.Number + " - "
        //        + this.palaceForm.CurOrderGraph.Ceiling.Name + ".xml";

        private void CloneOrderData(CeilingDataSet.ordersRow src,
            CeilingDataSet.ordersRow dst)
        {
            dst.ID = src.ID;
            if (!src.IsnumberNull())
                dst.number = src.number;
            if (!src.IscustomerNull())
                dst.customer = src.customer;

            if (!src.IsphoneNull())
                dst.phone = src.phone;
            if (!src.IsaddressNull())
                dst.address = src.address;

            if (!src.Isinstall_dateNull())
                dst.install_date = src.install_date;
            if (!src.IssalesmanNull())
                dst.salesman = src.salesman;
            if (!src.IsremarkNull())
                dst.remark = src.remark;
        }

        public void SaveFile(string filename)
        {
            this.orderFile = filename;
            CeilingDataSet dataSet = new CeilingDataSet();

            CeilingDataSet.ordersRow row = dataSet.orders.NewordersRow();
            //this.orderInfo.RefrushOrderData(orderRow);
            //dataSet.orders.AddordersRow(this.orderRow);
            CloneOrderData(orderRow, row);
            dataSet.orders.AddordersRow(row);
            dataSet.orders.AcceptChanges();

            RefrushGoodData(dataSet.goods);
            dataSet.goods.AcceptChanges();

            CeilingDataSet.good_viewDataTable good_view 
                = ShareData.CeilingDataSet.good_view;

            for (int i = 0; i < good_view.Count; i++)
            {
                CeilingDataSet.good_viewRow row1 = good_view[i];
                if (row1.id == 0 || row1.product_id == 0)
                    continue;
                CeilingDataSet.good_viewRow row2 = dataSet.good_view.Newgood_viewRow();

                row2.id = row1.id;
                row2.total = row1.total;
                row2.product_id = row1.product_id;
                row2.name = row1.name;
                row2.order_id = row1.order_id;
                row2.price = row1.price;
                row2.category = row1.category;
                row2.amount = row1.amount;

                if (!row1.IscolorNull())
                    row2.unit = row1.unit;
                if (!row1.IspatternNull())
                    row2.pattern = row1.pattern;
                if (!row1.IscolorNull())
                    row2.color = row1.color;
                if (!row1.IsmodelNull())
                    row2.model = row1.model;
                if (!row1.IsremarkNull())
                    row2.remark = row1.remark;

                dataSet.good_view.Addgood_viewRow(row2);
            }
            dataSet.good_view.AcceptChanges();

            for (int i = 0; i < this.orderGraphs.Count; i++)
                this.orderGraphs[i].SaveFile(dataSet, i + 10);

            dataSet.WriteXml(this.orderFile);
            this.changed = false;
            this.orderSource = CeilingDesigner.OrderSource.XMLDoc;
        }

        public bool SaveToFile()
        {
            if (this.orderFile == null || this.orderFile == "")
                this.orderFile = GetFileName();
            if (this.orderFile == null)
                return false;
            SaveFile(this.orderFile);
            return true;
        }

        public bool Save()
        {
            if (this.orderSource == CeilingDesigner.OrderSource.WebServer)
                return this.SaveToDB();
            else
                return this.SaveToFile();
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

        public void NewOrder()
        {
            this.editing = false;
            this.orderRow = ShareData.CeilingDataSet.orders.NewordersRow();
            ShareData.CeilingDataSet.orders.AddordersRow(this.orderRow);
            this.editing = true;

            System.Random rand = new Random();
            string secs = ConvertDateTimeInt(DateTime.Now).ToString();
            orderRow.ID = Convert.ToInt32(secs.Substring(2, secs.Length - 2)
                + rand.Next(100).ToString());

            this.orderSource = CeilingDesigner.OrderSource.Null;
            this.orderFile = "";
            palaceForm.NewGraph();
        }

        public bool CloseOrder()
        {
            // 先关闭当前的订单，如果已经修改了，应该先提示保存
            if (this.Changed)
            {
                DialogResult rlt = MessageBox.Show("订单内容已修改，是否保存？",
                    ShareData.AppName, MessageBoxButtons.YesNoCancel);
                if (rlt == DialogResult.Yes)
                    this.Save();
                else if (rlt == DialogResult.Cancel)
                    return false;
            }

            for (int i = 0; i < this.orderGraphs.Count; i++)
            {
                if (!this.orderGraphs[i].IsDisposed)
                {
                    ToolStripMenuItem item = this.orderGraphs[i].Tag as ToolStripMenuItem;
                    if (item != null && !item.IsDisposed)
                        item.Dispose();

                    if (this.orderGraphs[i].ToolTip != null)
                    {
                        this.orderGraphs[i].ToolTip.Hide(this.orderGraphs[i]);
                        this.orderGraphs[i].ToolTip = null;
                    }
                    this.orderGraphs[i].Dispose();
                }
                this.orderGraphs[i] = null;
            }

            this.orderGraphs.Clear();
            this.deleteGraphs.Clear();

            CeilingDataSet ceilingDataSet = ShareData.CeilingDataSet;
            ceilingDataSet.orders.Clear();
            ceilingDataSet.ceilings.Clear();

            ceilingDataSet.ceiling_walles.Clear();
            ceilingDataSet.goods.Clear();
            ceilingDataSet.szones.Clear();

            this.orderRow = null;

            return true;
        }

        private void ReadXml(string file)
        {
            CeilingDataSet ceilingDataSet = ShareData.CeilingDataSet;

            ceilingDataSet.ReadXml(file);
            if (ceilingDataSet.orders.Count > 0)
                return;

            DataSet ds = new DataSet();
            ds.ReadXml(file);

            if ((ds.Tables["orders"] != null))
            {
                DataTable table = ds.Tables["orders"];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];
                    CeilingDataSet.ordersRow orderRow = ceilingDataSet.orders.NewordersRow();

                    if (table.Columns.Contains("ID") && !row.IsNull("ID"))
                        orderRow.ID = int.Parse((string)row["ID"]);
                    if (table.Columns.Contains("address") && !row.IsNull("address"))
                        orderRow.address = (string)row["address"];
                    if (table.Columns.Contains("create_date") && !row.IsNull("create_date"))
                        orderRow.create_date = DateTime.Parse((string)row["create_date"]);
                    if (table.Columns.Contains("customer") && !row.IsNull("customer"))
                        orderRow.customer = (string)row["customer"];
                    if (table.Columns.Contains("install_date") && !row.IsNull("install_date"))
                        orderRow.install_date = DateTime.Parse((string)row["install_date"]);
                    if (table.Columns.Contains("number") && !row.IsNull("number"))
                        orderRow.number = (string)row["number"];
                    if (table.Columns.Contains("phone") && !row.IsNull("phone"))
                        orderRow.phone = (string)row["phone"];
                    if (table.Columns.Contains("remark") && !row.IsNull("remark"))
                        orderRow.remark = (string)row["remark"];
                    if (table.Columns.Contains("salesman") && !row.IsNull("salesman"))
                        orderRow.salesman = (string)row["salesman"];
                    if (table.Columns.Contains("sum_price") && !row.IsNull("sum_price"))
                        orderRow.sum_price = float.Parse((string)row["sum_price"]);

                    ceilingDataSet.orders.AddordersRow(orderRow);
                }
            }

            if ((ds.Tables["goods"] != null))
            {
                DataTable table = ds.Tables["goods"];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];
                    CeilingDataSet.goodsRow goodsRow = ceilingDataSet.goods.NewgoodsRow();

                    if (table.Columns.Contains("ID") && !row.IsNull("ID"))
                        goodsRow.ID = int.Parse((string)row["ID"]);
                    if (table.Columns.Contains("amount") && !row.IsNull("amount"))
                        goodsRow.amount = int.Parse((string)row["amount"]);
                    if (table.Columns.Contains("category") && !row.IsNull("category"))
                        goodsRow.category = (string)row["category"];
                    if (table.Columns.Contains("model") && !row.IsNull("model"))
                        goodsRow.model = (string)row["model"];
                    if (table.Columns.Contains("order_id") && !row.IsNull("order_id"))
                        goodsRow.order_id = int.Parse((string)row["order_id"]);
                    if (table.Columns.Contains("price") && !row.IsNull("price"))
                        goodsRow.price = float.Parse((string)row["price"]);
                    if (table.Columns.Contains("product_id") && !row.IsNull("product_id"))
                        goodsRow.product_id = int.Parse((string)row["product_id"]);
                    if (table.Columns.Contains("remark") && !row.IsNull("remark"))
                        goodsRow.remark = (string)row["remark"];
                    if (table.Columns.Contains("total") && !row.IsNull("total"))
                        goodsRow.total = float.Parse((string)row["total"]);

                    ceilingDataSet.goods.AddgoodsRow(goodsRow);
                }
            }

            if ((ds.Tables["good_view"] != null))
            {
                DataTable table = ds.Tables["good_view"];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];
                    CeilingDataSet.good_viewRow viewRow = ceilingDataSet.good_view.Newgood_viewRow();

                    if (table.Columns.Contains("id") && !row.IsNull("id"))
                        viewRow.id = int.Parse((string)row["id"]);

                    if (table.Columns.Contains("color") && !row.IsNull("color"))
                        viewRow.color = (string)row["color"];
                    if (table.Columns.Contains("name") && !row.IsNull("name"))
                        viewRow.name = (string)row["name"];
                    if (table.Columns.Contains("pattern") && !row.IsNull("pattern"))
                        viewRow.pattern = (string)row["pattern"];
                    if (table.Columns.Contains("unit") && !row.IsNull("unit"))
                        viewRow.unit = (string)row["unit"];

                    if (table.Columns.Contains("amount") && !row.IsNull("amount"))
                        viewRow.amount = int.Parse((string)row["amount"]);
                    if (table.Columns.Contains("category") && !row.IsNull("category"))
                        viewRow.category = (string)row["category"];
                    if (table.Columns.Contains("model") && !row.IsNull("model"))
                        viewRow.model = (string)row["model"];
                    if (table.Columns.Contains("order_id") && !row.IsNull("order_id"))
                        viewRow.order_id = int.Parse((string)row["order_id"]);
                    if (table.Columns.Contains("price") && !row.IsNull("price"))
                        viewRow.price = float.Parse((string)row["price"]);
                    if (table.Columns.Contains("product_id") && !row.IsNull("product_id"))
                        viewRow.product_id = int.Parse((string)row["product_id"]);
                    if (table.Columns.Contains("remark") && !row.IsNull("remark"))
                        viewRow.remark = (string)row["remark"];
                    if (table.Columns.Contains("total") && !row.IsNull("total"))
                        viewRow.total = float.Parse((string)row["total"]);

                    ceilingDataSet.good_view.Addgood_viewRow(viewRow);
                }
            }

            if ((ds.Tables["ceilings"] != null))
            {
                DataTable table = ds.Tables["ceilings"];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];
                    CeilingDataSet.ceilingsRow ceilingRow = ceilingDataSet.ceilings.NewceilingsRow();

                    if (table.Columns.Contains("ID") && !row.IsNull("ID"))
                        ceilingRow.ID = int.Parse((string)row["ID"]);

                    if (table.Columns.Contains("rows") && !row.IsNull("rows"))
                        ceilingRow.rows = int.Parse((string)row["rows"]);
                    if (table.Columns.Contains("clomns") && !row.IsNull("clomns"))
                        ceilingRow.columns = int.Parse((string)row["clomns"]);
                    if (table.Columns.Contains("scale") && !row.IsNull("scale"))
                        ceilingRow.scale = float.Parse((string)row["scale"]);

                    if (table.Columns.Contains("display_height") && !row.IsNull("display_height"))
                        ceilingRow.display_height = int.Parse((string)row["display_height"]);
                    if (table.Columns.Contains("display_left") && !row.IsNull("display_left"))
                        ceilingRow.display_left = int.Parse((string)row["display_left"]);
                    if (table.Columns.Contains("display_top") && !row.IsNull("display_top"))
                        ceilingRow.display_top = int.Parse((string)row["display_top"]);
                    if (table.Columns.Contains("display_width") && !row.IsNull("display_width"))
                        ceilingRow.display_width = int.Parse((string)row["display_width"]);

                    if (table.Columns.Contains("left") && !row.IsNull("left"))
                        ceilingRow.left = float.Parse((string)row["left"]);
                    if (table.Columns.Contains("top") && !row.IsNull("top"))
                        ceilingRow.top = float.Parse((string)row["top"]);
                    if (table.Columns.Contains("width") && !row.IsNull("width"))
                        ceilingRow.width = float.Parse((string)row["width"]);
                    if (table.Columns.Contains("height") && !row.IsNull("height"))
                        ceilingRow.height = int.Parse((string)row["height"]);

                    if (table.Columns.Contains("lines") && !row.IsNull("lines"))
                        ceilingRow.lines = int.Parse((string)row["lines"]);
                    if (table.Columns.Contains("name") && !row.IsNull("name"))
                        ceilingRow.name = (string)row["name"];
                    if (table.Columns.Contains("order_id") && !row.IsNull("order_id"))
                        ceilingRow.order_id = int.Parse((string)row["order_id"]);

                    if (table.Columns.Contains("paint_height") && !row.IsNull("paint_height"))
                        ceilingRow.paint_height = float.Parse((string)row["paint_height"]);
                    if (table.Columns.Contains("paint_width") && !row.IsNull("paint_width"))
                        ceilingRow.paint_width = float.Parse((string)row["paint_width"]);

                    if (table.Columns.Contains("keels") && !row.IsNull("keels"))
                        ceilingRow.keels = (string)row["keels"];
                    if (table.Columns.Contains("products") && !row.IsNull("products"))
                        ceilingRow.products = (string)row["products"];
                    if (table.Columns.Contains("appendix") && !row.IsNull("appendix"))
                        ceilingRow.appendix = (string)row["appendix"];

                    ceilingDataSet.ceilings.AddceilingsRow(ceilingRow);
                }
            }

            if ((ds.Tables["ceiling_walles"] != null))
            {
                DataTable table = ds.Tables["ceiling_walles"];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];
                    CeilingDataSet.ceiling_wallesRow ceiling_wallesRow = ceilingDataSet.ceiling_walles.Newceiling_wallesRow();

                    if (table.Columns.Contains("ID") && !row.IsNull("ID"))
                        ceiling_wallesRow.ID = int.Parse((string)row["ID"]);

                    if (table.Columns.Contains("ceiling_id") && !row.IsNull("ceiling_id"))
                        ceiling_wallesRow.ceiling_id = int.Parse((string)row["ceiling_id"]);
                    if (table.Columns.Contains("endx") && !row.IsNull("endx"))
                        ceiling_wallesRow.endx = float.Parse((string)row["endx"]);
                    if (table.Columns.Contains("endy") && !row.IsNull("endy"))
                        ceiling_wallesRow.endy = float.Parse((string)row["endy"]);

                    if (table.Columns.Contains("radian") && !row.IsNull("radian"))
                        ceiling_wallesRow.radian = float.Parse((string)row["radian"]);
                    if (table.Columns.Contains("wallnum") && !row.IsNull("wallnum"))
                        ceiling_wallesRow.wallnum = int.Parse((string)row["wallnum"]);

                    ceilingDataSet.ceiling_walles.Addceiling_wallesRow(ceiling_wallesRow);
                }
            }

            if ((ds.Tables["SZones"] != null))
            {
                DataTable table = ds.Tables["SZones"];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];
                    CeilingDataSet.szonesRow SZonesRow = ceilingDataSet.szones.NewszonesRow();

                    if (table.Columns.Contains("ID") && !row.IsNull("ID"))
                        SZonesRow.ID = int.Parse((string)row["ID"]);

                    if (table.Columns.Contains("ceiling_id") && !row.IsNull("ceiling_id"))
                        SZonesRow.ceiling_id = int.Parse((string)row["ceiling_id"]);
                    if (table.Columns.Contains("beginx") && !row.IsNull("beginx"))
                        SZonesRow.beginx = float.Parse((string)row["beginx"]);
                    if (table.Columns.Contains("beginy") && !row.IsNull("beginy"))
                        SZonesRow.beginy = float.Parse((string)row["beginy"]);

                    if (table.Columns.Contains("depth") && !row.IsNull("depth"))
                        SZonesRow.depth = int.Parse((string)row["depth"]);
                    if (table.Columns.Contains("remark") && !row.IsNull("remark"))
                        SZonesRow.remark = (string)row["remark"];
                    if (table.Columns.Contains("szone_num") && !row.IsNull("szone_num"))
                        SZonesRow.szone_num = int.Parse((string)row["szone_num"]);

                    ceilingDataSet.szones.AddszonesRow(SZonesRow);
                }
            }
        }

        public void LoadFromFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xml文件(*.xml)|*.xml|所有文件(*.*)|*.*";

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            if (!this.CloseOrder())
                return;
            this.editing = false;

            CeilingDataSet ceilingDataSet = ShareData.CeilingDataSet;
            ceilingDataSet.orders.Clear();
            ceilingDataSet.ceilings.Clear();
            ceilingDataSet.ceiling_walles.Clear();
            ceilingDataSet.goods.Clear();
            ceilingDataSet.good_view.Clear();
            ceilingDataSet.szones.Clear();

            this.orderFile = openFileDialog.FileName;
            this.ReadXml(this.orderFile);
            if (ceilingDataSet.orders.Count > 0)
                this.orderRow = ceilingDataSet.orders[0];

            palaceForm.NewGraph();
            OrderGraph graph = palaceForm.CurOrderGraph;
            if (ceilingDataSet.ceilings.Count > 0)
            {
                graph.LoadFromFile(ceilingDataSet, 0);
            }

            for (int i = 1; i < ceilingDataSet.ceilings.Count; i++)
            {
                graph = new OrderGraph(this);
                graph.LoadFromFile(ceilingDataSet, i);
            }

            this.orderSource = CeilingDesigner.OrderSource.XMLDoc;
            this.editing = true;
        }

        public void LoadFromDB(int order_id)
        {
            if (order_id <= 0)
                return;
            if (!this.CloseOrder())
                return;
            this.editing = false;

            palaceForm.NewGraph();
            CeilingDataSet ceilingDataSet = ShareData.CeilingDataSet;

            this.orderAdapter.FillByID(ceilingDataSet.orders, order_id);
            this.goodAdapter.FillByOrderId(ceilingDataSet.goods, order_id);
            this.ceilingAdapter.FillByOrderId(ceilingDataSet.ceilings, order_id);

            if (ceilingDataSet.orders.Count > 0)
                this.orderRow = ceilingDataSet.orders[0];

            OrderGraph graph = palaceForm.CurOrderGraph;
            if (ceilingDataSet.ceilings.Count > 0)
            {
                graph.LoadFromDB(ceilingDataSet.ceilings[0]);
            }

            for (int i = 1; i < ceilingDataSet.ceilings.Count; i++)
            {
                graph = new OrderGraph(this);
                graph.LoadFromDB(ceilingDataSet.ceilings[i]);
            }

            this.orderSource = CeilingDesigner.OrderSource.WebServer;
            this.orderFile = "";
            this.editing = true;
        }
    }

    public enum OrderSource { WebServer, XMLDoc, Null };
}
