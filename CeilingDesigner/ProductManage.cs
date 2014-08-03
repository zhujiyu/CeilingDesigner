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
    public partial class ProductManage : Form
    {
        //BuckleList list = new BuckleList();
        //DataView classView = new DataView();
        DataView productView = new DataView();
        private int msec = 500;
        private bool changed = false;

        public bool Changed
        {
            get { return changed; }
        }

        private CeilingDataSet set = null;

        //public CeilingDataSet Set
        //{
        //    get { return set; }
        //    set
        //    {
        //        set = value;
        //        productView.Table = set.products;
        //        classView.Table = set.product_classes;
        //        classView.RowFilter = "type = 'surface'";
        //    }
        //}

        private bool HasChanged
        {
            get
            {
                if (set.product_classes.GetChanges() != null)
                    return true;
                if (set.products.GetChanges() != null)
                    return true;
                return false;
            }
        }

        public ProductManage()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            set = ShareData.CeilingDataSet;
            productView.Table = set.products;

            BuckleList.DispPClass(set.product_classes, treeView1);
            if (treeView1.Nodes.Count > 0)
                treeView1.Nodes[0].Remove();
            this.panel1.AutoSize = true;

            this.Controls.Add(this.panel2);
            this.panel2.BringToFront();
            this.panel2.Left = (this.ClientSize.Width - this.panel2.Width) / 2;
            this.panel2.Top = this.splitContainer1.Panel1.Height 
                - this.panel2.Height / 2 + this.toolStrip1.Height;
            this.panel2.Visible = false;
        }

        void pNode_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                BuckleList.Current = sender as BuckleNode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        void pNode_MouseHover(object sender, EventArgs e)
        {
            try
            {
                BuckleNode node = sender as BuckleNode;
                string str = "名称：" + node.Name
                    + "\n规格：" + node.OriginalRow.width + " * " + node.OriginalRow.height;
                //+ "\n" + "价格：" + node.OriginalRow.price;
                this.toolTip1.SetToolTip(node, str);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("错误信息：" + ex.Message 
                    + "\n" + ex.ToString());
            }
        }

        public void AddProduct(BuckleNode pNode)
        {
            pNode.ContextMenuStrip = this.contextMenuStrip4;
            pNode.MouseDown += new MouseEventHandler(pNode_MouseDown);
            pNode.MouseHover += new EventHandler(pNode_MouseHover);
            this.panel1.Controls.Add(pNode);
        }

        public void AddProduct(BuckleNode pNode, int index)
        {
            int width = 120, rows = this.panel1.Height / width;
            pNode.Location = new Point((index / rows) * width + 10, 
                (index % rows) * width + 10);
            AddProduct(pNode);
        }

        // 点击产品分组，显示产品列表
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (!(e.Node.Tag is CeilingDataSet.product_classesRow))
                    return;
                BuckleList.Current = null;
                this.panel1.Controls.Clear();

                int width = 120, rows = this.panel1.Height / width;
                CeilingDataSet.product_classesRow row = e.Node.Tag as CeilingDataSet.product_classesRow;
                if (row.clone_id > 0)
                    row = row.product_classesRowParentByparent_clone;
                this.productView.RowFilter = "class_id = " + row.ID.ToString();

                for (int i = 0; i < this.productView.Count; i++)
                {
                    BuckleNode pNode = new BuckleNode(this.productView[i].Row 
                        as CeilingDataSet.productsRow);
                    pNode.Location = new Point((i / rows) * width + 10, 
                        (i % rows) * width + 10);
                    AddProduct(pNode);
                }

                //this.Text = BuckleNode.PhotoPath(row) + " | 产品数据管理 | " 
                //    + Palace.Properties.Settings.Default.AppName;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private int MaxProductID(CeilingDataSet.product_classesRow crow)
        {
            DataView view = new DataView();
            view.Table = set.products;
            view.RowFilter = "class_id = " + crow.ID.ToString();
            view.Sort = "ID desc";
            if (view.Count > 0)
                return (view[0].Row as CeilingDataSet.productsRow).ID + 1;
            else
                return crow.ID + 1;
        }

        private int MaxClassID()
        {
            DataView view = new DataView();
            view.Table = set.product_classes;
            view.RowFilter = "type = 'surface'";
            view.Sort = "ID desc";
            return (view[0].Row as CeilingDataSet.product_classesRow).ID;
        }

        private CeilingDataSet.product_classesRow AddChildClass(int parent_id)
        {
            CeilingDataSet.product_classesRow pcrow = set.product_classes.Newproduct_classesRow();
            pcrow.ID = (MaxClassID() / 100000 + 1) * 100000;
            //pcrow.ID = MaxClassID() + 10000;
            pcrow.name = "新建分组";
            pcrow.type = "surface";
            pcrow.clone_id = 0;
            pcrow.parent_class_id = parent_id;
            set.product_classes.Addproduct_classesRow(pcrow);
            return pcrow;
        }

        private CeilingDataSet.product_classesRow CloneClass(int parent_id, int clone_id)
        {
            CeilingDataSet.product_classesRow pcrow = set.product_classes.Newproduct_classesRow();
            pcrow.ID = (clone_id / 10000 + 1) * 10000;
            pcrow.type = "surface";
            pcrow.clone_id = clone_id;
            pcrow.parent_class_id = parent_id;
            set.product_classes.Addproduct_classesRow(pcrow);
            return pcrow;
        }

        private void 添加分组ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeView1.SelectedNode == null)
                return;

            TreeNode node = this.treeView1.SelectedNode;
            CeilingDataSet.product_classesRow parent_row = node.Tag as CeilingDataSet.product_classesRow;
            if (parent_row.clone_id > 0)
                parent_row = parent_row.product_classesRowParentByparent_clone;

            CeilingDataSet.product_classesRow crow;
            if (node.Level == 0)
                crow = AddChildClass(100000000);
            else
                crow = AddChildClass(parent_row.ID);

            TreeNode temp = node;
            while (temp.Level > 0)
            {
                temp = temp.Parent;
            }
            crow.category = temp.Name;
            //parent_row.withchildren = true;

            node = new TreeNode();
            node.Text = crow.name;
            node.Name = crow.name;
            node.Tag = crow;

            this.treeView1.SelectedNode.Nodes.Add(node);
            this.treeView1.SelectedNode.Expand();
            this.treeView1.SelectedNode = node;
            node.BeginEdit();
        }

        private void 添加产品ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            string[] _path = this.openFileDialog1.FileName.Split('\\');
            if (_path.Length < 1)
                return;
            string[] _file = _path[_path.Length - 1].Split('.');

            Random rand = new Random();
            Bitmap bmp = new Bitmap(this.openFileDialog1.FileName);

            CeilingDataSet.product_classesRow crow = this.treeView1.SelectedNode.Tag as CeilingDataSet.product_classesRow;
            if (crow.clone_id > 0)
                crow = crow.product_classesRowParentByparent_clone;
            CeilingDataSet.productsRow prow = set.products.NewproductsRow();

            prow.ID = MaxProductID(crow);
            prow.name = _file[0];

            prow.width = 300;
            prow.height = 300;
            prow.length = 0;
            prow.unit = "";
            prow.pattern = "";

            prow.class_id = crow.ID;
            prow.price = 0;
            prow.photo = _file[0] + rand.Next(1000000, 9999999).ToString() + "." + _file[1];
            set.products.AddproductsRow(prow);
            BuckleNode.SavePhoto(prow, bmp);

            BuckleNode node = new BuckleNode(prow);
            AddProduct(node, this.panel1.Controls.Count);
            node.SynLoadImage();
            BuckleList.Current = node;
            node.Edit();
        }

        private void 删除toolStripButton2_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem))
                return;
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item.Owner == null)
                return;
            ContextMenuStrip strip = item.Owner as ContextMenuStrip;

            if (strip.SourceControl is BuckleNode)
            {
                BuckleNode node = strip.SourceControl as BuckleNode;
                BuckleList.Current = node;
                if (MessageBox.Show("确实要删除该产品吗？", ShareData.AppName, 
                    MessageBoxButtons.YesNo) == DialogResult.No)
                    return;

                BuckleList.Current = null;
                node.OriginalRow.Delete();
                this.panel1.Controls.Remove(node);
                //this.splitContainer1.Panel2.Controls.Remove(current);
            }
            else if (this.treeView1.SelectedNode != null)
            {
                if (this.treeView1.SelectedNode.Level < 1)
                {
                    MessageBox.Show("该分组是顶层分组，不能删除。", ShareData.AppName);
                    return;
                }
                if (this.treeView1.SelectedNode.Nodes.Count > 0)
                {
                    MessageBox.Show("该分组下还有子分组，请先把子分组删除后再删除该分组。",
                        ShareData.AppName);
                    return;
                }

                if (MessageBox.Show("确实要删除该分类吗？该分类下的所有产品数据将会同时删除。",
                    ShareData.AppName, 
                    MessageBoxButtons.YesNo) == DialogResult.No)
                    return;

                CeilingDataSet.product_classesRow crow 
                    = this.treeView1.SelectedNode.Tag as CeilingDataSet.product_classesRow;
                if (crow.clone_id > 0)
                    crow = crow.product_classesRowParentByparent_clone;

                // 当前选中的节点取消选中状态
                BuckleList.Current = null;

                this.productView.RowFilter = "class_id = " + crow.ID.ToString();
                while (productView.Count > 0)
                {
                    (productView[0].Row as CeilingDataSet.productsRow).Delete();
                }

                // 当一条主分组删除时，它的所有拷贝分组也一并删除
                CeilingDataSet.product_classesRow[] rows
                    = crow.Getproduct_classesRowsByparent_clone();
                if (rows != null && rows.Length > 0)
                {
                    for (int i = rows.Length - 1; i >= 0; i--)
                        rows[i].Delete();
                }

                crow.Delete();
                this.treeView1.SelectedNode.Remove();
            }
        }

        private void 编辑toolStripButton1_Click(object sender, EventArgs e)
        {
            SetMenu(false);

            ToolStripMenuItem item = sender as ToolStripMenuItem;
            ContextMenuStrip strip = item.Owner as ContextMenuStrip;

            if (strip.SourceControl is BuckleNode)
            {
                BuckleList.Current = strip.SourceControl as BuckleNode;
                BuckleList.Current.Edit();
            }
            else if (this.treeView1.SelectedNode != null)
            {
                TreeNode node = this.treeView1.SelectedNode;

                if (node.Level == 0)
                {
                    MessageBox.Show("底层结点不能修改", ShareData.AppName);
                    return;
                }
                else if (node.Nodes.Count > 0)
                {
                    MessageBox.Show("含有子分组的分组无法改名，" 
                        + "\n你可以先把该分组下的子分组删除了，\n然后再改名。",
                        ShareData.AppName);
                    return;
                }

                CeilingDataSet.product_classesRow row = node.Tag as CeilingDataSet.product_classesRow;
                if (row.clone_id > 0)
                    row = row.product_classesRowParentByparent_clone;
                this.productView.RowFilter = "class_id = " + row.ID.ToString();

                if (this.productView.Count > 0)
                {
                    MessageBox.Show("含有产品的分组无法改名，"
                        + "\n你可以先把该分组下的产品删除了，\n然后再改名。",
                        ShareData.AppName);
                    return;
                }

                node.BeginEdit();
            }
        }

        private void SetMenu(bool enable)
        {
            this.删除toolStripButton2.Enabled = enable;
            this.编辑toolStripButton1.Enabled = enable;
            this.新建NToolStripButton.Enabled = enable;
            this.保存SToolStripButton.Enabled = enable;
        }

        private void treeView1_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            SetMenu(false);
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            SetMenu(true);

            if (e.Label == null)
                return;
            CeilingDataSet.product_classesRow row = e.Node.Tag as CeilingDataSet.product_classesRow;

            if (row.clone_id > 0)
                row = row.product_classesRowParentByparent_clone;
            if (row.name != e.Label)
                row.name = e.Label;
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            this.treeView1.SelectedNode = e.Node;
        }

        private void UploadPhoto(CeilingDataSet.productsDataTable products)
        {
            if (products == null)
                return;

            try
            {
                for (int i = 0; i < products.Count; i++)
                {
                    CeilingDataSet.productsRow row = set.products.FindByID(products[i].ID);
                    if (row == null)
                        continue;
                    BuckleNode.UploadPhoto(row);
                    BuckleNode.MovePhoto(row);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private CeilingDataSetTableAdapters.product_classesTableAdapter GetPCsAdapter()
        {
            CeilingDataSetTableAdapters.product_classesTableAdapter pcAdapter 
                = new CeilingDataSetTableAdapters.product_classesTableAdapter();
            pcAdapter.Connection = ShareData.Connection;

            pcAdapter.Adapter.InsertCommand = new MySql.Data.MySqlClient.MySqlCommand();
            pcAdapter.Adapter.InsertCommand.Connection = ShareData.Connection;
            pcAdapter.Adapter.InsertCommand.CommandText = "INSERT INTO `product_classes` (`ID`, `name`, `parent_class_id`, `type`, `category`, `cl" +
                "one_id`, `withchildren`) VALUES (@ID, @name, @parent_class_id, @type, @category, @clo" +
                "ne_id, @withchildren)";
            pcAdapter.Adapter.InsertCommand.CommandType = global::System.Data.CommandType.Text;

            MySql.Data.MySqlClient.MySqlParameter param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@ID";
            param.DbType = global::System.Data.DbType.UInt32;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.UInt32;
            param.IsNullable = true;
            param.SourceColumn = "ID";
            pcAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@name";
            param.DbType = global::System.Data.DbType.String;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.VarChar;
            param.IsNullable = true;
            param.SourceColumn = "name";
            pcAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@parent_class_id";
            param.DbType = global::System.Data.DbType.UInt32;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.UInt32;
            param.IsNullable = true;
            param.SourceColumn = "parent_class_id";
            pcAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@type";
            param.DbType = global::System.Data.DbType.StringFixedLength;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.String;
            param.IsNullable = true;
            param.SourceColumn = "type";
            pcAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@category";
            param.DbType = global::System.Data.DbType.String;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.VarChar;
            param.IsNullable = true;
            param.SourceColumn = "category";
            pcAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@clone_id";
            param.DbType = global::System.Data.DbType.UInt32;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.UInt32;
            param.IsNullable = true;
            param.SourceColumn = "clone_id";
            pcAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@withchildren";
            param.DbType = global::System.Data.DbType.SByte;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.Byte;
            param.IsNullable = true;
            param.SourceColumn = "withchildren";
            pcAdapter.Adapter.InsertCommand.Parameters.Add(param);

            return pcAdapter;
        }

        private CeilingDataSetTableAdapters.productsTableAdapter GetPsAdapter()
        {
            CeilingDataSetTableAdapters.productsTableAdapter pAdapter 
                = new CeilingDataSetTableAdapters.productsTableAdapter();
            pAdapter.Connection = ShareData.Connection;

            pAdapter.Adapter.InsertCommand = new global::MySql.Data.MySqlClient.MySqlCommand();
            pAdapter.Adapter.InsertCommand.Connection = ShareData.Connection;
            pAdapter.Adapter.InsertCommand.CommandText = "INSERT INTO `products` (`ID`, `name`, `pattern`, `color`, `width`, `height`, `length`, " +
                "`unit`, `price`, `remarks`, `class_id`, `photo`) VALUES (@ID, @name, @pattern, @color" +
                ", @width, @height, @length, @unit, @price, @remarks, @class_id, @photo)";
            pAdapter.Adapter.InsertCommand.CommandType = global::System.Data.CommandType.Text;

            MySql.Data.MySqlClient.MySqlParameter param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@ID";
            param.DbType = global::System.Data.DbType.UInt32;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.UInt32;
            param.IsNullable = true;
            param.SourceColumn = "ID";
            param.SourceVersion = global::System.Data.DataRowVersion.Original;
            pAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@name";
            param.DbType = global::System.Data.DbType.String;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.VarChar;
            param.IsNullable = true;
            param.SourceColumn = "name";
            pAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@pattern";
            param.DbType = global::System.Data.DbType.String;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.VarChar;
            param.IsNullable = true;
            param.SourceColumn = "pattern";
            pAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@color";
            param.DbType = global::System.Data.DbType.String;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.VarChar;
            param.IsNullable = true;
            param.SourceColumn = "color";
            pAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@width";
            param.DbType = global::System.Data.DbType.Single;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.Float;
            param.IsNullable = true;
            param.SourceColumn = "width";
            pAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@height";
            param.DbType = global::System.Data.DbType.Single;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.Float;
            param.IsNullable = true;
            param.SourceColumn = "height";
            pAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@length";
            param.DbType = global::System.Data.DbType.Single;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.Float;
            param.IsNullable = true;
            param.SourceColumn = "length";
            pAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@unit";
            param.DbType = global::System.Data.DbType.String;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.VarChar;
            param.IsNullable = true;
            param.SourceColumn = "unit";
            pAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@price";
            param.DbType = global::System.Data.DbType.Single;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.Float;
            param.IsNullable = true;
            param.SourceColumn = "price";
            pAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@remarks";
            param.DbType = global::System.Data.DbType.String;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.VarChar;
            param.IsNullable = true;
            param.SourceColumn = "remarks";
            pAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@class_id";
            param.DbType = global::System.Data.DbType.UInt32;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.UInt32;
            param.IsNullable = true;
            param.SourceColumn = "class_id";
            pAdapter.Adapter.InsertCommand.Parameters.Add(param);
            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@photo";
            param.DbType = global::System.Data.DbType.String;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.VarChar;
            param.IsNullable = true;
            param.SourceColumn = "photo";
            pAdapter.Adapter.InsertCommand.Parameters.Add(param);

            pAdapter.Adapter.DeleteCommand = new global::MySql.Data.MySqlClient.MySqlCommand();
            pAdapter.Adapter.DeleteCommand.Connection = ShareData.Connection;
            pAdapter.Adapter.DeleteCommand.CommandText = @"DELETE FROM `products` WHERE (`ID` = @Original_ID)";
            //pAdapter.Adapter.DeleteCommand.CommandText = @"DELETE FROM `products` WHERE ((`ID` = @Original_ID) AND ((@IsNull_name = 1 AND `name` IS NULL) OR (`name` = @Original_name)) AND ((@IsNull_pattern = 1 AND `pattern` IS NULL) OR (`pattern` = @Original_pattern)) AND ((@IsNull_color = 1 AND `color` IS NULL) OR (`color` = @Original_color)) AND ((@IsNull_width = 1 AND `width` IS NULL) OR (`width` = @Original_width)) AND ((@IsNull_height = 1 AND `height` IS NULL) OR (`height` = @Original_height)) AND ((@IsNull_length = 1 AND `length` IS NULL) OR (`length` = @Original_length)) AND ((@IsNull_unit = 1 AND `unit` IS NULL) OR (`unit` = @Original_unit)) AND ((@IsNull_price = 1 AND `price` IS NULL) OR (`price` = @Original_price)) AND ((@IsNull_remarks = 1 AND `remarks` IS NULL) OR (`remarks` = @Original_remarks)) AND ((@IsNull_class_id = 1 AND `class_id` IS NULL) OR (`class_id` = @Original_class_id)) AND ((@IsNull_photo = 1 AND `photo` IS NULL) OR (`photo` = @Original_photo)))";
            pAdapter.Adapter.DeleteCommand.CommandType = global::System.Data.CommandType.Text;

            param = new global::MySql.Data.MySqlClient.MySqlParameter();
            param.ParameterName = "@Original_ID";
            param.DbType = global::System.Data.DbType.UInt32;
            param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.UInt32;
            param.IsNullable = true;
            param.SourceColumn = "ID";
            param.SourceVersion = global::System.Data.DataRowVersion.Original;
            pAdapter.Adapter.DeleteCommand.Parameters.Add(param);

            return pAdapter;
        }

        #region 复制粘贴

        private bool cut = false;

        private BuckleNode copyProduct = null;

        public BuckleNode CopyProduct
        {
            set
            {
                if (copyProduct != null)
                    copyProduct.BackColor = Color.White;
                copyProduct = value;
                cut = false;
            }
        }

        public BuckleNode CutProduct
        {
            set
            {
                if (copyProduct != null)
                    copyProduct.BackColor = Color.White;
                copyProduct = value;
                if (copyProduct != null)
                    copyProduct.BackColor = Color.Gray;
                cut = true;
            }
        }

        private void 剪切UToolStripButton_Click(object sender, EventArgs e)
        {
            if (BuckleList.Current != null)
            {
                CutProduct = BuckleList.Current;
                BuckleList.Current = null;
                //CopyTreeNode = null;
                //current.OriginalRow.Delete();
                //this.splitContainer1.Panel2.Controls.Remove(current);
            }
            //else if (this.treeView1.SelectedNode != null)
            //{
            //    CopyProduct = null;
            //    CutTreeNode = this.treeView1.SelectedNode;
            //    //palaceDataSet.product_classesRow row = copyTreeNode.Tag as palaceDataSet.product_classesRow;
            //    //row.Delete();
            //    //copyTreeNode.Remove();
            //}
        }

        private void 复制CToolStripButton_Click(object sender, EventArgs e)
        {
            if (BuckleList.Current != null)
            {
                CopyProduct = BuckleList.Current;
                BuckleList.Current = null;
                //CopyTreeNode = null;
                //current.OriginalRow.Delete();
                //this.splitContainer1.Panel2.Controls.Remove(current);
            }
            //else if (this.treeView1.SelectedNode != null)
            //{
            //    CopyProduct = null;
            //    CopyTreeNode = this.treeView1.SelectedNode;
            //    //palaceDataSet.product_classesRow row = copyTreeNode.Tag as palaceDataSet.product_classesRow;
            //    //row.Delete();
            //    //copyTreeNode.Remove();
            //}
        }

        private void 粘贴PToolStripButton_Click(object sender, EventArgs e)
        {
            if (copyProduct != null)
            {
                CeilingDataSet.productsRow orow = copyProduct.OriginalRow;
                CeilingDataSet.productsRow prow = set.products.NewproductsRow();
                prow.name = orow.name;
                prow.width = orow.width;
                prow.height = orow.height;

                if (!orow.IspatternNull())
                    prow.pattern = orow.pattern;
                if (!orow.IspriceNull())
                    prow.price = orow.price;
                if (!orow.IsunitNull())
                    prow.unit = orow.unit;
                if (!orow.IsremarksNull())
                    prow.remarks = orow.remarks;
                if (!orow.IscolorNull())
                    prow.color = orow.color;
                if (!orow.IsphotoNull())
                    prow.photo = orow.photo;

                CeilingDataSet.product_classesRow crow = this.treeView1.SelectedNode.Tag as CeilingDataSet.product_classesRow;
                prow.class_id = crow.ID;
                prow.ID = MaxProductID(crow);
                set.products.AddproductsRow(prow);

                BuckleNode.SavePhoto(prow, copyProduct.OriginalImage);
                //ProductNode.SavePhoto(ProductNode.PhotoPath(prow.product_classesRow), prow.photo, copyProduct.OriginalImage);
                BuckleNode node = new BuckleNode(prow);
                AddProduct(node);

                if (cut)
                    orow.Delete();
            }
        }

        #endregion

        #region 进度条

        private void _progressIncrease()
        {
            if (this.progressBar1.Maximum - this.progressBar1.Value < this.progressBar1.Step * 5)
            {
                this.progressBar1.Step /= 2;
                this.msec *= 2;
                if (this.progressBar1.Step < 1)
                    this.progressBar1.Step = 1;
            }
            this.progressBar1.PerformStep();
        }

        private void _progressCompleted()
        {
            this.panel2.Visible = false;
            this.splitContainer1.Enabled = true;
        }

        private void progressStart(IAsyncResult asyncResult, Action close, int _msec)
        {
            // 本函数会执行两次，第一次在主线程中，this.InvokeRequired 是 false，
            // 第二次在辅助线程中，this.InvokeRequired 是 true
            if (!this.InvokeRequired) // 在主线程中，重新开一个线程操控进度条
            {
                this.msec = _msec;
                this.splitContainer1.Enabled = false;
                this.panel2.Visible = true;

                this.progressBar1.Value = 0;
                this.progressBar1.Step = 10;
                _progressIncrease();

                Action func = () => progressStart(asyncResult, close, 200);
                func.BeginInvoke(null, null);
            }
            else // 在辅助线程中，循环推进进度条
            {
                while (!asyncResult.IsCompleted)
                {
                    System.Threading.Thread.Sleep(msec);
                    this.BeginInvoke(new Action(_progressIncrease));
                }

                this.BeginInvoke(new Action(_progressCompleted), null);
                if (close != null)
                {
                    this.BeginInvoke(close, null);
                }
            }
        }

        #endregion

        private void Save()
        {
            this.changed = true;

            if (ShareData.HasNetServer)
            {
                try
                {
                    CeilingDataSetTableAdapters.product_classesTableAdapter pcAdapter 
                        = GetPCsAdapter();
                    pcAdapter.Update(set.product_classes);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
                //finally
                //{
                //    set.product_classes.AcceptChanges();
                //}

                try
                {
                    DataTable added = set.products.GetChanges(DataRowState.Added);
                    if (added != null)
                    {
                        CeilingDataSet.productsDataTable products = added as CeilingDataSet.productsDataTable;
                        Action<CeilingDataSet.productsDataTable> func = (result) => UploadPhoto(result);

                        IAsyncResult asyncResult = func.BeginInvoke(products, (result) =>
                        {
                            func.EndInvoke(result);
                        }, null);
                    }

                    BuckleList.RemoveProducts();
                    CeilingDataSetTableAdapters.productsTableAdapter pAdapter 
                        = GetPsAdapter();
                    pAdapter.Update(set.products);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
                //finally
                //{
                //    set.products.AcceptChanges();
                //}
            }

            set.products.AcceptChanges();
            set.product_classes.AcceptChanges();
            BuckleList.WriteXml(set);
        }

        private void 保存SToolStripButton_Click(object sender, EventArgs e)
        {
            if (!this.HasChanged)
                return;
            Action func = () => Save();

            IAsyncResult asyncResult = func.BeginInvoke((result) =>
            {
                func.EndInvoke(result);
            }, null);
            this.progressStart(asyncResult, null, 200);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.HasChanged)
                return;
            e.Cancel = true;
            DialogResult dr = MessageBox.Show("数据已经改变，是否要保存？", 
                ShareData.AppName, MessageBoxButtons.YesNoCancel);

            if (dr == System.Windows.Forms.DialogResult.No)
            {
                e.Cancel = false;

                set.product_classes.RejectChanges();
                set.products.RejectChanges();
            }
            else if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                Action func = () => Save();

                IAsyncResult asyncResult = func.BeginInvoke((result) =>
                {
                    func.EndInvoke(result);
                }, null);

                this.progressStart(asyncResult, () =>
                {
                    this.Close();
                }, 200);
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            SetMenu(true);
            BuckleList.Current = null;
        }
    }
}

//this.删除toolStripButton2.Enabled = false;
//this.编辑toolStripButton1.Enabled = false;
//this.新建NToolStripButton.Enabled = false;
//this.保存SToolStripButton.Enabled = false;

//this.删除toolStripButton2.Enabled = true;
//this.编辑toolStripButton1.Enabled = true;
//this.新建NToolStripButton.Enabled = true;
//this.保存SToolStripButton.Enabled = true;

//Encoding utf8 = Encoding.GetEncoding("utf-8");
//string path = ProductNode.PhotoPath(row.product_classesRow);
//string _url = "http://tianezhen.com/AUPU/move.php?file=" + row.photo + "&path=" + path;
//System.Net.WebRequest request = System.Net.WebRequest.Create(_url);
//request.GetResponse();

//private TreeNode copyTreeNode = null;

//public TreeNode CopyTreeNode
//{
//    set
//    {
//        if (copyTreeNode != null)
//            copyTreeNode.BackColor = Color.White;
//        copyTreeNode = value;
//        cut = false;
//    }
//}

//public TreeNode CutTreeNode
//{
//    set
//    {
//        if (copyTreeNode != null)
//            copyTreeNode.BackColor = Color.White;
//        copyTreeNode = value;
//        if (copyTreeNode != null)
//            copyTreeNode.BackColor = Color.Gray;
//        cut = true;
//    }
//}

//MySql.Data.MySqlClient.MySqlDataAdapter adapter = GetInsertAdapter();
//UploadPhoto(added as palaceDataSet.productsDataTable);

//private MySql.Data.MySqlClient.MySqlDataAdapter GetInsertPCsAdapter()
//{
//    MySql.Data.MySqlClient.MySqlParameter param;
//    MySql.Data.MySqlClient.MySqlDataAdapter adapter = new MySql.Data.MySqlClient.MySqlDataAdapter();

//    adapter.InsertCommand = new MySql.Data.MySqlClient.MySqlCommand();
//    adapter.InsertCommand.Connection = ShareData.Connection;
//    adapter.InsertCommand.CommandText = "INSERT INTO `product_classes` (`ID`, `name`, `parent_class_id`, `type`, `category`, `cl" +
//        "one_id`, `withchildren`) VALUES (@ID, @name, @parent_class_id, @type, @category, @clo" +
//        "ne_id, @withchildren)";
//    adapter.InsertCommand.CommandType = global::System.Data.CommandType.Text;

//    param = new global::MySql.Data.MySqlClient.MySqlParameter();
//    param.ParameterName = "@ID";
//    param.DbType = global::System.Data.DbType.UInt32;
//    param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.UInt32;
//    param.IsNullable = true;
//    param.SourceColumn = "ID";
//    adapter.InsertCommand.Parameters.Add(param);
//    param = new global::MySql.Data.MySqlClient.MySqlParameter();
//    param.ParameterName = "@name";
//    param.DbType = global::System.Data.DbType.String;
//    param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.VarChar;
//    param.IsNullable = true;
//    param.SourceColumn = "name";
//    adapter.InsertCommand.Parameters.Add(param);
//    param = new global::MySql.Data.MySqlClient.MySqlParameter();
//    param.ParameterName = "@parent_class_id";
//    param.DbType = global::System.Data.DbType.UInt32;
//    param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.UInt32;
//    param.IsNullable = true;
//    param.SourceColumn = "parent_class_id";
//    adapter.InsertCommand.Parameters.Add(param);
//    param = new global::MySql.Data.MySqlClient.MySqlParameter();
//    param.ParameterName = "@type";
//    param.DbType = global::System.Data.DbType.StringFixedLength;
//    param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.String;
//    param.IsNullable = true;
//    param.SourceColumn = "type";
//    adapter.InsertCommand.Parameters.Add(param);
//    param = new global::MySql.Data.MySqlClient.MySqlParameter();
//    param.ParameterName = "@category";
//    param.DbType = global::System.Data.DbType.String;
//    param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.VarChar;
//    param.IsNullable = true;
//    param.SourceColumn = "category";
//    adapter.InsertCommand.Parameters.Add(param);
//    param = new global::MySql.Data.MySqlClient.MySqlParameter();
//    param.ParameterName = "@clone_id";
//    param.DbType = global::System.Data.DbType.UInt32;
//    param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.UInt32;
//    param.IsNullable = true;
//    param.SourceColumn = "clone_id";
//    adapter.InsertCommand.Parameters.Add(param);
//    param = new global::MySql.Data.MySqlClient.MySqlParameter();
//    param.ParameterName = "@withchildren";
//    param.DbType = global::System.Data.DbType.SByte;
//    param.MySqlDbType = global::MySql.Data.MySqlClient.MySqlDbType.Byte;
//    param.IsNullable = true;
//    param.SourceColumn = "withchildren";
//    adapter.InsertCommand.Parameters.Add(param);
//    adapter.InsertCommand.Connection = ShareData.Connection;

//    palaceDataSetTableAdapters.product_classesTableAdapter pcAdapter = new palaceDataSetTableAdapters.product_classesTableAdapter();
//    pcAdapter.Connection = ShareData.Connection;
//    adapter.SelectCommand = pcAdapter.Adapter.SelectCommand;
//    adapter.DeleteCommand = pcAdapter.Adapter.DeleteCommand;
//    adapter.UpdateCommand = pcAdapter.Adapter.UpdateCommand;
//    return adapter;
//}

//palaceDataSetTableAdapters.productsTableAdapter pAdapter = new palaceDataSetTableAdapters.productsTableAdapter();
//pAdapter.Connection = ShareData.Connection;
//pAdapter.Adapter.DeleteCommand.CommandText = @"DELETE FROM `products` WHERE (`ID` = @Original_ID)";
//pAdapter.Adapter.DeleteCommand.CommandText = @"DELETE FROM `products` WHERE ((`ID` = @Original_ID) AND ((@IsNull_name = 1 AND `name` IS NULL) OR (`name` = @Original_name)) AND ((@IsNull_width = 1 AND `width` IS NULL) OR (`width` = @Original_width)) AND ((@IsNull_height = 1 AND `height` IS NULL) OR (`height` = @Original_height)) AND ((@IsNull_price = 1 AND `price` IS NULL) OR (`price` = @Original_price)) AND ((@IsNull_class_id = 1 AND `class_id` IS NULL) OR (`class_id` = @Original_class_id)) AND ((@IsNull_photo = 1 AND `photo` IS NULL) OR (`photo` = @Original_photo)))";
//pAdapter.Adapter.DeleteCommand.CommandText = @"DELETE FROM `products` WHERE ((`ID` = @Original_ID) AND ((@IsNull_name = 1 AND `name` IS NULL) OR (`name` = @Original_name)) AND ((@IsNull_pattern = 1 AND `pattern` IS NULL) OR (`pattern` = @Original_pattern)) AND ((@IsNull_color = 1 AND `color` IS NULL) OR (`color` = @Original_color)) AND ((@IsNull_width = 1 AND `width` IS NULL) OR (`width` = @Original_width)) AND ((@IsNull_height = 1 AND `height` IS NULL) OR (`height` = @Original_height)) AND ((@IsNull_length = 1 AND `length` IS NULL) OR (`length` = @Original_length)) AND ((@IsNull_unit = 1 AND `unit` IS NULL) OR (`unit` = @Original_unit)) AND ((@IsNull_price = 1 AND `price` IS NULL) OR (`price` = @Original_price)) AND ((@IsNull_remarks = 1 AND `remarks` IS NULL) OR (`remarks` = @Original_remarks)) AND ((@IsNull_class_id = 1 AND `class_id` IS NULL) OR (`class_id` = @Original_class_id)) AND ((@IsNull_photo = 1 AND `photo` IS NULL) OR (`photo` = @Original_photo)))";

//private void _test()
//{
//    System.Threading.Thread.Sleep(5000);
//}

//this.progressBar1.Text = desc;
//this.progressBar1.Text = (int)((float)this.progressBar1.Value * 100.0f / (float)this.progressBar1.Maximum) + "%";

//if (this.msec >= 100)
//{
//    Action func = () => _test();
//    IAsyncResult asyncResult = func.BeginInvoke((result) => { }, null);
//    this.progressStart(asyncResult, () =>
//    {
//        this.msec = 0;
//        this.Close();
//    }, 250);
//    e.Cancel = true;
//}

//if (dr == System.Windows.Forms.DialogResult.Cancel)
//{
//    e.Cancel = true;
//}
//else if (dr == System.Windows.Forms.DialogResult.Yes)
//{
//    this.msec = 100;
//    Action func = () => Save();
//    IAsyncResult asyncResult = func.BeginInvoke((result) => { }, null);
//    this.progressStart(asyncResult);
//    e.Cancel = true;
//}
//else
//{
//    set.product_classes.RejectChanges();
//    set.products.RejectChanges();
//}

//if (this.msec == 0)
//{
//    this.msec = 100;
//    Action func = () => _test();
//    IAsyncResult asyncResult = func.BeginInvoke((result) => { }, null);
//    this.progressStart(asyncResult);
//    e.Cancel = true;
//}

//private void progressStart(IAsyncResult asyncResult, int msec = 150)
//{
//    this.splitContainer1.Enabled = false;
//    this.panel2.Visible = true;

//    this.progressBar1.Value = 0;
//    this.progressBar1.Step = 10;
//    _progressIncrease();

//    while (!asyncResult.IsCompleted)
//    {
//        System.Threading.Thread.Sleep(msec);
//        _progressIncrease();
//    }

//    this.splitContainer1.Enabled = true;
//    this.panel2.Visible = false;
//}

//private void progressStart(int msec = 150)
//{
//    // 本函数会执行两次，第一次在主线程中，this.InvokeRequired 是 false，
//    // 第二次在辅助线程中，this.InvokeRequired 是 true
//    if (!this.InvokeRequired) // 在主线程中，重新开一个线程操控进度条
//    {
//        this.splitContainer1.Enabled = false;
//        this.panel2.Visible = true;

//        this.progressBar1.Value = 0;
//        this.progressBar1.Step = 10;
//        _progressIncrease();

//        Action func = () => progressStart(msec);
//        func.BeginInvoke(null, null);
//    }
//    else // 在辅助线程中，循环推进进度条
//    {
//        while (!this.saved)
//        {
//            System.Threading.Thread.Sleep(msec);
//            this.BeginInvoke(new Action(_progressIncrease));
//        }

//        this.BeginInvoke(new Action(_progressCompleted), null);
//    }
//}

//try
//{
//    MySql.Data.MySqlClient.MySqlDataAdapter adapter = GetInsertAdapter();
//    adapter.Update(set.product_classes);
//    set.product_classes.AcceptChanges();
//    //GetInsertAdapter().Update(set.product_classes);
//}
//catch (Exception ex)
//{
//    System.Diagnostics.Debug.WriteLine(ex);
//}

//palaceDataSetTableAdapters.productsTableAdapter pAdapter = new palaceDataSetTableAdapters.productsTableAdapter();
//pAdapter.Connection = ShareData.Connection;
//try
//{
//    DataTable added = set.products.GetChanges(DataRowState.Added);
//    if (added != null)
//        UploadPhoto(added as palaceDataSet.productsDataTable);
//    ProductList.RemoveProducts();
//    pAdapter.Update(set.products);
//    set.products.AcceptChanges();
//}
//catch (Exception ex)
//{
//    System.Diagnostics.Debug.WriteLine(ex);
//}

////set.AcceptChanges();
//ProductList.WriteXml(set);

//private void SaveTip()
//{
//    this.splitContainer1.Enabled = false;
//    this.panel2.Visible = true;
//    //this.progressStart();
//}

//adapter.SelectCommand = new global::MySql.Data.MySqlClient.MySqlCommand();
//adapter.SelectCommand.Connection = ShareData.Connection;
//adapter.SelectCommand.CommandText = "SELECT ID, name, parent_class_id, type, category, clone_id, withchildren\r\nFROM pr oduct_classes";
//adapter.SelectCommand.CommandType = global::System.Data.CommandType.Text;

//if (node.Level == 0)
//{
//    //DataView view = new DataView();
//    //view.Table = set.product_classes;
//    //view.RowFilter = "type = 'surface' and parent_class_id = 100000000 and name = ";
//    crow = AddChildClass(100000000);
//    //CloneClass(parent_row.ID, crow.ID);
//}
//else
//{
//    crow = AddChildClass(parent_row.ID);
//}

//this.label1.Text = ProductNode.PhotoPath(row);
//this.label1.Location = new Point(0, 0);
//this.panel1.Location = new Point(0, 20);

//pNode.MouseClick += new MouseEventHandler(pNode_MouseClick);
//pNode.Click += new EventHandler(pNode_Click);
//int i = this.panel1.Controls.Count, width = 120, rows = this.panel1.Height / width;
//pNode.Location = new Point((i / rows) * width + 10, (i % rows) * width + 10);

//private void _treeNode(TreeNodeCollection nodes)//(TreeNode node)
//{
//    for (int i = 0; i < nodes.Count; i++)
//    {
//        nodes[i].ContextMenuStrip = this.contextMenuStrip2;
//        _treeNode(nodes[i].Nodes);
//    }
//}

//uint max_id = MaxClassID();
//max_id = max_id / 100000 + 1;
//max_id = max_id * 100000;

//palaceDataSet.product_classesRow prow = node.Tag as palaceDataSet.product_classesRow;
//palaceDataSet.product_classesRow crow = set.product_classes.Newproduct_classesRow();
//crow.ID = MaxClassID() + 100000;
//crow.name = "未命名分组";
//crow.parent_class_id = prow.ID;
//crow.type = "surface";
//crow.clone_id = 0;
//set.product_classes.Addproduct_classesRow(crow);

//this.treeView1.SelectedNode.LastNode.BeginEdit();

//string path = ProductNode.PhotoPath(crow);
//string[] _ps = path.Split('/');
//string _path = ShareData.GetDataPath();

//for (int i = 0; i < _ps.Length; i++)
//{
//    _path += _ps[i] + "/";
//    if (!System.IO.Directory.Exists(_path))
//        System.IO.Directory.CreateDirectory(_path);
//}

//private void InsertProduct2DB(palaceDataSet.product_classesDataTable pcs)
//{
//    MySql.Data.MySqlClient.MySqlDataAdapter adapter = GetInsertAdapter();
//    adapter.Update(pcs);
//    pcs.AcceptChanges();
//}

//palaceDataSetTableAdapters.product_classesTableAdapter pcAdapter = new palaceDataSetTableAdapters.product_classesTableAdapter();
//pcAdapter.Connection = ShareData.Connection;
//try
//{
//    palaceDataSet.product_classesDataTable pc = set.product_classes.GetChanges(DataRowState.Added) as palaceDataSet.product_classesDataTable;
//    if (pc != null && pc.Count > 0)
//    {
//        GetInsertAdapter().Update(pc);
//        pc.AcceptChanges();
//    }
//    pcAdapter.Update(set.product_classes);
//}
//catch (Exception ex)
//{
//    System.Diagnostics.Debug.WriteLine(ex);
//}

//ProductNode current = null;

//public ProductNode Current
//{
//    get { return current; }
//    set
//    {
//        if (current != null)
//        {
//            if (current.IsEditing)
//                current.UnEdit();
//            current.BackColor = Color.Transparent;
//            current.ForeColor = Color.Black;
//        }
//        current = value;
//        if (current != null)
//        {
//            current.BackColor = SystemColors.ActiveCaption;
//            current.ForeColor = Color.White;
//        }
//    }
//}

//string path = System.Web.HttpUtility.UrlEncode(ProductNode.PhotoPath(row.product_classesRow), utf8);
//string _url = "http://tianezhen.com/AUPU/move.php?file=" + System.Web.HttpUtility.UrlEncode(row.photo, utf8) + "&path=tmp/" + path;
//System.Net.WebRequest request = System.Net.WebRequest.Create(_url);
//request.Method = "GET";
//request.GetResponse();

//int i = this.splitContainer1.Panel2.Controls.Count;
//int width = 120, rows = this.splitContainer1.Panel2.Height / width;
//this.splitContainer1.Panel2.Controls.Add(pNode);

//if (e.Node.Tag is palaceDataSet.product_classesRow)
//{
//    palaceDataSet.product_classesRow row = e.Node.Tag as palaceDataSet.product_classesRow;
//    this.productView.RowFilter = "class_id = " + row.ID.ToString();
//    //int width = 120, rows = this.splitContainer1.Panel2.Height / width;

//    for (int i = 0; i < this.productView.Count; i++)
//    {
//        ProductNode pNode = new ProductNode(this.productView[i].Row as palaceDataSet.productsRow);
//        AddProduct(pNode);
//    }
//}

//for (int i = 0; i < ctrls.Count; i++)
//{
//    ProductNode pNode = ctrls[i] as ProductNode;
//    pNode.MouseDown += new MouseEventHandler(pNode_MouseDown);
//    pNode.MouseUp += new MouseEventHandler(pNode_MouseUp);
//    pNode.MouseMove += new MouseEventHandler(pNode_MouseMove);
//    pNode.MouseHover += new EventHandler(pNode_MouseHover);
//    pNode.ContextMenuStrip = this.productContextMenuStrip;
//}

//PictureBox box = new PictureBox();
//box.Size = new System.Drawing.Size(300, 300);
//box.SizeMode = PictureBoxSizeMode.Zoom;
//box.Location = new Point(400, 50);
//box.Image = bmp;
//box.BackColor = Color.Black;
//this.splitContainer1.Panel2.Controls.Add(box);
//bmp.Save("D://32dsfs.jpg");

//TextBox box = new TextBox();
//box.Text = this.treeView1.SelectedNode.Text;
//this.treeView1.Controls.Add(box);
//box.Location = this.treeView1.SelectedNode.IsEditing
