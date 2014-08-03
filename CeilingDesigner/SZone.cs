using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Data;
using System.Drawing;

namespace CeilingDesigner
{
    public class SZone
    {
        #region 属性列表

        private CeilingDataSet.ceiling_wallesDataTable ceilingWalles
            = new CeilingDataSet.ceiling_wallesDataTable();

        List<Wall> walles = new List<Wall>();

        public List<Wall> Walles
        {
            get { return walles; }
        }

        List<Point> paintDingdians = new List<Point>();
        List<PointF> dingdians = new List<PointF>();

        System.Drawing.Drawing2D.GraphicsPath path = null;

        uint depth = 0;

        public uint Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        Remark remark = new Remark();

        public Remark Remark
        {
            get { return remark; }
        }

        public string RemarkStr
        {
            get { return remark.CustomRemark; }
            set { remark.CustomRemark = value; }
        }

        private Rectangle rect = new Rectangle(0, 0, 0, 0);

        //public Rectangle DispRect
        //{
        //    get { return rect; }
        //}

        static int szoneCount = 1;

        private int szoneNum = 0;

        public int SzoneNum
        {
            get { return szoneNum; }
        }

        private bool closed = false;

        public bool Closed
        {
            get { return closed; }
        }

        int sidx = -1, eidx = -1;

        #endregion

        public SZone()
        {
            szoneNum = szoneCount * 10000;
            szoneCount++;
        }

        //remark.CustomRemark = "天天有喜剧未删减版本";
        //remark.Str = "天天有喜剧未删减版本";

        public void AddWall(Wall wall)
        {
            wall.Wallnum = szoneNum + walles.Count;
            walles.Add(wall);
        }

        public void Invalidate(OrderGraph graph)
        {
            graph.InvalidateRect(this.rect, 10);
            graph.InvalidateRect(this.remark.DispRect, 10);
        }

        private void _modify(Wall _wall, Ceiling ceiling)
        {
            int index = walles.IndexOf(_wall);
            int prev = index - 1, next = index + 1;

            _wall.PhysicsCoord(ceiling);

            if (prev >= 0)
            {
                walles[prev].End = _wall.Begin;
                walles[prev].EndPaintPoint = _wall.BeginPaintPoint;
            }

            if (next < walles.Count)
            {
                walles[next].Begin = _wall.End;
                walles[next].BeginPaintPoint = _wall.EndPaintPoint;
            }

            this.CalcLoca(ceiling);
        }

        public void MoveWall(Wall _wall, PointF movedActual, Ceiling ceiling)
        {
            int index = walles.IndexOf(_wall);
            int prev = index - 1, next = index + 1;

            _wall.Begin = new PointF(_wall.Begin.X + movedActual.X, 
                _wall.Begin.Y + movedActual.Y);
            _wall.End = new PointF(_wall.End.X + movedActual.X, 
                _wall.End.Y + movedActual.Y);

            if (prev >= 0)
                _wall.Begin = _wall.Intersect(walles[prev]);
            if (next < walles.Count)
                _wall.End = _wall.Intersect(walles[next]);
            _modify(_wall, ceiling);
        }

        public void ModifyWall(Wall _wall, Ceiling ceiling, 
            PointF begin, PointF end)
        {
            int index = walles.IndexOf(_wall);
            int prev = index - 1, next = index + 1;

            _wall.Begin = begin;
            _wall.End = end;
            _modify(_wall, ceiling);
        }

        public void SetLength(Wall _wall, Ceiling ceiling, float length)
        {
            if (Math.Abs(_wall.Length - length) < 1)
                return;
            float scale = 1 - length / _wall.Length;
            int index = walles.IndexOf(_wall);

            scale = index > 0 ? scale : -scale;
            index += index > 0 ? -1 : 1;
            MoveWall(walles[index], new PointF(_wall.Vector.X * scale,
                _wall.Vector.Y * scale), ceiling);
        }

