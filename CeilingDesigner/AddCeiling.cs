using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CeilingDesigner
{
    public partial class AddCeiling : Form
    {
        private List<Wall> simpleWalles;
        private List<TextBox> boxes = new List<TextBox>();

        public List<Wall> Walles
        {
            get { return ceilSample1.Walles; }
        }

        public string CeilingName
        {
            get { return this.NameTextBox1.Text; }
        }

        public CeilSample Sample
        {
            get { return this.ceilSample1; }
        }

        public CeilingDataSet PalaceDataSet
        {
            get { return palaceDataSet; }
            set
            {
                palaceDataSet = value;
                this.ceilSample1.WallesTable = palaceDataSet.ceiling_sample_walles;
                this.ceilingsamplesBindingSource.DataSource = palaceDataSet;
            }
        }

        public TextBox DepthTextBox
        {
            get { return this.depthTextBox; }
        }

        public AddCeiling()
        {
            InitializeComponent();

            this.boxes.Add(this.ATextBox);
            this.boxes.Add(this.BTextBox);
            this.boxes.Add(this.CTextBox);
            this.boxes.Add(this.DTextBox);
            this.boxes.Add(this.ETextBox);
            this.boxes.Add(this.FTextBox);
            this.boxes.Add(this.GTextBox);
            this.boxes.Add(this.HTextBox);
        }

        //void TextBox_Leave(object sender, EventArgs e)
        //{
        //    TextBox box = sender as TextBox;
        //    Wall wall = box.Tag as Wall;

        //    if (wall == null)
        //        return;
        //    if (wall.Length.ToString() == box.Text)
        //        return;
        //    this.ceilSample1.Ceiling.Modify(wall, double.Parse(box.Text));

        //    //Rectangle rect1 = new Rectangle(
        //    //    (int)(this.ceilSample1.Width * 0.1), (int)(this.ceilSample1.Height * 0.1),
        //    //    (int)(this.ceilSample1.Width * 0.8), (int)(this.ceilSample1.Height * 0.8));
        //    RectangleF rect = new RectangleF(
        //        this.ceilSample1.Width * 0.1f, this.ceilSample1.Height * 0.1f,
        //        this.ceilSample1.Width * 0.8f, this.ceilSample1.Height * 0.8f);
        //    Rectangle rect1 = Rectangle.Round(rect);
        //    Rectangle rect2 = this.ceilSample1.Ceiling.DrawingRect;
        //    double sx = rect2.Width / (double)rect1.Width, 
        //        sy = rect2.Height / (double)rect1.Height;

        //    rect2 = rect1;
        //    if (sx > sy)
        //    {
        //        rect1.Height = (int)(sy * rect1.Width / sx);
        //        rect1.Y += (rect2.Height - rect1.Height) / 2;
        //    }
        //    else
        //    {
        //        rect1.Width = (int)(sx * rect1.Height / sy);
        //        rect1.X += (rect2.Width - rect1.Width) / 2;
        //    }
        //    this.ceilSample1.Ceiling.SetDrawingRect(rect1);
        //    this.ceilSample1.Invalidate();

        //    for (int i = 0; i < this.boxes.Count; i++)
        //    {
        //        Wall temp = this.boxes[i].Tag as Wall;
        //        if (temp == null) continue;
        //        if (this.boxes[i].Text != temp.Length.ToString())
        //            this.boxes[i].Text = temp.Length.ToString();
        //    }
        //}

        private Font lengthFont = new System.Drawing.Font("宋体", 14);

        private void LoadSample(CeilingDataSet.ceiling_samplesRow row)
        {
            if (row == null)
                return;
            this.ceilSample1.SampleName = row.name;
            this.ceilSample1.LoadSample(row.ID);
            
            for (int i = 0; i < this.boxes.Count; i++)
            {
                this.boxes[i].Enabled = false;
                this.boxes[i].Tag = null;
            }

            for (int i = 0; i < this.ceilSample1.Ceiling.Walles.Count 
                && i < this.boxes.Count; i++)
            {
                Wall wall = this.ceilSample1.Ceiling.Walles[i];
                this.boxes[i].Enabled = true;
                this.boxes[i].Tag = wall;
                this.boxes[i].Text = wall.Length.ToString();
            }

            this.simpleWalles = this.ceilSample1.Ceiling.Walles;
            this.NameTextBox1.Text = row.name + "房间";
            //this.NameTextBox1.Text = row.name + "未命名";
        }

        private void DrawStr(Graphics graphics, Wall wall, string str)
        {
            SizeF size = graphics.MeasureString(str, lengthFont);
            RectangleF rect = new RectangleF(
                (wall.BeginPaintPoint.X + wall.EndPaintPoint.X - size.Width) / 2, 
                (wall.BeginPaintPoint.Y + wall.EndPaintPoint.Y - size.Height) / 2, 
                size.Width, size.Height);

            // 水平方向向右
            if (wall.EndPaintPoint.X - wall.BeginPaintPoint.X > 0.1)
            {
                // 竖直向下
                if (wall.EndPaintPoint.Y - wall.BeginPaintPoint.Y > 0.1)
                {
                    rect.X += rect.Width / 2;
                    rect.Y -= rect.Height / 2;
                }
                // 竖直向上
                else if (wall.EndPaintPoint.Y - wall.BeginPaintPoint.Y < -0.1)
                {
                    rect.X -= rect.Width / 2;
                    rect.Y -= rect.Height / 2;
                }
                else
                {
                    rect.Y -= rect.Height / 2;
                }
            }
            // 水平方向向左
            else if (wall.EndPaintPoint.X - wall.BeginPaintPoint.X < -0.1)
            {
                if (wall.EndPaintPoint.Y - wall.BeginPaintPoint.Y > 0.1)
                {
                    rect.X += rect.Width / 2;
                    rect.Y += rect.Height / 2;
                }
                else if (wall.EndPaintPoint.Y - wall.BeginPaintPoint.Y < -0.1)
                {
                    rect.X -= rect.Width / 2;
                    rect.Y += rect.Height / 2;
                }
                else
                {
                    rect.Y += rect.Height / 2;
                }
            }
            else         // 竖直方向
            {
                if (wall.EndPaintPoint.Y - wall.BeginPaintPoint.Y > 0.1)
                {
                    rect.X += rect.Width / 2;
                }
                else if (wall.EndPaintPoint.Y - wall.BeginPaintPoint.Y < -0.1)
                {
                    rect.X -= rect.Width / 2;
                }
                else
                {
                    //throw new Exception("这是一个点");
                }
            }

            graphics.DrawString(str, lengthFont, Brushes.Black, rect);
        }

        private void ceilSample1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < this.simpleWalles.Count; i++)
            {
                this.DrawStr(e.Graphics, this.simpleWalles[i], ((char)(i + 'A')).ToString());
            }
        }

        private bool _checkClose()
        {
            if (this.Walles.Count < 3)
                return false;
            Wall wb = this.Walles[0], we = this.Walles[this.Walles.Count - 1];
            PointF _inter = we.Intersect(wb);

            wb.Begin = _inter;
            wb.BeginPaintPoint = new Point((int)(_inter.X / 10), (int)(_inter.Y / 10));
            we.End = _inter;
            we.EndPaintPoint = wb.BeginPaintPoint;

            return true;
        }

        private void generate(Wall wall1, Wall wall2, int length)
        {
            if (wall2.End.X > wall2.Begin.X)
            {
                wall1.End = new PointF(wall1.Begin.X + length, wall1.Begin.Y);
                wall1.EndPaintPoint = new Point(wall1.BeginPaintPoint.X + length / 10,
                    wall1.BeginPaintPoint.Y);
            }
            else if (wall2.End.Y > wall2.Begin.Y)
            {
                wall1.End = new PointF(wall1.Begin.X, wall1.Begin.Y + length);
                wall1.EndPaintPoint = new Point(wall1.BeginPaintPoint.X,
                    wall1.BeginPaintPoint.Y + length / 10);
            }
            else if (wall2.End.X < wall2.Begin.X)
            {
                wall1.End = new PointF(wall1.Begin.X - length, wall1.Begin.Y);
                wall1.EndPaintPoint = new Point(wall1.BeginPaintPoint.X - length / 10,
                    wall1.BeginPaintPoint.Y);
            }
            else if (wall2.End.Y < wall2.Begin.Y)
            {
                wall1.End = new PointF(wall1.Begin.X, wall1.Begin.Y - length);
                wall1.EndPaintPoint = new Point(wall1.BeginPaintPoint.X,
                    wall1.BeginPaintPoint.Y - length / 10);
            }
        }

        private Wall _add(PointF begin, Point beginPaint, int index)
        {
            Wall wall = new Wall();
            wall.Wallnum = index;
            wall.Begin = begin;
            wall.BeginPaintPoint = beginPaint;

            this.generate(wall, this.boxes[index].Tag as Wall,
                int.Parse(this.boxes[index].Text));
            this.Walles.Add(wall);

            return wall;
        }

        private void OKbutton1_Click(object sender, EventArgs e)
        {
            try
            {
                Wall temp, wall;

                int.Parse(this.depthTextBox.Text);
                this.Walles.Clear();

                wall = _add(new PointF(1000, 1000), new Point(100, 100), 0);

                for (int i = 1; i < this.boxes.Count; i++)
                {
                    if (!this.boxes[i].Enabled)
                        break;
                    temp = wall;
                    wall = _add(temp.End, temp.EndPaintPoint, i);
                }

                if (!_checkClose())
                {
                    MessageBox.Show("输入的各边参数不匹配", ShareData.AppName);
                    return;
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch 
            {
                MessageBox.Show("输入有误，边长和夹高只能输入数字！",
                    ShareData.AppName);
            }
        }

        private void CancelButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void focusTextBox(int index)
        {
            if (this.NameTextBox1.TabIndex == index)
            {
                this.NameTextBox1.Focus();
                return;
            }
            if (this.depthTextBox.TabIndex == index)
            {
                this.depthTextBox.Focus();
                return;
            }

            for (int i = 0; i < this.boxes.Count; i++)
            {
                if (this.boxes[i].TabIndex == index)
                {
                    this.boxes[i].Focus();
                    break;
                }
            }
        }

        private void ATextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                TextBox tb = sender as TextBox;
                this.focusTextBox(tb.TabIndex - 1);
            }
            else if (e.KeyCode == Keys.Down)
            {
                TextBox tb = sender as TextBox;
                this.focusTextBox(tb.TabIndex + 1);
            }
        }

        private void AddCeiling_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.comboBox1.Items.Count > 0)
                    this.LoadSample(((DataRowView)this.comboBox1.SelectedItem).Row 
                        as CeilingDataSet.ceiling_samplesRow);
                else
                    MessageBox.Show("没有数据源，可能正在连接服务器，或服务器连接失败。\n请稍后再试...",
                        ShareData.AppName);
            }
            catch
            {
                MessageBox.Show("没有数据源，可能正在连接服务器，或服务器连接失败。\n请稍后再试...",
                    ShareData.AppName);
                
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedItem == null)
                return;
            this.LoadSample(((DataRowView)this.comboBox1.SelectedItem).Row 
                as CeilingDataSet.ceiling_samplesRow);
        }
    }
}

