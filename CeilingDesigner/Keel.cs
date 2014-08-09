using System;
using System.Collections.Generic;
using System.Drawing;

namespace CeilingDesigner
{
    public class StrLine
    {
        #region 属性列表

        protected PointF begin;

        public PointF Begin
        {
            get { return begin; }
            set
            {
                begin = value;
                //begin.X = (float)Math.Round(begin.X, 2);
                //begin.Y = (float)Math.Round(begin.Y, 2);
                this.InitLine();
            }
        }

        protected PointF end;

        public PointF End
        {
            get { return end; }
            set
            {
                end = value;
                this.InitLine();
            }
        }

        protected PointF center = new PointF(0, 0);

        public PointF Center
        {
            get { return center; }
        }

        protected double length;

        public System.UInt32 Length
        {
            get { return (System.UInt32)Math.Abs(Math.Round(length)); }
        }

        private PointF vector;

        public PointF Vector
        {
            get { return vector; }
        }

        private double alpha;

        /// <summary>
        /// 直线与X轴的夹角
        /// </summary>
        public double Alpha
        {
            get { return alpha; }
        }

        //private double intercept;

        //public double Intercept
        //{
        //    get { return intercept; }
        //}

        public float A
        {
            get { return -this.vector.Y; }
        }

        public float B
        {
            get { return this.vector.X; }
        }

        private float c;

        public float C
        {
            get { return c; }
        }

        static public float MaxValue = 1e20f;

        #endregion

        public StrLine()
        {
            this.begin = new PointF(0, 0);
            this.end = new PointF(0, 0);
            this.InitLine();
        }

        public StrLine(PointF begin, PointF end)
        {
            this.begin = begin;
            this.end = end;
            this.InitLine();
        }

        public void InitLine()
        {
            this.vector.X = end.X - begin.X;
            this.vector.Y = end.Y - begin.Y;
            this.center.X = (begin.X + end.X) / 2;
            this.center.Y = (begin.Y + end.Y) / 2;

            this.length = Math.Sqrt(this.vector.X * this.vector.X
                + this.vector.Y * this.vector.Y);
            this.c = this.begin.X * this.end.Y - this.end.X * this.begin.Y;

            if (Math.Abs(vector.X) > 0 || Math.Abs(vector.Y) > 0)
                this.alpha = Math.Atan2(this.vector.Y, this.vector.X);
            else
                this.alpha = 0;
        }

        public static double Distance2(PointF p1, PointF p2)
        {
            PointF vector = new PointF(p2.X - p1.X, p2.Y - p1.Y);
            return vector.X * vector.X + vector.Y * vector.Y;
        }

