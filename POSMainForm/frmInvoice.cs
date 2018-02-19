using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Xml.Linq;

using MySql.Data.MySqlClient;
using Microsoft.Reporting.WinForms;

namespace POSMainForm
{
    public partial class frmInvoice : Form
    {
        int id;
        string customerName;

        string duePay;


        int discount = 0;
        string remainPay;
        string shopname;
        string address; 
        string genDesc;
        string contact;
        public frmInvoice(int i, string cname, string dp, string rp ,  int dis ,string shop , string add , string genD ,string cont)
        {
            InitializeComponent();

            id = i;
            genDesc = genD;
            address = add;
            shopname = shop;
            customerName = cname;
            contact = cont;

            duePay = dp;
            remainPay = rp;
            discount = dis;
        }

        private void frmReportDailSalesByInvoice_Load(object sender, EventArgs e)
        {

            //this.reportViewer1.RefreshReport();
            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
               // SQLConn.sqL = "SELECT T.InvoiceNo, ProductCode, TD.description_product as Description, REPLACE(TDate, '-', '/') as TDate, TTime,TD.ItemPrice, SUM(TD.Quantity) as totalQuantity, (TD.ItemPrice * SUM(TD.Quantity)) as TotalPrice , T.discount  FROM Product as P, Transactions as T, TransactionDetails as TD WHERE P.ProductNo = TD.ProductNo AND TD.InvoiceNo = T.InvoiceNo AND T.InvoiceNo =" + id + " ";
                SQLConn.sqL = "select a.InvoiceNo , TotalAmount as TotalPrice ,Quantity as totalQuantity ,ItemPrice ,TDate ,TTime ,description_product as Description ,ProductName as ProductCode from transactiondetails as a,transactions as b where a.InvoiceNo=b.InvoiceNo AND b.InvoiceNo = " + id + "";

               // MessageBox.Show(SQLConn.sqL);
                SQLConn.ConnDB();
                SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                SQLConn.da = new MySqlDataAdapter(SQLConn.cmd);

                this.dsReportC.Invoice.Clear();
                SQLConn.da.Fill(this.dsReportC.Invoice);

                this.reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
                this.reportViewer1.ZoomPercent = 25;
                this.reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;


                ReportParameter inputdata = new ReportParameter("customerName", customerName);
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { inputdata });

                ReportParameter inputdata2 = new ReportParameter("duePayment", duePay);
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { inputdata2 });

                ReportParameter inputdata3 = new ReportParameter("remainingpayment", remainPay);
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { inputdata3 });


                ReportParameter inputdata4 = new ReportParameter("discount", discount.ToString());
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { inputdata4 });


                ReportParameter inputdata5 = new ReportParameter("shopname", shopname);
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { inputdata5 });

                ReportParameter inputdata6 = new ReportParameter("address", address);
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { inputdata6 });

                ReportParameter inputdata7 = new ReportParameter("generalDesc", genDesc);
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { inputdata7 });

                ReportParameter inputdata8 = new ReportParameter("contact", contact);
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { inputdata8 });
                

                this.reportViewer1.RefreshReport();

            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.ToString());
            }
        }

        private void DailySalesByInvoiceBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void frmInvoice_Load(object sender, EventArgs e)
        {

        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {

        }
    }
}
