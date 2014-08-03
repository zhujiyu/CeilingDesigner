using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CeilingDesigner
{
    public partial class TileView : UserControl
    {
        PictureBox[] pboxs = new PictureBox[4];
        AupuTile tile = null;

        public AupuTile Tile
        {
            get { return tile; }
        }

        public TileView(AupuTile _tile)
        {
            InitializeComponent();

            pboxs[0] = this.pictureBox1;
            pboxs[1] = this.pictureBox2;
            pboxs[2] = this.pictureBox3;
            pboxs[3] = this.pictureBox4;

            this.tile = _tile.Clone();
            for (uint i = 0; i < 4; i++)
            {
                pboxs[i].Tag = tile[i];
                pboxs[i].Image = null;
            }
        }

        private void TileView_Resize(object sender, EventArgs e)
        {
            mainPanel.Location = new Point((Width - mainPanel.Width) / 2, 
                (Height - mainPanel.Height) / 2);
        }

        private void SetMenu(OrderGraph graph)
        {
            ShareData.form.SetDoMenu(graph);
            ShareData.form.SetCeilingMenu(true);
            ShareData.form.SetManageMenu(true);
            ShareData.form.SetWallMenu(true);
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            OrderGraph graph = (this.ParentForm as PalaceForm).CurOrderGraph;
            if (graph == null)
                return;
            graph.ProductSet.TileProducts(this.tile);
            SetMenu(graph);
            this.Dispose();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            OrderGraph graph = (this.ParentForm as PalaceForm).CurOrderGraph;
            if (graph != null)
                SetMenu(graph);
            this.Dispose();
        }

        PictureBox current = null;

        private Bitmap GetDispImage(AupuBuckle buckle)
        {
            if (buckle.Bitmap == null)
            {
                buckle.DrawingRect = new Rectangle(0, 0,
                    (int)Math.Round(buckle.Width),
                    (int)Math.Round(buckle.Height));
                buckle.GeneBitmap();
            }
            return buckle.Bitmap;
        }

        public void DispTile()
        {
            try
            {
                for (uint i = 0; i < 4; i++)
                {
                    //pboxs[i].Image = tile[i].Product != null ?
                    //    GetDispImage(tile[i].Product) : null;
                    pboxs[i].Image = tile[i].Visible ?
                        GetDispImage(tile[i].Product) : null;
                }
            }
            catch (Exception ex)
            {
                string logFile = ShareData.GetErrPath() + "err" + DateTime.Now.Year
                    + DateTime.Now.Month + DateTime.Now.Day + ".log";
                Program.WriteErr(ex, logFile);
                Program.MessageErr(ex, logFile);
            }
        }

        private void AddBuckle(AupuBuckle buckle, uint idx)
        {
            tile.AddBuckle(buckle, idx);
            DispTile();
        }

        public void AddBuckle(AupuBuckle buckle)
        {
            tile.AddBuckle(buckle, tile.Length);
            DispTile();
        }

        public void AddBuckle(AupuBuckle buckle, Point loca)
        {
            Point pt = panel1.PointToClient(loca);
            uint len = Math.Min(tile.Length + 1, 4);

            for (uint i = 0; i < len; i++)
            {
                if (pboxs[i].DisplayRectangle.Contains(
                    new Point(pt.X - pboxs[i].Left, pt.Y - pboxs[i].Top)))
                {
                    AddBuckle(buckle, i);
                    break;
                }
            }
        }

        public void Trans()
        {
            if (current != null)
            {
                int idx = -1;
                for (int i = 0; i < 4; i++)
                {
                    if (pboxs[i] == current)
                    {
                        idx = i;
                        break;
                    }
                }

                if (idx < 0 || idx > 3)
                    return;
                tile[idx].Product.Transpose(1);
                AddBuckle(tile[idx].Product, (uint)idx);
            }
            else
            {
                this.tile.Trans(1);
                this.DispTile();
            }
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            PictureBox box = sender as PictureBox;
            if (box == current)
                return;

            if (current != null)
            {
                current.Size = new Size(150, 150);
                current.Location = new Point(current.Left - 2, 
                    current.Top - 2);
            }

            current = box;
            current.Size = new Size(146, 146);
            current.Location = new Point(current.Left + 2, current.Top + 2);
        }

        private void mainPanel_Click(object sender, EventArgs e)
        {
            if (current != null)
            {
                current.Size = new Size(150, 150);
                current.Location = new Point(current.Left - 2, current.Top - 2);
            }
            current = null;
        }
    }
}
