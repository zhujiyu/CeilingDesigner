using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Text;
using System.Drawing.Drawing2D;
//using System.Linq;
//using System.ComponentModel;
//using System.Windows.Forms;

namespace CeilingDesigner
{
    public class Ceiling
    {
        #region 属性列表

        private CeilingDataSet.ceiling_wallesDataTable ceilingWalles 
            = new CeilingDataSet.ceiling_wallesDataTable();

        public CeilingDataSet.ceiling_wallesDataTable CeilingWalles
        {
            get { return ceilingWalles; }
        }

        private List<Wall> walles = new List<Wall>();

        public List<Wall> Walles
        {
            get { return walles; }
        }

        private int id = 0;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private string name = "";

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int orderID = 0;

        public int OrderID
        {
            get { return orderID; }
            set { orderID = value; }
        }

        // 绘制的时候，很难确定左上角的具体坐标，需要记录各边相对原点位置
        private float left = 0;

        public float Left
        {
            get { return left; }
        }

        private float top = 0;

        public float Top
        {
            get { return top; }
        }

        private float width = 0;

        public float Width
        {
            get { return width; }
        }

        private float height = 0;

        public float Height
        {
            get { return height; }
        }

        public float Right
        {
            get { return left + width; }
            set { width = value - left; }
        }

        public float Bottom
        {
            get { return top + height; }
            set { height = value - top; }
        }

        private System.UInt32 depth = 280;

        public System.UInt32 Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        private float scale = 10;

        public float Scale
        {
            get { return scale; }
        }

        private Rectangle drawingRect = new Rectangle(0, 0, 0, 0);

        public Rectangle DrawingRect
        {
            get { return drawingRect; }
        }

        private bool closed = false;

        public bool Closed
        {
            get { return closed; }
        }

        private int selected = -1;

        public int SelectedIndex
        {
            get
            {
                if (this.selected >= 0 && this.selected < this.walles.Count)
                    return this.selected;
                else
                    return -1;
            }
            set
            {
                if (value >= -1 && value < this.walles.Count)
                    this.selected = value;
                else
                    throw new Exception("超出房屋边数范围");
            }
        }

        public Wall SelectedWall
        {
            get
            {
                if (this.selected >= 0 && this.selected < this.walles.Count)
                    return this.walles[this.selected];
                else
                    return null;
            }
            set
            {
                if (value == null)
                    this.selected = -1;
                else if (this.walles.Contains(value))
                    this.selected = this.walles.IndexOf(value);
                else
                    throw new Exception("超出房屋边数范围");
            }
        }

        public Wall PrevWall
        {
            get
            {
                if (this.selected < 0 || this.selected >= this.walles.Count)
                    return null;
                if (this.selected > 0)
                    return this.walles[this.selected - 1];
                else
                    return this.walles[this.walles.Count - 1];
            }
        }

        public Wall NextWall
        {
            get
            {
                if (this.selected < 0 || this.selected >= this.walles.Count)
                    return null;
                if (this.selected < this.walles.Count - 1)
                    return this.walles[this.selected + 1];
                else
                    return this.walles[0];
            }
        }

        public int Length
        {
            get
            {
                if (this.walles != null)
                    return this.walles.Count;
                else
                    return 0;
            }
        }

        //RectangleF[] hRects = { }, vRects = { };
        //RectangleF[] hDispRects = { }, vDispRects = { };

        int clockwise = -1;

        public int Clockwise
        {
            get { return clockwise; }
        }

        private Matrix matrix = new Matrix(1, 0, 0, 1, 0, 0);
        private GraphicsPath path = new GraphicsPath();
        private Region region = new Region();
        private Region drawingRegion = new Region();
        RectangleF[] hRects = { }, hDispRects = { };

        #endregion

        public Ceiling() { }

        public Point GetDispCoord(PointF lp)
        {
            return new Point((int)Math.Round(drawingRect.X + (lp.X - Left) / scale),
                (int)Math.Round(drawingRect.Y + (lp.Y - Top) / scale));
        }

        public Rectangle GetDispCoord(RectangleF rect)
        {
            return new Rectangle((int)Math.Round((rect.X - left) / scale) + drawingRect.Left,
                (int)Math.Round((rect.Y - top) / scale) + drawingRect.Top,
                (int)Math.Round(rect.Width / scale), (int)Math.Round(rect.Height / scale));
        }

        public PointF GetLogicCoord(Point pp)
        {
            return new PointF((float)Math.Round(left + (pp.X - drawingRect.Left) * scale),
                  (float)Math.Round(top + (pp.Y - drawingRect.Top) * scale));
        }

        //return new PointF((float)Math.Round(left + (pp.X - drawingRect.Left) * scale),
        //      (float)Math.Round(top + (pp.Y - drawingRect.Top) * scale));
        //return new PointF(
        //      this.Left + (pp.X - this.drawingRect.Left) * this.Scale,
        //      this.Top + (pp.Y - this.drawingRect.Top) * this.Scale);

        private int ClockWise()
        {
            clockwise = 0;
            for (int i = 1; i < this.walles.Count; i++)
                clockwise = walles[i - 1].ClockWise(walles[i]) > 0 ? 
                    clockwise + 1 : clockwise - 1;
            return clockwise;
        }

        public int GetWall(Point pt)
        {
            // 选中一条边
            for (int i = 0; i < this.Walles.Count; i++)
            {
                //if (this.walles[i].PaintLine.Distance(pt) < 5)
                //    return i;
                if (Keel.IsInLine(walles[i].PaintLine, pt))
                    return i;
            }
            return -1;
        }

        public static bool FilterWall(Wall cwall, Wall we)
        {
            if (cwall.PaintLine.IsParallel(we.PaintLine))
            {
                we.End = cwall.End;
                we.EndPaintPoint = cwall.EndPaintPoint;
                cwall.Begin = we.End;
                cwall.BeginPaintPoint = we.EndPaintPoint;
                return true;
            }

            return false;
        }

        public bool FilterWall(Wall cwall)
        {
            return FilterWall(cwall, this.Walles[this.Walles.Count - 1]);
        }

        public void SetCRow(CeilingDataSet.ceilingsRow crow)
        {
            this.ID = crow.ID;
            if (!crow.IsnameNull())
                this.Name = crow.name;
            if (!crow.Isorder_idNull())
                this.OrderID = crow.order_id;
        }

        public void AddWalles(CeilingDataSet.ceiling_wallesDataTable cwalles)
        {
            for (int i = 0; i < cwalles.Count; i++)
            {
                CeilingDataSet.ceiling_wallesRow row1 = cwalles[i];
                if (row1.ceiling_id != this.ID)
                    continue;
                if (row1.wallnum > 9999)
                    continue;
                CeilingDataSet.ceiling_wallesRow row2 = this.ceilingWalles.FindByID(row1.ID);
                if (row2 == null)
                    row2 = this.ceilingWalles.Newceiling_wallesRow();

                row2.BeginEdit();
                row2.ID = row1.ID;
                row2.endx = row1.endx;
                row2.endy = row1.endy;
                row2.wallnum = row1.wallnum;
                row2.radian = row1.radian;
                row2.ceiling_id = row1.ceiling_id;
                row2.EndEdit();

                if (row2.RowState == DataRowState.Detached)
                    this.ceilingWalles.Addceiling_wallesRow(row2);
            }
        }

