using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CeilingDesigner
{
    /// <summary>
    /// 奥普扣板
    /// 这里将电器也看作扣板来处理
    /// </summary>
    public class AupuBuckle
    {
        #region 属性表

        private static Pen selectedPen = new Pen(Color.GreenYellow, 2);
        private static Pen borderPen = new Pen(Color.Black, 1);

        private Bitmap bitmap = null;

        public Bitmap Bitmap
        {
            get { return bitmap; }
        }

        private int transp = 0;

        /// <summary>
        /// 用1表示顺时针旋转90度
        /// </summary>
        public int Trans
        {
            get { return transp; }
        }

        //private bool _Trans
        //{
        //    get { return transp % 2 == 0; }
        //}

        public CeilingDataSet.productsRow OriginalRow
        {
            get { return buckleNode.OriginalRow; }
        }

        private BuckleNode buckleNode;

        public BuckleNode BuckleNode
        {
            get { return buckleNode; }
        }

        public string type
        {
            get
            {
                if (this.OriginalRow.product_classesRow != null)
                    return this.OriginalRow.product_classesRow.type;
                else
                    return "auxi";
            }
        }

        public float Width
        {
            get
            {
                return this.transp % 2 == 0 ? this.OriginalRow.width
                    : this.OriginalRow.height;
            }
        }

        public float Height
        {
            get
            {
                return this.transp % 2 == 0 ? this.OriginalRow.height
                    : this.OriginalRow.width;
            }
        }

        private PointF location; // 这个是在房顶平面里的相对位置

        public PointF Location
        {
            get { return location; }
            set { location = value; }
        }

        private int index = -1;

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        private float integrity = 100;

        public float Integrity
        {
            get { return integrity; }
            set { integrity = value; }
        }

        private Rectangle[] displayScans = null;

        /// <summary>
        /// 同Scans，但是屏幕上的坐标
        /// </summary>
        public Rectangle[] DisplayScans
        {
            get { return displayScans; }
            set { displayScans = value; }
        }

        private RectangleF[] scans = null;

        /// <summary>
        /// 产品所在的实际位置，这个位置可能因为吊顶的实际情况，
        /// 被分割成了多个小区域
        /// </summary>
        public RectangleF[] Scans
        {
            get { return scans; }
            set { scans = value; }
        }

        private Rectangle drawingRect;

        /// <summary>
        /// 此处是产品要显示的屏幕位置，
        /// 这个位置不是精确的，是显示区域的外框
        /// </summary>
        public Rectangle DrawingRect
        {
            get { return drawingRect; }
            set
            {
                drawingRect = value;

                if (this.drawingRect.Width < 2 || this.drawingRect.Height < 2)
                    return;
                if (this.buckleNode.OriginalImage == null)
                    return;
                if (this.bitmap != null && this.bitmap.Size != value.Size)
                {
                    this.GeneBitmap();
                }
                //if (this.bitmap != null && this.bitmap.Size == value.Size)
                //    return;
                //this.GeneBitmap();
            }
        }

        public string Name
        {
            get 
            {
                if (OriginalRow.RowState == System.Data.DataRowState.Deleted 
                    && OriginalRow.RowState == System.Data.DataRowState.Detached)
                    return "";
                return OriginalRow.name;
            }
        }

        #endregion

        public AupuBuckle(BuckleNode node)
        {
            this.buckleNode = node;
        }

        /// <summary>
        /// 创建一块新扣板
        /// </summary>
        /// <param name="node">扣板模版</param>
        /// <param name="trans">旋转角度，这里用1表示顺时针旋转90度</param>
        public AupuBuckle(BuckleNode node, int trans)
        {
            this.buckleNode = node;
            this.transp = (trans + 4) % 4;
        }

        public AupuBuckle clone()
        {
            AupuBuckle product = new AupuBuckle(this.buckleNode);

            product.transp = this.transp;
            product.index = -1;
            product.integrity = this.integrity;

            product.location = this.location;
            product.drawingRect = this.drawingRect;
            product.buckleNode = this.buckleNode;

            return product;
        }

        public void Merge(AupuBuckle product)
        {
            if (this.index != product.index)
                return;
            this.integrity += product.integrity;

            RectangleF[] _scans = new RectangleF[this.scans.Length + product.scans.Length];
            for (int i = 0; i < this.scans.Length; i++)
                _scans[i] = this.scans[i];
            for (int i = 0; i < product.scans.Length; i++)
                _scans[i + this.scans.Length] = product.scans[i];
            this.scans = _scans;

            Rectangle[] _dispscans = new Rectangle[this.displayScans.Length 
                + product.displayScans.Length];
            for (int i = 0; i < this.displayScans.Length; i++)
                _dispscans[i] = this.displayScans[i];
            for (int i = 0; i < product.displayScans.Length; i++)
                _dispscans[i + this.displayScans.Length] = product.displayScans[i];
            this.displayScans = _dispscans;
        }

        public void GeneBitmap()
        {
            if (transp % 2 == 0)
                this.bitmap = new Bitmap(this.buckleNode.OriginalImage,
                    this.drawingRect.Width, this.drawingRect.Height);
            else
                this.bitmap = new Bitmap(this.buckleNode.OriginalImage,
                    this.drawingRect.Height, this.drawingRect.Width);

            for (int i = 0; i < transp; i++)
                this.bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }

        public void Draw(Graphics graphics)
        {
            if (this.integrity < 1 
                || this.drawingRect.Width < 1 || this.drawingRect.Height < 1)
                return;

            if (this.bitmap == null )
            {
                if (this.buckleNode.OriginalImage == null)
                    this.buckleNode.SynLoadImage();
                this.GeneBitmap();
            }

            if (this.bitmap != null && this.displayScans != null)
            {
                for (int i = 0; i < this.displayScans.Length; i++)
                {
                    Rectangle rect = this.displayScans[i];
                    rect.X -= this.drawingRect.X; rect.Y -= this.drawingRect.Y;
                    graphics.DrawImage(this.bitmap, this.displayScans[i], rect,
                        GraphicsUnit.Pixel);
                }
            }
            else if (this.bitmap != null)
                graphics.DrawImage(this.bitmap, this.drawingRect);
            else
                graphics.FillRectangle(Brushes.LightGray, this.drawingRect);

            //if (this.integrity >= 100)
            //DrawBorder(graphics, borderPen);
        }

        public void DrawBorder(Graphics graphics, Pen pen)
        {
            if (this.displayScans != null && this.displayScans.Length == 1)
                graphics.DrawRectangle(pen, this.displayScans[0]);
            else
                graphics.DrawRectangle(pen, this.drawingRect);
            //graphics.DrawRectangle(pen, this.drawingRect);
        }

        public void DrawSelectedFlag(Graphics graphics)
        {
            DrawBorder(graphics, selectedPen);
        }

        public void Translate(Point point)
        {
            this.drawingRect.X += point.X;
            this.drawingRect.Y += point.Y;

            if (this.displayScans == null)
                return;

            for (int i = 0; i < this.displayScans.Length; i++)
            {
                this.displayScans[i].X += point.X;
                this.displayScans[i].Y += point.Y;
            }
        }

        /// <summary>
        /// 顺时针或者逆时针旋转90度
        /// </summary>
        /// <param name="_angle">1表示顺时针旋转90度，-1表示反向90度</param>
        public void Transpose(int _angle)
        {
            this.transp = (this.transp + _angle + 4) % 4;
            if (this.bitmap != null)
            {
                this.bitmap.RotateFlip(_angle == 1
                    ? RotateFlipType.Rotate90FlipNone
                    : RotateFlipType.Rotate270FlipNone);
            }
        }

        public void Release()
        {
            if (this.bitmap != null)
            {
                this.bitmap.Dispose();
                this.bitmap = null;
            }
        }
    }
}

