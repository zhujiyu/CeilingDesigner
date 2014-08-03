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

//public void Move(PointF delta, int moving, Ceiling ceiling)
//{
//    if (moving == 1)
//    {
//        this.BeginPaintPoint =
//            Point.Round(new PointF(this.BeginPaintPoint.X + delta.X,
//                this.BeginPaintPoint.Y + delta.Y));
//    }
//    else if (moving == 2)
//    {
//        this.BeginPaintPoint =
//            Point.Round(new PointF(this.BeginPaintPoint.X + delta.X,
//            this.BeginPaintPoint.Y + delta.Y));
//        this.EndPaintPoint =
//            Point.Round(new PointF(this.EndPaintPoint.X + delta.X,
//            this.EndPaintPoint.Y + delta.Y));
//    }
//    else if (moving == 3)
//    {
//        this.EndPaintPoint =
//            Point.Round(new PointF(this.EndPaintPoint.X + delta.X,
//                this.EndPaintPoint.Y + delta.Y));
//    }

//    this.LogicCoord(ceiling);
//    this.CalcRemarkLoca(ceiling);
//}

//public void Trans(int angle, Ceiling ceiling)
//{
//    //Point cd = new Point(
//    //    ceiling.DrawingRect.Left + ceiling.DrawingRect.Width / 2,
//    //    ceiling.DrawingRect.Top + ceiling.DrawingRect.Height / 2);
//    //PointF ca = new PointF(ceiling.Left + ceiling.Width / 2,
//    //    ceiling.Top + ceiling.Height / 2);
//    Trans(angle, this.center, ceiling);
//}

//public void Trans(int angle, PointF ca, Point cd, Ceiling ceiling)
//{
//    if (angle == 90)
//    {
//        this.Begin = StrLine.Trans(this.Begin, ca);
//        this.End = StrLine.Trans(this.End, ca);
//        this.BeginPaintPoint = StrLine.Trans(this.BeginPaintPoint, cd);
//        this.EndPaintPoint = StrLine.Trans(this.EndPaintPoint, cd);
//    }
//    else if (angle == -90)
//    {
//        this.Begin = StrLine.UnTrans(this.Begin, ca);
//        this.End = StrLine.UnTrans(this.End, ca);
//        this.BeginPaintPoint = StrLine.UnTrans(this.BeginPaintPoint, cd);
//        this.EndPaintPoint = StrLine.UnTrans(this.EndPaintPoint, cd);
//    }

//    this.CalcRemarkLoca(ceiling);
//}

//protected Point remarkLineEnd = new Point(0, 0);

//public Point RemarkLineEnd
//{
//    get { return remarkLineEnd; }
//}

//protected Point remarkLineBegin = new Point(0, 0);

//public Point RemarkLineBegin
//{
//    get { return remarkLineBegin; }
//}

//protected Point remarkLoca = new Point(0, 0);

//public Point RemarkLoca
//{
//    get { return remarkLoca; }
//}

//private Rectangle remarkRect = new Rectangle(0, 0, 0, 0);

//public Rectangle RemarkRect
//{
//    get { return remarkRect; }
//}

//protected string remark = "";

//public string Remark
//{
//    get { return remark; }
//    set { remark = value; }
//}

//graphics.DrawLine(Pens.Black, this.remarkLineBegin, this.remarkLineEnd);
//graphics.DrawRectangle(Pens.Black,
//    this.remarkLineEnd.X - 1, this.remarkLineEnd.Y - 1, 3, 3);
//Point d1 = new Point(
//    remarkLineEnd.X == remarkLineBegin.X ? 0 : (remarkLineEnd.X > remarkLineBegin.X ? 3 : -3),
//    remarkLineEnd.Y == remarkLineBegin.Y ? 0 : (remarkLineEnd.Y > remarkLineBegin.Y ? 3 : -3));
//StrLine.DrawArrow(graphics, remarkLineBegin, d1, arrowPen);

//int x = this.remarkLoca.X, y = this.remarkLoca.Y;

//if (Math.Abs(this.depth - defalt_depth) > 1)
//{
//    graphics.DrawString("夹高：" + this.depth.ToString(), font,
//        Brushes.Red, x, y);
//    y += 16;
//}

//if (this.remark.Length > 0)
//{
//    Rectangle rect = new Rectangle(x, y, remarkRect.Width, remarkRect.Height - 16);
//    graphics.DrawString(this.remark, font, Brushes.Red, rect);
//    //graphics.DrawString(this.remark, font, Brushes.Red, x, y);
//}

//Point d1 = new Point(p2.X - p1.X, p2.Y - p1.Y);
//d1.X = (int)(d1.X * 2 / dleng);
//d1.Y = (int)(d1.Y * 2 / dleng);

