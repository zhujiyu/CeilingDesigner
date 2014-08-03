using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace CeilingDesigner
{
    public partial class AupuReportForm : Form
    {
        private CeilingDataSet.ordersRow ordersRow = null;

        public static string ReportPhoto
        {
            get
            {
                return ShareData.GetTempPath() + "aupu_report";
                //return ShareData.GetTempPath() + "aupu_report.png";
            }
        }

        public AupuReportForm(CeilingDataSet.ordersRow _ordersRow)
        {
            InitializeComponent();
            this.reportViewer1.LocalReport.EnableExternalImages = true;
            this.ordersRow = _ordersRow;
        }

        public void RefrushData(List<OrderGraph> graphs)
        {
            DataView view = new DataView(ShareData.CeilingDataSet.good_view);
            view.Sort = "product_id desc";
            reportDataSet.Clear();

            for (int i = 0; i < view.Count; i++)
            {
                CeilingDataSet.good_viewRow grow = view[i].Row
                    as CeilingDataSet.good_viewRow;
                if (grow.id == 0 || grow.product_id == 0)
                    continue;

                ReportDataSet.reportGoodsRow rgrow = reportDataSet.reportGoods.FindByID(grow.id);
                if (rgrow == null)
                    rgrow = reportDataSet.reportGoods.NewreportGoodsRow();
                rgrow.BeginEdit();

                rgrow.ID = grow.id;
                rgrow.order_id = grow.order_id;
                rgrow.product_id = grow.product_id;

                rgrow.name = grow.name;
                rgrow.price = grow.price;
                rgrow.amount = grow.amount;
                rgrow.total = grow.amount * grow.price;

                if (!grow.IscategoryNull())
                    rgrow.category = grow.category;
                if (!grow.IsmodelNull())
                    rgrow.model = grow.model;

                rgrow.EndEdit();
                if (rgrow.RowState == DataRowState.Detached)
                    reportDataSet.reportGoods.AddreportGoodsRow(rgrow);
            }

            int gcount = graphs.Count / 2;
            for (int i = 0; i < gcount; i++)
            {
                ReportDataSet.PhotosRow row = reportDataSet.Photos.NewPhotosRow();
                row.BeginEdit();
                row.Address = ReportPhoto + i + ".png";
                row.EndEdit();
                reportDataSet.Photos.AddPhotosRow(row);
            }

            if (2 * gcount < graphs.Count)
            {
                ReportDataSet.PhotosRow row = reportDataSet.Photos.NewPhotosRow();
                row.BeginEdit();
                row.Address = ReportPhoto + gcount + ".png";
                row.EndEdit();
                reportDataSet.Photos.AddPhotosRow(row);
            }

            ReportParameter cmpName = new ReportParameter("cmpName", SettingFile.GetCmpName());
            ReportParameter rptName = new ReportParameter("rptName", SettingFile.GetRptName());
            ReportParameter svrTel = new ReportParameter("svrTel", SettingFile.GetPhoto());
            ReportParameter cmpAddress = new ReportParameter("cmpAddress", SettingFile.GetAddress());

            ReportParameter customer = new ReportParameter("customer",
                    ordersRow.IscustomerNull() ? "" : ordersRow.customer);
            ReportParameter address = new ReportParameter("address",
                    ordersRow.IsaddressNull() ? "" : ordersRow.address);
            ReportParameter phone = new ReportParameter("phone",
                    ordersRow.IsphoneNull() ? "" : ordersRow.phone);
            ReportParameter install_date = new ReportParameter("install_date",
                ordersRow.Isinstall_dateNull() ? "" : ordersRow.install_date.ToShortDateString());
            ReportParameter salesman = new ReportParameter("salesman",
                    ordersRow.IssalesmanNull() ? "" : ordersRow.salesman);
            ReportParameter remark = new ReportParameter("remark",
                    ordersRow.IsremarkNull() ? "" : ordersRow.remark);

            this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] 
            { 
                cmpName, rptName, svrTel, cmpAddress,
                customer, address, phone, install_date, salesman, remark
            });

            //this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] 
            //{
            //    new ReportParameter("cmpName", SettingFile.GetCmpName()), 
            //    new ReportParameter("rptName", SettingFile.GetRptName()), 
            //    new ReportParameter("svrTel",  SettingFile.GetPhoto()),
            //    new ReportParameter("cmpAddress", SettingFile.GetAddress()), 
                
            //    //new ReportParameter("imgPath", ReportPhoto + "0.png"),
            //    new ReportParameter("customer", 
            //        ordersRow.IscustomerNull() ? "" : ordersRow.customer),
            //    new ReportParameter("address", 
            //        ordersRow.IsaddressNull() ? "" : ordersRow.address),
            //    new ReportParameter("phone", 
            //        ordersRow.IsphoneNull() ? "" : ordersRow.phone),
            //    new ReportParameter("install_date", ordersRow.Isinstall_dateNull() 
            //        ? "" : ordersRow.install_date.ToShortDateString()),
            //    new ReportParameter("salesman", 
            //        ordersRow.IssalesmanNull() ? "" : ordersRow.salesman),
            //    new ReportParameter("remark", 
            //        ordersRow.IsremarkNull() ? "" : ordersRow.remark)
            //});
        }

        private void AupuReportForm_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();

            this.Text = "订单报表 - " + (ordersRow.IscustomerNull() ?
                "客户未命名" : ordersRow.customer) + " | " + ShareData.AppName;
        }
    }
}
