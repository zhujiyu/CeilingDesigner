using System;
using System.Collections.Generic;
using System.Linq;
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

//public void WriteData(CeilingDataSet.ceilingsRow row)
//{
//    row.BeginEdit();

//    if (row.RowState == System.Data.DataRowState.Detached)
//    {
//        row.keels = this.Serialize();
//    }
//    else
//    {
//        string strTemp = this.Serialize();
//        if (row.IskeelsNull() || row.keels != strTemp)
//            row.keels = strTemp;
//    }

//    row.EndEdit();
//}

//for (int i = 0; i < this.auxiKeelList.Count; i++)
//    this.auxiKeelList[i].Translate(delta);
//this.mainKeelList[i].Remark.CalcRect();

//public void DrawRemark(Graphics graphics, Pen arrowPen, Font lengthFont)
//{
//    List<Keel> aks = this.mainKeelList;

//    for (int j = 0; j < mainKeelList.Count; j++)
//    {
//        mainKeelList[j].DrawRemark(graphics, lengthFont, this.ceiling.Depth, arrowPen);
//    }
//}

//public void AddKeel(Keel _keel)
//{
//    _keel.Depth = this.CeilingDepth;
//    _keel.CalcRemarkLoca(this.ceiling);
//    this.MainKeelList.Add(_keel);
//}

//public void SetMainKeelVertical()
//{
//    for (float i = 1000.0f; i + 50.0f < this.ceiling.Width; i += 1000.0f)
//    {
//        Keel _keel = new Keel();
//        _keel.Begin = new PointF(this.ceiling.Left + i, this.ceiling.Top);
//        _keel.End = new PointF(this.ceiling.Left + i, this.ceiling.Bottom);

//        _keel.Adjust(ceiling);
//        // _keel.Cut(ceiling);
//        if (_keel.Length < float.MinValue)
//            continue;
//        _keel.PhysicsCoord(this.ceiling);

//        this.mainKeelList.Add(_keel);
//        _keel.Depth = ceiling.Depth;
//        _keel.CalcRemarkLoca(ceiling);
//    }
//}

//public void SetMainKeelHorizontal()
//{
//    for (float i = 1000.0f; i + 50.0f < this.ceiling.Height; i += 1000.0f)
//    {
//        Keel _keel = new Keel();
//        _keel.Begin = new PointF(this.ceiling.Left, this.ceiling.Top + i);
//        _keel.End = new PointF(this.ceiling.Right, this.ceiling.Top + i);

//        _keel.Adjust(ceiling);
//        // _keel.Cut(ceiling);
//        if (_keel.Length < float.MinValue)
//            continue;
//        _keel.PhysicsCoord(this.ceiling);

//        this.mainKeelList.Add(_keel);
//        _keel.Depth = ceiling.Depth;
//        _keel.CalcRemarkLoca(ceiling);
//    }
//}

//public void SetAuxiKeelHorizontal()
//{
//    for (float i = 300.0f; i < this.ceiling.Height; i += 300.0f)
//    {
//        Keel _keel = new Keel();
//        _keel.Begin = new PointF(this.ceiling.Left, this.ceiling.Top + i);
//        _keel.End = new PointF(this.ceiling.Right, this.ceiling.Top + i);

//        _keel.Adjust(ceiling);
//        // _keel.Cut(ceiling);
//        if (_keel.Length < float.MinValue)
//            continue;
//        _keel.PhysicsCoord(this.ceiling);
//        this.auxiKeelList.Add(_keel);
//    }
//}

//public void SetAuxiKeelVertical()
//{
//    for (float i = 300.0f; i < this.ceiling.Width; i += 300.0f)
//    {
//        Keel _keel = new Keel();
//        _keel.Begin = new PointF(this.ceiling.Left + i, this.ceiling.Top);
//        _keel.End = new PointF(this.ceiling.Left + i, this.ceiling.Bottom);