//private void _moveRemark(Point delta)
//{
//    this.remarkLoca = new Point(this.remarkLoca.X + delta.X,
//        this.remarkLoca.Y + delta.Y);
//    this.remarkLineBegin = new Point(this.remarkLineBegin.X + delta.X,
//        this.remarkLineBegin.Y + delta.Y);
//    this.remarkLineEnd = new Point(this.remarkLineEnd.X + delta.X,
//        this.remarkLineEnd.Y + delta.Y);
//}

//graphics.DrawLine(Pens.Black, this.remarkLineBegin, this.remarkLineEnd);
//graphics.DrawRectangle(Pens.Black,
//    this.remarkLineEnd.X - 1, this.remarkLineEnd.Y - 1, 3, 3);

//if (this.Remark.Length > 0)
//{
//    graphics.DrawString(this.Remark, font, Brushes.Red,
//        this.remarkLoca.X, this.remarkLoca.Y);
//}

//// 水平边
//if (Math.Abs(this.Vector.X) > Math.Abs(this.Vector.Y))
//{
//    if (this.center.X + 1 >= ceiling.Left + ceiling.Width / 2)
//    {
//        Point pt = this.dispLine.Vector.X > 0 ?
//            this.EndPaintPoint : this.BeginPaintPoint;
//        this.remark.LineBegin = this.Vector.X < 0 ?
//            this.BeginPaintPoint : this.EndPaintPoint;
//        this.remark.LineEnd = new Point(pt.X + 40, pt.Y);
//        this.remark.Loca = new Point(this.remark.LineEnd.X + 5,
//            this.remark.LineEnd.Y - 6);
//        //this.remarkLineEnd.X = pt.X + 40;
//        //this.remarkLineEnd.Y = pt.Y;
//        //this.remarkLoca.X = this.remarkLineEnd.X + 5;
//        //this.remarkLoca.Y = this.remarkLineEnd.Y - 6;
//    }
//    else
//    {
//        Point pt = this.dispLine.Vector.X < 0 ?
//            this.EndPaintPoint : this.BeginPaintPoint;
//        this.remark.LineBegin = this.Vector.X > 0 ?
//            this.BeginPaintPoint : this.EndPaintPoint;
//        this.remark.LineEnd = new Point(pt.X - 40, pt.Y);
//        this.remark.Loca = new Point(this.remark.LineEnd.X - 70,
//            this.remark.LineEnd.Y - 6);
//        //this.remarkLineEnd.X = pt.X - 40;
//        //this.remarkLineEnd.Y = pt.Y;
//        //this.remarkLoca.X = this.remarkLineEnd.X - 70;
//        //this.remarkLoca.Y = this.remarkLineEnd.Y - 6;
//    }
//}
//else // 竖直边
//{
//    if (this.center.Y - 1 > ceiling.Top + ceiling.Height / 2)
//    {
//        Point pt = this.Vector.Y < 0 ?
//            this.BeginPaintPoint : this.EndPaintPoint;
//        this.remark.LineBegin = this.Vector.Y < 0 ?
//            this.BeginPaintPoint : this.EndPaintPoint;
//        this.remark.LineEnd = new Point(pt.X, pt.Y + 40);
//        this.remark.Loca = new Point(this.remark.LineEnd.X - 35,
//            this.remark.LineEnd.Y + 5);
//        //this.remarkLineEnd.X = pt.X;
//        //this.remarkLineEnd.Y = pt.Y + 40;
//        //this.remarkLoca.X = this.remarkLineEnd.X - 35;
//        //this.remarkLoca.Y = this.remarkLineEnd.Y + 5;
//    }
//    else
//    {
//        Point pt = this.Vector.Y > 0 ?
//            this.BeginPaintPoint : this.EndPaintPoint;
//        this.remark.LineBegin = this.Vector.Y > 0 ?
//            this.BeginPaintPoint : this.EndPaintPoint;
//        this.remark.LineEnd = new Point(pt.X, pt.Y - 40);
//        this.remark.Loca = new Point(this.remark.LineEnd.X - 35,
//            this.remark.LineEnd.Y - 15);
//        //this.remarkLineEnd.X = pt.X;
//        //this.remarkLineEnd.Y = pt.Y - 40;
//        //this.remarkLoca.X = this.remarkLineEnd.X - 35;
//        //this.remarkLoca.Y = this.remarkLineEnd.Y - 15;
//    }
//}