        public bool FilterWall(Wall wall)
        {
            if (walles.Count > 0
                && Ceiling.FilterWall(wall, walles[walles.Count - 1]))
            {
                return true;
            }
            return false;
        }

        public bool CheckClosed(Ceiling ceiling)
        {
            List<Wall> cWalles = ceiling.Walles;
            int inters = 0;

            for (int i = 0; i < walles.Count; i++)
            {
                Wall wall = walles[i];
                for (int j = 0; j < cWalles.Count; j++)
                {
                    if (wall.IsParallel(cWalles[j]))
                        continue;
                    PointF inte = wall.Intersect(cWalles[j]);

                    if (cWalles[j].Contains(inte, true) 
                        && wall.Contains(inte, 50))
                        inters++;
                }
            }

            return (inters == 2);
        }

        public void Close(Ceiling ceiling)
        {
            List<Wall> cWalles = ceiling.Walles;
            Wall wall = walles[0];
            PointF inte = wall.Begin;

            for (int j = 0; j < cWalles.Count; j++)
            {
                if (wall.IsParallel(cWalles[j]))
                    continue;
                inte = wall.Intersect(cWalles[j]);

                if (cWalles[j].Contains(inte, true) 
                    && wall.Contains(inte, 50))
                {
                    sidx = j;
                    break;
                }
            }

            //wall.Begin = inte;
            wall.Begin = Point.Round(inte);
            wall.PhysicsCoord(ceiling);

            wall = walles[walles.Count - 1];
            if (walles.Count == 1)
            {
                for (int j = 0; j < cWalles.Count; j++)
                {
                    if (j == sidx || wall.IsParallel(cWalles[j]))
                        continue;
                    inte = wall.Intersect(cWalles[j]);

                    if (cWalles[j].Contains(inte, true) 
                        && wall.Contains(inte, 50))
                    {
                        eidx = j;
                        break;
                    }
                }
            }
            else
            {
                for (int j = 0; j < cWalles.Count; j++)
                {
                    if (wall.IsParallel(cWalles[j]))
                        continue;
                    inte = wall.Intersect(cWalles[j]);

                    if (cWalles[j].Contains(inte, true) 
                        && wall.Contains(inte, 50))
                    {
                        eidx = j;
                        break;
                    }
                }
            }

            //wall.End = inte;
            wall.End = Point.Round(inte);
            wall.PhysicsCoord(ceiling);

            this.closed = true;
        }

