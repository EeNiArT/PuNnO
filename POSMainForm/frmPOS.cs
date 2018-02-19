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
    public partial class frmPOS : Form
    {
        private frmAddEditProduct frmProduct = null;
        ListViewItem y = null;

        static int Counter = 0;
        static int id;
        static int total_amount = 0;
        static int total_amount_with_due = 0;
        static int StaffID = 0;
        int invoiceID = 0;


        string shopname = "";
        string address;
        string genDesc;

        string name="Retail";
        string duepayment;
        string remainingBal;

        string contact;

        bool update = false;
        public frmPOS(int sID , int invoice , bool updat)
        {
            update = updat;
            StaffID = sID;
            invoiceID = invoice;
            InitializeComponent();
            InitializeTimer();
            textBox4.Text = "0";

            textBox5.Text = "0";
        }
        private int counter;
        Timer t = new Timer();

        private void InitializeTimer()
        {
            counter = 0;
            t.Interval = 750;

            t.Tick += new EventHandler(timer1_Tick);
            t.Enabled = true;
        } 
        private void LoadCategory()
        {
            try
            {
                SQLConn.sqL = "SELECT * FROM Product WHERE ProductCode LIKE '" + txtCatName.Text + "%' ORDER BY ProductCode ";
                SQLConn.ConnDB();
                SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                SQLConn.dr = SQLConn.cmd.ExecuteReader();

                ListViewItem x = null;
                ListView1.Items.Clear();

                while (SQLConn.dr.Read() == true)
                {
                    x = new ListViewItem(SQLConn.dr["ProductNo"].ToString());
                    x.SubItems.Add(SQLConn.dr["ProductCode"].ToString());
                    x.SubItems.Add(SQLConn.dr["UnitPrice"].ToString());
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
        private void LoadCustomer()
        {
            try
            {
                SQLConn.sqL = "SELECT * FROM customer WHERE Firstname LIKE '" + textBox1.Text + "%' ORDER BY Firstname ";
                SQLConn.ConnDB();
                SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                SQLConn.dr = SQLConn.cmd.ExecuteReader();

                ListViewItem x = null;
                listView3.Items.Clear();

                while (SQLConn.dr.Read() == true)
                {
                    x = new ListViewItem(SQLConn.dr["CusID"].ToString());
                    x.SubItems.Add(SQLConn.dr["Firstname"].ToString());
                    x.SubItems.Add(SQLConn.dr["DuePayment"].ToString());
                    x.SubItems.Add(SQLConn.dr["RemainingBalance"].ToString());

                    listView3.Items.Add(x);
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
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = "Date-Time : " + DateTime.Now.ToString("MMMM dd, yyyy hh:mm:ss tt");
        }
        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmSelectCategory_Load(object sender, EventArgs e)
        {
            LoadCategory();
            LoadCustomer();
            total_amount_with_due = total_amount + Convert.ToInt32(textBox4.Text);
            txtWithDue.Text = total_amount_with_due.ToString();

            if (update)
            {
                LoadInvoices(invoiceID);
            }
        }
        public void LoadInvoices(int search)
        {
            try
            {
                SQLConn.sqL = "SELECT T.InvoiceNo, P.ProductNo, P.ProductCode,P.UnitPrice,TD.Quantity, TD.description_product, REPLACE(TDate, '-', '/') as TDate, TTime,TD.ItemPrice, SUM(TD.Quantity) as totalQuantity, (TD.ItemPrice * SUM(TD.Quantity)) as TotalPrice FROM Product as P, Transactions as T, TransactionDetails as TD WHERE P.ProductNo = TD.ProductNo AND TD.InvoiceNo = T.InvoiceNo AND  T.InvoiceNo =" + search + "  GROUP BY T.InvoiceNo, P.ProductNo, TDate ORDER By TDate";
                SQLConn.ConnDB();
                SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                SQLConn.dr = SQLConn.cmd.ExecuteReader();

                ListViewItem x = null;
                listView2.Items.Clear();

                while (SQLConn.dr.Read() == true)
                {
                    Counter++;
                    x = new ListViewItem(Counter.ToString());
                    x.SubItems.Add(SQLConn.dr["ProductNo"].ToString());
                    x.SubItems.Add(SQLConn.dr["ProductCode"].ToString());
                    x.SubItems.Add(SQLConn.dr["UnitPrice"].ToString());

                    x.SubItems.Add(SQLConn.dr["description_product"].ToString());
                    x.SubItems.Add(SQLConn.dr["Quantity"].ToString());
                    listView2.Items.Add(x);
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
            
            ///////////////////////////////////////////////

            try
            {
                int cusID = 0;
                SQLConn.sqL = "SELECT TotalAmountGrand,CusID,description_invoice, InvoiceNo, Firstname,TotalAmount,TDate,TTime, DuePayment, discount FROM transactions WHERE InvoiceNo =" + search + "  Limit 1";
                SQLConn.ConnDB();
                SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                SQLConn.dr = SQLConn.cmd.ExecuteReader();

                while (SQLConn.dr.Read() == true)
                {
                    txtTA.Text = SQLConn.dr["TotalAmount"].ToString();
                    txtGrand.Text = SQLConn.dr["TotalAmount"].ToString();
                    txtDiscount.Text = SQLConn.dr["discount"].ToString();
                    txtGeneralDesc.Text = SQLConn.dr["description_invoice"].ToString();
                    cusID = Convert.ToInt32(SQLConn.dr["CusID"].ToString());
                }
                SQLConn.cmd.Dispose();
                SQLConn.conn.Close();

                if (cusID != 0)
                {
                    SQLConn.sqL = "SELECT *,CONCAT(Street, ' ', City , ' ', Province) as Address FROM customer WHERE CusID =" + cusID + " Limit 1";
                    SQLConn.ConnDB();
                    SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                    SQLConn.dr = SQLConn.cmd.ExecuteReader();

                    while (SQLConn.dr.Read() == true)
                    {
                        textBox2.Text = cusID.ToString();
                        textBox3.Text = SQLConn.dr["Firstname"].ToString();
                        name = SQLConn.dr["Firstname"].ToString(); 
                        textBox4.Text = SQLConn.dr["DuePayment"].ToString();
                        textBox5.Text = SQLConn.dr["RemainingBalance"].ToString();
                        shopname = SQLConn.dr["shopname"].ToString();
                        address = SQLConn.dr["Address"].ToString();
                        contact = SQLConn.dr["mobileNo"].ToString();
                        txtWithDue.Text =txtTA.Text;
                        total_amount_with_due = Convert.ToInt32(txtTA.Text);
                        total_amount = Convert.ToInt32(txtTA.Text);

                    }
                    txtChange.Text = "0";
                }
                frmInvoice inv = new frmInvoice(invoiceID, name, textBox4.Text, textBox5.Text, Convert.ToInt32(txtDiscount.Text), shopname, address, genDesc, contact);

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

        private void txtCatName_TextChanged(object sender, EventArgs e)
        {
            LoadCategory();
        }

        private void ListView1_DoubleClick(object sender, EventArgs e)
        {
            Counter++; 
            y = new ListViewItem(Counter.ToString());
            y.SubItems.Add(ListView1.FocusedItem.SubItems[0].Text);
            y.SubItems.Add(ListView1.FocusedItem.SubItems[1].Text);
            y.SubItems.Add(ListView1.FocusedItem.SubItems[2].Text);
               y.SubItems.Add(ListView1.FocusedItem.SubItems[3].Text);

            string input = Microsoft.VisualBasic.Interaction.InputBox("Quantity","Quantity","1");

            y.SubItems.Add(input);

            listView2.Items.Add(y);

            txtCatName.Text="";

            int i;
            if (input != "" && int.TryParse(input, out i)) 
            {

                if (ListView1.FocusedItem != null) 
                { 
                        total_amount = total_amount + (Convert.ToInt32(ListView1.FocusedItem.SubItems[2].Text)*(Convert.ToInt32(input)));

                        if (textBox4.Text != "" && int.TryParse(textBox4.Text, out i))
                        {
                            total_amount_with_due = total_amount + Convert.ToInt32(textBox4.Text);
                            txtWithDue.Text = total_amount_with_due.ToString();
                        }

                        if (txtDiscount.Text !="" && int.TryParse(txtDiscount.Text, out i))
                        {
                            txtGrand.Text = (Convert.ToInt32(txtWithDue.Text) - Convert.ToInt32(txtDiscount.Text)).ToString();
                            txtTA.Text = total_amount.ToString();
                        }
                        
                }
            }

            LoadCategory();
        }

        private void lblDateTime_Click(object sender, EventArgs e)
        {

        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtTA_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCash_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void txtCash_TextChanged(object sender, EventArgs e)
        {
            int i;
            if (txtChange.Text != "" && int.TryParse(txtCash.Text, out i))
            {
                txtChange.Text = (total_amount_with_due - Convert.ToInt32(txtCash.Text)).ToString();
            }
            if(txtCash.Text =="")
            {
                txtChange.Text = "0";
            }
            if (txtCash.Text == "0")
            {
                txtChange.Text = "0";
            }
        }
        private void ClearFields()
        {
            LoadCategory();
            LoadCustomer();
            
            shopname = "";
            address = "";
            genDesc = "";

            name = "Retail";
            contact = "";
            invoiceID = 0;

            listView2.Items.Clear();
            txtTA.Text = "0";
            txtWithDue.Text = "0";
            txtCash.Text = "0";
            txtChange.Text = "0";
            txtGeneralDesc.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "0";
            textBox5.Text = "0";
            txtCatName.Text = "";
            textBox1.Text = "";
            txtDiscount.Text = "0";
            txtGrand.Text = "0";
            Counter = 0;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            LoadCustomer();
        }

        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Text = listView3.FocusedItem.SubItems[0].Text;
            textBox3.Text = listView3.FocusedItem.SubItems[1].Text;
            textBox4.Text = listView3.FocusedItem.SubItems[2].Text;
            textBox5.Text = listView3.FocusedItem.SubItems[3].Text;

            LoadCustomer();
            if (textBox4.Text != "0")
            {
                total_amount_with_due = total_amount + Convert.ToInt32(textBox4.Text);
                Label2.Text = "With Due Payment";
            }
            else if (textBox5.Text != "0")
            {
                total_amount_with_due = total_amount - Convert.ToInt32(textBox5.Text);
                Label2.Text = "With Advance Payment Minus";
            }

            txtWithDue.Text = total_amount_with_due.ToString();

             int i;
             if (int.TryParse(txtDiscount.Text, out i))
             {
                 txtGrand.Text = (total_amount_with_due - Convert.ToInt32(txtDiscount.Text)).ToString();
             }

             txtCash.Text = "0";
             txtChange.Text = "0";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (update)
            {
                try
                {

                    int diff = (Convert.ToInt32(txtGrand.Text)) - Convert.ToInt32(txtCash.Text);
                    // MessageBox.Show(diff.ToString());
                    if (textBox2.Text != "")
                    {
                        if (diff > 0)
                        {
                            SQLConn.sqL = "UPDATE customer SET DuePayment = '" + Math.Abs(diff) + "' , RemainingBalance = '0' WHERE CusID = " + textBox2.Text + "";
                            // MessageBox.Show(SQLConn.sqL);
                            SQLConn.ConnDB();
                            SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                            SQLConn.cmd.ExecuteNonQuery();
                            textBox5.Text = "0";
                            textBox4.Text = Math.Abs(diff).ToString();
                        }
                        if (diff < 0)
                        {
                            SQLConn.sqL = "UPDATE customer SET RemainingBalance = '" + Math.Abs(diff) + "', DuePayment = '0' WHERE CusID = " + textBox2.Text + "";
                            //MessageBox.Show(SQLConn.sqL);
                            SQLConn.ConnDB();
                            SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                            SQLConn.cmd.ExecuteNonQuery();
                            textBox4.Text = "0";
                            textBox5.Text = Math.Abs(diff).ToString();

                        }
                        if (diff == 0)
                        {
                            SQLConn.sqL = "UPDATE customer SET RemainingBalance = '0' ,DuePayment = '0' WHERE CusID = " + textBox2.Text + "";
                            // MessageBox.Show(SQLConn.sqL);
                            SQLConn.ConnDB();
                            SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                            SQLConn.cmd.ExecuteNonQuery();
                            textBox5.Text = "0";
                            textBox4.Text = "0";
                        }
                    }



                    SQLConn.sqL = "Update transactions SET TDate ='" + DateTime.Now.ToString("MM/dd/yyyy").ToString() + "', TTime = '" + DateTime.Now.ToString("hh:mm:ss").ToString() + "', NonVatTotal = '" + txtWithDue.Text + "', VatAmount='" + txtWithDue.Text + "', TotalAmount = '" + txtTA.Text + "', StaffID =  " + StaffID + ", CusID =  '" + textBox2.Text + "', Firstname ='" + textBox3.Text + "', RemainingBalance = '" + textBox4.Text + "', DuePayment = '" + textBox5.Text + "' , discount = " + txtDiscount.Text + ", description_invoice =  '" + txtGeneralDesc.Text + "' ,TotalAmountGrand =  " + txtGrand.Text + "   WHERE InvoiceNo = " + invoiceID + "";
                    SQLConn.ConnDB();
                    SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                    SQLConn.cmd.ExecuteNonQuery();

                    SQLConn.sqL = "DELETE FROM transactiondetails  WHERE InvoiceNo = " + invoiceID + "";
                    SQLConn.ConnDB();
                    SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                    SQLConn.cmd.ExecuteNonQuery();

                    id = (int)SQLConn.cmd.LastInsertedId;

                    List<string> ls = new List<string>();

                    foreach (ListViewItem itemRow in this.listView2.Items)
                    {
                        for (int i = 0; i < itemRow.SubItems.Count; i++)
                        {

                            ls.Add(itemRow.SubItems[i].Text.ToString());

                        }
                        try
                        {
                            SQLConn.sqL = "INSERT INTO transactiondetails(InvoiceNo, ProductNo, ProductName	, ItemPrice, Quantity, Discount,description_product) VALUES(" + invoiceID + ", " + ls.ElementAt(1).ToString() + ", '" + ls.ElementAt(2).ToString() + "', " + ls.ElementAt(3).ToString() + ", '" + ls.ElementAt(5).ToString() + "', '0' ,'" + ls.ElementAt(4).ToString() + "')";
                            SQLConn.ConnDB();
                            SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                            SQLConn.cmd.ExecuteNonQuery();

                            SQLConn.sqL = "INSERT INTO stockout(ProductCode, Description, DateOut, Quantity, Price,TotalAmount) VALUES(" + ls.ElementAt(1).ToString() + ", '" + ls.ElementAt(2).ToString() + "', " + DateTime.Now.ToString("MM/dd/yyyy").ToString() + ", " + ls.ElementAt(5).ToString() + ", " + ls.ElementAt(3).ToString() + ", " + txtTA.Text + ")";
                            //MessageBox.Show(SQLConn.sqL);
                            SQLConn.ConnDB();
                            SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                            SQLConn.cmd.ExecuteNonQuery();


                            SQLConn.sqL = "UPDATE Product SET StocksOnhand = StocksOnHand - " + ls.ElementAt(5).ToString() + " WHERE ProductNo = " + ls.ElementAt(1).ToString() + "";
                            //MessageBox.Show(SQLConn.sqL);
                            SQLConn.ConnDB();
                            SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                            SQLConn.cmd.ExecuteNonQuery();


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

                        ls.Clear();

                    }

                    Interaction.MsgBox("Transaction Saved.", MsgBoxStyle.Information, "Add Transaction");

                    try
                    {
                        int cusID = Convert.ToInt32(textBox2.Text.ToString());


                        if (cusID != 0)
                        {
                            SQLConn.sqL = "SELECT * , CONCAT(Street, ' ', City , ' ', Province) as Address FROM customer WHERE CusID =" + cusID + " Limit 1";
                            SQLConn.ConnDB();
                            SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                            SQLConn.dr = SQLConn.cmd.ExecuteReader();

                            while (SQLConn.dr.Read() == true)
                            {
                                shopname = SQLConn.dr["shopname"].ToString();
                                address = SQLConn.dr["Address"].ToString();
                                genDesc = txtGeneralDesc.Text;


                                name = SQLConn.dr["Firstname"].ToString(); ;
                                contact = SQLConn.dr["mobileNo"].ToString(); ; ; ;
                            }

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

                    frmInvoice inv = new frmInvoice(invoiceID, name, textBox4.Text, textBox5.Text, Convert.ToInt32(txtDiscount.Text), shopname, address, genDesc, contact);
                    inv.ShowDialog();
                    ClearFields();

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
            else if(update == false)
            {
                try
                {

                    int diff = (Convert.ToInt32(txtGrand.Text)) - Convert.ToInt32(txtCash.Text);
                    // MessageBox.Show(diff.ToString());
                    if (textBox2.Text != "")
                    {
                        if (diff > 0)
                        {
                            SQLConn.sqL = "UPDATE customer SET DuePayment = '" + Math.Abs(diff) + "' , RemainingBalance = '0' WHERE CusID = " + textBox2.Text + "";
                            // MessageBox.Show(SQLConn.sqL);
                            SQLConn.ConnDB();
                            SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                            SQLConn.cmd.ExecuteNonQuery();
                            textBox5.Text = "0";
                            textBox4.Text = Math.Abs(diff).ToString();
                        }
                        if (diff < 0)
                        {
                            SQLConn.sqL = "UPDATE customer SET RemainingBalance = '" + Math.Abs(diff) + "', DuePayment = '0' WHERE CusID = " + textBox2.Text + "";
                            //MessageBox.Show(SQLConn.sqL);
                            SQLConn.ConnDB();
                            SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                            SQLConn.cmd.ExecuteNonQuery();
                            textBox4.Text = "0";
                            textBox5.Text = Math.Abs(diff).ToString();

                        }
                        if (diff == 0)
                        {
                            SQLConn.sqL = "UPDATE customer SET RemainingBalance = '0' ,DuePayment = '0' WHERE CusID = " + textBox2.Text + "";
                            // MessageBox.Show(SQLConn.sqL);
                            SQLConn.ConnDB();
                            SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                            SQLConn.cmd.ExecuteNonQuery();
                            textBox5.Text = "0";
                            textBox4.Text = "0";
                        }
                    }

                    SQLConn.sqL = "INSERT INTO transactions(TDate, TTime, NonVatTotal, VatAmount, TotalAmount, StaffID ,CusID ,Firstname , RemainingBalance , DuePayment,discount,description_invoice,TotalAmountGrand ) VALUES('" + DateTime.Now.ToString("MM/dd/yyyy").ToString() + "', '" + DateTime.Now.ToString("hh:mm:ss").ToString() + "', '" + txtWithDue.Text + "', '" + txtWithDue.Text + "', '" + txtTA.Text + "', " + StaffID + ",  '" + textBox2.Text + "', '" + textBox3.Text + "', '" + textBox4.Text + "', '" + textBox5.Text + "' , " + txtDiscount.Text + ", '" + txtGeneralDesc.Text + "'," + txtGrand.Text + ")";
                    SQLConn.ConnDB();
                    SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                    SQLConn.cmd.ExecuteNonQuery();

                    id = (int)SQLConn.cmd.LastInsertedId;

                    List<string> ls = new List<string>();
                    foreach (ListViewItem itemRow in this.listView2.Items)
                    {
                        for (int i = 0; i < itemRow.SubItems.Count; i++)
                        {

                            ls.Add(itemRow.SubItems[i].Text.ToString());

                        }
                        // MessageBox.Show(ls.ElementAt(5).ToString());
                        try
                        {

                            SQLConn.sqL = "INSERT INTO transactiondetails(InvoiceNo, ProductNo, ProductName	, ItemPrice, Quantity, Discount,description_product) VALUES(" + id + ", " + ls.ElementAt(1).ToString() + ", '" + ls.ElementAt(2).ToString() + "', " + ls.ElementAt(3).ToString() + ", '" + ls.ElementAt(5).ToString() + "', '0' ,'" + ls.ElementAt(4).ToString() + "')";
                            //MessageBox.Show(SQLConn.sqL);
                            SQLConn.ConnDB();
                            SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                            SQLConn.cmd.ExecuteNonQuery();

                            SQLConn.sqL = "INSERT INTO stockout(ProductCode, Description, DateOut, Quantity, Price,TotalAmount) VALUES(" + ls.ElementAt(1).ToString() + ", '" + ls.ElementAt(2).ToString() + "', " + DateTime.Now.ToString("MM/dd/yyyy").ToString() + ", " + ls.ElementAt(5).ToString() + ", " + ls.ElementAt(3).ToString() + ", " + txtTA.Text + ")";
                            //MessageBox.Show(SQLConn.sqL);
                            SQLConn.ConnDB();
                            SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                            SQLConn.cmd.ExecuteNonQuery();


                            SQLConn.sqL = "UPDATE Product SET StocksOnhand = StocksOnHand - " + ls.ElementAt(5).ToString() + " WHERE ProductNo = " + ls.ElementAt(1).ToString() + "";
                            //MessageBox.Show(SQLConn.sqL);
                            SQLConn.ConnDB();
                            SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                            SQLConn.cmd.ExecuteNonQuery();


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

                        ls.Clear();

                    }



                    Interaction.MsgBox("Transaction Saved.", MsgBoxStyle.Information, "Add Transaction");

                    try
                    {
                        int cusID = Convert.ToInt32(textBox2.Text.ToString());


                        if (cusID != 0)
                        {
                            SQLConn.sqL = "SELECT * , CONCAT(Street, ' ', City , ' ', Province) as Address FROM customer WHERE CusID =" + cusID + " Limit 1";
                            SQLConn.ConnDB();
                            SQLConn.cmd = new MySqlCommand(SQLConn.sqL, SQLConn.conn);
                            SQLConn.dr = SQLConn.cmd.ExecuteReader();

                            while (SQLConn.dr.Read() == true)
                            {
                                shopname = SQLConn.dr["shopname"].ToString();
                                address = SQLConn.dr["Address"].ToString();
                                genDesc = txtGeneralDesc.Text;


                                name = SQLConn.dr["Firstname"].ToString(); ;
                                contact = SQLConn.dr["mobileNo"].ToString(); ; ; ;
                            }

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

                    frmInvoice inv = new frmInvoice(id, name, textBox4.Text, textBox5.Text, Convert.ToInt32(txtDiscount.Text), shopname, address, genDesc, contact);
                    inv.ShowDialog();

                    ClearFields();

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

        private void button2_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void listView2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //Point mousePosition = listView2.PointToClient(Control.MousePosition);
            //ListViewHitTestInfo hit = listView2.HitTest(mousePosition);
            //int columnindex = hit.Item.SubItems.IndexOf(hit.SubItem);
            //MessageBox.Show(columnindex.ToString());
        }
        private void lv_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //Int32 colIndex = Convert.ToInt32(e.Column.ToString());
            //listView2.Columns[colIndex].Text = "new text";
            //MessageBox.Show(colIndex.ToString());


        }

        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtDiscount_TextChanged_1(object sender, EventArgs e)
        {
           
       }
           

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void Label2_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void txtDiscount_MaskInputRejected_1(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void Label3_Click(object sender, EventArgs e)
        {

        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            string prod = listView2.FocusedItem.SubItems[2].Text.ToString();
            string input = Microsoft.VisualBasic.Interaction.InputBox("Product Description", "Enter Product ("+prod+") Description", "");

            listView2.FocusedItem.SubItems[4].Text = input;
        
        }

        private void txtChange_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDiscount_TextChanged_11(object sender, KeyPressEventArgs e)
        {
           
        }
        private void txtDiscount_TextChanged_1(object sender, KeyPressEventArgs e)
        {

        }

        private void txtDiscount_TextChanged_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int i;
                if (txtDiscount.Text != "" && int.TryParse(txtDiscount.Text, out i))
                {
                    total_amount_with_due = (total_amount_with_due - Convert.ToInt32(txtDiscount.Text));
                    txtWithDue.Text = total_amount_with_due.ToString();
                    txtTA.Text = (Convert.ToInt32(txtTA.Text) - Convert.ToInt32(txtDiscount.Text)).ToString();
                    txtGrand.Text = (Convert.ToInt32(txtGrand.Text) - Convert.ToInt32(txtDiscount.Text)).ToString();
                }
            }
        }

    }
}
