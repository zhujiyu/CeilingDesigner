using System;
using System.Collections.Generic;
using System.Linq;
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

        //public Remark() { }

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

//dispStr += dispStr.Length > 0 && remark != null && remark.Length > 0 ? "\n" : "";
//dispStr += remark != null && remark.Length > 0 ? remark : "";
//string dispStr = (defRemark != null && defRemark.Length > 0 ? defRemark + "\n" : "")
//    + this.remark;

//public void Draw(string str, Graphics graphics, Font font, Pen arrowPen)
//{
//    string _str = (str != null && str.Length > 0 ? str + "\n" : "") 
//        + this.remark;
//    if (_str.Length == 0)
//        return;

//    graphics.DrawLine(Pens.Black, this.lineBegin, this.lineEnd);
//    graphics.DrawRectangle(Pens.Black, this.lineEnd.X - 1, 
//        this.lineEnd.Y - 1, 3, 3);

//    Point d1 = new Point(
//        lineEnd.X == lineBegin.X ? 0 : (lineEnd.X > lineBegin.X ? 3 : -3),
//        lineEnd.Y == lineBegin.Y ? 0 : (lineEnd.Y > lineBegin.Y ? 3 : -3));
//    StrLine.DrawArrow(graphics, lineBegin, d1, arrowPen);

//    graphics.DrawRectangle(Pens.Black, dispRect);
//    graphics.DrawString(_str, font, Brushes.Red, dispRect);
//}

//public void CalcRect(string str)
//{
//    string dispStr = (str != null && str.Length > 0 ? str + "\n" : "")
//        + this.remark;
//    dispRect = new Rectangle(loca.X, loca.Y, 120, 0);

//    if (dispStr.Length > 0)
//    {
//        Graphics g = Graphics.FromImage(bmp);
//        SizeF size = g.MeasureString(dispStr, deffont, 100);
//        g.Dispose();
//        dispRect.Height += (int)size.Height;
//    }

//    if (dispRect.Height == 0)
//        return;

//    paintRect.X = Math.Min(lineBegin.X, lineEnd.X) - 3;
//    paintRect.Y = Math.Min(lineBegin.Y, lineEnd.Y) - 3;
//    paintRect.Width = Math.Max(lineBegin.X, lineEnd.X) - paintRect.X + 6;
//    paintRect.Height = Math.Max(lineBegin.Y, lineEnd.Y) - paintRect.Y + 6;
//    paintRect = Rectangle.Union(paintRect, dispRect);
//}

//public void CalcRect(int height)
//{
//    dispRect = new Rectangle(loca.X, loca.Y, 120, height);

//    if (this.remark.Length > 0)
//    {
//        Graphics g = Graphics.FromImage(bmp);
//        SizeF size = g.MeasureString(this.remark, deffont, 100);
//        g.Dispose();
//        dispRect.Height += (int)size.Height;
//    }

//    if (dispRect.Height == 0)
//        return;

//    paintRect.X = Math.Min(lineBegin.X, lineEnd.X) - 3;
//    paintRect.Y = Math.Min(lineBegin.Y, lineEnd.Y) - 3;
//    paintRect.Width = Math.Max(lineBegin.X, lineEnd.X) - paintRect.X + 6;
//    paintRect.Height = Math.Max(lineBegin.Y, lineEnd.Y) - paintRect.Y + 6;
//    paintRect = Rectangle.Union(paintRect, dispRect);
//}

//paintRect = new Rectangle(loca.X - 5, loca.Y - 5, 100, height);
//Rectangle rect = new Rectangle(0, 0, 0, 0);

//rect.X = Math.Min(lineBegin.X, lineEnd.X) - 3;
//rect.Y = Math.Min(lineBegin.Y, lineEnd.Y) - 3;
//rect.Width = Math.Max(lineBegin.X, lineEnd.X) - rect.X + 6;
//rect.Height = Math.Max(lineBegin.Y, lineEnd.Y) - rect.Y + 6;

//paintRect = Rectangle.Union(rect, paintRect);

//StringFormat sf = new StringFormat(StringFormatFlags.NoWrap);
//graphics.DrawRectangle(Pens.Black, new Rectangle(this.loca.X, this.loca.Y, 100, 60));
//graphics.DrawString(_str, font, Brushes.Red,
//    new RectangleF(this.loca.X, this.loca.Y, 100, 60));

//RectangleF rect = new RectangleF(this.loca.X, this.loca.Y,
//    106, 60);
//graphics.DrawRectangle(Pens.Black, Rectangle.Round(rect));
//graphics.DrawString(_str, font, Brushes.Red, rect);

//this.loca.X, this.loca.Y

//SizeF size = graphics.MeasureString(_str, font);

//float x = this.loca.X, y = this.loca.Y;

//if (str != null && str.Length > 0)
//{
//    graphics.DrawString(str, font, Brushes.Red, x, y);
//    SizeF size = graphics.MeasureString(str, font);
//    y += size.Height;
//}

//if (this.remark.Length > 0)
//{
//    SizeF size = graphics.MeasureString(this.remark, font);
//    RectangleF rect = new RectangleF(x, y, paintRect.Width, size.Height);
//    graphics.DrawString(this.remark, font, Brushes.Red, rect);
//    //graphics.DrawString(this.remark, font, Brushes.Red, x, y);
//}

//string _str = (str != null && str.Length > 0) ? 
//    str + "\n" + this.remark : this.remark;