        public void CalcLoca(Ceiling ceiling)
        {
            int d1 = eidx - sidx, d2 = sidx - eidx;
            List<Wall> cWalles = ceiling.Walles;
            dingdians.Clear();
            paintDingdians.Clear();

            paintDingdians.Add(walles[0].BeginPaintPoint);
            dingdians.Add(walles[0].Begin);

            for (int i = 0; i < walles.Count; i++)
            {
                paintDingdians.Add(walles[i].EndPaintPoint);
                dingdians.Add(walles[i].End);
            }

            d1 += d1 < 0 ? ceiling.Length : 0;
            d2 += d2 < 0 ? ceiling.Length : 0;

            if (d1 < d2)
            {
                for (int i = eidx; i != sidx; 
                    i = i == 0 ? ceiling.Length - 1 : i - 1)
                {
                    paintDingdians.Add(cWalles[i].BeginPaintPoint);
                    dingdians.Add(cWalles[i].Begin);
                }
            }
            else
            {
                for (int i = eidx; i != sidx; 
                    i = (i == ceiling.Length - 1) ? 0 : i + 1)
                {
                    paintDingdians.Add(cWalles[i].EndPaintPoint);
                    dingdians.Add(cWalles[i].End);
                }
            }

            if (this.path != null)
                this.path.Dispose();
            this.path = new System.Drawing.Drawing2D.GraphicsPath();
            for (int i = 1; i < this.paintDingdians.Count; i++)
                this.path.AddLine(paintDingdians[i - 1], paintDingdians[i]);
            this.path.AddLine(paintDingdians[paintDingdians.Count - 1], 
                paintDingdians[0]);

            RectangleF _rect = new RectangleF(dingdians[0].X, dingdians[0].Y,
                0, 0);
            for (int i = 1; i < dingdians.Count; i++)
            {
                _rect.X = Math.Min(_rect.X, dingdians[i].X);
                _rect.Y = Math.Min(_rect.Y, dingdians[i].Y);
            }
            for (int i = 1; i < dingdians.Count; i++)
            {
                _rect.Width = Math.Max(_rect.X + _rect.Width,
                    dingdians[i].X) - _rect.X;
                _rect.Height = Math.Max(_rect.Y + _rect.Height,
                    dingdians[i].Y) - _rect.Y;
            }

            this.remark.DefRemark = "夹高：" + this.depth.ToString() + "\n尺寸："
                + Math.Round(_rect.Width) + " X " + Math.Round(_rect.Height);
            
            rect.X = paintDingdians[0].X;
            rect.Y = paintDingdians[0].Y;
            rect.Width = rect.Height = 0;

            for (int i = 1; i < paintDingdians.Count; i++)
            {
                rect.X = Math.Min(rect.X, paintDingdians[i].X);
                rect.Y = Math.Min(rect.Y, paintDingdians[i].Y);
            }
            for (int i = 1; i < paintDingdians.Count; i++)
            {
                rect.Width = Math.Max(rect.X + rect.Width,
                    paintDingdians[i].X) - rect.X;
                rect.Height = Math.Max(rect.Y + rect.Height,
                    paintDingdians[i].Y) - rect.Y;
            }

            remark.LineBegin = new Point(rect.X + rect.Width / 2, 
                rect.Y + rect.Height / 2);

            Point end = new Point(remark.LineBegin.X, 0);
            if (remark.LineBegin.Y < ceiling.DrawingRect.Y 
                + ceiling.DrawingRect.Height / 2)
            {
                end.Y = ceiling.DrawingRect.Y - 40;
                remark.Loca = new Point(end.X - 50, end.Y - 30);
            }
            else
            {
                end.Y = ceiling.DrawingRect.Y + ceiling.DrawingRect.Height 
                    + 40;
                remark.Loca = new Point(end.X - 50, end.Y + 5);
            }
            remark.LineEnd = end;

            this.remark.CalcRect();
        }

        public void Trans(int angle, Ceiling ceiling)
        {
            //Rectangle rect = ceiling.DrawingRect;
            //Point cd = new Point(rect.Left + rect.Width / 2, 
            //    rect.Top + rect.Height / 2);
            PointF ca = new PointF(ceiling.Left + ceiling.Width / 2,
                ceiling.Top + ceiling.Height / 2);

            for (int i = 0; i < this.walles.Count; i++)
            {
                walles[i].Trans(angle, ca, ceiling);
            }
            this.CalcLoca(ceiling);
        }

        /// <summary>
        /// 该方法只改变边的显示位置，不改变逻辑位置，
        /// 只应用在整个图形的平移操作中
        /// </summary>
        /// <param name="delta">平移的偏移量</param>
        /// <param name="ceiling">所在房顶</param>
        public void Translate(Point delta, Ceiling ceiling)
        {
            for (int i = 0; i < this.walles.Count; i++)
                this.walles[i].Translate(delta);
            this.CalcLoca(ceiling);
        }

        public void Refrush(Ceiling ceiling)
        {
            for (int i = 0; i < this.walles.Count; i++)
            {
                walles[i].PhysicsCoord(ceiling);
            }
            CalcLoca(ceiling);
        }

