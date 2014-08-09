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
    /// <summary>
    /// 扣板控件
    /// 主要处理扣板图片的加载、显示、缩放等操作
    /// 这里将电器也看作扣板来处理
    /// </summary>
    public partial class BuckleNode : UserControl
    {
        private CeilingDataSet.productsRow originalRow;

        public CeilingDataSet.productsRow OriginalRow
        {
            get { return originalRow; }
        }

        private Bitmap drawingImage;

        public Bitmap DrawingImage
        {
            get { return drawingImage; }
            set { drawingImage = value; }
        }

        private Bitmap originalImage;

        public Bitmap OriginalImage
        {
            get { return originalImage; }
        }

        private Rectangle imgRect = new Rectangle(15, 0, 70, 70);

        public Rectangle ImgRect
        {
            get { return imgRect; }
        }

        private bool loaded = false;

        private Rectangle nameRect = new Rectangle(0, 70, 100, 30);

        private List<BuckleImage> images = new List<BuckleImage>();

        public static string CreateDirect(string _base, string path)
        {
            string _path = _base;
            string[] _ps = path.Split('/');

            for (int i = 0; i < _ps.Length; i++)
            {
                _path += _ps[i] + "/";
                if (!System.IO.Directory.Exists(_path))
                    System.IO.Directory.CreateDirectory(_path);
            }

            return _path;
        }

        public static string DownloadPhoto(string path, string photo)
        {
            string _path = CreateDirect(ShareData.GetDataPath(), path);
            System.Net.WebClient client = new System.Net.WebClient();
            client.Headers.Add("user-agent", 
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            client.DownloadFile("http://" + ShareData.Server + "/AUPU/" + path + photo, 
                _path + photo);
            return _path + photo;
        }

        public static string PhotoPath(CeilingDataSet.product_classesRow crow)
        {
            string path = "";
            while (crow != null)
            {
                if (!crow.IsnameNull())
                    path = crow.name + "/" + path;
                crow = crow.product_classesRowParentByparent_class;
                //crow = crow.ParentRowByParent_class_id;
            }
            return path;
        }

        public static void UploadPhoto(CeilingDataSet.productsRow prow)
        {
            try
            {
                string path = BuckleNode.PhotoPath(prow.product_classesRow);
                string file = ShareData.GetDataPath() + path + prow.photo;
                string address = @"ftp://www.tianezhen.com/tmp/" + prow.photo;

                System.Net.WebClient client = new System.Net.WebClient();
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                client.UploadFile(address, file);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        public static void MovePhoto(CeilingDataSet.productsRow row)
        {
            try
            {
                string path = BuckleNode.PhotoPath(row.product_classesRow);
                string _url = "http://tianezhen.com/AUPU/move.php?file=" + row.photo + "&path=" + path;
                System.Net.WebRequest request = System.Net.WebRequest.Create(_url);
                request.GetResponse();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        public static void SavePhoto(CeilingDataSet.productsRow prow, Bitmap bmp)
        {
            try
            {
                string _path = CreateDirect(ShareData.GetDataPath(), 
                    BuckleNode.PhotoPath(prow.product_classesRow));
                string file = _path + prow.photo;
                bmp.Save(file);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        public static string LoadPhoto(CeilingDataSet.productsRow row)
        {
            if (row.IsphotoNull())
                return null;
            if (row.product_classesRow == null)
                return null;
            string _file, photo = row.photo, path = PhotoPath(row.product_classesRow);
            if (path == null || path.Length < 1)
                return null;

            try
            {
                _file = ShareData.GetFilePath(path + photo);
                if (_file == null || _file.Length < 1)
                    _file = DownloadPhoto(path, photo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                _file = null;
            }

            return _file;
        }

        public BuckleNode(CeilingDataSet.productsRow product)
        {
            InitializeComponent();
            this.originalRow = product;
            this.Name = product.name; //+ "(" + product.width.ToString() + "×" + product.height.ToString() + ")";
            this.ZoomPicture(this.originalRow.width, this.originalRow.height, 60);
        }

        private void ZoomPicture(float width, float height, int side)
        {
            if (width > height)
            {
                this.imgRect.Width = side;
                this.imgRect.Height = (int)(side * height / width);
            }
            else
            {
                this.imgRect.Width = (int)(side * width / height);
                this.imgRect.Height = side;
            }

            this.imgRect.X = (this.Width - this.imgRect.Width) / 2;
            this.imgRect.Y = (this.nameRect.Top - this.imgRect.Height) / 2;
        }

        public void AsynLoadImage()
        {
            if (this.originalRow == null)
                return;
            LoadPhotoHandler _loadImg = (row) => LoadPhoto(row);
            //Func<CeilingDataSet.productsRow, string> _loadImg = (row) => LoadPhoto(row);

            IAsyncResult asyncResult = _loadImg.BeginInvoke(this.originalRow, (result) =>
            {
                try
                {
                    string _imageFile = _loadImg.EndInvoke(result);
                    if (_imageFile != null && _imageFile.Length > 0)
                        this.originalImage = new Bitmap(_imageFile);
                    this.Invalidate();
                }
                catch (Exception ex)
                {
                    //this.originalRow = null;
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }, null);
        }

        public void SynLoadImage()
        {
            try
            {
                string imageFile = LoadPhoto(this.originalRow);
                if (imageFile != null && imageFile.Length > 0)
                    this.originalImage = new Bitmap(imageFile);
            }
            catch (Exception ex)
            {
                this.originalImage = null;
                System.Diagnostics.Debug.WriteLine(ex);
            }
            //this.originalImage = GetPreSetBitmap(this.originalRow);
        }

        private void ProductNode_Paint(object sender, PaintEventArgs e)
        {
            if (this.originalImage == null && !this.loaded)
            {
                AsynLoadImage();
                this.loaded = true;
            }

            if (this.drawingImage == null && this.originalImage != null)
                this.drawingImage = new Bitmap(this.originalImage, this.imgRect.Width, this.imgRect.Height);
            if (this.drawingImage != null)
            {
                e.Graphics.DrawImage(this.drawingImage, this.imgRect);
                e.Graphics.DrawRectangle(Pens.Black, this.imgRect);
            }

            SizeF size = e.Graphics.MeasureString(this.Name, this.Font);
            if (size.Width < this.nameRect.Width)
            {
                Point loca = new Point((int)((this.nameRect.Width - size.Width) / 2), this.nameRect.Top);
                e.Graphics.DrawString(this.Name, this.Font, brush, loca);
            }
            else
                e.Graphics.DrawString(this.Name, this.Font, brush, this.nameRect);
        }

        private void ProductNode_ForeColorChanged(object sender, EventArgs e)
        {
            brush = new SolidBrush(this.ForeColor);
        }

        SolidBrush brush = new SolidBrush(Color.Black);
        TextBox nBox = new TextBox();
        TextBox wBox = new TextBox();
        TextBox hBox = new TextBox();

        private bool isEditing = false;

        public bool IsEditing
        {
            get { return isEditing; }
        }

        public void Edit()
        {
            isEditing = true;
            Label label = new Label();

            nBox.Text = this.Name;
            nBox.Size = new Size(this.Width, 20);
            nBox.Location = new Point(0, this.nameRect.Y - 12);

            wBox.Text = this.originalRow.width.ToString();
            wBox.Size = new Size(this.Width / 2 - 10, 20);
            wBox.Location = new Point(0, this.nameRect.Y + 10);

            hBox.Text = this.originalRow.height.ToString();
            hBox.Size = new Size(this.Width / 2 - 10, 20);
            hBox.Location = new Point(this.Width / 2 + 10, this.nameRect.Y + 10);

            label.Text = "X";
            label.Size = new System.Drawing.Size(20, 20);
            label.Location = new Point(this.Width / 2 - 10, this.nameRect.Y + 10);
            label.BackColor = SystemColors.Control;
            label.ForeColor = Color.Black;

            this.Controls.Add(nBox);
            this.Controls.Add(wBox);
            this.Controls.Add(hBox);
            this.Controls.Add(label);

            nBox.BringToFront();
            wBox.BringToFront();
            hBox.BringToFront();
            label.BringToFront();

            nBox.Focus();
            nBox.TabIndex = 0;
            wBox.TabIndex = 1;
            hBox.TabIndex = 2;
        }

        public void UnEdit()
        {
            bool resize = false;

            if (this.Name != nBox.Text)
            {
                this.Name = nBox.Text;
                this.originalRow.name = nBox.Text;
            }
            if (this.originalRow.width.ToString() != wBox.Text)
            {
                this.originalRow.width = int.Parse(wBox.Text);
                resize = true;
            }
            if (this.originalRow.height.ToString() != hBox.Text)
            {
                this.originalRow.height = int.Parse(hBox.Text);
                resize = true;
            }

            if (resize)
            {
                this.drawingImage = null;
                this.ZoomPicture(this.originalRow.width, this.originalRow.height, 60);
            }

            isEditing = false;
            this.Controls.Clear();
        }
    }

    /// <summary>
    /// 扣板图片
    /// 主要处理图片的复用，避免出现同一个产品的图片多次加载，消耗内存
    /// 这里将电器也看作扣板来处理
    /// </summary>
    public class BuckleImage
    {
        public Bitmap bmp = null;

        private int count = 0;

        private List<BuckleImage> parent;

        public BuckleImage(List<BuckleImage> _parent)
        {
            parent = _parent;
            parent.Add(this);
        }

        public void Release()
        {
            this.count--;
            if (this.count < 1 && this.bmp != null)
            {
                this.bmp.Dispose();
                this.bmp = null;
            }
        }

        public void quote()
        {
            this.count++;
        }
    }
}