//private void _Add(Wall wall1, Wall wall2, int index, uint length)
//{
//    wall1.Begin = new PointF(1000, 1000);
//    wall1.BeginPaintPoint = new Point(100, 100);
//    wall1.Wallnum = index;

//    this.generate(wall1, wall2, length);
//    this.Walles.Add(wall1);
//}

//wall1 = new Wall();
//wall1.Wallnum = 0;
//wall1.Begin = new PointF(1000, 1000);
//wall1.BeginPaintPoint = new Point(100, 100);

////wall2 = this.boxes[0].Tag as Wall;
////this.generate(wall1, wall2, int.Parse(this.boxes[0].Text));

//this.generate(wall1, this.boxes[0].Tag as Wall, 
//    int.Parse(this.boxes[0].Text));
//this.Walles.Add(wall1);

//for (int i = 1; i < this.boxes.Count; i++)
//{
//    if (!this.boxes[i].Enabled)
//        break;
//    temp = wall;

//    wall1 = new Wall();
//    wall1.Wallnum = i;
//    wall1.Begin = temp.End;
//    wall1.BeginPaintPoint = temp.EndPaintPoint;

//    //wall2 = this.boxes[i].Tag as Wall;
//    //this.generate(wall1, wall2, int.Parse(this.boxes[i].Text));