        public void Fill(Ceiling ceiling)
        {
            if (!ceiling.Walles[sidx].Contains(walles[0].Begin))
            {
                walles[0].Begin = ceiling.Walles[sidx].Intersect(walles[0]);
                walles[0].PhysicsCoord(ceiling);
            }
            if (!ceiling.Walles[eidx].Contains(walles[walles.Count - 1].End))
            {
                walles[walles.Count - 1].End = ceiling.Walles[eidx].Intersect
                    (walles[walles.Count - 1]);
                walles[walles.Count - 1].PhysicsCoord(ceiling);
            }
            CalcLoca(ceiling);
        }

        public Wall GetWall(Point pt)
        {
            for (int i = 0; i < walles.Count; i++)
            {
                //if (walles[i].PaintLine.Contains(pt, false))
                //    return walles[i];
                if (Keel.IsInLine(walles[i].PaintLine, pt))
                    return walles[i];
            }
            return null;
        }

        public void Save(CeilingDataSet.szonesDataTable _szones, 
            CeilingDataSet.ceiling_wallesDataTable _ceilingWalles, 
            int _ceiling_id)
        {
            CeilingDataSet.szonesRow szrow = _szones.NewszonesRow();
            CeilingDataSet.ceiling_wallesRow cwrow = null;

            szrow.ceiling_id = _ceiling_id;
            szrow.depth = (int)this.depth;
            szrow.remark = remark.CustomRemark;
            szrow.szone_num = this.szoneNum;
            //szrow.szone_num = walles.Count;
            szrow.beginx = walles[0].Begin.X;
            szrow.beginy = walles[0].Begin.Y;
            _szones.AddszonesRow(szrow);

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
                    cwrow.ceiling_id = _ceiling_id;
                    cwrow.endx = wall.End.X;
                    cwrow.endy = wall.End.Y;
                    cwrow.radian = wall.Randian;
                    cwrow.wallnum = wall.Wallnum;
                }
                else
                {
                    if (cwrow.ceiling_id != _ceiling_id)
                        cwrow.ceiling_id = _ceiling_id;
                    if (cwrow.endx != wall.End.X)
                        cwrow.endx = wall.End.X;
                    if (cwrow.endy != wall.End.Y)
                        cwrow.endy = wall.End.Y;
                    if (cwrow.radian != wall.Randian)
                        cwrow.radian = wall.Randian;
                    if (cwrow.wallnum != wall.Wallnum)
                        cwrow.wallnum = wall.Wallnum;
                }
                cwrow.EndEdit();

                if (cwrow.RowState == DataRowState.Detached)
                    _ceilingWalles.Addceiling_wallesRow(cwrow);
            }