        public void ParseData(CeilingDataSet.ceilingsRow ceilingRow)
        {
            CeilingDataSet.ceiling_wallesRow row0, row1;
            row0 = this.ceilingWalles[this.ceilingWalles.Count - 1];

            for (int i = 0; i < this.ceilingWalles.Count; i++)
            {
                Wall wall = new Wall();
                row1 = this.ceilingWalles[i];
                wall.Id = row1.ID;
                wall.Begin = new PointF(row0.endx, row0.endy);
                wall.End = new PointF(row1.endx, row1.endy);
                wall.Wallnum = row1.wallnum;
                wall.Randian = row1.radian;
                this.AddWall(wall);
                row0 = row1;
            }

            //this.SetDrawingRect(new Rectangle(ceilingRow.display_left,
            //    ceilingRow.display_top,
            //    ceilingRow.display_width, ceilingRow.display_height));
        }

        public void WriteData(CeilingDataSet.ceilingsRow row, int orderId)
        {
            row.BeginEdit();

            if (row.RowState == System.Data.DataRowState.Detached)
            {
                row.order_id = orderId;// this.orderData.ID;
                row.name = this.name;
                row.lines = this.Walles.Count;

                row.display_top = this.DrawingRect.Top;
                row.display_left = this.DrawingRect.Left;
                row.display_width = this.DrawingRect.Width;
                row.display_height = this.DrawingRect.Height;

                row.top = (float)Math.Round(this.Top, 3);
                row.left = (float)Math.Round(this.Left);
                row.width = (float)Math.Round(this.Width);
                row.height = (float)Math.Round(this.Height);

                row.scale = (float)Math.Round(this.Scale, 3);
            }
            else
            {
                if (row.order_id != orderId)
                    row.order_id = orderId;// this.orderData.ID;
                if (row.name != this.Name)
                    row.name = this.Name;
                if (row.lines != this.Walles.Count)
                    row.lines = this.Walles.Count;

                if (row.display_left != this.DrawingRect.Left)
                    row.display_left = this.DrawingRect.Left;
                if (row.display_top != this.DrawingRect.Top)
                    row.display_top = this.DrawingRect.Top;
                if (row.display_width != this.DrawingRect.Width)
                    row.display_width = this.DrawingRect.Width;
                if (row.display_height != this.DrawingRect.Height)
                    row.display_height = this.DrawingRect.Height;

                float floatTemp = (float)Math.Round(this.Top, 3);
                if (row.top != floatTemp)
                    row.top = floatTemp;
                floatTemp = (float)Math.Round(this.Left, 3);
                if (row.left != floatTemp)
                    row.left = floatTemp;
                floatTemp = (float)Math.Round(this.Width, 3);
                if (row.width != floatTemp)
                    row.width = floatTemp;
                floatTemp = (float)Math.Round(this.Height, 3);
                if (row.height != floatTemp)
                    row.height = floatTemp;
                floatTemp = (float)Math.Round(this.Scale, 3);
                if (row.scale != floatTemp)
                    row.scale = floatTemp;
            }

            row.EndEdit();
        }

        public void SaveToDB(CeilingDataSetTableAdapters.ceiling_wallesTableAdapter cwAdapter)
        {
            if (this.id < 1)
                return;

            if (this.ceilingWalles != null)
            {
                DataTable table = this.ceilingWalles.GetChanges();
                if (table != null && table.Rows.Count > 0)
                {
                    cwAdapter.Update(this.ceilingWalles);
                    this.ceilingWalles.AcceptChanges();
                }
            }

            cwAdapter.FillByCeilingId(this.ceilingWalles, this.id);
            this.PareperWalles(this.ceilingWalles);
            cwAdapter.Update(this.ceilingWalles);
            this.ceilingWalles.AcceptChanges();
        }

        private void PareperWalles(CeilingDataSet.ceiling_wallesDataTable _ceilingWalles)
        {
            CeilingDataSet.ceiling_wallesRow cwrow = null;

            for (int i = 0; i < this.walles.Count; i++)
            {
                Wall wall = this.walles[i];

                if (wall.Id < 1 || _ceilingWalles.FindByID(wall.Id) == null)
                    cwrow = _ceilingWalles.Newceiling_wallesRow();
                else
                    cwrow = _ceilingWalles.FindByID(wall.Id);

                cwrow.BeginEdit();
                if (cwrow.RowState == DataRowState.Detached)
                {
                    cwrow.endx = wall.End.X;
                    cwrow.endy = wall.End.Y;
                    cwrow.wallnum = wall.Wallnum;
                    cwrow.radian = wall.Randian;
                    cwrow.ceiling_id = this.id;
                }
                else
                {
                    if (cwrow.endx != wall.End.X)
                        cwrow.endx = wall.End.X;
                    if (cwrow.endy != wall.End.Y)
                        cwrow.endy = wall.End.Y;
                    if (cwrow.wallnum != wall.Wallnum)
                        cwrow.wallnum = wall.Wallnum;
                    if (cwrow.radian != wall.Randian)
                        cwrow.radian = wall.Randian;
                    if (cwrow.ceiling_id != this.id)
                        cwrow.ceiling_id = this.id;
                }
                cwrow.EndEdit();

                if (cwrow.RowState == DataRowState.Detached)
                    _ceilingWalles.Addceiling_wallesRow(cwrow);
            }
        }

        public void SaveToFile(CeilingDataSet.ceiling_wallesDataTable _ceilingWalles, 
            int _ceiling_id)
        {
            this.PareperWalles(_ceilingWalles);

            for (int i = 0; i < _ceilingWalles.Count; i++)
            {
                CeilingDataSet.ceiling_wallesRow cwrow = _ceilingWalles[i];
                if (cwrow.ceiling_id != this.id || cwrow.ceiling_id > 0)
                    continue;
                cwrow.BeginEdit();
                cwrow.ceiling_id = _ceiling_id;
                cwrow.EndEdit();
            }

            _ceilingWalles.AcceptChanges();
        }

        public void Clear()
        {
            this.walles.Clear();
            this.drawingRect = new Rectangle(0, 0, 0, 0);
            this.left = this.width = 0;
            this.top = this.height = 0;

            for (int i = 0; i < this.ceilingWalles.Count; i++)
            {
                if (this.ceilingWalles[i].RowState == DataRowState.Deleted)
                    continue;
                this.ceilingWalles[i].Delete();
            }
        }

        //this.ceilingWalles[i].BeginEdit();
        //this.ceilingWalles[i].EndEdit();

        //if (this.path != null)
        //{
        //    this.path.Dispose();
        //    this.path = new GraphicsPath();
        //}

        public void AddWall(Wall wall)
        {
            float right = this.Right;
            float bottom = this.Bottom;

            if (this.walles.Count > 0)
            {
                this.left = Math.Min(this.left, wall.End.X);
                this.top = Math.Min(this.top, wall.End.Y);
                right = Math.Max(right, wall.End.X);
                bottom = Math.Max(bottom, wall.End.Y);
            }
            else
            {
                this.left = wall.End.X;
                this.top = wall.End.Y;
                right = wall.End.X;
                bottom = wall.End.Y;
            }
            this.width = right - this.left;
            this.height = bottom - this.top;

            this.walles.Add(wall);
            this.drawingRect = GetDrawingRect(this.walles);
            
            if (this.walles.Count > 0)
                this.ClockWise();
        }

        public Wall PushWall()
        {
            Wall _wall = this.walles[this.walles.Count - 1];
            this.walles.RemoveAt(this.walles.Count - 1);
            this.ClockWise();
            return _wall;
        }

        // 这个是逻辑的区域
        private void RefrushRect()
        {
            if (this.walles.Count < 1) 
                return;

            float right = this.Right;
            float bottom = this.Bottom;

            this.left = this.walles[0].End.X;
            this.top = this.walles[0].End.Y;
            right = this.walles[0].End.X;
            bottom = this.walles[0].End.Y;

            for (int i = 1; i < this.walles.Count; i++)
            {
                this.left = Math.Min(this.left, this.walles[i].End.X);
                this.top = Math.Min(this.top, this.walles[i].End.Y);
                right = Math.Max(right, this.walles[i].End.X);
                bottom = Math.Max(bottom, this.walles[i].End.Y);
            }

            this.width = right - this.left;
            this.height = bottom - this.top;
        }