//if (Math.Abs(this.Vector.X) > Math.Abs(this.Vector.Y))
//{
//    if (intep.X > ceiling.Left + ceiling.Width / 2)
//    {
//        this.remark.LineBegin = this.Vector.X < 0 ?
//            this.BeginPaintPoint : this.EndPaintPoint;
//        this.remark.LineEnd = new Point(pt.X + 40, pt.Y);
//        this.remark.Loca = new Point(this.remark.LineEnd.X + 5, 
//            this.remark.LineEnd.Y - 6);
//        //this.remark.LineEnd.X = pt.X + 40;
//        //this.remark.LineEnd.Y = pt.Y;
//        //this.remarkLoca.X = this.remarkLineEnd.X + 5;
//        //this.remarkLoca.Y = this.remarkLineEnd.Y - 6;
//    }
//    else
//    {
//        this.remark.LineBegin = this.Vector.X > 0 ?
//            this.BeginPaintPoint : this.EndPaintPoint;
//        this.remark.LineEnd = new Point(pt.X - 40, pt.Y);
//        this.remark.Loca = new Point(this.remark.LineEnd.X - 70,
//            this.remark.LineEnd.Y - 6);
//        //this.remarkLineEnd.X = pt.X - 40;
//        //this.remarkLineEnd.Y = pt.Y;
//        //this.remarkLoca.X = this.remarkLineEnd.X - 70;
//        //this.remarkLoca.Y = this.remarkLineEnd.Y - 6;
//    }
//}
//else
//{
//    if (intep.Y > ceiling.Top + ceiling.Height / 2)
//    {
//        this.remark.LineBegin = this.Vector.Y < 0 ?
//            this.BeginPaintPoint : this.EndPaintPoint;
//        this.remark.LineEnd = new Point(pt.X, pt.Y + 40);
//        this.remark.Loca = new Point(this.remark.LineEnd.X - 35,
//            this.remark.LineEnd.Y + 5);
//        //this.remarkLineEnd.X = pt.X;
//        //this.remarkLineEnd.Y = pt.Y + 40;
//        //this.remarkLoca.X = this.remarkLineEnd.X - 35;
//        //this.remarkLoca.Y = this.remarkLineEnd.Y + 5;
//    }
//    else
//    {
//        this.remark.LineBegin = this.Vector.Y > 0 ?
//            this.BeginPaintPoint : this.EndPaintPoint;
//        this.remark.LineEnd = new Point(pt.X, pt.Y - 40);
//        this.remark.Loca = new Point(this.remark.LineEnd.X - 35,
//            this.remark.LineEnd.Y - 15);
//        //this.remarkLineEnd.X = pt.X;
//        //this.remarkLineEnd.Y = pt.Y - 40;
//        //this.remarkLoca.X = this.remarkLineEnd.X - 35;
//        //this.remarkLoca.Y = this.remarkLineEnd.Y - 15;
//    }
//}

//remarkRect = new Rectangle(remarkLoca.X - 5, remarkLoca.Y - 5, 100, 0);
//if (this.depth == ceiling.Depth && this.remark.Length == 0)
//    return;

//if (Math.Abs(this.depth - ceiling.Depth) > 1)
//    remarkRect.Height += 16;
//if (this.remark.Length > 0)
//{
//    Bitmap bmp = new Bitmap(100, 100);
//    Graphics g = Graphics.FromImage(bmp);
//    SizeF size = g.MeasureString(this.remark, new System.Drawing.Font("宋体", 10), 100);
//    remarkRect.Height += (int)size.Height;
//}

//Rectangle rect = new Rectangle(0, 0, 0, 0);

//rect.X = Math.Min(remarkLineBegin.X, remarkLineEnd.X) - 3;
//rect.Y = Math.Min(remarkLineBegin.Y, remarkLineEnd.Y) - 3;
//rect.Width = Math.Max(remarkLineBegin.X, remarkLineEnd.X) - rect.X + 6;
//rect.Height = Math.Max(remarkLineBegin.Y, remarkLineEnd.Y) - rect.Y + 6;

//remarkRect = Rectangle.Union(rect, remarkRect);

//public static Keel Parse(string kstr, uint ceiling_depth)
//{
//    MatchCollection pts = Regex.Matches(kstr, @"\((\d+\.?\d*),(\d+\.?\d*)\)");
//    if (pts.Count != 2)
//        return null;
//    Keel keel = new Keel();

//    keel.Begin = new PointF(float.Parse(pts[1].Groups[1].Value), 
//        float.Parse(pts[2].Groups[1].Value));
//    keel.End = new PointF(float.Parse(pts[3].Groups[1].Value), 
//        float.Parse(pts[4].Groups[1].Value));
//    keel.Depth = ceiling_depth;

//    MatchCollection rms = Regex.Matches(kstr, @"\<(\d*)\$?([^\>]*)?\>");
//    if (rms.Count == 0 || rms[0].Groups.Count == 0)
//        return keel;
//    keel.Depth = uint.Parse(rms[0].Groups[1].Value);

