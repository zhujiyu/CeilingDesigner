using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace CeilingDesigner
{
    public partial class PalaceForm : Form
    {
        #region 属性

        public Cursor penCur = null;//new Cursor(@"../images/pen_r.cur");
        public Cursor hMoveCur = null;// = new Cursor(@"../images/hmove.cur");
        private HelpCtrl helpCtrl1 = null;

        Order order = null; // = new Order();

        public Order Order
        {
            get { return order; }
            //set { order = value; }
        }

        public OrderGraph CurOrderGraph
        {
            get
            {
                for (int i = 0; i < this.order.OrderGraphs.Count; i++)
                {
                    OrderGraph graph = this.order.OrderGraphs[i];
                    ToolStripMenuItem item = graph.Tag as ToolStripMenuItem;
                    if (item.Checked)
                        return graph;
                }
                return null;
            }
        }

        //private List<Action> ActionList = new List<Action>();
        //private DataView prodView = new DataView();
        //private DataView classView = new DataView();

        private WallEdit wallEdit = null;

        public WallEdit WallEdit
        {
            get
            {
                if (wallEdit == null)
                    _NewWallEdit();
                return this.wallEdit;
            }
        }

        public ToolStripLabel ScaleLabel
        {
            get { return this.progressScaleLabel; }
        }

        #endregion

        #region 加载数据

        private void SetDataSource(string str)
        {
            this.mysqlConnectionStatus.Text = str;
        }

        private void SetSystemStatus(string str)
        {
            this.systemStatusLabel.Text = str;
        }

        void PalaceForm_LoadDataFromFile(object sender, LoadDataEventArgs e)
        {
            this.loadCurs();
            this.systemStatusLabel.Text = "加载数据...";

            Func<CeilingDataSet, CeilingDataSet> func = (set) => BuckleList.DataReading(set);

            IAsyncResult asyncResult = func.BeginInvoke(ShareData.CeilingDataSet, (result) =>
            {
                CeilingDataSet set = func.EndInvoke(result);

                if (set == null || set.product_classes.Count < 1 || set.products.Count < 1
                    || set.ceiling_samples.Count < 1 || set.ceiling_sample_walles.Count < 1)
                {
                    this.BeginInvoke(new Action<string>(SetDataSource), "没有本地数据");
                    if (e != null && e.LoadData != null)
                        this.BeginInvoke(e.LoadData, this, new LoadDataEventArgs(e.Save));
                }
                else
                {
                    this.BeginInvoke(new Action<string>(SetDataSource), "本地数据");
                    this.InitProducts();
                }

                this.BeginInvoke(new Action<string>(SetSystemStatus), "就绪");
            }, null);
        }

        void PalaceForm_LoadDataFromDB(object sender, LoadDataEventArgs e)
        {
            this.loadCurs();
            this.systemStatusLabel.Text = "加载数据...";

            Func<CeilingDataSet, CeilingDataSet> func = (set) => BuckleList.DataLoading(set);

            IAsyncResult asyncResult = func.BeginInvoke(ShareData.CeilingDataSet, (result) =>
            {
                CeilingDataSet set = func.EndInvoke(result);

                if (set == null || set.product_classes.Count < 1 || set.products.Count < 1
                    || set.ceiling_samples.Count < 1 || set.ceiling_sample_walles.Count < 1)
                {
                    this.BeginInvoke(new Action<string>(SetDataSource), "无法连接服务器");
                    if (e.LoadData != null)
                        this.BeginInvoke(e.LoadData, this, new LoadDataEventArgs());
                }
                else
                {
                    this.BeginInvoke(new Action<CeilingDataSet, bool>(_updateLocalData), 
                        set, e.Save);
                    this.BeginInvoke(new Action<string>(SetDataSource), 
                        ShareData.Server);
                    this.order.InitMysqlDB();
                    ShareData.HasNetServer = true;
                    this.InitProducts();
                }

                this.BeginInvoke(new Action<string>(SetSystemStatus), "就绪");
            }, null);
        }

        private void InitProducts()
        {
            CeilingDataSet set = ShareData.CeilingDataSet;
            BuckleList.List.Clear();

            if (set == null || set.products.Count < 1
                || set.product_classes.Count < 1)
                return;

            for (int i = 0; i < set.products.Count; i++)
            {
                CeilingDataSet.productsRow product = set.products[i];
                if (product.product_classesRow == null)
                    continue;
                if (product.product_classesRow.type != "surface")
                    continue;

                BuckleNode node = BuckleList.GetProductNode(product.ID);
                if (node != null)
                    continue;
                AddProductNode(product);
            }

            this.BeginInvoke(new Action(_dataLoaded));
        }

        private void _dataLoaded()
        {
            BuckleList.DispPClass(ShareData.CeilingDataSet.product_classes, 
                treeView1);
        }

        private void _updateLocalData(CeilingDataSet set, bool _save)
        {
            if (_save || MessageBox.Show("是否同步更新本地数据？",
                ShareData.AppName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                BuckleList.WriteXml(set);
        }

        #endregion

        public PalaceForm()
        {
            InitializeComponent();
            ShareData.form = this;
        }

        private void _InitUI()
        {
            this.movePicture.SizeMode = PictureBoxSizeMode.StretchImage;
            this.movePicture.BorderStyle = BorderStyle.FixedSingle;
            this.orderPositoinLabel.Text = "0, 0";
            this.systemStatusLabel.Text = "初始化中...";
            this.productPanel1.AutoSize = true;

            this.保存SToolStripButton.Enabled = false;
            this.保存SToolStripMenuItem.Enabled = false;

            this.toolStripMenuItem1.Tag = TileStyle.OnRows;
            this.toolStripMenuItem3.Tag = TileStyle.OnOddRows;
            this.toolStripMenuItem4.Tag = TileStyle.OnOddColumns;
            this.toolStripMenuItem5.Tag = TileStyle.OnEvenRows;
            this.toolStripMenuItem6.Tag = TileStyle.OnEvenColumns;

            this.奇数行奇排列ToolStripMenuItem.Tag = TileStyle.OnOddRowOdd;
            this.奇数行偶排列ToolStripMenuItem.Tag = TileStyle.OnOddRowEven;
            this.偶数行奇排列ToolStripMenuItem.Tag = TileStyle.OnEvenRowOdd;
            this.偶数行偶排列ToolStripMenuItem.Tag = TileStyle.OnEvenRowEven;
        }

        private void _HelpCtrl()
        {
            if (HelpCtrl.IsHide())
                return;
            this.helpCtrl1 = new HelpCtrl();
            this.helpCtrl1.BackColor = SystemColors.Control;
            this.helpCtrl1.BorderStyle = BorderStyle.FixedSingle;

            this.helpCtrl1.Location =
                new Point((this.mainPanel.Width - this.helpCtrl1.Width) / 2, 
                    (mainPanel.Height - helpCtrl1.Height) / 2 + 100);

            this.helpCtrl1.Name = "helpCtrl1";
            this.helpCtrl1.TabIndex = 0;
            this.mainPanel.Controls.Add(this.helpCtrl1);

            SetEditMenu(false);
            SetDoMenu(false);
            SetCeilingMenu(false);
            SetManageMenu(false);
            SetWallMenu(false);
            //SetTileModuleMenu(false);
            SetOtherMenu(false);
        }

        private void _NewWallEdit()
        {
            this.wallEdit = new CeilingDesigner.WallEdit();
            this.wallEdit.Visible = false;

            this.wallEdit.AutoScroll = true;
            this.wallEdit.BackColor = SystemColors.Control;
            this.wallEdit.Dock = DockStyle.Bottom;
            this.wallEdit.Location = new System.Drawing.Point(0, 547);
            this.wallEdit.Name = "wallEditer1";
            this.wallEdit.Size = new System.Drawing.Size(772, 100);
            this.wallEdit.TabIndex = 4;

            this.wallEdit.modify += new CeilingDesigner.KeelChangeEventHandler(this.WallEditer_modify);
            this.wallEdit.excape += new CeilingDesigner.ExcapeEvnetHandler(this.WallEditer_excape);
            this.baseSplitContainer.Panel1.Controls.Add(this.wallEdit);
        }

        private void _PrepareOrder()
        {
            ShareData.CeilingDataSet = new CeilingDataSet();
            this.order = new Order(this);
            this.order.NewOrder();
        }

        private void PalaceForm_Load(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(_InitUI), null);
            this.BeginInvoke(
                new Action<PalaceForm, LoadDataEventArgs>(PalaceForm_LoadDataFromFile),
                this, null);
            _HelpCtrl();
            _PrepareOrder();
        }

        public void SetOtherMenu(bool enabled)
        {
            this.新建NToolStripButton.Enabled = enabled;
            this.新建NToolStripMenuItem.Enabled = enabled;

            this.打开OToolStripButton.Enabled = enabled;
            this.打开OToolStripMenuItem.Enabled = enabled;

            this.网格ToolStripMenuItem.Enabled = enabled;
            this.关闭CToolStripMenuItem.Enabled = enabled;
            this.另存为ToolStripMenuItem.Enabled = enabled;
        }

        public void SetCeilingMenu(bool enabled)
        {
            this.刷新ToolStripButton.Enabled = enabled;
            this.刷新ToolStripMenuItem1.Enabled = enabled;
            this.刷新ToolStripMenuItem2.Enabled = enabled;

            this.设置龙骨ToolStripButton.Enabled = enabled;
            this.设置龙骨ToolStripMenuItem1.Enabled = enabled;
            this.设置龙骨ToolStripMenuItem2.Enabled = enabled;

            this.对齐ToolStripButton.Enabled = enabled;
            this.对齐ToolStripMenuItem.Enabled = enabled;

            this.模块ToolStripButton1.Enabled = enabled;
            this.模块ToolStripMenuItem1.Enabled = enabled;
            this.模块ToolStripMenuItem2.Enabled = enabled;

            this.清空ToolStripButton.Enabled = enabled;
            this.清空ToolStripMenuItem1.Enabled = enabled;
            this.清空ToolStripMenuItem2.Enabled = enabled;

            this.旋转ToolStripButton.Enabled = enabled;
            this.旋转ToolStripMenuItem1.Enabled = enabled;
            this.旋转ToolStripMenuItem2.Enabled = enabled;

            this.导出图纸ToolStripMenuItem1.Enabled = enabled;
            this.导出图纸ToolStripMenuItem2.Enabled = enabled;

            this.添加图纸ToolStripMenuItem1.Enabled = enabled;
            this.添加图纸ToolStripMenuItem2.Enabled = enabled;

            this.删除图纸ToolStripMenuItem1.Enabled = enabled;
            this.删除图纸ToolStripMenuItem2.Enabled = enabled;
        }

        public void SetManageMenu(bool enabled)
        {
            this.统计toolStripButton4.Enabled = enabled;
            this.统计ToolStripMenuItem1.Enabled = enabled;
            this.统计ToolStripMenuItem2.Enabled = enabled;

            this.报表LToolStripButton.Enabled = enabled;
            this.报表ToolStripMenuItem1.Enabled = enabled;
            this.报表ToolStripMenuItem2.Enabled = enabled;

            this.产品管理ToolStripButton.Enabled = enabled;
            this.产品管理ToolStripMenuItem.Enabled = enabled;
        }

        public void SetTileMenu(bool enabled)
        {
            this.旋转ToolStripButton.Enabled = enabled;
            this.旋转ToolStripMenuItem1.Enabled = enabled;
            this.旋转ToolStripMenuItem2.Enabled = enabled;

            this.清空ToolStripButton.Enabled = enabled;
            this.清空ToolStripMenuItem1.Enabled = enabled;
            this.清空ToolStripMenuItem2.Enabled = enabled;
        }

        public void SetWallMenu(bool enabled)
        {
            this.画笔ToolStripButton.Enabled = enabled;
            this.画笔ToolStripMenuItem1.Enabled = enabled;
            this.画笔ToolStripMenuItem2.Enabled = enabled;

            this.房间ToolStripButton.Enabled = enabled;
            this.房间ToolStripMenuItem1.Enabled = enabled;
            this.房间ToolStripMenuItem2.Enabled = enabled;
        }

        public void SetEditMenu(bool enabled)
        {
            this.剪切UToolStripButton.Enabled = enabled;
            this.剪切TToolStripMenuItem.Enabled = enabled;
            this.剪切ToolStripMenuItem2.Enabled = enabled;

            this.复制CToolStripButton.Enabled = enabled;
            this.复制CToolStripMenuItem.Enabled = enabled;
            this.复制ToolStripMenuItem2.Enabled = enabled;

            this.删除DToolStripButton.Enabled = enabled;
            this.删除DToolStripMenuItem.Enabled = enabled;
            this.删除ToolStripMenuItem2.Enabled = enabled;
        }

        public void SetDeleteMenu(bool enabled)
        {
            this.删除DToolStripButton.Enabled = enabled;
            this.删除DToolStripMenuItem.Enabled = enabled;
            this.删除ToolStripMenuItem2.Enabled = enabled;
        }

        public bool GetDeleteMenuEnable()
        {
            return this.删除DToolStripButton.Enabled;
        }

        public void SetPasteMenu(bool enabled)
        {
            this.粘贴PToolStripButton.Enabled = enabled;
            this.粘贴PToolStripMenuItem.Enabled = enabled;
            this.粘贴ToolStripMenuItem2.Enabled = enabled;
        }

        private void SetUndoMenu(bool enabled)
        {
            this.撤销ToolStripButton.Enabled = enabled;
            this.撤消ToolStripMenuItem1.Enabled = enabled;
            this.撤销ToolStripMenuItem2.Enabled = enabled;
        }

        private void SetRedoMenu(bool enabled)
        {
            this.重做ToolStripButton.Enabled = enabled;
            this.重复ToolStripMenuItem1.Enabled = enabled;
            this.重做ToolStripMenuItem2.Enabled = enabled;
        }

        public void SetDoMenu(OrderGraph graph)
        {
            if (graph == null)
                return;
            SetUndoMenu(graph.UndoCount > 0);
            SetRedoMenu(graph.RedoCount > 0);
        }

        public void SetDoMenu(bool enabled)
        {
            SetUndoMenu(enabled);
            SetRedoMenu(enabled);
        }

        public void Change()
        {
            this.保存SToolStripButton.Enabled = true;
            this.保存SToolStripMenuItem.Enabled = true;
        }

        public void Saved()
        {
            this.保存SToolStripButton.Enabled = false;
            this.保存SToolStripMenuItem.Enabled = false;
        }
        
        public void SetAlignMenu(int align)
        {
            this.左对齐ToolStripMenuItem1.Enabled = true;
            this.右对齐ToolStripMenuItem1.Enabled = true;
            this.上对齐ToolStripMenuItem1.Enabled = true;
            this.下对齐ToolStripMenuItem1.Enabled = true;

            this.左对齐ToolStripMenuItem2.Enabled = true;
            this.右对齐ToolStripMenuItem2.Enabled = true;
            this.上对齐ToolStripMenuItem2.Enabled = true;
            this.下对齐ToolStripMenuItem2.Enabled = true;

            if ((align & 0x0F) == 0x00)
            {
                this.左对齐ToolStripMenuItem1.Enabled = false;
                this.左对齐ToolStripMenuItem2.Enabled = false;
            }
            else if ((align & 0x0F) == 0x01)
            {
                this.右对齐ToolStripMenuItem1.Enabled = false;
                this.右对齐ToolStripMenuItem2.Enabled = false;
            }

            if ((align & 0xF0) == 0x00)
            {
                this.上对齐ToolStripMenuItem1.Enabled = false;
                this.上对齐ToolStripMenuItem2.Enabled = false;
            }
            else if ((align & 0xF0) == 0x10)
            {
                this.下对齐ToolStripMenuItem1.Enabled = false;
                this.下对齐ToolStripMenuItem2.Enabled = false;
            }
        }

        //public void AlignMenu(OrderGraph graph)
        //{
        //    this.左对齐ToolStripMenuItem1.Checked = false;
        //    this.右对齐ToolStripMenuItem1.Checked = false;
        //    this.上对齐ToolStripMenuItem1.Checked = false;
        //    this.下对齐ToolStripMenuItem1.Checked = false;

        //    this.左对齐ToolStripMenuItem2.Checked = false;
        //    this.右对齐ToolStripMenuItem2.Checked = false;
        //    this.上对齐ToolStripMenuItem2.Checked = false;
        //    this.下对齐ToolStripMenuItem2.Checked = false;

        //    if (graph.ProductSet == null)
        //        return;
        //    int align = graph.ProductSet.Align;

        //    if ((align & 0x0F) == 0x00)
        //    {
        //        this.左对齐ToolStripMenuItem1.Checked = true;
        //        this.左对齐ToolStripMenuItem2.Checked = true;
        //    }
        //    else if ((align & 0x0F) == 0x01)
        //    {
        //        this.右对齐ToolStripMenuItem1.Checked = true;
        //        this.右对齐ToolStripMenuItem2.Checked = true;
        //    }

        //    if ((align & 0xF0) == 0x00)
        //    {
        //        this.上对齐ToolStripMenuItem1.Checked = true;
        //        this.上对齐ToolStripMenuItem2.Checked = true;
        //    }
        //    else if ((align & 0xF0) == 0x10)
        //    {
        //        this.下对齐ToolStripMenuItem1.Checked = true;
        //        this.下对齐ToolStripMenuItem2.Checked = true;
        //    }
        //}

        private void 画笔ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.画笔ToolStripMenuItem2.Checked = this.画笔ToolStripButton.Checked;
            this.画笔ToolStripMenuItem1.Checked = this.画笔ToolStripButton.Checked;
        }

        private void 统计toolStripButton4_EnabledChanged(object sender, EventArgs e)
        {
            this.统计ToolStripMenuItem2.Enabled = this.统计toolStripButton4.Enabled;
            this.统计ToolStripMenuItem1.Enabled = this.统计toolStripButton4.Enabled;
        }

        private void loadCurs()
        {
            string _hmove = ShareData.GetFilePath("hmove.cur");
            string _pen = ShareData.GetFilePath("pen_r.cur");

            if (_pen != null && _pen.Length > 0)
                this.penCur = new System.Windows.Forms.Cursor(_pen);
            else
                this.penCur = Cursors.PanNW;

            if (_hmove != null && _hmove.Length > 0)
                this.hMoveCur = new System.Windows.Forms.Cursor(_hmove);
            else
                this.hMoveCur = Cursors.Hand;
        }

        public void InitOrderGraph(OrderGraph graph)
        {
            graph.Name = "OrderGraph";
            graph.Location = new System.Drawing.Point(0, 0);
            graph.Size = new System.Drawing.Size(1440, 900);
            graph.Dock = DockStyle.Fill;

            graph.SelectWall(-1);
            graph.Space = 25;
            graph.SampleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            graph.Grid = true;
            graph.Visible = false;

            graph.ContextMenuStrip = this.graphContextMenuStrip;
            graph.ToolTip = this.toolTip1;
            this.mainPanel.Controls.Add(graph);
        }

        public void ShowOrderGraph(OrderGraph graph)
        {
            try
            {
                OrderGraph curr = this.CurOrderGraph;
                ToolStripMenuItem _item;

                if (curr != null)
                {
                    _item = curr.Tag as ToolStripMenuItem;
                    if (_item != null)
                        _item.Checked = false;
                    curr.Hide();
                }

                if (graph.Ceiling != null)
                {
                    this.NameTextBox1.Text = graph.Ceiling.Name;
                    this.depthTextBox.Text = graph.Ceiling.Depth.ToString();
                }
                graph.Show();

                _item = graph.Tag as ToolStripMenuItem;
                if (_item != null)
                    _item.Checked = true;
                this.网格ToolStripMenuItem.Checked = graph.Grid;

                RefrushText(graph);

                this.SetPasteMenu(ShareData.list.Count > 0);
                this.SetEditMenu(false);
                this.SetDoMenu(graph);
                this.SetCeilingMenu(graph.Ceiling != null && graph.Ceiling.Length > 0);
                this.SetAlignMenu(graph.ProductSet.Align);
                graph.Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        public void AddGraph(OrderGraph graph)
        {
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text = graph.Ceiling.Name;
            item.Name = graph.Ceiling.Name;
            item.Click += new EventHandler(OrderGraph_Click);
            item.Checked = false;
            item.Tag = graph;
            this.图纸ToolStripMenuItem.DropDownItems.Add(item);
            graph.Tag = item;
        }

        public OrderGraph NewGraph()
        {
            OrderGraph graph = new OrderGraph(this.order);
            this.AddGraph(graph);
            this.InitOrderGraph(graph);
            this.ShowOrderGraph(graph);
            return graph;
        }

        public void OpenOrder()
        {
            order.Editing = false;
            this.customerTextBox.Text = order.OrderRow.IscustomerNull()
                ? "" : order.OrderRow.customer.ToString();
            order.Editing = true;

            List<OrderGraph> graphs = this.order.OrderGraphs;

            for (int i = 0; i < graphs.Count; i++)
            {
                OrderGraph graph = graphs[i];
                this.InitOrderGraph(graph);
                graph.Hide();

                if (graph.Tag != null)
                {
                    ToolStripMenuItem item = graph.Tag as ToolStripMenuItem;
                    item.Text = graph.Ceiling.Name;
                    item.Name = graph.Ceiling.Name;
                    item.Checked = false;
                }
                else
                    this.AddGraph(graph);
            }

            if (graphs.Count > 0)
                ShowOrderGraph(graphs[0]);
        }

        // 图像菜单
        private void OrderGraph_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            OrderGraph graph = item.Tag as OrderGraph;
            if (this.CurOrderGraph == graph)
                return;
            this.ShowOrderGraph(graph);
        }

        // 从本地文件打开订单
        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (ShareData.CeilingDataSet.products.Count < 1)
                {
                    MessageBox.Show("数据加载还没有完成...", ShareData.AppName);
                    return;
                }

                this.order.LoadFromFile();
                this.OpenOrder();
            }
            catch (Exception ex)
            {
                this.systemStatusLabel.Text = ex.Message;
            }
        }

        // 从服务器打开订单
        private void 打开OToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (ShareData.CeilingDataSet.products.Count < 1)
                {
                    MessageBox.Show("数据加载还没有完成...", ShareData.AppName);
                    return;
                }

                SelectOrderForm frm = new SelectOrderForm();
                if (frm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
                this.order.LoadFromDB(frm.OrderId);
                this.OpenOrder();
            }
            catch (Exception ex)
            {
                this.systemStatusLabel.Text = ex.Message;
            }
        }

        // 保存
        private void 保存SToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.order.Save())
                    return;
                this.保存SToolStripButton.Enabled = false;
                this.保存SToolStripMenuItem.Enabled = false;
            }
            catch (Exception ex)
            {
                this.systemStatusLabel.Text = ex.Message;
            }
        }

        // 保存在服务器
        private void 服务器ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.order.SaveToDB())
                    return;
                this.Saved();
            }
            catch (Exception ex)
            {
                this.systemStatusLabel.Text = ex.Message;
            }
        }

        // 保存在本地
        private void 本地文件ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.order.SaveToFile())
                    return;
                this.Saved();
            }
            catch (Exception ex)
            {
                this.systemStatusLabel.Text = ex.Message;
            }
        }

        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = this.order.GetFileName();
                if (filename == null || filename.Length == 0)
                    return;
                this.order.SaveFile(filename);
                this.Saved();
            }
            catch (Exception ex)
            {
                this.systemStatusLabel.Text = ex.Message;
            }
        }

        private void 剪切UToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.CurOrderGraph != null)
            {
                this.CurOrderGraph.ClipProducts();
                //SetPasteMenu(ShareData.list.Count > 0);
            }
        }

        private void 复制CToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.CurOrderGraph != null)
            {
                this.CurOrderGraph.CopyProducts();
                //SetPasteMenu(ShareData.list.Count > 0);
            }
        }

        private void 粘贴PToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.CurOrderGraph != null)
            {
                this.CurOrderGraph.PasteProducts();
                SetPasteMenu(ShareData.list.Count > 0);
            }
        }

        public void WallDrafted()
        {
            this.画笔ToolStripButton.Checked = false;
        }

        // 画笔
        private void penToolStripButton_Click(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null)
                return;

            if (graph.Ceiling != null && graph.Ceiling.Length > 0)
            {
                if (graph.BeginDrawWall(this.penCur, this.orderPositoinLabel))
                    this.画笔ToolStripButton.Checked = true;
            }
            else
            {
                if (graph.BeginDraft(this.penCur, this.orderPositoinLabel))
                    this.画笔ToolStripButton.Checked = true;
            }
        }

        OrderForm statForm = null;

        // 统计
        private void 统计toolStripButton4_Click(object sender, EventArgs e)
        {
            //Statistic();
            this.order.Statistic();

            if (statForm != null && !statForm.IsDisposed)
                statForm.BringToFront();
            else
                statForm = new OrderForm(this, this.order, this.customerTextBox);

            statForm.SetData(this.order.OrderRow);
            statForm.Show();
        }

        public void ReportGraph()
        {
            //int maxHeihgt = gcount * height
            //    + (2 * gcount < order.OrderGraphs.Count ? height : 0);

            //Bitmap bmp = new Bitmap(width * 2, maxHeihgt);
            //graphics.FillRectangle(Brushes.Black, 0, 0, width * 2, maxHeihgt);

            List<OrderGraph> graphs = order.OrderGraphs;
            int gcount = graphs.Count / 2, width = 640, height = 640;
            Rectangle rect = new Rectangle(0, 0, width, height);
            Bitmap bmp = new Bitmap(width * 2, height);
            Graphics graphics = Graphics.FromImage(bmp);

            try
            {
                for (int i = 0; i < gcount; i++)
                {
                    graphics.FillRectangle(Brushes.White, 0, 0, width * 2,
                        height);

                    rect.X = 0; //rect.Y = i * height;
                    graphs[2 * i].DisplayGraph(graphics, rect);
                    rect.X = width; //rect.Y = i * height;
                    graphs[2 * i + 1].DisplayGraph(graphics, rect);

                    bmp.Save(AupuReportForm.ReportPhoto + i + ".png");
                }

                if (2 * gcount < graphs.Count)
                {
                    graphics.FillRectangle(Brushes.White, 0, 0, width * 2,
                        height);

                    rect.X = 0; //rect.Y = a * height;
                    rect.Width *= 2;
                    graphs[2 * gcount].DisplayGraph(graphics, rect);

                    bmp.Save(AupuReportForm.ReportPhoto + gcount + ".png");
                }

                bmp.Save(AupuReportForm.ReportPhoto);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                graphics.Dispose();
            }
        }

        // 先不做报表
        AupuReportForm rptForm = null;

        public void Report()
        {
            this.order.Statistic();
            this.ReportGraph();

            if (rptForm == null || rptForm.IsDisposed)
                rptForm = new AupuReportForm(this.order.OrderRow);
            else
                rptForm.BringToFront();

            rptForm.RefrushData(order.OrderGraphs);
            rptForm.Show();
        }

        // 报表
        private void 报表PToolStripButton_Click(object sender, EventArgs e)
        {
            Report();
        }

        private void 关于AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox frm = new AboutBox();
            frm.ShowDialog();
        }

        private void 新建订单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.order.CloseOrder())
                return;
            this.order.NewOrder();
        }

        private void 添加图纸ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.NewGraph();
        }

        private Bitmap DrawingGraph(OrderGraph graph, string title)
        {
            int width = 900, height = 600,
                left = 600, top = 150, bottom = height - 30;
            RectangleF rect = new Rectangle(40, 100, width - 360,
                height - 160);

            String[] tags = { "客户：", "订单：", "房间：", "尺寸：", "产品：", 
                                "业务员：", "备注：" };
            String[] vlus = { customerTextBox.Text, "", NameTextBox1.Text, 
                                graph.Ceiling.Width + " x " + graph.Ceiling.Height, 
                                "", "", "" };

            Font font = new Font("宋体", 10.0f), titleFont = new Font("宋体", 14.0f);
            Bitmap drawing = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(drawing);

            try
            {
                graphics.FillRectangle(Brushes.White, 0, 0, width, height);
                graphics.DrawImage(Properties.Resources.logo,
                    new Rectangle(width - 155, 0, 150, 30));

                string photo = SettingFile.GetAddress() + " 客服：" 
                    + SettingFile.GetPhoto();
                SizeF size = graphics.MeasureString(photo, font);
                graphics.DrawString(SettingFile.GetCmpName(), font, Brushes.Black,
                    30, bottom);
                graphics.DrawString(photo, font, Brushes.Black, 
                    880 - size.Width, bottom);

                size = graphics.MeasureString(title, titleFont, width - 200);
                graphics.DrawString(title, titleFont, Brushes.Black,
                    (width - size.Width) / 2, 50);

                rect = new Rectangle(40, 100, width - 360, height - 160);
                if (graph.Ceiling.Width > graph.Ceiling.Height)
                    rect.Height = Math.Min(rect.Width * graph.Ceiling.Height
                        / graph.Ceiling.Width, rect.Height);
                graph.DisplayGraph(graphics, rect);

                CeilingDataSet.ordersRow orow = order.OrderRow;
                if (order.Customer == null)
                    vlus[0] = "--";

                if (!orow.IsnumberNull())
                    vlus[1] = orow.number.ToString();
                else
                    vlus[1] = "--";

                if (!orow.IssalesmanNull())
                    vlus[5] = orow.salesman.ToString();
                if (!orow.IsremarkNull())
                    vlus[6] = orow.remark.ToString();

                if (graph.ProductSet != null)
                    graph.ProductSet.Statistic(ShareData.CeilingDataSet.good_view);
                List<ProStatis> _statis = graph.ProductSet.Statis;

                string products = "";
                for (int k = 0; k < _statis.Count; k++)
                {
                    CeilingDataSet.productsRow prow =
                        ShareData.CeilingDataSet.products.FindByID(_statis[k].id);
                    products += prow.name + "\n\n";
                }
                vlus[4] = products;

                size = graphics.MeasureString(vlus[4], font, 220);
                rect = new RectangleF(left + 50, top, 220, size.Height);

                for (int i = 0; i < 7; i++)
                {
                    graphics.DrawString(tags[i], font, Brushes.Black, left, 
                        top);
                    top += i == 4 ? (int)size.Height : 30;
                }

                for (int i = 0; i < 7; i++)
                {
                    graphics.DrawString(vlus[i], font, Brushes.Black, rect);
                    rect.Y += i == 4 ? (int)size.Height : 30;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                drawing = null;
            }
            finally
            {
                graphics.Dispose();
            }

            return drawing;
        }

        private void 导出图片SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null)
                return;

            try
            {
                string title = graph.Ceiling.Name + "-吊顶设计图";
                SaveFileDialog saveFileDialog = null;

                saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = "png";
                saveFileDialog.FileName = "奥普吊顶设计图.png";
                saveFileDialog.Filter = "png 文件(*.png)|*.png|"
                    + "jpeg 文件(*.jpg;*.jpeg)|*.jpg;*.jpeg|"
                    + "windows 位图(*.bmp)|*.bmp|所有文件(*.*)|*.*";
                saveFileDialog.FileName = (this.order.Customer == null ?
                    "客户名未设置" : this.order.Customer) + "-" + title + ".png";

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                Bitmap drawing = DrawingGraph(graph, title);

                if (drawing != null)
                    drawing.Save(saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void 删除图纸ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null)
                return;
            if (MessageBox.Show("删除图纸后将无法恢复，你确定删除该图纸吗？",
                ShareData.AppName, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                return;

            ToolStripMenuItem item = graph.Tag as ToolStripMenuItem;
            this.图纸ToolStripMenuItem.DropDownItems.Remove(item);
            graph.Remove();
            this.mainPanel.Controls.Remove(graph);

            for (int i = this.图纸ToolStripMenuItem.DropDownItems.Count - 1; i >= 0; i--)
            {
                ToolStripItem dropitem = this.图纸ToolStripMenuItem.DropDownItems[i];
                if (dropitem.Tag != null)
                {
                    ShowOrderGraph(dropitem.Tag as OrderGraph);
                    break;
                }
            }
        }

        private void 关闭CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.order.CloseOrder();
                this.order.NewOrder();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ShareData.AppName);
            }
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 撤消UToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OrderGraph graph = this.CurOrderGraph;
                if (graph == null)
                    return;
                graph.Revocate();
                SetDoMenu(graph);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void 重复RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OrderGraph graph = this.CurOrderGraph;
                if (graph == null)
                    return;
                graph.Redo();
                SetDoMenu(graph);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        // 选择房屋类型
        private void 房屋Class_Click(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            AddCeiling frm = new AddCeiling();

            frm.PalaceDataSet = ShareData.CeilingDataSet;
            if (frm.ShowDialog() == DialogResult.Cancel)
                return;

            if (graph == null)
            {
                graph = this.NewGraph();
            }

            if (graph.InitFromSample(frm.Walles, frm.CeilingName,
                System.UInt32.Parse(frm.DepthTextBox.Text)))
            {
                this.NameTextBox1.Text = graph.Ceiling.Name;
                this.depthTextBox.Text = graph.Ceiling.Depth.ToString();
            }
        }

        #region 处理产品列表

        private bool downFlag = false;
        private Point downPoint;
        private Point movePicBeginPos = new Point(0, 0);
        private PictureBox movePicture = new PictureBox();

        private void TreeNodeClear(TreeNodeCollection nodes)
        {
            if (nodes == null)
                return;
            for (int i = 0; i < nodes.Count; i++)
                TreeNodeClear(nodes[i].Nodes);
            nodes.Clear();
        }

        private void productPanel1_Click(object sender, EventArgs e)
        {
            BuckleList.Current = null;
        }

        void pNode_MouseHover(object sender, EventArgs e)
        {
            try
            {
                BuckleNode node = sender as BuckleNode;
                string str = "名称：" + node.Name + "\n规格：" 
                    + node.OriginalRow.width + " * " + node.OriginalRow.height;
                this.toolTip1.SetToolTip(node, str);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("错误信息：" + ex.Message 
                    + "\n" + ex.ToString());
            }
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

            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null)
                return;
            BuckleNode pNode = sender as BuckleNode;
            this.productPanel1.ScrollControlIntoView(pNode);

            if (!pNode.ImgRect.Contains(e.Location))
                return;
            this.downFlag = true;

            if (!this.ProdTransCheckBox.Checked)
                this.movePicture.Size = new Size(
                    (int)Math.Round(pNode.OriginalRow.width / graph.Ceiling.Scale),
                    (int)Math.Round(pNode.OriginalRow.height / graph.Ceiling.Scale));
            else
                this.movePicture.Size = new Size(
                    (int)Math.Round(pNode.OriginalRow.height / graph.Ceiling.Scale),
                    (int)Math.Round(pNode.OriginalRow.width / graph.Ceiling.Scale));

            this.movePicture.Image = pNode.DrawingImage;
            pNode.DrawingImage = null;
            if (this.ProdTransCheckBox.Checked)
                this.movePicture.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pNode.Invalidate();

            this.movePicBeginPos = new Point(e.X - this.movePicture.Size.Width / 2,
                e.Y - this.movePicture.Size.Height / 2);
            this.movePicBeginPos = this.PointToClient(pNode.PointToScreen(this.movePicBeginPos));
            this.downPoint = e.Location;
            this.movePicture.Location = this.movePicBeginPos;

            this.Controls.Add(this.movePicture);
            this.movePicture.BringToFront();
        }

        void pNode_MouseUp(object sender, MouseEventArgs e)
        {
            if (!this.downFlag)
                return;
            this.downFlag = false;
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;
            this.Controls.Remove(this.movePicture);

            Point loca = this.PointToScreen(movePicture.Location);
            BuckleNode pNode = sender as BuckleNode;

            if (this.mainPanel.Controls[0] is OrderGraph)
            {
                OrderGraph graph = this.CurOrderGraph;
                if (graph == null)
                    return;
                Point pt = graph.PointToClient(loca);

                pNode.DrawingImage = (Bitmap)this.movePicture.Image;
                if (this.ProdTransCheckBox.Checked)
                    pNode.DrawingImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                pNode.Invalidate();
                if (graph.DisplayRectangle.Contains(pt))
                    graph.AddProduct(new AupuBuckle(pNode, 
                        this.ProdTransCheckBox.Checked ? 1 : 0), pt);
            }
            else if (this.mainPanel.Controls[0] is TileView)
            {
                TileView view = this.mainPanel.Controls[0] as TileView;
                view.AddBuckle(new AupuBuckle(pNode,
                        this.ProdTransCheckBox.Checked ? 1 : 0), loca);
            }
        }

        void pNode_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.downFlag)
                return;
            BuckleNode pNode = sender as BuckleNode;

            Point location = new Point(e.X - this.downPoint.X + this.movePicBeginPos.X, 
                e.Y - this.downPoint.Y + this.movePicBeginPos.Y);
            this.movePicture.Location = location;
        }

        private BuckleNode AddProductNode(CeilingDataSet.productsRow row)
        {
            BuckleNode pNode = new BuckleNode(row);
            BuckleList.List.Add(pNode);
            pNode.ContextMenuStrip = productContextMenuStrip;

            pNode.MouseDown += new MouseEventHandler(pNode_MouseDown);
            pNode.MouseUp += new MouseEventHandler(pNode_MouseUp);
            pNode.MouseMove += new MouseEventHandler(pNode_MouseMove);
            pNode.MouseHover += new EventHandler(pNode_MouseHover);

            return pNode;
        }

        // 点击产品分组，显示产品列表
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (!(e.Node.Tag is CeilingDataSet.product_classesRow))
                    return;
                this.productPanel1.Controls.Clear();

                int width = 110, rows = this.productPanel1.Height / width;
                CeilingDataSet.product_classesRow row = 
                    e.Node.Tag as CeilingDataSet.product_classesRow;
                if (row.clone_id > 0)
                    row = row.product_classesRowParentByparent_clone;

                DataView prodView = new DataView(ShareData.CeilingDataSet.products);
                prodView.RowFilter = "class_id = " + row.ID.ToString();

                for (int i = 0; i < prodView.Count; i++)
                {
                    BuckleNode pNode = BuckleList.GetProductNode(
                        (prodView[i].Row as CeilingDataSet.productsRow).ID);
                    if (pNode == null)
                        pNode = AddProductNode(prodView[i].Row as CeilingDataSet.productsRow);

                    this.productPanel1.Controls.Add(pNode);
                    pNode.Location = new Point((i / rows) * width + 10, 
                        (i % rows) * width + 10);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        #endregion

        private void 旋转XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mainPanel.Controls[0] is OrderGraph)
            {
                OrderGraph graph = this.CurOrderGraph;
                if (graph == null)
                    return;
                graph.Trans();
            }
            else if (mainPanel.Controls[0] is TileView)
            {
                (mainPanel.Controls[0] as TileView).Trans();
            }
        }

        private void 删除DToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null)
                return;

            if (graph.ProductSet.BuckleSet.SelectProducts.Count > 0)
                graph.DeleteProducts();
            else if (graph.SelectedKeel != null)
                graph.DeleteKeel();
            else if (graph.SZones != null)
                graph.DeleteSZone();
        }

        private void graphContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            this.网格ToolStripMenuItem.Checked = this.CurOrderGraph.Grid;
        }

        private void 水平填充ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null)
                return;
            graph.SetKeel(KeelOrientation.Horizontal);
        }

        private void 竖直填充ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null)
                return;
            graph.SetKeel(KeelOrientation.Vertical);
        }

        private void 自动填充ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null)
                return;
            graph.SetKeel(KeelOrientation.Auto);
        }

        private void 手工添加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null)
                return;
            graph.BeginFillKeel(penCur);
        }

        public void RefrushText(OrderGraph graph)
        {
            if (graph != null)
            {
                string cName = graph.Ceiling.Name;
                this.Text = this.order.Customer
                    + (cName == null || cName.Length == 0 ? "" : " - " + cName);
            }
            else
                this.Text = "无订单";

            this.Text += " | " + ShareData.AppName
                + (this.order.OrderFile.Length > 0 ? " | " + this.order.OrderFile : "");
        }

        private void NameTextBox1_TextChanged(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null)
                return;

            if (graph.Ceiling.Name != this.NameTextBox1.Text)
            {
                graph.SetName(this.NameTextBox1.Text);
                RefrushText(graph);
            }
        }

        private void customerTextBox_TextChanged(object sender, EventArgs e)
        {
            CeilingDataSet.ordersRow row = this.order.OrderRow;
            if (row == null)
                return;
            if (!row.IscustomerNull() 
                && row.customer.CompareTo(this.customerTextBox.Text) == 0)
                return;
            row.customer = this.customerTextBox.Text;

            RefrushText(this.CurOrderGraph);
            if (statForm != null && !statForm.IsDisposed)
                statForm.Text = "订单信息 - " + row.customer + " | " + ShareData.AppName;
            if (rptForm != null && !rptForm.IsDisposed)
                rptForm.Text = "订单报表 - " + row.customer + " | " + ShareData.AppName;
        }

        private void depthTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                OrderGraph graph = this.CurOrderGraph;
                if (graph == null)
                    return;

                System.UInt32 depth = System.UInt32.Parse(this.depthTextBox.Text);
                if (graph.Ceiling.Depth != depth)
                {
                    graph.Ceiling.Depth = depth;
                    graph.ProductSet.KeelSet.CeilingDepth = depth;
                    graph.Invalidate();
                }
            }
            catch
            {
                MessageBox.Show("夹高必须输入有效数字。", ShareData.AppName);
            }
        }

        private void depthTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                OrderGraph graph = this.CurOrderGraph;
                if (graph == null)
                    return;

                uint depth = uint.Parse(this.depthTextBox.Text);
                if (e.KeyChar == '\r' || graph.Ceiling.Depth != depth)
                {
                    graph.Ceiling.Depth = depth;
                    graph.ProductSet.KeelSet.CeilingDepth = depth;
                }
            }
            catch 
            {
                MessageBox.Show("夹高必须输入有效数字。", ShareData.AppName);
            }
        }

        private void 网格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null)
                return;
            graph.Grid = this.网格ToolStripMenuItem.Checked;
        }

        // 修改连接字符串
        private void changeConnectString_Click(object sender, EventArgs e)
        {
            ConnectString frm = new ConnectString();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                order.SetConnection(ShareData.Connection);
            }
        }

        // 尝试重新连接
        private void reConnectStatusMenu_Click(object sender, EventArgs e)
        {
            PalaceForm_LoadDataFromDB(this, new LoadDataEventArgs());
        }

        private void 单机状态ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PalaceForm_LoadDataFromFile(this, new LoadDataEventArgs());
        }

        private void PalaceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.order.CloseOrder())
                e.Cancel = true;
        }

        private void WallEditer_excape(object sender, KeyPressEventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph != null)
                graph.WallEdite_Excape(sender, e);
        }

        private void WallEditer_modify(object sender, KeelChangeEventArgs e)
        {
            try
            {
                OrderGraph graph = this.CurOrderGraph;
                if (graph != null)
                    graph.WallEdite_Modify(sender, e);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null)
                return;
            graph.DisplayCeiling();
            graph.Invalidate();
        }

        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.mainPanel.Controls[0] is OrderGraph)
            {
                OrderGraph graph = this.CurOrderGraph;
                if (graph == null)
                    return;
                graph.ClearProducts();
            }
            else if (this.mainPanel.Controls[0] is TileView)
            {
                TileView view = this.mainPanel.Controls[0] as TileView;
                view.Tile.Clear();
                view.DispTile();
            }
        }

        private void 全选AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null)
                return;
            graph.ProductSet.BuckleSet.AllSelect();
            this.SetEditMenu(true);
            graph.Invalidate(graph.Ceiling.DrawingRect);
        }

        private void mainPanel3_SizeChanged(object sender, EventArgs e)
        {
            if (this.helpCtrl1 != null)
            {
                this.helpCtrl1.Left = (mainPanel.Width - helpCtrl1.Width) / 2;
                this.helpCtrl1.Top = (mainPanel.Height - helpCtrl1.Height) / 2 
                    + 100;
            }
        }

        private void 产品管理TtoolStripButton_Click(object sender, EventArgs e)
        {
            this.order.Editing = false;

            ProductManage frm = new ProductManage();
            frm.ShowDialog();

            if (frm.Changed)
            {
                this.productPanel1.Controls.Clear();
                TreeNodeClear(this.treeView1.Nodes);
                InitProducts();
            }

            this.order.Editing = true;
        }

        private void 左对齐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null || graph.ProductSet == null)
                return;
            graph.ProductSet.Align = (graph.ProductSet.Align & 0xF0) + 0x00;
            graph.Invalidate(graph.Ceiling.DrawingRect);
            //this.AlignMenu(graph);
        }

        private void 右对齐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null || graph.ProductSet == null)
                return;
            graph.ProductSet.Align = (graph.ProductSet.Align & 0xF0) + 0x01;
            graph.Invalidate(graph.Ceiling.DrawingRect);
            //this.AlignMenu(graph);
        }

        private void 上对齐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null || graph.ProductSet == null)
                return;
            graph.ProductSet.Align = (graph.ProductSet.Align & 0x0F) + 0x00;
            graph.Invalidate(graph.Ceiling.DrawingRect);
            //this.AlignMenu(graph);
        }

        private void 下对齐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null || graph.ProductSet == null)
                return;
            graph.ProductSet.Align = (graph.ProductSet.Align & 0x0F) + 0x10;
            graph.Invalidate(graph.Ceiling.DrawingRect);
            //this.AlignMenu(graph);
        }

        private void OpenModule()
        {
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null || graph.ProductSet == null)
                return;
            TileView view = new TileView(graph.ProductSet.Tile);
            this.mainPanel.Controls.Add(view);

            view.Dock = DockStyle.Fill;
            view.BringToFront();
            view.BackColor = Color.Transparent;
            view.BackgroundImage = graph.BackgroundImage;

            SetManageMenu(false);
            SetWallMenu(false);
            SetDoMenu(false);
            SetEditMenu(false);
            SetCeilingMenu(false);
            SetTileMenu(true);
        }

        private void 模块ToolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenModule();
                TileView view = this.mainPanel.Controls[0] as TileView;
                view.DispTile();
            }
            catch (Exception ex)
            {
                string logFile = ShareData.GetErrPath() + "err" + DateTime.Now.Year
                    + DateTime.Now.Month + DateTime.Now.Day + ".log";
                Program.WriteErr(ex, logFile);
                Program.MessageErr(ex, logFile);
            }
        }

        private void 拼接扣板ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control ctrl = productContextMenuStrip.SourceControl;
            if (ctrl == null || !(ctrl is BuckleNode))
                return;

            try
            {
                AupuBuckle buckle = new AupuBuckle(ctrl as BuckleNode,
                    this.ProdTransCheckBox.Checked ? 1 : 0);
                if (mainPanel.Controls[0] is TileView == false)
                    OpenModule();
                (mainPanel.Controls[0] as TileView).AddBuckle(buckle);
            }
            catch (Exception ex)
            {
                string logFile = ShareData.GetErrPath() + "err" + DateTime.Now.Year
                    + DateTime.Now.Month + DateTime.Now.Day + ".log";
                Program.WriteErr(ex, logFile);
                Program.MessageErr(ex, logFile);
            }
        }

        private void 铺设扣板StripMenuItem1_Click(object sender, EventArgs e)
        {
            Control ctrl = this.productContextMenuStrip.SourceControl;
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            OrderGraph graph = this.CurOrderGraph;
            if (graph == null || ctrl == null || !(ctrl is BuckleNode))
                return;

            try
            {
                BuckleList.Current = ctrl as BuckleNode;
                graph.TileProducts(ctrl as BuckleNode, (TileStyle)item.Tag,
                    this.ProdTransCheckBox.Checked ? 1 : 0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }

    public class LoadDataEventArgs : EventArgs
    {
        private bool save = false;

        public bool Save
        {
            get { return save; }
        }

        private LoadDataHandler loadData;

        public LoadDataHandler LoadData
        {
            get { return this.loadData; }
        }

        public LoadDataEventArgs()
        {
            this.loadData = null;
        }

        public LoadDataEventArgs(LoadDataHandler load)
        {
            this.loadData = load;
        }

        public LoadDataEventArgs(bool _save)
        {
            this.save = _save;
        }

        public LoadDataEventArgs(LoadDataHandler load, bool _save)
        {
            this.loadData = load;
            this.save = _save;
        }

        public LoadDataEventArgs(bool _save, LoadDataHandler load)
        {
            this.loadData = load;
            this.save = _save;
        }
    }

    public delegate void LoadDataHandler(object sender, LoadDataEventArgs e);
}
