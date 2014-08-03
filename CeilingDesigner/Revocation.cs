using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CeilingDesigner
{
    public class Revocation
    {
        public RevocationHandler handler;
        public RevocationEventArgs args;

        private static void AddRedo(RevocationEventArgs revo_arg,
            RevocationHandler handler, RevocationEventArgs arg)
        {
            revo_arg.Redos = arg.Revos;
            revo_arg.Revos = arg.Redos;

            Revocation revo = new Revocation();
            revo.handler = handler;
            revo.args = revo_arg;
            arg.Redos.Push(revo);
        }

        public static OrderGraphRevocationEventArgs GetRevoGraph(ProductSet pSet,
            Ceiling ceiling)
        {
            OrderGraphRevocationEventArgs revo = new OrderGraphRevocationEventArgs();

            if (ceiling != null)
            {
                for (int i = 0; i < ceiling.Walles.Count; i++)
                    revo.walles.Add(ceiling.Walles[i]);
                revo.name = ceiling.Name;
                revo.depth = ceiling.Depth;
            }

            if (pSet != null)
            {
                revo.tile = pSet.Tile.Clone();

                for (int i = 0; i < pSet.BuckleSet.AddedProducts.Count; i++)
                    revo.additional.Add(pSet.BuckleSet.AddedProducts[i]);
                for (int i = 0; i < pSet.KeelSet.MainKeelList.Count; i++)
                    revo.mains.Add(pSet.KeelSet.MainKeelList[i]);
            }

            return revo;
        }

        public static void UnTrans(object sender, RevocationEventArgs e)
        {
            if (!(e is TransRevocationEventArgs))
                return;
            TransRevocationEventArgs arg = e as TransRevocationEventArgs;
            OrderGraph graph = sender as OrderGraph;
            TransRevocationEventArgs revo_arg = new TransRevocationEventArgs();

            revo_arg.products = arg.products;
            revo_arg.trans = arg.trans * -1;
            AddRedo(revo_arg, Revocation.UnTrans, arg);

            if (arg.products != null && arg.products.Count > 0)
            {
                for (int i = 0; i < arg.products.Count; i++)
                {
                    graph.Invalidate(arg.products[i].DrawingRect);
                    arg.products[i].Transpose(arg.trans == 90 ? 1 : -1);
                }
                graph.ProductSet.BuckleSet.ReTileProducts();
                for (int i = 0; i < arg.products.Count; i++)
                {
                    graph.Invalidate(arg.products[i].DrawingRect);
                }
            }
            else
            {
                graph.InvalidateAll();
                graph.TransAll(arg.trans);
                graph.InvalidateAll();
            }
        }

        public static void ChangeGraph(object sender, RevocationEventArgs e)
        {
            if (!(e is OrderGraphRevocationEventArgs))
                return;
            OrderGraphRevocationEventArgs arg = e as OrderGraphRevocationEventArgs;
            OrderGraph graph = sender as OrderGraph;

            OrderGraphRevocationEventArgs revo_arg = GetRevoGraph(
                graph.ProductSet, graph.Ceiling);
            AddRedo(revo_arg, Revocation.ChangeGraph, arg);
            graph.InvalidateRect(graph.Ceiling.DrawingRect, 50);

            graph.ProductSet.Clear();
            graph.Ceiling.Clear();
            graph.Ceiling.Name = arg.name;
            graph.Ceiling.Depth = arg.depth;
            for (int i = 0; i < arg.walles.Count; i++)
                graph.Ceiling.AddWall(arg.walles[i]);

            if (graph.Ceiling.Length > 0)
            {
                for (int i = 0; i < arg.mains.Count; i++)
                    graph.ProductSet.KeelSet.MainKeelList.Add(arg.mains[i]);
                for (int i = 0; i < arg.additional.Count; i++)
                    graph.ProductSet.BuckleSet.AddedProducts.Add(arg.additional[i]);
                graph.ProductSet.Tile = arg.tile;
                graph.ProductSet.BuckleSet.ReTileProducts();
            }

            //graph.SetCeilingRect(graph.Ceiling.DrawingRect);
            graph.DisplayCeiling();
            (graph.ParentForm as PalaceForm).SetCeilingMenu(graph.Ceiling.Walles.Count > 0);
            graph.InvalidateRect(graph.Ceiling.DrawingRect, 50);
        }

        public static void Products(object sender, RevocationEventArgs e)
        {
            if (!(e is ProductRevocationEventArgs))
                return;
            ProductRevocationEventArgs arg = e as ProductRevocationEventArgs;
            ProductRevocationEventArgs revo_arg = new ProductRevocationEventArgs();

            List<AupuBuckle> drops = arg.productSet.BuckleSet.Revocate
                (arg.dropProducts, arg.add_Products, arg.moveProducts, arg.logic);
            revo_arg.add_Products = arg.dropProducts;
            for (int i = 0; i < drops.Count; i++)
                revo_arg.add_Products.Add(drops[i]);

            revo_arg.dropProducts = arg.add_Products;
            revo_arg.moveProducts = arg.moveProducts;
            revo_arg.logic = new PointF(-arg.logic.X, -arg.logic.Y);
            revo_arg.productSet = arg.productSet;
            AddRedo(revo_arg, Revocation.Products, arg);
            (sender as OrderGraph).Invalidate();
        }

        public static void UnTileProduct(object sender, RevocationEventArgs e)
        {
            if (!(e is TileRevocationEventArgs))
                return;
            OrderGraph graph = sender as OrderGraph;
            TileRevocationEventArgs arg = e as TileRevocationEventArgs;
            TileRevocationEventArgs revo_arg = new TileRevocationEventArgs();

            revo_arg.tile = graph.ProductSet.Tile;
            //revo_arg.tile = arg.buckleSet.Tile;
            //revo_arg.buckleSet = arg.buckleSet;
            AddRedo(revo_arg, Revocation.UnTileProduct, arg);

            graph.ProductSet.Tile = arg.tile;
            graph.ProductSet.BuckleSet.ReTileProducts();
            //arg.buckleSet.Tile = arg.tile;
            //arg.buckleSet.TileProducts();

            graph.InvalidateRect(graph.Ceiling.DrawingRect);
        }

        public static void ReTileProduct(object sender, RevocationEventArgs e)
        {
            if (!(e is TileRevocationEventArgs))
                return;
            OrderGraph graph = sender as OrderGraph;
            ClearProductsRevocationEventArgs arg = e as ClearProductsRevocationEventArgs;
            TileRevocationEventArgs revo_arg = new TileRevocationEventArgs();

            revo_arg.tile = graph.ProductSet.Tile;
            //revo_arg.tile = arg.buckleSet.Tile;
            //revo_arg.buckleSet = arg.buckleSet;
            AddRedo(revo_arg, Revocation.ClearProducts, arg);

            for (int i = 0; i < arg.addeds.Count; i++)
                graph.ProductSet.BuckleSet.AddedProducts.Add(arg.addeds[i]);
            graph.ProductSet.Tile = arg.tile;
            graph.ProductSet.BuckleSet.ReTileProducts();
            //arg.buckleSet.Tile = arg.tile;
                //arg.buckleSet.AddedProducts.Add(arg.addeds[i]);
            //arg.buckleSet.TileProducts();

            graph.InvalidateRect(graph.Ceiling.DrawingRect);
        }

        public static void ClearProducts(object sender, RevocationEventArgs e)
        {
            if (!(e is TileRevocationEventArgs))
                return;
            OrderGraph graph = sender as OrderGraph;
            TileRevocationEventArgs arg = e as TileRevocationEventArgs;
            ClearProductsRevocationEventArgs revo_arg = new ClearProductsRevocationEventArgs();

            revo_arg.tile = graph.ProductSet.Tile.Clone();
            //revo_arg.buckleSet = arg.buckleSet;
            BuckleSet buckleSet = graph.ProductSet.BuckleSet;
            for (int i = 0; i < buckleSet.AddedProducts.Count; i++)
                revo_arg.addeds.Add(buckleSet.AddedProducts[i]);
            AddRedo(revo_arg, Revocation.ReTileProduct, arg);

            graph.ProductSet.Tile.Clear();
            buckleSet.Clear();
            //arg.buckleSet.Clear();
            graph.InvalidateRect(graph.Ceiling.DrawingRect);
        }

        public static void ModifyKeel(object sender, RevocationEventArgs e)
        {
            if (!(e is ModifyKeelRevocationEventArgs))
                return;
            ModifyKeelRevocationEventArgs arg = e as ModifyKeelRevocationEventArgs;
            ModifyKeelRevocationEventArgs revo_arg = new ModifyKeelRevocationEventArgs();
            OrderGraph graph = sender as OrderGraph;

            revo_arg.keel = arg.keel;
            revo_arg.depth = arg.keel.Depth;
            revo_arg.remark = arg.keel.RemarkStr;

            revo_arg.begin = arg.keel.Begin;
            revo_arg.end = arg.keel.End;
            AddRedo(revo_arg, Revocation.ModifyKeel, arg);
            graph.InvalidateKeel(arg.keel);

            if (arg.keel.Begin != arg.begin || arg.keel.End != arg.end)
            {
                arg.keel.Begin = arg.begin;
                arg.keel.End = arg.end;
                arg.keel.PhysicsCoord((sender as OrderGraph).Ceiling);
            }

            if (arg.keel.Depth != arg.depth)
            {
                arg.keel.Depth = arg.depth;
            }
            if (arg.keel.RemarkStr.CompareTo(arg.remark) != 0)
            {
                arg.keel.RemarkStr = arg.remark;
            }

            arg.keel.CalcRemarkLoca((sender as OrderGraph).Ceiling);
            graph.InvalidateKeel(arg.keel);
        }

        public static void ModifyWall(object sender, RevocationEventArgs e)
        {
            if (!(e is ModifyWallRevocationEventArgs))
                return;
            ModifyWallRevocationEventArgs arg = e as ModifyWallRevocationEventArgs;
            ModifyWallRevocationEventArgs revo_arg = new ModifyWallRevocationEventArgs();
            OrderGraph graph = sender as OrderGraph;

            revo_arg.ceiling = arg.ceiling;
            revo_arg.wall = arg.wall;
            revo_arg.remark = arg.wall.RemarkStr;
            revo_arg.begin = arg.wall.Begin;
            revo_arg.end = arg.wall.End;
            AddRedo(revo_arg, Revocation.ModifyWall, arg);
            graph.InvalidateRect(graph.Ceiling.DrawingRect, 50);

            if (arg.wall.Begin != arg.begin || arg.wall.End != arg.end)
            {
                int index = arg.ceiling.Walles.IndexOf(arg.wall);

                if (arg.wall.Begin != arg.begin)
                {
                    PointF delta = new PointF(arg.begin.X - arg.wall.Begin.X, 
                        arg.begin.Y - arg.wall.Begin.Y);
                    arg.ceiling.MoveBeginPoint(index, delta);
                }

                if (arg.wall.End != arg.end)
                {
                    PointF delta = new PointF(arg.end.X - arg.wall.End.X,
                        arg.end.Y - arg.wall.End.Y);
                    arg.ceiling.MoveEndPoint(index, delta);
                }
            }

            if (arg.wall.RemarkStr.CompareTo(arg.remark) != 0)
            {
                arg.wall.RemarkStr = arg.remark;
                arg.wall.CalcRemarkLoca(arg.ceiling);
            }

            (sender as OrderGraph).AdjustCeiling();
            graph.InvalidateRect(graph.Ceiling.DrawingRect, 50);
        }

        public static void ModifySZoneWall(object sender, RevocationEventArgs e)
        {
            if (!(e is ModifySZoneWallRevocationEventArgs))
                return;
            OrderGraph graph = sender as OrderGraph;
            ModifySZoneWallRevocationEventArgs arg = e as ModifySZoneWallRevocationEventArgs;
            ModifySZoneWallRevocationEventArgs revo = new ModifySZoneWallRevocationEventArgs();

            revo.szone = arg.szone;
            revo.wall = arg.wall;
            revo.remark = arg.wall.RemarkStr;
            revo.begin = arg.wall.Begin;
            revo.end = arg.wall.End;

            int index = arg.szone.Walles.IndexOf(arg.wall);
            if (index > 0)
                revo.prev = arg.szone.Walles[index - 1].Begin;
            if (index < arg.szone.Walles.Count - 1)
                revo.next = arg.szone.Walles[index + 1].End;
            AddRedo(revo, Revocation.ModifySZoneWall, arg);
            arg.szone.Invalidate(graph);

            if (arg.wall.Begin != arg.begin || arg.wall.End != arg.end)
            {
                //int index = arg.szone.Walles.IndexOf(arg.wall);
                Wall wall = arg.wall;
                PointF bp = arg.begin, ep = arg.end;

                if (index > 0 && arg.prev != arg.szone.Walles[index - 1].Begin)
                {
                    wall = arg.szone.Walles[index - 1];
                    bp = arg.prev;
                    ep = arg.begin;
                }
                if (index < arg.szone.Walles.Count - 1 
                    && arg.next != arg.szone.Walles[index + 1].End)
                {
                    wall = arg.szone.Walles[index + 1];
                    bp = arg.end;
                    ep = arg.next;
                }
                arg.szone.ModifyWall(wall, (sender as OrderGraph).Ceiling, bp, ep);
            }

            if (arg.wall.RemarkStr.CompareTo(arg.remark) != 0)
            {
                arg.wall.RemarkStr = arg.remark;
                arg.wall.CalcRemarkLoca((sender as OrderGraph).Ceiling);
            }
            arg.szone.Invalidate(graph);
        }

        //graph.InvalidateRect(arg.szone.DispRect, 100);

        public static void ReSetKeel(object sender, RevocationEventArgs e)
        {
            if (!(e is SetKeelRevocationEventArgs))
                return;
            OrderGraph graph = sender as OrderGraph;
            SetKeelRevocationEventArgs arg = e as SetKeelRevocationEventArgs;
            SetKeelRevocationEventArgs revo_arg = new SetKeelRevocationEventArgs();

            revo_arg.keelSet = arg.keelSet;
            revo_arg.orien = arg.orien;
            AddRedo(revo_arg, Revocation.UnSetKeel, arg);
            arg.keelSet.SetKeel(arg.orien);
            graph.InvalidateRect(graph.Ceiling.DrawingRect);
        }

        public static void UnSetKeel(object sender, RevocationEventArgs e)
        {
            if (!(e is SetKeelRevocationEventArgs))
                return;
            OrderGraph graph = sender as OrderGraph;
            SetKeelRevocationEventArgs arg = e as SetKeelRevocationEventArgs;
            SetKeelRevocationEventArgs revo_arg = new SetKeelRevocationEventArgs();

            revo_arg.keelSet = arg.keelSet;
            revo_arg.orien = arg.orien;
            AddRedo(revo_arg, Revocation.ReSetKeel, arg);
            arg.keelSet.Clear();
            graph.InvalidateRect(graph.Ceiling.DrawingRect);
        }

        public static void DeleteKeel(object sender, RevocationEventArgs e)
        {
            if (!(e is EditKeelRevocationEventArgs))
                return;
            OrderGraph graph = sender as OrderGraph;
            EditKeelRevocationEventArgs arg = e as EditKeelRevocationEventArgs;
            EditKeelRevocationEventArgs revo_arg = new EditKeelRevocationEventArgs();

            revo_arg.container = arg.container;
            revo_arg.keel = arg.keel;
            AddRedo(revo_arg, Revocation.AddKeel, arg);

            arg.container.Remove(arg.keel);
            graph.InvalidateRect(arg.keel.PaintRect, 40);
        }

        public static void AddKeel(object sender, RevocationEventArgs e)
        {
            if (!(e is EditKeelRevocationEventArgs))
                return;
            OrderGraph graph = sender as OrderGraph;
            EditKeelRevocationEventArgs arg = e as EditKeelRevocationEventArgs;
            EditKeelRevocationEventArgs revo_arg = new EditKeelRevocationEventArgs();

            revo_arg.container = arg.container;
            revo_arg.keel = arg.keel;
            AddRedo(revo_arg, Revocation.DeleteKeel, arg);

            arg.container.Add(arg.keel);
            graph.InvalidateRect(arg.keel.PaintRect, 40);
        }

        public static void ModifySZone(object sender, RevocationEventArgs e)
        {
            if (!(e is ModifySZoneRevocationEventArgs))
                return;
            OrderGraph graph = sender as OrderGraph;
            ModifySZoneRevocationEventArgs arg = e as ModifySZoneRevocationEventArgs;
            ModifySZoneRevocationEventArgs revo_arg = new ModifySZoneRevocationEventArgs();

            //revo_arg.ceiling = arg.ceiling;
            revo_arg.szone = arg.szone;
            revo_arg.remark = arg.szone.RemarkStr;
            revo_arg.depth = arg.szone.Depth;
            AddRedo(revo_arg, Revocation.ModifySZone, arg);
            arg.szone.Invalidate(graph);
            //graph.InvalidateRect(arg.szone.DispRect, 100);

            if (arg.szone.Depth != arg.depth)
                arg.szone.Depth = arg.depth;
            if (arg.szone.RemarkStr.CompareTo(arg.remark) != 0)
                arg.szone.RemarkStr = arg.remark;

            arg.szone.CalcLoca(graph.Ceiling);
            arg.szone.Invalidate(graph);
            //graph.InvalidateRect(arg.szone.DispRect, 100);
        }

        public static void DeleteSZone(object sender, RevocationEventArgs e)
        {
            if (!(e is EditSZoneRevocationEventArgs))
                return;
            OrderGraph graph = sender as OrderGraph;
            EditSZoneRevocationEventArgs arg = e as EditSZoneRevocationEventArgs;

            EditSZoneRevocationEventArgs revo_arg = new EditSZoneRevocationEventArgs();
            revo_arg.szone = arg.szone;
            AddRedo(revo_arg, Revocation.AddSZone, arg);

            graph.SZones.Remove(arg.szone);
            arg.szone.Invalidate(graph);
            //graph.InvalidateRect(arg.szone.DispRect, 100);
        }

        public static void AddSZone(object sender, RevocationEventArgs e)
        {
            if (!(e is EditSZoneRevocationEventArgs))
                return;
            OrderGraph graph = sender as OrderGraph;
            EditSZoneRevocationEventArgs arg = e as EditSZoneRevocationEventArgs;

            EditSZoneRevocationEventArgs revo_arg = new EditSZoneRevocationEventArgs();
            revo_arg.szone = arg.szone;
            AddRedo(revo_arg, Revocation.DeleteSZone, arg);

            graph.SZones.Add(arg.szone);
            arg.szone.Invalidate(graph);
            //graph.InvalidateRect(arg.szone.DispRect, 100);
        }
    }

    public delegate void RevocationHandler(object sender, RevocationEventArgs e);

    public class RevocationEventArgs : EventArgs
    {
        public Stack<Revocation> Redos;
        public Stack<Revocation> Revos;

        public void Release() { }
    }

    public class ProductRevocationEventArgs : RevocationEventArgs
    {
        public ProductRevocationEventArgs() { }

        public List<AupuBuckle> dropProducts = new List<AupuBuckle>();
        public List<AupuBuckle> add_Products = new List<AupuBuckle>();
        public List<AupuBuckle> moveProducts = new List<AupuBuckle>();
        
        public PointF logic = new PointF(0, 0);
        public ProductSet productSet = null;
    }

    public class TileRevocationEventArgs : RevocationEventArgs
    {
        public TileRevocationEventArgs() { }

        //public BuckleSet buckleSet = null;
        public AupuTile tile = null;

        public new void Release()
        {
            if (tile != null)
                tile.Release();
        }
    }

    public class ClearProductsRevocationEventArgs : TileRevocationEventArgs
    {
        public ClearProductsRevocationEventArgs() { }

        public List<AupuBuckle> addeds = new List<AupuBuckle>();
    }

    public class ModifyRevocationEventArgs : RevocationEventArgs
    {
        public ModifyRevocationEventArgs() { }

        public PointF begin = new PointF(0, 0);
        public PointF end = new PointF(0, 0);

        public string remark = "";
    }

    public class ModifyWallRevocationEventArgs : ModifyRevocationEventArgs
    {
        public ModifyWallRevocationEventArgs() { }

        public Ceiling ceiling;
        public Wall wall = null;
    }

    public class ModifyKeelRevocationEventArgs : ModifyRevocationEventArgs
    {
        public ModifyKeelRevocationEventArgs() { }

        public Keel keel = null;
        public uint depth = 0;
    }

    public class ModifySZoneWallRevocationEventArgs : ModifyRevocationEventArgs
    {
        public ModifySZoneWallRevocationEventArgs() { }

        public SZone szone = null;
        public Wall wall = null;

        public PointF prev = new PointF(0, 0), next = new PointF(0, 0);
    }

    public class SetKeelRevocationEventArgs : RevocationEventArgs
    {
        public SetKeelRevocationEventArgs() { }

        public KeelSet keelSet = null;
        public KeelOrientation orien = KeelOrientation.Auto;
    }

    public class EditKeelRevocationEventArgs : RevocationEventArgs
    {
        public EditKeelRevocationEventArgs() { }

        public Keel keel = null;
        public List<Keel> container = null;
    }

    public class EditSZoneRevocationEventArgs : RevocationEventArgs
    {
        public EditSZoneRevocationEventArgs() { }

        public SZone szone = null;
    }

    public class ModifySZoneRevocationEventArgs : RevocationEventArgs
    {
        public ModifySZoneRevocationEventArgs() { }

        public SZone szone = null;
        public uint depth = 0;
        public string remark = "";
    }

    public class OrderGraphRevocationEventArgs : RevocationEventArgs // TileRevocationEventArgs
    {
        public OrderGraphRevocationEventArgs() { }

        public List<Wall> walles = new List<Wall>();
        public AupuTile tile = null;
        public List<AupuBuckle> additional = new List<AupuBuckle>();

        public string name = "";
        public uint depth = 0;
        public List<Keel> mains = new List<Keel>();
    }

    public class TransRevocationEventArgs : RevocationEventArgs
    {
        public TransRevocationEventArgs() { }

        public List<AupuBuckle> products = new List<AupuBuckle>();
        public int trans = 0;
    }
}

