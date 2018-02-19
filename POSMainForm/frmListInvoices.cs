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
    public partial class frmListInvoices : Form
    {

        
        public int customerID;
        public int InvoiceID;

        public frmListInvoices()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadInvoices("");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SQLConn.adding = true;
            SQLConn.updating = false;
            int init = 0;
            frmAddEditCustomer f2 = new frmAddEditCustomer(init);
            f2.ShowDialog();
            LoadInvoices("");
        }

        public void LoadInvoices(string search)
        {
            try
            {
                SQLConn.sqL = "SELECT description_invoice, InvoiceNo, Firstname,TotalAmount,TDate,TTime, DuePayment, discount FROM transactions WHERE InvoiceNo LIKE '" + search.Trim() + "%' ORDER By InvoiceNo DESC";
                SQLConn.ConnDB();
                SQLConn.cmd = new MySqlCommand (SQLConn.sqL, SQLConn.conn);
                SQLConn.dr = SQLConn.cmd.ExecuteReader ();

                ListViewItem x = null;
                ListView1.Items.Clear();

                while (SQLConn.dr.Read() == true)
                {
                    x = new ListViewItem(SQLConn.dr["InvoiceNo"].ToString());
                    x.SubItems.Add(SQLConn.dr["Firstname"].ToString());
                    x.SubItems.Add(SQLConn.dr["TotalAmount"].ToString());
                    x.SubItems.Add(SQLConn.dr["TDate"].ToString() + " / " +SQLConn.dr["TTime"].ToString());
                    x.SubItems.Add(SQLConn.dr["DuePayment"].ToString());
                    x.SubItems.Add(SQLConn.dr["discount"].ToString());

                    x.SubItems.Add(SQLConn.dr["description_invoice"].ToString());

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


        private void button3_Click(object sender, EventArgs e)
        {
            
            SQLConn.strSearch = Interaction.InputBox("ENTER Invoice No.", "Search Invoice", " ");
           
            if (SQLConn.strSearch.Length >= 1)
            {
                LoadInvoices(SQLConn.strSearch.Trim());
            }
            else if (string.IsNullOrEmpty(SQLConn.strSearch))
            {
                return;
            }
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (ListView1.Items.Count == 0)
            {
                Interaction.MsgBox("Please select Invoice from list ", MsgBoxStyle.Exclamation, "Detail of Invoices");
                return;
            }
            try
            {
                if (string.IsNullOrEmpty(ListView1.FocusedItem.Text))
                {

                }
                else
                {
                    SQLConn.adding = false;
                    SQLConn.updating = true;
                    InvoiceID = Convert.ToInt32(ListView1.FocusedItem.Text);
                    frmListDetailinvoice f2 = new frmListDetailinvoice(InvoiceID);
                    f2.ShowDialog();
                }
            }
            catch
            {
                Interaction.MsgBox("Please select record to update", MsgBoxStyle.Exclamation, "Update");
                return;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (ListView1.Items.Count == 0)
            {
                Interaction.MsgBox("Please select record to Delete", MsgBoxStyle.Exclamation, "Delete Invoice");
                return;
            }
            try
            {
                if (string.IsNullOrEmpty(ListView1.FocusedItem.Text))
                {

                }
                else
                {
                    SQLConn.adding = false;
                    SQLConn.updating = true;
                  int  iID = Convert.ToInt32(ListView1.FocusedItem.Text);
                    try
                    {
                        SQLConn.sqL = "DELETE FROM 	transactions WHERE InvoiceNo = " + iID + "";
                        SQLConn.ConnDB();
                        SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                        SQLConn.cmd.ExecuteNonQuery();
                        Interaction.MsgBox("Invoice Deleted.", MsgBoxStyle.Information, "Confirmation Delete Invoice");
                        LoadInvoices("");
                    }
                    catch (Exception ex)
                    {
                        Interaction.MsgBox(ex.ToString());
                    }
                    finally
                    {
                        SQLConn.cmd.Dispose();
                        SQLConn.conn.Close();
                    }

                }
            }
            catch
            {
                Interaction.MsgBox("Please select record to delete", MsgBoxStyle.Exclamation, "delete");
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListView1.Items.Count == 0)
            {
                Interaction.MsgBox("Please select record to Update", MsgBoxStyle.Exclamation, "Update Invoice");
                return;
            }
            try
            {
                if (string.IsNullOrEmpty(ListView1.FocusedItem.Text))
                {

                }
                else
                {
                    int iID = Convert.ToInt32(ListView1.FocusedItem.Text);
                    frmPOS fp = new frmPOS(frmMain.StaffID, iID , true);
                    fp.ShowDialog();
                }
            }
            catch
            {
                Interaction.MsgBox("Please select invoice to update", MsgBoxStyle.Exclamation, "update");
                return;
            }
        }

    }
}