//int temp = this.drawingRect.Width;
//this.drawingRect.Width = this.drawingRect.Height;
//this.drawingRect.Height = temp;

////this.transp = !this.transp;
////_angle = _angle == 90 ? 1 : -1;

//PixelFormat pf = PixelFormat.Format32bppArgb;
//if (bkColor == Color.Transparent)
//{
//    pf = PixelFormat.Format32bppArgb;
//}
//else
//{
//    pf = bmp.PixelFormat;
//}

//System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
//path.AddRectangle(new RectangleF(0f, 0f, drawingRect.Width, drawingRect.Height));

//Matrix mtrx = new Matrix();
//mtrx.Rotate(90);

//RectangleF rct = path.GetBounds(mtrx);
//Bitmap dst = new Bitmap((int)rct.Width, (int)rct.Height, bitmap.PixelFormat);
//if (this.bitmap != null)
//{
//    Bitmap tmp = new Bitmap(drawingRect.Width, drawingRect.Height, bitmap.PixelFormat);
//    Graphics g = Graphics.FromImage(tmp);
//    g.DrawImageUnscaled(bitmap, 1, 1);
//    g.Dispose();

//    g = Graphics.FromImage(dst);
//    //g.Clear(bkColor);
//    g.TranslateTransform(-rct.X, -rct.Y);
//    g.RotateTransform(90);
//    g.InterpolationMode = InterpolationMode.HighQualityBilinear;
//    g.DrawImageUnscaled(tmp, 0, 0);
//    g.Dispose();

