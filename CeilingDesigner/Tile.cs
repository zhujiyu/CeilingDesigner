using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CeilingDesigner
{
    public enum TileStyle
    {
        OnRows, OnColumns, OnOddRows, OnOddColumns, OnEvenRows,
        OnEvenColumns, OnOddRowOdd, OnOddRowEven, OnEvenRowOdd,
        OnEvenRowEven
    };

    public class TileGrid
    {
        AupuBuckle product = null;

        public AupuBuckle Product
        {
            get { return product; }
            set { product = value; }
        }

        bool visible = false;

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        uint rows = 0;

        public uint Rows
        {
            get { return rows; }
            set { rows = value; }
        }

        uint cols = 0;

        public uint Cols
        {
            get { return cols; }
            set { cols = value; }
        }

        public uint Width
        {
            get
            {
                if (product != null )
                    return (uint)Math.Round(product.Width * colRepeat);
                else
                    return 0;
            }
        }

        public uint Height
        {
            get
            {
                if (product != null )
                    return (uint)Math.Round(product.Height * rowRepeat);
                else
                    return 0;
            }
        }

        uint rowRepeat = 0;

        public uint RowRepeat
        {
            get { return rowRepeat; }
            set { rowRepeat = value; }
        }

        uint colRepeat = 0;

        public uint ColRepeat
        {
            get { return colRepeat; }
            set { colRepeat = value; }
        }

        public TileGrid()
        {
            this.rowRepeat = 0;
            this.colRepeat = 0;
            this.product = null;
            this.visible = false;
        }

        public TileGrid(AupuBuckle _product, uint _rows, uint _cols)
        {
            this.rowRepeat = _rows;
            this.colRepeat = _cols;
            this.product = _product;
            this.visible = true;
        }

        public TileGrid Clone()
        {
            TileGrid tile = new TileGrid();
            if (this.product != null)
                tile.product = this.product.clone();
            tile.rowRepeat = this.rowRepeat;
            tile.colRepeat = this.colRepeat;
            tile.rows = this.rows;
            tile.cols = this.cols;
            tile.visible = this.visible;
            return tile;
        }
    }

    public class AupuTile
    {
        private TileGrid[] tiles = new TileGrid[4];

        uint factor = 0;

        public uint Factor
        {
            get { return factor; }
        }

        private uint maxunit = 0;

        public uint MaxUnit
        {
            get { return maxunit; }
        }

        uint width = 0, height = 0;

        uint rows = 12;

        public uint Rows
        {
            get { return rows; }
        }

        uint cols = 12;

        public uint Cols
        {
            get { return cols; }
        }

        public uint Length
        {
            get
            {
                uint _len = 0;

                for (int i = 0; i < 4; i++)
                {
                    if (tiles[i].Product != null && tiles[i].Visible
                        && tiles[i].ColRepeat > 0 && tiles[i].RowRepeat > 0)
                        _len++;
                }

                return _len;
            }
        }

        public AupuTile()
        {
            for (int i = 0; i < 4; i++)
            {
                this.tiles[i] = new TileGrid();
            }
        }

        public TileGrid this[uint index]
        {
            get
            {
                //if (index < 0 || index >= 4)
                //    throw new Exception("索引超出了数组界限");
                return this.tiles[index];
            }
        }

        public TileGrid this[int index]
        {
            get
            {
                if (index < 0 || index >= 4)
                    throw new Exception("索引超出了数组界限");
                return this.tiles[index];
            }
        }

        public void Clear()
        {
            for (int i = 0; i < 4; i++)
            {
                if (tiles[i].Product != null)
                {
                    tiles[i].Product.Release();
                    tiles[i].Product = null;
                }

                tiles[i].RowRepeat = 0;
                tiles[i].ColRepeat = 0;
                tiles[i].Rows = 0;
                tiles[i].Cols = 0;
                tiles[i].Visible = false;
            }
        }

        public string Serialize()
        {
            string _str = "#tile:[" + width + "," + height + "]{";

            for (int i = 0; i < 4; i++)
            {
                if (tiles[i].Product == null)
                    _str += "[0-0";
                else
                    _str += "[" + tiles[i].Product.OriginalRow.ID + "-" 
                        + tiles[i].Product.Trans;
                        //+ (tiles[i].Product.Trans ? "1" : "0");
                _str += "-" + (tiles[i].Visible ? 1 : 0) + "," + tiles[i].ColRepeat 
                    + "," + tiles[i].RowRepeat + "]";
            }

            return _str + "}#";
        }

        public static BuckleNode GetProductNode(CeilingDataSet.productsRow row)
        {
            BuckleNode node = BuckleList.GetProductNode(row.ID);

            if (node == null)
            {
                node = new BuckleNode(row);
                BuckleList.List.Add(node);
                node.SynLoadImage();
            }

            return node;
        }

        public void UnSerialize(string str, CeilingDataSet.productsDataTable productsDataTable)
        {
            MatchCollection ms = Regex.Matches(str, @"#tile:\[(\d+)\,(\d+)\]\{(\[(\d+)\-(\d)\-(\d)\,(\d+),(\d+)\])+\}#");
            if (ms.Count < 1)
                return;
            MatchCollection size = Regex.Matches(ms[0].Value,
                @"tile:\[(\d+)\,(\d+)\]");
            MatchCollection _tiles = Regex.Matches(ms[0].Value,
                @"\[(\d+)\-(\d)\-(\d)\,(\d+),(\d+)\]");

            this.width = uint.Parse(size[0].Groups[1].Value);
            this.height = uint.Parse(size[0].Groups[2].Value);

            for (int i = 0; i < 4 && i < _tiles.Count; i++)
            {
                if (tiles[i].Product != null)
                    tiles[i].Product.Release();

                int _pid = System.Int32.Parse(_tiles[i].Groups[1].Value);
                CeilingDataSet.productsRow row = _pid > 0 ? 
                    productsDataTable.FindByID(_pid) : null;
                BuckleNode node = row != null ? GetProductNode(row) : null;
                if (node == null)
                    continue;

                tiles[i].Product = new AupuBuckle(node, int.Parse(_tiles[i].Groups[2].Value));
                tiles[i].Visible = int.Parse(_tiles[i].Groups[3].Value) == 1 ? true : false;
                tiles[i].ColRepeat = uint.Parse(_tiles[i].Groups[4].Value);
                tiles[i].RowRepeat = uint.Parse(_tiles[i].Groups[5].Value);
            }

            _Stand();
        }

        /// <summary>
        /// 旋转铺设单元
        /// </summary>
        /// <param name="_angle">1表示顺时针旋转90度，-1表示反向90度</param>
        public void Trans(int _angle)
        {
            TileGrid[] _tiles = new TileGrid[4];
            uint temp;

            if (_angle == 1)
            {
                _tiles[0] = tiles[2];
                _tiles[1] = tiles[0];
                _tiles[3] = tiles[1];
                _tiles[2] = tiles[3];
            }
            else
            {
                _tiles[0] = tiles[1];
                _tiles[1] = tiles[3];
                _tiles[3] = tiles[2];
                _tiles[2] = tiles[0];
            }

            tiles = _tiles;

            if (!tiles[0].Visible && tiles[0].Product != null)
            {
                if (_angle == 1 && tiles[1].Visible &&
                    tiles[0].Product.OriginalRow.ID == tiles[1].Product.OriginalRow.ID)
                {
                    tiles[0].RowRepeat = tiles[1].RowRepeat;
                    tiles[0].Visible = true;
                    tiles[1].Visible = false;
                }
                if (_angle == -1 && tiles[2].Visible &&
                    tiles[0].Product.OriginalRow.ID == tiles[2].Product.OriginalRow.ID)
                {
                    tiles[0].ColRepeat = tiles[2].ColRepeat;
                    tiles[0].Visible = true;
                    tiles[2].Visible = false;
                }
            }

            if (_angle == 1 && !tiles[2].Visible && tiles[3].Visible &&
                tiles[2].Product != null && 
                tiles[2].Product.OriginalRow.ID == tiles[3].Product.OriginalRow.ID)
            {
                tiles[2].RowRepeat = tiles[3].RowRepeat;
                tiles[2].Visible = true;
                tiles[3].Visible = false;
            }

            if (_angle == -1 && !tiles[1].Visible && tiles[3].Visible &&
                tiles[1].Product != null &&
                tiles[1].Product.OriginalRow.ID == tiles[3].Product.OriginalRow.ID)
            {
                tiles[1].ColRepeat = tiles[3].ColRepeat;
                tiles[1].Visible = true;
                tiles[3].Visible = false;
            }

            for (int i = 0; i < 4; i++)
            {
                if (tiles[i].Product == null)
                    continue;
                tiles[i].Product.Transpose(_angle);

                temp = tiles[i].RowRepeat;
                tiles[i].RowRepeat = tiles[i].ColRepeat;
                tiles[i].ColRepeat = temp;

                temp = tiles[i].Rows;
                tiles[i].Rows = tiles[i].Cols;
                tiles[i].Cols = temp;
            }

            temp = this.rows;
            this.rows = this.cols;
            this.cols = temp;

            temp = this.width;
            this.width = this.height;
            this.height = temp;
        }

        //tiles[0] = tiles[1].Clone();
        //tiles[0].ColRepeat = tiles[1].ColRepeat;
        //tiles[0].Cols = tiles[1].Cols;
        //tiles[0].Rows = tiles[1].Rows;

        //tiles[0] = tiles[2].Clone();
        //tiles[0].RowRepeat = tiles[2].RowRepeat;
        //tiles[0].Rows = tiles[2].Rows;
        //tiles[0].Cols = tiles[2].Cols;

        //if (!tiles[2].Visible && tiles[3].Visible)
        //{
        //    tiles[2] = tiles[3].Clone();
        //    tiles[3].Visible = false;
        //}
        //if (!tiles[1].Visible && tiles[3].Visible)
        //{
        //    tiles[1] = tiles[3].Clone();
        //    tiles[3].Visible = false;
        //}

        public void Release()
        {
            for (int i = 0; i < 4; i++)
            {
                if (tiles[i].Product != null)
                {
                    tiles[i].Product.Release();
                }
            }
        }

        public AupuTile Clone()
        {
            AupuTile aupuTile = new AupuTile();
            //aupuTile.tiles = new TileGrid[4];
            for (int i = 0; i < 4; i++)
                aupuTile.tiles[i] = this.tiles[i].Clone();
            aupuTile.rows = this.rows;
            aupuTile.cols = this.cols;
            aupuTile.factor = this.factor;
            return aupuTile;
        }

        private int _CommonFactor(int a, int b)
        {
            if (a <= 1 || b <= 1)
                return 1;
            else if (a < b)
                return _CommonFactor(b, a);
            else if (a == b)
                return a;
            else if (a % b == 0)
                return b;
            else
                return _CommonFactor(b, a % b);
        }

        private uint _CommonFactor(uint a, uint b)
        {
            if (a == 0 && b == 0)
                throw new Exception("0和0的公约数不存在");
            else if (a == 0 || b == 0)
                return Math.Max(a, b);
            else if (a == 1 || b == 1)
                return 1;
            else if (a < b)
                return _CommonFactor(b, a);
            else if (a == b)
                return a;
            else if (a % b == 0)
                return b;
            else
                return _CommonFactor(b, a % b);
        }

        private uint _CommonMultiple(uint a, uint b)
        {
            if (a == 0)
                return b;
            else if (b == 0)
                return a;

            uint _factor = _CommonFactor(a, b);
            return a * b / _factor;
        }

        private void _SetProduct(AupuBuckle product, uint index)
        {
            if (index >= 4)
                return;

            if (this.tiles[index].Product != null)
                this.tiles[index].Product.Release();

            if (product != null)
            {
                this.tiles[index].Product = product;
                this.tiles[index].RowRepeat = 1;
                this.tiles[index].ColRepeat = 1;
                this.tiles[index].Visible = true;
            }
            else
            {
                this.tiles[index].Product = null;
                this.tiles[index].RowRepeat = 0;
                this.tiles[index].ColRepeat = 0;
                this.tiles[index].Visible = false;
            }
        }

        private void _AdjustHeight(TileGrid idx, TileGrid ad1, TileGrid ad2)
        {
            if ((!ad1.Visible && !ad2.Visible) 
                || (ad1.Height == 0 && ad2.Height == 0))
            {
                this.height = idx.Height;
                return;
            }
            ad1.RowRepeat = ad2.RowRepeat = 1;

            uint _fact1 = _CommonFactor(idx.Height, ad1.Height);
            uint _fact2 = _CommonFactor(idx.Height, ad2.Height);
            uint _rept1 = ad1.Height / _fact1;
            uint _rept2 = ad2.Height / _fact2;

            ad1.RowRepeat *= idx.Height / _fact1;
            ad2.RowRepeat *= idx.Height / _fact2;
            idx.RowRepeat *= (ad1.Visible ? _rept1 : 1)
                + (ad2.Visible ? _rept2 : 0);

            if (ad1.Visible && ad2.Visible)
            {
                uint _fact = _CommonFactor(ad1.RowRepeat, ad2.RowRepeat);
                _fact = _CommonFactor(_fact, idx.RowRepeat);
                ad1.RowRepeat /= _fact;
                ad2.RowRepeat /= _fact;
                idx.RowRepeat /= _fact;
            }

            this.height = idx.Height;
        }

        private void _AdjustWidth(TileGrid bt, TileGrid t1, TileGrid t2)
        {
            if ((!t1.Visible && !t2.Visible) 
                || (t1.Width == 0 && t2.Width == 0))
            {
                this.width = bt.Width;
                return;
            }
            t1.ColRepeat = t2.ColRepeat = 1;

            uint _fact1 = _CommonFactor(bt.Width, t1.Width);
            uint _fact2 = _CommonFactor(bt.Width, t2.Width);
            uint _rept1 = t1.Width / _fact1;
            uint _rept2 = t2.Width / _fact2;

            t1.ColRepeat *= bt.Width / _fact1;
            t2.ColRepeat *= bt.Width / _fact2;
            bt.ColRepeat *= (t1.Visible ? _rept1 : 1)
                + (t2.Visible ? _rept2 : 0);

            if (t1.Visible && t2.Visible)
            {
                uint _fact = _CommonFactor(t1.ColRepeat, t2.ColRepeat);
                _fact = _CommonFactor(_fact, bt.ColRepeat);
                t1.ColRepeat /= _fact;
                t2.ColRepeat /= _fact;
                bt.ColRepeat /= _fact;
            }

            this.width = bt.Width;
        }

        private void _AdjustColRepeat(TileGrid t1, TileGrid t2)
        {
            if (t1.Product != null && t2.Product != null
                && t1.Width != t2.Width)
            {
                int fact = _CommonFactor((int)Math.Round(t1.Product.Width),
                    (int)Math.Round(t2.Product.Width));
                t1.ColRepeat = (uint)(t2.Product.Width / fact);
                t2.ColRepeat = (uint)(t1.Product.Width / fact);
            }
        }

        private void _AdjustRowRepeat(TileGrid t1, TileGrid t2)
        {
            if (t1.Product != null && t2.Product != null
                && t1.Height != t2.Height)
            {
                int fact = _CommonFactor((int)Math.Round(t1.Product.Height),
                    (int)Math.Round(t2.Product.Height));
                t1.RowRepeat = (uint)(t2.Product.Height / fact);
                t2.RowRepeat = (uint)(t1.Product.Height / fact);
            }
        }

        private void _SetHeight(uint idx, uint tile1, uint tile2)
        {
            if (this.tiles[tile1].Product != null && this.tiles[tile1].RowRepeat > 0)
                this.height = this.tiles[tile1].Height + tiles[idx].Height;
            else if (this.tiles[tile2].Product != null && this.tiles[tile2].RowRepeat > 0)
                this.height = this.tiles[tile2].Height + tiles[idx].Height;
            else
                this.height = tiles[idx].Height * 2;
        }

        private void _SetWidth(uint idx, uint tile1, uint tile2)
        {
            if (this.tiles[tile1].Product != null && this.tiles[tile1].ColRepeat > 0)
                this.width = this.tiles[tile1].Width + tiles[idx].Width;
            else if (this.tiles[tile2].Product != null && this.tiles[tile2].ColRepeat > 0)
                this.width = this.tiles[tile2].Width + tiles[idx].Width;
            else
                this.width = tiles[idx].Width * 2;
        }

        private void _TileAll(AupuBuckle product)
        {
            Clear();

            _SetProduct(product, 0);
            _SetProduct(product.clone(), 1);
            _SetProduct(product.clone(), 2);
            _SetProduct(product.clone(), 3);

            this.tiles[1].Visible = false;
            this.tiles[2].Visible = false;
            this.tiles[3].Visible = false;
            this.width = (uint)tiles[0].Product.Width;
            this.height = (uint)tiles[0].Product.Height;
        }

        private void _TileOddRow(AupuBuckle product)
        {
            this._SetProduct(product, 0);
            this._SetProduct(product.clone(), 1);
            this.tiles[1].Visible = false;
            _AdjustRowRepeat(tiles[2], tiles[3]);

            if (tiles[2].RowRepeat > 0 && tiles[2].ColRepeat > 0)
                tiles[2].Visible = true;
            if (this.width > tiles[2].Width && tiles[3].Product != null 
                && tiles[3].RowRepeat > 0 && tiles[3].ColRepeat > 0)
                tiles[3].Visible = true;

            _AdjustWidth(tiles[0], tiles[2], tiles[3]);
            _SetHeight(0, 2, 3);
        }

        private void _TileEvenRow(AupuBuckle product)
        {
            this._SetProduct(product, 2);
            this._SetProduct(product.clone(), 3);
            this.tiles[3].Visible = false;
            _AdjustRowRepeat(tiles[0], tiles[1]);

            if (tiles[0].Product == null)
            {
                tiles[0].RowRepeat = 1;
                tiles[0].ColRepeat = 1;
                tiles[0].Rows = 1;
                tiles[0].Cols = 1;
                tiles[0].Visible = false;
            }

            if (tiles[0].Product != null && 
                tiles[0].RowRepeat > 0 && tiles[0].ColRepeat > 0)
                tiles[0].Visible = true;
            if (tiles[1].Product != null && this.width > tiles[0].Width && 
                tiles[1].RowRepeat > 0 && tiles[1].ColRepeat > 0)
                tiles[1].Visible = true;

            _AdjustWidth(tiles[2], tiles[0], tiles[1]);
            _SetHeight(2, 0, 1);
        }

        private void _TileOddCol(AupuBuckle product)
        {
            this._SetProduct(product, 0);
            this._SetProduct(product.clone(), 2);
            this.tiles[2].Visible = false;
            _AdjustColRepeat(tiles[1], tiles[3]);

            if (tiles[1].ColRepeat > 0 && tiles[1].ColRepeat > 0)
                tiles[1].Visible = true;
            if (this.height > tiles[1].Height
                && tiles[3].RowRepeat > 0 && tiles[3].ColRepeat > 0)
                tiles[3].Visible = true;

            _AdjustHeight(tiles[0], tiles[1], tiles[3]);
            _SetWidth(0, 1, 3);
        }

        private void _TileEvenCol(AupuBuckle product)
        {
            this._SetProduct(product, 1);
            this._SetProduct(product.clone(), 3);
            this.tiles[3].Visible = false;
            _AdjustColRepeat(tiles[0], tiles[2]);

            if (tiles[0].Product == null)
            {
                tiles[0].RowRepeat = 1;
                tiles[0].ColRepeat = 1;
                tiles[0].Rows = 1;
                tiles[0].Cols = 1;
                tiles[0].Visible = false;
            }

            if (tiles[0].Product != null && 
                tiles[0].ColRepeat > 0 && tiles[0].ColRepeat > 0)
                tiles[0].Visible = true;
            if (tiles[2].Product != null && this.height > tiles[0].Height && 
                tiles[2].RowRepeat > 0 && tiles[2].ColRepeat > 0)
                tiles[2].Visible = true;

            _AdjustHeight(tiles[1], tiles[0], tiles[2]);
            _SetWidth(1, 0, 2);
        }

        private void _AlignWidth(TileGrid bt, TileGrid ot, TileGrid t1, 
            TileGrid t2)
        {
            // 1、ot所在列按列排同种产品
            // 2、当前按行排列
            if (ot.Visible && ot.Width == t2.Width)
            {
                if (t1.Visible)
                {
                    uint _fact = _CommonFactor(t1.Width, bt.Width);
                    uint _temp = bt.Width / _fact;
                    bt.ColRepeat *= t1.Width / _fact;
                    t1.ColRepeat *= _temp;
                }
                this.width = bt.Width + ot.Width;
            }
            else if (!ot.Visible && !t1.Visible && !t2.Visible)
            {
                this.width = bt.Width * 2;
            }
            else if (!t1.Visible && !t2.Visible)
            {
                this.width = bt.Width + ot.Width;
            }
            else if (!ot.Visible && t1.Visible)
            {
                uint _fact = _CommonFactor(t1.Width, bt.Width);
                uint _temp = bt.Width / _fact;
                bt.ColRepeat *= t1.Width / _fact;
                t1.ColRepeat *= _temp;
                this.width = bt.Width * 2;
            }
            else if (!ot.Visible && t2.Visible)
            {
                this.width = bt.Width + t2.Width;
            }
            else
            {
                uint _width = (t2.Visible ? t2.Width : 0) 
                    + (t1.Visible ? t1.Width : 0);
                uint _fact1 = _CommonFactor(bt.Width, _width);
                uint _fact2 = _CommonFactor(ot.Width, _width);

                t2.ColRepeat *= bt.Width / _fact1 + ot.Width / _fact2;
                t1.ColRepeat *= bt.Width / _fact1 + ot.Width / _fact2;
                bt.ColRepeat *= _width / _fact1;
                ot.ColRepeat *= _width / _fact2;
                this.width = bt.Width + ot.Width;
            }
        }

        private void _AlignHeight(TileGrid bt, TileGrid ot, TileGrid t1, 
            TileGrid t2)
        {
            // 1、ot所在列按列排同种产品
            // 2、当前按行排列
            if (ot.Visible && ot.Height == t2.Height)
            {
                if (t1.Visible)
                {
                    uint _fact = _CommonFactor(t1.Height, bt.Height);
                    uint _temp = bt.Height / _fact;
                    bt.RowRepeat *= t1.Height / _fact;
                    t1.RowRepeat *= _temp;
                }
                this.height = bt.Height + ot.Height;
            }
            else if (!ot.Visible && !t1.Visible && !t2.Visible)
            {
                this.height = bt.Height * 2;
            }
            else if (!t1.Visible && !t2.Visible)
            {
                this.height = bt.Height + ot.Height;
            }
            else if (!ot.Visible && t1.Visible)
            {
                uint _fact = _CommonFactor(t1.Height, bt.Height);
                uint _temp = bt.Height / _fact;
                bt.RowRepeat *= t1.Height / _fact;
                t1.RowRepeat *= _temp;
                this.height = bt.Height * 2;
            }
            else if (!ot.Visible && t2.Visible)
            {
                this.height = bt.Height + t2.Height;
            }
            else
            {
                uint _height = (t2.Visible ? t2.Height : 0) + (t1.Visible ? t1.Height : 0);
                uint _fact1 = _CommonFactor(bt.Height, _height);
                uint _fact2 = _CommonFactor(ot.Height, _height);

                t2.RowRepeat *= bt.Height / _fact1 + ot.Height / _fact2;
                t1.RowRepeat *= bt.Height / _fact1 + ot.Height / _fact2;
                bt.RowRepeat *= _height / _fact1;
                ot.RowRepeat *= _height / _fact2;

                this.height = bt.Height + ot.Height;
            }
        }

        private void _TileOddOdd(AupuBuckle buckle)
        {
            for (int i = 0; i < 4; i++)
                _InitTileGrid(tiles[i]);
            this._SetProduct(buckle, 0);

            _AlignHeight(tiles[0], tiles[2], tiles[1], tiles[3]);
            _AlignWidth(tiles[0], tiles[1], tiles[2], tiles[3]);
            _StadRepeat();
        }

        private void _TileOddEven(AupuBuckle buckle)
        {
            for (int i = 0; i < 4; i++)
                _InitTileGrid(tiles[i]);
            this._SetProduct(buckle, 1);

            _AlignHeight(tiles[1], tiles[3], tiles[0], tiles[2]);
            _AlignWidth(tiles[1], tiles[0], tiles[3], tiles[2]);
            _StadRepeat();
        }

        private void _TileEvenOdd(AupuBuckle buckle)
        {
            for (int i = 0; i < 4; i++)
                _InitTileGrid(tiles[i]);
            this._SetProduct(buckle, 2);

            _AlignHeight(tiles[2], tiles[0], tiles[3], tiles[1]);
            _AlignWidth(tiles[2], tiles[3], tiles[0], tiles[1]);
            _StadRepeat();
        }

        private void _TileEvenEven(AupuBuckle buckle)
        {
            for (int i = 0; i < 4; i++)
                _InitTileGrid(tiles[i]);
            this._SetProduct(buckle, 3);

            _AlignHeight(tiles[3], tiles[1], tiles[2], tiles[0]);
            _AlignWidth(tiles[3], tiles[2], tiles[1], tiles[0]);
            _StadRepeat();
        }

        private void _StadRepeat()
        {
            _StadColRepeat(tiles[0], tiles[2]);
            _StadColRepeat(tiles[1], tiles[3]);
            _StadRowRepeat(tiles[0], tiles[1]);
            _StadRowRepeat(tiles[2], tiles[3]);

            if (tiles[0].Width != tiles[2].Width)
            {
                _StadColRepeat(tiles[0], tiles[3]);
                _StadColRepeat(tiles[1], tiles[2]);
            }

            if (tiles[0].Height != tiles[1].Height)
            {
                _StadRowRepeat(tiles[0], tiles[3]);
                _StadRowRepeat(tiles[1], tiles[2]);
            }
        }

        private void _StadColRepeat(TileGrid grid1, TileGrid grid2)
        {
            if (grid1.ColRepeat <= 1 || grid2.ColRepeat <= 1)
                return;
            int _w1 = (int)Math.Round(grid1.Product.Width),
                _w2 = (int)Math.Round(grid2.Product.Width);
            bool reply = false;
            uint idx1 = 0, idx2 = 0;

            //for (uint i = 1; i < grid1.ColRepeat; i++)
            //{
            //    for (uint j = 1; j < grid2.ColRepeat; j++)
            //    {
            //        if (i * _w1 == j * _w2)
            //        {
            //            uint temp = grid1.Width;
            //            grid1.ColRepeat = i;
            //            grid2.ColRepeat = j;
            //            this.width -= temp - grid1.Width;
            //        }
            //    }
            //}

            for (uint i = 1; i < grid1.ColRepeat; i++)
            {
                for (uint j = 1; j < grid2.ColRepeat; j++)
                {
                    if (i * _w1 == j * _w2)
                    {
                        reply = true;
                        idx1 = i;
                        idx2 = j;
                        break;
                    }
                }
                if (reply)
                    break;
            }

            if (reply)
            {
                while (grid1.ColRepeat > idx1 && grid2.ColRepeat > idx2)
                {
                    uint temp = grid1.Width;
                    grid1.ColRepeat -= idx1;
                    grid2.ColRepeat -= idx2;
                    this.width -= temp - grid1.Width;
                }
            }
        }

        private void _StadRowRepeat(TileGrid grid1, TileGrid grid2)
        {
            if (grid1.RowRepeat <= 1 || grid2.RowRepeat <= 1)
                return;
            int _h1 = (int)Math.Round(grid1.Product.Height),
                _h2 = (int)Math.Round(grid2.Product.Height);
            bool reply = false;
            uint idx1 = 0, idx2 = 0;

            for (uint i = 1; i < grid1.RowRepeat; i++)
            {
                for (uint j = 1; j < grid2.RowRepeat; j++)
                {
                    if (i * _h1 == j * _h2)
                    {
                        reply = true;
                        idx1 = i;
                        idx2 = j;
                        break;
                    }
                }
                if (reply)
                    break;
            }

            if (reply)
            {
                while (grid1.RowRepeat > idx1 && grid2.RowRepeat > idx2)
                {
                    uint temp = grid1.Height;
                    grid1.RowRepeat -= idx1;
                    grid2.RowRepeat -= idx2;
                    this.height -= temp - grid1.Height;
                }
            }
        }

        private void _InitTileGrid(TileGrid grid)
        {
            grid.RowRepeat = 1;
            grid.ColRepeat = 1;
            grid.Rows = 1;
            grid.Cols = 1;
        }

        private void _Stand()
        {
            uint _fact = 1, i = 0;
            this.maxunit = 0;

            for (; i < 4; i++)
            {
                if (tiles[i].Product != null
                    && tiles[i].RowRepeat > 0 && tiles[i].ColRepeat > 0)
                {
                    _fact = _CommonFactor((uint)this.tiles[i].Product.Width,
                        (uint)this.tiles[i].Product.Height);
                    break;
                }
            }

            for (i++; i < 4; i++)
            {
                if (tiles[i].Product != null
                    && tiles[i].RowRepeat > 0 && tiles[i].ColRepeat > 0)
                {
                    _fact = _CommonFactor(_fact, (uint)this.tiles[i].Product.Width);
                    _fact = _CommonFactor(_fact, (uint)this.tiles[i].Product.Height);
                }
            }

            this.factor = _fact;
            this.rows = this.height / factor;
            this.cols = this.width / factor;

            for (i = 0; i < 4; i++)
            {
                if (this.tiles[i].Product == null)
                    continue;
                tiles[i].Rows = (uint)(tiles[i].Product.Height / factor);
                tiles[i].Cols = (uint)(tiles[i].Product.Width / factor);

                if (maxunit < tiles[i].Rows)
                    maxunit = tiles[i].Rows;
                if (maxunit < tiles[i].Cols)
                    maxunit = tiles[i].Cols;
            }
        }

        public void AddBuckle(AupuBuckle buckle, uint index)
        {
            if (this.Length == index)
            {
                switch (this.Length)
                {
                    case 0:
                        _TileAll(buckle);
                        break;
                    case 1:
                        _TileEvenCol(buckle);
                        break;
                    case 2:
                        _TileEvenRow(buckle);
                        break;
                    case 3:
                        _TileEvenEven(buckle);
                        break;
                }
            }
            else
            {
                switch (index)
                {
                    case 0:
                        _TileOddOdd(buckle);
                        break;
                    case 1:
                        _TileOddEven(buckle);
                        break;
                    case 2:
                        _TileEvenOdd(buckle);
                        break;
                    case 3:
                        _TileEvenEven(buckle);
                        break;
                }

                if (!tiles[1].Visible && !tiles[3].Visible)
                    this.width /= 2;
                if (!tiles[2].Visible && !tiles[3].Visible)
                    this.height /= 2;
            }

            _Stand();
        }

        public void AddBuckle(AupuBuckle buckle)
        {
            this.AddBuckle(buckle, this.Length);
        }

        public void SetTile(AupuBuckle product, TileStyle style)
        {
            switch (style)
            {
                case TileStyle.OnRows:
                    _TileAll(product);
                    break;
                case TileStyle.OnOddRows:
                    _TileOddRow(product);
                    break;
                case TileStyle.OnEvenRows:
                    _TileEvenRow(product);
                    break;
                case TileStyle.OnOddColumns:
                    _TileOddCol(product);
                    break;
                case TileStyle.OnEvenColumns:
                    _TileEvenCol(product);
                    break;
                case TileStyle.OnOddRowOdd:
                    _TileOddOdd(product);
                    break;
                case TileStyle.OnOddRowEven:
                    _TileOddEven(product);
                    break;
                case TileStyle.OnEvenRowOdd:
                    _TileEvenOdd(product);
                    break;
                case TileStyle.OnEvenRowEven:
                    _TileEvenEven(product);
                    break;
                default:
                    throw new Exception("没有实现该方式");
            }

            _Stand();
        }
    }
}