            _ceilingWalles.AcceptChanges();
        }

        public void ParseData(Ceiling ceiling, CeilingDataSet.szonesRow _szone)
        {
            if (!_szone.IsdepthNull())
                this.depth = (uint)_szone.depth;
            if (!_szone.IsremarkNull())
                this.remark.CustomRemark = _szone.remark;
            if (!_szone.Isszone_numNull())
                this.szoneNum = _szone.szone_num;

            DataView dv = new DataView(this.ceilingWalles);
            dv.Sort = "wallnum";
            CeilingDataSet.ceiling_wallesRow row0 = dv[0].Row as CeilingDataSet.ceiling_wallesRow; // ceilingWalles[0];

            Wall wall = new Wall();
            wall.Id = row0.ID;
            wall.Begin = new PointF(_szone.beginx, _szone.beginy);
            wall.End = new PointF(row0.endx, row0.endy);
            wall.Wallnum = row0.wallnum;
            wall.Randian = row0.radian;
            wall.PhysicsCoord(ceiling);
            this.AddWall(wall);

            for (int i = 1; i < this.ceilingWalles.Count; i++)
            {
                CeilingDataSet.ceiling_wallesRow row1 = row0;
                row0 = dv[i].Row as CeilingDataSet.ceiling_wallesRow; // this.ceilingWalles[i];
                wall = new Wall();
                wall.Id = row0.ID;
                wall.Begin = new PointF(row1.endx, row1.endy);
                wall.End = new PointF(row0.endx, row0.endy);
                wall.Wallnum = row0.wallnum;
                wall.Randian = row0.radian;
                wall.PhysicsCoord(ceiling);
                this.AddWall(wall);
            }
        }

        public void AddWalles(CeilingDataSet.ceiling_wallesDataTable cwalles, 
            int ceilingId, int num)
        {
            int _num = (num / 10000) * 10000;
            CeilingDataSet.ceiling_wallesRow row1, row2;

            if (szoneCount <= num)
                szoneCount = num + 1;
            this.szoneNum = num;

            for (int i = 0; i < cwalles.Count; i++)
            {
                row1 = cwalles[i];

                if (row1.ceiling_id != ceilingId)
                    continue;
                if ((row1.wallnum / 10000) * 10000 != _num)
                    continue;
                
                row2 = this.ceilingWalles.FindByID(row1.ID);
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

        public void Draw(Graphics graphics, Pen mainKeelPen, Pen arrowPen, 
            Font font)
        {
            for (int i = 0; i < walles.Count; i++)
            {
                walles[i].Draw(graphics, mainKeelPen);
            }

            this.remark.Draw(graphics, font, arrowPen);

            for (int i = 0; i < walles.Count; i++)
            {
                walles[i].DrawRemark(graphics, font, arrowPen);
            }
        }

        //public void DrawRemark(Graphics graphics, Font font, Pen arrowPen)
        //{
        //    this.remark.Draw(graphics, font, arrowPen);
        //    for (int i = 0; i < walles.Count; i++)
        //    {
        //        walles[i].DrawRemark(graphics, font, arrowPen);
        //    }
        //}

        public void DrawSelectFlag(Graphics graphics, Pen selectPen)
        {
            if (!this.closed)
                return;

            if (this.path != null && this.path.PointCount > 0)
            {
                Brush brush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
                graphics.FillPath(brush, this.path);
            }

            List<Point> pdds = this.paintDingdians;
            Rectangle rect = new Rectangle(pdds[0].X - 4,
                pdds[0].Y - 4, 9, 9);
            graphics.DrawRectangle(selectPen, rect);

            for (int i = 1; i < pdds.Count; i++)
            {
                rect.X = (pdds[i - 1].X + pdds[i].X) / 2 - 4;
                rect.Y = (pdds[i - 1].Y + pdds[i].Y) / 2 - 4;
                graphics.DrawRectangle(selectPen, rect);

                rect.X = pdds[i].X - 4;
                rect.Y = pdds[i].Y - 4;
                graphics.DrawRectangle(selectPen, rect);
            }

            rect.X = (pdds[pdds.Count - 1].X + pdds[0].X) / 2 - 4;
            rect.Y = (pdds[pdds.Count - 1].Y + pdds[0].Y) / 2 - 4;
            graphics.DrawRectangle(selectPen, rect);
        }
    }
}

//if (sidx < eidx)
//{
//    for (int i = eidx; i > sidx; i--)
//    {
//        paintDingdians.Add(cWalles[i].BeginPaintPoint);
//        dingdians.Add(cWalles[i].Begin);
//    }
//}
//else
//{
//    for (int i = eidx; i < sidx; i++)
//    {
//        paintDingdians.Add(cWalles[i].EndPaintPoint);
//        dingdians.Add(cWalles[i].End);
//    }
//}

//CeilingDataSet.ceiling_wallesRow row0, row1;
//row0 = ceilingWalles[this.ceilingWalles.Count - 1];

//for (int i = 0; i < this.ceilingWalles.Count; i++)
//{
//    Wall wall = new Wall();
//    //row1 = dv[i].Row as CeilingDataSet.ceiling_wallesRow;
//    row1 = this.ceilingWalles[i];
//    wall.Id = row1.ID;
//    wall.Begin = new PointF(row0.endx, row0.endy);
//    wall.End = new PointF(row1.endx, row1.endy);
//    wall.Wallnum = row1.wallnum;
//    wall.Randian = row1.radian;
//    wall.PhysicsCoord(ceiling);
//    this.AddWall(wall);
//    row0 = row1;
//}

