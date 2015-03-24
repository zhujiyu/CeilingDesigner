using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
//using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CeilingDesigner
{
    public partial class WallEdit : UserControl
    {
        public TextBox LengthTextbox
        {
            get { return this.lenghTextBox; }
        }

        public TextBox OffsetTextbox
        {
            get { return this.offsetTextBox; }
        }

        public TextBox BeginTextbox
        {
            get { return this.beginTextBox; }
        }

        public TextBox EndTextbox
        {
            get { return this.endTextBox; }
        }

        public TextBox DepthTextbox
        {
            get { return this.depthTextBox; }
        }

        public TextBox RemarkTextBox
        {
            get { return this.remarkTextBox; }
        }

        public event KeelChangeEventHandler modify;
        public event ExcapeEvnetHandler excape;

        private Keel keel = null;
        private SZone szone = null;

        private bool deleteMenuEnable = false;

        public WallEdit()
        {
            InitializeComponent();
        }

        public void HideLengthGroup()
        {
            this.groupBox1.Hide();
        }

        public void HideOffsetGroup()
        {
            this.groupBox2.Hide();
        }

        public void HideRemarkGroup()
        {
            this.groupBox3.Hide();
        }

        public void ShowAllGroups()
        {
            this.groupBox1.Show();
            this.groupBox2.Show();
            this.groupBox3.Show();
        }

        public void RefrushData()
        {
            if (this.keel != null)
            {
                this.lenghTextBox.Text = keel.Length > 0 ? keel.Length.ToString() : "";
                this.beginTextBox.Text = "0, 0";
                this.endTextBox.Text = "0, 0";
                this.offsetTextBox.Text = "0";
                this.depthTextBox.Text = keel.Depth.ToString();
                this.remarkTextBox.Text = keel.RemarkStr;

                this.offsetLabel.Text = Math.Abs(keel.Vector.X) < Math.Abs(keel.Vector.Y) 
                    ? "水平移动" : "竖直移动";
                this.ALabel.Text = keel.AStr;
                this.BLabel.Text = keel.BStr;
            }
            else if (this.szone != null)
            {
                this.depthTextBox.Text = szone.Depth.ToString();
                this.remarkTextBox.Text = szone.RemarkStr;
            }
        }

        public void SetData(Keel _keel)
        {
            this.keel = _keel;
            this.szone = null;

            this.RefrushData();
        }

        public void SetData(SZone _szone)
        {
            this.keel = null;
            this.szone = _szone;

            this.RefrushData();
        }

        public void UnSelect()
        {
            this.Hide();
        }

        private PointF RegexPoint(string text)
        {
            PointF p = new PointF(0, 0);
            if (keel == null || text == "0, 0")
                return p;

            Match m = Regex.Match(text, @"(-?\d+(\.\d+)?)(,[ 　]*(-?\d+(\.\d+)?))?");
            if (!m.Success || m.Groups.Count < 3)
                return p;

            p.X = float.Parse(m.Groups[1].Value);
            p.Y = float.Parse(m.Groups[4].Value);
            return p;
        }

        private void checkLength()
        {
            try
            {
                uint length = uint.Parse(this.lenghTextBox.Text);
                if (keel == null || keel.Length == length || length == 0)
                    return;
                KeelChangeEventArgs args = new KeelChangeEventArgs();

                args.PValue = length;
                args.Type = KeelChangeType.Length;
                args.Keel = keel;

                if (this.modify != null)
                    this.modify(this, args);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private void checkOffset(PointF pt, KeelChangeType type)
        {
            if (pt.X == 0 && pt.Y == 0)
                return;
            KeelChangeEventArgs args = new KeelChangeEventArgs();

            args.Offset = pt;
            args.Type = type;
            args.Keel = keel;

            if (this.modify != null)
                this.modify(this, args);
        }

        private void checkDepth()
        {
            try
            {
                uint depth = uint.Parse(this.depthTextBox.Text);
                KeelChangeEventArgs args = new KeelChangeEventArgs();

                if (keel != null)
                {
                    if (keel.Depth == depth || depth == 0)
                        return;
                    args.Keel = keel;
                }
                else if (szone != null)
                {
                    if (szone.Depth == depth || depth == 0)
                        return;
                    args.Szone = szone;
                }
                else return;

                args.PValue = depth;
                args.Type = KeelChangeType.Depth;

                if (this.modify != null)
                    this.modify(this, args);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private void checkRemark()
        {
            try
            {
                KeelChangeEventArgs args = new KeelChangeEventArgs();

                if (keel != null)
                {
                    if (keel.RemarkStr.CompareTo(this.remarkTextBox.Text) == 0)
                        return;
                    args.Keel = keel;
                }
                else if (szone != null)
                {
                    if (szone.RemarkStr.CompareTo(this.remarkTextBox.Text) == 0)
                        return;
                    args.Szone = szone;
                }
                else return;

                args.PValue = this.remarkTextBox.Text;
                args.Type = KeelChangeType.Remark;

                if (this.modify != null)
                    this.modify(this, args);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private void lenghTextBox_Leave(object sender, EventArgs e)
        {
            this.checkLength();
            GoBackMenu();
        }

        private void OffsetTextBox_Leave(object sender, EventArgs e)
        {
            Point opt = Math.Abs(keel.Vector.X) < Math.Abs(keel.Vector.Y)
                ? new Point(int.Parse(this.offsetTextBox.Text), 0)
                : new Point(0, int.Parse(this.offsetTextBox.Text));
            this.checkOffset(opt, KeelChangeType.Offset);
            this.offsetTextBox.Text = "0";
            GoBackMenu();
        }

        private void beginTextBox_Leave(object sender, EventArgs e)
        {
            this.checkOffset(RegexPoint(this.beginTextBox.Text), 
                KeelChangeType.Begin);
            GoBackMenu();
            this.beginTextBox.Text = "0, 0";
        }

        private void endTextBox_Leave(object sender, EventArgs e)
        {
            this.checkOffset(RegexPoint(this.endTextBox.Text), KeelChangeType.End);
            GoBackMenu();
            this.endTextBox.Text = "0, 0";
        }

        private void lenghTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                this.checkLength();
            else if (e.KeyChar == 27 && this.excape != null)
                this.excape(this, e);
        }

        private void OffsetTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                Point opt = Math.Abs(keel.Vector.X) < Math.Abs(keel.Vector.Y) 
                    ? new Point(int.Parse(this.offsetTextBox.Text), 0)
                    : new Point(0, int.Parse(this.offsetTextBox.Text));
                this.checkOffset(opt, KeelChangeType.Offset);
                this.offsetTextBox.Text = "0";
            }
        }

        private void beginTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                this.checkOffset(RegexPoint(this.beginTextBox.Text), 
                    KeelChangeType.Begin);
                this.beginTextBox.Text = "0, 0";
            }
        }

        private void endTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                this.checkOffset(RegexPoint(this.endTextBox.Text), 
                    KeelChangeType.End);
                this.endTextBox.Text = "0, 0";
            }
        }

        private void depthTextBox_Leave(object sender, EventArgs e)
        {
            checkDepth();
            GoBackMenu();
        }

        private void depthTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                checkDepth();
        }

        private void GoBackMenu()
        {
            PalaceForm pfrm = this.ParentForm as PalaceForm;
            pfrm.SetDeleteMenu(deleteMenuEnable);
            pfrm.SetDoMenu(pfrm.CurOrderGraph);
            pfrm.SetPasteMenu(ShareData.list.Count > 0);
        }

        private void remarkTextBox_Leave(object sender, EventArgs e)
        {
            checkRemark();
            GoBackMenu();
        }

        private void depthTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                checkDepth();
        }
        
        private void TextBox_Enter(object sender, EventArgs e)
        {
            PalaceForm pfrm = this.ParentForm as PalaceForm;
            deleteMenuEnable = pfrm.GetDeleteMenuEnable();
            pfrm.SetEditMenu(false);
            pfrm.SetPasteMenu(false);
            pfrm.SetDoMenu(false);
        }
    }

    public class KeelChangeEventArgs : EventArgs
    {
        private KeelChangeType type;

        public KeelChangeType Type
        {
            get { return type; }
            set { type = value; }
        }

        private object pValue;

        public object PValue
        {
            get { return pValue; }
            set { pValue = value; }
        }

        private PointF offset;

        public PointF Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        private Keel kell = null;

        public Keel Keel
        {
            get { return kell; }
            set { kell = value; }
        }

        private SZone szone = null;

        internal SZone Szone
        {
            get { return szone; }
            set { szone = value; }
        }

        public KeelChangeEventArgs() { }
    }

    public enum KeelChangeType { Length, Offset, Begin, End, Depth, Remark };

    public delegate void KeelChangeEventHandler(object sender, KeelChangeEventArgs e);
    public delegate void ExcapeEvnetHandler(object sender, KeyPressEventArgs e);
}