//(sender as OrderGraph).Invalidate();
//arg.prev = arg.szone.Walles[index - 1].Begin;
//arg.next = arg.szone.Walles[index + 1].End;
//(sender as OrderGraph).AdjustCeiling();
//(sender as OrderGraph).RefrushGraph();

//if (arg.buckleSet.Products != null)
//    revo_arg.products = arg.buckleSet.Products.Clone() as AupuBuckle[];
//arg.buckleSet.ReTileProducts(arg.products);

//public CeilingDataSet.ceilingsRow row = null;
//public ProductSet productSet = null;
//public AupuBuckle[] products = null;

//public int rows, columns;
//public float paintsize = 0;

//public ProductSet productSet = null;
//public Ceiling ceiling = null;
//public Keel keel = null;

//OrderGraphRevocationEventArgs revo_arg = new OrderGraphRevocationEventArgs();

//if (graph.ProductSet != null)
//{
//    if (graph.ProductSet.BuckleSet.Products != null)
//        revo_arg.products = graph.ProductSet.BuckleSet.Products.Clone() as AupuBuckle[];

//    revo_arg.columns = graph.ProductSet.BuckleSet.Columns;
//    revo_arg.rows = graph.ProductSet.BuckleSet.Rows;
//    revo_arg.paintsize = graph.ProductSet.BuckleSet.TileUnit;