//public static int GetSZoneNum(CeilingDataSet.ceiling_wallesDataTable cwalles, 
//    int max)
//{
//    int num = 0;

//    for (int i = 0; i < cwalles.Count; i++)
//    {
//        if (cwalles[i].wallnum < max)
//            continue;
//        if (cwalles[i].wallnum < num)
//            num = cwalles[i].wallnum;
//    }

//    return num;
//}

//public void Save(CeilingDataSet.ceiling_wallesDataTable _ceilingWalles,
//    int _ceiling_id)
//{
//    this.PareperWalles(_ceilingWalles, _ceiling_id);
//    CeilingDataSet.ceiling_wallesRow cwrow = null;

//    for (int i = 0; i < _ceilingWalles.Count; i++)
//    {
//        cwrow = _ceilingWalles[i];
//        cwrow.BeginEdit();
//        cwrow.ceiling_id = _ceiling_id;
//        cwrow.EndEdit();
//    }

//    _ceilingWalles.AcceptChanges();
//}

//StrLine temp = new StrLine(
//    new PointF(walles[index].Begin.X + movedActual.X, walles[index].Begin.Y + movedActual.Y),
//    new PointF(walles[index].End.X + movedActual.X, walles[index].End.Y + movedActual.Y));

//PointF bp = new PointF(walles[index].Begin.X + movedActual.X,
//    walles[index].Begin.Y + movedActual.Y);
//PointF ep = new PointF(walles[index].End.X + movedActual.X,
//    walles[index].End.Y + movedActual.Y);
//StrLine temp = new StrLine(bp, ep);

//int prev = index - 1, next = index + 1;
//if (prev >= 0)
//    walles[index].Begin = temp.Intersect(walles[prev]);
//else
//    walles[index].Begin = temp.Begin;

//if (next < walles.Count)
//    walles[index].End = temp.Intersect(walles[next]);
//else
//    walles[index].End = temp.End;

//_wall.PhysicsCoord(ceiling);

//if (prev >= 0)
//{
//    walles[prev].End = _wall.Begin;
//    walles[prev].EndPaintPoint = _wall.BeginPaintPoint;
//}

//if (next < walles.Count)
//{
//    walles[next].Begin = _wall.End;
//    walles[next].BeginPaintPoint = _wall.EndPaintPoint;
//}

//this.RefrushData(ceiling);
//this.CalcLoca(ceiling);

//if (index > 0)
//{
//    MoveWall(walles[index - 1], new PointF(_wall.Vector.X * scale, 
//        _wall.Vector.Y * scale), ceiling);
//}
//else
//{
//    MoveWall(walles[index + 1], new PointF(_wall.Vector.X * scale, 
//        _wall.Vector.Y * scale), ceiling);
//}

//int move = (Math.Abs(_wall.Vector.X) > 0 ? _wall.Vector.X : _wall.Vector.Y) > 0 ? 1 : 0;

//if (Math.Abs(_wall.Vector.X) > 0)
//{
//    move = _wall.End.X > _wall.Begin.X ? 1 : 0;
//}
//else
//{
//    move = _wall.End.Y > _wall.Begin.Y ? 1 : 0;
//}

//scale = move > 0 ? -scale : scale;

//PointF movedActual = new PointF(_wall.Vector.X * scale, 
//    _wall.Vector.Y * scale);

//Point movedPixel = new Point((int)(_wall.PaintLine.Vector.X * scale),
//    (int)(_wall.PaintLine.Vector.Y * scale));

//if (move == 0)
//    index = index > 0 ? index - 1 : this.walles.Count - 1;

//PointF pt = new PointF(_wall.End.X + movedActual.X,
//    _wall.End.Y + movedActual.Y);
//ModifyWall(_wall, ceiling, _wall.Begin, pt);

