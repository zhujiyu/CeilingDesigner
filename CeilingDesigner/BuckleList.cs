using System;
using System.Collections.Generic;
//using System.Linq;
using System.Data;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace CeilingDesigner
{
    /// <summary>
    /// 扣板列表
    /// 这里将电器也看作扣板来处理
    /// </summary>
    class BuckleList
    {
        public BuckleList() { }

        private static BuckleNode current = null;

        public static BuckleNode Current
        {
            get { return current; }
            set
            {
                if (current == value)
                    return;
                if (current != null)
                {
                    if (current.IsEditing)
                        current.UnEdit();
                    current.BackColor = Color.Transparent;
                    current.ForeColor = Color.Black;
                }
                current = value;
                if (current != null)
                {
                    current.BackColor = SystemColors.ActiveCaption;
                    current.ForeColor = Color.White;
                }
            }
        }

        public static void WriteXml(CeilingDataSet set)
        {
            string path = ShareData.GetDataPath();
            set.product_classes.WriteXml(path + "aupu.product.classes.xml");
            set.products.WriteXml(path + "aupu.products.xml");
            set.ceiling_sample_walles.WriteXml(path + "aupu.ceiling.sample.walles.xml");
            set.ceiling_samples.WriteXml(path + "aupu.ceiling.samples.xml");
        }

        public static CeilingDataSet DataReading(CeilingDataSet set)
        {
            string[] path = { "", "data/", "../data/" };
            //palaceDataSet palaceDataSet = this.OrderData.PalaceDataSet;

            set.ceiling_samples.Clear();
            set.ceiling_sample_walles.Clear();
            set.product_classes.Clear();
            set.products.Clear();

            for (int i = 0; i < 3; i++)
            {
                string file = path[i] + "aupu.ceiling.samples.xml";
                if (File.Exists(file))
                {
                    set.ceiling_samples.ReadXml(file);
                    break;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                string file = path[i] + "aupu.ceiling.sample.walles.xml";
                if (File.Exists(file))
                {
                    set.ceiling_sample_walles.ReadXml(file);
                    break;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                string file = path[i] + "aupu.product.classes.xml";
                if (File.Exists(file))
                {
                    set.product_classes.ReadXml(file);
                    break;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                string file = path[i] + "aupu.products.xml";
                if (File.Exists(file))
                {
                    set.products.ReadXml(file);
                    break;
                }
            }

            ShareData.form.Order.Editing = false;
            set.AcceptChanges();
            ShareData.form.Order.Editing = true;

            return set;
        }

        public static CeilingDataSet DataLoading(CeilingDataSet set)
        {
            CeilingDataSetTableAdapters.productsTableAdapter products 
                = new CeilingDataSetTableAdapters.productsTableAdapter();
            CeilingDataSetTableAdapters.product_classesTableAdapter classes 
                = new CeilingDataSetTableAdapters.product_classesTableAdapter();
            CeilingDataSetTableAdapters.ceiling_samplesTableAdapter csAdapter 
                = new CeilingDataSetTableAdapters.ceiling_samplesTableAdapter();
            CeilingDataSetTableAdapters.ceiling_sample_wallesTableAdapter cswAdapter 
                = new CeilingDataSetTableAdapters.ceiling_sample_wallesTableAdapter();

            products.Connection = ShareData.Connection;
            classes.Connection = ShareData.Connection;
            csAdapter.Connection = ShareData.Connection;
            cswAdapter.Connection = ShareData.Connection;

            try
            {
                csAdapter.Fill(set.ceiling_samples);
                cswAdapter.Fill(set.ceiling_sample_walles);
                classes.Fill(set.product_classes);
                products.Fill(set.products);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("错误信息：" 
                    + ex.Message + "\n" + ex.ToString());
                return null;
            }

            ShareData.form.Order.Editing = false;
            set.AcceptChanges();
            ShareData.form.Order.Editing = true;

            return set;
        }

        //public static void AddProductNodes(CeilingDataSet.productsDataTable products)
        //{
        //    list.Clear();

        //    for (int i = 0; i < products.Count; i++)
        //    {
        //        if (products[i].product_classesRow == null)
        //            continue;
        //        if (products[i].product_classesRow.type != "surface")
        //            continue;

        //        BuckleNode node = GetProductNode(products[i].ID);
        //        if (node == null)
        //        {
        //            node = new BuckleNode(products[i]);
        //            list.Add(node);
        //        }
        //    }
        //}

        //public static void DispProducts(DataView view, Panel panel)
        //{
        //    int width = 120, rows = panel.Height / width;

        //    for (int i = 0; i < view.Count; i++)
        //    {
        //        BuckleNode pNode = GetProductNode((view[i].Row as CeilingDataSet.productsRow).ID);
        //        pNode.Location = new Point((i / rows) * width + 10, (i % rows) * width + 10);
        //        panel.Controls.Add(pNode);
        //    }
        //}

        //public static void DispProducts(List<BuckleNode> pNodes, Panel panel)
        //{
        //    int width = 120, rows = panel.Height / width;

        //    for (int i = 0; i < pNodes.Count; i++)
        //    {
        //        BuckleNode pNode = pNodes[i];
        //        pNode.Location = new Point((i / rows) * width + 10, 
        //            (i % rows) * width + 10);
        //        panel.Controls.Add(pNode);
        //    }
        //}

        private static List<BuckleNode> list = new List<BuckleNode>();

        public static List<BuckleNode> List
        {
            get { return list; }
        }

        public static BuckleNode GetProductNode(int id)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].OriginalRow == null)
                    continue;
                if (list[i].OriginalRow.RowState == DataRowState.Deleted)
                    continue;
                if (list[i].OriginalRow.ID == id)
                    return list[i];
            }
            return null;
        }

        public static void RemoveProducts()
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                BuckleNode node = list[i];
                if (node.OriginalRow.RowState == DataRowState.Deleted 
                    || node.OriginalRow.RowState == DataRowState.Modified)
                    list.Remove(node);
            }
        }

        private static void _disp(CeilingDataSet.product_classesRow row, 
            TreeNode node)
        {
            CeilingDataSet.product_classesRow _clone = null;
            string name = row.IsnameNull() ? "未命名类" : row.name;

            if (row.clone_id > 0)
            {
                _clone = row;
                row = _clone.product_classesRowParentByparent_clone;
                row = row == null ? _clone : row;

                if (_clone.IsnameNull())
                    name = row.IsnameNull() ? "未命名分组" : row.name;
                else
                    name = _clone.name;
                node.Tag = _clone;
            }
            else
                node.Tag = row;

            node.Text = name;
            node.Name = name;
        }

        private static void _expand(DataView pClass, CeilingDataSet.product_classesRow row, 
            TreeNode node, string category)
        {
            pClass.RowFilter = "type = 'surface' and category = '" + category 
                + "' and parent_class_id = " + row.ID;
            if (pClass.Count > 0)
                DispPClass(pClass, category, node.Nodes);
            if (row.clone_id == 0)
                return;

            row = row.product_classesRowParentByparent_clone;
            pClass.RowFilter = "type = 'surface' and category = '" + category 
                + "' and parent_class_id = " + row.ID;
            if (pClass.Count > 0)
                DispPClass(pClass, category, node.Nodes);
        }

        public static void DispPClass(DataView pClass, string category, 
            TreeNodeCollection nodes)
        {
            TreeNode[] nlist = new TreeNode[pClass.Count];
            CeilingDataSet.product_classesRow[] rows = 
                new CeilingDataSet.product_classesRow[pClass.Count];

            for (int i = 0; i < pClass.Count; i++)
            {
                nlist[i] = new TreeNode();
                nodes.Add(nlist[i]);
                rows[i] = pClass[i].Row as CeilingDataSet.product_classesRow;
                _disp(rows[i], nlist[i]);
            }

            for (int i = 0; i < rows.Length; i++)
            {
                _expand(pClass, rows[i], nlist[i], category);
            }
        }

        public static void DispPClass(DataView pClass, TreeNodeCollection nodes)
        {
            TreeNode[] nlist = new TreeNode[pClass.Count];
            CeilingDataSet.product_classesRow[] rows = 
                new CeilingDataSet.product_classesRow[pClass.Count];

            for (int i = 0; i < pClass.Count; i++)
            {
                rows[i] = pClass[i].Row as CeilingDataSet.product_classesRow;
                nlist[i] = new TreeNode();
                nodes.Add(nlist[i]);
                _disp(rows[i], nlist[i]);
            }

            for (int i = 0; i < rows.Length; i++)
            {
                pClass.RowFilter = "type = 'surface' and parent_class_id = " + rows[i].ID;
                if (pClass.Count > 0)
                    DispPClass(pClass, nlist[i].Nodes);
            }
        }

        public static void DispPClass(CeilingDataSet.product_classesDataTable pcTable, 
            TreeView treeView)
        {
            DataView view = new DataView(pcTable);
            view.RowFilter = "type = 'surface' and parent_class_id = 0";

            TreeNode[] nlist = new TreeNode[view.Count];
            CeilingDataSet.product_classesRow[] rows 
                = new CeilingDataSet.product_classesRow[view.Count];

            for (int i = 0; i < view.Count; i++)
            {
                rows[i] = view[i].Row as CeilingDataSet.product_classesRow;
                nlist[i] = new TreeNode();
                treeView.Nodes.Add(nlist[i]);
                _disp(rows[i], nlist[i]);
            }

            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i].name != "浴顶")
                {
                    _expand(view, rows[i], nlist[i], rows[i].name);
                }
                else
                {
                    view.RowFilter = "type = 'surface' and parent_class_id = 100000000";
                    if (view.Count > 0)
                        DispPClass(view, nlist[i].Nodes);
                }
            }
        }
    }
}