//    for (int i = 0; i < graph.ProductSet.BuckleSet.AddedProducts.Count; i++)
//        revo_arg.additional.Add(graph.ProductSet.BuckleSet.AddedProducts[i]);
//    for (int i = 0; i < graph.ProductSet.KeelSet.MainKeelList.Count; i++)
//        revo_arg.mains.Add(graph.ProductSet.KeelSet.MainKeelList[i]);
//}
//if (graph.Ceiling != null)
//{
//    for (int i = 0; i < graph.Ceiling.Walles.Count; i++)
//        revo_arg.walles.Add(graph.Ceiling.Walles[i]);
//}

//revo_arg.productSet = graph.ProductSet;

//graph.Ceiling.SetDrawingRect(graph.Ceiling.DrawingRect);
//graph.Ceiling.RefrushRegion();
//(graph.ParentForm as PalaceForm).SetCeilingMenu(graph.Ceiling.Walles.Count > 0);

//if (arg.productSet == null)
//    return;
//graph.ProductSet.Clear();

//for (int i = 0; i < arg.mains.Count; i++)
//    graph.ProductSet.KeelSet.MainKeelList.Add(arg.mains[i]);
//for (int i = 0; i < arg.additional.Count; i++)
//    graph.ProductSet.BuckleSet.AddedProducts.Add(arg.additional[i]);
//graph.ProductSet.BuckleSet.Tile = arg.tile;
//graph.ProductSet.BuckleSet.TileProducts();