        public static double Distance(PointF p1, PointF p2)
        {
            return Math.Sqrt(Distance2(p1, p2));
            //PointF vector = new PointF(p2.X - p1.X, p2.Y - p1.Y);
            //return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }

        /// <summary>
        /// 点到指定直线的距离
        /// </summary>
        /// <param name="p">点的坐标</param>
        /// <returns>距离值</returns>
        public double Distance(PointF pt)
        {
            return Math.Abs(this.A * pt.X + this.B * pt.Y + this.c) 
                / this.length;
        }

        /// <summary>
        /// 返回两条直线顺时针的夹角
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public double ClockWise(StrLine line)
        {
            double _alph = line.alpha - this.alpha;
            _alph = _alph > Math.PI ? _alph - 2 * Math.PI : _alph;
            _alph = _alph < -Math.PI ? _alph + 2 * Math.PI : _alph;
            return _alph;
        }

        public bool Contains(PointF p)
        {
            return this.Contains(p, false);
        }

        public bool Contains(PointF p, bool real)
        {
            //double delta = 0.01;
            double delta = 5;
            if (this.Distance(p) > delta)
                return false;
            double l1 = Distance(this.begin, p), l2 = Distance(this.end, p);

            // 当交点在该段的顶点时，不属于在线段的内部
            if (real && (l1 < delta || l2 < delta))
                return false;
            if (l1 + l2 > this.length + delta)
                return false;
            return true;
        }

        public bool Contains(PointF pt, double delta)
        {
            if (this.Distance(pt) > delta)
                return false;
            double l1 = Distance(this.begin, pt), 
                l2 = Distance(this.end, pt);

            if (l1 < delta || l2 < delta)
                return true;
            if (l1 + l2 > delta + this.length)
                return false;
            return true;
        }

        /// <summary>
        /// 点是否在某条线上
        /// </summary>
        /// <param name="line">基准线</param>
        /// <param name="pt">当前点</param>
        /// <returns>是否在线上，是返回true</returns>
        public static bool IsInLine(StrLine line, Point pt)
        {
            if (StrLine.Distance(line.Begin, pt) < line.Length
                && StrLine.Distance(line.End, pt) < line.Length
                && line.Distance(pt) < 5)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 两条直线是否平行
        /// </summary>
        /// <param name="line">另一条直线</param>
        /// <returns>平行返回true，否则返回false</returns>
        public bool IsParallel(StrLine line)
        {
            double delta = Math.Abs(this.alpha - line.alpha);
            if (delta< 0.02 || Math.Abs(delta - Math.PI) < 0.02 )
                return true;
            else
                return false;
            //if (Math.Abs(delta) < 0.02 || Math.Abs(delta - Math.PI) < 0.02 
            //    || Math.Abs(delta + Math.PI) < 0.02)
            //    return true;
            //else
            //    return false;
        }

        /// <summary>
        /// 两条直线的交点
        /// </summary>
        /// <param name="seg">另一条直线</param>
        /// <returns>交点，平行时返回无穷大的点</returns>
        public PointF Intersect(StrLine seg)
        {
            PointF inter = new PointF(StrLine.MaxValue, StrLine.MaxValue);

            if (Math.Abs(this.alpha - seg.alpha) > 0.005)
            {
                inter.X = (float)Math.Round((this.B * seg.c - seg.B * this.c)
                    / (this.A * seg.B - seg.A * this.B));
                inter.Y = (float)Math.Round((this.A * seg.c - seg.A * this.c)
                    / (seg.A * this.B - this.A * seg.B));
            }

            return inter;
        }

        /// <summary>
        /// 直线上距离起点的距离为r的点
        /// </summary>
        /// <param name="r">距离起点的距离</param>
        /// <returns>返回所求直线上的点</returns>
        public PointF GetPoint(double r)
        {
            PointF p = this.begin;

            p.X += (float)(r * Math.Cos(this.alpha));
            p.Y += (float)(r * Math.Sin(this.alpha));

            return p;
        }

        /// <summary>
        /// 绕某个中心点旋转90度
        /// </summary>
        /// <param name="p">旋转的点</param>
        /// <param name="c">中心点</param>
        /// <returns>旋转结果点</returns>
        public static PointF Trans(PointF p, PointF c)
        {
            return new PointF(c.X + c.Y - p.Y, c.Y + p.X - c.X);
        }

        /// <summary>
        /// 绕某个中心点旋转90度
        /// </summary>
        /// <param name="p">旋转的点</param>
        /// <param name="c">中心点</param>
        /// <returns>旋转结果点</returns>
        public static Point Trans(Point p, Point c)
        {
            return new Point(c.X + c.Y - p.Y, c.Y + p.X - c.X);
        }

        /// <summary>
        /// 绕某个中心点逆时针旋转90度
        /// </summary>
        /// <param name="p">旋转的点</param>
        /// <param name="c">中心点</param>
        /// <returns>旋转结果点</returns>
        public static PointF UnTrans(PointF p, PointF c)
        {
            return new PointF(c.X + p.Y - c.Y, c.Y + c.X - p.X);
        }

        /// <summary>
        /// 绕某个中心点逆时针旋转90度
        /// </summary>
        /// <param name="p">旋转的点</param>
        /// <param name="c">中心点</param>
        /// <returns>旋转结果点</returns>
        public static Point UnTrans(Point p, Point c)
        {
            return new Point(c.X + p.Y - c.Y, c.Y + c.X - p.X);
        }

        public static void DrawArrow(Graphics graphics, Point p, Point delta,
            Pen arrowPen)
        {
            double a = 0.5, b = 0.866, c;// = (delta.X + delta.Y) * 3;

            if (Math.Abs(delta.X) < 0.1 || Math.Abs(delta.Y) < 0.1)
            {
                graphics.DrawLine(arrowPen, p.X, p.Y,
                    p.X + delta.Y + delta.X * 3,
                    p.Y + delta.X + delta.Y * 3);
                graphics.DrawLine(arrowPen, p.X, p.Y,
                    p.X - delta.Y + delta.X * 3,
                    p.Y - delta.X + delta.Y * 3);
            }
            else if (Math.Abs(delta.X - delta.Y) < 0.1)
            {
                c = (delta.X + delta.Y) * 3;
                graphics.DrawLine(arrowPen, p.X, p.Y, p.X + (int)(b * c),
                    p.Y + (int)(a * c));
                graphics.DrawLine(arrowPen, p.X, p.Y, p.X + (int)(a * c),
                    p.Y + (int)(b * c));
            }
            else
            {
                c = (delta.X - delta.Y) * 3;
                graphics.DrawLine(arrowPen, p.X, p.Y, p.X + (int)(b * c),
                    p.Y - (int)(a * c));
                graphics.DrawLine(arrowPen, p.X, p.Y, p.X + (int)(a * c),
                    p.Y - (int)(b * c));
            }
        }
    }

    public class Keel : StrLine
    {
        #region 属性表

        protected StrLine dispLine = new StrLine();

        public StrLine PaintLine
        {
            get { return dispLine; }
        }

        public Point CenterPaintPoint
        {
            get { return Point.Round(dispLine.Center); }
        }

        public Point BeginPaintPoint
        {
            get { return Point.Round(dispLine.Begin); }
            set
            {
                dispLine.Begin = value;
                InitDispParam();
                this.beginAnchor.X = (int)dispLine.Begin.X - 4;
                this.beginAnchor.Y = (int)dispLine.Begin.Y - 4;
            }
        }

        public Point EndPaintPoint
        {
            get { return Point.Round(dispLine.End); }
            set
            {
                dispLine.End = value;
                InitDispParam();
                this.endAnchor.X = (int)dispLine.End.X - 4;
                this.endAnchor.Y = (int)dispLine.End.Y - 4;
            }
        }

        protected Rectangle paintRect = new Rectangle(0, 0, 9, 9);

        public Rectangle PaintRect
        {
            get { return paintRect; }
        }

        protected Rectangle centerAnchor = new Rectangle(0, 0, 9, 9);

        public Rectangle CenterAnchor
        {
            get { return centerAnchor; }
        }

        protected Rectangle beginAnchor = new Rectangle(0, 0, 9, 9);

        public Rectangle BeginAnchor
        {
            get { return beginAnchor; }
        }

        protected Rectangle endAnchor = new Rectangle(0, 0, 9, 9);

        public Rectangle EndAnchor
        {
            get { return endAnchor; }
        }

        private int id = 0;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        protected Remark remark = new Remark();

        public Remark Remark
        {
            get { return remark; }
        }

        public string RemarkStr
        {
            get { return remark.CustomRemark; }
            set { remark.CustomRemark = value; }
        }

        protected uint depth = 300;

        /// <summary>
        /// 吊顶夹高
        /// </summary>
        public System.UInt32 Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        #endregion

        public void PhysicsCoord(Ceiling ceiling)
        {
            this.BeginPaintPoint = ceiling.GetDispCoord(this.begin);
            this.EndPaintPoint = ceiling.GetDispCoord(this.end);
        }

        public void LogicCoord(Ceiling ceiling)
        {
            this.Begin = ceiling.GetLogicCoord(this.BeginPaintPoint);
            this.End = ceiling.GetLogicCoord(this.EndPaintPoint);
        }

        private void InitDispParam()
        {
            this.paintRect.X = (int)Math.Min(dispLine.Begin.X, dispLine.End.X) - 5;
            this.paintRect.Y = (int)Math.Min(dispLine.Begin.Y, dispLine.End.Y) - 5;
            this.paintRect.Width = (int)Math.Max(dispLine.Begin.X, dispLine.End.X)
                - this.paintRect.X + 10;
            this.paintRect.Height = (int)Math.Max(dispLine.Begin.Y, dispLine.End.Y)
                - this.paintRect.Y + 10;

            this.centerAnchor.X = (int)dispLine.Center.X - 4;
            this.centerAnchor.Y = (int)dispLine.Center.Y - 4;
        }

        private PointF RemarkEndP(Ceiling ceiling)
        {
            PointF pt, inte = new PointF(0, 0);
            double d2, max = StrLine.MaxValue;

            for (int i = 0; i < ceiling.Walles.Count; i++)
            {
                StrLine cwpl = ceiling.Walles[i];
                if (this.IsParallel(cwpl))
                    continue;
                pt = this.Intersect(cwpl);
                if (!cwpl.Contains(pt, true))
                    continue;

                d2 = StrLine.Distance2(pt, this.center);
                if (max > d2)
                {
                    max = d2;
                    inte = pt;
                }
            }

            return inte;
        }

        protected void _CalcRemarkLoca(Point pt, PointF intep, Ceiling ceiling)
        {
            if (Math.Abs(this.Vector.X) > Math.Abs(this.Vector.Y))
            {
                if (intep.X > ceiling.Left + ceiling.Width / 2)
                {
                    this.remark.LineBegin = this.Vector.X < 0 ?
                        this.BeginPaintPoint : this.EndPaintPoint;
                    this.remark.LineEnd = new Point(pt.X + 40, pt.Y);
                    this.remark.Loca = new Point(this.remark.LineEnd.X + 5,
                        this.remark.LineEnd.Y - 6);
                }
                else
                {
                    this.remark.LineBegin = this.Vector.X > 0 ?
                        this.BeginPaintPoint : this.EndPaintPoint;
                    this.remark.LineEnd = new Point(pt.X - 40, pt.Y);
                    this.remark.Loca = new Point(this.remark.LineEnd.X - 70,
                        this.remark.LineEnd.Y - 6);
                }
            }
            else
            {
                if (intep.Y > ceiling.Top + ceiling.Height / 2)
                {
                    this.remark.LineBegin = this.Vector.Y < 0 ?
                        this.BeginPaintPoint : this.EndPaintPoint;
                    this.remark.LineEnd = new Point(pt.X, pt.Y + 40);
                    this.remark.Loca = new Point(this.remark.LineEnd.X - 35,
                        this.remark.LineEnd.Y + 5);
                }
                else
                {
                    this.remark.LineBegin = this.Vector.Y > 0 ?
                        this.BeginPaintPoint : this.EndPaintPoint;
                    this.remark.LineEnd = new Point(pt.X, pt.Y - 40);
                    this.remark.Loca = new Point(this.remark.LineEnd.X - 35,
                        this.remark.LineEnd.Y - 15);
                }
            }
        }

        public void CalcRemarkLoca(Ceiling ceiling)
        {
            PointF intep = RemarkEndP(ceiling);
            Point pt = ceiling.GetDispCoord(intep);

            this._CalcRemarkLoca(pt, intep, ceiling);
            this.remark.DefRemark = Math.Abs(this.depth - ceiling.Depth) > 1 ? 
                "夹高：" + this.depth : "";
            this.remark.CalcRect();
        }

        private double distance(PointF p1, PointF p2)
        {
            double dx = p1.X - p2.X, dy = p1.Y - p2.Y;
            return dx * dx + dy * dy;
        }

        public void DrawRemark(Graphics graphics, Font font,
            float defalt_depth, Pen arrowPen)
        {
            this.remark.Draw(graphics, font, arrowPen);
        }

        public string AStr
        {
            get { return "A(" + Math.Round(this.begin.X).ToString() + "," 
                + Math.Round(this.begin.Y).ToString() + ")"; }
        }

        public string BStr
        {
            get { return "B(" + Math.Round(this.end.X).ToString() + "," 
                + Math.Round(this.end.Y).ToString() + ")"; }
        }

        public void DrawSelectedFlag(Graphics graphics, Pen pen, 
            Font lengthFont)
        {
            graphics.DrawRectangle(pen, this.BeginAnchor);
            graphics.DrawRectangle(pen, this.CenterAnchor);
            graphics.DrawRectangle(pen, this.EndAnchor);

            Rectangle rect = new Rectangle(this.BeginPaintPoint, 
                new Size(1, 1));
            graphics.DrawRectangle(pen, rect);
            rect.Location = this.EndPaintPoint;
            graphics.DrawRectangle(pen, rect);
            rect.Location = this.CenterPaintPoint;
            graphics.DrawRectangle(pen, rect);

            rect.Width = 100; rect.Height = 20;
            rect.X = BeginPaintPoint.X + 10; rect.Y = BeginPaintPoint.Y - 15;
            graphics.DrawString(AStr, lengthFont, Brushes.Red, rect);
            rect.X = EndPaintPoint.X + 10; rect.Y = EndPaintPoint.Y - 15;
            graphics.DrawString(BStr, lengthFont, Brushes.Red, rect);
        }

        public new static void DrawArrow(Graphics graphics, Point p, Point delta,
            Pen arrowPen)
        {
            StrLine.DrawArrow(graphics, p, delta, arrowPen);
            delta.X *= 5; delta.Y *= 5;
            graphics.DrawLine(arrowPen, p.X + delta.Y, p.Y - delta.X,
                p.X - delta.Y, p.Y + delta.X);
        }

        public void DrawLength(int clockwise, Graphics graphics, Pen arrowPen, 
            Font lengthFont, StringFormat strFormat)
        {
            Point p1 = this.BeginPaintPoint, p2 = this.EndPaintPoint;
            //Point d1 = new Point(p2.X == p1.X ? 0 : (p2.X > p1.X ? 2 : -2),
            //    p2.Y == p1.Y ? 0 : (p2.Y > p1.Y ? 2 : -2));
            double dleng = this.PaintLine.Length;
            string lstr = Math.Round(length).ToString();

            Point d1 = new Point(p2.X - p1.X, p2.Y - p1.Y);
            d1.X = (int)Math.Round(d1.X * 2 / dleng);
            d1.Y = (int)Math.Round(d1.Y * 2 / dleng);
            //d1.X = (int)(d1.X * 2 / dleng);
            //d1.Y = (int)(d1.Y * 2 / dleng);

            if (clockwise >= 0)
            {
                p1.X += d1.Y * 8; p1.Y -= d1.X * 8;
                p2.X += d1.Y * 8; p2.Y -= d1.X * 8;
            }
            else
            {
                p1.X -= d1.Y * 8; p1.Y += d1.X * 8;
                p2.X -= d1.Y * 8; p2.Y += d1.X * 8;
            }

            Keel.DrawArrow(graphics, p1, d1, arrowPen);
            Keel.DrawArrow(graphics, p2, new Point(-d1.X, -d1.Y), arrowPen);
            graphics.DrawLine(arrowPen, p1, p2);

            if (Math.Abs(this.Alpha - Math.PI / 2) < 0.02 
                || Math.Abs(this.Alpha + Math.PI / 2) < 0.02)
            {
                SizeF size = graphics.MeasureString(lstr, lengthFont, 20, 
                    strFormat);
                RectangleF rect = new RectangleF((p1.X + p2.X - size.Width) / 2,
                    (p1.Y + p2.Y - size.Height) / 2, size.Width, size.Height);

                if (clockwise >= 0)
                {
                    rect.X += d1.Y * 4;
                    rect.Y -= d1.X * 4;
                }
                else
                {
                    rect.X -= d1.Y * 4;
                    rect.Y += d1.X * 4;
                }

                graphics.DrawString(lstr, lengthFont, Brushes.Black, rect, 
                    strFormat);
            }
            else
            {
                SizeF size = graphics.MeasureString(lstr, lengthFont);
                RectangleF rect = new RectangleF((p1.X + p2.X - size.Width) / 2,
                    (p1.Y + p2.Y - size.Height) / 2, size.Width, size.Height);

                if (clockwise >= 0)
                {
                    rect.X += d1.Y * 4;
                    rect.Y -= d1.X * 4;
                }
                else
                {
                    rect.X -= d1.Y * 4;
                    rect.Y += d1.X * 4;
                }

                graphics.DrawString(lstr, lengthFont, Brushes.Black, rect);
            }
        }

        public void Draw(Graphics graphics, Pen pen)
        {
            graphics.DrawLine(pen, this.BeginPaintPoint, this.EndPaintPoint);
        }

        public void Next(Keel _keel)
        {
            this.Begin = _keel.End;
            this.BeginPaintPoint = _keel.EndPaintPoint;
            this.End = _keel.End;
            this.EndPaintPoint = _keel.EndPaintPoint;
        }

        public void InitPoint(Point p)
        {
            this.Begin = new PointF(p.X * 10, p.Y * 10);
            this.BeginPaintPoint = p;
            this.End = new PointF(p.X * 10, p.Y * 10);
            this.EndPaintPoint = p;
        }

        public void ChangeLenth(float length, Ceiling ceiling)
        {
            float s = (float)(length / this.length);
            this.End = new PointF(this.Begin.X + s * this.Vector.X, 
                this.Begin.Y + s * this.Vector.Y);
            this.EndPaintPoint = new Point(
                this.BeginPaintPoint.X + (int)(s * this.dispLine.Vector.X),
                this.BeginPaintPoint.Y + (int)(s * this.dispLine.Vector.Y));
            this.CalcRemarkLoca(ceiling);
        }

        public void SetLength(float length)
        {
            // 竖直向下
            if (this.Alpha > Math.PI / 3 && this.Alpha < Math.PI * 2 / 3)
            {
                this.End = new PointF(this.Begin.X, this.Begin.Y + length);
                this.EndPaintPoint = new Point(this.BeginPaintPoint.X,
                    this.BeginPaintPoint.Y + (int)(length / 10));
            }
            // 竖直向上
            else if (this.Alpha < -Math.PI / 3 && this.Alpha > -Math.PI * 2 / 3)
            {
                this.End = new PointF(this.Begin.X, this.Begin.Y - length);
                this.EndPaintPoint = new Point(this.BeginPaintPoint.X, 
                    this.BeginPaintPoint.Y - (int)(length / 10));
            }
            // 水平向左
            else if (Math.Abs(this.Alpha) < Math.PI / 6)
            {
                this.End = new PointF(this.Begin.X + length, this.Begin.Y);
                this.EndPaintPoint = new Point(
                    this.BeginPaintPoint.X + (int)(length / 10),
                    this.BeginPaintPoint.Y);
            }
            // 水平向右
            else if (Math.Abs(this.Alpha) > Math.PI * 5 / 6)
            {
                this.End = new PointF(this.Begin.X - length, this.Begin.Y);
                this.EndPaintPoint = new Point(
                    this.BeginPaintPoint.X - (int)(length / 10),
                    this.BeginPaintPoint.Y);
            }
            // 斜向左下
            else if (this.Alpha > Math.PI / 6 && this.Alpha < Math.PI / 3)
            {
                float _lth = (float)(length * Math.Sqrt(2) / 2);
                this.End = new PointF(this.Begin.X + _lth, this.Begin.Y + _lth);
                this.EndPaintPoint = new Point(
                    this.BeginPaintPoint.X + (int)(_lth / 10),
                    this.BeginPaintPoint.Y + (int)(_lth / 10));
            }
            // 斜向左上
            else if (this.Alpha < -Math.PI / 6 && this.Alpha > -Math.PI / 3)
            {
                float _lth = (float)(length * Math.Sqrt(2) / 2);
                this.End = new PointF(this.Begin.X + _lth, this.Begin.Y - _lth);
                this.EndPaintPoint = new Point(
                    this.BeginPaintPoint.X + (int)(_lth / 10),
                    this.BeginPaintPoint.Y - (int)(_lth / 10));
            }
            // 斜向右下
            else if (this.Alpha < Math.PI * 5 / 6 && this.Alpha > Math.PI * 2 / 3)
            {
                float _lth = (float)(length * Math.Sqrt(2) / 2);
                this.End = new PointF(this.Begin.X - _lth, this.Begin.Y + _lth);
                this.EndPaintPoint = new Point(
                    this.BeginPaintPoint.X - (int)(_lth / 10),
                    this.BeginPaintPoint.Y + (int)(_lth / 10));
            }
            // 斜向右上
            else if (this.Alpha > -Math.PI * 5 / 6 && this.Alpha < -Math.PI * 2 / 3)
            {
                float _lth = (float)(length * Math.Sqrt(2) / 2);
                this.End = new PointF(this.Begin.X - _lth, this.Begin.Y - _lth);
                this.EndPaintPoint = new Point(
                    this.BeginPaintPoint.X - (int)(_lth / 10),
                    this.BeginPaintPoint.Y - (int)(_lth / 10));
            }
        }

        public void Move(PointF delta, Ceiling ceiling)
        {
            this.Begin = new PointF(this.Begin.X + delta.X,
                this.Begin.Y + delta.Y);
            this.End = new PointF(this.End.X + delta.X, this.End.Y + delta.Y);
            this.PhysicsCoord(ceiling);
            this.remark.Move(ceiling.GetDispCoord(delta));
            //_moveRemark(ceiling.GetDispCoord(delta));
        }

        public void MoveBegin(PointF delta, Ceiling ceiling)
        {
            this.Begin = new PointF(this.begin.X + delta.X, 
                this.begin.Y + delta.Y);
            this.PhysicsCoord(ceiling);
            this.CalcRemarkLoca(ceiling);
        }

        public void MoveEnd(PointF delta, Ceiling ceiling)
        {
            this.End = new PointF(this.End.X + delta.X, this.End.Y + delta.Y);
            this.PhysicsCoord(ceiling);
            this.CalcRemarkLoca(ceiling);
        }

        // 先去掉龙骨和墙壁重叠的部分
        public void EraseCoincide(Keel seg, Keel tempSeg)
        {
            PointF p0 = seg.Begin, p1;
            bool b0 = tempSeg.Contains(p0), b1;

            // 去掉开头的重合部分，如果有
            for (double r = 10; r < seg.Length && b0; r += 10)
            {
                p1 = seg.GetPoint(r);
                b1 = tempSeg.Contains(p1);

                if (!b1) break;
                b0 = b1;
                p0 = p1;
            }
            this.Begin = p0;

            // 去掉结尾的重合部分，如果有
            p0 = seg.End; b0 = tempSeg.Contains(p0);
            for (double r = seg.Length - 10; r > 0 && b0; r -= 10)
            {
                p1 = seg.GetPoint(r);
                b1 = tempSeg.Contains(p1);

                if (!b1) break;
                b0 = b1;
                p0 = p1;
            }
            this.End = p0;

            // 去掉中间的重合部分，如果有
            // 暂不处理
            //return _keel;
        }

        public void Cut(Ceiling ceiling)
        {
            List<Wall> walles = ceiling.Walles;

            // 先去掉龙骨和墙壁重叠的部分
            for (int i = 0; i < walles.Count; i++)
            {
                Keel tempSeg = walles[i];
                if (Math.Abs(this.Alpha - tempSeg.Alpha) > float.Epsilon
                    || this.Distance(tempSeg.Begin) > 1)
                    continue;
                this.EraseCoincide(this, tempSeg);
            }

            PointF r, r1 = this.Begin, r2 = this.End;
            for (int i = 0; i < walles.Count; i++)
            {
                if (this.IsParallel(walles[i]))
                    continue;
                r = this.Intersect(walles[i]);

                if (walles[i].Contains(r, true) && this.Contains(r, true))
                {
                    if (distance(r, this.Begin) < distance(r, this.End))
                        r1 = r;
                    else
                        r2 = r;
                }
            }

            this.Begin = r1; this.End = r2;
            this.PhysicsCoord(ceiling);
        }

        public void Adjust(Ceiling ceiling)
        {
            double d2, td2; int b = 0, e = 0;
            Keel tempSeg;
            List<PointF> inters = new List<PointF>();

            // 先去掉龙骨和墙壁重叠的部分
            for (int i = 0; i < ceiling.Length; i++)
            {
                tempSeg = ceiling.Walles[i];
                if (!IsParallel(tempSeg) || this.Distance(tempSeg.Begin) > 1)
                    continue;
                this.EraseCoincide(this, tempSeg);
            }

            // 去掉处在房屋外面的部分，
            // 只处理两边端点在屋外的部分，不处理龙骨中间部分处在屋外的情况
            for (int i = 0; i < ceiling.Length; i++)
            {
                tempSeg = ceiling.Walles[i];
                if (tempSeg.Contains(this.Begin))
                {
                    b++;
                }
                else if (tempSeg.Contains(this.End))
                {
                    e++;
                }
                else
                {
                    PointF temp = this.Intersect(tempSeg);
                    if (this.Contains(temp, true) && tempSeg.Contains(temp, true))
                        inters.Add(temp);
                }
            }

            if (b == 0)
            {
                d2 = this.Length * this.Length;
                PointF bp = this.End;
                for (int i = inters.Count - 1; i >= 0; i--)
                {
                    td2 = StrLine.Distance2(this.Begin, inters[i]);
                    if (td2 < d2)
                    {
                        bp = inters[i];
                        d2 = td2;
                    }
                }
                this.Begin = bp;
            }

            if (e == 0)
            {
                d2 = this.Length * this.Length;
                PointF ep = this.Begin;
                for (int i = 0; i < inters.Count; i++)
                {
                    td2 = StrLine.Distance2(this.End, inters[i]);
                    if (td2 < d2)
                    {
                        ep = inters[i];
                        d2 = td2;
                    }
                }
                this.End = ep;
            }

            this.PhysicsCoord(ceiling);
        }

        public void Fill(Ceiling ceiling, Point p, KeelOrientation ori)
        {
            if (ori == KeelOrientation.Horizontal)
            {
                this.BeginPaintPoint = new Point(ceiling.DrawingRect.Left, p.Y);
                this.EndPaintPoint = new Point(ceiling.DrawingRect.Right, p.Y);
            }
            else
            {
                this.BeginPaintPoint = new Point(p.X, ceiling.DrawingRect.Top);
                this.EndPaintPoint = new Point(p.X, ceiling.DrawingRect.Bottom);
            }

            this.LogicCoord(ceiling);
            this.Adjust(ceiling);
        }

        public void Fill(Ceiling ceiling)
        {
            if (this.begin.X != this.end.X)
            {
                this.begin.X = this.begin.X < this.end.X ? ceiling.Left : ceiling.Right;
                this.end.X = this.begin.X > this.end.X ? ceiling.Left : ceiling.Right;
            }
            if (this.begin.Y != this.end.Y)
            {
                this.begin.Y = this.begin.Y < this.end.Y ? ceiling.Top : ceiling.Bottom;
                this.end.Y = this.begin.Y > this.end.Y ? ceiling.Top : ceiling.Bottom;
            }
            this.Adjust(ceiling);
        }

        public void Move(PointF delta, int moving, Ceiling ceiling)
        {
            if (moving == 1)
            {
                this.Begin = new PointF(this.Begin.X + delta.X,
                    this.Begin.Y + delta.Y);
            }
            else if (moving == 2)
            {
                this.Begin = new PointF(this.Begin.X + delta.X,
                    this.Begin.Y + delta.Y);
                this.End = new PointF(this.End.X + delta.X, this.End.Y + delta.Y);
            }
            else if (moving == 3)
            {
                this.End = new PointF(this.End.X + delta.X, this.End.Y + delta.Y);
            }

            //this.LogicCoord(ceiling);
            this.PhysicsCoord(ceiling);
            this.CalcRemarkLoca(ceiling);
        }

        public KeelOrientation GetOri()
        {
            if (Math.Abs(this.BeginPaintPoint.X - this.EndPaintPoint.X)
                > Math.Abs(this.BeginPaintPoint.Y - this.EndPaintPoint.Y))
                return KeelOrientation.Horizontal;
            else
                return KeelOrientation.Vertical;
        }

        public void Trans(int angle, PointF ca, Ceiling ceiling)
        {
            if (angle == 90)
            {
                this.Begin = StrLine.Trans(this.Begin, ca);
                this.End = StrLine.Trans(this.End, ca);
            }
            else if (angle == -90)
            {
                this.Begin = StrLine.UnTrans(this.Begin, ca);
                this.End = StrLine.UnTrans(this.End, ca);
            }

            this.PhysicsCoord(ceiling);
            this.CalcRemarkLoca(ceiling);
        }

        /// <summary>
        /// 该方法只改变边的显示位置，不改变逻辑位置，
        /// 只应用在整个图形的平移操作中
        /// </summary>
        /// <param name="delta">平移的偏移量</param>
        public void Translate(Point delta)
        {
            this.dispLine.Begin = new PointF(this.dispLine.Begin.X + delta.X,
                this.dispLine.Begin.Y + delta.Y);
            this.dispLine.End = new PointF(this.dispLine.End.X + delta.X,
                this.dispLine.End.Y + delta.Y);

            this.paintRect.X += delta.X;
            this.paintRect.Y += delta.Y;

            this.beginAnchor.X += delta.X;
            this.beginAnchor.Y += delta.Y;
            this.centerAnchor.X += delta.X;
            this.centerAnchor.Y += delta.Y;
            this.endAnchor.X += delta.X;
            this.endAnchor.Y += delta.Y;

            this.remark.Move(delta);
            this.remark.CalcRect();
        }

        public Keel clone()
        {
            Keel keel = new Keel();
            keel.End = this.end;
            keel.Begin = this.begin;
            keel.BeginPaintPoint = this.BeginPaintPoint;
            keel.EndPaintPoint = this.EndPaintPoint;
            return keel;
        }
    }

    public class Wall : Keel
    {
        private int editAdjust = 0;

        public int EditAdjust
        {
            get { return editAdjust; }
            set { editAdjust = value; }
        }

        private int wallnum;

        public int Wallnum
        {
            get { return wallnum; }
            set { wallnum = value; }
        }

        private float radian;

        public float Randian
        {
            get { return radian; }
            set { radian = value; }
        }

        //public Wall() { }

        public void DrawRemark(Graphics graphics, Font font, Pen arrowPen)
        {
            if (this.RemarkStr.Length < 1)
                return;
            this.Remark.Draw(graphics, font, arrowPen);
        }

        public new void CalcRemarkLoca(Ceiling ceiling)
        {
            Point pt;

            // 水平边
            if (Math.Abs(this.Vector.X) > Math.Abs(this.Vector.Y))
            {
                if (this.center.X + 1 >= ceiling.Left + ceiling.Width / 2)
                {
                    pt = this.Vector.X > 0 ? 
                        this.EndPaintPoint : this.BeginPaintPoint;
                }
                else
                {
                    pt = this.Vector.X < 0 ?
                        this.EndPaintPoint : this.BeginPaintPoint;
                }
            }
            else // 竖直边
            {
                if (this.center.Y - 1 > ceiling.Top + ceiling.Height / 2)
                {
                    pt = this.Vector.Y < 0 ?
                        this.BeginPaintPoint : this.EndPaintPoint;
                }
                else
                {
                    pt = this.Vector.Y > 0 ?
                        this.BeginPaintPoint : this.EndPaintPoint;
                }
            }

            _CalcRemarkLoca(pt, new PointF(this.center.X + 1, this.center.Y - 1), 
                ceiling);
            this.remark.DefRemark = "";
            this.remark.CalcRect();
        }

        public new void Draw(Graphics graphics, Pen pen)
        {
            if (this.radian > this.Length * 0.01)
                return;
            Point bp = this.BeginPaintPoint, ep = this.EndPaintPoint;

            if (Math.Abs(bp.X - ep.X) > Math.Abs(bp.Y - ep.Y) * 10)
            {
                if (bp.X > ep.X)
                {
                    bp.X += 2;
                    ep.X -= 2;
                }
                else
                {
                    bp.X -= 2;
                    ep.X += 2;
                }
            }
            else
            {
                if (bp.Y > ep.Y)
                {
                    bp.Y += 2;
                    ep.Y -= 2;
                }
                else
                {
                    bp.Y -= 2;
                    ep.Y += 2;
                }
            }

            graphics.DrawLine(pen, bp, ep);
        }

        public Wall Clone()
        {
            Wall wall = new Wall();

            wall.End = this.end;
            wall.Begin = this.begin;
            wall.Randian = this.radian;
            wall.wallnum = this.wallnum;
            
            return wall;
        }
    }
}