//if (keel.Length > 0)
//    this.lenghTextBox.Text = keel.Length.ToString();
//else
//    this.lenghTextBox.Text = "";

//if (_keel == null)
//    return;
//if (_szone == null)
//    return;

//private void checkOffset(string offset, KeelChangeType type)
//{
//    PointF p = RegexData(offset);
//    if (p.X == 0 && p.Y == 0)
//        return;
//    KeelChangeEventArgs args = new KeelChangeEventArgs();

//    args.Offset = p;
//    args.Type = type;
//    args.Keel = keel;

//    if (this.modify != null)
//        this.modify(this, args);
//}

//if (keel == null || keel.Depth == depth || depth == 0)
//    return;
//KeelChangeEventArgs args = new KeelChangeEventArgs();
//args.Keel = keel;

//if (keel == null || 
//    keel.RemarkStr.CompareTo(this.remarkTextBox.Text) == 0)
//    return;
//KeelChangeEventArgs args = new KeelChangeEventArgs();
//args.Keel = keel;

//(this.ParentForm as PalaceForm).SetObjectsMenu(true);
//(this.ParentForm as PalaceForm).SetPasteMenu(ShareData.list.Count > 0);

//Point opt = keel.Alpha == 0 ? new Point(int.Parse(this.offsetTextBox.Text), 0)
//    : new Point(0, int.Parse(this.offsetTextBox.Text));