//DataView view = new DataView(pcTable, "type = 'surface' and parent_class_id = 0", "", DataViewRowState.Unchanged);
//view.Table = pcTable;

//ProductManage.AddProductNodes(set.products);

//public static void DispPClass(DataView pClass, DataView pView, System.UInt32 pId, TreeNodeCollection nodes)
//{
//    for (int i = 0; i < pClass.Count; i++)
//    {
//        palaceDataSet.product_classesRow _clone = null, row = pClass[i].Row as palaceDataSet.product_classesRow;
//        if (row.Isparent_class_idNull() || row.parent_class_id != pId)
//            continue;
//        TreeNode node = new TreeNode();
//        string name = row.IsnameNull() ? "未命名类" : row.name;

//        if (row.clone_id > 0)
//        {
//            _clone = row;
//            row = _clone.product_classesRowParentByproduct_classes_clone;
//            row = row == null ? _clone : row;

//            if (_clone.IsnameNull())
//                name = row.IsnameNull() ? "未命名分组" : row.name;
//            else
//                name = _clone.name;

//            //if (!_clone.IswithchildrenNull() && _clone.withchildren)
//            //    DispPClass(pClass, pView, row.ID, node.Nodes);
//            if (_clone.Getproduct_classesRows().Length > 0)
//                DispPClass(pClass, pView, row.ID, node.Nodes);
//            node.Tag = _clone;
//        }
//        else
//        {
//            DispPClass(pClass, pView, row.ID, node.Nodes);
//            node.Tag = row;
//        }

