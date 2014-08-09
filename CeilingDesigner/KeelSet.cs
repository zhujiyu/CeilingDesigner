using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace CeilingDesigner
{
    public class KeelSet
    {
        private Ceiling ceiling = null;

        private Rectangle depth_rect = new Rectangle(0, 0, 0, 0);

        public Rectangle DepthRect
        {
            get { return depth_rect; }
            set { depth_rect = value; }
        }

        private System.UInt32 ceiling_depth = 300;

        /// <summary>
        /// 吊顶夹高
        /// </summary>
        public System.UInt32 CeilingDepth
        {
            get { return ceiling_depth; }
            set
            {
                if (ceiling_depth == value)
                    return;
                for (int i = 0; i < this.mainKeelList.Count; i++)
                {
                    Keel keel = this.mainKeelList[i];
                    if (keel.Depth == ceiling_depth)
                        keel.Depth = value;
                    keel.CalcRemarkLoca(ceiling);
                }
                ceiling_depth = value;
            }
        }

        private List<Keel> mainKeelList = new List<Keel>();

        public List<Keel> MainKeelList
        {
            get { return mainKeelList; }
        }

        private List<Keel> auxiKeelList = new List<Keel>();

        public List<Keel> AuxiKeelList
        {
            get { return auxiKeelList; }
        }

        public KeelSet(Ceiling ceiling)
        {
            this.ceiling = ceiling;
        }

        public void Clear()
        {
            this.mainKeelList.Clear();
            this.auxiKeelList.Clear();
        }

        public void SetDefaultDepth(uint depth, uint old_depth)
        {
            for (int i = 0; i < this.mainKeelList.Count; i++)
            {
                Keel keel = this.mainKeelList[i];
                if (keel.Depth == old_depth)
                    keel.Depth = depth;
            }
        }

        //刷新
        public void Refrush()
        {
            for (int i = 0; i < this.mainKeelList.Count; i++)
            {
                this.mainKeelList[i].PhysicsCoord(ceiling);
                this.mainKeelList[i].CalcRemarkLoca(ceiling);
            }
        }

        public KeelOrientation GetKeelOri(Point pt)
        {
            KeelOrientation orin = KeelOrientation.Auto;
            Keel _keel = null;
            List<Wall> walles = this.ceiling.Walles;

            if (this.mainKeelList.Count == 0)
            {
                double dist = 0, min_dist = 1000000;
                _keel = walles[0];

                for (int i = 0; i < walles.Count; i++)
                {
                    StrLine line = walles[i].PaintLine;
                    dist = line.Distance(pt);

                    if (Math.Abs(pt.X - line.Begin.X) > line.Length 
                        || Math.Abs(pt.X - line.End.X) > line.Length)
                        continue;
                    if (Math.Abs(pt.Y - line.Begin.Y) > line.Length 
                        || Math.Abs(pt.Y - line.End.Y) > line.Length)
                        continue;

                    if (min_dist > dist)
                    {
                        min_dist = dist;
                        _keel = walles[i];
                    }
                }

                if (Math.Abs(_keel.BeginPaintPoint.X - _keel.EndPaintPoint.X) 
                    > Math.Abs(_keel.BeginPaintPoint.Y - _keel.EndPaintPoint.Y))
                    orin = KeelOrientation.Vertical;
                else
                    orin = KeelOrientation.Horizontal;
            }
            else
            {
                orin = this.mainKeelList[0].GetOri();
            }

            return orin;
        }

        public Keel GetKeel(int index)
        {
            if (index < 0)
                return null;

            if (index < this.mainKeelList.Count)
                return this.mainKeelList[index];
            //index -= this.mainKeelList.Count;

            return null;
        }

        public int GetMainKeel(Point pt)
        {
            // 选中一条主龙骨
            for (int i = 0; i < this.mainKeelList.Count; i++)
            {
                //if (this.mainKeelList[i].PaintLine.Distance(pt) < 5)
                //    return i;
                if (Keel.IsInLine(this.mainKeelList[i].PaintLine, pt))
                    return i;
            }
            return -1;
        }

        private void _AddMainKeel(Keel _keel)
        {
            _keel.Adjust(ceiling);
            // _keel.Cut(ceiling);
            if (_keel.Length < float.MinValue)
                return;
            _keel.PhysicsCoord(this.ceiling);
            this.mainKeelList.Add(_keel);

            _keel.Depth = ceiling.Depth;
            _keel.CalcRemarkLoca(ceiling);
        }

        public void SetKeel(KeelOrientation orien)
        {
            this.mainKeelList.Clear();
            this.auxiKeelList.Clear();

            if ((orien == KeelOrientation.Auto && ceiling.Width <= ceiling.Height)
                || orien == KeelOrientation.Horizontal)
            {
                for (float i = 1000.0f; i + 50.0f < this.ceiling.Height; i += 1000.0f)
                {
                    Keel _keel = new Keel();
                    _keel.Begin = new PointF(this.ceiling.Left, this.ceiling.Top + i);
                    _keel.End = new PointF(this.ceiling.Right, this.ceiling.Top + i);
                    _AddMainKeel(_keel);
                }
            }
            else
            {
                for (float i = 1000.0f; i + 50.0f < this.ceiling.Width; i += 1000.0f)
                {
                    Keel _keel = new Keel();
                    _keel.Begin = new PointF(this.ceiling.Left + i, this.ceiling.Top);
                    _keel.End = new PointF(this.ceiling.Left + i, this.ceiling.Bottom);
                    _AddMainKeel(_keel);
                }
            }
        }

        public void Trans(int _angle, PointF ca)
        {
            for (int i = 0; i < this.mainKeelList.Count; i++)
            {
                this.mainKeelList[i].Trans(_angle, ca, ceiling);
            }
        }

        /// <summary>
        /// 该方法只改变边的显示位置，不改变逻辑位置，
        /// 只应用在整个图形的平移操作中
        /// </summary>
        /// <param name="delta">平移的偏移量</param>
        public void Translate(Point delta)
        {
            for (int i = 0; i < this.mainKeelList.Count; i++)
            {
                this.mainKeelList[i].Translate(delta);
            }
        }

        public void Draw(Graphics graphics, Pen keelPen, Pen arrowPen, 
            Font lengthFont)
        {
            for (int i = 0; i < mainKeelList.Count; i++)
            {
                mainKeelList[i].Draw(graphics, keelPen);
                mainKeelList[i].DrawRemark(graphics, lengthFont, 
                    this.ceiling.Depth, arrowPen);
            }
        }

        public Keel ParseKeel(string kstr)
        {
            MatchCollection pts = Regex.Matches(kstr, @"\((\d+\.?\d*),(\d+\.?\d*)\)");
            if (pts.Count != 2)
                return null;
            Keel keel = new Keel();

            keel.Begin = new PointF(float.Parse(pts[0].Groups[1].Value),
                float.Parse(pts[0].Groups[2].Value));
            keel.End = new PointF(float.Parse(pts[1].Groups[1].Value),
                float.Parse(pts[1].Groups[2].Value));
            keel.PhysicsCoord(ceiling);
            keel.Depth = ceiling.Depth;

            MatchCollection rms = Regex.Matches(kstr, @"\<(\d*)\$?([^\>]*)?\>");
            if (rms.Count > 0 && rms[0].Groups.Count > 1)
            {
                keel.Depth = uint.Parse(rms[0].Groups[1].Value);
                if (rms[0].Groups.Count == 3)
                    keel.RemarkStr = rms[0].Groups[2].Value;
            }

            return keel;
        }

        public void Unserialize(string str)
        {
            MatchCollection ms = Regex.Matches(str, @"#keel:\{([^\}]*)\}#");
            if (ms.Count == 0)
                return;
            MatchCollection ks = Regex.Matches(ms[0].Groups[1].Value, @"\[([^\]]*)\]");

            for (int i = 0; i < ks.Count; i++)
            {
                Keel keel = ParseKeel(ks[i].Groups[1].Value);
                if (keel != null)
                    this.mainKeelList.Add(keel);
            }
        }

        public string Serialize()
        {
            string ksstr = "#keel:{";

            for (int i = 0; i < this.mainKeelList.Count; i++)
            {
                Keel _keel = this.mainKeelList[i];
                string pts = "(" + _keel.Begin.X + "," + _keel.Begin.Y + "),"
                    + "(" + _keel.End.X + "," + _keel.End.Y + ")", astr = "";
                if (_keel.Depth != ceiling.Depth || _keel.RemarkStr.Length > 0)
                    astr += _keel.Depth.ToString();
                if (_keel.RemarkStr.Length > 0)
                    astr += "$" + _keel.RemarkStr;
                ksstr += "[" + pts + (astr.Length > 0 ? "<" + astr + ">" : "") + "]";
            }

            return ksstr + "}#";
        }

        private void _statisticSuspender(Keel keel, List<System.UInt32> counts,
            List<System.UInt32> models)
        {
            int flag = 0;

            for (int j = 0; j < models.Count; j++)
            {
                if (models[j] == keel.Depth)
                {
                    counts[j] += 2;
                    flag = 1;
                }
            }

            if (flag == 0)
            {
                counts.Add(2);
                models.Add(keel.Depth);
            }
        }

        private void _StatisticMainKeel(ProductSet pset)
        {
            List<System.UInt32> _counts = new List<System.UInt32>(),
                suspender_counts = new List<System.UInt32>(), 
                keel_models = new List<System.UInt32>(),
                suspender_models = new List<System.UInt32>();
            System.Int32 id = 0;
            System.UInt32 length = 0, count = 0;

            _counts.Add(0); keel_models.Add(3000);
            suspender_counts.Add(0); suspender_models.Add(this.ceiling.Depth);

            for (int i = 0; i < this.mainKeelList.Count; i++)
            {
                Keel _keel = this.mainKeelList[i];
                length = _keel.Length;

                while (length >= 3000)
                {
                    _counts[0]++;
                    length -= 3000;
                    _statisticSuspender(_keel, suspender_counts, 
                        suspender_models);
                }

                if (length == 0)
                    continue;
                int flag = 0;

                for (int j = 0; j < keel_models.Count; j++)
                {
                    if (keel_models[j] == length)
                    {
                        _counts[j]++;
                        flag = 1;
                        _statisticSuspender(_keel, suspender_counts, 
                            suspender_models);
                    }
                }

                if (flag == 0)
                {
                    _counts.Add(1);
                    keel_models.Add(length);
                    _statisticSuspender(_keel, suspender_counts, 
                        suspender_models);
                }
            }

            id = pset.FindByProductName("主龙骨");
            for (int i = 0; id > 0 && i < _counts.Count; i++)
            {
                if (_counts[i] == 0)
                    continue;
                pset.AddGood(id, _counts[i], 
                    keel_models[i].ToString() + "mm");
            }

            id = pset.FindByProductName("吊杆");
            for (int i = 0; id > 0 && i < suspender_counts.Count; i++)
            {
                if (suspender_counts[i] == 0)
                    continue;
                pset.AddGood(id, suspender_counts[i], 
                    suspender_models[i].ToString() + "mm");
                count += suspender_counts[i];
            }

            id = pset.FindByProductName("38吊件");
            if (id > 0)
                pset.AddGood(id, count);
        }

        private void _StatisticAuxiKeel(ProductSet pset)
        {
            List<System.UInt32> counts = new List<System.UInt32>(),
                models = new List<System.UInt32>();
            System.Int32 id = 0;
            System.UInt32 length = 0, count = 0, flag = 0;

            counts.Add(0); models.Add(3000);
            for (int i = 0; i < this.auxiKeelList.Count; i++)
            {
                Keel _keel = this.auxiKeelList[i];
                length = _keel.Length;

                while (length >= 3000)
                {
                    counts[0]++;
                    length -= 3000;
                    count++;
                }

                if (length <= 0)
                    continue;
                flag = 0;

                for (int j = 0; j < models.Count; j++)
                {
                    if (models[j] == length)
                    {
                        counts[j]++;
                        flag = 1;
                        count++;
                    }
                }

                if (flag == 0)
                {
                    counts.Add(1);
                    models.Add(length);
                    count++;
                }
            }

            id = pset.FindByProductName("三角龙骨");
            for (int i = 0; id > 0 && i < counts.Count; i++)
            {
                if (counts[i] > 0)
                    pset.AddGood(id, counts[i], models[i].ToString() + "mm");
            }
        }

        private void _Statistic吊件(ProductSet pset)
        {
            System.Int32 id = 0;
            System.UInt32 count = 0;

            for (int i = 0; i < this.auxiKeelList.Count; i++)
            {
                Keel auxi = this.auxiKeelList[i];

                for (int j = 0; j < this.mainKeelList.Count; j++)
                {
                    Keel main = this.mainKeelList[j];
                    if (main.Depth != ceiling.Depth)
                        continue;

                    PointF p = main.Intersect(auxi);
                    if (main.Contains(p, true)) 
                        count++;
                }
            }

            id = pset.FindByProductName("三角吊件");
            if (id > 0)
                pset.AddGood(id, count);
        }

        private void _AddAuxiKeel(Keel _keel)
        {
            _keel.Adjust(ceiling);
            // _keel.Cut(ceiling);
            if (_keel.Length < float.MinValue)
                return;
            _keel.PhysicsCoord(this.ceiling);
            this.auxiKeelList.Add(_keel);
        }

        public void Statistic(ProductSet pset)
        {
            if (this.mainKeelList.Count < 1)
                return;
            this.auxiKeelList.Clear();

            if (Math.Abs(this.mainKeelList[0].Alpha) < 0.01)
            {
                //this.SetAuxiKeelVertical();
                for (float i = 300.0f; i < this.ceiling.Width; i += 300.0f)
                {
                    Keel _keel = new Keel();
                    _keel.Begin = new PointF(this.ceiling.Left + i, this.ceiling.Top);
                    _keel.End = new PointF(this.ceiling.Left + i, this.ceiling.Bottom);
                    _AddAuxiKeel(_keel);
                }
            }
            else
            {
                //this.SetAuxiKeelHorizontal();
                for (float i = 300.0f; i < this.ceiling.Height; i += 300.0f)
                {
                    Keel _keel = new Keel();
                    _keel.Begin = new PointF(this.ceiling.Left, this.ceiling.Top + i);
                    _keel.End = new PointF(this.ceiling.Right, this.ceiling.Top + i);
                    _AddAuxiKeel(_keel);
                }
            }

            this._StatisticMainKeel(pset);
            this._StatisticAuxiKeel(pset);
            this._Statistic吊件(pset);
        }
    }
}