//        _keel.Adjust(ceiling);
//        // _keel.Cut(ceiling);
//        if (_keel.Length < float.MinValue)
//            continue;
//        _keel.PhysicsCoord(this.ceiling);
//        this.auxiKeelList.Add(_keel);
//    }
//}

//public void RevoSetKeel(KeelOrientation orien, SetKeelRevocationEventArgs revo_arg)
//{
//    this.mainKeelList.Clear();
//    this.auxiKeelList.Clear();

//    if ((orien == KeelOrientation.Auto && this.ceiling.Width <= this.ceiling.Height)
//        || orien == KeelOrientation.Horizontal)
//    {
//        this.SetMainKeelHorizontal();
//    }
//    else
//    {
//        this.SetMainKeelVertical();
//    }
//}

//for (int i = 0; i < this.mainKeelList.Count; i++)
//    revo_arg.main.Add(this.mainKeelList[i]);
//for (int i = 0; i < this.auxiKeelList.Count; i++)
//    revo_arg.auxi.Add(this.auxiKeelList[i]);

//this.SetAuxiKeelVertical();
//this.SetAuxiKeelHorizontal();
//for (int i = 0; i < this.auxiKeelList.Count; i++)
//{
//    this.auxiKeelList[i].Trans(ca, cd);
//}

//public void KeelUnserialize(string str)
//{
//    MatchCollection ms = Regex.Matches(str, @"#keel:\{[^\}]*\}#");
//    //MatchCollection ms = Regex.Matches(str, @"#keel:\{([\d\[\]\,\.\(\)]+)*\}#");
//    if (ms.Count == 0)
//        return;
//    ms = Regex.Matches(str, @"\[\(\d+(\.\d+)?,\d+(\.\d+)?\),\(\d+(\.\d+)?,\d+(\.\d+)?\)\]");

//    for (int i = 0; i < ms.Count; i++)
//    {
//        MatchCollection datas = Regex.Matches(ms[i].Value, @"\d+(\.\d*)?");
//        if (datas.Count != 4)
//            continue;
//        Keel keel = new Keel();

//        keel.Begin = new PointF(float.Parse(datas[0].Value), 
//            float.Parse(datas[1].Value));
//        keel.End = new PointF(float.Parse(datas[2].Value), 
//            float.Parse(datas[3].Value));
//        keel.PhysicsCoord(ceiling);

//        keel.Depth = ceiling.Depth;

//        this.mainKeelList.Add(keel);
//    }
//}

//int length = 0, flag = 0;
//List<System.UInt32> keel_models = new List<System.UInt32>(),
//    suspender_models = new List<System.UInt32>();

//if (id > 0)
//    pset.AddGood(id, 2 * count, this.ceiling_depth.ToString() + "mm");
////pset.AddGood(id, 2 * count, this.ceiling.Depth.ToString() + "mm");
//    pset.AddGood(id, 2 * count);

///// <summary>
///// 移动主龙骨时，对移动范围进行限制，
///// 限定当前的龙骨，不能移动到墙外，不能和其他龙骨重合
///// </summary>
///// <param name="_keel">要移动的龙骨</param>
///// <param name="delta">位移</param>
///// <returns>是否在可移动的范围，true是可以移动，false是不能移动</returns>
//public bool MoveKeel(Keel _keel, PointF delta)
//{
//    if (Math.Abs(delta.X) + Math.Abs(delta.Y) < 3)
//        return true;

//    int dis = (int)(100 / this.ceiling.Scale);
//    Point center = _keel.CenterPaintPoint;
//    PointF dp = new PointF(delta.X + center.X, delta.Y + center.Y);

//    if (!this.ceiling.Contain(Point.Round(dp), dis))
//        return false;
//    for (int i = 0; i < this.mainKeelList.Count; i++)
//    {
//        if (_keel == this.mainKeelList[i])
//            continue;
//        if (this.mainKeelList[i].PaintLine.Distance(dp) < dis)
//            return false;
//    }

//    return true;
//}

//private void AdjustKeel(Keel _keel)
//{
//    PointF temp; Keel tempSeg;
//    double d2, td2;
//    int b = 0, e = 0;
//    List<PointF> inters = new List<PointF>();

