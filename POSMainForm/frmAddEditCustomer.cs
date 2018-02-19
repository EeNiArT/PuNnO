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
    public partial class frmAddEditCustomer : Form
    {
        int LSCustomerID;
        public frmAddEditCustomer(int CusID)
        {
            
            InitializeComponent();
            LSCustomerID = CusID;
        }

     
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (SQLConn.adding == true)
            {
                AddCustomer();
            }
            else
            {
                UpdateCustomer();
            }

            if (System.Windows.Forms.Application.OpenForms["frmListCustomer"] != null)
            {
                (System.Windows.Forms.Application.OpenForms["frmListCustomer"] as frmListCustomer).LoadCustomers("");
            }

            //
        }

        private void GetCustomerID()
        {
            try
            {
                SQLConn.sqL = "SELECT CusID FROM customer ORDER BY CusID DESC";
                SQLConn.ConnDB();
                SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                SQLConn.dr = SQLConn.cmd.ExecuteReader();

                if (SQLConn.dr.Read() == true)
                {
                    lblProductNo.Text = (Convert.ToInt32(SQLConn.dr["CusID"]) + 1).ToString();
                }
                else
                {
                    lblProductNo.Text = "1";
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

        private void AddCustomer()
        {
            try
            {
                if(txtDue.Text == "" || txtComments.Text == "" || txtRemaining.Text == "" || txtShop.Text == ""  )
                {

                    Interaction.MsgBox("Please Enter All Required Business Fields", MsgBoxStyle.Information, "Incomplete Form Submission");

                }
                else
                {
                    SQLConn.sqL = "INSERT INTO customer(Lastname, Firstname, CNIC, Street, City, Province, ContactNo, DuePayment, CommentOnDuePayment, RemainingBalance, shopname, mobileNo) VALUES('" + txtLastname.Text + "', '" + txtFirstname.Text + "', '" + txtCNIC.Text + "', '" + txtStreet.Text + "',  '" + txtCity.Text + "', '" + txtProvince.Text + "', '" + txtContractNo.Text + "', '" + txtDue.Text + "', '" + txtComments.Text + "', '" + txtRemaining.Text + "', '" + txtShop.Text + "', '" + textBox1.Text + "')";
                    SQLConn.ConnDB();
                    SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                    SQLConn.cmd.ExecuteNonQuery();
                    Interaction.MsgBox("New Customer successfully added.", MsgBoxStyle.Information, "Add Customer");
                    this.Close();
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

        private void UpdateCustomer()
        {
            try
            {
                SQLConn.sqL = "Update customer SET Lastname = '" + txtLastname.Text + "', Firstname = '" + txtFirstname.Text + "', CNIC = '" + txtCNIC.Text + "', Street= '" + txtStreet.Text + "', City = '" + txtCity.Text + "', Province = '" + txtProvince.Text + "', ContactNo = '" + txtContractNo.Text + "', DuePayment ='" + txtDue.Text + "', CommentOnDuePayment = '" + txtComments.Text + "', RemainingBalance = '" + txtRemaining.Text + "' , shopname = '" + txtShop.Text + "', mobileNo = '" + textBox1.Text + "' WHERE CusID = '" + LSCustomerID + "'";
                SQLConn.ConnDB();
                SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                SQLConn.cmd.ExecuteNonQuery();
                Interaction.MsgBox("Customer record successfully updated", MsgBoxStyle.Information, "Update Customer");
             

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

        private void LoadUpdateCustomer()
        {
            try
            {
                SQLConn.sqL = "SELECT * FROM customer WHERE CusID = '" + LSCustomerID + "'";
                SQLConn.ConnDB();
                SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                SQLConn.dr = SQLConn.cmd.ExecuteReader();

                if (SQLConn.dr.Read() == true)
                {
                    lblProductNo.Text = SQLConn.dr["CusID"].ToString();
                    txtLastname.Text = SQLConn.dr["lastname"].ToString();
                    txtFirstname.Text = SQLConn.dr["Firstname"].ToString();
                    txtCNIC.Text = SQLConn.dr["CNIC"].ToString();
                    txtStreet.Text = SQLConn.dr["Street"].ToString();
                    txtCity.Text = SQLConn.dr["City"].ToString();
                    txtProvince.Text = SQLConn.dr["Province"].ToString();
                    txtContractNo.Text = SQLConn.dr["ContactNo"].ToString();
                    txtDue.Text = SQLConn.dr["DuePayment"].ToString();
                    txtComments.Text = SQLConn.dr["CommentOnDuePayment"].ToString();
                    txtRemaining.Text = SQLConn.dr["RemainingBalance"].ToString();
                    txtShop.Text = SQLConn.dr["shopname"].ToString();
                    textBox1.Text = SQLConn.dr["mobileNo"].ToString();
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

        private void ClearFields()
        {
            lblProductNo.Text = "";
            txtLastname.Text = "";
            txtFirstname.Text = "";
            txtCNIC.Text = "";
            txtStreet.Text = "";
            txtCity.Text = "";
            txtProvince.Text = "";
            txtContractNo.Text = "";
            txtDue.Text = "";
            txtComments.Text = "";
            txtRemaining.Text = "";
            txtShop.Text = "";
            textBox1.Text = "";
        }

        private void frmAddEditCustomer_Load(object sender, EventArgs e)
        {
            if (SQLConn.adding == true)
            {
                lblTitle.Text = "Adding New Customer";
                ClearFields();
                GetCustomerID();
            }
            else
            {
                lblTitle.Text = "Updating Customer";
                LoadUpdateCustomer();

            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblProductNo_Click(object sender, EventArgs e)
        {

        }
    }
}