//if (arg.rows > 0 && arg.columns > 0)
//{
//    //graph.ProductSet.BuckleSet.Products = arg.products;
//}

//arg.productSet.Clear();
//if (arg.rows > 0 && arg.columns > 0)
//{
//    arg.productSet.BuckleSet.Products = arg.products;
//    for (int i = 0; i < arg.additional.Count; i++)
//        arg.productSet.BuckleSet.AddedProducts.Add(arg.additional[i]);
//    for (int i = 0; i < arg.mains.Count; i++)
//        arg.productSet.KeelSet.MainKeelList.Add(arg.mains[i]);
//}

//revo_arg.productSet = arg.productSet;
//revo_arg.ceiling = arg.ceiling;

//if (arg.products != null && arg.products.Count > 0)
//{
//    for (int i = 0; i < arg.products.Count; i++)
//        arg.products[i].Transpose();
//}
//else if (arg.keel != null)
//{
//    revo_arg.keel = arg.keel;
//    arg.keel.Trans(arg.trans, graph.Ceiling);
//}
//else
//{
//    Rectangle rect = graph.Ceiling.DrawingRect;
//    Point cd = new Point(rect.Left + rect.Width / 2,
//        rect.Top + rect.Height / 2);
//    PointF ca = new PointF(graph.Ceiling.Left + graph.Ceiling.Width / 2,
//        graph.Ceiling.Top + graph.Ceiling.Height / 2);
//    PointF ba = new PointF(graph.Ceiling.Left, graph.Ceiling.Top);