//    // 先去掉龙骨和墙壁重叠的部分
//    for (int i = 0; i < this.ceiling.Length; i++)
//    {
//        tempSeg = this.ceiling.Walles[i];// walls[i];
//        //double delta = Math.Abs(_keel.Alpha - tempSeg.Alpha);
//        if (Math.Abs(_keel.Alpha - tempSeg.Alpha) > float.Epsilon
//            || _keel.Distance(tempSeg.Begin) > 1)
//            continue;
//        _keel.EraseCoincide(_keel, tempSeg);
//        //this.EraseCoincide(_keel, _keel, tempSeg);
//    }

//    // 去掉处在房屋外面的部分，只处理两边端点在屋外的部分，
//    // 不处理龙骨中间部分处在屋外的情况
//    for (int i = 0; i < this.ceiling.Length; i++)
//    {
//        tempSeg = this.ceiling.Walles[i];// walls[i];
//        if (tempSeg.Contains(_keel.Begin))
//        {
//            b++;
//        }
//        else if (tempSeg.Contains(_keel.End))
//        {
//            e++;
//        }
//        else
//        {
//            temp = _keel.Intersect(tempSeg);
//            if (_keel.Contains(temp, true) && tempSeg.Contains(temp, true))
//                inters.Add(temp);
//        }
//    }

//    if (b == 0)
//    {
//        d2 = _keel.Length * _keel.Length;
//        temp = _keel.End;
//        for (int i = inters.Count - 1; i >= 0; i--)
//        {
//            td2 = StrLine.Distance2(_keel.Begin, inters[i]);
//            if (td2 < d2)
//            {
//                temp = inters[i];
//                d2 = td2;
//            }
//        }
//        _keel.Begin = temp;
//    }

//    if (e == 0)
//    {
//        d2 = _keel.Length * _keel.Length;
//        temp = _keel.Begin;
//        for (int i = 0; i < inters.Count; i++)
//        {
//            td2 = StrLine.Distance2(_keel.End, inters[i]);
//            if (td2 < d2)
//            {
//                temp = inters[i];
//                d2 = td2;
//            }
//        }
//        _keel.End = temp;
//    }
//}

//keels += "[(" + this.mainKeelList[i].Begin.X + ","
//    + this.mainKeelList[i].Begin.Y + "),("
//    + this.mainKeelList[i].End.X + ","
//    + this.mainKeelList[i].End.Y + ")]";

//keel.BeginPaintPoint = new Point(ceiling.DrawingRect.X + (int)((keel.Begin.X - ceiling.Left) / ceiling.Scale), 
//    ceiling.DrawingRect.Y + (int)((keel.Begin.Y - ceiling.Top) / ceiling.Scale));
//keel.EndPaintPoint = new Point(ceiling.DrawingRect.X + (int)((keel.End.X - ceiling.Left) / ceiling.Scale), 
//    ceiling.DrawingRect.Y + (int)((keel.End.Y - ceiling.Top) / ceiling.Scale));

//_keel = this.MainKeelList[0];

//if (Math.Abs(_keel.BeginPaintPoint.X - _keel.EndPaintPoint.X) 
//    > Math.Abs(_keel.BeginPaintPoint.Y - _keel.EndPaintPoint.Y))
//    orin = KeelOrientation.Horizontal;
//else
//    orin = KeelOrientation.Vertical;