//        node.Text = name;
//        node.Name = name;
//        nodes.Add(node);
//    }
//}

//if (row.clone_id > 0)
//{
//    row = row.product_classesRowParentByproduct_classes_clone;
//    pClass.RowFilter = "type = 'surface' and category = '" + category + "' and parent_class_id = " + row.ID;
//    if (pClass.Count > 0)
//        DispPClass(pClass, node.Nodes);
//}

//public static void DispPClass(DataView pClass, System.UInt32 pId, TreeNodeCollection nodes)
//{
//    for (int i = 0; i < pClass.Count; i++)
//    {
//        palaceDataSet.product_classesRow _clone = null, 
//            row = pClass[i].Row as palaceDataSet.product_classesRow;
//        if (row.Isparent_class_idNull() || row.parent_class_id != pId)
//            continue;
//        TreeNode node = new TreeNode();
//        string name = row.IsnameNull() ? "未命名类" : row.name;

//        if (row.clone_id > 0)
//        {
//            _clone = row;
//            row = _clone.product_classesRowParentByproduct_classes_clone;
//            row = row == null ? _clone : row;

//            if (_clone.IsnameNull())
//                name = row.IsnameNull() ? "未命名分组" : row.name;
//            else
//                name = _clone.name;

//            //if (!_clone.IswithchildrenNull() && _clone.withchildren)
//            //    DispPClass(pClass, pView, row.ID, node.Nodes);
//            if (_clone.Getproduct_classesRows().Length > 0)
//                DispPClass(pClass, row.ID, node.Nodes);
//            node.Tag = _clone;
//        }
//        else
//        {
//            DispPClass(pClass, row.ID, node.Nodes);
//            node.Tag = row;
//        }

