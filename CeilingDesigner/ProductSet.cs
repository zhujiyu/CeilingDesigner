using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace CeilingDesigner
{
    public class ProductSet
    {
        #region 属性表

        private Ceiling ceiling = null;
        private Order order = null;

        private KeelSet keelset = null;

        internal KeelSet KeelSet
        {
            get { return keelset; }
        }

        private AupuTile tile = new AupuTile();

        public AupuTile Tile
        {
            get { return tile; }
            set { tile = value; }
        }

        private BuckleSet buckleset = null;

        internal BuckleSet BuckleSet
        {
            get { return buckleset; }
        }

        private List<ProStatis> _statis = new List<ProStatis>();

        public List<ProStatis> Statis
        {
            get { return _statis; }
        }

        private Region actualRegion;

        public Region ActuralRegion
        {
            get { return actualRegion; }
        }

        private Region drawingRegion;

        public Region DrawingRegion
        {
            get { return drawingRegion; }
        }

        private int align = 0x00;

        public int Align
        {
            get { return align; }
            set
            {
                if (align == value)
                    return;
                align = value;

                ShareData.form.SetAlignMenu(align);
                buckleset.GenerateAlignBase(tile, ceiling);
                buckleset.ReTileProducts();
            }
        }

        #endregion

        public ProductSet(OrderGraph graph)
        {
            this.ceiling = graph.Ceiling;
            this.order = graph.Order;

            this.buckleset = new BuckleSet(this, ceiling);
            this.keelset = new KeelSet(ceiling);
            this.keelset.CeilingDepth = ceiling.Depth;

            this.GenerateActualRegion();
            this.GenerateDrawingRegion();
        }

        public void Trans(int _angle, PointF center)
        {
            this.Align = ((align & 0xF0) > 0 ? 0x00 : 0x01) + (align & 0x0F) * 0x10;
            this.GenerateActualRegion();
            this.GenerateDrawingRegion();

            this.keelset.Trans(_angle, center);
            _angle = _angle == 90 ? 1 : -1;
            this.tile.Trans(_angle);
            buckleset.Trans(_angle);
            //this.tile.Trans(_angle == 90 ? 1 : -1);
            //buckleset.Trans(_angle == 90 ? 1 : -1);
            //buckleset.GenerateAlignBase(tile, ceiling);
        }

        public void TileProducts(AupuTile _tile)
        {
            this.tile.Release();
            this.tile = _tile;
            buckleset.ReTileProducts();
        }

        public void ProductUnserialize(string str,
            CeilingDataSet.productsDataTable productsDataTable)
        {
            try
            {
                MatchCollection ms = Regex.Matches(str, @"#ali:(\d+)#");

                if (ms.Count > 0)
                {
                    //align = int.Parse(ms[0].Groups[1].Value);
                    this.Align = int.Parse(ms[0].Groups[1].Value);
                    this.buckleset.GenerateAlignBase(this.tile, this.ceiling);
                }

                tile.UnSerialize(str, productsDataTable);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        public void GenerateActualRegion()
        {
            System.Drawing.Drawing2D.GraphicsPath path 
                = new System.Drawing.Drawing2D.GraphicsPath();

            if (this.actualRegion != null)
                this.actualRegion.Dispose();
            for (int i = 0; i < this.ceiling.Walles.Count; i++)
                path.AddLine(this.ceiling.Walles[i].Begin, 
                    this.ceiling.Walles[i].End);
            this.actualRegion = new Region(path);
        }

        public void GenerateDrawingRegion()
        {
            System.Drawing.Drawing2D.GraphicsPath path 
                = new System.Drawing.Drawing2D.GraphicsPath();

            if (this.drawingRegion != null)
                this.drawingRegion.Dispose();
            for (int i = 0; i < this.ceiling.Walles.Count; i++)
                path.AddLine(this.ceiling.Walles[i].BeginPaintPoint, 
                    this.ceiling.Walles[i].EndPaintPoint);
            this.drawingRegion = new Region(path);
        }

        public int KeelIndex(Keel _keel)
        {
            if (_keel == null)
                return -1;
            if (this.keelset.MainKeelList.Contains(_keel))
                return this.keelset.MainKeelList.IndexOf(_keel);
            return -1;
        }

        public void Clear()
        {
            this.keelset.Clear();
            this.buckleset.Clear();
            this.tile.Clear();
        }

        /// <summary>
        /// 该方法只改变边的显示位置，不改变逻辑位置，
        /// 只应用在整个图形的平移操作中
        /// </summary>
        /// <param name="delta">平移的偏移量</param>
        public void Translate(Point delta)
        {
            this.drawingRegion.Translate(delta.X, delta.Y);
            this.keelset.Translate(delta);
            this.buckleset.Translate(delta);
        }

        public bool MoveKeel(Keel keel, PointF delta, int moving)
        {
            if (this.ceiling.InFreeZone(keel, this.keelset.MainKeelList,
                delta))
            {
                keel.Move(delta, moving, ceiling);
                return true;
            }
            return false;
        }

        public System.Int32 FindByProductName(string _name)
        {
            CeilingDataSet cset = ShareData.CeilingDataSet;
            for (int i = 0; i < cset.products.Count; i++)
            {
                if (cset.products[i].IsnameNull())
                    continue;
                if (cset.products[i].name == _name)
                    return (int)cset.products[i].ID;
            }
            return 0;
        }

        private List<CeilingDataSet.good_viewRow> FindProductRowsById(System.Int32 product_id)
        {
            CeilingDataSet.good_viewDataTable goodView
                = ShareData.CeilingDataSet.good_view;
            List<CeilingDataSet.good_viewRow> rows = new List<CeilingDataSet.good_viewRow>();

            for (int i = 0; i < goodView.Count; i++)
            {
                if (goodView[i].Isproduct_idNull())
                    continue;
                if (goodView[i].product_id == product_id)
                    rows.Add(goodView[i]);
            }

            return rows;
        }

        private CeilingDataSet.good_viewRow FindByProductId(System.UInt32 product_id)
        {
            CeilingDataSet.good_viewDataTable goodView 
                = ShareData.CeilingDataSet.good_view;

            for (int i = 0; i < goodView.Count; i++)
            {
                if (goodView[i].Isproduct_idNull())
                    continue;
                if (goodView[i].product_id == product_id)
                    return goodView[i];
            }

            return null;
        }

        private void _AddGood(CeilingDataSet.productsRow prow, 
            System.UInt32 amount, string model)
        {
            CeilingDataSet.good_viewDataTable goodView 
                = ShareData.CeilingDataSet.good_view;
            List<CeilingDataSet.good_viewRow> rows = FindProductRowsById(prow.ID);
            CeilingDataSet.good_viewRow gvrow = null;

            for (int i = 0; i < rows.Count; i++)
            {
                if (!rows[i].IsmodelNull() && rows[i].model == model)
                {
                    gvrow = rows[i];
                    break;
                }
            }

            //CeilingDataSet.good_viewRow gvrow = this.FindByProductId(prow.ID);
            //if (gvrow != null && (gvrow.IsmodelNull() || gvrow.model != model))
            //    gvrow = null;

            if (gvrow == null)
                gvrow = goodView.Newgood_viewRow();
            gvrow.BeginEdit();

            if (gvrow.RowState == System.Data.DataRowState.Detached)
            {
                gvrow.order_id = order.ID;
                gvrow.product_id = (int)prow.ID;
                gvrow.category = prow.product_classesRow.name;

                CeilingDataSet.product_classesRow row = prow.product_classesRow;
                if (row != null)
                {
                    if (!row.IscategoryNull() && row.category != "")
                        gvrow.category = row.category;
                    else if (row.type == "auxiliary")
                        gvrow.category = "辅料";
                    else
                        gvrow.category = row.name;
                }

                gvrow.name = prow.name;
                if (!prow.IspriceNull())
                    gvrow.price = prow.price;
                else
                    gvrow.price = 0;
                if (!prow.IsunitNull())
                    gvrow.unit = prow.unit;
                else
                    gvrow.unit = "";

                gvrow.amount = (int)amount;
                gvrow.model = model;
                gvrow.total = (float)Math.Round(gvrow.amount * prow.price, 2);

                if (!prow.IspatternNull())
                    gvrow.pattern = prow.pattern;
                if (!prow.IscolorNull())
                    gvrow.color = prow.color;
            }
            else
            {
                gvrow.amount += (int)amount;
                gvrow.total = (float)Math.Round(gvrow.amount * prow.price, 2);
            }

            gvrow.EndEdit();
            if (gvrow.RowState == System.Data.DataRowState.Detached)
                goodView.Rows.Add(gvrow);
        }

        public void AddGood(System.Int32 product_id, System.UInt32 amount, 
            string model)
        {
            CeilingDataSet.productsRow prow 
                = ShareData.CeilingDataSet.products.FindByID(product_id);
            if (prow == null)
                return;
            this._AddGood(prow, amount, model);
        }

        public void AddGood(System.Int32 product_id, System.UInt32 amount)
        {
            CeilingDataSet.productsRow prow 
                = ShareData.CeilingDataSet.products.FindByID(product_id);
            if (prow == null)
                return;
            string model = "";

            if (!prow.IspatternNull() && prow.pattern != "")
                model = prow.pattern;
            else if (prow.width > 0 && prow.height > 0)
                model = prow.width + "mm x " + prow.height + "mm";
            else if (prow.length > 0)
                model = prow.length + "mm";
            this._AddGood(prow, amount, model);
        }

        public void ParseData(CeilingDataSet.ceilingsRow ceilingRow, 
            CeilingDataSet.productsDataTable products)
        {
            this.Clear();

            if (!ceilingRow.IsproductsNull())
                this.ProductUnserialize(ceilingRow.products, products);
            if (!ceilingRow.IsappendixNull())
                this.BuckleSet.AddedUnserialize(ceilingRow.appendix, products);
            if (!ceilingRow.IskeelsNull())
                this.KeelSet.Unserialize(ceilingRow.keels);
        }

        public void WriteData(CeilingDataSet.ceilingsRow row)
        {
            float tileunit = tile.Factor / this.ceiling.Scale;
            row.BeginEdit();

            if (row.RowState == System.Data.DataRowState.Detached)
            {
                row.order_id = order.ID;
                row.paint_width = (float)Math.Round(tileunit, 3);
                row.paint_height = (float)Math.Round(tileunit, 3);

                row.rows = (int)buckleset.Rows;
                row.columns = (int)buckleset.Columns;

                row.products = this.tile.Serialize();
                row.products += "ali:" + align + "#";
                row.appendix = buckleset.AddedSerialize();
                row.keels = keelset.Serialize();
            }
            else
            {
                if (row.order_id != order.ID)
                    row.order_id = order.ID;
                float floatTemp = (float)Math.Round(tileunit, 3);
                if (row.paint_width != floatTemp)
                    row.paint_width = floatTemp;
                floatTemp = (float)Math.Round(tileunit, 3);
                if (row.paint_height != floatTemp)
                    row.paint_height = floatTemp;

                if (row.rows != buckleset.Rows)
                    row.rows = (int)buckleset.Rows;
                if (row.columns != buckleset.Columns)
                    row.columns = (int)buckleset.Columns;

                string strTemp = this.tile.Serialize() + "ali:" + align + "#";
                if (row.IsproductsNull() || row.products != strTemp)
                    row.products = strTemp;
                strTemp = buckleset.AddedSerialize();
                if (row.IsappendixNull() || row.appendix != strTemp)
                    row.appendix = strTemp;

                strTemp = keelset.Serialize();
                if (row.IskeelsNull() || row.keels != strTemp)
                    row.keels = strTemp;
            }

            row.EndEdit();
        }

        public bool Statistic(CeilingDataSet.good_viewDataTable gview)
        {
            try
            {
                this._statis.Clear();
                this.BuckleSet.StatisticSurface(gview);
                this.ceiling.Statistic角线(this);
                this.KeelSet.Statistic(this);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return false;
            }
        }
    }

    public class ProStatis
    {
        public System.Int32 id;
        public float count;
    }
}

///// <summary>
///// 旋转图
///// </summary>
///// <param name="_angle">90度或者-90度</param>
///// <param name="ca">图纸中心点，逻辑坐标</param>
///// <param name="ba">
///// 图纸左上点，逻辑坐标，之所以传递这个参数，
///// 是因为这个参数做旋转的过程中发生了变化，
///// 而在计算游离扣板旋转后的位置时，需要图纸未旋转前的坐上点位置
///// </param>
//public void Trans(int _angle, PointF ca, PointF ba)
//{
//    this.GenerateActualRegion();
//    this.GenerateDrawingRegion();

//    this.buckleset.Trans(_angle == 90 ? 1 : -1, ba);
//    this.keelset.Trans(_angle, ca);
//}

//TileProducts(this.tile);
//buckleset.ReTileProducts();

//this.buckleset.WriteData(row);
//this.keelset.WriteData(row);

//private OrderGraph graph = null;
//private System.Drawing.Drawing2D.Matrix matrix 
//    = new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, 0, 0);

//gvrow.id = (int)prow.ID;

//this.rows = this.columns = 0;
//this.append_keelset = new KeelSet(ceiling);
//this.cset = ShareData.CeilingDataSet;
//private CeilingDataSet cset = null;

//public void Interect(AupuBuckle product)
//{
//    PointF rlt = new PointF(ceiling.Left, ceiling.Top);
//    Point  dlt = this.ceiling.DrawingRect.Location;

//    // 逻辑坐标
//    RectangleF rf = new RectangleF(product.Location.X, product.Location.Y, 
//        product.Width, product.Height);

//    // 屏幕物理坐标
//    product.DrawingRect = Rectangle.Round(new RectangleF(
//        (rf.X - ceiling.Left) / ceiling.Scale + dlt.X, 
//        (rf.Y - ceiling.Top) / ceiling.Scale + dlt.Y, 
//        rf.Width / ceiling.Scale, rf.Height / ceiling.Scale));

//    Region rgn = new System.Drawing.Region(rf);
//    rgn.Intersect(this.actualRegion);

//    product.Scans = rgn.GetRegionScans(matrix);
//    product.DisplayScans = new Rectangle[product.Scans.Length];
//    product.Integrity = 0;

//    for (int i = 0; i < product.Scans.Length; i++)
//    {
//        product.DisplayScans[i] = product.Trans(product.Scans[i], dlt,
//            rlt, ceiling.Scale);
//        if (product.Scans[i].Width <= 40 || product.Scans[i].Height <= 40)
//            continue;
//        //if (product.Scans[i].Width < 50 || product.Scans[i].Height < 50)
//        //    continue;
//        product.Integrity += 100 * (product.Scans[i].Width * product.Scans[i].Height) 
//            / (product.Width * product.Height);
//    }
//}

//Point cd = new Point(
//    this.ceiling.DrawingRect.Left + this.ceiling.DrawingRect.Width / 2,
//    this.ceiling.DrawingRect.Top + this.ceiling.DrawingRect.Height / 2);
//PointF ca = new PointF(this.ceiling.Left + this.ceiling.Width / 2, 
//    this.ceiling.Top + this.ceiling.Height / 2);

//public Point ProductSetBase
//{
//    get
//    {
//        if (this.ceiling == null)
//            return new Point(0, 0);
//        else
//            return this.ceiling.DrawingRect.Location;
//    }
//}

//private KeelSet append_keelset = null;

//internal KeelSet Append_KeelSet
//{
//    get { return append_keelset; }
//    set { append_keelset = value; }
//}

//private List<KeelSet> append_keelset = new List<KeelSet>();

//internal List<KeelSet> Append_KeelSet
//{
//    get { return append_keelset; }
//}

//public Keel GetKeel(int index)
//{
//    if (index < 0)
//        return null;

//    if (index < this.keelset.MainKeelList.Count)
//        return this.keelset.MainKeelList[index];
//    //index -= this.keelset.MainKeelList.Count;

//    return null;
//}

//public int SelectKeel(Point pt)
//{
//    return this.keelset.GetMainKeel(pt);
//}

//public void AddProduct(AupuBuckle product, Point location)
//{
//    if (buckleset.SelectProducts.Count > 0)
//    {
//        graph.InvalidateRect(buckleset.SelectRectangle());
//        buckleset.CancelSelect();
//    }

//    ProductRevocationEventArgs arg = new ProductRevocationEventArgs();
//    List<AupuBuckle> prods = buckleset.AddProduct(product, location);

//    if (prods != null)
//    {
//        for (int i = 0; i < prods.Count; i++)
//            arg.add_Products.Add(prods[i]);
//    }

//    arg.productSet = this;
//    arg.dropProducts.Add(product);
//    graph.AddUndo(arg, Revocation.Products);

//    graph.InvalidateRect(product.DrawingRect);
//}

//if (this.append_keelset.MainKeelList.Contains(_keel))
//    return this.append_keelset.MainKeelList.IndexOf(_keel)
//        + this.keelset.MainKeelList.Count;            

//if (index < this.append_keelset.MainKeelList.Count)
//    return this.append_keelset.MainKeelList[index];

//int index = this.keelset.GetMainKeel(pt);
//if (index < 0)
//{
//    index = this.append_keelset.GetMainKeel(pt);
//    index = index < 0 ? index : index + this.keelset.MainKeelList.Count;
//}
//return index;
