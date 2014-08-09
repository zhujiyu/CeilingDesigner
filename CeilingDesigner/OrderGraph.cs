using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CeilingDesigner
{
    public partial class OrderGraph : UserControl
    {
        #region 属性表

        private Stack<Revocation> revocations = new Stack<Revocation>();
        private Stack<Revocation> redos = new Stack<Revocation>();

        public int UndoCount
        {
            get { return revocations.Count; }
        }

        public int RedoCount
        {
            get { return redos.Count; }
        }

        private ToolStripStatusLabel positionStatusLabel;
        private ToolTip toolTip = null;

        public ToolTip ToolTip
        {
            get { return toolTip; }
            set { toolTip = value; }
        }

        private Keel keel = null;

        /// <summary>
        /// 正在绘制的边
        /// </summary>
        private Wall cWall = null;

        private Wall selectSZoneWall = null;
        private SZone szone = null;
        private SZone _selectSZone = null;

        private List<SZone> sZones = new List<SZone>();

        public List<SZone> SZones
        {
            get { return sZones; }
        }

        public Keel SelectedKeel
        {
            get { return keel; }
        }

        /// <summary>
        /// 选中的边的序号，从0开始
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                if (this.ceiling != null)
                    return this.ceiling.SelectedIndex;
                else
                    return -1;
            }
        }

        /// <summary>
        /// 选中的边
        /// </summary>
        public Wall SelectedWall
        {
            get
            {
                if (this.ceiling != null)
                    return this.ceiling.SelectedWall;
                else
                    return null;
            }
        }

        private int moving = 0;      // 标识移动的是边上的哪一个点，或者移动整个图形
        private Rectangle movingRect;
        private Point mouseDownPoint, mouseLocation;

        private bool ctrlDown = false;
        private bool draft = false;
        private bool keelFilling = false;
        private bool wallFuzhu = false;

        private Pen grid1Pen = new Pen(Color.FromArgb(255, 185, 185, 185), 1);
        private Pen grid2Pen = new Pen(Color.FromArgb(255, 215, 215, 215), 1);
        private Pen arrowPen = new Pen(Color.FromArgb(255, 88, 88, 88), 1);
        private Pen dottedPen = new Pen(Color.DimGray, 2);
        private Pen mainKeelPen = new Pen(Color.FromArgb(255, 5, 5, 5), 4);
        private Pen selectedPen = new Pen(Color.GreenYellow, 2);

        private Font lengthFont = new Font("宋体", 10);
        private StringFormat strFormat = new StringFormat(StringFormatFlags.DirectionVertical);

        private bool changed = false;

        public bool Changed
        {
            get { return changed; }
        }

        private Order order = null;

        public Order Order
        {
            get { return order; }
        }

        private int space = 25;

        public int Space
        {
            get { return space; }
            set
            {
                if (space == value)
                    return;
                space = value;

                if (grid)
                {
                    if (this.BackgroundImage != null)
                        this.BackgroundImage.Dispose();
                    this.BackgroundImage = GridBackImage();
                    this.Invalidate();
                }
            }
        }

        private bool grid = true;

        public bool Grid
        {
            get { return grid; }
            set
            {
                if (grid == value)
                    return;
                grid = value;
                if (this.BackgroundImage != null)
                    this.BackgroundImage.Dispose();
                if (grid)
                    this.BackgroundImage = GridBackImage();
                else
                    this.BackgroundImage = null;
                this.Invalidate();
            }
        }

        private Rectangle sampleRect = new Rectangle();

        public Rectangle SampleRect
        {
            get { return sampleRect; }
            set { sampleRect = value; }
        }

        private Ceiling ceiling = new Ceiling();

        public Ceiling Ceiling
        {
            get { return ceiling; }
        }

        private CeilingDataSet.ceilingsRow ceilingRow = null;

        public CeilingDataSet.ceilingsRow CeilingRow
        {
            get { return ceilingRow; }
            set { ceilingRow = value; }
        }

        private ProductSet productSet = null;

        public ProductSet ProductSet
        {
            get { return productSet; }
        }

        private ModifyKeelRevocationEventArgs keelArg = null;
        private ModifyWallRevocationEventArgs wallArg = null;
        private ModifySZoneRevocationEventArgs szoneArg = null;
        ModifySZoneWallRevocationEventArgs szoneWallArg = null;

        #endregion

        public OrderGraph(Order _orderData)
        {
            InitializeComponent();
            _orderData.OrderGraphs.Add(this);
            this.order = _orderData;
            this.productSet = new ProductSet(this);
            this.InitGraph();
        }

        private Bitmap GridBackImage()
        {
            int i, width = this.space * 40, height = this.space * 40, 
                step = this.space * 5;
            Bitmap bmp = new Bitmap(width, height);
            Graphics graphic = Graphics.FromImage(bmp);

            for (i = 1; i < 40; i++)
                graphic.DrawLine(grid2Pen, this.space * i, 0, this.space * i, height);
            for (i = 1; i < 40; i++)
                graphic.DrawLine(grid2Pen, 0, this.space * i, width, this.space * i);

            for (i = 0; i < 8; i++)
                graphic.DrawLine(grid1Pen, step * i, 0, step * i, height);
            for (i = 0; i < 8; i++)
                graphic.DrawLine(grid1Pen, 0, step * i, width, step * i);

            graphic.Dispose();

            return bmp;
        }

        public void SetName(string name)
        {
            this.ceiling.Name = name;
            (this.Tag as ToolStripMenuItem).Text = name;
        }

        private void InitGraph()
        {
            this.grid2Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
            //this.dottedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            this.BackgroundImage = GridBackImage();
            this.MouseWheel += new MouseEventHandler(Graph_MouseWheel);
        }

        private bool InitCeiling()
        {
            if (this.ceiling.Length > 0
                && MessageBox.Show("房顶平面图数据已存在，是否重新设计？",
                ShareData.AppName, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return false;
            }
            else
            {
                OrderGraphRevocationEventArgs arg = Revocation.GetRevoGraph(
                    this.productSet, this.ceiling);
                this.AddUndo(arg, Revocation.ChangeGraph);

                if (this.productSet == null)
                    this.productSet = new ProductSet(this);
                else
                    this.productSet.Clear();
                this.ceiling.Clear();

                this.Invalidate();
            }
            return true;
        }

        public bool InitFromSample(List<Wall> walles, string name, 
            System.UInt32 depth)
        {
            if (this.order == null || this.ceiling == null 
                || walles == null)
                return false;
            if (this.InitCeiling() == false)
                return false;

            this.ceiling.Depth = depth;
            if (this.ceiling.Name.Length == 0
                && ceiling.Name.CompareTo(name) != 0)
                this.SetName(name);
            this.productSet.KeelSet.CeilingDepth = depth;

            for (int i = 0; i < walles.Count; i++)
            {
                Wall wall = walles[i].Clone();
                this.ceiling.AddWall(wall);
            }
            this.DisplayCeiling();

            ToolStripMenuItem item = this.Tag as ToolStripMenuItem;
            if (item != null)
                item.Text = this.ceiling.Name;
            (this.ParentForm as PalaceForm).RefrushText(this);
            (this.ParentForm as PalaceForm).SetCeilingMenu(true);
            (this.ParentForm as PalaceForm).SetAlignMenu(this.productSet.Align);

            return true;
        }

        public void InvalidateKeel(Keel _keel)
        {
            Rectangle rect = new Rectangle(0, 0, 100, 20);
            
            rect.X = _keel.BeginPaintPoint.X;
            rect.Y = _keel.BeginPaintPoint.Y - 20;
            this.Invalidate(rect);
            rect.X = _keel.EndPaintPoint.X;
            rect.Y = _keel.EndPaintPoint.Y - 20;
            this.Invalidate(rect);

            if (_keel.Remark.PaintRect.Height > 0)
                this.Invalidate(_keel.Remark.PaintRect);
            this.Invalidate(_keel.PaintRect);
        }

        private void SetSelectKeel(Keel _keel)
        {
            PalaceForm pfrm = this.ParentForm as PalaceForm;
            pfrm.SetEditMenu(false);
            pfrm.SetDeleteMenu(true);
            pfrm.SetPasteMenu(false);
            pfrm.SetDoMenu(false);

            WallEdit wallEdit = (this.ParentForm as PalaceForm).WallEdit;
            wallEdit.SetData(_keel);
            wallEdit.ShowAllGroups();
            wallEdit.Show();

            //wallEdit.HideRemarkGroup();
            wallEdit.LengthTextbox.Enabled = true;
            wallEdit.BeginTextbox.Enabled = true;
            wallEdit.EndTextbox.Enabled = true;
            wallEdit.OffsetTextbox.Enabled = true;
            wallEdit.RemarkTextBox.Enabled = true;
            wallEdit.DepthTextbox.Enabled = false;

            //this.Focus();
            InvalidateKeel(_keel);
        }

        public void SelectKeel(int index)
        {
            if (this.productSet == null)
                return;
            List<Keel> keels = this.productSet.KeelSet.MainKeelList;
            if (keels.Count == 0 || (this.keel == null && index < 0) 
                || keels.IndexOf(this.keel) == index)
                return;
            PalaceForm pfrm = this.ParentForm as PalaceForm;

            if (this.keel != null)
            {
                if (keelArg != null)
                {
                    if (keelArg.begin != this.keel.Begin || keelArg.end != this.keel.End
                        || keelArg.depth != this.keel.Depth
                        || keelArg.remark.CompareTo(this.keel.RemarkStr) != 0)
                        AddUndo(keelArg, Revocation.ModifyKeel);
                    keelArg = null;
                }

                pfrm.SetDoMenu(this);
                pfrm.WallEdit.UnSelect();
                this.InvalidateKeel(this.keel);
            }

            this.keel = this.productSet.KeelSet.GetKeel(index);
            if (this.keel == null)
                return;
            this.SetSelectKeel(this.keel);
            pfrm.WallEdit.BeginTextbox.Enabled = false;
            pfrm.WallEdit.EndTextbox.Enabled = false;
            pfrm.WallEdit.HideOffsetGroup();
            pfrm.WallEdit.LengthTextbox.Enabled = false;

            keelArg = new ModifyKeelRevocationEventArgs();
            keelArg.keel = this.keel;
            keelArg.begin = this.keel.Begin;
            keelArg.end = this.keel.End;
            keelArg.depth = this.keel.Depth;
            keelArg.remark = this.keel.RemarkStr;
        }
        
        // && keels.IndexOf(this.keel) != index)//index < 0)
        // && this.ceiling.SelectedIndex != index)
        //pfrm.SetDoMenu(false);
        //pfrm.SetEditMenu(true);
        //pfrm.SetPasteMenu(false);

        public void SelectWall(int index)
        {
            if (this.ceiling == null || this.ceiling.Length < 1)
                return;
            if (this.ceiling.SelectedIndex == index)
                return;
            Wall wall = this.ceiling.SelectedWall;
            PalaceForm pfrm = this.ParentForm as PalaceForm;

            if (wall != null)
            {
                if (wallArg != null)
                {
                    if (wallArg.begin != wall.Begin || wallArg.end != wall.End
                        || wallArg.remark.CompareTo(wall.RemarkStr) != 0)
                        AddUndo(wallArg, Revocation.ModifyWall);
                    wallArg = null;
                }

                pfrm.SetDoMenu(this);
                pfrm.WallEdit.UnSelect();
                this.InvalidateKeel(wall);
            }

            try
            {
                this.ceiling.SelectedIndex = index;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("错误信息："
                    + ex.Message + "\n" + ex.ToString());
                return;
            }

            if (this.ceiling.SelectedIndex < 0)
                return;
            wall = this.ceiling.SelectedWall;
            this.SetSelectKeel(wall);
            pfrm.WallEdit.HideRemarkGroup();

            wallArg = new ModifyWallRevocationEventArgs();
            wallArg.ceiling = this.ceiling;
            wallArg.wall = wall;
            wallArg.begin = wall.Begin;
            wallArg.end = wall.End;
            wallArg.remark = wall.RemarkStr;
        }

        private void SelectSZoneWall(Wall _wall)
        {
            if (sZones.Count == 0 || selectSZoneWall == _wall)
                return;
            PalaceForm pfrm = this.ParentForm as PalaceForm;
            
            if (selectSZoneWall != null)// && _wall == null)
            {
                if (szoneWallArg != null)
                {
                    if (szoneWallArg.begin != selectSZoneWall.Begin
                        || szoneWallArg.end != selectSZoneWall.End
                        || szoneWallArg.remark.CompareTo(selectSZoneWall.RemarkStr) != 0)
                        AddUndo(szoneWallArg, Revocation.ModifySZoneWall);
                    szoneWallArg = null;
                }

                pfrm.SetDoMenu(this);
                pfrm.WallEdit.UnSelect();
                _selectSZone = null;
                this.InvalidateKeel(selectSZoneWall);
            }

            this.selectSZoneWall = _wall;
            if (this.selectSZoneWall == null)
                return;

            for (int i = 0; i < sZones.Count; i++)
            {
                if (sZones[i].Walles.IndexOf(_wall) >= 0)
                    _selectSZone = sZones[i];
            }
            this.SetSelectKeel(this.selectSZoneWall);
            pfrm.SetDeleteMenu(false);
            pfrm.SetDoMenu(false);
            pfrm.WallEdit.HideOffsetGroup();

            szoneWallArg = new ModifySZoneWallRevocationEventArgs();
            szoneWallArg.szone = _selectSZone;
            szoneWallArg.wall = selectSZoneWall;
            szoneWallArg.remark = selectSZoneWall.RemarkStr;

            szoneWallArg.begin = selectSZoneWall.Begin;
            szoneWallArg.end = selectSZoneWall.End;

            int index = _selectSZone.Walles.IndexOf(selectSZoneWall);
            if (index > 0)
                szoneWallArg.prev = _selectSZone.Walles[index - 1].Begin;
            if (index < _selectSZone.Walles.Count - 1)
                szoneWallArg.next = _selectSZone.Walles[index + 1].End;
        }

        private void SelectZone(int index)
        {
            if (this.sZones == null || this.sZones.Count == 0)
                return;
            PalaceForm pfrm = this.ParentForm as PalaceForm;
            WallEdit wallEdit = pfrm.WallEdit;

            if (index < 0 && this.szone != null)
            {
                if (szoneArg != null)
                {
                    if (szoneArg.depth != this.szone.Depth
                        || szoneArg.remark.CompareTo(this.szone.RemarkStr) != 0)
                        AddUndo(szoneArg, Revocation.ModifySZone);
                    szoneArg = null;
                }

                wallEdit.UnSelect();
                pfrm.SetDoMenu(this);
                this.Invalidate();
            }

            if (index < 0 || this.sZones.Count <= index)
            {
                this.szone = null;
                return;
            }

            if (this.szone != null && this.sZones.IndexOf(this.szone) == index)
                return;
            this.szone = this.sZones[index];
            pfrm.SetDoMenu(false);
            pfrm.SetDeleteMenu(true);

            wallEdit.SetData(szone);
            wallEdit.ShowAllGroups();
            wallEdit.HideLengthGroup();
            wallEdit.HideOffsetGroup();
            wallEdit.Show();

            wallEdit.BeginTextbox.Enabled = false;
            wallEdit.EndTextbox.Enabled = false;
            wallEdit.OffsetTextbox.Enabled = false;
            wallEdit.LengthTextbox.Enabled = false;
            wallEdit.DepthTextbox.Enabled = true;
            wallEdit.RemarkTextBox.Enabled = true;

            szoneArg = new ModifySZoneRevocationEventArgs();
            szoneArg.szone = this.szone;
            szoneArg.depth = this.szone.Depth;
            szoneArg.remark = this.szone.RemarkStr;

            this.Invalidate();
        }

        public void Change()
        {
            (this.ParentForm as PalaceForm).Change();
            this.changed = true;
        }

        /// <summary>
        /// 将实际尺寸的吊顶图映射到屏幕显示
        /// </summary>
        /// <param name="oriRect">吊顶图的实际尺寸</param>
        /// <returns>屏幕显示区域</returns>
        private Rectangle ParseDrawingRect(RectangleF oriRect)
        {
            double x = oriRect.Width / oriRect.Height, 
                y = this.Width / (this.Height - 100.0);
            double scale = oriRect.Width > oriRect.Height ? 0.6 : 0.7;
            Rectangle rect = new Rectangle();

            if (x < y)
            {
                rect.Height = (int)((this.Height - 100.0) * scale);
                rect.Width = (int)(rect.Height * x);
            }
            else
            {
                rect.Width = (int)(this.Width * scale);
                rect.Height = (int)(rect.Width / x);
            }

            rect.X = (this.Width - rect.Width) / 2;
            rect.Y = (this.Height - rect.Height - 100) / 2;

            return rect;
        }

        private void RefrushGraph(Rectangle rect)
        {
            this.ceiling.SetDrawingRect(rect);
            //this.ceiling.RefrushRegion();

            if (this.productSet != null)
            {
                this.productSet.GenerateActualRegion();
                this.productSet.GenerateDrawingRegion();
                this.productSet.BuckleSet.ReTileProducts();
                this.productSet.KeelSet.Refrush();
            }

            for (int i = 0; i < sZones.Count; i++)
            {
                sZones[i].Refrush(ceiling);
            }
        }

        public OrderGraph DisplayCeiling()
        {
            this.RefrushGraph(ParseDrawingRect(this.ceiling.GetRect( )));
            (this.ParentForm as PalaceForm).ScaleLabel.Text = "100%";
            return this;
        }

        public void AdjustCeiling()
        {
            for (int i = 0; i < sZones.Count; i++)
            {
                sZones[i].Fill(this.ceiling);
            }

            if (this.productSet != null)
            {
                this.productSet.GenerateActualRegion();
                this.productSet.GenerateDrawingRegion();
                this.productSet.BuckleSet.ReTileProducts();

                List<Keel> ks = this.productSet.KeelSet.MainKeelList;
                for (int i = 0; i < ks.Count; i++)
                {
                    ks[i].Fill(this.ceiling);
                }
            }
        }

        private void Graph_MouseDown(object sender, MouseEventArgs e)
        {
            this.mouseLocation = e.Location;
            this.mouseDownPoint = e.Location;

            if (this.ceiling == null || this.ceiling.Length < 1)
                return;
            if (this.keelFilling || this.wallFuzhu || this.draft)
                return;
            int index = -1;
            PalaceForm pfrm = this.ParentForm as PalaceForm;

            try
            {
                if (this.SelectedWall != null)
                {
                    Wall _wall = this.SelectedWall;

                    if (_wall.BeginAnchor.Contains(e.Location))
                        moving = 1;
                    else if (_wall.CenterAnchor.Contains(e.Location))
                        moving = 2;
                    else if (_wall.EndAnchor.Contains(e.Location))
                        moving = 3;
                    else
                        moving = 0;
                    if (moving > 0)
                        return;
                }
                else if (this.keel != null)
                {
                    if (this.keel.BeginAnchor.Contains(e.Location))
                        moving = 1;
                    else if (this.keel.CenterAnchor.Contains(e.Location))
                        moving = 2;
                    else if (this.keel.EndAnchor.Contains(e.Location))
                        moving = 3;
                    else
                        moving = 0;
                    if (moving > 0)
                        return;
                }

                if (szone != null)
                {
                    if (szone.Remark.DispRect.Contains(e.Location))
                        return;
                    SelectZone(-1);
                }
                else if (this.selectSZoneWall != null)
                {
                    if (Keel.IsInLine(this.selectSZoneWall.PaintLine, 
                        e.Location))
                        return;
                    this.SelectSZoneWall(null);
                }
                else if (this.keel != null)
                {
                    if (Keel.IsInLine(this.keel.PaintLine, e.Location))
                        return;
                    this.SelectKeel(-1);
                }
                else if (this.SelectedWall != null)
                {
                    if (Keel.IsInLine(this.SelectedWall.PaintLine, e.Location))
                        return;
                    this.SelectWall(-1);
                }
                pfrm.SetEditMenu(false);
                pfrm.SetPasteMenu(ShareData.list.Count > 0);

                index = this.ceiling.GetWall(e.Location);
                if (index > -1)
                {
                    this.SelectWall(index);
                    //pfrm.SetEditMenu(true);
                    return;
                }

                for (int i = 0; i < sZones.Count; i++)
                {
                    if (sZones[i].Remark.DispRect.Contains(e.Location))
                    {
                        SelectZone(i);
                        return;
                    }

                    Wall _wall = sZones[i].GetWall(e.Location);
                    if (_wall != null)
                    {
                        this.SelectSZoneWall(_wall);
                        return;
                    }
                }

                if (this.productSet != null)
                {
                    index = this.productSet.KeelSet.GetMainKeel(e.Location);
                    if (index > -1)
                    {
                        this.SelectKeel(index);
                        //pfrm.SetEditMenu(true);
                        return;
                    }

                    BuckleSet buckleset = this.productSet.BuckleSet;
                    List<AupuBuckle> sps = buckleset.SelectProducts;
                    if (sps.Count > 0
                        && buckleset.SelectRectangle().Contains(e.Location) 
                        && e.Button == MouseButtons.Right)
                    {
                        //pfrm.SetEditMenu(true);
                        //pfrm.SetPasteMenu(false);
                        return;
                    }

                    if (!this.ctrlDown && sps.Count > 0)
                    {
                        for (int i = 0; i < sps.Count; i++)
                        {
                            if (sps[i].DrawingRect.Contains(e.Location))
                            {
                                this.movingRect = buckleset.SelectRectangle();
                                this.moving = 5;
                                pfrm.SetEditMenu(true);
                                pfrm.SetPasteMenu(false);
                                return;
                            }
                        }

                        InvalidateRect(buckleset.SelectRectangle());
                        buckleset.CancelSelect();
                    }

                    if (buckleset.Select(e.Location) != null)
                    {
                        pfrm.SetEditMenu(true);
                        pfrm.SetPasteMenu(false);
                        moving = 5;
                        this.movingRect = buckleset.SelectRectangle();
                        this.InvalidateRect(this.movingRect);
                        return;
                    }
                }

                Rectangle rect = this.ceiling.DrawingRect, r1 = rect;
                r1.X -= 40; r1.Y -= 40; r1.Width = 40; r1.Height = 40;
                Rectangle r2 = r1, r3 = r1, r4 = r1;
                r2.X = rect.Right; r2.Y = rect.Bottom;
                r3.X = rect.Left - 40; r3.Y = rect.Bottom;
                r4.X = rect.Right; r4.Y = rect.Top - 40;

                if (r1.Contains(e.Location) || r2.Contains(e.Location) 
                    || r3.Contains(e.Location) || r4.Contains(e.Location))
                {
                    moving = 4;
                    if (this.productSet != null)
                        this.productSet.BuckleSet.CancelSelect();
                    this.movingRect = this.ceiling.DrawingRect;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private void PositionTip(Keel _keel)
        {
            Rectangle rect = this.ceiling.DrawingRect;

            if (this.ceiling.Walles.Count == 0)
            {
                rect.Size = new Size(0, 0);
                rect.Location = _keel.BeginPaintPoint;
            }

            this.positionStatusLabel.Text = rect.Width + "×" + rect.Height + "  "
                + (_keel.EndPaintPoint.X - rect.Left) + "×" + (_keel.EndPaintPoint.Y - rect.Top);
        }

        private void DraftWall(Keel _keel, Point pt)
        {
            Point delta = new Point(pt.X - _keel.BeginPaintPoint.X,
                pt.Y - _keel.BeginPaintPoint.Y);

            if (Math.Abs(delta.X) * 1.7 < Math.Abs(delta.Y))
            {
                delta.X = 0;
            }
            else if (Math.Abs(delta.X) > Math.Abs(delta.Y) * 1.7)
            {
                delta.Y = 0;
            }
            else
            {
                int x = (Math.Abs(delta.X) + Math.Abs(delta.Y)) / 2;
                delta.X = delta.X > 0 ? x : -x;
                delta.Y = delta.Y > 0 ? x : -x;
            }

            _keel.EndPaintPoint = new Point(_keel.BeginPaintPoint.X + delta.X,
                _keel.BeginPaintPoint.Y + delta.Y);

            if (Math.Abs(_keel.Vector.X) < 1)
            {
                // 竖直线变水平线或斜线
                if (Math.Abs(delta.X) > 0.01)
                    this.Invalidate();
            }
            else if (Math.Abs(_keel.Vector.Y) < 1)
            {
                // 水平线变竖直线或者斜线
                if (Math.Abs(delta.Y) > 0.01)
                    this.Invalidate();
            }
            else if (Math.Abs(_keel.Vector.Y - _keel.Vector.X) < 1)
            {
                // 由一、三象限斜线改变方向
                if (Math.Abs(delta.X - delta.Y) > 0.01)
                    this.Invalidate();
            }
            else if (Math.Abs(_keel.Vector.Y + _keel.Vector.X) < 1)
            {
                // 由二、四象限斜线改变方向
                if (Math.Abs(delta.X + delta.Y) > 0.01)
                    this.Invalidate();
            }
        }

        private void Graph_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.ceiling == null || this.productSet == null)
                return;
            Point delta = new Point(e.X - this.mouseLocation.X, 
                e.Y - this.mouseLocation.Y);
            if (Math.Abs(delta.X) + Math.Abs(delta.Y) < 3)
                return;
            this.mouseLocation = e.Location;

            try
            {
                if (!this.draft && !this.keelFilling && !this.wallFuzhu 
                    && this.moving == 0 && this.toolTip != null)
                {
                    if (this.ceiling.Contain(e.Location))
                    {
                        string str = "吊顶区域";

                        if (this.ceiling.GetWall(e.Location) > -1)
                        {
                            str = "墙壁";
                        }
                        else if (this.productSet != null)
                        {
                            AupuBuckle p = this.productSet.BuckleSet
                                .CoverProduct(e.Location);
                            if (p != null)
                                str = p.Name;
                            if (this.productSet.KeelSet.GetMainKeel(e.Location) > -1)
                                str = "主龙骨";
                        }

                        this.Cursor = Cursors.Hand;
                        this.toolTip.Show(str, this, e.X, e.Y - 20);
                    }
                    else
                    {
                        this.toolTip.Hide(this);
                        this.Cursor = Cursors.Default;
                    }
                }

                if (this.draft && this.cWall != null)
                {
                    this.DraftWall(this.cWall, e.Location);
                    this.cWall.End = new PointF(this.cWall.EndPaintPoint.X * 10,
                        this.cWall.EndPaintPoint.Y * 10);
                    this.InvalidateRect(this.cWall.PaintRect, 30);
                    this.PositionTip(this.cWall);
                }
                else if (this.wallFuzhu && this.cWall != null)
                {
                    this.DraftWall(this.cWall, e.Location);
                    this.cWall.End = this.ceiling.GetLogicCoord(this.cWall.EndPaintPoint);
                    this.PositionTip(this.cWall);
                    this.InvalidateRect(this.cWall.PaintRect, 30);
                }
                else if (this.keelFilling)
                {
                    if (!this.ceiling.Contain(e.Location))
                        return;

                    if (this.keel == null)
                        this.keel = new Keel();
                    else
                        this.InvalidateRect(this.keel.PaintRect);

                    this.keel.Fill(ceiling, e.Location, 
                        this.productSet.KeelSet.GetKeelOri(e.Location));
                    this.InvalidateRect(this.keel.PaintRect);
                }
                else if (moving > 0)
                {
                    if (moving == 4 || moving == 5)
                    {
                        this.InvalidateRect(this.movingRect);
                        this.movingRect.X += delta.X;
                        this.movingRect.Y += delta.Y;
                        this.InvalidateRect(this.movingRect);
                    }
                    else if (moving < 4)
                    {
                        if (this.SelectedWall != null)
                        {
                            delta = new Point(e.X - this.mouseDownPoint.X, 
                                e.Y - this.mouseDownPoint.Y);
                            this.PriviewMoveWall(delta, this.moving);
                        }
                        else if (this.keel != null)
                        {
                            MoveKeel(this.keel, 
                                new PointF(delta.X * ceiling.Scale, delta.Y * ceiling.Scale),
                                this.moving);
                            //MoveKeel(this.keel, delta, this.moving);
                        }
                    }
                    this.Change();
                }

                if (!this.draft && !this.wallFuzhu && !this.keelFilling 
                    && this.moving == 0)
                {
                    Rectangle rect = this.ceiling.DrawingRect, r1 = rect;
                    r1.X -= 40; r1.Y -= 40; r1.Width = 40; r1.Height = 40;
                    Rectangle r2 = r1, r3 = r1, r4 = r1;
                    r2.X = rect.Right; r2.Y = rect.Bottom;
                    r3.X = rect.Left - 40; r3.Y = rect.Bottom;
                    r4.X = rect.Right; r4.Y = rect.Top - 40;

                    if (r1.Contains(e.Location) || r2.Contains(e.Location)
                        || r3.Contains(e.Location) || r4.Contains(e.Location))
                    {
                        this.Cursor = Cursors.NoMove2D;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        public void InvalidateRect(Rectangle rect)
        {
            rect.X -= 5; rect.Y -= 5; 
            rect.Width += 10; rect.Height += 10;
            this.Invalidate(rect);
        }

        public void InvalidateRect(Rectangle rect, int delta)
        {
            rect.X -= delta; rect.Y -= delta;
            rect.Width += delta * 2; rect.Height += delta * 2;
            this.Invalidate(rect);
        }

        public bool BeginDraft(Cursor cur, ToolStripStatusLabel label)
        {
            if (this.order == null || this.ceiling == null)
                return false;
            if (this.InitCeiling() == false)
                return false;

            this.positionStatusLabel = label;
            this.Cursor = cur;
            this.cWall = null;
            this.draft = true;

            (this.ParentForm as PalaceForm).RefrushText(this);
            (this.ParentForm as PalaceForm).SetCeilingMenu(false);

            WallEdit wallEdit = (this.ParentForm as PalaceForm).WallEdit;
            wallEdit.ShowAllGroups();
            //wallEdit.HideOffsetGroup();
            wallEdit.HideRemarkGroup();

            wallEdit.BeginTextbox.Enabled = false;
            wallEdit.EndTextbox.Enabled = false;
            wallEdit.OffsetTextbox.Enabled = false;
            wallEdit.RemarkTextBox.Enabled = false;
            wallEdit.DepthTextbox.Enabled = false;
            wallEdit.DepthTextbox.Text = this.ceiling.Depth.ToString(); // "0";

            return true;
        }

        private bool CheckDraftEnd()
        {
            if (this.ceiling.Walles.Count < 3)
                return false;
            Wall w0 = this.ceiling.Walles[0];
            if (Math.Abs(this.cWall.End.X - w0.Begin.X)
                + Math.Abs(this.cWall.End.Y - w0.Begin.Y) > 100)
                return false;

            // 将新生成的一条边去掉，
            // 指向加入平面图的最后一条边，并把它从平面图中提出来
            if (this.cWall.Length < 100)
                this.cWall = this.ceiling.PushWall();

            // 如果当前的边和平面图中的第一条边平行，就把这条边丢弃
            if (this.cWall.IsParallel(w0))
                this.cWall = this.ceiling.Walles[this.ceiling.Walles.Count - 1];
            else
                this.ceiling.AddWall(this.cWall);

            PointF _inter = this.cWall.Intersect(w0);
            w0.Begin = _inter;
            w0.BeginPaintPoint = new Point((int)(_inter.X / 10),
                (int)(_inter.Y / 10));
            this.cWall.End = _inter;
            this.cWall.EndPaintPoint = w0.BeginPaintPoint;

            this.draft = false;
            this.cWall = null;
            this.Cursor = Cursors.Default;

            PalaceForm pfrm = this.ParentForm as PalaceForm;
            pfrm.WallDrafted();
            pfrm.WallEdit.UnSelect();
            pfrm.SetCeilingMenu(true);
            pfrm.SetAlignMenu(this.ProductSet.Align);

            this.DisplayCeiling();
            this.Invalidate();

            return true;
        }

        public void BeginFillKeel(Cursor cur)
        {
            if (this.ceiling == null || this.ceiling.Length == 0 
                || this.productSet == null)
                return;
            this.Cursor = cur;
            this.keelFilling = true;
            (this.ParentForm as PalaceForm).SetDoMenu(false);
        }

        public void EndFillKeel()
        {
            EditKeelRevocationEventArgs arg = new EditKeelRevocationEventArgs();
            arg.keel = this.keel;
            arg.container = this.productSet.KeelSet.MainKeelList;
            this.AddUndo(arg, Revocation.DeleteKeel);

            this.keel.Depth = this.productSet.KeelSet.CeilingDepth;
            this.keel.CalcRemarkLoca(this.ceiling);
            this.productSet.KeelSet.MainKeelList.Add(this.keel);

            //this._AddKeel();
            this.keel = null;
            this.keelFilling = false;
            this.Cursor = Cursors.Default;
            (this.ParentForm as PalaceForm).SetDoMenu(this);
        }

        public bool BeginDrawWall(Cursor cur, ToolStripStatusLabel label)
        {
            if (this.order == null || this.ceiling == null)
                return false;

            this.positionStatusLabel = label;
            this.Cursor = cur;
            this.wallFuzhu = true;
            this.cWall = null;
            this.szone = new SZone();

            (this.ParentForm as PalaceForm).SetDoMenu(false);
            WallEdit wallEdit = (this.ParentForm as PalaceForm).WallEdit;
            wallEdit.ShowAllGroups();
            wallEdit.HideRemarkGroup();

            wallEdit.LengthTextbox.Enabled = true;
            wallEdit.BeginTextbox.Enabled = false;
            wallEdit.EndTextbox.Enabled = false;
            wallEdit.OffsetTextbox.Enabled = false;
            wallEdit.RemarkTextBox.Enabled = false;
            wallEdit.DepthTextbox.Enabled = false;
            wallEdit.DepthTextbox.Text = this.ceiling.Depth.ToString();

            return true;
        }

        public void EndDrawWall()
        {
            this.szone.Close(this.ceiling);
            this.szone.Depth = (uint)ceiling.Depth;
            this.szone.CalcLoca(this.ceiling);
            this.sZones.Add(this.szone);

            EditSZoneRevocationEventArgs arg = new EditSZoneRevocationEventArgs();
            arg.szone = this.szone;
            this.AddUndo(arg, Revocation.DeleteSZone);

            this.szone = null;
            this.cWall = null;

            PalaceForm pfrm = this.ParentForm as PalaceForm;
            pfrm.WallDrafted();
            pfrm.WallEdit.UnSelect();
            this.wallFuzhu = false;
            this.Cursor = Cursors.Default;

            (this.ParentForm as PalaceForm).SetDoMenu(this);
            this.szone.Invalidate(this);
        }

        private void Graph_MouseUp(object sender, MouseEventArgs e)
        {
            Point delta = new Point(e.X - this.mouseDownPoint.X,
                e.Y - this.mouseDownPoint.Y);
            PalaceForm pfrm = this.ParentForm as PalaceForm;
            WallEdit wedit = pfrm.WallEdit;

            try
            {
                if (this.draft) // 绘制草图
                {
                    if (this.cWall != null && this.ceiling.Walles.Count > 0)
                    {
                        if (this.ceiling.FilterWall(this.cWall))
                        {
                            wedit.SetData(this.cWall);
                            return;
                        }
                        if (this.CheckDraftEnd())
                            return;
                    }

                    Wall wall = new Wall();
                    if (this.cWall != null)
                    {
                        this.ceiling.AddWall(this.cWall);
                        wall.Next(this.cWall);
                    }
                    else
                    {
                        wall.InitPoint(e.Location);
                    }

                    this.cWall = wall;
                    wall.Wallnum = this.ceiling.Walles.Count + 1;

                    wedit.SetData(wall);
                    wedit.Show();
                    wedit.LengthTextbox.Focus();
                }
                else if (this.wallFuzhu) // 绘制辅助墙
                {
                    if (this.cWall != null)// && fuzhuWalls.Count > 0)
                    {
                        if (szone.FilterWall(this.cWall))
                        {
                            wedit.SetData(this.cWall);
                            return;
                        }

                        this.cWall.Cut(this.ceiling);
                        this.cWall.CalcRemarkLoca(this.ceiling);
                        this.szone.AddWall(this.cWall);

                        if (szone.CheckClosed(this.ceiling))
                        {
                            this.EndDrawWall();
                            return;
                        }
                    }

                    Wall wall = new Wall();
                    if (this.cWall != null)
                    {
                        wall.Next(this.cWall);
                    }
                    else
                    {
                        bool su = false;

                        for (int w = 0; w < ceiling.Walles.Count; w++)
                        {
                            if (ceiling.Walles[w].PaintLine.Contains(e.Location, true))
                                su = true;
                            else if (!this.ceiling.DrawingRect.Contains(e.Location)
                                && ceiling.Walles[w].PaintLine.Distance(e.Location) < 10)
                                su = true;
                        }

                        if (!su)
                        {
                            MessageBox.Show(@"绘制分割区域，需要从吊顶图中的房顶的某条外边开始，"
                                + "到某条外边结束。", ShareData.AppName);
                            return;
                        }

                        wall.InitPoint(e.Location);
                        wall.Begin = this.ceiling.GetLogicCoord(wall.BeginPaintPoint);
                        wall.End = wall.Begin;
                    }

                    this.cWall = wall;
                    wedit.SetData(this.cWall);
                    wedit.Show();
                    wedit.LengthTextbox.Focus();
                }
                else if (this.keelFilling)
                {
                    if (!this.ceiling.DrawingRect.Contains(e.Location))
                        return;
                    this.EndFillKeel();
                }

                if (moving > 0 && Math.Abs(delta.X) + Math.Abs(delta.Y) > 3)
                {
                    if (moving < 4)
                    {
                        if (this.SelectedWall != null)
                        {
                            delta = new Point(e.X - this.mouseDownPoint.X,
                                e.Y - this.mouseDownPoint.Y);
                            if (moving == 1)
                                this.ceiling.MoveBeginPoint(this.SelectedIndex, delta);
                            else if (moving == 2)
                                this.ceiling.MoveWall(this.SelectedIndex, delta);
                            else if (moving == 3)
                                this.ceiling.MoveEndPoint(this.SelectedIndex, delta);
                            this.Change();
                        }
                    }
                    else if (moving == 4)
                    {
                        if (this.ceiling.Length > 0)
                        {
                            this.ceiling.Translate(delta);
                        }
                        if (this.productSet != null)
                            this.productSet.Translate(delta);
                        for (int i = 0; i < this.sZones.Count; i++)
                            this.sZones[i].Translate(delta, ceiling);
                    }
                    else if (moving == 5)
                    {
                        this.MoveProducts(new PointF(delta.X * ceiling.Scale, 
                            delta.Y * ceiling.Scale));
                        //this.MoveProducts(ceiling.GetLogicCoord(delta));
                        //this.MoveProducts(delta);
                    }
                    this.Invalidate();
                }

                moving = 0;
                if (this.productSet == null)
                    return;
                BuckleSet bs = this.productSet.BuckleSet;
                if (bs.SelectProducts.Count < 1)
                    return;

                if (bs.SelectRectangle().Contains(e.Location)
                    && e.Button == MouseButtons.Right)
                {
                    (this.ParentForm as PalaceForm).SetEditMenu(true);
                    (this.ParentForm as PalaceForm).SetPasteMenu(false);
                    return;
                }

                if (!this.ctrlDown && Math.Abs(delta.X) + Math.Abs(delta.Y) < 5)
                {
                    Rectangle rect = bs.SelectRectangle();
                    this.InvalidateRect(rect);
                    bs.CancelSelect();
                    if (bs.Select(e.Location) != null)
                        this.InvalidateRect(bs.SelectRectangle());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private void Graph_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!this.ctrlDown || this.ceiling == null)
                return;

            try
            {
                Rectangle rect = this.ceiling.DrawingRect;
                float scale = (float)e.Delta / 1200;

                rect.X -= (int)(rect.Width * scale);
                rect.Y -= (int)(scale * rect.Height);
                rect.Width += (int)(2 * rect.Width * scale);
                rect.Height += (int)(2 * scale * rect.Height);

                Rectangle dr = ParseDrawingRect(this.ceiling.GetRect());
                scale = rect.Width / (float)dr.Width;
                if (scale > 0.5)
                {
                    (this.ParentForm as PalaceForm).ScaleLabel.Text
                        = ((int)(scale * 100)).ToString() + "%";
                    RefrushGraph(rect);
                    this.Invalidate();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        public void AddUndo(RevocationEventArgs arg, RevocationHandler handler)
        {
            arg.Revos = this.revocations;
            arg.Redos = this.redos;

            Revocation revo = new Revocation();
            revo.args = arg;
            revo.handler = handler;
            this.revocations.Push(revo);

            for (int i = 0; i < this.redos.Count; i++)
            {
                Revocation redo = this.redos.Pop();
                redo.args.Release();
            }
            this.redos.Clear();

            this.Change();
            (this.ParentForm as PalaceForm).SetDoMenu(this);
        }

        private void MoveKeel(Keel _keel, PointF delta, int _moving)
        {
            this.InvalidateKeel(_keel);
            if (!this.productSet.MoveKeel(_keel, delta, _moving))
                return;
            this.Change();
            this.InvalidateKeel(_keel);
            (this.ParentForm as PalaceForm).WallEdit.RefrushData();
        }

        //Point pb = _keel.BeginPaintPoint, pe = _keel.EndPaintPoint;
        //Rectangle _rt = _keel.PaintRect;

        //InvalidateRect(_rt, 8);
        //_rt.Width = 100; _rt.Height = 20;
        //_rt.X = pb.X; _rt.Y = pb.Y - 20;
        //this.Invalidate(_rt);
        //_rt.X = pe.X; _rt.Y = pe.Y - 20;
        //this.Invalidate(_rt);

        //InvalidateKeel(_keel);

        private Rectangle _rect(Point p1, Point p2)
        {
            Rectangle rect = new Rectangle();
            rect.X = Math.Min(p1.X, p2.X) - 5;
            rect.Width = Math.Max(p1.X, p2.X) - rect.X + 10;
            rect.Y = Math.Min(p1.Y, p2.Y) - 5;
            rect.Height = Math.Max(p1.Y, p2.Y) - rect.Y + 10;
            return rect;
        }

        private void PriviewMoveWall(Point delta, int moving)
        {
            if (Math.Abs(delta.X) + Math.Abs(delta.Y) < 2)
                return;
            Wall wall1 = this.SelectedWall, wall2 = this.SelectedWall;

            if (moving == 1)
            {
                if (this.SelectedIndex > 0)
                    wall1 = this.ceiling.Walles[this.SelectedIndex - 1];
                else
                    wall1 = this.ceiling.Walles[this.ceiling.Walles.Count - 1];

                this.Invalidate(_rect(wall1.BeginPaintPoint, this.mouseLocation));
                this.Invalidate(_rect(wall2.EndPaintPoint, this.mouseLocation));
            }
            else if (moving == 2)
            {
                Point center = new Point(
                    (wall1.BeginPaintPoint.X + wall1.EndPaintPoint.X) / 2, 
                    (wall1.BeginPaintPoint.Y + wall1.EndPaintPoint.Y) / 2);
                Point begin = new Point(wall1.BeginPaintPoint.X + delta.X, 
                    wall1.BeginPaintPoint.Y + delta.Y);
                Point end = new Point(wall1.EndPaintPoint.X + delta.X, 
                    wall1.EndPaintPoint.Y + delta.Y);
                this.Invalidate(_rect(begin, end));
            }
            else if (this.moving == 3)
            {
                if (this.SelectedIndex < this.ceiling.Walles.Count - 1)
                    wall2 = this.ceiling.Walles[this.SelectedIndex + 1];
                else
                    wall2 = this.ceiling.Walles[0];

                this.Invalidate(_rect(wall1.BeginPaintPoint, this.mouseLocation));
                this.Invalidate(_rect(wall2.EndPaintPoint, this.mouseLocation));
            }
        }

        private void Draw(Graphics graphics)
        {
            this.ceiling.Draw(graphics, this.mainKeelPen);

            if (this.productSet != null)
            {
                this.productSet.BuckleSet.Draw(graphics);
                this.productSet.KeelSet.Draw(graphics, this.mainKeelPen, 
                    arrowPen, lengthFont);
            }

            this.ceiling.DrawLength(graphics, arrowPen, lengthFont, strFormat);
            this.ceiling.DrawBorder(graphics, this.mainKeelPen, arrowPen,
                lengthFont);

            for (int i = 0; i < sZones.Count; i++)
            {
                sZones[i].Draw(graphics, mainKeelPen, arrowPen, lengthFont);
            }
        }

        public void DisplayGraph(Graphics graphics, RectangleF dstRect, 
            int border)
        {
            if (this.ceiling == null || this.ceiling.Length < 3)
                return;

            Rectangle rect = this.ceiling.DrawingRect;
            Bitmap bitmap = new Bitmap(rect.Width + rect.X * 2, 
                rect.Height + rect.Y * 2);
            Graphics g = Graphics.FromImage(bitmap);

            try
            {
                //g.FillRectangle(Brushes.Aqua, rect);
                this.Draw(g);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ShareData.AppName);
            }
            finally
            {
                g.Dispose();
            }

            RectangleF srcRect = this.ceiling.DrawingRect;
            srcRect.X -= border; srcRect.Y -= border;
            srcRect.Width += border * 2; srcRect.Height += border * 2;
            //srcRect.X -= 80; srcRect.Y -= 80;
            //srcRect.Width += 160; srcRect.Height += 160;

            float sw = srcRect.Width / dstRect.Width,
                sh = srcRect.Height / dstRect.Height;

            if (sw < sh)
            {
                float width = dstRect.Width * sh;
                srcRect.X -= (width - srcRect.Width) / 2;
                srcRect.Width = width;
            }
            else
            {
                float height = dstRect.Height * sw;
                srcRect.Y -= (height - srcRect.Height) / 2;
                srcRect.Height = height;
            }

            try
            {
                graphics.DrawImage(bitmap, dstRect, srcRect, 
                    GraphicsUnit.Pixel);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ShareData.AppName);
            }
            finally
            {
                bitmap.Dispose();
            }
        }

        private void Graph_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (this.ceiling == null)
                    return;
                this.Draw(e.Graphics);

                if (this.moving > 0 && this.moving < 4 && this.SelectedIndex >= 0)
                {
                    this.ceiling.PaintSelectWall(e.Graphics, this.moving,
                        this.dottedPen, this.mouseLocation);
                }
                else if (this.moving == 4 || this.moving == 5)
                {
                    e.Graphics.DrawRectangle(dottedPen, this.movingRect);
                }

                if (this.cWall != null)
                {
                    e.Graphics.DrawLine(mainKeelPen, this.cWall.BeginPaintPoint,
                        this.cWall.EndPaintPoint);
                    this.cWall.DrawLength(this.ceiling.Clockwise, e.Graphics,
                        arrowPen, lengthFont, strFormat);
                }
                else if (this.ceiling.SelectedWall != null)
                {
                    this.ceiling.SelectedWall.DrawSelectedFlag(e.Graphics, 
                        this.selectedPen, lengthFont);
                }
                else if (this.selectSZoneWall != null)
                {
                    this.selectSZoneWall.DrawSelectedFlag(e.Graphics,
                        this.selectedPen, lengthFont);
                }
                else if (this.keel != null)
                {
                    if (this.keelFilling)
                        this.keel.Draw(e.Graphics, this.mainKeelPen);
                    else
                        this.keel.DrawSelectedFlag(e.Graphics, this.selectedPen,
                            lengthFont);
                }
                else if (this.productSet != null 
                    && this.productSet.BuckleSet.SelectProducts.Count > 0)
                {
                    this.productSet.BuckleSet.DrawSelectedFlag(e.Graphics);
                }

                if (szone != null && this.selectSZoneWall == null)
                {
                    szone.Draw(e.Graphics, mainKeelPen, arrowPen, lengthFont);
                    szone.DrawSelectFlag(e.Graphics, selectedPen);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void Graph_KeyDown(object sender, KeyEventArgs e)
        {
            this.ctrlDown = e.Control;
        }

        private void Graph_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.X)
                {
                    this.ClipProducts();
                }
                else if (e.KeyCode == Keys.C)
                {
                    this.CopyProducts();
                }
                else if (e.KeyCode == Keys.V)
                {
                    this.PasteProducts();
                }
            }
            this.ctrlDown = false;

            if (e.KeyCode == Keys.Escape)
            {
                if (this.wallFuzhu)
                    CancelDrawWall();
                else if (this.draft)
                    CancelDraft();
                else if (this.keelFilling)
                    CancelFillKeel();
                this.Invalidate();
                return;
            }

            if (e.KeyCode != Keys.Up && e.KeyCode != Keys.Down 
                && e.KeyCode != Keys.Left && e.KeyCode != Keys.Right)
                return;
            PointF delta = new PointF(0, 0);
            float stepx = 0, stepy = 0;

            List<AupuBuckle> sps = this.productSet.BuckleSet.SelectProducts;
            //if (this.productSet != null 
            //    && this.productSet.BuckleSet.SelectProducts.Count > 0)
            if (sps.Count > 0)
            {
                AupuBuckle product = sps[0];
                stepx = (float)Math.Max(10, 0.2 * product.Width);
                stepy = (float)Math.Max(10, 0.2 * product.Height);
                //stepx = (float)Math.Max(5, 0.2 * product.Width / ceiling.Scale);
                //stepy = (float)Math.Max(5, 0.2 * product.Height / ceiling.Scale);
            }
            else if (this.keel != null || this.SelectedWall != null
                || this.selectSZoneWall != null)
            {
                stepx = (float)Math.Max(10, this.ceiling.Width * 0.0125);
                stepy = (float)Math.Max(10, this.ceiling.Height * 0.0125);
                //stepx = (float)Math.Max(5, this.ceiling.DrawingRect.Width * 0.0125);
                //stepy = (float)Math.Max(5, this.ceiling.DrawingRect.Height * 0.0125);
            }

            if (e.KeyCode == Keys.Up)
                delta.Y -= stepy;
            else if (e.KeyCode == Keys.Down)
                delta.Y += stepy;
            else if (e.KeyCode == Keys.Left)
                delta.X -= stepx;
            else if (e.KeyCode == Keys.Right)
                delta.X += stepx;
            else
                return;

            //if (this.productSet != null 
            //    && this.productSet.BuckleSet.SelectProducts.Count > 0)
            if (sps.Count > 0)
            {
                this.MoveProducts(delta);
            }
            else if (this.keel != null)
            {
                this.MoveKeel(this.keel, delta, 2);
            }
            else if (this.SelectedWall != null)
            {
                this.InvalidateRect(this.SelectedWall.PaintRect, 40);
                this.ceiling.MoveWall(this.SelectedIndex, delta);
                this.InvalidateRect(this.SelectedWall.PaintRect, 40);
                this.InvalidateRect(this.ceiling.PrevWall.PaintRect, 40);
                this.InvalidateRect(this.ceiling.NextWall.PaintRect, 40);
                this.Change();
            }
            else if (this.selectSZoneWall != null)
            {
                this._selectSZone.Invalidate(this);
                this._selectSZone.MoveWall(this.selectSZoneWall, delta,
                    ceiling);
                this._selectSZone.Invalidate(this);
                this.Change();
            }
        }

        public void SetKeel(KeelOrientation orien)
        {
            if (this.productSet == null)
                return;
            SetKeelRevocationEventArgs revo_arg = new SetKeelRevocationEventArgs();

            revo_arg.keelSet = this.productSet.KeelSet;
            revo_arg.orien = orien;
            this.productSet.KeelSet.SetKeel(orien);

            this.AddUndo(revo_arg, Revocation.UnSetKeel);
            this.Invalidate();
        }

        /// <summary>
        /// 旋转整个图
        /// </summary>
        /// <param name="angle">90度或者-90度</param>
        public void TransAll(int angle)
        {
            PointF center = new PointF(ceiling.Left + ceiling.Width / 2,
                ceiling.Top + ceiling.Height / 2);

            this.ceiling.Trans(angle, center);
            this.productSet.Trans(angle, center);

            for (int i = 0; i < this.sZones.Count; i++)
            {
                sZones[i].Trans(angle, ceiling);
            }
        }

        public void Trans()
        {
            if (this.ceiling == null || this.productSet == null)
                return;
            BuckleSet buckleSet = this.productSet.BuckleSet;
            List<AupuBuckle> sp = buckleSet.SelectProducts;

            if (sp.Count > 0)
            {
                TransRevocationEventArgs revo_arg = new TransRevocationEventArgs();
                revo_arg.trans = -90;
                for (int i = 0; i < sp.Count; i++)
                    revo_arg.products.Add(sp[i]);
                this.AddUndo(revo_arg, Revocation.UnTrans);

                this.Invalidate(buckleSet.SelectRectangle());
                buckleSet.TransSelection();
                this.Invalidate(buckleSet.SelectRectangle());
            }
            else if (this.keel != null)
            {
                this.InvalidateKeel(this.keel);
                this.keel.Trans(90, this.keel.Center, ceiling);
                this.InvalidateKeel(this.keel);
            }
            else if (this.SelectedIndex < 0)
            {
                TransRevocationEventArgs revo_arg = new TransRevocationEventArgs();
                revo_arg.trans = -90;
                this.AddUndo(revo_arg, Revocation.UnTrans);

                this.InvalidateAll();
                this.TransAll(90);
                this.InvalidateAll();
            }
        }

        public void InvalidateAll()
        {
            this.InvalidateRect(this.ceiling.DrawingRect, 40);
            for (int i = 0; i < this.sZones.Count; i++)
            {
                this.sZones[i].Invalidate(this);
            }
        }

        public void SaveDB(CeilingDataSet ceilingDataSet,
            CeilingDataSetTableAdapters.ceilingsTableAdapter ceilingAdapter)
        {
            if (this.Ceiling.ID < 1
                || ceilingDataSet.ceilings.FindByID(this.ceiling.ID) == null)
                this.ceilingRow = ceilingDataSet.ceilings.NewceilingsRow();
            else
                this.ceilingRow = ceilingDataSet.ceilings.FindByID(this.ceiling.ID);

            if (this.productSet != null)
                this.productSet.WriteData(this.ceilingRow);

            if (this.ceilingRow.RowState == DataRowState.Detached)
                ceilingDataSet.ceilings.AddceilingsRow(this.ceilingRow);

            // 将新加的图纸的ID，按照数据库里的新ID进行更新，
            // 解决新建订单和由文件打开的订单到服务器订单的转化问题
            if (this.ceilingRow.RowState == DataRowState.Added)
            {
                ceilingAdapter.Update(this.ceilingRow);
                int _id = (int)ceilingAdapter.Adapter.InsertCommand.LastInsertedId;
                //this.ceilingRow.AcceptChanges();
                this.ceiling.ID = _id;
                this.ceilingRow.ID = _id;
                this.ceilingRow.AcceptChanges();
            }
        }

        public void SaveToDB()
        {
            this.changed = false;
        }

        public void SaveFile(CeilingDataSet dataSet, int _ceiling_id)
        {
            if (this.ceiling == null && this.productSet == null)
                return;
            CeilingDataSet.ceilingsRow row = dataSet.ceilings.NewceilingsRow();

            row.BeginEdit();
            if (this.ceiling.ID < 1)
                row.ID = _ceiling_id;
            else
                row.ID = this.ceiling.ID;
            row.EndEdit();

            this.ceiling.WriteData(row, order.ID);
            this.productSet.WriteData(row);

            dataSet.ceilings.AddceilingsRow(row);
            dataSet.ceilings.AcceptChanges();

            this.ceiling.SaveToFile(dataSet.ceiling_walles, _ceiling_id);
            for (int i = 0; i < sZones.Count; i++)
            {
                sZones[i].Save(dataSet.szones, dataSet.ceiling_walles, 
                    _ceiling_id);
            }
            dataSet.szones.AcceptChanges();
            dataSet.ceiling_walles.AcceptChanges();

            this.changed = false;
        }

        public void ClipProducts()
        {
            this.CopyProducts();
            this.DeleteProducts();
        }

        public void CopyProducts()
        {
            if (this.productSet == null 
                || this.productSet.BuckleSet.SelectProducts.Count < 1)
                return;
            BuckleSet buckleset = this.productSet.BuckleSet;
            ShareData.list.Clear();

            for (int i = 0; i < buckleset.SelectProducts.Count; i++)
            {
                ShareData.list.Add(buckleset.SelectProducts[i]);
            }
        }

        public void PasteProducts()
        {
            if (this.productSet == null || ShareData.list.Count < 1)
                return;
            AupuBuckle temp = ShareData.list[0] as AupuBuckle;
            Rectangle rect = temp.DrawingRect;

            for (int i = 1; i < ShareData.list.Count; i++)
            {
                temp = ShareData.list[i] as AupuBuckle;
                rect = Rectangle.Union(rect, temp.DrawingRect);
            }

            Point pp = new Point(0, this.ceiling.DrawingRect.Bottom 
                + temp.DrawingRect.Height - rect.Top);
            PointF ap = new PointF(0, pp.Y * this.ceiling.Scale);
            ProductRevocationEventArgs arg = new ProductRevocationEventArgs();

            for (int i = 0; i < ShareData.list.Count; i++)
            {
                temp = (ShareData.list[i] as AupuBuckle).clone();
                temp.Index = -1;
                temp.Translate(pp);
                temp.Location = new PointF(temp.Location.X, temp.Location.Y + ap.Y);

                arg.dropProducts.Add(temp);
                this.productSet.BuckleSet.AddedProducts.Add(temp);
            }

            arg.add_Products.Clear();
            arg.moveProducts.Clear();
            arg.productSet = this.productSet;
            this.AddUndo(arg, Revocation.Products);

            this.Invalidate();
        }

        public void ClearProducts()
        {
            if (this.productSet == null)
                return;
            BuckleSet buckleset = this.productSet.BuckleSet;

            ClearProductsRevocationEventArgs arg = new ClearProductsRevocationEventArgs();
            for (int i = 0; i < buckleset.AddedProducts.Count; i++)
                arg.addeds.Add(buckleset.AddedProducts[i]);
            arg.tile = productSet.Tile.Clone();
            this.AddUndo(arg, Revocation.ReTileProduct);

            productSet.Tile.Clear();
            buckleset.Clear();
            this.Invalidate(this.ceiling.DrawingRect);
        }

        public void TileProducts(BuckleNode buckle, TileStyle style, 
            int trans)
        {
            if (this.ceiling == null || this.productSet == null)
                return;
            if (buckle == null && buckle.OriginalRow.width < 1
                || buckle.OriginalRow.height < 1)
                return;
            BuckleSet buckleset = this.productSet.BuckleSet;

            TileRevocationEventArgs arg = new TileRevocationEventArgs();
            arg.tile = productSet.Tile.Clone();
            this.AddUndo(arg, Revocation.UnTileProduct);

            productSet.Tile.SetTile(new AupuBuckle(buckle, trans), style);
            buckleset.ReTileProducts();
            this.Invalidate(this.ceiling.DrawingRect);
        }

        public void MoveProducts(Point delta)
        {
            if (this.productSet == null || this.productSet.BuckleSet.SelectProducts.Count < 1)
                return;
            BuckleSet buckleset = this.productSet.BuckleSet;
            InvalidateRect(buckleset.SelectRectangle());

            ProductRevocationEventArgs arg = new ProductRevocationEventArgs();
            arg.productSet = this.productSet;
            arg.add_Products = buckleset.MoveSelectedProducts(delta);

            for (int i = 0; i < buckleset.SelectProducts.Count; i++)
            {
                arg.moveProducts.Add(buckleset.SelectProducts[i]);
            }

            PointF pt = ceiling.GetLogicCoord(delta);
            arg.logic = new PointF(-pt.X, -pt.Y);

            this.AddUndo(arg, Revocation.Products);
            InvalidateRect(buckleset.SelectRectangle());
        }

        public void MoveProducts(PointF delta)
        {
            if (this.productSet == null || this.productSet.BuckleSet.SelectProducts.Count < 1)
                return;
            BuckleSet buckleset = this.productSet.BuckleSet;
            InvalidateRect(buckleset.SelectRectangle());

            ProductRevocationEventArgs arg = new ProductRevocationEventArgs();
            arg.productSet = this.productSet;
            arg.add_Products = buckleset.MoveSelectedProducts(delta);

            for (int i = 0; i < buckleset.SelectProducts.Count; i++)
            {
                arg.moveProducts.Add(buckleset.SelectProducts[i]);
            }
            arg.logic = new PointF(-delta.X, -delta.Y);

            this.AddUndo(arg, Revocation.Products);
            InvalidateRect(buckleset.SelectRectangle());
        }

        public void DeleteProducts()
        {
            if (this.ProductSet == null || this.productSet.BuckleSet.SelectProducts.Count < 1)
                return;
            BuckleSet buckleset = this.productSet.BuckleSet;

            ProductRevocationEventArgs arg = new ProductRevocationEventArgs();
            arg.productSet = this.productSet;
            arg.dropProducts.Clear();
            arg.moveProducts.Clear();

            for (int i = 0; i < buckleset.SelectProducts.Count; i++)
            {
                AupuBuckle product = buckleset.SelectProducts[i];
                arg.add_Products.Add(product);
            }
            this.AddUndo(arg, Revocation.Products);

            InvalidateRect(buckleset.SelectRectangle());
            buckleset.DropSelectedProducts();
        }

        public void DeleteKeel()
        {
            if (this.productSet == null || this.keel == null)
                return;

            EditKeelRevocationEventArgs arg = new EditKeelRevocationEventArgs();
            arg.container = this.productSet.KeelSet.MainKeelList;
            arg.keel = this.keel;
            this.AddUndo(arg, Revocation.AddKeel);

            this.SelectKeel(-1);
            this.productSet.KeelSet.MainKeelList.Remove(arg.keel);
            this.Invalidate();
        }

        public void DeleteSZone()
        {
            if (this.sZones.Count == 0 || this.szone == null)
                return;

            EditSZoneRevocationEventArgs arg = new EditSZoneRevocationEventArgs();
            arg.szone = this.szone;
            this.AddUndo(arg, Revocation.AddSZone);

            this.SelectZone(-1);
            this.sZones.Remove(arg.szone);
            this.Invalidate();
        }

        public void Remove()
        {
            this.order.OrderGraphs.Remove(this);
            this.order.DeleteGraphs.Add(this);
            this.Change();

            if (this.ceilingRow != null)
            {
                this.ceilingRow.Delete();
                this.ceilingRow.Table.AcceptChanges();
            }

            CeilingDataSet.ceiling_wallesRow row;
            for (int i = 0; i < this.ceiling.Walles.Count; i++)
            {
                Wall _wall = this.ceiling.Walles[i];
                //this.ceiling.Walles.Remove(_wall);
                row = this.ceiling.CeilingWalles.FindByID(_wall.Id);
                if (row != null)
                    row.Delete();
            }
            this.ceiling.Walles.Clear();
        }

        public void AddProduct(AupuBuckle product, Point location)
        {
            if (this.productSet == null)
                return;
            BuckleSet bs = this.productSet.BuckleSet;

            if (this.productSet != null && bs.SelectProducts.Count > 0)
            {
                this.InvalidateRect(bs.SelectRectangle());
                bs.CancelSelect();
            }

            ProductRevocationEventArgs arg = new ProductRevocationEventArgs();
            List<AupuBuckle> prods = bs.AddProduct(product, location);

            if (prods != null)
            {
                for (int i = 0; i < prods.Count; i++)
                    arg.add_Products.Add(prods[i]);
            }

            arg.productSet = this.productSet;
            arg.dropProducts.Add(product);
            this.AddUndo(arg, Revocation.Products);

            InvalidateRect(product.DrawingRect);
        }

        public void Revocate()
        {
            if (this.revocations.Count < 1)
                return;
            Revocation revo = this.revocations.Pop();
            revo.handler(this, revo.args);
            this.Change();
        }

        public void Redo()
        {
            if (this.redos.Count < 1)
                return;
            Revocation redo = this.redos.Pop();
            redo.handler(this, redo.args);
            this.Change();
        }

        private void ParseData()
        {
            if (this.ceiling.CeilingWalles.Count > 0)
                this.ceiling.ParseData(this.ceilingRow);

            for (int i = 0; i < sZones.Count; i++)
            {
                sZones[i].Close(this.ceiling);
                sZones[i].CalcLoca(this.ceiling);
            }

            if (this.productSet == null)
                this.productSet = new ProductSet(this);
            this.productSet.ParseData(this.ceilingRow,
                ShareData.CeilingDataSet.products);

            this.RefrushGraph(new Rectangle(ceilingRow.display_left,
                ceilingRow.display_top,
                ceilingRow.display_width, ceilingRow.display_height));
            //this.DisplayCeiling();
        }

        private void AddSZones(CeilingDataSet.szonesDataTable _szones, 
            CeilingDataSet.ceiling_wallesDataTable cwalles)
        {
            for (int i = 0; i < _szones.Count; i++)
            {
                if (_szones[i].ceiling_id != ceiling.ID)
                    continue;
                SZone _szone = new SZone();
                _szone.AddWalles(cwalles, ceilingRow.ID, _szones[i].szone_num);
                sZones.Add(_szone);
                _szone.ParseData(this.ceiling, _szones[i]);
            }
        }

        public void LoadFromDB(CeilingDataSet.ceilingsRow _ceilingsRow)
        {
            this.revocations.Clear();
            this.redos.Clear();

            this.ceilingRow = _ceilingsRow;
            this.ceiling.SetCRow(_ceilingsRow);
            
            this.order.CwAdapter.FillByCeilingId(this.ceiling.CeilingWalles,
                _ceilingsRow.ID);
            this.AddSZones(ShareData.CeilingDataSet.szones, this.ceiling.CeilingWalles);

            this.ParseData();
        }

        public void LoadFromFile(CeilingDataSet dataSet, int index)
        {
            if (this.order == null || dataSet.ceilings.Count <= index)
                return;
            this.redos.Clear();
            this.revocations.Clear();

            this.ceilingRow = dataSet.ceilings[index];
            this.ceiling.SetCRow(ceilingRow);
            this.ceiling.AddWalles(dataSet.ceiling_walles);
            this.AddSZones(dataSet.szones, dataSet.ceiling_walles);

            this.ParseData();
        }

        public void WallEdite_Modify(object sender, KeelChangeEventArgs e)
        {
            WallEdit editer = sender as WallEdit;
            
            if (this.draft)
            {
                if (e.Type == KeelChangeType.Length)
                    this.cWall.SetLength((uint)e.PValue);
                this.InvalidateKeel(this.cWall);

                if (this.ceiling.Walles.Count > 0) //this.cWall != null && 
                {
                    if (this.ceiling.FilterWall(this.cWall))
                    {
                        editer.SetData(this.cWall);
                        return;
                    }
                    if (this.CheckDraftEnd())
                    {
                        return;
                    }
                }

                this.ceiling.AddWall(this.cWall);

                Wall wall = new Wall();
                wall.Next(this.cWall);
                wall.Wallnum = this.ceiling.Walles.Count + 1;

                editer.SetData(wall);
                this.cWall = wall;
                this.PositionTip(this.cWall);
                //this.InvalidateKeel(_keel);
            }
            else if (this.wallFuzhu)
            {
                if (e.Type == KeelChangeType.Length)
                    this.cWall.SetLength((uint)e.PValue);
                this.InvalidateKeel(this.cWall);

                if (szone.FilterWall(this.cWall))
                {
                    editer.SetData(this.cWall);
                    return;
                }

                this.cWall.Cut(this.ceiling);
                this.cWall.CalcRemarkLoca(this.ceiling);
                this.szone.AddWall(this.cWall);
                this.InvalidateKeel(this.cWall);

                if (szone.CheckClosed(this.ceiling))
                {
                    this.EndDrawWall();
                    return;
                }

                Wall wall = new Wall();
                wall.Next(this.cWall);
                this.cWall = wall;

                editer.SetData(this.cWall);
                editer.Show();
                editer.LengthTextbox.Focus();
            }
            else
            {
                if (e.Keel != null && this.SelectedWall == e.Keel)
                {
                    this.InvalidateRect(ceiling.DrawingRect, 40);
                    if (e.Type == KeelChangeType.Length)
                    {
                        this.ceiling.ChangeLength(this.SelectedIndex, 
                            (uint)e.PValue);
                    }
                    else if (e.Type == KeelChangeType.Offset)
                    {
                        this.ceiling.MoveWall(this.SelectedIndex, e.Offset);
                    }
                    else if (e.Type == KeelChangeType.Begin)
                    {
                        this.ceiling.MoveBeginPoint(this.SelectedIndex, e.Offset);
                    }
                    else if (e.Type == KeelChangeType.End)
                    {
                        this.ceiling.MoveEndPoint(this.SelectedIndex, e.Offset);
                    }
                    else if (e.Type == KeelChangeType.Remark)
                    {
                        e.Keel.RemarkStr = (string)e.PValue;
                        e.Keel.CalcRemarkLoca(this.ceiling);
                    }
                    AdjustCeiling();
                    this.InvalidateRect(ceiling.DrawingRect, 40);
                }
                else if (this.keel != null && this.keel == e.Keel)
                {
                    this.InvalidateKeel(this.keel);
                    if (e.Type == KeelChangeType.Length)
                    {
                        this.keel.ChangeLenth((uint)e.PValue, this.ceiling);
                    }
                    else if (e.Type == KeelChangeType.Depth)
                    {
                        this.keel.Depth = (uint)e.PValue;
                        this.keel.CalcRemarkLoca(this.ceiling);
                    }
                    else if (e.Type == KeelChangeType.Offset)
                    {
                        this.keel.Move(e.Offset, this.ceiling);
                    }
                    else if (e.Type == KeelChangeType.Begin)
                    {
                        this.keel.MoveBegin(e.Offset, this.ceiling);
                    }
                    else if (e.Type == KeelChangeType.End)
                    {
                        this.keel.MoveEnd(e.Offset, this.ceiling);
                    }
                    else if (e.Type == KeelChangeType.Remark)
                    {
                        this.keel.RemarkStr = (string)e.PValue;
                        this.keel.CalcRemarkLoca(this.ceiling);
                    }
                    this.InvalidateKeel(this.keel);
                }
                else if (this.szone != null && this.szone == e.Szone)
                {
                    this.szone.Invalidate(this);
                    if (e.Type == KeelChangeType.Depth)
                    {
                        this.szone.Depth = (uint)e.PValue;
                    }
                    else if (e.Type == KeelChangeType.Remark)
                    {
                        this.szone.RemarkStr = (string)e.PValue;
                    }
                    this.szone.CalcLoca(this.ceiling);
                    this.szone.Invalidate(this);
                }
                else if (e.Keel != null && e.Keel == this.selectSZoneWall)
                {
                    this._selectSZone.Invalidate(this);
                    if (e.Type == KeelChangeType.Length)
                    {
                        this._selectSZone.SetLength(this.selectSZoneWall, 
                            this.ceiling, (uint)e.PValue);
                    }
                    else if (e.Type == KeelChangeType.Offset)
                    {
                        this._selectSZone.MoveWall(this.selectSZoneWall,
                            e.Offset, this.ceiling);
                    }
                    else if (e.Type == KeelChangeType.Begin)
                    {
                        PointF pt = new PointF(this.selectSZoneWall.Begin.X + e.Offset.X,
                            this.selectSZoneWall.Begin.Y + e.Offset.Y);
                        this._selectSZone.ModifyWall(this.selectSZoneWall,
                            this.ceiling, pt, this.selectSZoneWall.End);
                    }
                    else if (e.Type == KeelChangeType.End)
                    {
                        PointF pt = new PointF(this.selectSZoneWall.End.X + e.Offset.X,
                            this.selectSZoneWall.End.Y + e.Offset.Y);
                        this._selectSZone.ModifyWall(this.selectSZoneWall,
                            this.ceiling, this.selectSZoneWall.Begin, pt);
                    }
                    else if (e.Type == KeelChangeType.Remark)
                    {
                        this.selectSZoneWall.RemarkStr = (string)e.PValue;
                        this.selectSZoneWall.CalcRemarkLoca(this.ceiling);
                    }
                    this._selectSZone.Invalidate(this);
                }
            }
        }

        private void CancelDraft()
        {
            this.draft = false;
            this.cWall = null;
            this.Cursor = Cursors.Default;
            this.ceiling.Clear();

            PalaceForm pfrm = this.ParentForm as PalaceForm;
            pfrm.WallDrafted();
            pfrm.WallEdit.UnSelect();
        }

        public void CancelDrawWall()
        {
            this.wallFuzhu = false;
            this.cWall = null;
            this.szone = null;
            this.Cursor = Cursors.Default;

            PalaceForm pfrm = this.ParentForm as PalaceForm;
            pfrm.WallDrafted();
            pfrm.WallEdit.UnSelect();
        }

        public void CancelFillKeel()
        {
            this.keelFilling = false;
            this.keel = null;
            this.Cursor = Cursors.Default;
        }

        public void WallEdite_Excape(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 27)
                return;
            PalaceForm pfrm = this.ParentForm as PalaceForm;

            if (this.wallFuzhu)
            {
                CancelDrawWall();
            }
            else if (this.draft)
            {
                CancelDraft();
            }

            this.Invalidate();
        }
    }

    public enum KeelOrientation { Auto, Horizontal, Vertical };
}