//    if (rms[0].Groups.Count > 2)
//        keel.Remark = rms[0].Groups[2].Value;

//    return keel;
//}

//public Rectangle GetRemarkRect(uint defalt_depth)
//{
//    Rectangle rect = new Rectangle(remarkLoca.X, remarkLoca.Y, 100, 0);
//    if (this.depth == defalt_depth && this.remark.Length == 0)
//        return rect;

//    if (Math.Abs(this.depth - defalt_depth) > 1)
//        rect.Height += 20;
//    if (this.remark.Length > 0)
//        rect.Height += 20;

//    Rectangle r2 = new Rectangle(0, 0, 0, 0);

//    r2.X = Math.Min(remarkLineBegin.X, remarkLineEnd.X) - 3;
//    r2.Y = Math.Min(remarkLineBegin.Y, remarkLineEnd.Y) - 3;
//    r2.Width = Math.Max(remarkLineBegin.X, remarkLineEnd.X) - r2.X + 6;
//    r2.Height = Math.Max(remarkLineBegin.Y, remarkLineEnd.Y) - r2.Y + 6;

//    return Rectangle.Union(rect, r2);
//}

//this.intercept = this._Intercept();
//if (Math.Abs(this.vector.X) < 0.1)
//    this.intercept = StrLine.MaxValue;
//else
//    this.intercept = this.c / this.vector.X;

//if (vector.X == 0 && vector.Y == 0)
//{
//    this.alpha = 0;
//    return;
//}

//if (this.alpha >= Math.PI)
//    this.alpha -= Math.PI;
//if (this.alpha <= -Math.PI)
//    this.alpha += Math.PI;

//private float _Intercept()
//{
//    if (Math.Abs(this.vector.X) < 0.01)
//        return StrLine.MaxValue;
//    return this.c / this.vector.X;
//    //return (this.begin.X * this.end.Y - this.end.X * this.begin.Y) / this.vector.X;
//}

//this.remarkLoca = StrLine.Trans(this.remarkLoca, cd);
//this.remarkLineBegin = StrLine.Trans(this.remarkLineBegin, cd);
//this.remarkLineEnd = StrLine.Trans(this.remarkLineEnd, cd);

//double delta = Math.Abs(_keel.Alpha - tempSeg.Alpha);
//if (this.PaintLine.IsParallel(walles[i].PaintLine))
//    continue;
//if (walles[i].Contains(r, true))

//public void UnTrans(PointF ca, Point cd, Ceiling ceiling)
//{
//    this.Begin = StrLine.UnTrans(this.Begin, ca);
//    this.End = StrLine.UnTrans(this.End, ca);
//    this.BeginPaintPoint = StrLine.UnTrans(this.BeginPaintPoint, cd);
//    this.EndPaintPoint = StrLine.UnTrans(this.EndPaintPoint, cd);
//    this.CalcRemarkLoca(ceiling);
//    //this.remarkLoca = StrLine.UnTrans(this.remarkLoca, cd);
//    //this.remarkLineBegin = StrLine.UnTrans(this.remarkLineBegin, cd);
//    //this.remarkLineEnd = StrLine.UnTrans(this.remarkLineEnd, cd);
//}

//this.dispLine.ChangeLine(this.beginPaintPoint, this.endPaintPoint);
//this.dispLine.ChangeLine(this.beginPaintPoint, this.endPaintPoint);

//this.End = new PointF(this.Begin.X + s * (this.End.X - this.Begin.X),
//    this.Begin.Y + s * (this.End.Y - this.Begin.Y));
//this.EndPaintPoint = new Point(
//    this.BeginPaintPoint.X + (int)(s * (this.EndPaintPoint.X - this.BeginPaintPoint.X)),
//    this.BeginPaintPoint.Y + (int)(s * (this.EndPaintPoint.Y - this.BeginPaintPoint.Y)));
//this.dispLine.ChangeLine(this.BeginPaintPoint, this.EndPaintPoint);

//this.remarkLineBegin = this.BeginPaintPoint.X
//    < this.EndPaintPoint.X ?
//    this.EndPaintPoint : this.BeginPaintPoint;
//this.remarkLineBegin = this.BeginPaintPoint.X
//    < this.EndPaintPoint.X ?
//    this.BeginPaintPoint : this.EndPaintPoint;
//this.remarkLineBegin = this.BeginPaintPoint.Y
//    < this.EndPaintPoint.Y ?
//    this.EndPaintPoint : this.BeginPaintPoint;
//this.remarkLineBegin = this.BeginPaintPoint.Y
//    < this.EndPaintPoint.Y ?
//    this.BeginPaintPoint : this.EndPaintPoint;

