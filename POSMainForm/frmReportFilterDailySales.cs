using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;

namespace POSMainForm
{
    public partial class frmReportFilterDailySales : Form
    {
        public frmReportFilterDailySales()
        {
            InitializeComponent();
        }

        private void rbUser_CheckedChanged(object sender, EventArgs e)
        {
            if (rbUser.Checked == true)
            {
                rbInvoice.Checked = false;
            }
        }

        private void rbInvoice_CheckedChanged(object sender, EventArgs e)
        {
            if (rbInvoice.Checked == true)
            {
                rbUser.Checked = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if ((rbUser.Checked == false) && (rbInvoice.Checked == false))
            {
                Interaction.MsgBox("Please select report by Customer or Invoice No",MsgBoxStyle.Information, "Select Report");
                return;
            }

            if (rbUser.Checked == true)
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox("Customer ID", "Customer ID", "");




                try
                {

                    SQLConn.sqL = "SELECT * FROM customer WHERE CusID=" + input + " ORDER BY CusID Limit 1 ";
                    SQLConn.ConnDB();
                    SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                    SQLConn.dr = SQLConn.cmd.ExecuteReader();

                    while (SQLConn.dr.Read() == true)
                    {
                        frmReportDailSalesByCustomer R = new frmReportDailSalesByCustomer(DateTimePicker1.Value, dateTimePicker2.Value, input, SQLConn.dr["Firstname"].ToString(), SQLConn.dr["DuePayment"].ToString());
                        R.Show();
                    }

                }
                catch (Exception ex)
                {
                    //Interaction.MsgBox(ex.ToString());
                }
                finally
                {
                    SQLConn.cmd.Dispose();
                    SQLConn.conn.Close();
                }




            }
            else
            {
                frmReportDailSalesByInvoice R = new frmReportDailSalesByInvoice(DateTimePicker1.Value, dateTimePicker2.Value);
                R.Show();
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