//    graph.Ceiling.Trans(arg.trans, ca, cd);
//    graph.ProductSet.Trans(arg.trans, ba, ca, cd);

//    for (int i = 0; i < graph.SZones.Count; i++)
//    {
//        graph.SZones[i].Trans(arg.trans, graph.Ceiling);
//    }
//}

//revo.columns = pSet.BuckleSet.Columns;
//revo.rows = pSet.BuckleSet.Rows;
//revo.paintsize = pSet.BuckleSet.TileUnit;
//if (pSet.BuckleSet.Products != null)
//    revo.products = pSet.BuckleSet.Products.Clone() as AupuBuckle[];

//revo.productSet = pSet;

//public ProductSet productSet = null;
//public List<Keel> main = new List<Keel>();

//revo_arg.productSet = arg.productSet;
//for (int i = 0; i < arg.productSet.KeelSet.MainKeelList.Count; i++)
//    revo_arg.main.Add(arg.productSet.KeelSet.MainKeelList[i]);
//arg.productSet.KeelSet.SetKeel(arg.orien);

//revo_arg.productSet = arg.productSet;
//KeelSet ks = arg.productSet.KeelSet;
//ks.Clear();
//for (int i = 0; i < arg.main.Count; i++)
//    ks.MainKeelList.Add(arg.main[i]);