//public void FillKeel(Keel keel, Point p)
//{
//    if (GetKeelOri(p) == KeelOrientation.Horizontal)
//    {
//        keel.Begin = new PointF(this.ceiling.Left,
//            this.ceiling.Top + (p.Y - this.ceiling.DrawingRect.Top) * this.ceiling.Scale);
//        keel.End = new PointF(this.ceiling.Right,
//            this.ceiling.Top + (p.Y - this.ceiling.DrawingRect.Top) * this.ceiling.Scale);
//        keel.BeginPaintPoint = new Point(this.ceiling.DrawingRect.Left, p.Y);
//        keel.EndPaintPoint = new Point(this.ceiling.DrawingRect.Right, p.Y);
//    }
//    else
//    {
//        keel.Begin = new PointF(this.ceiling.Left + (p.X - this.ceiling.DrawingRect.Left) * this.ceiling.Scale,
//            this.ceiling.Top);
//        keel.End = new PointF(this.ceiling.Left + (p.X - this.ceiling.DrawingRect.Left) * this.ceiling.Scale,
//            this.ceiling.Bottom);
//        keel.BeginPaintPoint = new Point(p.X, this.ceiling.DrawingRect.Top);
//        keel.EndPaintPoint = new Point(p.X, this.ceiling.DrawingRect.Bottom);
//    }
//    keel.Cut(ceiling);
//}

//// 先去掉龙骨和墙壁重叠的部分
//private Keel EraseCoincide(Keel _keel, StrLine seg, StrLine tempSeg)
//{
//    PointF p0 = seg.Begin, p1;
//    bool b0 = tempSeg.Contains(p0), b1;

//    // 去掉开头的重合部分，如果有
//    for (double r = 10; r < seg.Length && b0; r += 10)
//    {
//        p1 = seg.GetPoint(r);
//        b1 = tempSeg.Contains(p1);

//        if (!b1) break;
//        b0 = b1;
//        p0 = p1;
//    }
//    _keel.Begin = p0;

//    // 去掉结尾的重合部分，如果有
//    p0 = seg.End;
//    b0 = tempSeg.Contains(p0);
//    for (double r = seg.Length - 10; r > 0 && b0; r -= 10)
//    {
//        p1 = seg.GetPoint(r);
//        b1 = tempSeg.Contains(p1);

//        if (!b1) break;
//        b0 = b1;
//        p0 = p1;
//    }
//    _keel.End = p0;

//    // 去掉中间的重合部分，如果有
//    // 暂不做处理了
//    return _keel;
//}

//_keel.Move(delta, moving, ceiling);
//_keel.Cut(ceiling);

//private void _SetKeelBeginPaintPoint(Keel _keel, Point point)
//{
//    //InvalidateRect(_keel.PaintRect);
//    _keel.BeginPaintPoint = point;
//    _keel.Begin = new PointF(this.ceiling.Left + this.ceiling.Scale * (_keel.BeginPaintPoint.X - this.ceiling.DrawingRect.Left),
//        this.ceiling.Top + this.ceiling.Scale * (_keel.BeginPaintPoint.Y - this.ceiling.DrawingRect.Top));
//    //InvalidateRect(_keel.PaintRect);
//}

//private void _SetKeelEndPaintPoint(Keel _keel, Point point)
//{
//    //InvalidateRect(_keel.PaintRect);
//    _keel.EndPaintPoint = point;
//    _keel.End = new PointF(this.ceiling.Left + this.ceiling.Scale * (_keel.EndPaintPoint.X - this.ceiling.DrawingRect.Left),
//        this.ceiling.Top + this.ceiling.Scale * (_keel.EndPaintPoint.Y - this.ceiling.DrawingRect.Top));
//    //InvalidateRect(_keel.PaintRect);
//}

//Rectangle rect = _keel.PaintRect;
//Point center = _keel.CenterPaintPoint;
//PointF dp = new PointF(delta.X + center.X, delta.Y + center.Y);

////InvalidateRect(_keel.PaintRect, 30);
//if (moving == 1)
//{
//    this._SetKeelBeginPaintPoint(_keel, 
//        Point.Round(new PointF(_keel.BeginPaintPoint.X + delta.X, 
//            _keel.BeginPaintPoint.Y + delta.Y)));
//}
//else if (moving == 2)
//{
//    this._SetKeelBeginPaintPoint(_keel, 
//        Point.Round(new PointF(_keel.BeginPaintPoint.X + delta.X, 
//            _keel.BeginPaintPoint.Y + delta.Y)));
//    this._SetKeelEndPaintPoint(_keel, 
//        Point.Round(new PointF(_keel.EndPaintPoint.X + delta.X, 
//            _keel.EndPaintPoint.Y + delta.Y)));
//}
//else if (moving == 3)
//{
//    this._SetKeelEndPaintPoint(_keel, 
//        Point.Round(new PointF(_keel.EndPaintPoint.X + delta.X, 
//            _keel.EndPaintPoint.Y + delta.Y)));
//}
//_keel.CutKeel(ceiling);
//InvalidateRect(_keel.PaintRect, 30);