//this.remarkLineBegin = this.BeginPaintPoint.X
//    < this.EndPaintPoint.X ?
//    this.EndPaintPoint : this.BeginPaintPoint;
//this.remarkLineBegin = this.BeginPaintPoint.X
//    < this.EndPaintPoint.X ?
//    this.BeginPaintPoint : this.EndPaintPoint;
//this.remarkLineBegin = this.BeginPaintPoint.Y
//    < this.EndPaintPoint.Y ?
//    this.EndPaintPoint : this.BeginPaintPoint;
//this.remarkLineBegin = this.BeginPaintPoint.Y
//    < this.EndPaintPoint.Y ?
//    this.BeginPaintPoint : this.EndPaintPoint;

//Rectangle rect = ceiling.DrawingRect;
//if (Math.Abs(this.dispLine.Vector.X) > Math.Abs(this.dispLine.Vector.Y))
//{
//    //if (this.CenterPaintPoint.X > rect.X + rect.Width / 2)
//    if (pt.X > rect.X + rect.Width / 2)
//    {
//        this.remarkLineBegin = this.BeginPaintPoint.X
//            < this.EndPaintPoint.X ?
//            this.EndPaintPoint : this.BeginPaintPoint;
//        this.remarkLineEnd.X = pt.X + 40;
//        this.remarkLineEnd.Y = pt.Y;
//        this.remarkLoca.X = this.remarkLineEnd.X + 5;
//        this.remarkLoca.Y = this.remarkLineEnd.Y - 6;
//    }
//    else
//    {
//        this.remarkLineBegin = this.BeginPaintPoint.X
//            < this.EndPaintPoint.X ?
//            this.BeginPaintPoint : this.EndPaintPoint;
//        this.remarkLineEnd.X = pt.X - 40;
//        this.remarkLineEnd.Y = pt.Y;
//        this.remarkLoca.X = this.remarkLineEnd.X - 70;
//        this.remarkLoca.Y = this.remarkLineEnd.Y - 6;
//    }
//}
//else
//{
//    //if (this.CenterPaintPoint.Y > rect.Y + rect.Height / 2)
//    if (pt.Y > rect.Y + rect.Height / 2)
//    {
//        this.remarkLineBegin = this.BeginPaintPoint.Y
//            < this.EndPaintPoint.Y ?
//            this.EndPaintPoint : this.BeginPaintPoint;
//        this.remarkLineEnd.X = pt.X;
//        this.remarkLineEnd.Y = pt.Y + 40;
//        this.remarkLoca.X = this.remarkLineEnd.X - 35;
//        this.remarkLoca.Y = this.remarkLineEnd.Y + 5;
//    }
//    else
//    {
//        this.remarkLineBegin = this.BeginPaintPoint.Y
//            < this.EndPaintPoint.Y ?
//            this.BeginPaintPoint : this.EndPaintPoint;
//        this.remarkLineEnd.X = pt.X;
//        this.remarkLineEnd.Y = pt.Y - 40;
//        this.remarkLoca.X = this.remarkLineEnd.X - 35;
//        this.remarkLoca.Y = this.remarkLineEnd.Y - 15;
//    }
//}

//Point p1 = new Point(this.BeginPaintPoint.X, this.BeginPaintPoint.Y);
//Point p2 = new Point(this.EndPaintPoint.X, this.EndPaintPoint.Y);

//Rectangle rect = ceiling.DrawingRect;
//this.BeginPaintPoint = new Point(
//    (int)Math.Round(rect.X + (this.begin.X - ceiling.Left) / ceiling.Scale),
//    (int)Math.Round(rect.Y + (this.begin.Y - ceiling.Top) / ceiling.Scale));
//this.EndPaintPoint = new Point(
//    (int)Math.Round(rect.X + (this.end.X - ceiling.Left) / ceiling.Scale),
//    (int)Math.Round(rect.Y + (this.end.Y - ceiling.Top) / ceiling.Scale));

//Rectangle rect = ceiling.DrawingRect;
//Point bp = this.BeginPaintPoint, ep = this.EndPaintPoint;
//this.Begin = new PointF(
//    ceiling.Left + (bp.X - rect.Left) * ceiling.Scale,
//    ceiling.Top + (bp.Y - rect.Top) * ceiling.Scale);
//this.End = new PointF(
//    ceiling.Left + (ep.X - rect.Left) * ceiling.Scale,
//    ceiling.Top + (ep.Y - rect.Top) * ceiling.Scale);

//private Point RemarkEndP(Ceiling ceiling)
//{
//    PointF pt, inte = new PointF(0, 0);
//    double d2, max = StrLine.MaxValue;