//public class ModifyRevocationEventArgs : RevocationEventArgs
//{
//    public ModifyRevocationEventArgs() { }

//    //public Ceiling ceiling;
//    public Keel keel = null;

//    /// <summary>
//    /// 负值表示龙骨，零或正值表示墙壁，数值代表标号
//    /// </summary>
//    public int index = -1;

//    public PointF delta = new PointF(0, 0);
//    public float length = 0;
//    public uint depth = 0;
//    public string remark = "";
//}

//arg.productSet.KeelSet.Clear();
//for (int i = 0; i < arg.main.Count; i++)
//    arg.productSet.KeelSet.MainKeelList.Add(arg.main[i]);

//public static void AddGraph(object sender, RevocationEventArgs e)
//{
//    if (!(e is OrderGraphRevocationEventArgs))
//        return;
//}

//revo_arg.Redos = arg.Revos;
//revo_arg.Revos = arg.Redos;

//Revocation revo = new Revocation();
//revo.handler = Revocation.Products;
//revo.args = revo_arg;
//e.Redos.Push(revo);

//revo_arg.Redos = arg.Revos;
//revo_arg.Revos = arg.Redos;

//Revocation revo = new Revocation();
//revo.handler = Revocation.UnTileProduct;
//revo.args = revo_arg;
//e.Redos.Push(revo);

//arg.productSet.Products = arg.products;

//revo_arg.Redos = arg.Revos;
//revo_arg.Revos = arg.Redos;

//Revocation revo = new Revocation();
//revo.handler = Revocation.ClearProducts;
//revo.args = revo_arg;
//e.Redos.Push(revo);

//revo_arg.Redos = arg.Revos;
//revo_arg.Revos = arg.Redos;

//Revocation revo = new Revocation();
//revo.handler = Revocation.ReTileProduct;
//revo.args = revo_arg;
//e.Redos.Push(revo);

//public static void UnModifyKeel(object sender, RevocationEventArgs e)
//{
//    if (!(e is ModifyRevocationEventArgs))
//        return;

//    ModifyRevocationEventArgs arg = e as ModifyRevocationEventArgs;
//    ModifyRevocationEventArgs revo_arg = new ModifyRevocationEventArgs();

//    revo_arg.ceiling = arg.ceiling;
//    revo_arg.keel = arg.keel;
//    revo_arg.index = arg.index;
//    revo_arg.depth = arg.depth;
//    revo_arg.remark = arg.remark;
//    revo_arg.length = arg.keel.Length;
//    revo_arg.delta = new PointF(-arg.delta.X, -arg.delta.Y);
//    AddRedo(revo_arg, Revocation.UnModifyKeel, arg);

//    //revo_arg.Redos = arg.Revos;
//    //revo_arg.Revos = arg.Redos;

//    //Revocation revo = new Revocation();
//    //revo.args = revo_arg;
//    //revo.handler = Revocation.UnModifyKeel;
//    //e.Redos.Push(revo);

//    if (arg.index < 0)
//    {
//        if (arg.keel.Length != arg.length)
//            arg.keel.ChangeLenth(arg.length, arg.ceiling);
//    }
//    else
//    {
//        if (arg.keel.Length != arg.length)
//            arg.ceiling.Modify(arg.index, arg.length);
//    }

//    arg.keel.Depth = arg.depth;
//    arg.keel.Remark = arg.remark;
//}

//OrderGraph graph = sender as OrderGraph;
//graph.Change();
//graph.Invalidate();

//(sender as OrderGraph).SetKeel(arg.orien);

//Revocation revo = new Revocation();
//revo.handler = Revocation.UnSetKeel;
//revo_arg.Redos = arg.Revos;
//revo_arg.Revos = arg.Redos;
//revo.args = revo_arg;
//e.Redos.Push(revo);

//OrderGraph graph = sender as OrderGraph;
//graph.SetKeel(arg.orien);
//graph.RevoSetKeel(arg.orien);

//arg.productSet.KeelSet.MainKeelList.Clear();
//arg.productSet.KeelSet.AuxiKeelList.Clear();

//(sender as OrderGraph).Change();

//for (int i = 0; i < arg.auxi.Count; i++)
//    arg.productSet.KeelSet.AuxiKeelList.Add(arg.auxi[i]);

//revo_arg.Redos = arg.Revos;
//revo_arg.Revos = arg.Redos;

//Revocation revo = new Revocation();
//revo.handler = Revocation.ReSetKeel;
//revo.args = revo_arg;
//e.Redos.Push(revo);

//revo_arg.Redos = arg.Revos;
//revo_arg.Revos = arg.Redos;

//Revocation revo = new Revocation();
//revo.handler = Revocation.AddKeel;
//revo.args = revo_arg;
//e.Redos.Push(revo);

//revo_arg.Redos = arg.Revos;
//revo_arg.Revos = arg.Redos;

//Revocation revo = new Revocation();
//revo.handler = Revocation.DeleteKeel;
//revo.args = revo_arg;
//e.Redos.Push(revo);

//Revocation revo = new Revocation();
//revo.handler = Revocation.UnTrans;
//revo_arg.Redos = arg.Revos;
//revo_arg.Revos = arg.Redos;
//revo.args = revo_arg;
//e.Redos.Push(revo);