//this._movePoint(index, movedActual);
//this._movePoint(index, movedPixel);
//this._CalcRemarkLoca(index);

//public void RefrushData(Ceiling ceiling)
//{
//    List<Wall> cWalles = ceiling.Walles;
//    dingdians.Clear();
//    paintDingdians.Clear();

//    paintDingdians.Add(walles[0].BeginPaintPoint);
//    dingdians.Add(walles[0].Begin);

//    for (int i = 0; i < walles.Count; i++)
//    {
//        paintDingdians.Add(walles[i].EndPaintPoint);
//        dingdians.Add(walles[i].End);
//    }

//    if (sidx < eidx)
//    {
//        for (int i = eidx; i > sidx; i--)
//        {
//            paintDingdians.Add(cWalles[i].BeginPaintPoint);
//            dingdians.Add(cWalles[i].Begin);
//        }
//    }
//    else
//    {
//        for (int i = eidx; i < sidx; i++)
//        {
//            paintDingdians.Add(cWalles[i].EndPaintPoint);
//            dingdians.Add(cWalles[i].End);
//        }
//    }

//    this.path = new System.Drawing.Drawing2D.GraphicsPath();
//    for (int i = 1; i < this.paintDingdians.Count; i++)
//        this.path.AddLine(paintDingdians[i - 1], paintDingdians[i]);

//    this.depth = ceiling.Depth;

//    rect.X = paintDingdians[0].X; rect.Y = paintDingdians[0].Y;
//    for (int i = 1; i < dingdians.Count; i++)
//    {
//        rect.X = Math.Min(rect.X, paintDingdians[i].X);
//        rect.Y = Math.Min(rect.Y, paintDingdians[i].Y);
//        rect.Width = Math.Max(rect.X + rect.Width, paintDingdians[i].X) - rect.X;
//        rect.Height = Math.Max(rect.Y + rect.Height, paintDingdians[i].Y) - rect.Y;
//    }
//}

//for (int i = 0; i < walles.Count; i++)
//{
//    paintDingdians.Add(walles[i].EndPaintPoint);
//    dingdians.Add(walles[i].End);
//}

//if (sidx < eidx)
//{
//    for (int i = eidx; i > sidx; i--)
//    {
//        paintDingdians.Add(cWalles[i].BeginPaintPoint);
//        dingdians.Add(cWalles[i].Begin);
//    }
//}
//else
//{
//    for (int i = eidx; i < sidx; i++)
//    {
//        paintDingdians.Add(cWalles[i].EndPaintPoint);
//        dingdians.Add(cWalles[i].End);
//    }
//}

//this.path = new System.Drawing.Drawing2D.GraphicsPath();
//for (int i = 1; i < this.paintDingdians.Count; i++)
//    this.path.AddLine(paintDingdians[i - 1], paintDingdians[i]);

//this.depth = ceiling.Depth;

//rect.X = paintDingdians[0].X; rect.Y = paintDingdians[0].Y;
//for (int i = 1; i < dingdians.Count; i++)
//{
//    rect.X = Math.Min(rect.X, paintDingdians[i].X);
//    rect.Y = Math.Min(rect.Y, paintDingdians[i].Y);
//    rect.Width = Math.Max(rect.X + rect.Width, paintDingdians[i].X) - rect.X;
//    rect.Height = Math.Max(rect.Y + rect.Height, paintDingdians[i].Y) - rect.Y;
//}

//this.closed = true;

//public bool Select(Point pt)
//{
//    return remark.DispRect.Contains(pt);
//}

//public void DrawRemark(Graphics graphics, Font font, Pen arrowPen)
//{
//    this.remark.Draw(graphics, font, arrowPen);
//}