//    for (int i = 0; i < ceiling.Walles.Count; i++)
//    {
//        StrLine cwpl = ceiling.Walles[i].dispLine;
//        if (this.dispLine.IsParallel(cwpl))
//            continue;
//        pt = this.dispLine.Intersect(cwpl);
//        if (!cwpl.Contains(pt, true))
//            continue;

//        d2 = StrLine.Distance2(pt, this.CenterPaintPoint);
//        if (max > d2)
//        {
//            max = d2;
//            inte = pt;
//        }
//    }

//    return Point.Round(inte);
//}

//public new PointF Begin
//{
//    get { return begin; }
//    set
//    {
//        //begin = Point.Round(value);
//        begin = value;
//        this.InitLine();
//        this.center = new PointF((this.begin.X + this.end.X) / 2, 
//            (this.begin.Y + this.end.Y) / 2);
//    }
//}

//public new PointF End
//{
//    get { return end; }
//    set
//    {
//        end = Point.Round(value);
//        //end   = value;
//        this.InitLine();
//        this.center = new PointF((this.begin.X + this.end.X) / 2, 
//            (this.begin.Y + this.end.Y) / 2);
//    }
//}

//private PointF center;

//public PointF Center
//{
//    get { return center; }
//}

//protected Point centerPaintPoint = new Point(0, 0);

//public Point CenterPaintPoint
//{
//    get { return centerPaintPoint; }
//}

//protected Point beginPaintPoint = new Point(0, 0);

//public Point BeginPaintPoint
//{
//    get { return beginPaintPoint; }
//    set
//    {
//        beginPaintPoint = value;
//        this.paintRect.X = Math.Min(this.beginPaintPoint.X, 
//            this.endPaintPoint.X);
//        this.paintRect.Y = Math.Min(this.beginPaintPoint.Y, 
//            this.endPaintPoint.Y);
//        this.paintRect.Width = Math.Max(this.beginPaintPoint.X, 
//            this.endPaintPoint.X) - this.paintRect.X;
//        this.paintRect.Height = Math.Max(this.beginPaintPoint.Y, 
//            this.endPaintPoint.Y) - this.paintRect.Y;

//        this.beginAnchor.X = this.beginPaintPoint.X - 4;
//        this.beginAnchor.Y = this.beginPaintPoint.Y - 4;
//        this.centerAnchor.X = (this.beginPaintPoint.X + this.endPaintPoint.X) / 2 - 4;
//        this.centerAnchor.Y = (this.beginPaintPoint.Y + this.endPaintPoint.Y) / 2 - 4;

//        //this.CenterPaintPoint = new Point((this.beginPaintPoint.X + this.endPaintPoint.X) / 2,
//        //    (this.beginPaintPoint.Y + this.endPaintPoint.Y) / 2);
//        this.paintLine.ChangeLine(this.beginPaintPoint, this.endPaintPoint);
//    }
//}

//protected Point endPaintPoint = new Point(0, 0);

//public Point EndPaintPoint
//{
//    get { return endPaintPoint; }
//    set
//    {
//        endPaintPoint = value;
//        this.paintRect.X = Math.Min(this.beginPaintPoint.X, 
//            this.endPaintPoint.X);
//        this.paintRect.Y = Math.Min(this.beginPaintPoint.Y, 
//            this.endPaintPoint.Y);
//        this.paintRect.Width = Math.Max(this.beginPaintPoint.X, 
//            this.endPaintPoint.X) - this.paintRect.X;
//        this.paintRect.Height = Math.Max(this.beginPaintPoint.Y, 
//            this.endPaintPoint.Y) - this.paintRect.Y;

//        this.endAnchor.X = this.endPaintPoint.X - 4;
//        this.endAnchor.Y = this.endPaintPoint.Y - 4;
//        this.centerAnchor.X = (this.beginPaintPoint.X + this.endPaintPoint.X) / 2 - 4;
//        this.centerAnchor.Y = (this.beginPaintPoint.Y + this.endPaintPoint.Y) / 2 - 4;

//        //this.centerPaintPoint = new Point((this.beginPaintPoint.X + this.endPaintPoint.X) / 2,
//        //    (this.beginPaintPoint.Y + this.endPaintPoint.Y) / 2);
//        this.dispLine.ChangeLine(this.beginPaintPoint, this.endPaintPoint);
//    }
//}

//private Rectangle remark_rect = new Rectangle(0, 0, 0, 0);

//public Rectangle RemarkRect
//{
//    get { return remark_rect; }
//    set { remark_rect = value; }
//}