//this.checkOffset(new Point(int.Parse(this.offsetTextBox.Text),
//    int.Parse(this.yOffsetTextBox.Text)), KeelChangeType.Offset);
//this.checkOffset(this.xOffsetTextBox, KeelChangeType.Offset);
//(this.ParentForm as PalaceForm).SetObjectsMenu(true);
//(this.ParentForm as PalaceForm).SetPasteMenu(ShareData.list.Count > 0);

//(this.ParentForm as PalaceForm).SetObjectsMenu(true);
//(this.ParentForm as PalaceForm).SetPasteMenu(ShareData.list.Count > 0);

//(this.ParentForm as PalaceForm).SetObjectsMenu(true);
//(this.ParentForm as PalaceForm).SetPasteMenu(ShareData.list.Count > 0);

//this.checkOffset(new Point(int.Parse(this.offsetTextBox.Text),
//    int.Parse(this.yOffsetTextBox.Text)), KeelChangeType.Offset);
//this.checkOffset(this.xOffsetTextBox, KeelChangeType.Offset);

//(this.ParentForm as PalaceForm).SetObjectsMenu(true);
//(this.ParentForm as PalaceForm).SetPasteMenu(ShareData.list.Count > 0);

//pfrm.SetEditMenu(true);
//pfrm.SetEditMenu(false);

//(this.ParentForm as PalaceForm).SetObjectsMenu(true);
//(this.ParentForm as PalaceForm).SetPasteMenu(ShareData.list.Count > 0);

//(this.ParentForm as PalaceForm).SetDoMenu(false);
//(this.ParentForm as PalaceForm).SetObjectsMenu(false);
//(this.ParentForm as PalaceForm).SetPasteMenu(false);

//if (this.offsetTextBox.Focused || this.yOffsetTextBox.Focused)
//{
//    this.checkOffset(new Point(int.Parse(this.offsetTextBox.Text),
//        int.Parse(this.yOffsetTextBox.Text)), KeelChangeType.Offset);
//}
//if (this.offsetTextBox.Focused)
//{
//    Point opt = keel.Alpha == 0 ? new Point(int.Parse(this.offsetTextBox.Text), 0)
//        : new Point(0, int.Parse(this.offsetTextBox.Text));
//    this.checkOffset(opt, KeelChangeType.Offset);
//    //this.checkOffset(new Point(int.Parse(this.offsetTextBox.Text), 0), 
//    //    KeelChangeType.Offset);
//}
//else if (this.beginTextBox.Focused)
//    this.checkOffset(RegexPoint(this.beginTextBox.Text), KeelChangeType.Begin);
//else if (this.endTextBox.Focused)
//    this.checkOffset(RegexPoint(this.endTextBox.Text), KeelChangeType.End);
//else if (this.lenghTextBox.Focused)
//    this.checkLength();

//if (this.lenghTextBox.Focused)
//    this.checkLength();
//else if (this.xOffsetTextBox.Focused || this.yOffsetTextBox.Focused)
//{
//    //Point pt = new Point(int.Parse(this.xOffsetTextBox.Text),
//    //    int.Parse(this.yOffsetTextBox.Text));
//    //this.checkOffset(pt, KeelChangeType.Offset);
//    this.checkOffset(new Point(int.Parse(this.xOffsetTextBox.Text),
//        int.Parse(this.yOffsetTextBox.Text)), KeelChangeType.Offset);
//}
//else if (this.beginTextBox.Focused)
//    this.checkOffset(this.beginTextBox.Text, KeelChangeType.Begin);
//else if (this.endTextBox.Focused)
//    this.checkOffset(this.endTextBox.Text, KeelChangeType.End);

//if (keel.Length > 0)
//    this.lenghTextBox.Text = keel.Length.ToString();
//else
//    this.lenghTextBox.Text = "";

//this.beginTextBox.Text = "0, 0";
//this.endTextBox.Text = "0, 0";
//this.offsetTextBox.Text = "0, 0";
//this.ALabel.Text = "A(" + keel.Begin.X + "," + keel.Begin.Y + "):";
//this.BLabel.Text = "B(" + keel.End.X + "," + keel.End.Y + "):";

//this.begintext.Text = keel.Begin.X + "," + keel.Begin.Y;
//this.endtext.Text = keel.End.X + "," + keel.End.Y;
//this.beginTextBox.Text = keel.Begin.X + ", " + keel.Begin.Y;
//this.endTextBox.Text = keel.End.X + ", " + keel.End.Y;

//if (length == 0)
//    return;
//if (keel == null || keel.Length.ToString() == this.lenghTextBox.Text)
//    return;