//rect.X = paintDingdians[0].X; 
//rect.Y = paintDingdians[0].Y;
//for (int i = 1; i < dingdians.Count; i++)
//{
//    rect.X = Math.Min(rect.X, paintDingdians[i].X);
//    rect.Y = Math.Min(rect.Y, paintDingdians[i].Y);
//    rect.Width = Math.Max(rect.X + rect.Width, 
//        paintDingdians[i].X) - rect.X;
//    rect.Height = Math.Max(rect.Y + rect.Height, 
//        paintDingdians[i].Y) - rect.Y;
//}

//this.remark.CalcRect(this.remark.DefRemark);
//this.remark.CalcRect(Math.Abs(this.depth - ceiling.Depth) > 1 ? 16 : 0);

//string str = "夹高：" + this.depth.ToString() + "\n尺寸："
//    + (int)rect.Width + " X " + (int)rect.Height;
//string str = "尺寸：" + (int)rect.Width + " X " + (int)rect.Height
//    + "\n夹高：" + this.depth.ToString();
//this.remark.Draw(str, graphics, font, arrowPen);

//walls[i].DrawLength(this.ceiling.Clockwise, graphics,
//    arrowPen, lengthFont, strFormat);

//this.remark.Draw((int)rect.Width + " X " + (int)rect.Height
//    +" 夹高：" + this.depth.ToString(),
//    graphics, font, arrowPen);
//if (inters == 2)
//{
//}

//graphics.DrawLine(Pens.Black, this.remarkLineBegin, this.remarkLineEnd);
//graphics.DrawRectangle(Pens.Black,
//    this.remarkLineEnd.X - 1, this.remarkLineEnd.Y - 1, 3, 3);
//Point d1 = new Point(
//    remarkLineEnd.X == remarkLineBegin.X ? 0 : (remarkLineEnd.X > remarkLineBegin.X ? 3 : -3),
//    remarkLineEnd.Y == remarkLineBegin.Y ? 0 : (remarkLineEnd.Y > remarkLineBegin.Y ? 3 : -3));
//StrLine.DrawArrow(graphics, remarkLineBegin, d1, arrowPen);

//int x = this.remarkLoca.X, y = this.remarkLoca.Y;

//graphics.DrawString((uint)rect.Width + " X " + (uint)rect.Height,
//    font, Brushes.Red, x, y);
//y += 16;
//graphics.DrawString("夹高：" + this.depth.ToString(), font,
//    Brushes.Red, x, y);

////if (Math.Abs(this.depth - defalt_depth) > 1)
////{
////    graphics.DrawString("夹高：" + this.depth.ToString(), font,
////        Brushes.Red, x, y);
////    y += 20;
////}

//if (this.remark.Length > 0)
//{
//    y += 16;
//    graphics.DrawString(this.remark, font, Brushes.Red, x, y);
//}

//this.remarkLoca.X = this.remark.LineEnd.X - 35;
//this.remarkLoca.Y = this.remark.LineEnd.Y - 25;
//this.remarkLoca.X = this.remarkLineEnd.X - 35;
//this.remarkLoca.Y = this.remarkLineEnd.Y + 5;

//remark.LineBegin.X = rect.X + rect.Width / 2;
//remark.LineBegin.Y = rect.Y + rect.Height / 2;
//remark.LineEnd.X = remark.LineBegin.X;
//remark.PaintRect = new Rectangle(remark.Loca.X - 5, remark.Loca.Y - 5, 100, 20);

//if (Math.Abs(this.depth - ceiling.Depth) > 1)
//    remarkRect.Height += 20;
//if (this.remark.Length > 0)
//    remarkRect.Height += 25;

//rect.X = Math.Min(remarkLineBegin.X, remarkLineEnd.X) - 3;
//rect.Y = Math.Min(remarkLineBegin.Y, remarkLineEnd.Y) - 3;
//rect.Width = Math.Max(remarkLineBegin.X, remarkLineEnd.X) - rect.X + 6;
//rect.Height = Math.Max(remarkLineBegin.Y, remarkLineEnd.Y) - rect.Y + 6;

//remarkRect = Rectangle.Union(rect, remarkRect);