//if ((_keel.Begin.X - _keel.End.X) * (_keel.Begin.X - _keel.End.X) + (_keel.Begin.Y - _keel.End.Y) * (_keel.Begin.Y - _keel.End.Y) < float.MinValue)
//    continue;
//_keel.BeginPaintPoint = new Point((int)(this.ceiling.DrawingRect.Left + (_keel.Begin.X - this.ceiling.Left) / this.ceiling.Scale),
//    (int)(this.ceiling.DrawingRect.Top + (_keel.Begin.Y - this.ceiling.Top) / this.ceiling.Scale));
//_keel.EndPaintPoint = new Point((int)(this.ceiling.DrawingRect.Left + (_keel.End.X - this.ceiling.Left) / this.ceiling.Scale),
//    (int)(this.ceiling.DrawingRect.Top + (_keel.End.Y - this.ceiling.Top) / this.ceiling.Scale));

//if ((_keel.Begin.X - _keel.End.X) * (_keel.Begin.X - _keel.End.X) + (_keel.Begin.Y - _keel.End.Y) * (_keel.Begin.Y - _keel.End.Y) < float.MinValue)
//    continue;
//_keel.BeginPaintPoint = new Point((int)(this.ceiling.DrawingRect.Left + (_keel.Begin.X - this.ceiling.Left) / this.ceiling.Scale),
//    (int)(this.ceiling.DrawingRect.Top + (_keel.Begin.Y - this.ceiling.Top) / this.ceiling.Scale));
//_keel.EndPaintPoint = new Point((int)(this.ceiling.DrawingRect.Left + (_keel.End.X - this.ceiling.Left) / this.ceiling.Scale),
//    (int)(this.ceiling.DrawingRect.Top + (_keel.End.Y - this.ceiling.Top) / this.ceiling.Scale));

//if ((_keel.Begin.X - _keel.End.X) * (_keel.Begin.X - _keel.End.X) + (_keel.Begin.Y - _keel.End.Y) * (_keel.Begin.Y - _keel.End.Y) < float.MinValue)
//    continue;
//_keel.BeginPaintPoint = new Point((int)(this.ceiling.DrawingRect.Left + (_keel.Begin.X - this.ceiling.Left) / this.ceiling.Scale),
//    (int)(this.ceiling.DrawingRect.Top + (_keel.Begin.Y - this.ceiling.Top) / this.ceiling.Scale));
//_keel.EndPaintPoint = new Point((int)(this.ceiling.DrawingRect.Left + (_keel.End.X - this.ceiling.Left) / this.ceiling.Scale),
//    (int)(this.ceiling.DrawingRect.Top + (_keel.End.Y - this.ceiling.Top) / this.ceiling.Scale));

//if ((_keel.Begin.X - _keel.End.X) * (_keel.Begin.X - _keel.End.X) + (_keel.Begin.Y - _keel.End.Y) * (_keel.Begin.Y - _keel.End.Y) < float.MinValue)
//    continue;
//_keel.BeginPaintPoint = new Point((int)(this.ceiling.DrawingRect.Left + (_keel.Begin.X - this.ceiling.Left) / this.ceiling.Scale),
//    (int)(this.ceiling.DrawingRect.Top + (_keel.Begin.Y - this.ceiling.Top) / this.ceiling.Scale));
//_keel.EndPaintPoint = new Point((int)(this.ceiling.DrawingRect.Left + (_keel.End.X - this.ceiling.Left) / this.ceiling.Scale),
//    (int)(this.ceiling.DrawingRect.Top + (_keel.End.Y - this.ceiling.Top) / this.ceiling.Scale));

