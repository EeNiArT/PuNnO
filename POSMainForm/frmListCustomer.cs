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
    public partial class frmListCustomer : Form
    {

        
        public int customerID;

        public frmListCustomer()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadCustomers("");
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
            LoadCustomers("");
        }

        public void LoadCustomers(string search)
        {
            try
            {
                SQLConn.sqL = "SELECT shopname ,CusID, CONCAT(Firstname, ' ', Lastname) as ClientName, CONCAT(Street, ' ', City , ' ', Province) as Address, ContactNo, CommentOnDuePayment, DuePayment FROM customer WHERE LASTNAME LIKE '" + search.Trim() + "%' ORDER By Firstname";
                SQLConn.ConnDB();
                SQLConn.cmd = new MySqlCommand (SQLConn.sqL, SQLConn.conn);
                SQLConn.dr = SQLConn.cmd.ExecuteReader ();

                ListViewItem x = null;
                ListView1.Items.Clear();

                while (SQLConn.dr.Read() == true)
                {
                    x = new ListViewItem(SQLConn.dr["CusID"].ToString());

                    x.SubItems.Add(SQLConn.dr["shopname"].ToString());
                    x.SubItems.Add(SQLConn.dr["ClientName"].ToString());
                    x.SubItems.Add(SQLConn.dr["ContactNo"].ToString());
                    x.SubItems.Add(SQLConn.dr["Address"].ToString());
                    x.SubItems.Add(SQLConn.dr["DuePayment"].ToString());
                    x.SubItems.Add(SQLConn.dr["CommentOnDuePayment"].ToString());

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

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListView1.Items.Count == 0)
            {
                Interaction.MsgBox("Please select record to update", MsgBoxStyle.Exclamation, "Update");
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
                    customerID = Convert.ToInt32(ListView1.FocusedItem.Text);
                    frmAddEditCustomer f2 = new frmAddEditCustomer(customerID);
                    f2.ShowDialog();

                    LoadCustomers("");
                }
            }
            catch 
            {
                Interaction.MsgBox("Please select record to update", MsgBoxStyle.Exclamation, "Update");
                return;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            SQLConn.strSearch = Interaction.InputBox("ENTER LAST NAME OF THE Customer.", "Search Customer", " ");
           
            if (SQLConn.strSearch.Length >= 1)
            {
                LoadCustomers(SQLConn.strSearch.Trim());
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

        private void button5_Click(object sender, EventArgs e)
        {
            if (ListView1.Items.Count == 0)
            {
                Interaction.MsgBox("Please select record to Delete", MsgBoxStyle.Exclamation, "Delete Client");
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
                    customerID = Convert.ToInt32(ListView1.FocusedItem.Text);
                    try
                    {
                        SQLConn.sqL = "DELETE FROM 	customer WHERE CusID = " + customerID + "";
                        SQLConn.ConnDB();
                        SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                        SQLConn.cmd.ExecuteNonQuery();
                        Interaction.MsgBox("Customer Deleted.", MsgBoxStyle.Information, "Confirmation Delete Customer");
                        LoadCustomers("");
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

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