//this.paintRect.X = (int)Math.Min(dispLine.Begin.X, dispLine.End.X);
//this.paintRect.Y = (int)Math.Min(dispLine.Begin.Y, dispLine.End.Y);
//this.paintRect.Width = (int)Math.Max(dispLine.Begin.X, dispLine.End.X) 
//    - this.paintRect.X;
//this.paintRect.Height = (int)Math.Max(dispLine.Begin.Y, dispLine.End.Y) 
//    - this.paintRect.Y;

//this.beginAnchor.X = (int)dispLine.Begin.X - 4;
//this.beginAnchor.Y = (int)dispLine.Begin.Y - 4;
//this.centerAnchor.X = (int)dispLine.Center.X - 4;
//this.centerAnchor.Y = (int)dispLine.Center.Y - 4;

//this.beginPaintPoint.X += point.X;
//this.beginPaintPoint.Y += point.Y;
//this.endPaintPoint.X += point.X;
//this.endPaintPoint.Y += point.Y;

//this.paintRect.X += point.X;
//this.paintRect.Y += point.Y;
//this.beginAnchor.X += point.X;
//this.beginAnchor.Y += point.Y;
//this.centerAnchor.X += point.X;
//this.centerAnchor.Y += point.Y;
//this.endAnchor.X += point.X;
//this.endAnchor.Y += point.Y;

//this.dispLine.ChangeLine(this.beginPaintPoint, this.endPaintPoint);

//if (Math.Abs(this.beginPaintPoint.X - this.endPaintPoint.X)
//    > Math.Abs(this.beginPaintPoint.Y - this.endPaintPoint.Y))
//{
//    if (this.CenterPaintPoint.X > rect.X + rect.Width / 2)
//    {
//        this.remarkLineBegin = this.beginPaintPoint.X
//            < this.endPaintPoint.X ?
//            this.endPaintPoint : this.beginPaintPoint;
//        this.remarkLineEnd.X = pt.X + 40;
//        this.remarkLineEnd.Y = pt.Y;
//        this.remarkLoca.X = this.remarkLineEnd.X + 5;
//        this.remarkLoca.Y = this.remarkLineEnd.Y - 6;

//        //this.remarkLineEnd.X = this.remarkLineBegin.X + 40;
//        //this.remarkLineEnd.Y = this.remarkLineBegin.Y;
//    }
//    else
//    {
//        this.remarkLineBegin = this.beginPaintPoint.X
//            < this.endPaintPoint.X ?
//            this.beginPaintPoint : this.endPaintPoint;
//        this.remarkLineEnd.X = pt.X - 40;
//        this.remarkLineEnd.Y = pt.Y;
//        this.remarkLoca.X = this.remarkLineEnd.X - 70;
//        this.remarkLoca.Y = this.remarkLineEnd.Y - 6;
//        //this.remarkLineBegin = this.beginPaintPoint;
//        //this.remarkLineEnd.X = this.remarkLineBegin.X - 40;
//        //this.remarkLineEnd.Y = this.remarkLineBegin.Y;
//    }
//}
//else
//{
//    if (this.CenterPaintPoint.Y > rect.Y + rect.Height / 2)
//    {
//        this.remarkLineBegin = this.beginPaintPoint.Y
//            < this.endPaintPoint.Y ?
//            this.endPaintPoint : this.beginPaintPoint;
//        this.remarkLineEnd.X = pt.X;
//        this.remarkLineEnd.Y = pt.Y + 40;
//        this.remarkLoca.X = this.remarkLineEnd.X - 35;
//        this.remarkLoca.Y = this.remarkLineEnd.Y + 5;
//        //this.remarkLineBegin = this.endPaintPoint;
//        //this.remarkLineEnd.X = this.remarkLineBegin.X;
//        //this.remarkLineEnd.Y = this.remarkLineBegin.Y + 40;
//    }
//    else
//    {
//        this.remarkLineBegin = this.beginPaintPoint.Y
//            < this.endPaintPoint.Y ?
//            this.beginPaintPoint : this.endPaintPoint;
//        this.remarkLineEnd.X = pt.X;
//        this.remarkLineEnd.Y = pt.Y - 40;
//        this.remarkLoca.X = this.remarkLineEnd.X - 35;
//        this.remarkLoca.Y = this.remarkLineEnd.Y - 15;
//        //this.remarkLineBegin = this.beginPaintPoint;
//        //this.remarkLineEnd.X = this.remarkLineBegin.X;
//        //this.remarkLineEnd.Y = this.remarkLineBegin.Y - 40;
//    }
//}