//Point cd = new Point(
//    arg.ceiling.DrawingRect.Left + arg.ceiling.DrawingRect.Width / 2,
//    arg.ceiling.DrawingRect.Top + arg.ceiling.DrawingRect.Height / 2);
//PointF ca = new PointF(arg.ceiling.Left + arg.ceiling.Width / 2, 
//    arg.ceiling.Top + arg.ceiling.Height / 2);
//revo_arg.keel = arg.keel;
//arg.keel.Trans(arg.trans, ca, cd, arg.ceiling);
//if (arg.trans > 0)
//    arg.keel.Trans(arg.trans, ca, cd, arg.ceiling);
//else
//    arg.keel.UnTrans(ca, cd, arg.ceiling);

//if (arg.trans < 0)
//{
//    arg.ceiling.UnTrans();
//}
//else if (arg.trans > 0)
//{
//    arg.ceiling.Trans();
//}

//revo_arg.Redos = arg.Revos;
//revo_arg.Revos = arg.Redos;

//Revocation revo = new Revocation();
//revo.handler = Revocation.ChangeGraph;
//revo.args = revo_arg;
//e.Redos.Push(revo);

//public static void BackMoveWall(object sender, RevocationEventArgs e)
//{
//    if (!(e is ModifyRevocationEventArgs))
//        return;

//    ModifyRevocationEventArgs arg = e as ModifyRevocationEventArgs;
//    ModifyRevocationEventArgs revo_arg = new ModifyRevocationEventArgs();

//    revo_arg.ceiling = arg.ceiling;
//    revo_arg.keel = arg.keel;
//    revo_arg.index = arg.index;

//    revo_arg.delta = new PointF(-arg.delta.X, -arg.delta.Y);
//    AddRedo(revo_arg, Revocation.BackMoveWall, arg);

//    //revo_arg.Redos = arg.Revos;
//    //revo_arg.Revos = arg.Redos;

//    //Revocation revo = new Revocation();
//    //revo.args = revo_arg;
//    //revo.handler = Revocation.BackMoveWall;
//    //e.Redos.Push(revo);

//    if (arg.index < 0)
//        arg.keel.Translate(arg.delta, arg.ceiling);
//    else
//        arg.ceiling.MoveWall(arg.index, arg.delta);
//}

//public static void BackBeginPoint(object sender, RevocationEventArgs e)
//{
//    if (!(e is ModifyRevocationEventArgs))
//        return;

//    ModifyRevocationEventArgs arg = e as ModifyRevocationEventArgs;
//    ModifyRevocationEventArgs revo_arg = new ModifyRevocationEventArgs();

//    revo_arg.ceiling = arg.ceiling;
//    revo_arg.keel = arg.keel;
//    revo_arg.index = arg.index;

//    revo_arg.delta = new PointF(-arg.delta.X, -arg.delta.Y);
//    AddRedo(revo_arg, Revocation.BackBeginPoint, arg);

//    //revo_arg.Redos = arg.Revos;
//    //revo_arg.Revos = arg.Redos;

//    //Revocation revo = new Revocation();
//    //revo.handler = Revocation.BackBeginPoint;
//    //revo.args = revo_arg;
//    //e.Redos.Push(revo);

//    if (arg.index < 0)
//        arg.keel.MoveBegin(arg.delta, arg.ceiling);
//    else
//        arg.ceiling.MoveBeginPoint(arg.index, arg.delta);
//}

//public static void BackEndPoint(object sender, RevocationEventArgs e)
//{
//    if (!(e is ModifyRevocationEventArgs))
//        return;

//    ModifyRevocationEventArgs arg = e as ModifyRevocationEventArgs;
//    ModifyRevocationEventArgs revo_arg = new ModifyRevocationEventArgs();

//    revo_arg.ceiling = arg.ceiling;
//    revo_arg.keel = arg.keel;
//    revo_arg.index = arg.index;

//    revo_arg.delta = new PointF(-arg.delta.X, -arg.delta.Y);
//    AddRedo(revo_arg, Revocation.BackEndPoint, arg);

//    //revo_arg.Redos = arg.Revos;
//    //revo_arg.Revos = arg.Redos;

//    //Revocation revo = new Revocation();
//    //revo.handler = Revocation.BackEndPoint;
//    //revo.args = revo_arg;
//    //e.Redos.Push(revo);

//    if (arg.index < 0)
//        arg.keel.MoveEnd(arg.delta, arg.ceiling);
//    else
//        arg.ceiling.MoveEndPoint(arg.index, arg.delta);
//}

//public OrderGraph graph = null;
//for (int i = 0; i < graph.ProductSet.KeelSet.AuxiKeelList.Count; i++)
//    revo_arg.auxis.Add(graph.ProductSet.KeelSet.AuxiKeelList[i]);
//arg.productSet.SetInfo(arg.paintsize, arg.paintsize,
//    arg.rows, arg.columns);
//for (int i = 0; i < arg.auxis.Count; i++)
//    arg.productSet.KeelSet.AuxiKeelList.Add(arg.auxis[i]);
//arg.productSet.RefrushLength();

//public static void UnChangeLength(object sender, RevocationEventArgs e)
//{
//    if (!(e is ModifyRevocationEventArgs))
//        return;

//    ModifyRevocationEventArgs arg = e as ModifyRevocationEventArgs;
//    Revocation revo = new Revocation();
//    ModifyRevocationEventArgs revo_arg = new ModifyRevocationEventArgs();

//    revo_arg.keel = arg.keel;
//    revo_arg.ceiling = arg.ceiling;
//    revo_arg.index = arg.index;
//    revo_arg.length = arg.keel.Length;
//    revo_arg.delta = new PointF(-arg.delta.X, -arg.delta.Y);
//    revo_arg.Redos = arg.Revos;
//    revo_arg.Revos = arg.Redos;

