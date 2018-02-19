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

namespace POSMainForm
{
    public partial class frmListDetailinvoice : Form
    {

        
        public int invoiceID;

        public frmListDetailinvoice(int i)
        {

            InitializeComponent();
            invoiceID = i;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadInvoiceDetail(invoiceID.ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      

        public void LoadInvoiceDetail(string search)
        {
            try
            {
                SQLConn.sqL = "SELECT description_product,ProductNo,ItemPrice,Quantity,ProductName,InvoiceNo FROM transactiondetails WHERE InvoiceNo LIKE '" + search.Trim() + "%' ORDER By InvoiceNo";
                SQLConn.ConnDB();
                SQLConn.cmd = new MySqlCommand (SQLConn.sqL, SQLConn.conn);
                SQLConn.dr = SQLConn.cmd.ExecuteReader ();

                ListViewItem x = null;
                ListView1.Items.Clear();

                while (SQLConn.dr.Read() == true)
                {
                    x = new ListViewItem(SQLConn.dr["ProductNo"].ToString());
                    x.SubItems.Add(SQLConn.dr["ProductName"].ToString());
                    x.SubItems.Add(SQLConn.dr["ItemPrice"].ToString());
                    x.SubItems.Add(SQLConn.dr["Quantity"].ToString());
                    x.SubItems.Add(SQLConn.dr["InvoiceNo"].ToString());
                    x.SubItems.Add(SQLConn.dr["description_product"].ToString());

                    ListView1.Items.Add(x);
                }
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
            }
            finally
            {
                SQLConn.cmd.Dispose();
                SQLConn.conn.Close();
            }
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
