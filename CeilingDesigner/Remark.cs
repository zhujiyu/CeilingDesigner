using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Drawing;

namespace CeilingDesigner
{
    public class Remark
    {
        private Point lineEnd = new Point(0, 0);

        public Point LineEnd
        {
            get { return lineEnd; }
            set { lineEnd = value; }
        }

        private Point lineBegin = new Point(0, 0);

        public Point LineBegin
        {
            get { return lineBegin; }
            set { lineBegin = value; }
        }

        private Point loca = new Point(0, 0);

        public Point Loca
        {
            get { return loca; }
            set { loca = value; }
        }

        private Rectangle dispRect = new Rectangle(0, 0, 0, 0);

        public Rectangle DispRect
        {
            get { return dispRect; }
            //set { dispRect = value; }
        }

        private Rectangle paintRect = new Rectangle(0, 0, 0, 0);

        public Rectangle PaintRect
        {
            get { return paintRect; }
        }

        private string defRemark = "";

        public string DefRemark
        {
            get { return defRemark; }
            set { defRemark = value; }
        }

        protected string remark = "";

        public string CustomRemark
        {
            get { return remark; }
            set { remark = value; }
        }

        private string dispStr;
        private Font deffont = new Font("宋体", 10);
        Bitmap bmp = new Bitmap(100, 100);

        public void CalcRect()
        {
            dispStr = (defRemark != null && defRemark.Length > 0) ? defRemark : "";
            remark = remark != null ? remark : "";
            dispStr += (dispStr.Length > 0 && remark.Length > 0 ? "\n" : "") + remark;
            dispRect = new Rectangle(loca.X, loca.Y, 120, 0);

            if (dispStr.Length > 0)
            {
                Graphics g = Graphics.FromImage(bmp);
                SizeF size = g.MeasureString(dispStr, deffont, 100);
                g.Dispose();
                dispRect.Height += (int)size.Height;
            }

            if (dispRect.Height == 0)
                return;

            paintRect.X = Math.Min(lineBegin.X, lineEnd.X) - 3;
            paintRect.Y = Math.Min(lineBegin.Y, lineEnd.Y) - 3;
            paintRect.Width = Math.Max(lineBegin.X, lineEnd.X) - paintRect.X + 6;
            paintRect.Height = Math.Max(lineBegin.Y, lineEnd.Y) - paintRect.Y + 6;
            paintRect = Rectangle.Union(paintRect, dispRect);
        }

        public void Draw(Graphics graphics, Font font, Pen arrowPen)
        {
            if (dispStr == null || dispStr.Length == 0)
                return;

            graphics.DrawLine(Pens.Black, this.lineBegin, this.lineEnd);
            graphics.DrawRectangle(Pens.Black, this.lineEnd.X - 1,
                this.lineEnd.Y - 1, 2, 2);

            Point d1 = new Point(
                lineEnd.X == lineBegin.X ? 0 : (lineEnd.X > lineBegin.X ? 3 : -3),
                lineEnd.Y == lineBegin.Y ? 0 : (lineEnd.Y > lineBegin.Y ? 3 : -3));
            StrLine.DrawArrow(graphics, lineBegin, d1, arrowPen);

            //graphics.DrawRectangle(Pens.Black, dispRect);
            graphics.DrawString(dispStr, font, Brushes.Red, dispRect);
        }

        public void Move(Point delta)
        {
            this.loca = new Point(this.loca.X + delta.X, 
                this.loca.Y + delta.Y);
            this.lineBegin = new Point(this.lineBegin.X + delta.X, 
                this.lineBegin.Y + delta.Y);
            this.lineEnd = new Point(this.lineEnd.X + delta.X, 
                this.lineEnd.Y + delta.Y);
        }
    }
}
