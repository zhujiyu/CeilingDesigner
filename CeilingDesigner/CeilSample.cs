using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CeilingDesigner
{
    public partial class CeilSample : UserControl
    {
        private List<Wall> walles = new List<Wall>();

        public List<Wall> Walles
        {
            get { return walles; }
            //set { walles = value; }
        }

        private Ceiling ceiling = new Ceiling();

        public Ceiling Ceiling
        {
            get { return ceiling; }
            set { ceiling = value; }
        }

        private Font font = new System.Drawing.Font("宋体", 12);
        private Pen samplePen = new Pen(Color.Black, 2);

        private CeilingDataSet.ceiling_sample_wallesDataTable wallesTable = null;

        public CeilingDataSet.ceiling_sample_wallesDataTable WallesTable
        {
            get { return wallesTable; }
            set { wallesTable = value; }
        }

        private string sampleName;

        public string SampleName
        {
            get { return sampleName; }
            set { sampleName = value; }
        }

        public CeilSample()
        {
            InitializeComponent();
        }

        public CeilSample(int sampleid)
        {
            InitializeComponent();
            this.LoadSample(sampleid);
        }

        public void LoadSample(int sampleid)
        {
            float x1, y1, x2, y2;
            DataView view = new DataView();
            CeilingDataSet.ceiling_sample_wallesRow row;

            this.ceiling.Clear();
            view.Table = this.wallesTable;
            view.RowFilter = "sample_id = " + sampleid;
            view.Sort = "wallnum";
            if (view.Count < 1)
                return;
            
            row = view[view.Count - 1].Row as CeilingDataSet.ceiling_sample_wallesRow;
            x1 = row.endx;
            y1 = row.endy;

            for (int i = 0; i < view.Count; i++)
            {
                row = view[i].Row as CeilingDataSet.ceiling_sample_wallesRow;
                x2 = row.endx;
                y2 = row.endy;

                Wall wall = new Wall();
                wall.Wallnum = i + 1;
                wall.Begin = new PointF(x1, y1);
                wall.End = new PointF(x2, y2);
                wall.Randian = row.radian;

                this.ceiling.AddWall(wall);
                x1 = x2; y1 = y2;
            }

            //this.wallesAdapter.FillBySampleID(this.wallesTable, sampleid);
            //if (this.wallesTable.Count <= 0)
            //    return;

            //x1 = wallesTable[wallesTable.Count - 1].endx;
            //y1 = wallesTable[wallesTable.Count - 1].endy;

            //for (int i = 0; i < this.wallesTable.Count; i++)
            //{
            //    x2 = wallesTable[i].endx;
            //    y2 = wallesTable[i].endy;

            //    Wall wall = new Wall();
            //    wall.Wallnum = i + 1;
            //    //wall.OriginalDataRow = wallesTable[i];
            //    wall.Begin = new PointF(x1, y1);
            //    wall.End = new PointF(x2, y2);
            //    wall.Randian = wallesTable[i].radian;

            //    this.ceiling.AddWall(wall);
            //    x1 = x2; y1 = y2;
            //}

            Rectangle rect = new Rectangle(this.Width / 10, this.Height / 10, this.Width * 8 / 10, this.Height * 8 / 10);
            float sw = this.ceiling.Width / rect.Width;
            float sh = this.ceiling.Height / rect.Height;
            if (sw > sh)
            {
                int height = (int)(sh * rect.Width / sw);
                rect.Y += (rect.Height - height) / 2;
                rect.Height = height;
            }
            else
            {
                int width = (int)(sw * rect.Height / sh);
                rect.X += (rect.Width - width) / 2;
                rect.Width = width;
            }
            this.ceiling.SetDrawingRect(rect);
            this.Invalidate();
        }

        public void DrawSample(Graphics graphics, Pen pen, Rectangle rect)
        {
            this.ceiling.SetDrawingRect(rect);
            for (int i = 0; i < this.ceiling.Walles.Count; i++)
            {
                this.ceiling.Walles[i].Draw(graphics, pen);
            }
        }

        private void CeilSample_Paint(object sender, PaintEventArgs e)
        {
            this.ceiling.Draw(e.Graphics, samplePen);
            SizeF size = e.Graphics.MeasureString(sampleName, font, this.Width);
            RectangleF rect = new RectangleF((this.Width - size.Width) / 2, 
                (this.Height - size.Height) / 2, size.Width, size.Height);
            e.Graphics.DrawString(this.sampleName, font, Brushes.Black, rect);
        }
    }
}
