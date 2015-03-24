using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace CeilingDesigner
{
    public class BuckleSet
    {
        #region 属性列表

        private static Pen borderPen = new Pen(Color.Black, 1);
        private System.Drawing.Drawing2D.Matrix matrix
            = new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, 0, 0);

        private ProductSet pset = null;
        private Ceiling ceiling = null;

        private PointF alignBase = new PointF(0, 0);

        private uint rows = 0;

        public uint Rows
        {
            get { return rows; }
        }

        private uint cols = 0;

        public uint Columns
        {
            get { return cols; }
        }

        /// <summary>
        /// 当一个尺寸较大的产品内部，嵌入一个小产品时，
        /// 大产品将被小产品切分成几个小部分。
        /// </summary>
        private List<AupuBuckle> aidProducts = new List<AupuBuckle>();

        /// <summary>
        /// 平铺的扣板之外添加的孤立的扣板或者电器
        /// </summary>
        private List<AupuBuckle> addedProducts = new List<AupuBuckle>();

        public List<AupuBuckle> AddedProducts
        {
            get { return addedProducts; }
        }

        private AupuBuckle[] products = null;

        public AupuBuckle[] Products
        {
            get { return products; }
            set { products = value; }
        }

        private List<AupuBuckle> selectProducts = new List<AupuBuckle>();

        public List<AupuBuckle> SelectProducts
        {
            get { return selectProducts; }
        }

        bool allSelected = false;

        public bool AllSelected
        {
            get { return allSelected; }
        }

        #endregion

        public BuckleSet(ProductSet set, Ceiling ceiling)
        {
            this.ceiling = ceiling;
            this.pset = set;
            this.alignBase = new PointF(ceiling.Left, ceiling.Top);
        }

        public void GenerateAlignBase(AupuTile tile, Ceiling _ceiling)
        {
            if (pset.Align == 0x00)      // 左上
                alignBase = new PointF(_ceiling.Left, _ceiling.Top);
            else if (pset.Align == 0x01) // 右上
                alignBase = new PointF(_ceiling.Right - this.cols * tile.Factor, _ceiling.Top);
            else if (pset.Align == 0x10) // 左下
                alignBase = new PointF(_ceiling.Left, _ceiling.Bottom - this.rows * tile.Factor);
            else if (pset.Align == 0x11) // 右下
                alignBase = new PointF(_ceiling.Right - this.cols * tile.Factor,
                    _ceiling.Bottom - this.rows * tile.Factor);
            //alignBase = new PointF(ceiling.Left, ceiling.Top);
        }

        public void AddedUnserialize(string str, 
            CeilingDataSet.productsDataTable _products)
        {
            MatchCollection ms = Regex.Matches(str, @"(\d+)-(\d+)-(\d+)-(\d+)");

            for (int i = 0; i < ms.Count; i++)
            {
                try
                {
                    System.Int32 _pid = System.Int32.Parse(ms[i].Groups[1].Value);
                    CeilingDataSet.productsRow row = _products.FindByID(_pid);
                    if (row == null)
                        continue;
                    //BuckleNode node = AupuTile.GetProductNode(row);

                    Point _loca = new Point(int.Parse(ms[i].Groups[3].Value),
                        int.Parse(ms[i].Groups[4].Value));
                    int _trans = int.Parse(ms[i].Groups[2].Value);
                    this.AddProduct(new AupuBuckle(AupuTile.GetProductNode(row), 
                        _trans), _loca);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex);
                }
            }
        }

        public string AddedSerialize()
        {
            string pdts = "#products:{";

            for (int i = 0; i < this.addedProducts.Count; i++)
            {
                AupuBuckle product = addedProducts[i];
                pdts += "[" + product.OriginalRow.ID + "-" + product.Trans
                    + "-" + product.DrawingRect.X + "-" + product.DrawingRect.Y + "]";
            }

            return pdts + "}#";
        }

        public void Draw(Graphics graphics)
        {
            try
            {
                for (int i = (int)(this.rows * this.cols) - 1; i >= 0; i--)
                {
                    if (this.products[i] == null)
                        continue;
                    this.products[i].Draw(graphics);
                    this.products[i].DrawBorder(graphics, borderPen);
                }

                for (int i = 0; i < this.aidProducts.Count; i++)
                {
                    if (this.aidProducts[i] == null)
                        continue;
                    this.aidProducts[i].Draw(graphics);
                    this.aidProducts[i].DrawBorder(graphics, borderPen);
                }

                for (int i = 0; i < this.addedProducts.Count; i++)
                {
                    if (this.addedProducts[i] == null)
                        continue;
                    this.addedProducts[i].Draw(graphics);
                    this.addedProducts[i].DrawBorder(graphics, borderPen);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void DrawSelectProducts(Graphics graphics)
        {
            for (int i = 0; i < this.selectProducts.Count; i++)
                this.selectProducts[i].DrawSelectedFlag(graphics);
        }

        public void DrawSelectedFlag(Graphics graphics)
        {
            try
            {
                for (int i = 0; i < this.selectProducts.Count; i++)
                {
                    this.selectProducts[i].DrawSelectedFlag(graphics);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void appendAdded(AupuBuckle product)
        {
            if (!addedProducts.Contains(product))
                addedProducts.Add(product);
        }

        private List<AupuBuckle> InsertAdded(AupuBuckle product, 
            PointF logicLoca)
        {
            product.Location = logicLoca;
            product.Index = -1;
            this.appendAdded(product);

            Point dispLoca = ceiling.GetDispCoord(logicLoca);
            product.DrawingRect = new Rectangle(dispLoca.X, dispLoca.Y,
                (int)Math.Round(product.Width / this.ceiling.Scale),
                (int)Math.Round(product.Height / this.ceiling.Scale));

            if (this.ceiling.Contain(logicLoca, 40))
                this.Intersect(product);
            else
                product.Integrity = 100;
            return filterProducts(product);
        }

        private void TransProduct(AupuBuckle buckle, int _angle, PointF pba)
        {
            int _row, _col, _trow, _tcol;
            uint fact = pset.Tile.Factor;
            buckle.Transpose(_angle);

            if (buckle.Index > -1)
            {
                _trow = buckle.Index / (int)this.rows;
                _tcol = buckle.Index % (int)this.rows;

                if (_angle > 0)
                {
                    _row = _tcol;
                    _col = (int)this.cols - _trow - (int)(buckle.Width / fact);
                }
                else
                {
                    _row = (int)this.rows - _tcol - (int)(buckle.Height / fact);
                    _col = _trow;
                }

                if (_row < 0 || _col < 0)
                    return;
                this.SetDrawingRect(buckle, _row, _col);
                this.appendAdded(buckle);
                this.filterProducts(buckle, (uint)_row, (uint)_col);
            }
            else
            {
                float xdelta, ydelta;

                _trow = (int)((buckle.Location.Y - pba.Y) / fact);
                _tcol = (int)((buckle.Location.X - pba.X) / fact);
                xdelta = buckle.Location.X - _tcol * fact - pba.X;
                ydelta = buckle.Location.Y - _trow * fact - pba.Y;

                if (_angle > 0)
                {
                    this.InsertAdded(buckle, new PointF(alignBase.X
                        + (this.cols - _trow) * fact - buckle.Width - ydelta,
                        alignBase.Y + _tcol * fact + xdelta));
                }
                else
                {
                    this.InsertAdded(buckle, new PointF(alignBase.X + _trow * fact + ydelta,
                        alignBase.Y + (this.rows - _tcol) * fact - buckle.Height - xdelta));
                }
            }
        }

        /// <summary>
        /// 顺时针或者逆时针旋转扣板集90度
        /// </summary>
        /// <param name="_angle">1表示顺时针旋转90度，-1表示反向90度</param>
        /// <param name="ba">
        /// 图纸左上点，逻辑坐标，之所以传递这个参数，
        /// 是因为这个参数做旋转的过程中发生了变化，
        /// 而在计算游离扣板旋转后的位置时，需要图纸未旋转前的坐上点位置
        /// </param>
        public void Trans(int _angle)
        {
            PointF pba = this.alignBase;
            List<AupuBuckle> _temp = this.addedProducts;
            this.addedProducts = new List<AupuBuckle>();

            try
            {
                //pset.Tile.Trans(_angle);
                //align = ((align & 0xF0) > 0 ? 0x00 : 0x01) + (align & 0x0F) * 0x10;
                this.GenerateAlignBase(pset.Tile, ceiling);
                this.TileProducts(pset.Tile);

                for (int i = 0; i < _temp.Count; i++)
                    TransProduct(_temp[i], _angle, pba);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        public void TransSelection()
        {
            for (int i = 0; i < this.selectProducts.Count; i++)
            {
                this.selectProducts[i].Transpose(1);
                if (selectProducts[i].Index >= 0)
                    this.products[selectProducts[i].Index] = null;
                this.InsertAdded(selectProducts[i], selectProducts[i].Location);
            }
        }

        public void Translate(Point point)
        {
            for (int i = 0; i < this.addedProducts.Count; i++)
                this.addedProducts[i].Translate(point);
            for (int i = 0; i < this.aidProducts.Count; i++)
                this.aidProducts[i].Translate(point);

            if (this.products == null)
                return;
            for (int i = 0; i < this.products.Length; i++)
            {
                if (this.products[i] != null)
                    this.products[i].Translate(point);
            }
        }

        private void Intersect(AupuBuckle buckle)
        {
            RectangleF rect = new RectangleF(buckle.Location.X, buckle.Location.Y,
                buckle.Width, buckle.Height);
            Region rgn = new Region(rect);

            rgn.Intersect(pset.ActuralRegion);
            buckle.Integrity = 0;
            buckle.Scans = rgn.GetRegionScans(matrix);
            buckle.DisplayScans = new Rectangle[buckle.Scans.Length];

            for (int i = 0; i < buckle.Scans.Length; i++)
            {
                buckle.DisplayScans[i] = ceiling.GetDispCoord(buckle.Scans[i]);
                if (buckle.Scans[i].Width <= 40 || buckle.Scans[i].Height <= 40)
                    continue;
                buckle.Integrity += 100 *
                    (buckle.Scans[i].Width * buckle.Scans[i].Height)
                    / (buckle.Width * buckle.Height);
            }
        }

        private void _Remove(AupuBuckle product)
        {
            if (product.Index > -1)
                this.products[product.Index] = null;
            if (this.addedProducts.Contains(product))
                this.addedProducts.Remove(product);
            if (this.aidProducts.Contains(product))
                this.aidProducts.Remove(product);
        }

        private AupuBuckle _split(AupuBuckle orig,
            Rectangle r1, Rectangle r2, RectangleF r3, RectangleF r4)
        {
            List<RectangleF> scans = new List<RectangleF>();
            List<Rectangle> dispscans = new List<Rectangle>();
            AupuBuckle _np1 = orig.clone();

            _np1.Integrity = 0;
            _np1.Index = orig.Index;

            if (r3.Left - r4.Left > 0)
            {
                RectangleF scan = new RectangleF(r4.Left, r4.Top,
                    r3.Left - r4.Left, r4.Height);
                scans.Add(scan);

                Rectangle dispscan = new Rectangle(r2.Left, r2.Top,
                    r1.Left - r2.Left, r2.Height);
                dispscans.Add(dispscan);

                _np1.Integrity += 100.0f * (scan.Width * scan.Height)
                    / (_np1.Width * _np1.Height);
            }

            if (r3.Top - r4.Top > 0)
            {
                RectangleF scan = new RectangleF(Math.Max(r3.Left, r4.Left),
                    r4.Top,
                    Math.Min(r3.Right, r4.Right) - Math.Max(r3.Left, r4.Left),
                    r3.Top - r4.Top);
                scans.Add(scan);

                Rectangle dispscan = new Rectangle(Math.Max(r1.Left, r2.Left),
                    r2.Top,
                    Math.Min(r1.Right, r2.Right) - Math.Max(r1.Left, r2.Left),
                    r1.Top - r2.Top);
                dispscans.Add(dispscan);

                _np1.Integrity += 100.0f * (scan.Width * scan.Height)
                    / (_np1.Width * _np1.Height);
            }

            if (r4.Bottom - r3.Bottom > 0)
            {
                RectangleF scan = new RectangleF(Math.Max(r3.Left, r4.Left),
                    r3.Bottom,
                    Math.Min(r3.Right, r4.Right) - Math.Max(r3.Left, r4.Left),
                    r4.Bottom - r3.Bottom);
                scans.Add(scan);

                Rectangle dispscan = new Rectangle(Math.Max(r1.Left, r2.Left),
                    r1.Bottom,
                    Math.Min(r1.Right, r2.Right) - Math.Max(r1.Left, r2.Left),
                    r2.Bottom - r1.Bottom);
                dispscans.Add(dispscan);

                _np1.Integrity += 100.0f * (scan.Width * scan.Height)
                    / (_np1.Width * _np1.Height);
            }

            if (r4.Right - r3.Right > 0)
            {
                RectangleF scan = new RectangleF(r3.Right, r4.Top,
                    r4.Right - r3.Right, r4.Height);
                scans.Add(scan);

                Rectangle dispscan = new Rectangle(r1.Right, r2.Top,
                    r2.Right - r1.Right, r2.Height);
                dispscans.Add(dispscan);

                _np1.Integrity += 100.0f * (scan.Width * scan.Height)
                    / (_np1.Width * _np1.Height);
            }

            _np1.Scans = new RectangleF[scans.Count];
            for (int i = 0; i < scans.Count; i++)
                _np1.Scans[i] = scans[i];

            _np1.DisplayScans = new Rectangle[dispscans.Count];
            for (int i = 0; i < dispscans.Count; i++)
                _np1.DisplayScans[i] = dispscans[i];

            return _np1;
        }

        private void _filterCore(AupuBuckle product, AupuBuckle orig, 
            List<AupuBuckle> result)
        {
            if (orig.Scans == null)
                return;
            bool cover = true;
            RectangleF rect = new RectangleF(product.Location.X,
                product.Location.Y, product.Width, product.Height);

            for (int i = 0; i < orig.Scans.Length; i++)
            {
                if (!rect.Contains(orig.Scans[i]))
                {
                    cover = false;
                    break;
                }
            }

            if (!cover)
            {
                RectangleF r1 = new RectangleF(orig.Location.X,
                    orig.Location.Y, orig.Width, orig.Height);
                RectangleF r2 = RectangleF.Intersect(r1, rect);

                if (r2.Width < 40 || r2.Height < 40)
                    return;
                result.Add(orig);
                this._Remove(orig);

                if (orig.Index > -1)
                {
                    AupuBuckle np = _split(orig, product.DrawingRect,
                        orig.DisplayScans[0], rect, orig.Scans[0]);
                    this.products[orig.Index] = np;

                    for (int i = 1; i < orig.Scans.Length; i++)
                    {
                        np = _split(orig, product.DrawingRect,
                            orig.DisplayScans[i], rect, orig.Scans[i]);
                        this.products[orig.Index].Merge(np);
                    }
                }
                else
                {
                    for (int i = 0; i < orig.Scans.Length; i++)
                    {
                        this.aidProducts.Add(_split(orig, product.DrawingRect,
                            orig.DisplayScans[i], rect, orig.Scans[i]));
                    }
                }
            }
            else
            {
                this._Remove(orig);
                result.Add(orig);
            }
        }

        private List<AupuBuckle> _filter(AupuBuckle product, uint _row, uint _col)
        {
            List<AupuBuckle> result = new List<AupuBuckle>();
            AupuTile tile = pset.Tile;

            uint _rs = (uint)Math.Max(0, (int)(_row - tile.MaxUnit));
            uint _re = (uint)Math.Min(this.rows,
                _row + (uint)Math.Round(product.Height / tile.Factor));
            uint _cs = (uint)Math.Max(0, (int)(_col - tile.MaxUnit));
            uint _ce = (uint)Math.Min(this.cols,
                _col + (uint)Math.Round(product.Width / tile.Factor));

            try
            {
                for (int i = aidProducts.Count - 1; i >= 0; i--)
                {
                    AupuBuckle orig = aidProducts[i];
                    if (orig == null || orig == product || orig.Scans == null)
                        continue;
                    _filterCore(product, orig, result);
                }

                for (int i = addedProducts.Count - 1; i >= 0; i--)
                {
                    AupuBuckle orig = addedProducts[i];
                    if (orig == null || orig == product || orig.Scans == null)
                        continue;
                    _filterCore(product, orig, result);
                }

                for (uint i = _rs; i <= _re && i < this.rows; i++)
                {
                    for (uint j = _cs; j <= _ce && j < this.cols; j++)
                    {
                        uint index = i * this.cols + j;
                        AupuBuckle orig = this.products[index];
                        if (orig == null || orig == product || orig.Scans == null
                            || addedProducts.Contains(orig))
                            continue;
                        _filterCore(product, orig, result);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }

            return result;
        }

        /// <summary>
        /// 插入一个新产品，将覆盖住的产品去掉
        /// </summary>
        /// <param name="product">新产品</param>
        /// <param name="_row">插入的行号</param>
        /// <param name="_column">插入处的列号</param>
        /// <returns></returns>
        private List<AupuBuckle> filterProducts(AupuBuckle product, 
            uint _row, uint _col)
        {
            if (_row >= this.rows || _col >= this.cols)
                return null;

            if (products[product.Index] == null
                && !this.addedProducts.Contains(product))
                this.products[product.Index] = product;

            return _filter(product, _row, _col);
        }

        private List<AupuBuckle> filterProducts(AupuBuckle product)
        {
            uint factor = pset.Tile.Factor;
            int _row = (int)((product.Location.Y - alignBase.Y) / factor);
            int _col = (int)((product.Location.X - alignBase.X) / factor);

            if (_row < 0 || _row >= this.rows || _col < 0 
                || _col >= this.cols)
                return null;
            return _filter(product, (uint)_row, (uint)_col);
        }

        private void SetDrawingRect(AupuBuckle product, int _row, int _col)
        {
            try
            {
                // 产品所在位置的逻辑坐标
                uint factor = pset.Tile.Factor;
                product.Location = new PointF(alignBase.X + factor * _col,
                    alignBase.Y + factor * _row);
                product.Index = (int)(_row * this.cols + _col);

                // 屏幕物理坐标
                Point loca = ceiling.GetDispCoord(product.Location);
                product.DrawingRect = new Rectangle(loca.X, loca.Y,
                    (int)Math.Round(product.Width / ceiling.Scale),
                    (int)Math.Round(product.Height / ceiling.Scale));
                this.Intersect(product);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private void _TileCore(TileGrid _grid, int _row, int _col)
        {
            if (_grid.Product.OriginalRow.RowState == System.Data.DataRowState.Detached)
                return;
            if (!_grid.Visible || _grid.RowRepeat == 0 || _grid.ColRepeat == 0)
                return;

            int _rows = _row + (int)(_grid.RowRepeat * _grid.Rows);
            int _cols = _col + (int)(_grid.ColRepeat * _grid.Cols);
            if (_rows + _grid.Rows < 0 || _cols + _grid.Cols < 0)
                return;

            while (_row < _rows && _row < this.rows)
            {
                int _tcol = _col;

                while (_tcol < _cols && _tcol < this.cols)
                {
                    AupuBuckle buckle = _grid.Product.clone();
                    if (_row >= 0 && _tcol >= 0)
                    {
                        SetDrawingRect(buckle, _row, _tcol);
                        if (buckle.Integrity > 10)
                            this.products[buckle.Index] = buckle;
                    }
                    else if (_row + _grid.Rows >= 0 && _tcol + _grid.Cols >= 0)
                    {
                        SetDrawingRect(buckle, _row, _tcol);
                        if (buckle.Integrity > 10)
                            aidProducts.Add(buckle);
                    }
                    _tcol += (int)_grid.Cols;
                }

                _row += (int)_grid.Rows;
            }
        }

        private void _Tile(AupuTile tile, int i, int j)
        {
            if (tile[0].Visible && tile[0].Product != null)
                _TileCore(tile[0], i, j);
            if (tile[1].Visible && tile[1].Product != null)
                _TileCore(tile[1], i, j + (int)(tile[0].ColRepeat * tile[0].Cols));
            if (tile[2].Visible && tile[2].Product != null)
                _TileCore(tile[2], i + (int)(tile[0].RowRepeat * tile[0].Rows), j);
            if (tile[3].Visible && tile[3].Product != null)
                _TileCore(tile[3], i + (int)(tile[1].RowRepeat * tile[1].Rows), j + (int)(tile[2].ColRepeat * tile[2].Cols));
        }

        public void TileProducts(AupuTile tile)
        {
            if (products != null)
            {
                for (int i = 0; i < products.Length; i++)
                {
                    if (this.products[i] != null)
                        this.products[i].Release();
                    this.products[i] = null;
                }
            }

            for (int i = 0; i < aidProducts.Count; i++)
            {
                if (aidProducts[i] != null)
                    aidProducts[i].Release();
            }
            aidProducts.Clear();

            if (tile.Length == 0 || tile.Factor == 0)
                return;
            int _rstep = (int)tile.Rows, _cstep = (int)tile.Cols;
            uint _fact = tile.Factor;
            uint _rows = (uint)Math.Ceiling(ceiling.Height / _fact);
            uint _cols = (uint)Math.Ceiling(ceiling.Width / _fact);

            if (this.rows != _rows || this.cols != _cols)
            {
                this.rows = _rows;
                this.cols = _cols;
                this.GenerateAlignBase(tile, ceiling);

                if (this.products != null)
                    this.products = null;

                this.products = new AupuBuckle[_rows * _cols];
                for (int i = 0; i < products.Length; i++)
                    products[i] = null;
            }

            if ((pset.Align & 0x0F) == 0)
            {
                if ((pset.Align & 0xF0) == 0)
                {
                    for (int i = 0; i < this.rows; i += _rstep)
                    {
                        for (int j = 0; j < this.cols; j += _cstep)
                        {
                            _Tile(tile, i, j);
                        }
                    }
                }
                else
                {
                    for (int i = (int)(this.rows - _rstep); i > -_rstep; 
                        i -= _rstep)
                    {
                        for (int j = 0; j < this.cols; j += _cstep)
                        {
                            _Tile(tile, i, j);
                        }
                    }
                }
            }
            else
            {
                if ((pset.Align & 0xF0) == 0)
                {
                    for (int i = 0; i < this.rows; i += _rstep)
                    {
                        for (int j = (int)(this.cols - _cstep); j > -_cstep;
                            j -= _cstep)
                        {
                            _Tile(tile, i, j);
                        }
                    }
                }
                else
                {
                    for (int i = (int)(this.rows - _rstep); i > -_rstep; 
                        i -= _rstep)
                    {
                        for (int j = (int)(this.cols - _cstep); j > -_cstep; 
                            j -= _cstep)
                        {
                            _Tile(tile, i, j);
                        }
                    }
                }
            }
        }

        public void ReTileProducts()
        {
            List<AupuBuckle> _temp = this.addedProducts;
            this.addedProducts = new List<AupuBuckle>();

            try
            {
                this.TileProducts(pset.Tile);
                this.addedProducts = _temp;
                
                for (int i = 0; i < this.addedProducts.Count; i++)
                {
                    this._insert(this.addedProducts[i],
                        this.addedProducts[i].Location);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private bool IsAccurate(int _row, int _col, PointF pt, double _delta)
        {
            uint factor = pset.Tile.Factor;
            if (Math.Abs(pt.X - _col * factor - ceiling.Left) < _delta
                && Math.Abs(pt.Y - _row * factor - ceiling.Top) < _delta)
                return true;
            return false;
        }

        private bool IsAccurate(AupuBuckle product, int _row, int _col)
        {
            if (_col >= this.cols || _col < 0 || _row >= this.rows
                || _row < 0)
                return false;
            return IsAccurate(_row, _col, product.Location,
                0.2 * Math.Min(product.Width, product.Height));
        }

        private List<AupuBuckle> _insert(AupuBuckle product, PointF pt)
        {
            if (product.OriginalRow.RowState == System.Data.DataRowState.Detached)
                return null;
            uint factor = pset.Tile.Factor;
            int _row = (int)Math.Round((pt.Y - alignBase.Y) / factor);
            int _col = (int)Math.Round((pt.X - alignBase.X) / factor);

            if (_row >= 0 && _col >= 0 && _row < this.rows && _col < this.cols
                && IsAccurate(_row, _col, pt, 0.2 * Math.Min(product.Width,
                product.Height)))
            {
                this.appendAdded(product);
                this.SetDrawingRect(product, _row, _col);
                return filterProducts(product, (uint)_row, (uint)_col);
            }
            else
                return InsertAdded(product, pt);
        }

        public List<AupuBuckle> Revocate(List<AupuBuckle> drops,
            List<AupuBuckle> adds, List<AupuBuckle> moves, PointF delta)
        {
            List<AupuBuckle> result = new List<AupuBuckle>();

            for (int i = 0; i < moves.Count; i++)
                this.MoveProduct(moves[i], delta);
            for (int i = 0; i < drops.Count; i++)
                this.DropProduct(drops[i], drops[i].Location);

            for (int i = 0; i < adds.Count; i++)
            {
                if (adds[i].Index >= 0 && this.cols > 0)
                {
                    List<AupuBuckle> temp = filterProducts(adds[i],
                        (uint)adds[i].Index / this.cols,
                        (uint)adds[i].Index % this.cols);
                    for (int k = 0; k < temp.Count; k++)
                        result.Add(temp[k]);
                }
                else
                {
                    if (adds[i].Integrity >= 99)
                    {
                        if (!this.addedProducts.Contains(adds[i]))
                            addedProducts.Add(adds[i]);
                        if (this.aidProducts.Contains(adds[i]))
                            aidProducts.Remove(adds[i]);
                    }
                    else
                    {
                        if (this.addedProducts.Contains(adds[i]))
                            addedProducts.Remove(adds[i]);
                        if (!this.aidProducts.Contains(adds[i]))
                            aidProducts.Add(adds[i]);
                    }
                }
            }

            return result;
        }

        public AupuBuckle CoverProduct(Point p)
        {
            if (this.ceiling == null || this.ceiling.Length < 1)
                return null;

            for (int i = 0; i < this.addedProducts.Count; i++)
            {
                if (this.addedProducts[i].DrawingRect.Contains(p))
                    return this.addedProducts[i];
            }

            if (!this.ceiling.DrawingRect.Contains(p))
                return null;

            for (int i = 0; i < this.aidProducts.Count; i++)
            {
                if (this.aidProducts[i].DrawingRect.Contains(p))
                    return this.aidProducts[i];
            }

            for (uint i = 0; i < this.rows; i++)
            {
                for (uint j = 0; j < this.cols; j++)
                {
                    uint idx = i * cols + j;
                    if (this.products[idx] == null)
                        continue;
                    if (this.products[idx].DrawingRect.Contains(p))
                        return this.products[idx];
                }
            }

            return null;
        }

        public List<AupuBuckle> AddProduct(AupuBuckle product, Point dispLoca)
        {
            return this._insert(product, 
                this.ceiling.GetLogicCoord(dispLoca));
        }

        public List<AupuBuckle> AddProduct(AupuBuckle product, PointF logicLoca)
        {
            return this._insert(product, logicLoca);
        }

        public List<AupuBuckle> MoveProduct(AupuBuckle product, PointF delta)
        {
            // 从扣板格子里取下，放到附加扣板队列里
            if (product.Index > -1)
            {
                if (products[product.Index] == product)
                    products[product.Index] = null;
                if (!addedProducts.Contains(product))
                    addedProducts.Add(product);
            }
            
            product.Translate(Point.Round(new PointF(delta.X / ceiling.Scale, 
                delta.Y / ceiling.Scale)));
            product.Index = -1;

            SizeF size = new SizeF(product.Width / ceiling.Scale,
                product.Height / ceiling.Scale);
            product.DrawingRect = new Rectangle(product.DrawingRect.Location,
                Size.Round(size));

            return this._insert(product, new PointF(product.Location.X + delta.X,
                product.Location.Y + delta.Y));
        }

        public List<AupuBuckle> MoveSelectedProducts(Point delta)
        {
            List<AupuBuckle> result = new List<AupuBuckle>();

            for (int i = 0; i < this.selectProducts.Count; i++)
            {
                List<AupuBuckle> temp = this.MoveProduct(this.selectProducts[i], 
                    delta);
                if (temp == null)
                    continue;
                for (int j = 0; j < temp.Count; j++)
                    result.Add(temp[j]);
            }

            return result;
        }

        public List<AupuBuckle> MoveSelectedProducts(PointF delta)
        {
            List<AupuBuckle> temp, result = new List<AupuBuckle>();

            for (int i = 0; i < this.selectProducts.Count; i++)
            {
                temp = this.MoveProduct(this.selectProducts[i], delta);
                if (temp == null)
                    continue;
                for (int j = 0; j < temp.Count; j++)
                    result.Add(temp[j]);
            }

            return result;
        }

        public void DropProduct(AupuBuckle product, PointF loca)
        {
            uint factor = pset.Tile.Factor;
            int row = (int)Math.Round((loca.Y - alignBase.Y) / factor);
            int col = (int)Math.Round((loca.X - alignBase.X) / factor);

            if (col < this.cols && col > -1 && row < this.rows && row > -1)
            {
                if (this.products[row * this.cols + col] == product)
                    this.products[row * this.cols + col] = null;
            }

            if (this.addedProducts.Contains(product))
                this.addedProducts.Remove(product);
            if (this.aidProducts.Contains(product))
                this.aidProducts.Remove(product);
        }

        public void DropSelectedProducts()
        {
            if (selectProducts.Count < 1)
                return;

            for (int i = 0; i < this.selectProducts.Count; i++)
            {
                AupuBuckle product = selectProducts[i];
                if (product.Index > -1 && products[product.Index] == product)
                    products[product.Index] = null;
                if (addedProducts.Contains(product))
                    addedProducts.Remove(product);
                if (aidProducts.Contains(product))
                    aidProducts.Remove(product);
            }

            this.CancelSelect();
        }

        public Rectangle SelectRectangle()
        {
            if (this.selectProducts.Count < 1)
                return new Rectangle(0, 0, 0, 0);
            Rectangle rect = this.selectProducts[0].DrawingRect;

            for (int i = 1; i < this.selectProducts.Count; i++)
                rect = Rectangle.Union(rect, this.selectProducts[i].DrawingRect);
            return rect;
        }

        public AupuBuckle Select(Point p)
        {
            AupuBuckle product = this.CoverProduct(p);

            if (product != null && !this.selectProducts.Contains(product))
            {
                this.selectProducts.Add(product);
            }

            return product;
        }

        public void AllSelect()
        {
            for (int i = 0; i < this.addedProducts.Count; i++)
            {
                this.selectProducts.Add(addedProducts[i]);
            }

            if (this.products == null)
                return;
            this.allSelected = true;

            for (int j = 0; j < this.products.Length; j++)
            {
                AupuBuckle product = this.products[j];
                if (product != null)
                {
                    this.selectProducts.Add(product);
                }
            }
        }

        public void CancelSelect()
        {
            this.allSelected = false;
            this.selectProducts.Clear();
        }

        public void Clear()
        {
            for (int i = 0; i < this.addedProducts.Count; i++)
                this.addedProducts[i].Release();
            this.addedProducts.Clear();

            for (int i = 0; i < this.aidProducts.Count; i++)
                this.aidProducts[i].Release();
            this.aidProducts.Clear();

            for (int i = (int)(this.rows * this.cols) - 1; i >= 0; i--)
            {
                if (this.products[i] != null)
                    this.products[i].Release();
                this.products[i] = null;
            }

            this.rows = this.cols = 0;
            this.selectProducts.Clear();
            //this.tile.Clear();
        }

        private void CountProduct(List<ProStatis> _statis, int id, 
            float integrity)
        {
            int flag = _statis.Count;

            for (int k = 0; k < _statis.Count; k++)
            {
                if (_statis[k].id == id)
                {
                    if (integrity > 50)
                        _statis[k].count += 1;
                    else
                        _statis[k].count += 0.5f;
                    flag = k;
                    break;
                }
            }

            if (flag >= _statis.Count)
            {
                ProStatis temp = new ProStatis();
                temp.id = id;
                //temp.product = pdt;
                if (integrity > 50)
                    temp.count += 1;
                else
                    temp.count += 0.5f;
                _statis.Add(temp);
            }
        }

        private void CountProduct(List<ProStatis> _statis, AupuBuckle pdt)
        {
            CountProduct(_statis, pdt.OriginalRow.ID, pdt.Integrity);
        }

        public void StatisticSurface(CeilingDataSet.good_viewDataTable goodView)
        {
            List<ProStatis> _statis = pset.Statis;

            for (int i = 0; i < this.Products.Length; i++)
            {
                AupuBuckle pdt = this.Products[i];
                if (pdt == null || pdt.Integrity < 10)
                    continue;
                this.CountProduct(_statis, pdt);
            }

            for (int i = 0; i < this.aidProducts.Count; i++)
            {
                bool _counted = false;
                int id = aidProducts[i].OriginalRow.ID;

                for (int k = i - 1; k >= 0; k--)
                {
                    if (aidProducts[k].Index == aidProducts[i].Index
                        && id == aidProducts[k].OriginalRow.ID)
                    {
                        _counted = true;
                        break;
                    }
                }

                if (_counted)
                    continue;
                float integrity = aidProducts[i].Integrity;

                for (int k = i + 1; k < aidProducts.Count; k++)
                {
                    if (aidProducts[k].Index == aidProducts[i].Index 
                        && id == aidProducts[k].OriginalRow.ID)
                        integrity += aidProducts[k].Integrity;
                }

                this.CountProduct(_statis, id, integrity);
            }

            for (int i = 0; i < this.addedProducts.Count; i++)
            {
                AupuBuckle pdt = this.addedProducts[i];
                if (pdt == null || pdt.Integrity < 10)
                    continue;
                if (pdt.Index > -1 && Products[pdt.Index] == pdt)
                    continue;
                this.CountProduct(_statis, pdt);
            }

            for (int k = 0; k < _statis.Count; k++)
            {
                pset.AddGood(_statis[k].id, 
                    (System.UInt32)Math.Ceiling(_statis[k].count));
            }
        }
    }
}

//// 产品所在位置的逻辑坐标
//int _c = _tcol < 0 ? 0 : _tcol, _r = _row < 0 ? 0 : _row;
//uint factor = pset.Tile.Factor;
//buckle.Location = new PointF(alignBase.X + factor * _c,
//    alignBase.Y + factor * _r);
//buckle.Index = (int)(_r * this.cols + _c);

//this.SetDrawingRect(buckle, _row < 0 ? 0 : _row,
//    _tcol < 0 ? 0 : _tcol);
//aidProducts.Add(buckle);

//Point loca = ceiling.GetDispCoord(buckle.Location);
//buckle.DrawingRect = new Rectangle(loca.X, loca.Y,
//    (int)Math.Round(buckle.Width / ceiling.Scale),
//    (int)Math.Round(buckle.Height / ceiling.Scale));
//if (buckle.Integrity > 10)
//    this.products[buckle.Index] = buckle;

//int w1 = (int)(tile[0].Visible ? (tile[0].ColRepeat * tile[0].Cols) : (tile[2].ColRepeat * tile[2].Cols)),
//    h1 = (int)(tile[0].Visible ? (tile[0].RowRepeat * tile[0].Rows) : (tile[1].RowRepeat * tile[1].Rows));
//int w0 = (int)(tile[0].ColRepeat * tile[0].Cols),
//    h0 = (int)(tile[0].RowRepeat * tile[0].Rows);


//for (uint i = 0; i < this.rows; i += tile.Rows)
//{
//    for (uint j = 0; j < this.cols; j += tile.Cols)
//    {
//        if (tile[0].Visible)
//            _Tile(tile[0], i, j);
//        if (tile[1].Visible)
//            _Tile(tile[1], i, j + tile[0].ColRepeat * tile[0].Cols);
//        if (tile[2].Visible)
//            _Tile(tile[2], i + tile[0].RowRepeat * tile[0].Rows, j);
//        if (tile[3].Visible)
//            _Tile(tile[3], i + tile[1].RowRepeat * tile[1].Rows,
//                j + tile[2].ColRepeat * tile[2].Cols);
//    }
//}

//private void _TileProducts(AupuTile tile)
//{
//    if (products != null)
//    {
//        for (int i = 0; i < products.Length; i++)
//        {
//            if (this.products[i] != null)
//                this.products[i].Release();
//            this.products[i] = null;
//        }
//    }

//    for (int i = 0; i < aidProducts.Count; i++)
//    {
//        if (aidProducts[i] != null)
//            aidProducts[i].Release();
//    }
//    aidProducts.Clear();

//    if (tile.Length == 0 || tile.Factor == 0)
//        return;
//    this.tileunit = tile.Factor / this.ceiling.Scale;

//    uint _rows = (uint)Math.Ceiling(ceiling.Height / tile.Factor);
//    uint _cols = (uint)Math.Ceiling(ceiling.Width / tile.Factor);

//    if (this.rows != _rows || this.cols != _cols)
//    {
//        this.rows = _rows;
//        this.cols = _cols;
//        this.GenerateAlignBase();

//        if (this.products != null)
//            this.products = null;

//        this.products = new AupuBuckle[_rows * _cols];
//        for (int i = 0; i < products.Length; i++)
//            products[i] = null;
//    }

//    for (uint i = 0; i < this.rows; i += tile.Rows)
//    {
//        for (uint j = 0; j < this.cols; j += tile.Cols)
//        {
//            if (tile.Tiles[0].Visible)
//                _Tile(tile.Tiles[0], i, j);
//            if (tile.Tiles[1].Visible)
//                _Tile(tile.Tiles[1], i,
//                    j + tile.Tiles[0].ColRepeat * tile.Tiles[0].Cols);
//            if (tile.Tiles[2].Visible)
//                _Tile(tile.Tiles[2],
//                    i + tile.Tiles[0].RowRepeat * tile.Tiles[0].Rows, j);
//            if (tile.Tiles[3].Visible)
//                _Tile(tile.Tiles[3],
//                    i + tile.Tiles[1].RowRepeat * tile.Tiles[1].Rows,
//                    j + tile.Tiles[2].ColRepeat * tile.Tiles[2].Cols);
//        }
//    }
//}

//private void _TileAddedProducts()
//{
//    for (int i = 0; i < this.addedProducts.Count; i++)
//    {
//        this._insert(this.addedProducts[i], 
//            this.addedProducts[i].Location);
//    }
//}

//public void TileProducts(BuckleNode product, TileStyle style, 
//    int trans)
//{
//    if (product == null && product.OriginalRow.width < 1 
//        || product.OriginalRow.height < 1)
//        return;
//    this.tile.SetTile(new AupuBuckle(product, trans), style);
//    this.TileProducts();
//}

//public void WriteData(CeilingDataSet.ceilingsRow row)
//{
//    row.BeginEdit();

//    if (row.RowState == System.Data.DataRowState.Detached)
//    {
//        row.paint_width = (float)Math.Round(this.tileunit, 3);
//        row.paint_height = (float)Math.Round(this.tileunit, 3);

//        row.rows = (int)this.Rows;
//        row.clomns = (int)this.Columns;

//        row.products = this.tile.Serialize();
//        row.products += "ali:" + align + "#";
//        row.appendix = this.AddedSerialize();
//    }
//    else
//    {
//        float floatTemp = (float)Math.Round(this.tileunit, 3);
//        if (row.paint_width != floatTemp)
//            row.paint_width = floatTemp;
//        floatTemp = (float)Math.Round(this.tileunit, 3);
//        if (row.paint_height != floatTemp)
//            row.paint_height = floatTemp;

//        if (row.rows != this.Rows)
//            row.rows = (int)this.Rows;
//        if (row.clomns != this.Columns)
//            row.clomns = (int)this.Columns;

//        string strTemp = this.tile.Serialize() + "ali:" + align + "#";
//        if (row.IsproductsNull() || row.products != strTemp)
//            row.products = strTemp;
//        strTemp = this.AddedSerialize();
//        if (row.IsappendixNull() || row.appendix != strTemp)
//            row.appendix = strTemp;
//    }

//    row.EndEdit();
//}

//if (cover)
//{
//    this._Remove(orig);
//    result.Add(orig);
//}
//else
//{
//    RectangleF r1 = new RectangleF(orig.Location.X,
//        orig.Location.Y, orig.Width, orig.Height);
//    RectangleF r2 = RectangleF.Intersect(r1, rect);

//    if (r2.Width < 40 || r2.Height < 40)
//        return;
//    result.Add(orig);
//    this._Remove(orig);

//    if (orig.Index > -1)
//    {
//        AupuBuckle np = _splitCore(orig, product.DrawingRect,
//            orig.DisplayScans[0], rect, orig.Scans[0]);
//        this.products[orig.Index] = np;

//        for (int i = 1; i < orig.Scans.Length; i++)
//        {
//            np = _splitCore(orig, product.DrawingRect,
//                orig.DisplayScans[i], rect, orig.Scans[i]);
//            this.products[orig.Index].Merge(np);
//        }
//    }
//    else
//    {
//        for (int i = 0; i < orig.Scans.Length; i++)
//        {
//            this.aidProducts.Add(_splitCore(orig, product.DrawingRect,
//                orig.DisplayScans[i], rect, orig.Scans[i]));
//        }
//    }
//}

//AupuBuckle np = _splitCore(orig, product.DrawingRect,
//    orig.DisplayScans[0], rect, orig.Scans[0]);
//if (orig.Index > -1)
//    this.products[orig.Index] = np;
//else
//    this.aidProducts.Add(np);

//for (int i = 1; i < orig.Scans.Length; i++)
//{
//    np = _splitCore(orig, product.DrawingRect,
//        orig.DisplayScans[i], rect, orig.Scans[i]);
//    if (orig.Index > -1)
//        this.products[orig.Index].Merge(np);
//    else
//        this.aidProducts.Add(np);
//}

//for (int i = 0; i < orig.Scans.Length; i++)
//{
//    AupuBuckle np = _splitCore(orig, product.DrawingRect,
//        orig.DisplayScans[i], rect, orig.Scans[i]);
//    if (orig.Index > -1 && this.products[orig.Index] == null)
//        this.products[orig.Index] = np;
//    else
//        this.aidProducts.Add(np);
//}

//if (r2.Width > 40 && r2.Height > 40)
//{
//    this._Remove(orig);
//    result.Add(orig);

//    for (int i = 0; i < orig.Scans.Length; i++)
//    {
//        AupuBuckle np = _splitCore(orig, product.DrawingRect,
//            orig.DisplayScans[i], rect, orig.Scans[i]);
//        if (orig.Index > -1 && this.products[orig.Index] == null)
//            this.products[orig.Index] = np;
//        else
//            this.aidProducts.Add(np);
//    }
//}

//if (r3.Left - r4.Left > 0)
//{
//    AupuBuckle _np = orig.clone();
//    //_np.DrawingRect = temp.DisplayScans[0];

//    _np.Scans = new RectangleF[1];
//    _np.Scans[0] = new RectangleF(r4.Left, r4.Top,
//        r3.Left - r4.Left, r4.Height);

//    _np.DisplayScans = new Rectangle[1];
//    _np.DisplayScans[0] = new Rectangle(r2.Left, r2.Top,
//        r1.Left - r2.Left, r2.Height);

//    _np.Integrity = 100.0f * (_np.Scans[0].Width * _np.Scans[0].Height)
//        / (_np.Width * _np.Height);

//    _np.Index = orig.Index;
//    aidProducts.Add(_np);
//}

//if (r3.Top - r4.Top > 0)
//{
//    AupuBuckle _np = orig.clone();
//    //_np.DrawingRect = temp.DisplayScans[0];

//    _np.Scans = new RectangleF[1];
//    _np.Scans[0] = new RectangleF(Math.Max(r3.Left, r4.Left), 
//        r4.Top,
//        Math.Min(r3.Right, r4.Right) - Math.Max(r3.Left, r4.Left), 
//        r3.Top - r4.Top);

//    _np.DisplayScans = new Rectangle[1];
//    _np.DisplayScans[0] = new Rectangle(Math.Max(r1.Left, r2.Left), 
//        r2.Top,
//        Math.Min(r1.Right, r2.Right) - Math.Max(r1.Left, r2.Left), 
//        r1.Top - r2.Top);

//    _np.Integrity = 100.0f * (_np.Scans[0].Width * _np.Scans[0].Height)
//        / (_np.Width * _np.Height);

//    _np.Index = orig.Index;
//    aidProducts.Add(_np);
//}

//if (r4.Bottom - r3.Bottom > 0)
//{
//    AupuBuckle _np = orig.clone();
//    //_np.DrawingRect = temp.DisplayScans[0];

//    _np.Scans = new RectangleF[1];
//    _np.Scans[0] = new RectangleF(Math.Max(r3.Left, r4.Left), 
//        r3.Bottom,
//        Math.Min(r3.Right, r4.Right) - Math.Max(r3.Left, r4.Left), 
//        r4.Bottom - r3.Bottom);

//    _np.DisplayScans = new Rectangle[1];
//    _np.DisplayScans[0] = new Rectangle(Math.Max(r1.Left, r2.Left), 
//        r1.Bottom,
//        Math.Min(r1.Right, r2.Right) - Math.Max(r1.Left, r2.Left), 
//        r2.Bottom - r1.Bottom);

//    _np.Integrity = 100.0f * (_np.Scans[0].Width * _np.Scans[0].Height)
//        / (_np.Width * _np.Height);

//    _np.Index = orig.Index;
//    aidProducts.Add(_np);
//}

//if (r4.Right - r3.Right > 0)
//{
//    AupuBuckle _np = orig.clone();
//    //_np.DrawingRect = temp.DisplayScans[0];

//    _np.Scans = new RectangleF[1];
//    _np.Scans[0] = new RectangleF(r3.Right, r4.Top,
//        r4.Right - r3.Right, r4.Height);

//    _np.DisplayScans = new Rectangle[1];
//    _np.DisplayScans[0] = new Rectangle(r1.Right, r2.Top,
//        r2.Right - r1.Right, r2.Height);

//    _np.Integrity = 100.0f * (_np.Scans[0].Width * _np.Scans[0].Height)
//        / (_np.Width * _np.Height);

//    _np.Index = orig.Index;
//    aidProducts.Add(_np);
//}

//private void _split(AupuBuckle product, AupuBuckle orig)
//{
//    RectangleF rect = new RectangleF(product.Location.X, 
//        product.Location.Y, product.Width, product.Height);

//    for (int i = 0; i < orig.Scans.Length; i++)
//    {
//        _splitCore(orig, product.DrawingRect, orig.DisplayScans[i], 
//            rect, orig.Scans[i]);
//    }

//    if (orig.Index > -1)
//        products[orig.Index] = null;
//}

//if (orig.Scans != null)
//{
//    for (int i = 0; i < orig.Scans.Length; i++)
//    {
//        if (!rect.Contains(orig.Scans[i]))
//        {
//            cover = false;
//            break;
//            //return;
//        }
//    }
//}

//PointF pt;
//if (_angle > 0)
//{
//    pt = new PointF(alignBase.X + (this.cols - _trow)
//        * this.tile.Factor - product.Width - ydelta,
//        alignBase.Y + _tcol * this.tile.Factor + xdelta);
//}
//else
//{
//    pt = new PointF(alignBase.X + _trow * this.tile.Factor + ydelta,
//        alignBase.Y + (this.rows - _tcol) * this.tile.Factor
//        - product.Height - xdelta);
//}
//this.InsertAdded(product, pt);

//_np.Integrity = 100.0f * (_np.Scans[0].Width * _np.Scans[0].Height)
//    / (product.Width * product.Height);

//for (int i = 0; i < _temp.Count; i++)
//{
//    _temp[i].Transpose(_angle);
//    TransProduct(_temp[i], _angle, pba);

//    if (_temp[i].Index > -1)
//    {
//        int _trow = _temp[i].Index / (int)this.rows,
//            _tcol = _temp[i].Index % (int)this.rows;

//        if (_angle > 0)
//        {
//            _row = _tcol;
//            _col = (int)this.cols - _trow
//                - (int)(_temp[i].Width / this.tile.Factor);
//        }
//        else
//        {
//            _row = (int)this.rows - _tcol
//                - (int)(_temp[i].Height / this.tile.Factor);
//            _col = _trow;
//        }

//        if (_row < 0 || _col < 0)
//            continue;
//        this.SetDrawingRect(_temp[i], (uint)_row, (uint)_col);
//        this.appendAdded(_temp[i]);
//        this.filterProducts(_temp[i], (uint)_row, (uint)_col);
//    }
//    else
//    {
//        int _trow = (int)((_temp[i].Location.Y - ba.Y)
//            / this.tile.Factor);
//        int _tcol = (int)((_temp[i].Location.X - ba.X)
//            / this.tile.Factor);
//        float xdelta = _temp[i].Location.X - _tcol * this.tile.Factor - ba.X;
//        float ydelta = _temp[i].Location.Y - _trow * this.tile.Factor - ba.Y;
//        PointF pt;

//        if (_angle > 0)
//        {
//            pt = new PointF(ceiling.Left + (this.cols - _trow)
//                * this.tile.Factor - _temp[i].Width - ydelta,
//                ceiling.Top + _tcol * this.tile.Factor + xdelta);
//        }
//        else
//        {
//            pt = new PointF(ceiling.Left + _trow * this.tile.Factor + ydelta,
//                ceiling.Top + (this.rows - _tcol) * this.tile.Factor
//                - _temp[i].Height - xdelta);
//        }

//        this.InsertAdded(_temp[i], pt);
//    }
//}

//product.Location = new PointF(
//    ceiling.Left + this.tile.Factor * _col,
//    ceiling.Top + this.tile.Factor * _row);

//int col = (int)Math.Round((location.X - this.ceiling.DrawingRect.Left) 
//    / this.tileunit);
//int row = (int)Math.Round((location.Y - this.ceiling.DrawingRect.Top) 
//    / this.tileunit);

//private List<AupuBuckle> _drawProducts = new List<AupuBuckle>();

//uint _rs = (uint)Math.Max(0, (int)(_row - tile.MaxUnit));
//uint _re = (uint)Math.Min(this.rows,
//    _row + (uint)Math.Round(product.Height / tile.Factor));
//uint _cs = (uint)Math.Max(0, (int)(_col - tile.MaxUnit));
//uint _ce = (uint)Math.Min(this.columns,
//    _col + (uint)Math.Round(product.Width / tile.Factor));

//return _filter(product, _rs, _re, _cs, _ce);

//uint _rs = (uint)Math.Max(0, (int)(_row - tile.MaxUnit));
//uint _re = (uint)Math.Min(this.rows,
//    _row + (uint)Math.Round(product.Height / tile.Factor));
//uint _cs = (uint)Math.Max(0, (int)(_col - tile.MaxUnit));
//uint _ce = (uint)Math.Min(this.columns,
//    _col + (uint)Math.Round(product.Width / tile.Factor));

//return _filter(product, _rs, _re, _cs, _ce);

//for (uint i = this.rows * this.columns - 1; i >= 0; i--)
//{
//    if (this.products[i] != null)
//        //&& !this.addedProducts.Contains(this.products[i]))
//        this.products[i].Release();
//    this.products[i] = null;
//}

//// 产品所在位置的逻辑坐标
//product.Location = new PointF(ceiling.Left
//    + this.tile.Factor * _tcol, 
//    ceiling.Top + this.tile.Factor * _row);
//this.SetDrawingRect(product, _row, _tcol);
//this.products[product.Index] = product;
//this.filterProducts(product, _row, _tcol);

//this.appendAdded(product);
//this.appendAdded(product);

//product.Translate(ceiling.GetDispCoord(delta));
//this.ceiling.GetLogicCoord(product.DrawingRect.Location));

//_PreviewTile();

//if (this.tile.Length > 0)
//{
//    //this.addedProducts = new List<AupuBuckle>();
//    //_PreviewTile();
//    _TileProducts();
//}

//public void ReTileProducts(AupuBuckle[] _products)
//{
//    _PreviewTile();

//    for (int i = 0; _products != null && i < _products.Length; i++)
//    {
//        if (_products[i] == null || _products[i].Index < 0)
//            continue;
//        filterProducts(_products[i], 
//            (uint)_products[i].Index / this.columns,
//            (uint)_products[i].Index % this.columns);
//    }

//    _TileAddedProducts();
//}

//if (allSelected)
//    this.tile.Clear();

//this.removeDrawProduct(product);
//this.removeDrawProduct(product);
//this.removeDrawProduct(product);
//this.appendDrawProduct(product);
//this.appendDrawProduct(product);
//this.appendDrawProduct(addedProducts[i]);

//// 产品所在位置的逻辑坐标
//product.Location = new PointF(ceiling.Left + this.tile.Factor * _col,
//    ceiling.Top + this.tile.Factor * _row);

//this.SetDrawingRect(product, (uint)_row, (uint)_col);
//if (product.Integrity > 10)
//{
//    appendAdded(product);
//    return filterProducts(product, (uint)_row, (uint)_col);
//}
//else
//    return null;

//else if (this.ceiling.Contain(pt, 40))
//{
//    return InsertAdded(product, pt);
//    //Intersect(product);
//    //return filterProducts(product);
//}
//else
//{
//    this.appendAdded(product);
//    return null;
//}

//if (_row < 0 || _col < 0)
//    throw new Exception("这里出错了啊！");
//InsertAdded(_temp[i], (uint)_row, (uint)_col);

//filterProducts(selectProducts[i],
//    (uint)selectProducts[i].Index / this.columns,
//    (uint)selectProducts[i].Index % this.columns);

//if (selectProducts[i].Index < 0)
//    continue;
//this.appendAdded(selectProducts[i]);
//this.products[selectProducts[i].Index] = null;
//selectProducts[i].Index = -1;

//_row = _tcol;
//_col = (int)this.columns - _trow
//    - (int)(_temp[i].Width / this.tile.Factor);
//pt = new PointF(
//    ceiling.Left + _col * this.tile.Factor - ydelta,
//    ceiling.Top + _row * this.tile.Factor + xdelta);

//_row = (int)this.rows - _tcol
//    - (int)(_temp[i].Height / this.tile.Factor);
//_col = _trow;
//pt = new PointF(
//    ceiling.Left + _col * this.tile.Factor + ydelta,
//    ceiling.Top + _row * this.tile.Factor - xdelta);

//public List<AupuBuckle> TransSelection()
//{
//    List<AupuBuckle> result = new List<AupuBuckle>();

//    for (int i = 0; i < this.selectProducts.Count; i++)
//    {
//        this.selectProducts[i].Transpose(1);
//        if (selectProducts[i].Index < 0)
//            continue;

//        List<AupuBuckle> list = filterProducts(selectProducts[i],
//            (uint)selectProducts[i].Index / this.columns,
//            (uint)selectProducts[i].Index % this.columns);
//        if (list != null)
//        {
//            for (int j = 0; j < list.Count; j++)
//                result.Add(list[j]);
//        }

//        this.appendAdded(selectProducts[i]);
//        this.products[selectProducts[i].Index] = null;
//    }

//    return result;
//}

//if (selectProducts[i].Index >= 0)
//{
//    List<AupuBuckle> list = filterProducts(selectProducts[i],
//        (uint)selectProducts[i].Index / this.columns,
//        (uint)selectProducts[i].Index % this.columns);
//    if (list != null)
//    {
//        for (int j = 0; j < list.Count; j++)
//            result.Add(list[j]);
//    }

//    this.appendAdded(selectProducts[i]);
//    this.products[selectProducts[i].Index] = null;
//}

//_np.Index = -1;
//_np.Index = -1;
//_np.Index = -1;
//_np.Index = -1;
//if (r2.Left < r1.Left)
//if (r2.Top < r1.Top)
//if (r2.Bottom > r1.Bottom)
//if (r2.Right > r1.Right)

//private void _PreviewTile()
//{
//    if (products != null)
//    {
//        for (int i = 0; i < products.Length; i++)
//        {
//            if (this.products[i] != null)
//                this.products[i].Release();
//            this.products[i] = null;
//        }
//    }

//    for (int i = 0; i < aidProducts.Count; i++)
//    {
//        if (aidProducts[i] != null)
//            aidProducts[i].Release();
//    }
//    aidProducts.Clear();

//    //this._drawProducts.Clear();

//    if (this.tile == null || this.tile.Factor == 0)
//        return;
//    this.tileunit = this.tile.Factor / this.ceiling.Scale;

//    uint _rows = (uint)Math.Ceiling(this.ceiling.Height
//        / this.tile.Factor);
//    uint _cols = (uint)Math.Ceiling(this.ceiling.Width
//        / this.tile.Factor);

//    if (this.rows == _rows && this.columns == _cols)
//        return;
//    this.rows = _rows; this.columns = _cols;

//    if (this.products != null)
//        this.products = null;
//    this.products = new AupuBuckle[_rows * _cols];
//    for (int i = 0; i < products.Length; i++)
//        products[i] = null;
//}

//private void _TileProducts()
//{
//    if (tile == null || tile.Rows == 0 || tile.Cols == 0)
//        return;

//    for (uint i = 0; i < this.rows; i += tile.Rows)
//    {
//        for (uint j = 0; j < this.columns; j += tile.Cols)
//        {
//            if (tile.Tiles[0].Visible)
//                _Tile(tile.Tiles[0], i, j);
//            if (tile.Tiles[1].Visible)
//                _Tile(tile.Tiles[1], i,
//                    j + tile.Tiles[0].ColRepeat * tile.Tiles[0].Cols);
//            if (tile.Tiles[2].Visible)
//                _Tile(tile.Tiles[2],
//                    i + tile.Tiles[0].RowRepeat * tile.Tiles[0].Rows, j);
//            if (tile.Tiles[3].Visible)
//                _Tile(tile.Tiles[3],
//                    i + tile.Tiles[1].RowRepeat * tile.Tiles[1].Rows,
//                    j + tile.Tiles[2].ColRepeat * tile.Tiles[2].Cols);
//        }
//    }
//}

//for (int i = 0; i < this.rows; i++)
//{
//    for (int j = 0; j < this.columns; j++)
//    {
//        if (this.products[i * this.columns + j] != null 
//            && !this.addedProducts.Contains(this.products[i 
//            * this.columns + j]))
//            this.products[i * this.columns + j].Release();
//        this.products[i * this.columns + j] = null;
//    }
//}

//for (int i = 0; i < this.rows; i++)
//{
//    for (int j = 0; j < this.columns; j++)
//    {
//        AupuBuckle product = this.products[i * this.columns + j];
//        if (product != null)
//        {
//            this.appendDrawProduct(product);
//            this.selectProducts.Add(product);
//        }
//    }
//}

//for (int i = 0; i < this.rows; i++)
//{
//    for (int j = 0; j < this.columns; j++)
//    {
//        if (this.products[i * this.columns + j] != null)
//            this.products[i * this.columns + j].Release();
//        this.products[i * this.columns + j] = null;
//    }
//}

//for (int i = 0; i < this._drawProducts.Count; i++)
//    this.addedProducts[i].Release();
//this._drawProducts.Clear();

//public void ClearProducts()
//{
//    for (int i = 0; i < this.addedProducts.Count; i++)
//        this.addedProducts[i].Release();
//    this.addedProducts.Clear();

//    for (int i = 0; i < this.aidProducts.Count; i++)
//        this.aidProducts[i].Release();
//    this.aidProducts.Clear();

//    for (int i = (int)(this.rows * this.columns) - 1; i >= 0; i--)
//    {
//        if (this.products[i] != null)
//            this.products[i].Release();
//        this.products[i] = null;
//    }

//    this.selectProducts.Clear();
//    this.rows = this.columns = 0;
//    this.tile.Clear();
//}

//for (int i = 0; i < this.rows; i++)
//{
//    for (int j = 0; j < this.columns; j++)
//    {
//        if (this.products[i * this.columns + j] != null)
//            this.products[i * this.columns + j].Release();
//        this.products[i * this.columns + j] = null;
//    }
//}

//private void removeDrawProduct(AupuBuckle product)
//{
//    //if (this._drawProducts.Contains(product))
//    //    this._drawProducts.Remove(product);
//}

//private void appendDrawProduct(AupuBuckle product)
//{
//    //if (this._drawProducts.Contains(product))
//    //    this._drawProducts.Remove(product);
//    //this._drawProducts.Add(product);
//}

//if (tile.Length > 0)
//{
//    _PreviewTile();
//    _TileProducts();
//}

//this.aidProducts[i].DrawBorder(graphics, borderPen);
//for (int i = 0; i < this._drawProducts.Count; i++)
//{
//    this._drawProducts[i].Draw(graphics);
//}

//for (int i = 0; i < this.rows; i++)
//{
//    for (int j = 0; j < this.columns; j++)
//    {
//        if (this.products[i * columns + j] == null)
//            continue;
//        this.products[i * columns + j].Draw(graphics);
//        this.products[i * columns + j].DrawBorder(graphics, 
//            borderPen);
//    }
//}

//private List<AupuBuckle> InsertAdded(AupuBuckle product, uint _row, uint _col)
//{
//    this.SetDrawingRect(product, _row, _col);
//    this.appendAdded(product);
//    return this.filterProducts(product, _row, _col);
//}

//// 产品所在位置的逻辑坐标
//product.Location = new PointF(
//    ceiling.Left + this.tile.Factor * _col,
//    ceiling.Top + this.tile.Factor * _row);
//this.appendAdded(product);

//this.SetDrawingRect(product, _row, _col);
//if (product.Integrity > 10)
//{
//    this.appendAdded(product);
//    return this.filterProducts(product, _row, _col);
//}
//else
//    return null;

//if (product.Integrity > 10)
//{
//    this.appendAdded(product);
//    return filterProducts(product);
//}
//else
//{
//    return null;
//}

//if (tile.Length > 0)
//{
//    this.tile.Trans(_angle);
//    _PreviewTile();
//    _TileProducts();
//}

//if (_angle > 0)
//{
//    for (int i = 0; i < _temp.Count; i++)
//    {
//        _temp[i].Transpose();

//        if (_temp[i].Index > -1)
//        {
//            int _row = _temp[i].Index % (int)this.rows,
//                _col = (int)this.columns - _temp[i].Index / (int)this.rows
//                - (int)(_temp[i].Width / this.tile.Factor);
//            if (_row < 0 || _col < 0)
//                throw new Exception("这里出错了啊！");
//            InsertAdded(_temp[i], (uint)_row, (uint)_col);
//        }
//        else
//        {
//            PointF pt = new PointF(_temp[i].Location.X,
//                _temp[i].Location.Y + _temp[i].Height);
//            pt = StrLine.Trans(pt, ca);
//            InsertAdded(_temp[i], pt);
//        }
//    }
//}
//else
//{
//    for (int i = 0; i < _temp.Count; i++)
//    {
//        _temp[i].Transpose();

//        if (_temp[i].Index > -1)
//        {
//            int _col = _temp[i].Index / (int)this.rows,
//                _row = (int)this.rows - _temp[i].Index % (int)this.rows
//                - (int)(_temp[i].Height / this.tile.Factor);
//            if (_row < 0 || _col < 0)
//                throw new Exception("这里出错了啊！");
//            InsertAdded(_temp[i], (uint)_row, (uint)_col);
//        }
//        else
//        {
//            PointF pt = new PointF(_temp[i].Location.X + _temp[i].Width,
//                _temp[i].Location.Y);
//            pt = StrLine.UnTrans(pt, ca);
//            InsertAdded(_temp[i], pt);
//        }
//    }
//}

//PointF pt = new PointF(
//    ceiling.Left + _col * this.tile.Factor - ydelta,
//    ceiling.Top + _row * this.tile.Factor + xdelta);

//if (this.products != null)
//{
//    for (int i = 0; i < this.products.Length; i++)
//    {
//        if (this.products[i] != null)
//            this.products[i].Translate(point);
//    }
//}

//for (int i = 0; i < this.rows; i++)
//{
//    for (int j = 0; j < this.columns; j++)
//    {
//        if (this.products[i * columns + j] != null)
//            this.products[i * columns + j].Translate(point);
//    }
//}

//public Rectangle Trans(RectangleF rect, Point dlt, PointF rlt,
//    float scale)
//{
//    return new Rectangle(
//        (int)Math.Round((rect.X - rlt.X) / scale) + dlt.X,
//        (int)Math.Round((rect.Y - rlt.Y) / scale) + dlt.Y,
//        (int)Math.Round(rect.Width / scale), 
//        (int)Math.Round(rect.Height / scale));
//}

//product.DisplayScans[i] = Trans(product.Scans[i],
//    this.ceiling.DrawingRect.Location, rlt, ceiling.Scale);

//for (int i = 0; i < aidProducts.Count; i++)
//if (isCover(product, temp))
//{
//    result.Add(temp);
//    this.addedProducts.Remove(temp);
//    this.removeDrawProduct(temp);
//}

//if (isCover(product, temp))
//{
//    result.Add(temp);
//    this.addedProducts.Remove(temp);
//    this.removeDrawProduct(temp);
//}
//Rectangle rect = Rectangle.Intersect(product.DrawingRect,
//    temp.DrawingRect);

//if (rect.Width * rect.Height * 10 >=
//    temp.DrawingRect.Width * temp.DrawingRect.Height * 8)
//{
//    result.Add(temp);
//    addedProducts.Remove(temp);
//    this.removeDrawProduct(temp);
//}

//if (isCover(product, temp))
//{
//    result.Add(temp);
//    this.removeDrawProduct(temp);
//    this.products[index] = null;
//}
//else if (product.Width * product.Height * 10
//    > temp.Width * temp.Height)
//{
//    _split(product, temp);
//    result.Add(temp);
//    this.removeDrawProduct(temp);
//    this.products[temp.Index] = null;
//}

//_filterTileProduct(product, index, result);

//Rectangle rect = Rectangle.Intersect(product.DrawingRect,
//    temp.DrawingRect);

//if (rect.Width * rect.Height * 10 >=
//    temp.DrawingRect.Width * temp.DrawingRect.Height * 9)
//{
//    result.Add(temp);
//    this.removeDrawProduct(temp);
//    this.products[index] = null;
//}
//else if (rect.Width * rect.Height * 10 >=
//    temp.DrawingRect.Width * temp.DrawingRect.Height)
//{
//    _split(product, temp);
//    result.Add(temp);
//    this.removeDrawProduct(temp);
//    products[temp.Index] = null;
//}

//List<AupuBuckle> result = new List<AupuBuckle>();
//for (int i = 0; i < aidProducts.Count; i++)
//{
//    AupuBuckle temp = aidProducts[i];
//    if (temp == null || temp == product)
//        continue;
//    _filterAidProduct(product, temp, result);
//}

//for (int i = 0; i < addedProducts.Count; i++)
//{
//    AupuBuckle temp = addedProducts[i];
//    if (temp == null || temp == product)
//        continue;
//    Rectangle rect = Rectangle.Intersect(product.DrawingRect, 
//        temp.DrawingRect);

//    if (rect.Width * rect.Height * 10 >= 
//        temp.DrawingRect.Width * temp.DrawingRect.Height * 8)
//    {
//        result.Add(temp);
//        addedProducts.Remove(temp);
//        this.removeDrawProduct(temp);
//    }
//}

//for (uint i = _rs; i < _re; i++)
//{
//    for (uint j = _cs; j < _ce; j++)
//    {
//        AupuBuckle temp = this.products[i * this.columns + j];
//        if (temp == null || temp == product 
//            || addedProducts.Contains(temp))
//            continue;
//        _filterTileProduct(product, (uint)(i * this.columns + j), 
//            result);
//    }
//}
//return result;

//double _delta = 0.2 * Math.Min(product.Width, product.Height);
//if (Math.Abs(product.Location.X - _col * tile.Factor - ceiling.Left) < _delta
//    && Math.Abs(product.Location.Y - _row * tile.Factor - ceiling.Top) < _delta)
//    return true;
//return false;
//int _fact = this.tile.Factor;

//product.Location = logicLoca;
//product.Location = this.ceiling.GetLogicCoord(location);
//PointF logicPt = ceiling.GetLogicCoord(product.DrawingRect.Location);
//product.Location = new PointF(logicPt.X + product.Location.X,
//    logicPt.Y + product.Location.Y);

//// 产品所在位置的逻辑坐标
//product.Location = ceiling.GetLogicCoord(product.DrawingRect.Location);

//int _col = (int)Math.Round((product.DrawingRect.X 
//    - ceiling.DrawingRect.X) / tileunit);
//int _row = (int)Math.Round((product.DrawingRect.Y 
//    - ceiling.DrawingRect.Y) / tileunit);

//if (_col >= 0 && _row >= 0 && IsAccurate(product, _row, _col))
//{
//    product.Location = new PointF(ceiling.Left + this.tile.Factor * _col,
//        ceiling.Top + this.tile.Factor * _row);
//    this.SetDrawingRect(product, (uint)_row, (uint)_col);
//    return filterProducts(product, (uint)_row, (uint)_col);
//}

//return null;

//Rectangle rect = Rectangle.Intersect(ceiling.DrawingRect, 
//    pdt.DrawingRect);
//if (rect.Width * rect.Height * 5 
//    < pdt.DrawingRect.Width * pdt.DrawingRect.Height)
//    continue;

//for (int i = 0; i < this.Rows; i++)
//{
//    for (int j = 0; j < this.Columns; j++)
//    {
//        AupuBuckle pdt = this.Products[i * this.Columns + j];
//        if (pdt == null || pdt.Integrity < 10)
//            continue;
//        this.CountProduct(_statis, pdt);
//    }
//}

//for (int i = 0; i < this.aidProducts.Count; i++)
//{
//    AupuBuckle pdt = this.aidProducts[i];
//    if (pdt == null)
//        continue;
//    if (pdt.Index > -1 && Products[pdt.Index] == pdt)
//        continue;

//    if (index != pdt.Index)
//    {
//        if (id > 0)
//            this.CountProduct(_statis, id, integrity);
//        id = pdt.OriginalRow.ID;
//        index = pdt.Index;
//        integrity = 0;
//    }
//    integrity += pdt.Integrity;
//}

//if (index > -1)
//{
//    this.CountProduct(_statis, id, integrity);
//}

//for (int i = 0; i < product.Scans.Length; i++)
//{
//    if (product.Scans[i].Width <= 40 || product.Scans[i].Height <= 40)
//        continue;
//    product.Integrity += 100 *
//        (product.Scans[i].Width * product.Scans[i].Height)
//        / (product.Width * product.Height);
//}
//if (product.Scans.Length == 1)
//    return;
//if (product.Scans.Length == 1)
//{
//    product.Integrity += 100 * (product.Scans[0].Width 
//        * product.Scans[0].Height) / (product.Width * product.Height);
//    return;
//}

//RectangleF rect = new RectangleF(product.Location.X,
//    product.Location.Y, product.Width, product.Height);
//Region rgn = new Region(rect);
//PointF rlt = new PointF(ceiling.Left, ceiling.Top);

//rgn.Intersect(pset.ActuralRegion);
//product.Integrity = 0;
//product.Scans = rgn.GetRegionScans(matrix);
//product.DisplayScans = new Rectangle[product.Scans.Length];

//for (int i = 0; i < product.Scans.Length; i++)
//{
//    product.DisplayScans[i] = Trans(product.Scans[i],
//        this.ceiling.DrawingRect.Location, rlt, ceiling.Scale);

//    if (product.Scans[i].Width <= 40 || product.Scans[i].Height <= 40)
//        continue;
//    product.Integrity += 100 *
//        (product.Scans[i].Width * product.Scans[i].Height)
//        / (product.Width * product.Height);
//}

//private void DrawAddedProducts(Graphics graphics)
//{
//    for (int i = 0; i < this.addedProducts.Count; i++)
//        this.addedProducts[i].Draw(graphics);
//}

//private void _filterTileProduct(AupuBuckle product, uint index, 
//    List<AupuBuckle> result)
//{
//    AupuBuckle temp = products[index];
//    Rectangle rect = Rectangle.Intersect(product.DrawingRect, 
//        temp.DrawingRect);

//    if (rect.Width * rect.Height * 10 >= 
//        temp.DrawingRect.Width * temp.DrawingRect.Height * 9)
//    {
//        result.Add(temp);
//        this.removeDrawProduct(temp);
//        this.products[index] = null;
//    }
//    else if (rect.Width * rect.Height * 10 >= 
//        temp.DrawingRect.Width * temp.DrawingRect.Height)
//    {
//        _split(product, temp);
//        products[temp.Index] = null;
//        result.Add(temp);
//        this.removeDrawProduct(temp);
//    }
//}

//private void _filterAidProduct(AupuBuckle product, AupuBuckle temp, 
//    List<AupuBuckle> result)
//{
//    Rectangle rect = Rectangle.Intersect(product.DrawingRect, 
//        temp.DrawingRect);

//    if (rect.Width * rect.Height * 10 >= 
//        temp.DrawingRect.Width * temp.DrawingRect.Height * 8)
//    {
//        result.Add(temp);
//        this.aidProducts.Remove(temp);
//        this.removeDrawProduct(temp);
//    }
//    //else if (rect.Width * rect.Height * 4 >= 
//    //    temp.DrawingRect.Width * temp.DrawingRect.Height)
//    //{
//    //    _split(product, temp);
//    //    result.Add(temp);
//    //    this.aidProducts.Remove(temp);
//    //    this.removeDrawProduct(temp);
//    //}
//}

//private void _split(AupuBuckle product, AupuBuckle temp)
//{
//    Rectangle r1 = product.DrawingRect, r2 = temp.DrawingRect;

//    if (r2.Left < r1.Left)
//    {
//        AupuBuckle _np = temp.clone();
//        _np.DrawingRect = new Rectangle(r2.Left, r2.Top,
//            r1.Left - r2.Left, r2.Height);
//        _np.Index = temp.Index;
//        _np.Integrity = 100.0f
//            * (_np.DrawingRect.Width * _np.DrawingRect.Height)
//            / (float)(r2.Width * r2.Height);
//        aidProducts.Add(_np);
//    }

//    if (r2.Top < r1.Top)
//    {
//        AupuBuckle _np = temp.clone();
//        _np.DrawingRect = new Rectangle(Math.Max(r1.Left, r2.Left), r2.Top,
//            Math.Min(r1.Right, r2.Right) - Math.Max(r1.Left, r2.Left),
//            r1.Top - r2.Top);
//        _np.Index = temp.Index;
//        _np.Integrity = 100.0f
//            * (_np.DrawingRect.Width * _np.DrawingRect.Height)
//            / (float)(r2.Width * r2.Height);
//        aidProducts.Add(_np);
//    }

//    if (r2.Bottom > r1.Bottom)
//    {
//        AupuBuckle _np = temp.clone();
//        _np.DrawingRect = new Rectangle(Math.Max(r1.Left, r2.Left), r1.Bottom,
//            Math.Min(r1.Right, r2.Right) - Math.Max(r1.Left, r2.Left),
//            r2.Bottom - r1.Bottom);
//        _np.Index = temp.Index;
//        _np.Integrity = 100.0f
//            * (_np.DrawingRect.Width * _np.DrawingRect.Height)
//            / (float)(r2.Width * r2.Height);
//        aidProducts.Add(_np);
//    }

//    if (r2.Right > r1.Right)
//    {
//        AupuBuckle _np = temp.clone();
//        _np.DrawingRect = new Rectangle(r1.Right, r2.Top,
//            r2.Right - r1.Right, r2.Height);
//        _np.Index = temp.Index;
//        _np.Integrity = 100.0f
//            * (_np.DrawingRect.Width * _np.DrawingRect.Height)
//            / (float)(r2.Width * r2.Height);
//        aidProducts.Add(_np);
//    }
//}

//private bool isCover(AupuBuckle product, AupuBuckle temp)
//{
//    RectangleF rect = new RectangleF(product.Location.X, product.Location.Y, 
//        product.Width, product.Height);

//    for (int i = 0; i < temp.Scans.Length; i++)
//    {
//        if (!rect.Contains(temp.Scans[i]))
//            return false;
//    }

//    return true;
//}

//Rectangle r1 = product.DrawingRect, r2 = temp.DisplayScans[0];
//RectangleF
//    r3 = new RectangleF(product.Location.X, product.Location.Y,
//        product.Width, product.Height),
//    r4 = temp.Scans[0];
//r4 = new RectangleF(temp.Location.X, temp.Location.Y, 
//    temp.Width, temp.Height);

//private void _split(AupuBuckle product, AupuBuckle temp)
//{
//    Rectangle r1 = product.DrawingRect, r2 = temp.DisplayScans[0];
//    RectangleF
//        r3 = new RectangleF(product.Location.X, product.Location.Y,
//            product.Width, product.Height),
//        r4 = temp.Scans[0];
//        //r4 = new RectangleF(temp.Location.X, temp.Location.Y, 
//        //    temp.Width, temp.Height);

//    if (r2.Left < r1.Left)
//    {
//        AupuBuckle _np = temp.clone();
//        _np.Scans = new RectangleF[1];
//        _np.Scans[0] = new RectangleF(r4.Left, r4.Top,
//            r3.Left - r4.Left, r4.Height);
//        _np.DisplayScans = new Rectangle[1];
//        _np.DisplayScans[0] = new Rectangle(r2.Left, r2.Top,
//            r1.Left - r2.Left, r2.Height);
//        _np.Index = temp.Index;
//        _np.Integrity = 100.0f * (_np.Scans[0].Width * _np.Scans[0].Height) 
//            / (product.Width * product.Height);
//        aidProducts.Add(_np);
//    }

//    if (r2.Top < r1.Top)
//    {
//        AupuBuckle _np = temp.clone();

//        _np.Scans = new RectangleF[1];
//        _np.Scans[0] = new RectangleF(Math.Max(r3.Left, r4.Left), r4.Top,
//            Math.Min(r3.Right, r4.Right) - Math.Max(r3.Left, r4.Left), r3.Top - r4.Top);
//        _np.DisplayScans = new Rectangle[1];
//        _np.DisplayScans[0] = new Rectangle(Math.Max(r1.Left, r2.Left), r2.Top,
//            Math.Min(r1.Right, r2.Right) - Math.Max(r1.Left, r2.Left), r1.Top - r2.Top);

//        _np.Integrity = 100.0f * (_np.Scans[0].Width * _np.Scans[0].Height)
//            / (product.Width * product.Height);

//        _np.Index = temp.Index;
//        aidProducts.Add(_np);
//    }

//    if (r2.Bottom > r1.Bottom)
//    {
//        AupuBuckle _np = temp.clone();

//        _np.Scans = new RectangleF[1];
//        _np.Scans[0] = new RectangleF(Math.Max(r3.Left, r4.Left), r3.Bottom,
//            Math.Min(r3.Right, r4.Right) - Math.Max(r3.Left, r4.Left), r4.Bottom - r3.Bottom);
//        _np.DisplayScans = new Rectangle[1];
//        _np.DisplayScans[0] = new Rectangle(Math.Max(r1.Left, r2.Left), r1.Bottom,
//            Math.Min(r1.Right, r2.Right) - Math.Max(r1.Left, r2.Left), r2.Bottom - r1.Bottom);

//        _np.Integrity = 100.0f * (_np.Scans[0].Width * _np.Scans[0].Height)
//            / (product.Width * product.Height);

//        _np.Index = temp.Index;
//        aidProducts.Add(_np);
//    }

//    if (r2.Right > r1.Right)
//    {
//        AupuBuckle _np = temp.clone();

//        _np.Scans = new RectangleF[1];
//        _np.Scans[0] = new RectangleF(r3.Right, r4.Top,
//            r4.Right - r3.Right, r4.Height);
//        _np.DisplayScans = new Rectangle[1];
//        _np.DisplayScans[0] = new Rectangle(r1.Right, r2.Top,
//            r2.Right - r1.Right, r2.Height);

//        _np.Integrity = 100.0f * (_np.Scans[0].Width * _np.Scans[0].Height)
//            / (product.Width * product.Height);

//        _np.Index = temp.Index;
//        aidProducts.Add(_np);
//    }
//}

//_np.DrawingRect = new Rectangle(r1.Right, r2.Top,
//    r2.Right - r1.Right, r2.Height);
//_np.Integrity = 100.0f
//    * (_np.DrawingRect.Width * _np.DrawingRect.Height)
//    / (float)(r2.Width * r2.Height);

//_np.Scans[0] = new RectangleF(Math.Max(product.Location.X, temp.Location.X), 
//    temp.Location.Y,
//    Math.Min(product.Location.X + product.Width, temp.Location.X + temp.Width) 
//    - Math.Max(product.Location.X, temp.Location.X),
//    product.Location.Y - temp.Location.Y);

//_np.DrawingRect = new Rectangle(Math.Max(r1.Left, r2.Left), r1.Bottom,
//    Math.Min(r1.Right, r2.Right) - Math.Max(r1.Left, r2.Left),
//    r2.Bottom - r1.Bottom);
//_np.Integrity = 100.0f
//    * (_np.DrawingRect.Width * _np.DrawingRect.Height)
//    / (float)(r2.Width * r2.Height);

//_np.DrawingRect = new Rectangle(r2.Left, r2.Top,
//    r1.Left - r2.Left, r2.Height);
//_np.Integrity = 100.0f
//    * (_np.DrawingRect.Width * _np.DrawingRect.Height)
//    / (float)(r2.Width * r2.Height);

//_np.DrawingRect = new Rectangle(Math.Max(r1.Left, r2.Left), r2.Top,
//    Math.Min(r1.Right, r2.Right) - Math.Max(r1.Left, r2.Left),
//    r1.Top - r2.Top);
//_np.Integrity = 100.0f
//    * (_np.DrawingRect.Width * _np.DrawingRect.Height)
//    / (float)(r2.Width * r2.Height);

//if (product.Index > -1)
//    this.products[product.Index] = null;
//else if (this.addedProducts.Contains(product))
//    this.addedProducts.Remove(product);
//else if (this.aidProducts.Contains(product))
//    this.aidProducts.Remove(product);
//this.removeDrawProduct(product);

//public void TileProducts()
//{
//    if (this.tile.Length > 0)
//    {
//        _PreviewTile();
//        _TileProducts();
//    }
//    _TileAddedProducts();
//}

//if (row != null)
//{
//    BuckleNode node = AupuTile.GetProductNode(row);
//    Point location = new Point(int.Parse(ms[i].Groups[3].Value),
//        int.Parse(ms[i].Groups[4].Value));
//    bool _trans = (int.Parse(ms[i].Groups[2].Value) == 1);
//    this.AddProduct(new AupuBuckle(node, _trans), location);
//}

//return Rectangle.Round(new RectangleF((rf.X - rlt.X) / scale + dlt.X,
//    (rf.Y - rlt.Y) / scale + dlt.Y,
//    rf.Width / scale, rf.Height / scale));

//float scale = ceiling.Scale;
//RectangleF rf = product.Scans[i];
//Point dlt = this.ceiling.DrawingRect.Location;

//product.DisplayScans[i] = new Rectangle(
//    (int)Math.Round((rf.X - rlt.X) / scale) + dlt.X, 
//    (int)Math.Round((rf.Y - rlt.Y) / scale) + dlt.Y, 
//    (int)Math.Round(rf.Width / scale), 
//    (int)Math.Round(rf.Height / scale));

////刷新
//public void Refrush()
//{
//    for (int i = 0; products != null && i < products.Length; i++)
//    {
//        AupuBuckle product = products[i];

//        if (product == null || product.Index < 0)
//            continue;
//        uint _row = (uint)product.Index / this.columns;
//        uint _col = (uint)product.Index % this.columns;
//        this.SetDrawingRect(product, _row, _col);
//        //filterProducts(product, _row, _col);
//        //filterProducts(products[i],
//        //    (uint)products[i].Index / this.columns,
//        //    (uint)products[i].Index % this.columns);
//    }

//    for (int i = 0; i < this.addedProducts.Count; i++)
//    {
//        AupuBuckle product = addedProducts[i];

//        if (product.Index >= 0)
//        {
//            uint _row = (uint)product.Index / this.columns;
//            uint _col = (uint)product.Index % this.columns;
//            this.SetDrawingRect(product, _row, _col);
//            //filterProducts(product, _row, _col);
//        }
//        else
//            _add(product, product.Location);
//    }
//}

//double x = product.Location.X - _col * this.tile.Factor - ceiling.Left;
//double y = product.Location.Y - _row * this.tile.Factor - ceiling.Top;
//if (Math.Abs(x) < _delta && Math.Abs(y) < _delta)
//    return true;
//return false;

//this.ReTileProducts(this.products.Clone() as AupuBuckle[]);

//for (int i = 0; i < aidProducts.Count; i++)
//{
//    if (aidProducts[i].Index >= 0 
//        && products[aidProducts[i].Index] != null)
//    {
//        products[aidProducts[i].Index].Release();
//        products[aidProducts[i].Index] = null;
//    }
//}

//List<AupuBuckle> _temp = addedProducts;
//if (tile.Length > 0)
//{
//    //addedProducts = new List<AupuBuckle>();
//    _PreviewTile();
//    _TileProducts();
//}
////_TileAddedProducts();
//addedProducts = _temp;

//for (int i = 0; i < this.addedProducts.Count; i++)
//{
//    this.AddProduct(addedProducts[i], addedProducts[i].Location);
//}

//_drawingRect(product);

//private void _clock(AupuBuckle product, PointF ca)
//{
//    if (product.Index > -1)
//    {
//        int _row = product.Index % (int)this.rows,
//            _col = (int)this.columns - product.Index / (int)this.rows
//            - (int)(product.Width / this.tile.Factor);
//        if (_row < 0 || _col < 0)
//            throw new Exception("这里出错了啊！");
//        _add(product, (uint)_row, (uint)_col);
//    }
//    else
//    {
//        PointF pt = new PointF(product.Location.X,
//            product.Location.Y + product.Height);
//        pt = StrLine.Trans(pt, ca);
//        _add(product, pt);
//    }
//}

//private void _unclock(AupuBuckle product, PointF ca)
//{
//    if (product.Index > -1)
//    {
//        int _col = product.Index / (int)this.rows,
//            _row = (int)this.rows - product.Index % (int)this.rows
//            - (int)(product.Height / this.tile.Factor);
//        if (_row < 0 || _col < 0)
//            throw new Exception("这里出错了啊！");
//        _add(product, (uint)_row, (uint)_col);
//    }
//    else
//    {
//        PointF pt = new PointF(product.Location.X + product.Width,
//            product.Location.Y);
//        pt = StrLine.UnTrans(pt, ca);
//        _add(product, pt);
//    }
//}

//public void Trans(int _angle, PointF ca)
//{
//    if (this.tile == null || this.tile.Factor == 0)
//        return;
//    this.tile.Trans(_angle);

//    this.tileunit = this.tile.Factor / this.ceiling.Scale;
//    this.rows = (uint)Math.Ceiling(this.ceiling.Height 
//        / this.tile.Factor);
//    this.columns = (uint)Math.Ceiling(this.ceiling.Width 
//        / this.tile.Factor);

//    List<AupuBuckle> _temp = this.addedProducts;
//    this.addedProducts = new List<AupuBuckle>();

//    if (_angle > 0)
//    {
//        for (int i = 0; i < products.Length; i++)
//        {
//            if (products[i] == null)
//                continue;
//            products[i].Transpose();
//            _clock(products[i], ca);
//        }
//        for (int i = 0; i < _temp.Count; i++)
//        {
//            _temp[i].Transpose();
//            _clock(_temp[i], ca);
//        }
//        for (int i = 0; i < this.aidProducts.Count; i++)
//        {
//            aidProducts[i].Transpose();
//            _clock(aidProducts[i], ca);
//        }
//    }
//    else
//    {
//        for (int i = 0; i < products.Length; i++)
//        {
//            if (products[i] == null)
//                continue;
//            products[i].Transpose();
//            _unclock(products[i], ca);
//        }
//        for (int i = 0; i < _temp.Count; i++)
//        {
//            _temp[i].Transpose();
//            _unclock(_temp[i], ca);
//        }
//        for (int i = 0; i < this.aidProducts.Count; i++)
//        {
//            aidProducts[i].Transpose();
//            _unclock(aidProducts[i], ca);
//        }
//    }
//}

//private void _drawingRect(AupuBuckle product)
//{
//    Point dispt = ceiling.GetDispCoord(product.Location);
//    product.DrawingRect = new Rectangle(dispt.X, dispt.Y,
//        (int)Math.Round(product.Width / this.ceiling.Scale),
//        (int)Math.Round(product.Height / this.ceiling.Scale));
//}

//private void _transClock(AupuBuckle product, PointF ca)
//{
//    if (product.Index > -1)
//    {
//        int _row = product.Index % (int)this.rows,
//            _col = (int)this.columns - product.Index / (int)this.rows
//            - (int)(product.Width / this.tile.Factor);
//        if (_row >= 0 && _col >= 0)
//            _add(product, (uint)_row, (uint)_col);
//        else
//        {
//            PointF pt = StrLine.Trans(new PointF(product.Location.X,
//                product.Location.Y + product.Height), ca);
//            _add(product, pt);
//        }
//    }
//    else
//    {
//        PointF pt = StrLine.Trans(new PointF(product.Location.X,
//            product.Location.Y + product.Height), ca);
//        _add(product, pt);
//    }
//}

//private void _transUnclock(AupuBuckle product, PointF ca)
//{
//}

//if (_row >= 0 && _col >= 0)
//    _add(_temp[i], (uint)_row, (uint)_col);
//else
//{
//    //PointF pt = StrLine.Trans(new PointF(_temp[i].Location.X,
//    //    _temp[i].Location.Y + _temp[i].Height), ca);
//    PointF pt = new PointF(_temp[i].Location.X,
//        _temp[i].Location.Y + _temp[i].Height);
//    pt = StrLine.Trans(pt, ca);
//    _add(_temp[i], pt);
//}

//if (_row >= 0 && _col >= 0)
//    _add(_temp[i], (uint)_row, (uint)_col);
//else
//{
//    //PointF pt = StrLine.UnTrans(new PointF(_temp[i].Location.X + _temp[i].Width,
//    //    _temp[i].Location.Y), ca);
//    PointF pt = new PointF(_temp[i].Location.X + _temp[i].Width,
//        _temp[i].Location.Y);
//    pt = StrLine.UnTrans(pt, ca);
//    AddProduct(_temp[i], pt);
//}

//PointF pt = StrLine.Trans(new PointF(_temp[i].Location.X,
//    _temp[i].Location.Y + _temp[i].Height), ca);

//PointF pt = StrLine.UnTrans(new PointF(_temp[i].Location.X + _temp[i].Width,
//    _temp[i].Location.Y), ca);
//AddProduct(_temp[i], pt);

//private bool IsAccurate(Point p, int _row, int _col, double alpha)
//{
//    float dx = p.X - ceiling.DrawingRect.X - _col * tileunit;
//    float dy = p.Y - ceiling.DrawingRect.Y - _row * tileunit;

//    if ((_col < this.columns && _col > -1 && _row < this.rows && _row > -1)
//        && (Math.Abs(dx) < alpha && Math.Abs(dy) < alpha))
//        return true;

//    return false;
//}

//private bool IsAccurate(PointF pt, int _row, int _col, double alpha)
//{
//    float dx = pt.X - ceiling.Left - _col * this.tile.Factor;
//    float dy = pt.Y - ceiling.Top - _row * this.tile.Factor;

//    if ((_col < this.columns && _col > -1 && _row < this.rows && _row > -1)
//        && (Math.Abs(dx) < alpha && Math.Abs(dy) < alpha))
//        return true;

//    return false;
//}

//float dx = product.Location.X - ceiling.Left - _col * this.tile.Factor;
//float dy = product.Location.Y - ceiling.Top - _row * this.tile.Factor;

//if ((_col < this.columns && _col > -1 && _row < this.rows && _row > -1)
//    && (Math.Abs(dx) < _delta && Math.Abs(dy) < _delta))
//    return true;

//return false;

//private void _filter(AupuBuckle product, double temp)
//{
//    int _row = (int)Math.Round((product.DrawingRect.Y - ceiling.DrawingRect.Y) / tileunit);
//    int _col = (int)Math.Round((product.DrawingRect.X - ceiling.DrawingRect.X) / tileunit);
//    double _delta = temp * Math.Max(product.Width, product.Height);

//    if (_row>0&&_col>0&&IsAccurate(product.DrawingRect.Location, _row, _col, _delta))
//    {
//        product.Index = _row * (int)this.columns + _col;
//        filterProducts(product, (uint)_row, (uint)_col);
//    }
//    else
//        product.Index = -1;
//}

//AupuBuckle product = addedProducts[i];

//int _row = (int)Math.Round((product.Location.Y - ceiling.Top) / tile.Factor);
//int _col = (int)Math.Round((product.Location.X - ceiling.Left) / tile.Factor);

//if (_row > 0 && _col > 0 && IsAccurate(product, _row, _col))
//{
//    this.SetDrawingRect(product, (uint)_row, (uint)_col);
//    filterProducts(product, (uint)_row, (uint)_col);
//}
//else
//    _add(product, product.Location);

//double temp = 0.25 / ceiling.Scale;
//product.Index = _row * (int)this.columns + _col;
//product.Index = -1;

//double _delta = 0.2 * Math.Min(product.Width, product.Height);
//_filter(addedProducts[i], temp);

//int _row = (int)Math.Round((product.DrawingRect.Y - ceiling.DrawingRect.Y) / tileunit);
//int _col = (int)Math.Round((product.DrawingRect.X - ceiling.DrawingRect.X) / tileunit);

//int _col = (int)Math.Round((location.X - ceiling.DrawingRect.X) / tileunit);
//int _row = (int)Math.Round((location.Y - ceiling.DrawingRect.Y) / tileunit);

//if (_row > 0 && _col > 0 && IsAccurate(product, _row, _col))
//{
//    this.SetDrawingRect(product, (uint)_row, (uint)_col);
//    return filterProducts(product, (uint)_row, (uint)_col);
//}
//else
//{
//    _add(product, product.Location);
//}

//return null;

//int _col = (int)Math.Round((logicLoca.X - ceiling.Left) / this.tile.Factor);
//int _row = (int)Math.Round((logicLoca.Y - ceiling.Top) / this.tile.Factor);

//if (_col > 0 && _row > 0 && IsAccurate(product, _row, _col))
//{
//    this.SetDrawingRect(product, (uint)_row, (uint)_col);
//    return filterProducts(product, (uint)_row, (uint)_col);
//}
//else
//{
//    _add(product, logicLoca);
//}

//return null;

//PointF pt = this.ceiling.GetLogicCoord(location);
//double _delta = 0.2 * Math.Min(product.Width, product.Height);
//double _delta = 0.25 * Math.Max(product.Width, product.Height) / ceiling.Scale;
//double _delta = 0.2 * Math.Min(product.Width, product.Height);
//double _delta = 0.25 * Math.Max(product.Width, product.Height) / ceiling.Scale;
//double _delta = 0.2 * Math.Min(product.Width, product.Height);
//double _delta = 0.25 * Math.Max(product.Width, product.Height) / ceiling.Scale;

//int _col = (int)Math.Round((product.DrawingRect.X - ceiling.DrawingRect.X) / tileunit);
//int _row = (int)Math.Round((product.DrawingRect.Y - ceiling.DrawingRect.Y) / tileunit);

//int _col = (int)Math.Round((product.Location.X - ceiling.Left) / tile.Factor);
//int _row = (int)Math.Round((product.Location.Y - ceiling.Top) / tile.Factor);

//public List<AupuBuckle> MoveProduct(AupuBuckle product, PointF delta)
//{
//    Point pt = ceiling.GetDispCoord(delta);
//    product.Translate(pt);

//    // 从扣板格子里取下，放到附加扣板队列里
//    if (product.Index > -1)
//    {
//        if (products[product.Index] == product)
//            products[product.Index] = null;
//        if (!addedProducts.Contains(product))
//            addedProducts.Add(product);

//        product.Index = -1;
//        SizeF size = new SizeF(product.Width / ceiling.Scale,
//            product.Height / ceiling.Scale);
//        product.DrawingRect = new Rectangle(product.DrawingRect.Location,
//            Size.Round(size));
//    }

//    int _col = (int)Math.Round((product.DrawingRect.X - ceiling.DrawingRect.X) / tileunit);
//    int _row = (int)Math.Round((product.DrawingRect.Y - ceiling.DrawingRect.Y) / tileunit);

//    // 产品所在位置的逻辑坐标
//    product.Location = new PointF(ceiling.Left + this.tile.Factor * _col,
//        ceiling.Top + this.tile.Factor * _row);

//    if (_col > 0 && _row > 0 && IsAccurate(product, _row, _col))
//    {
//        this.SetDrawingRect(product, (uint)_row, (uint)_col);
//        return filterProducts(product, (uint)_row, (uint)_col);
//    }

//    return null;
//}

//pset.Interect(product);
//if (product.Scans[i].Width < 50 || product.Scans[i].Height < 50)
//    continue;

//PointF rlt = new PointF(ceiling.Left, ceiling.Top);
//Point dlt = this.ceiling.DrawingRect.Location;

//// 产品所在位置的逻辑坐标
//product.Location = new PointF(ceiling.Left + this.tile.Factor * _column,
//    ceiling.Top + this.tile.Factor * _row);

//// 逻辑坐标
//RectangleF rf = new RectangleF(product.Location.X, product.Location.Y,
//    product.Width, product.Height);

//// 屏幕物理坐标
//product.DrawingRect = Rectangle.Round(new RectangleF(
//    (rf.X - ceiling.Left) / ceiling.Scale + dlt.X,
//    (rf.Y - ceiling.Top) / ceiling.Scale + dlt.Y,
//    rf.Width / ceiling.Scale, rf.Height / ceiling.Scale));

//Region rgn = new System.Drawing.Region(rf);
//rgn.Intersect(pset.ActuralRegion);

//product.Scans = rgn.GetRegionScans(matrix);
//product.DisplayScans = new Rectangle[product.Scans.Length];
//product.Integrity = 0;

//for (int i = 0; i < product.Scans.Length; i++)
//{
//    product.DisplayScans[i] = product._trans(product.Scans[i], dlt,
//        rlt, ceiling.Scale);
//    if (product.Scans[i].Width <= 40 || product.Scans[i].Height <= 40)
//        continue;
//    //if (product.Scans[i].Width < 50 || product.Scans[i].Height < 50)
//    //    continue;
//    product.Integrity += 100 * 
//        (product.Scans[i].Width * product.Scans[i].Height)
//        / (product.Width * product.Height);
//}
