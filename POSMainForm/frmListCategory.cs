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
    public partial class frmListCategory : Form
    {
        public int catgoryID;

        public frmListCategory()
        {
            InitializeComponent();
        }

        public void LoadCategories(string strSearch)
        {
            try
            {
                SQLConn.sqL = "SELECT * FROM CATEGORY WHERE CategoryName LIKE '" + strSearch + "%' ORDER By CategoryName";
                SQLConn.ConnDB();
                SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                SQLConn.dr = SQLConn.cmd.ExecuteReader(CommandBehavior.CloseConnection);

                ListViewItem x = null;
                ListView1.Items.Clear();

                while (SQLConn.dr.Read() == true)
                {
                    x = new ListViewItem(SQLConn.dr["CategoryNo"].ToString());
                    x.SubItems.Add(SQLConn.dr["CategoryName"].ToString());
                    x.SubItems.Add(SQLConn.dr["Description"].ToString());
                    ListView1.Items.Add(x);
                }
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

        private void btnNew_Click(object sender, EventArgs e)
        {
            SQLConn.adding = true;
            SQLConn.updating = false;
            int init = 0;
            frmAddEditCategory aeC = new frmAddEditCategory(init);
            aeC.ShowDialog();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
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
                    catgoryID = Convert.ToInt32(ListView1.FocusedItem.Text);
                    frmAddEditCategory aeC = new frmAddEditCategory(catgoryID);
                    aeC.ShowDialog();
                }
            }
            catch 
            {
                Interaction.MsgBox("Please select record to update", MsgBoxStyle.Exclamation, "Update Brand");
                return;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SQLConn.strSearch = Interaction.InputBox("ENTER Brand Name.", "Search Brand", " ");
           
            if (SQLConn.strSearch.Length >= 1)
            {
                LoadCategories(SQLConn.strSearch.Trim());
            }
            else if (string.IsNullOrEmpty(SQLConn.strSearch))
            {
                return;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmListCategory_Load(object sender, EventArgs e)
        {
             LoadCategories("");
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (ListView1.Items.Count == 0)
            {
                Interaction.MsgBox("Please select record to Delete", MsgBoxStyle.Exclamation, "Delete Brand");
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
                    int catID = Convert.ToInt32(ListView1.FocusedItem.Text);
                    try
                    {
                        SQLConn.sqL = "DELETE FROM  category WHERE CategoryNo = " + catID + "";
                        SQLConn.ConnDB();
                        SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                        SQLConn.cmd.ExecuteNonQuery();
                        Interaction.MsgBox("Brand Deleted.", MsgBoxStyle.Information, "Confirmation Delete Brand");
                        LoadCategories("");
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
                Interaction.MsgBox("Please select record to delete", MsgBoxStyle.Exclamation, "Delete");
                return;
            }
        }
    }
}