//this.BeginPaintPoint = new Point(
//    (int)(ceiling.DrawingRect.Left + (this.Begin.X - ceiling.Left) / ceiling.Scale),
//    (int)(ceiling.DrawingRect.Top + (this.Begin.Y - ceiling.Top) / ceiling.Scale));
//this.EndPaintPoint = new Point(
//    (int)(ceiling.DrawingRect.Left + (this.End.X - ceiling.Left) / ceiling.Scale),
//    (int)(ceiling.DrawingRect.Top + (this.End.Y - ceiling.Top) / ceiling.Scale));
//this.EndPaintPoint = new Point(
//    (int)(ceiling.DrawingRect.Left + (this.End.X - ceiling.Left) / ceiling.Scale),
//    (int)(ceiling.DrawingRect.Top + (this.End.Y - ceiling.Top) / ceiling.Scale));
//this.BeginPaintPoint = new Point(
//    (int)(ceiling.DrawingRect.Left + (this.Begin.X - ceiling.Left) / ceiling.Scale),
//    (int)(ceiling.DrawingRect.Top + (this.Begin.Y - ceiling.Top) / ceiling.Scale));
//this.BeginPaintPoint = new Point(
//    (int)(ceiling.DrawingRect.Left + (this.Begin.X - ceiling.Left) / ceiling.Scale),
//    (int)(ceiling.DrawingRect.Top + (this.Begin.Y - ceiling.Top) / ceiling.Scale));
//this.EndPaintPoint = new Point(
//    (int)(ceiling.DrawingRect.Left + (this.End.X - ceiling.Left) / ceiling.Scale),
//    (int)(ceiling.DrawingRect.Top + (this.End.Y - ceiling.Top) / ceiling.Scale));

//this.Begin = new PointF(ceiling.Left,
//    ceiling.Top + (p.Y - ceiling.DrawingRect.Top) * ceiling.Scale);
//this.End = new PointF(ceiling.Right,
//    ceiling.Top + (p.Y - ceiling.DrawingRect.Top) * ceiling.Scale);
//this.Begin = new PointF(ceiling.Left + (p.X - ceiling.DrawingRect.Left) * ceiling.Scale,
//    ceiling.Top);
//this.End = new PointF(ceiling.Left + (p.X - ceiling.DrawingRect.Left) * ceiling.Scale,
//    ceiling.Bottom);

//this._SetKeelBeginPaintPoint(_keel,
//    Point.Round(new PointF(_keel.BeginPaintPoint.X + delta.X,
//        _keel.BeginPaintPoint.Y + delta.Y)));
//this._SetKeelBeginPaintPoint(_keel,
//    Point.Round(new PointF(_keel.BeginPaintPoint.X + delta.X,
//        _keel.BeginPaintPoint.Y + delta.Y)));
//this._SetKeelEndPaintPoint(_keel,
//    Point.Round(new PointF(_keel.EndPaintPoint.X + delta.X,
//        _keel.EndPaintPoint.Y + delta.Y)));
//this._SetKeelEndPaintPoint(_keel,
//    Point.Round(new PointF(_keel.EndPaintPoint.X + delta.X,
//        _keel.EndPaintPoint.Y + delta.Y)));

//if (Math.Abs(delta.X) + Math.Abs(delta.Y) < 3)
//    return true;
//Rectangle rect = this.PaintRect;
//Point center = this.CenterPaintPoint;
//PointF dp = new PointF(delta.X + center.X, delta.Y + center.Y);

//int dis = (int)(100 / ceiling.Scale);
//if (!ceiling.Contain(Point.Round(dp), dis))
//    return false;
//for (int i = 0; i < this.MainKeelList.Count; i++)
//{
//    if (_keel == this.MainKeelList[i])
//        continue;
//    if (this.MainKeelList[i].PaintLine.Distance(dp) < dis)
//        return false;
//}

//private void _SetKeelBeginPaintPoint(Keel _keel, Point point)
//{
//    //InvalidateRect(_keel.PaintRect);
//    _keel.BeginPaintPoint = point;
//    _keel.Begin = new PointF(this.ceiling.Left + this.ceiling.Scale * (_keel.BeginPaintPoint.X - this.ceiling.DrawingRect.Left),
//        this.ceiling.Top + this.ceiling.Scale * (_keel.BeginPaintPoint.Y - this.ceiling.DrawingRect.Top));
//    //InvalidateRect(_keel.PaintRect);
//}

//private void _SetKeelEndPaintPoint(Keel _keel, Point point)
//{
//    //InvalidateRect(_keel.PaintRect);
//    _keel.EndPaintPoint = point;
//    _keel.End = new PointF(this.ceiling.Left + this.ceiling.Scale * (_keel.EndPaintPoint.X - this.ceiling.DrawingRect.Left),
//        this.ceiling.Top + this.ceiling.Scale * (_keel.EndPaintPoint.Y - this.ceiling.DrawingRect.Top));
//    //InvalidateRect(_keel.PaintRect);
//}