//        node.Text = name;
//        node.Name = name;
//        nodes.Add(node);
//    }
//}

//for (int i = 0; i < rows.Length; i++)
//{
//    palaceDataSet.product_classesRow row = rows[i];
//    pClass.RowFilter = "type = 'surface' and category = '" + category + "' and parent_class_id = " + row.ID;
//    if (pClass.Count > 0)
//        DispPClass(pClass, nlist[i].Nodes);
//    if (row.clone_id > 0)
//        row = row.product_classesRowParentByproduct_classes_clone;
//    pClass.RowFilter = "type = 'surface' and category = '" + category + "' and parent_class_id = " + row.ID;
//    if (pClass.Count > 0)
//        DispPClass(pClass, nlist[i].Nodes);
//}

//ToolTip toolTip1 = null;

//public ToolTip ToolTip1
//{
//    get { return toolTip1; }
//    set { toolTip1 = value; }
//}

//ContextMenuStrip menuStrip;

//public ContextMenuStrip MenuStrip
//{
//    get { return menuStrip; }
//    set { menuStrip = value; }
//}

//public void AddProduct(ProductNode pNode, System.Windows.Forms.Control.ControlCollection ctrls, ContextMenuStrip menuStrip)
//{
//    int i = ctrls.Count, width = 120, rows = ctrls.Owner.Height / width;
//    pNode.ContextMenuStrip = menuStrip;
//    pNode.Click += new EventHandler(pNode_Click);
//    pNode.MouseHover += new EventHandler(pNode_MouseHover);
//    pNode.Location = new Point((i / rows) * width + 10, (i % rows) * width + 10);
//    ctrls.Add(pNode);
//}

//void pNode_MouseHover(object sender, EventArgs e)
//{
//    try
//    {
//        ProductNode node = sender as ProductNode;
//        string str = "名称：" + node.Name
//            + "\n规格：" + node.OriginalRow.width + " * " + node.OriginalRow.height;
//        this.toolTip1.SetToolTip(node, str);
//    }
//    catch (Exception ex)
//    {
//        System.Diagnostics.Debug.WriteLine("错误信息：" + ex.Message + "\n" + ex.ToString());
//    }
//}

//void pNode_Click(object sender, EventArgs e)
//{
//    try
//    {
//        Current = sender as ProductNode;
//    }
//    catch (Exception ex)
//    {
//        System.Diagnostics.Debug.Write(ex);
//    }
//}

//public void AddProducts(DataView view, System.Windows.Forms.Control.ControlCollection ctrls, ContextMenuStrip menuStrip)
//{
//    for (int i = 0; i < view.Count; i++)
//    {
//        ProductNode pNode = ProductList.GetProductNode((view[i].Row as palaceDataSet.productsRow).ID);
//        if (pNode == null)
//            pNode = new ProductNode(view[i].Row as palaceDataSet.productsRow);
//        AddProduct(pNode, ctrls, menuStrip);
//    }

//    //for (int i = 0; i < view.Count; i++)
//    //{
//    //    AddProduct(new ProductNode(view[i].Row as palaceDataSet.productsRow), ctrls, menuStrip);
//    //    //ProductNode pNode = new ProductNode(view[i].Row as palaceDataSet.productsRow);
//    //}
//}

//node.MouseDown += new MouseEventHandler(pNode_MouseDown);
//node.MouseUp += new MouseEventHandler(pNode_MouseUp);
//node.MouseMove += new MouseEventHandler(pNode_MouseMove);
//node.MouseHover += new EventHandler(pNode_MouseHover);
//node.ContextMenuStrip = this.productContextMenuStrip;

//public static void DispProducts(TreeNode node, Panel panel)
//{
//    if (node.Tag == null || !(node.Tag is List<ProductNode>))
//        return;

//    List<ProductNode> pNodes = node.Tag as List<ProductNode>;
//    int width = 120, rows = panel.Height / width;

//    for (int i = 0; i < pNodes.Count; i++)
//    {
//        ProductNode pNode = pNodes[i];
//        pNode.Location = new Point((i / rows) * width + 10, (i % rows) * width + 10);
//        panel.Controls.Add(pNode);
//    }
//}