        public RectangleF GetRect()
        {
            return new RectangleF(left, top, width, height);
        }

        public void RefrushRegion()
        {
            if (this.region != null)
                this.region.Dispose();
            if (this.path != null)
                this.path.Dispose();
            if (this.drawingRegion != null)
                this.drawingRegion.Dispose();

            this.path = new GraphicsPath();
            for (int i = 0; i < this.walles.Count; i++)
                this.path.AddLine(this.walles[i].BeginPaintPoint,
                    this.walles[i].EndPaintPoint);
            this.drawingRegion = new Region(this.path);

            GraphicsPath _path = new GraphicsPath();
            for (int i = 0; i < this.walles.Count; i++)
                _path.AddLine(this.walles[i].Begin, this.walles[i].End);
            this.region = new Region(_path);

            this.hRects = this.region.GetRegionScans(matrix);
            this.hDispRects = this.drawingRegion.GetRegionScans(matrix);
        }

        //Matrix matrix = new Matrix(1, 0, 0, 1, 0, 0);
        //Matrix matrix = new Matrix(1, 0, 1, 0, 0, 0);
        //this.hRects = this.region.GetRegionScans(matrix);
        //this.hDispRects = this.drawingRegion.GetRegionScans(matrix);

        //_path = new GraphicsPath();
        //for (int i = 0; i < this.walles.Count; i++)
        //    _path.AddLine(this.walles[i].Begin, this.walles[i].End);
        //this.region = new Region(_path);

        //this.path = new GraphicsPath();
        //for (int i = 0; i < this.walles.Count; i++)
        //    this.path.AddLine(this.walles[i].BeginPaintPoint,
        //        this.walles[i].EndPaintPoint);
        //this.drawingRegion = new Region(this.path);

        //matrix = new Matrix(0, 1, 1, 0, 0, 0);
        //matrix = new Matrix(1, 0, 1, 0, 0, 0);
        //this.vRects = this.region.GetRegionScans(matrix);
        //this.vDispRects = this.drawingRegion.GetRegionScans(matrix);

        //for (int i = 0; i < this.vDispRects.Length; i++)
        //    this.vDispRects[i] = new RectangleF(
        //        this.vDispRects[i].Y, this.vDispRects[i].X,
        //        this.vDispRects[i].Height, this.vDispRects[i].Width);

        public void CaclRemarkLoca()
        {
            for (int i = 0; i < this.walles.Count; i++)
            {
                this.walles[i].CalcRemarkLoca(this);
            }
        }

        public static Rectangle GetUnionRect(Rectangle rect, Point p)
        {
            rect.Width = rect.Right - Math.Min(rect.Left, p.X);
            rect.X = Math.Min(rect.X, p.X);
            rect.Height = rect.Bottom - Math.Min(rect.Y, p.Y);
            rect.Y = Math.Min(rect.Y, p.Y);
            rect.Width = Math.Max(rect.Right, p.X) - rect.X;
            rect.Height = Math.Max(rect.Bottom, p.Y) - rect.Y;

            return rect;
        }

        public static Rectangle GetDrawingRect(List<Wall> walles)
        {
            if (walles.Count < 1)
                return new Rectangle(0, 0, 0, 0);
            Rectangle rect = new Rectangle();

            rect.X = walles[0].EndPaintPoint.X;
            rect.Y = walles[0].EndPaintPoint.Y;
            rect.Width = 0; rect.Height = 0;

            for (int i = 1; i < walles.Count; i++)
            {
                rect = GetUnionRect(rect, walles[i].EndPaintPoint);
            }

            return GetUnionRect(rect, walles[0].BeginPaintPoint);
        }

        public void SetDrawingRect(Rectangle rect)
        {
            if (this.Length <= 1)
                return;
            float revScale = Math.Min(rect.Width / this.width, 
                rect.Height / this.height);
            this.scale = 1 / revScale;

            float xbase = rect.Left - this.left * revScale;
            float ybase = rect.Top - this.top * revScale;

            PointF endPaint = walles[walles.Count - 1].End;
            int X = (int)(endPaint.X * revScale + xbase);
            int Y = (int)(endPaint.Y * revScale + ybase);

            for (int i = 0; i < this.walles.Count; i++)
            {
                if (endPaint == this.walles[i].Begin)
                    this.walles[i].BeginPaintPoint = new Point(X, Y);
                endPaint = this.walles[i].End;
                X = (int)(endPaint.X * revScale + xbase);
                Y = (int)(endPaint.Y * revScale + ybase);
                this.walles[i].EndPaintPoint = new Point(X, Y);
            }

            this.drawingRect = rect;
            this.RefrushRegion();
            this.CaclRemarkLoca();
        }

        public bool Contain(Point p)
        {
            for (int i = 0; i < this.hDispRects.Length; i++)
            {
                if (this.hDispRects[i].Contains(p))
                    return true;
            }
            //for (int i = 0; i < this.vDispRects.Length; i++)
            //{
            //    if (this.vDispRects[i].Contains(p))
            //        return true;
            //}

            return false;
        }

        //public bool Contain(RectangleF rect)
        //{
        //    if (!this.GetRect().Contains(rect))
        //        return false;
            
        //    for (int i = 0; i < this.hRects.Length; i++)
        //    {
        //        if (this.hRects[i].Contains(rect))
        //            return true;
        //    }
        //    for (int i = 0; i < this.vRects.Length; i++)
        //    {
        //        if (this.vRects[i].Contains(rect))
        //            return true;
        //    }

        //    return false;
        //}

        //for (int i = 0; i < this.hRects.Length; i++)
        //{
        //    if (this.hRects[i].Contains(rect))
        //        return true;
        //}
        //for (int i = 0; i < this.vRects.Length; i++)
        //{
        //    if (this.vRects[i].Contains(rect))
        //        return true;
        //}

        //return false;
        //return this.Contain(rect);

        //if (_rects.Length == 0)
        //    return false;
        //return true;

        //RectangleF[] _rects = _rgn.GetRegionScans(matrix);
        //return _rects.Length > 0;

        public bool Contain(PointF pt, float dis)
        {
            RectangleF rect = new RectangleF(pt.X - dis, pt.Y - dis, dis * 2, dis * 2);
            Region _rgn = new Region(rect);

            if (!this.GetRect().Contains(rect))
                return false;
            _rgn.Intersect(this.region);
            return _rgn.GetRegionScans(matrix).Length > 0;
        }