//Keel _keel = this.MainKeelList[i];
//_keel.KeelPhysicsCoord(this.ceiling);
//_keel.CutKeel(ceiling);
//_keel.BeginPaintPoint = new Point(this.ceiling.DrawingRect.Left + (int)((_keel.Begin.X - this.ceiling.Left) / this.ceiling.Scale),
//    this.ceiling.DrawingRect.Top + (int)((_keel.Begin.Y - this.ceiling.Top) / this.ceiling.Scale));
//_keel.EndPaintPoint = new Point(this.ceiling.DrawingRect.Left + (int)((_keel.End.X - this.ceiling.Left) / this.ceiling.Scale),
//    this.ceiling.DrawingRect.Top + (int)((_keel.End.Y - this.ceiling.Top) / this.ceiling.Scale));

//private double distance(PointF p1, PointF p2)
//{
//    double dx = p1.X - p2.X, dy = p1.Y - p2.Y;
//    return dx * dx + dy * dy;
//}

//public void CutKeel(Keel _keel)
//{
//    PointF r, r1 = _keel.Begin, r2 = _keel.End;
//    List<Wall> walles = this.ceiling.Walles;

//    for (int i = 0; i < walles.Count; i++)
//    {
//        if (_keel.PaintLine.IsParallel(walles[i].PaintLine))
//            continue;

//        r = _keel.Intersect(walles[i]);
//        if (walles[i].Contains(r, true) && _keel.Contains(r, true))
//        {
//            if (distance(r, _keel.Begin) < distance(r, _keel.End))
//                r1 = r;
//            else
//                r2 = r;
//        }
//    }

//    _keel.Begin = r1; _keel.End = r2;
//    _keel.KeelPhysicsCoord(ceiling);
//}

//if (_keel == null)
//    return;

//_keel.BeginPaintPoint = new Point((int)Math.Round(this.ceiling.DrawingRect.X + (r1.X - ceiling.Left) / ceiling.Scale),
//    (int)Math.Round(ceiling.DrawingRect.Y + (r1.Y - ceiling.Top) / ceiling.Scale));
//_keel.EndPaintPoint = new Point((int)Math.Round(this.ceiling.DrawingRect.X + (r2.X - ceiling.Left) / ceiling.Scale),
//    (int)Math.Round(ceiling.DrawingRect.Y + (r2.Y - ceiling.Top) / ceiling.Scale));

//double d1 = distance(r, _keel.Begin);
//double d2 = distance(r, _keel.End);
//if( d1 < d2)
//    r1 = r;
//else
//    r2 = r;

//for (i = 0; i < walles.Count; i++)
//{
//    if (_keel.PaintLine.IsParallel(walles[i].PaintLine))
//        continue;

//    r = _keel.Intersect(walles[i]);
//    if (walles[i].Contains(r, true))
//    {
//        if (_keel.Contains(r, true))
//            r1 = r;
//        break;
//    }
//    //if (walles[i].Contains(r, true) && _keel.Contains(r, true))
//    //{
//    //    r1 = r;
//    //    break;
//    //}
//}

//for (i++; i < walles.Count; i++)
//{
//    if (_keel.PaintLine.IsParallel(walles[i].PaintLine))
//        continue;

//    r = _keel.Intersect(walles[i]);
//    if (walles[i].Contains(r, true))
//    {
//        if (_keel.Contains(r, true))
//            r2 = r;
//        break;
//    }
//    //if (walles[i].Contains(r, true) && _keel.Contains(r, true))
//    //{
//    //    r2 = r;
//    //    break;
//    //}
//}

//private void ParseData()
//{
//    if (this.MainKeelList.Count > 0)
//    {
//        Keel keel = this.MainKeelList[0];
//        if (Math.Abs(keel.Alpha) < 0.01)
//            this.SetAuxiKeelVertical();
//        else
//            this.SetAuxiKeelHorizontal();
//    }
//}