//    this.generate(wall1, this.boxes[i].Tag as Wall,
//        int.Parse(this.boxes[i].Text));
//    this.Walles.Add(wall1);
//}

//this.ATextBox.Leave += new EventHandler(TextBox_Leave);
//this.BTextBox.Leave += new EventHandler(TextBox_Leave);
//this.CTextBox.Leave += new EventHandler(TextBox_Leave);
//this.DTextBox.Leave += new EventHandler(TextBox_Leave);
//this.ETextBox.Leave += new EventHandler(TextBox_Leave);
//this.FTextBox.Leave += new EventHandler(TextBox_Leave);
//this.GTextBox.Leave += new EventHandler(TextBox_Leave);
//this.HTextBox.Leave += new EventHandler(TextBox_Leave);

//wall1 = this.Walles[0]; wall2 = this.Walles[this.Walles.Count - 1];
//if (Math.Abs(wall1.Begin.X - wall2.End.X) > 10 || Math.Abs(wall1.Begin.Y - wall2.End.Y) > 10)
//{
//    MessageBox.Show("输入的各边参数不匹配", Palace.Properties.Settings.Default.AppName);
//    return;
//}

//int last = this.Walles.Count - 1;
//if (Math.Abs(this.Walles[0].Begin.X - this.Walles[last].Begin.X) > 10
//    || Math.Abs(this.Walles[0].Begin.Y - this.Walles[last].Begin.Y) > 10)
//    throw new Exception("输入的各边参数不匹配");

