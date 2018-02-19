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
    public partial class frmReportDailSalesByCustomer : Form
    {

        DateTime ReportDate;
        DateTime ReportDate2;
        string cusName= "0";
        string cusID = "0";
        string duePayment = "0";
        public frmReportDailSalesByCustomer(DateTime reportDate, DateTime reportDate2, string cusI, string cusN, string duePay)
        {
            InitializeComponent();
            cusName = cusN;
            cusID = cusI;
            duePayment = duePay;
            ReportDate = reportDate;

            ReportDate2 = reportDate2;
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
                SQLConn.sqL = "SELECT T.Firstname, T.InvoiceNo, ProductCode, P.Description, REPLACE(TDate, '-', '/') as TDate, TTime,TD.ItemPrice, SUM(TD.Quantity) as totalQuantity, (TD.ItemPrice * SUM(TD.Quantity)) as TotalPrice  FROM Product as P, Transactions as T, TransactionDetails as TD WHERE P.ProductNo = TD.ProductNo AND TD.InvoiceNo = T.InvoiceNo AND  STR_TO_DATE(REPLACE(TDate, '-', '/'), '%m/%d/%Y') BETWEEN '" + ReportDate.ToString("yyyy-MM-dd") + "' AND '" + ReportDate2.ToString("yyyy-MM-dd") + "' AND  T.CusID = " + cusID + " GROUP BY T.InvoiceNo, P.ProductNo, TDate ORDER By TDate";

                SQLConn.ConnDB();
                SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                SQLConn.da = new MySqlDataAdapter(SQLConn.cmd);

                this.dsReportC.Customer.Clear();

                SQLConn.da.Fill(this.dsReportC.Customer);

                this.reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
                this.reportViewer1.ZoomPercent = 40;
                this.reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;



                ReportParameter inputdata = new ReportParameter("customerName", cusName);
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { inputdata });

                ReportParameter inputdata1 = new ReportParameter("duePayment", duePayment);
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { inputdata1 });

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
    }
}