        public void Draw(Graphics graphics, Pen pen)
        {
            try
            {
                if (this.walles.Count < 1)
                    return;

                if (this.path != null && this.path.PointCount > 0)
                {
                    graphics.FillPath(Brushes.White, this.path);
                    graphics.DrawPath(pen, this.path);
                }
                else
                {
                    throw new Exception("绘制时出错！");
                    //DrawBorder(graphics, pen);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        public void DrawBorder(Graphics graphics, Pen pen, Pen arrowPen,
            Font lengthFont)
        {
            for (int i = 0; i < this.walles.Count; i++)
            {
                this.walles[i].Draw(graphics, pen);
                this.walles[i].DrawRemark(graphics, lengthFont, arrowPen);
            }
        }

        public void DrawLength(Graphics graphics, Pen arrowPen,
            Font lengthFont, StringFormat strFormat)
        {
            for (int i = 0; i < this.Walles.Count; i++)
            {
                this.Walles[i].DrawLength(this.clockwise, graphics,
                    arrowPen, lengthFont, strFormat);
            }
        }

        /// <summary>
        /// 移动墙壁时，绘制虚线反馈移动
        /// </summary>
        /// <param name="Graphics">画板</param>
        /// <param name="moving">移动位置</param>
        /// <param name="auxiKeelPen">画笔</param>
        /// <param name="location">移动到的位置</param>
        public void PaintSelectWall(Graphics Graphics, int moving, 
            Pen auxiKeelPen, Point location)
        {
            Wall wall1, wall2;

            if (moving == 1)
            {
                if (this.SelectedIndex > 0)
                    wall1 = this.Walles[this.SelectedIndex - 1];
                else
                    wall1 = this.Walles[this.Walles.Count - 1];
                wall2 = this.SelectedWall;

                Graphics.DrawLine(auxiKeelPen, wall1.BeginPaintPoint, location);
                Graphics.DrawLine(auxiKeelPen, wall2.EndPaintPoint, location);
            }
            else if (moving == 2)
            {
                //Point delta = new Point(location.X 
                //    - (wall1.BeginPaintPoint.X + wall1.EndPaintPoint.X) / 2,
                //    location.Y 
                //    - (wall1.BeginPaintPoint.Y + wall1.EndPaintPoint.Y) / 2);

                wall1 = this.SelectedWall;
                Point delta = new Point(location.X - wall1.CenterPaintPoint.X,
                    location.Y - wall1.CenterPaintPoint.Y);
                Point begin = new Point(wall1.BeginPaintPoint.X + delta.X, 
                    wall1.BeginPaintPoint.Y + delta.Y);
                Point end = new Point(wall1.EndPaintPoint.X + delta.X, 
                    wall1.EndPaintPoint.Y + delta.Y);
                Graphics.DrawLine(auxiKeelPen, begin, end);
            }
            else if (moving == 3)
            {
                wall1 = this.SelectedWall;

                if (this.SelectedIndex < this.Walles.Count - 1)
                    wall2 = this.Walles[this.SelectedIndex + 1];
                else
                    wall2 = this.Walles[0];

                Graphics.DrawLine(auxiKeelPen, wall1.BeginPaintPoint, 
                    location);
                Graphics.DrawLine(auxiKeelPen, wall2.EndPaintPoint, location);
            }
        }

        //Point center = new Point((wall1.BeginPaintPoint.X + wall1.EndPaintPoint.X) / 2, 
        //(wall1.BeginPaintPoint.Y + wall1.EndPaintPoint.Y) / 2);
        //Point delta = new Point(mouseLocation.X - center.X, mouseLocation.Y - center.Y);

        /// <summary>
        /// 移动主龙骨时，对移动范围进行限制，
        /// 限定当前的龙骨，不能移动到墙外，不能和其他龙骨重合
        /// </summary>
        /// <param name="_keel">要移动的龙骨</param>
        /// <param name="_kSet">吊顶上的龙骨列表</param>
        /// <param name="delta">位移，逻辑坐标</param>
        /// <returns>是否在可移动的范围，true是可以移动，false是不能移动</returns>
        public bool InFreeZone(Keel _keel, List<Keel> _kSet, PointF delta)
        {
            if (Math.Abs(delta.X) + Math.Abs(delta.Y) < 10)
                return true;
            PointF dpt = new PointF(delta.X + _keel.Center.X, delta.Y + _keel.Center.Y);

            if (!this.Contain(dpt, 40.0f))
                return false;

            for (int i = 0; i < _kSet.Count; i++)
            {
                if (_keel == _kSet[i])
                    continue;
                if (_kSet[i].Distance(dpt) < 100)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 该方法只改变边的显示位置，不改变逻辑位置，
        /// 只应用在整个图形的平移操作中
        /// </summary>
        /// <param name="delta">平移的偏移量</param>
        public void Translate(Point delta)
        {
            for (int i = 0; i < this.walles.Count; i++)
            {
                this.walles[i].Translate(delta);
                //this.walles[i].Remark.CalcRect();
            }

            this.drawingRect.X += delta.X;
            this.drawingRect.Y += delta.Y;
            this.RefrushRegion();
        }

        public void Modify(Wall wall, double length)
        {
            PointF delta1, delta2, direct;
            bool adb = false, ade = false;
            int index = this.walles.IndexOf(wall), i = 0, flag1, flag2, 
                adjust = 1;
            double dleng = length - wall.Length, _len = wall.Length * 0.1;

            if (wall.Length < 1)
                throw new Exception("出现了长度为0的边！");
            wall.EditAdjust++;

            delta1 = new PointF(
                (float)(Math.Abs(wall.Vector.X) * dleng / wall.Length) / 2,
                (float)(Math.Abs(wall.Vector.Y) * dleng / wall.Length) / 2);
            delta2 = new PointF(-delta1.X, -delta1.Y);

            if (wall.Begin.X - wall.End.X > _len 
                || wall.Begin.Y - wall.End.Y > _len)
            {
                PointF temp = delta1;
                delta1 = delta2;
                delta2 = temp;
            }

            while (!adb && !ade)
            {
                for (i = 1; i < this.walles.Count && !adb; i ++)
                {
                    flag1 = index + i;
                    flag2 = flag1 + 1;
                    flag1 = flag1 >= this.walles.Count ? flag1 - this.walles.Count : flag1;
                    flag2 = flag2 >= this.walles.Count ? flag2 - this.walles.Count : flag2;

                    if (walles[flag1].PaintLine.IsParallel(wall.PaintLine))
                        continue;
                    direct = this.walles[flag2].Vector;
                    _len = this.walles[flag2].Length;
                    this.MoveWall(flag1, delta1);

                    if (this.walles[flag2].EditAdjust < adjust 
                        && this.walles[flag2].Length > _len * 0.1
                        && (direct.X * this.walles[flag2].Vector.X >= 0 
                        && direct.Y * this.walles[flag2].Vector.Y >= 0 ) )
                    {
                        adb = true;
                        break;
                    }
                }

                for (i = 1; i < this.walles.Count && !ade; i ++ )
                {
                    flag1 = index - i; 
                    flag2 = flag1 - 1;
                    flag1 = flag1 < 0 ? flag1 + this.walles.Count : flag1;
                    flag2 = flag2 < 0 ? flag2 + this.walles.Count : flag2;

                    if (walles[flag1].PaintLine.IsParallel(wall.PaintLine))
                        continue;
                    direct = this.walles[flag2].Vector;
                    _len = this.walles[flag2].Length;
                    this.MoveWall(flag1, delta2);

                    if (this.walles[flag2].EditAdjust < adjust 
                        && this.walles[flag2].Length > _len * 0.1
                        && (direct.X * this.walles[flag2].Vector.X >= 0 
                        && direct.Y * this.walles[flag2].Vector.Y >= 0))
                    {
                        ade = true;
                        break;
                    }
                }
                adjust++;
            }

            this.CaclRemarkLoca();
        }

        public void ChangeLength(int index, uint length)
        {
            Wall wall = this.walles[index];
            if (Math.Abs(wall.Length - length) < 1)
                return;
            double scale = 1 - length / (double)wall.Length;
            int move = (Math.Abs(wall.Vector.X) > 0 ? 
                wall.Vector.X : wall.Vector.Y) > 0 ? 1 : 0;

            scale = move > 0 ? -scale : scale;

            PointF movedActual = new PointF((float)(wall.Vector.X * scale),
                (float)(wall.Vector.Y * scale));
            Point movedPixel = new Point((int)(wall.PaintLine.Vector.X * scale),
                (int)(wall.PaintLine.Vector.Y * scale));

            if (move == 0)
                index = index > 0 ? index - 1 : this.walles.Count - 1;

            this._movePoint(index, movedActual);
            this._movePoint(index, movedPixel);
            this._CalcRemarkLoca(index);
        }

        private void _CalcRemarkLoca(int index)
        {
            this.walles[index].CalcRemarkLoca(this);
            if (index < this.walles.Count - 1)
                this.walles[index + 1].CalcRemarkLoca(this);
            else
                this.walles[0].CalcRemarkLoca(this);
        }

        private void _movePoint(int index, PointF movedActual)
        {
            Wall wall = this.walles[index];

            wall.End = new PointF(wall.End.X + movedActual.X, 
                wall.End.Y + movedActual.Y);
            if (index < this.walles.Count - 1)
                this.walles[index + 1].Begin = wall.End;
            else
                this.walles[0].Begin = wall.End;

            this.RefrushRect();
        }

        private void _movePoint(int index, Point movedPixel)
        {
            Wall wall = this.walles[index];

            wall.EndPaintPoint = new Point(wall.EndPaintPoint.X + movedPixel.X,
                wall.EndPaintPoint.Y + movedPixel.Y);
            if (index < this.walles.Count - 1)
                this.walles[index + 1].BeginPaintPoint = wall.EndPaintPoint;
            else
                this.walles[0].BeginPaintPoint = wall.EndPaintPoint;
            
            this.drawingRect = GetDrawingRect(this.walles);
            this.RefrushRegion();
        }

        public void MoveBeginPoint(int index, Point movedPixel)
        {
            PointF movedActual = new PointF(movedPixel.X * scale, 
                movedPixel.Y * scale);

            if (index > 0)
            {
                this._movePoint(index - 1, movedActual);
                this._movePoint(index - 1, movedPixel);
                this._CalcRemarkLoca(index - 1);
            }
            else
            {
                this._movePoint(this.walles.Count - 1, movedActual);
                this._movePoint(this.walles.Count - 1, movedPixel);
                this._CalcRemarkLoca(this.walles.Count - 1);
            }
        }

        public void MoveBeginPoint(int index, PointF movedActual)
        {
            Point movedPixel = new Point(
                (int)Math.Round(movedActual.X / this.scale), 
                (int)Math.Round(movedActual.Y / this.scale));

            if (index > 0)
            {
                this._movePoint(index - 1, movedActual);
                this._movePoint(index - 1, movedPixel);
                this._CalcRemarkLoca(index - 1);
            }
            else
            {
                this._movePoint(this.walles.Count - 1, movedActual);
                this._movePoint(this.walles.Count - 1, movedPixel);
                this._CalcRemarkLoca(this.walles.Count - 1);
            }
        }

        public void MoveEndPoint(int index, PointF movedActual)
        {
            Point movedPixel = new Point(
                (int)Math.Round(movedActual.X / this.scale), 
                (int)Math.Round(movedActual.Y / this.scale));
            this._movePoint(index, movedActual);
            this._movePoint(index, movedPixel);
            this._CalcRemarkLoca(index);
        }

        public void MoveEndPoint(int index, Point movedPixel)
        {
            PointF movedActual = new PointF(movedPixel.X * scale, 
                movedPixel.Y * scale);
            this._movePoint(index, movedActual);
            this._movePoint(index, movedPixel);
            this._CalcRemarkLoca(index);
        }

        public void MoveWall(int index, Point movedPixel)
        {
            MoveWall(index, new PointF(movedPixel.X * scale, 
                movedPixel.Y * scale));
        }

        public void MoveWall(int index, PointF movedActual)
        {
            Wall wall = walles[index];
            int prev = index - 1, next = index + 1;

            StrLine temp = new StrLine(
                new PointF(wall.Begin.X + movedActual.X, wall.Begin.Y + movedActual.Y),
                new PointF(wall.End.X + movedActual.X, wall.End.Y + movedActual.Y));

            prev = prev < 0 ? prev + walles.Count : prev;
            next = next >= walles.Count ? next - walles.Count : next;

            wall.Begin = temp.Intersect(walles[prev]);
            wall.End = temp.Intersect(walles[next]);

            wall.PhysicsCoord(this);

            walles[prev].End = walles[index].Begin;
            walles[next].Begin = walles[index].End;
            walles[prev].EndPaintPoint = walles[index].BeginPaintPoint;
            walles[next].BeginPaintPoint = walles[index].EndPaintPoint;

            this.RefrushRect();
            this.drawingRect = GetDrawingRect(this.walles);
            this.RefrushRegion();
            this.CaclRemarkLoca();
        }

        //Point cd = new Point(
        //    this.drawingRect.Left + this.drawingRect.Width / 2,
        //    this.drawingRect.Top + this.drawingRect.Height / 2);
        //PointF ca = new PointF(this.left + this.width / 2, 
        //    this.top + this.height / 2);

        public void Trans(int angle, PointF ca)
        {
            for (int i = 0; i < this.walles.Count; i++)
            {
                walles[i].Trans(angle, ca, this);
            }

            this.RefrushRect();
            this.drawingRect = GetDrawingRect(this.walles);
            this.RefrushRegion();
            this.CaclRemarkLoca();
        }

        public void Statistic角线(ProductSet pset)
        {
            int id = 0;
            System.UInt32 length = 0, count = 0;

            for (int i = 0; i < this.Walles.Count; i++)
            {
                length += this.Walles[i].Length + 60;
            }

            count = (System.UInt32)Math.Ceiling((float)length / 3000);
            id = pset.FindByProductName("角线");
            if (id > 0)
                pset.AddGood(id, count);
        }
    }
}

//public bool Contain(int x, int y)
//{
//    for (int i = 0; i < this.hDispRects.Length; i++)
//    {
//        if (this.hDispRects[i].Contains(x, y))
//            return true;
//    }
//    for (int i = 0; i < this.vDispRects.Length; i++)
//    {
//        if (this.vDispRects[i].Contains(x, y))
//            return true;
//    }
//    return false;
//}

//public bool Contain(Point pt, int dis)
//{
//    Rectangle rect = new Rectangle(pt.X - dis, pt.Y - dis, 
//        dis * 2, dis * 2);
//    if (!this.drawingRect.Contains(rect))
//        return false;

//    for (int i = 0; i < this.hDispRects.Length; i++)
//    {
//        if (this.hDispRects[i].Contains(rect))
//            return true;
//    }
//    for (int i = 0; i < this.vDispRects.Length; i++)
//    {
//        if (this.vDispRects[i].Contains(rect))
//            return true;
//    }

//    return false;
//}

//public bool InFreeZone(Keel _keel, List<Keel> keelset, PointF delta)
//{
//    if (Math.Abs(delta.X) + Math.Abs(delta.Y) < 3)
//        return true;

//    PointF dpt = new PointF(delta.X + _keel.CenterPaintPoint.X,
//        delta.Y + _keel.CenterPaintPoint.Y);
//    int dis = (int)(100 / this.scale);

//    if (!this.Contain(Point.Round(dpt), dis))
//        return false;

//    for (int i = 0; i < keelset.Count; i++)
//    {
//        if (_keel == keelset[i])
//            continue;
//        if (keelset[i].PaintLine.Distance(dpt) < dis)
//            return false;
//    }

//    return true;
//}

//if (Math.Abs(wall.Vector.X) > 0)
//{
//    move = wall.End.X > wall.Begin.X ? 1 : 0;
//}
//else
//{
//    move = wall.End.Y > wall.Begin.Y ? 1 : 0;
//}

//PointF r1 = temp.Intersect(walles[prev]), r2 = temp.Intersect(walles[next]);
//walles[index].BeginPaintPoint = new Point(
//    (int)(walles[index].BeginPaintPoint.X + (r1.X - walles[index].Begin.X) / scale),
//    (int)(walles[index].BeginPaintPoint.Y + (r1.Y - walles[index].Begin.Y) / scale));
//walles[prev].EndPaintPoint = walles[index].BeginPaintPoint;
//walles[index].EndPaintPoint = new Point(
//    (int)(walles[index].EndPaintPoint.X + (r2.X - walles[index].End.X) / scale),
//    (int)(walles[index].EndPaintPoint.Y + (r2.Y - walles[index].End.Y) / scale));
//walles[next].BeginPaintPoint = walles[index].EndPaintPoint;

//Wall we = this.Walles[this.Walles.Count - 1];

//if (cwall.PaintLine.IsParallel(we.PaintLine))
//{
//    we.End = cwall.End;
//    we.EndPaintPoint = cwall.EndPaintPoint;
//    cwall.Begin = we.End;
//    cwall.BeginPaintPoint = we.EndPaintPoint;
//    return true;
//}

//return false;

//public void ParseData(CeilingDataSet.ceilingsRow crow,
//    CeilingDataSet.ceiling_wallesDataTable cwalles)
//{
//    CeilingDataSet.ceiling_wallesRow row1, row2;

//    this.ID = crow.ID;
//    if (!crow.IsnameNull())
//        this.Name = crow.name;
//    if (!crow.Isorder_idNull())
//        this.OrderID = crow.order_id;

//    for (int i = 0; i < cwalles.Count; i++)
//    {
//        row1 = cwalles[i];
//        if (row1.ceiling_id != this.ID)
//            continue;
//        row2 = this.CeilingWalles.FindByID(row1.ID);
//        if (row2 == null)
//            row2 = this.CeilingWalles.Newceiling_wallesRow();

//        row2.BeginEdit();
//        row2.ID = row1.ID;
//        row2.endx = row1.endx;
//        row2.endy = row1.endy;
//        row2.wallnum = row1.wallnum;
//        row2.radian = row1.radian;
//        row2.ceiling_id = row1.ceiling_id;
//        row2.EndEdit();

//        if (row2.RowState == DataRowState.Detached)
//            this.CeilingWalles.Addceiling_wallesRow(row2);
//    }

//    row1 = this.CeilingWalles[this.CeilingWalles.Count - 1];

//    for (int i = 0; i < this.CeilingWalles.Count; i++)
//    {
//        Wall wall = new Wall();
//        row2 = this.CeilingWalles[i];
//        wall.Id = row2.ID;
//        wall.Begin = new PointF(row1.endx, row1.endy);
//        wall.End = new PointF(row2.endx, row2.endy);
//        wall.Wallnum = row2.wallnum;
//        wall.Randian = row2.radian;
//        this.AddWall(wall);
//        row1 = row2;
//    }

//    this.SetDrawingRect(new Rectangle(crow.display_left, crow.display_top, 
//        crow.display_width, crow.display_height));
//}

//public void Modify(int index, double length)
//{
//    if (index < 0 || index >= this.walles.Count)
//        return;
//    this.Modify(this.walles[index], length);
//}

//if (move == 1)
//{
//    this._movePoint(index, movedActual);
//    this._movePoint(index, movedPixel);
//    this._CalcRemarkLoca(index);
//}
//else
//{
//    index = index > 0 ? index - 1 : this.walles.Count - 1;
//    this._movePoint(index, movedActual);
//    this._movePoint(index, movedPixel);
//    this._CalcRemarkLoca(index);
//}

//PointF movedActual = new PointF((float)(wall.Begin.X + wall.Vector.X * scale),
//    (float)(wall.Begin.Y + wall.Vector.Y * scale));
//Point movedPixel = new Point(
//    wall.BeginPaintPoint.X + (int)(wall.PaintLine.Vector.X * scale),
//    wall.BeginPaintPoint.Y + (int)(wall.PaintLine.Vector.Y * scale));

//MoveBeginPoint(index, movedActual);
//MoveBeginPoint(index, movedPixel);

//wall.End = new PointF(
//    (float)(wall.Begin.X + (wall.End.X - wall.Begin.X) * scale), 
//    (float)(wall.Begin.Y + (wall.End.Y - wall.Begin.Y) * scale));
//if (index < this.walles.Count - 1)
//    this.walles[index + 1].Begin = wall.End;
//else
//    this.walles[0].Begin = wall.End;

//wall.EndPaintPoint = new Point(
//    wall.BeginPaintPoint.X + (int)((wall.EndPaintPoint.X - wall.BeginPaintPoint.X) * scale), 
//    wall.BeginPaintPoint.Y + (int)((wall.EndPaintPoint.Y - wall.BeginPaintPoint.Y) * scale));
//if (index < this.walles.Count - 1)
//    this.walles[index + 1].BeginPaintPoint = wall.EndPaintPoint;
//else
//    this.walles[0].BeginPaintPoint = wall.EndPaintPoint;

//this.RefrushRect();
//this.drawingRect = GetDrawingRect(this.walles);

//this.CaclRemarkLoca();

//wall.CalcRemarkLoca(this);
//if (index < this.walles.Count - 1)
//    this.walles[index + 1].CalcRemarkLoca(this);
//else
//    this.walles[0].CalcRemarkLoca(this);

//wall.CalcRemarkLoca(this);
//if (index < this.walles.Count - 1)
//    this.walles[index + 1].CalcRemarkLoca(this);
//else
//    this.walles[0].CalcRemarkLoca(this);

//public void UnTrans()
//{
//    Point cd = new Point(
//        this.drawingRect.Left + this.drawingRect.Width / 2,
//        this.drawingRect.Top + this.drawingRect.Height / 2);
//    PointF ca = new PointF(this.left + this.width / 2, 
//        this.top + this.height / 2);

//    for (int i = 0; i < this.walles.Count; i++)
//    {
//        walles[i].Trans(-90, ca, cd, this);
//        //walles[i].UnTrans(ca, cd, this);
//    }

//    this.RefrushRect();
//    this.drawingRect = GetDrawingRect(this.walles);
//    this.RefrushRegion();
//    this.CaclRemarkLoca();
//}

//public Ceiling Clone()
//{
//    Ceiling ceiling = new Ceiling();
//    for (int i = 0; i < this.walles.Count; i++)
//    {
//        ceiling.AddWall(this.walles[i].Clone());
//    }
//    return ceiling;
//}

//public Point toPaintPoint(PointF pf)
//{
//    Point p = new Point();
//    p.X = (int)(this.drawingRect.Left + (pf.X - this.left) / this.scale);
//    p.Y = (int)(this.drawingRect.Top + (pf.Y - this.top) / this.scale);
//    return p;
//}

//public PointF toActualPoint(Point p)
//{
//    PointF pf = new PointF();
//    pf.X = this.left + (p.X - this.drawingRect.Left) * this.scale;
//    pf.Y = this.top + (p.Y - this.drawingRect.Top) * this.scale;
//    return pf;
//}

//{
//    for (int i = 0; i < this.walles.Count; i++)
//        this.walles[i].Draw(graphics, pen);
//}
//graphics.DrawRectangle(pen, this.drawingRect);

//private void DrawArrow(Graphics graphics, Point p, Point delta, Pen arrowPen)
//{
//    double a = 0.5, b = 0.866, c = (delta.X + delta.Y) * 3;
//    graphics.DrawLine(arrowPen, p.X + 6 * delta.Y, p.Y - 6 * delta.X, p.X - 6 * delta.Y, p.Y + 6 * delta.X);

//    if (Math.Abs(delta.X) < 0.1 || Math.Abs(delta.Y) < 0.1)
//    {
//        graphics.DrawLine(arrowPen, p.X, p.Y, p.X + delta.Y + delta.X * 3, p.Y + delta.X + delta.Y * 3);
//        graphics.DrawLine(arrowPen, p.X, p.Y, p.X - delta.Y + delta.X * 3, p.Y - delta.X + delta.Y * 3);
//    }
//    else if (Math.Abs(delta.X - delta.Y) < 0.1)
//    {
//        c = (delta.X + delta.Y) * 3;
//        graphics.DrawLine(arrowPen, p.X, p.Y, p.X + (int)(b * c), p.Y + (int)(a * c));
//        graphics.DrawLine(arrowPen, p.X, p.Y, p.X + (int)(a * c), p.Y + (int)(b * c));
//    }
//    else
//    {
//        c = (delta.X - delta.Y) * 3;
//        graphics.DrawLine(arrowPen, p.X, p.Y, p.X + (int)(b * c), p.Y - (int)(a * c));
//        graphics.DrawLine(arrowPen, p.X, p.Y, p.X + (int)(a * c), p.Y - (int)(b * c));
//    }
//}

//public void DrawLength(Wall wall, Graphics graphics, Pen arrowPen, Font lengthFont, StringFormat strFormat)
//{
//    Point p1 = wall.BeginPaintPoint;
//    Point p2 = wall.EndPaintPoint;
//    Point d1 = new Point(p2.X - p1.X, p2.Y - p1.Y);
//    double dleng = wall.PaintLine.Length;

//    d1.X = (int)(d1.X * 2 / dleng);
//    d1.Y = (int)(d1.Y * 2 / dleng);

//    if (this.Clockwise < 0)
//    {
//        p1.X += d1.Y * 8; p1.Y -= d1.X * 8;
//        p2.X += d1.Y * 8; p2.Y -= d1.X * 8;
//    }
//    else
//    {
//        p1.X -= d1.Y * 8; p1.Y += d1.X * 8;
//        p2.X -= d1.Y * 8; p2.Y += d1.X * 8;
//    }

//    string length = wall.Length.ToString();
//    this.DrawArrow(graphics, p1, d1, arrowPen);
//    this.DrawArrow(graphics, p2, new Point(-d1.X, -d1.Y), arrowPen);
//    graphics.DrawLine(arrowPen, p1, p2);

//    if (Math.Abs(wall.Alpha - Math.PI / 2) < 0.02 || Math.Abs(wall.Alpha + Math.PI / 2) < 0.02)
//    {
//        SizeF size = graphics.MeasureString(length, lengthFont, 20, strFormat);
//        RectangleF rect = new RectangleF((p1.X + p2.X - size.Width) / 2,
//            (p1.Y + p2.Y - size.Height) / 2, size.Width, size.Height);

//        if (this.Clockwise < 0)
//        {
//            rect.X += d1.Y * 4;
//            rect.Y -= d1.X * 4;
//        }
//        else
//        {
//            rect.X -= d1.Y * 4;
//            rect.Y += d1.X * 4;
//        }
//        graphics.DrawString(length, lengthFont, Brushes.Black, rect, strFormat);
//    }
//    else
//    {
//        SizeF size = graphics.MeasureString(length, lengthFont);
//        RectangleF rect = new RectangleF((p1.X + p2.X - size.Width) / 2,
//            (p1.Y + p2.Y - size.Height) / 2, size.Width, size.Height);

//        if (this.Clockwise < 0)
//        {
//            rect.X += d1.Y * 4;
//            rect.Y -= d1.X * 4;
//        }
//        else
//        {
//            rect.X -= d1.Y * 4;
//            rect.Y += d1.X * 4;
//        }
//        graphics.DrawString(length, lengthFont, Brushes.Black, rect);
//    }
//}

//PointF movedActual = new PointF(movedPixel.X * scale, movedPixel.Y * scale);
//this._moveEndPoint(index, movedActual);
//this._moveEndPoint(index, movedPixel);
//this._moveBeginPoint(index, movedActual);
//this._moveBeginPoint(index, movedPixel);

//Point movedPixel = new Point((int)Math.Round(movedActual.X / this.scale), (int)Math.Round(movedActual.Y / this.scale));
//this._moveEndPoint(index, movedActual);
//this._moveEndPoint(index, movedPixel);
//this._moveBeginPoint(index, movedActual);
//this._moveBeginPoint(index, movedPixel);

//this.RefrushDrawingRect();
//this.RefrushDrawingRect();

//if (wall.Length < 1)
//{
//    flag1 = index + i;
//    flag1 = flag1 >= this.walles.Count ? flag1 -= this.walles.Count : flag1;
//    if (Math.Abs(this.walles[flag1].Begin.X - this.walles[flag1].End.X) > Math.Abs(this.walles[flag1].Begin.Y - this.walles[flag1].End.Y))
//        delta1 = new PointF(0, (float)dleng / 2);
//    else
//        delta1 = new PointF((float)dleng / 2, 0);
//}
//else
//{
//    delta1 = new PointF((float)(Math.Abs(wall.Vector.X) * dleng / wall.Length) / 2,
//        (float)(Math.Abs(wall.Vector.Y) * dleng / wall.Length) / 2);
//}
//delta2 = new PointF(-delta1.X, -delta1.Y);

//for (i = 1; i < this.walles.Count && !adb; i += 2)
//{
//    flag1 = index + i; flag2 = flag1 + 1;
//    flag1 = flag1 >= this.walles.Count ? flag1 - this.walles.Count : flag1;
//    flag2 = flag2 >= this.walles.Count ? flag2 - this.walles.Count : flag2;
//    _len = this.walles[flag2].Length;

//    this.MoveWall(flag1, delta1);
//    if (this.walles[flag2].Adjust < adjust && this.walles[flag2].Length > _len * 0.1)
//    {
//        adb = true;
//        break;
//    }
//}

//for (i = 1; i < this.walles.Count && !ade; i += 2)
//{
//    flag1 = index - i; flag2 = flag1 - 1;
//    flag1 = flag1 < 0 ? flag1 + this.walles.Count : flag1;
//    flag2 = flag2 < 0 ? flag2 + this.walles.Count : flag2;
//    _len = this.walles[flag2].Length;

//    this.MoveWall(flag1, delta2);
//    if (this.walles[flag2].Adjust < adjust && this.walles[flag2].Length > _len * 0.1)
//    {
//        ade = true;
//        break;
//    }
//}

//public void Test(Graphics g)
//{
//    Random rand = new Random(13);
//    for (int i = 0; i < this.hrects.Length; i++)
//    {
//        Color color = Color.FromArgb(255, rand.Next(255), rand.Next(255), rand.Next(255));
//        g.FillRectangle(new SolidBrush(color), Rectangle.Round(this.hrects[i]));
//    }
//    for (int i = 0; i < this.vrects.Length; i++)
//    {
//        Color color = Color.FromArgb(255, rand.Next(255), rand.Next(255), rand.Next(255));
//        g.FillRectangle(new SolidBrush(color), Rectangle.Round(this.vrects[i]));
//    }
//}

//private void _moveBeginPoint(int index, Point movedPixel)
//{
//    if (index > 0)
//    {
//        this._moveEndPoint(index - 1, movedPixel);
//    }
//    else
//    {
//        this._moveEndPoint(this.walles.Count - 1, movedPixel);
//    }
//}

//private void _moveBeginPoint(int index, PointF movedActual)
//{
//    if (index > 0)
//    {
//        this._moveEndPoint(index - 1, movedActual);
//    }
//    else
//    {
//        this._moveEndPoint(this.walles.Count - 1, movedActual);
//    }
//}

//private void RefrushDrawingRect()
//{
//    Rectangle rect = new Rectangle();

//    rect.X = this.walles[0].EndPaintPoint.X;
//    rect.Y = this.walles[0].EndPaintPoint.Y;
//    rect.Width = 0; rect.Height = 0;

//    for (int i = 1; i < this.walles.Count; i++)
//    {
//        rect = GetUnionRect(rect, this.walles[i].EndPaintPoint);
//    }
//    this.drawingRect = GetUnionRect(rect, this.walles[0].BeginPaintPoint);
//}

//public bool Contain(int x, int y)
//{
//    int inter = 0;
//    StrLine line = new StrLine(new PointF(x, y), new PointF(1e11f, y));

//    for (int i = 0; i < this.walles.Count; i++)
//    {
//        if (!walles[i].PaintLine.IsParallel(line))
//        {
//            PointF p = walles[i].PaintLine.Intersect(line);
//            if (walles[i].PaintLine.Contains(p) && p.X > x)
//                inter++;
//        }
//    }

//    return inter % 2 == 1;
//}

//public bool Contain(Point p)
//{
//    if (!this.drawingRect.Contains(p))
//        return false;

//    int inters = 0;
//    for (int i = 0; i < this.walles.Count; i++)
//    {
//        Wall w = this.walles[i];
//        if (w.BeginPaintPoint.Y > p.Y && w.EndPaintPoint.Y > p.Y)
//            continue;
//        if (w.BeginPaintPoint.Y < p.Y && w.EndPaintPoint.Y < p.Y)
//            continue;
//        if (Math.Abs(w.Alpha) < 0.1 || Math.Abs(w.Alpha - Math.PI) < 0.1 || Math.Abs(w.Alpha + Math.PI) < 0.1)
//            continue;

//        StrLine line = new StrLine(new PointF(-p.X, p.Y), p);
//        PointF inter = w.PaintLine.Intersect(line);
//        if (inter.X > p.X)
//            inters++;
//    }

//    return inters % 2 == 1;
//}

//public bool Contain(Point p, int dis)
//{
//    Rectangle rect = new Rectangle(p.X - dis, p.Y - dis, dis * 2, dis * 2);
//    if (!this.drawingRect.Contains(rect))
//        return false;

//    int inters = 0;
//    for (int i = 0; i < this.walles.Count; i++)
//    {
//        Wall w = this.walles[i];
//        if (w.BeginPaintPoint.Y > p.Y + dis && w.EndPaintPoint.Y > p.Y + dis)
//            continue;
//        if (w.BeginPaintPoint.Y < p.Y - dis && w.EndPaintPoint.Y < p.Y - dis)
//            continue;

//        PointF inter;
//        if (Math.Abs(w.Alpha) < 0.1 || Math.Abs(w.Alpha - Math.PI) < 0.1 || Math.Abs(w.Alpha + Math.PI) < 0.1)
//        {
//            StrLine line = new StrLine(new PointF(p.X, -p.Y), p);
//            inter = w.PaintLine.Intersect(line);
//        }
//        else
//        {
//            StrLine line = new StrLine(new PointF(-p.X, p.Y), p);
//            inter = w.PaintLine.Intersect(line);
//            if (inter.X > p.X && w.PaintLine.Contains(inter))
//                inters++;
//        }

//        if (w.PaintLine.Distance(p) < dis && w.PaintLine.Contains(inter, true))
//            return false;
//    }

//    if (inters % 2 == 1)
//        return true;
//    return false;
//}

//public MySql.Data.MySqlClient.MySqlConnection Connection
//{
//    get { return cwAdapter.Connection; }
//    set { cwAdapter.Connection = value; }
//}

//palaceDataSet.ceiling_wallesRow cwrow = null;
//for (int i = 0; i < this.walles.Count; i++)
//{
//    Wall wall = this.walles[i];

//    if (wall.Id < 1 || this.ceilingWalles.FindByID(wall.Id) == null)
//        cwrow = this.ceilingWalles.Newceiling_wallesRow();
//    else
//        cwrow = this.ceilingWalles.FindByID(wall.Id);

//    cwrow.BeginEdit();
//    if (cwrow.RowState == DataRowState.Detached)
//    {
//        cwrow.endx = wall.End.X;
//        cwrow.endy = wall.End.Y;
//        cwrow.wallnum = wall.Wallnum;
//        cwrow.radian = wall.Randian;
//        cwrow.ceiling_id = this.id;
//    }
//    else
//    {
//        if (cwrow.endx != wall.End.X) 
//            cwrow.endx = wall.End.X;
//        if (cwrow.endy != wall.End.Y) 
//            cwrow.endy = wall.End.Y;
//        if (cwrow.wallnum != wall.Wallnum) 
//            cwrow.wallnum = wall.Wallnum;
//        if (cwrow.radian != wall.Randian)
//            cwrow.radian = wall.Randian;
//        if (cwrow.ceiling_id != this.id) 
//            cwrow.ceiling_id = this.id;
//    }
//    cwrow.EndEdit();

//    if (cwrow.RowState == DataRowState.Detached)
//        this.ceilingWalles.Addceiling_wallesRow(cwrow);
//}

//rect.Width = rect.Right - Math.Min(rect.Left, this.walles[i].EndPaintPoint.X);
//rect.X = Math.Min(rect.X, this.walles[i].EndPaintPoint.X);
//rect.Height = rect.Bottom - Math.Min(rect.Y, this.walles[i].EndPaintPoint.Y);
//rect.Y = Math.Min(rect.Y, this.walles[i].EndPaintPoint.Y);
//rect.Width = Math.Max(rect.Right, this.walles[i].EndPaintPoint.X) - rect.X;
//rect.Height = Math.Max(rect.Bottom, this.walles[i].EndPaintPoint.Y) - rect.Y;

//rect.Width = rect.Right - Math.Min(rect.Left, this.walles[0].BeginPaintPoint.X);
//rect.X = Math.Min(rect.X, this.walles[0].BeginPaintPoint.X);
//rect.Height = rect.Bottom - Math.Min(rect.Y, this.walles[0].BeginPaintPoint.Y);
//rect.Y = Math.Min(rect.Y, this.walles[0].BeginPaintPoint.Y);
//rect.Width = Math.Max(rect.Right, this.walles[0].BeginPaintPoint.X) - rect.X;
//rect.Height = Math.Max(rect.Bottom, walles[0].BeginPaintPoint.Y) - rect.Y;

//for (int i = 0; i < this.walles.Count; i++)
//    this.walles[i].Draw(graphics, pen);

//if (this.id < 1)
//    return;
//palaceDataSet.ceiling_wallesRow cwrow = null;
//for (int i = 0; i < this.walles.Count; i++)
//{
//    Wall wall = this.walles[i];

//    if (wall.Id < 1 || _ceilingWalles.FindByID(wall.Id) == null)
//        cwrow = _ceilingWalles.Newceiling_wallesRow();
//    else
//        cwrow = _ceilingWalles.FindByID(wall.Id);

//    cwrow.BeginEdit();
//    if (cwrow.RowState == DataRowState.Detached)
//    {
//        cwrow.endx = wall.End.X;
//        cwrow.endy = wall.End.Y;
//        cwrow.wallnum = wall.Wallnum;
//        cwrow.radian = wall.Randian;
//        cwrow.ceiling_id = this.id;
//    }
//    else
//    {
//        if (cwrow.endx != wall.End.X)
//            cwrow.endx = wall.End.X;
//        if (cwrow.endy != wall.End.Y)
//            cwrow.endy = wall.End.Y;
//        if (cwrow.wallnum != wall.Wallnum)
//            cwrow.wallnum = wall.Wallnum;
//        if (cwrow.radian != wall.Randian)
//            cwrow.radian = wall.Randian;
//        if (cwrow.ceiling_id != this.id)
//            cwrow.ceiling_id = this.id;
//    }
//    cwrow.EndEdit();

//    if (cwrow.RowState == DataRowState.Detached)
//        _ceilingWalles.Addceiling_wallesRow(cwrow);
//}