//int.Parse(this.ATextBox.Text);
//int.Parse(this.BTextBox.Text);
//int.Parse(this.CTextBox.Text);
//int.Parse(this.DTextBox.Text);
//int.Parse(this.ETextBox.Text);
//int.Parse(this.FTextBox.Text);
//int.Parse(this.GTextBox.Text);
//int.Parse(this.HTextBox.Text);

//this.mainWalls.Clear();
//for (int i = 0; i < this.ceilSample1.Ceiling.Walles.Count; i++)
//{
//    Wall wall = this.ceilSample1.Ceiling.Walles[i];
//    if (this.mainWalls.Count >= 8)
//        continue;
//    this.boxes[this.mainWalls.Count].Enabled = true;
//    this.boxes[this.mainWalls.Count].Tag = wall;
//    this.boxes[this.mainWalls.Count].Text = wall.Length.ToString();
//    this.mainWalls.Add(wall);
//}

//private void AddCeilingGraphForm_Load(object sender, EventArgs e)
//{
//    try
//    {
//        if (this.comboBox1.Items.Count > 0)
//            this.LoadSample(((DataRowView)this.comboBox1.SelectedItem).Row as palaceDataSet.ceiling_samplesRow);
//        else
//            MessageBox.Show("没有数据源，可能正在连接服务器，或服务器连接失败。\n请稍后再试...");
//    }
//    catch
//    {
//        MessageBox.Show("没有数据源，可能正在连接服务器，或服务器连接失败。\n请稍后再试...");
//    }
//}