//    tmp.Dispose();

//    this.bitmap.Dispose();
//}
//this.bitmap = dst;

//if (this.transp)
//    return this.OriginalRow.height;
//else
//    return this.OriginalRow.width;
//if (this.transp)
//    return this.OriginalRow.width;
//else
//    return this.OriginalRow.height;

//Rectangle rect = new Rectangle(0, 0, this.drawingRect.Width,
//    this.drawingRect.Height);
//graphics.DrawImage(this.bitmap, this.drawingRect, rect,
//    GraphicsUnit.Pixel);

//if (this.bitmap == null)
//    graphics.FillRectangle(Brushes.LightGray, this.drawingRect);
//else if (this.displayScans == null)
//    graphics.DrawImage(this.bitmap, this.drawingRect);
//else
//{
//    for (int i = 0; i < this.displayScans.Length; i++)
//    {
//        Rectangle rect = this.displayScans[i];
//        rect.X -= this.drawingRect.X; rect.Y -= this.drawingRect.Y;
//        graphics.DrawImage(this.bitmap, this.displayScans[i], rect, 
//            GraphicsUnit.Pixel);
//    }
//}

//if (this.displayScans.Length > 0)
//{
//    if (this.displayScans.Length > 1)
//        graphics.DrawRectangle(selectedPen, this.drawingRect);
//    else
//        graphics.DrawRectangle(selectedPen, this.displayScans[0]);
//}
//graphics.DrawRectangle(selectedPen, this.drawingRect);

//if (this.bitmap != null)
//{
//    for (int i = 0; i < this.displayScans.Length; i++)
//    {
//        graphics.DrawImage(this.bitmap, this.displayScans[i]);
//    }
//}
//else
//    graphics.FillRectangle(Brushes.LightGray, this.drawingRect);

//if (this.displayScans.Length > 0)
//{
//    if (this.displayScans.Length > 1)
//        graphics.DrawRectangle(borderPen, this.drawingRect);
//    else
//        graphics.DrawRectangle(borderPen, this.displayScans[0]);
//}

//if (this.bitmap != null)
//    //graphics.DrawImage(this.bitmap, this.drawingRect.Location);
//    graphics.DrawImage(this.bitmap, this.drawingRect);
//else
//    graphics.FillRectangle(Brushes.LightGray, this.drawingRect);

//if (this.integrity > 95)
//graphics.DrawRectangle(borderPen, this.drawingRect);