//    revo.args = revo_arg;
//    revo.handler = Revocation.UnChangeLength;
//    e.Redos.Push(revo);

//    if (arg.index > -1)
//        arg.ceiling.Modify(arg.index, arg.length);
//    else
//        arg.keel.ChangeLenth(arg.length);
//}

//public List<Point> locations = new List<Point>();
//public int rows, columns;
//public List<Keel> mains = new List<Keel>();
//public List<Keel> auxis = new List<Keel>();
//public float paintsize = 0;

//public List<Keel> auxi = new List<Keel>();
//public List<Keel> auxi = new List<Keel>();
//public AupuProduct[] products = null;

//public List<AupuProduct> additional = new List<AupuProduct>();
//public int rows, columns;
//public List<Keel> mains = new List<Keel>();
//public List<Keel> auxis = new List<Keel>();
//public float paintsize = 0;

//public static void MoveProducts(object sender, RevocationEventArgs e)
//{
//    if (!(e is ProductRevocationEventArgs))
//        return;

//    ProductRevocationEventArgs arg = e as ProductRevocationEventArgs;
//    Revocation revo = new Revocation();
//    ProductRevocationEventArgs revo_arg = new ProductRevocationEventArgs();
//    revo.handler = Revocation.MoveProducts;

//    for (int i = 0; i < arg.mainProducts.Count; i++)
//        arg.productSet.MoveProduct(arg.mainProducts[i], arg.delta);

//    //revo_arg.locations = arg.locations;
//    revo_arg.mainProducts = arg.mainProducts;
//    revo_arg.appeProducts = arg.appeProducts;
//    revo_arg.delta = new Point(-arg.delta.X, -arg.delta.Y);

//    revo_arg.productSet = arg.productSet;
//    revo_arg.Redos = arg.Revos;
//    revo_arg.Revos = arg.Redos;
//    revo.args = revo_arg;
//    e.Redos.Push(revo);
//}

//public static void IndeProducts(object sender, RevocationEventArgs e)
//{
//    if (!(e is ProductRevocationEventArgs))
//        return;

//    ProductRevocationEventArgs arg = e as ProductRevocationEventArgs;
//    Revocation revo = new Revocation();
//    ProductRevocationEventArgs revo_arg = new ProductRevocationEventArgs();

//    revo.handler = Revocation.IndeProducts;
//    for (int i = 0; i < arg.mainProducts.Count; i++)
//        arg.productSet.DropProduct(arg.mainProducts[i], arg.mainProducts[i].DrawingRect.Location);
//    for (int i = 0; i < arg.appeProducts.Count; i++)
//        arg.productSet.AddProduct(arg.appeProducts[i], arg.appeProducts[i].DrawingRect.Location);

//    //revo_arg.locations = arg.locations;
//    //revo_arg.products = arg.products;
//    //revo_arg.products.Add(arg.product);
//    //revo_arg.locations.Add(arg.product.DrawingRect.Location);
//    //revo_arg.product = null;

//    revo_arg.appeProducts = arg.mainProducts;
//    revo_arg.mainProducts = arg.appeProducts;
//    revo_arg.productSet = arg.productSet;

//    revo_arg.Redos = arg.Revos;
//    revo_arg.Revos = arg.Redos;
//    revo.args = revo_arg;
//    e.Redos.Push(revo);
//}

//public static void AddProducts(object sender, RevocationEventArgs e)
//{
//    if (!(e is ProductRevocationEventArgs))
//        return;

//    ProductRevocationEventArgs arg = e as ProductRevocationEventArgs;
//    Revocation revo = new Revocation();
//    ProductRevocationEventArgs revo_arg = new ProductRevocationEventArgs();
//    revo.handler = Revocation.DropProducts;
//    //revo.handler = Revocation.DeleteProducts;

//    for (int i = 0; i < arg.products.Count; i++)
//    {
//        List<AupuProduct> rs = arg.productSet.AddProduct(arg.products[i], arg.locations[i]);
//        for (int j = 0; j < rs.Count; j++)
//        {
//            revo_arg.products.Add(rs[j]);
//            revo_arg.locations.Add(rs[j].DrawingRect.Location);
//        }
//    }
//    arg.productSet.CancelSelect();

//    revo_arg.locations = arg.locations;
//    revo_arg.products = arg.products;
//    revo_arg.product = arg.products.Count > 0 ? arg.products[0] : null;
//    revo_arg.productSet = arg.productSet;

//    revo_arg.Redos = arg.Revos;
//    revo_arg.Revos = arg.Redos;
//    revo.args = revo_arg;
//    e.Redos.Push(revo);
//}

//public static void DeleteProducts(object sender, RevocationEventArgs e)
//{
//    if (!(e is ProductRevocationEventArgs))
//        return;

//    ProductRevocationEventArgs arg = e as ProductRevocationEventArgs;
//    Revocation revo = new Revocation();
//    ProductRevocationEventArgs revo_arg = new ProductRevocationEventArgs();
//    revo.handler = Revocation.AddProducts;

//    for (int i = 0; i < arg.products.Count; i++)
//        arg.productSet.DropProduct(arg.products[i], arg.locations[i]);

//    revo_arg.locations = arg.locations;
//    revo_arg.products = arg.products;
//    revo_arg.productSet = arg.productSet;
//    revo_arg.Redos = arg.Revos;
//    revo_arg.Revos = arg.Redos;
//    revo.args = revo_arg;
//    e.Redos.Push(revo);
//}
