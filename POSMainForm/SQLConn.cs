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
using System.Data.SqlClient;

namespace POSMainForm
{
	static class SQLConn
	{
		public static string ServerMySQL;
		public static string PortMySQL;
		public static string UserNameMySQL;
		public static string PwdMySQL;
		public static string DBNameMySQL;

		public static string sqL;
		public static DataSet ds = new DataSet();
		public static MySqlCommand cmd;
		public static MySqlDataReader dr;

        public static bool adding;
        public static bool updating;

        public static string strSearch = "";

		public static MySqlDataAdapter da = new MySqlDataAdapter();

		public static MySqlConnection conn = new MySqlConnection();
		public static void getData()
		{
			string AppName = Application.ProductName;
			try {

				    DBNameMySQL = Interaction.GetSetting(AppName, "DBSection", "DB_Name", "temp");
				    ServerMySQL = Interaction.GetSetting(AppName, "DBSection", "DB_IP", "temp");
				    PortMySQL = Interaction.GetSetting(AppName, "DBSection", "DB_Port", "temp");
				    UserNameMySQL = Interaction.GetSetting(AppName, "DBSection", "DB_User", "temp");
				    PwdMySQL = Interaction.GetSetting(AppName, "DBSection", "DB_Password", "temp");
			
            } catch {
				Interaction.MsgBox("System registry was not established, you can set/save " + "these settings by pressing F1", MsgBoxStyle.Information);
			}

		}

		public static void ConnDB()
		{
			conn.Close();
			try {
                String str = "server=EENIART/SQLEXPRESS;database=demo;UID=sa;password=sa123";

                String query = "select * from data";

                SqlConnection con = new SqlConnection(str);

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                DataSet ds = new DataSet();

                //MessageBox.Show("connect with sql server");

                //con.Close();

                conn.ConnectionString = "Server = '" + ServerMySQL + "';  " + "Port = '" + PortMySQL + "'; " + "Database = '" + DBNameMySQL + "'; " + "user id = '" + UserNameMySQL + "'; " + "password = '" + PwdMySQL + "'";
                    conn.Open();
			} catch(Exception ed)  {
				    Interaction.MsgBox("The system failed to establish a connection", MsgBoxStyle.Information, "Database Settings");
			}

		}


		public static void DisconnMy()
		{
			conn.Close();
			conn.Dispose();

		}

		public static void SaveData()
		{
			string AppName = Application.ProductName;

			Interaction.SaveSetting(AppName, "DBSection", "DB_Name", DBNameMySQL);
			Interaction.SaveSetting(AppName, "DBSection", "DB_IP", ServerMySQL);
			Interaction.SaveSetting(AppName, "DBSection", "DB_Port", PortMySQL);
			Interaction.SaveSetting(AppName, "DBSection", "DB_User", UserNameMySQL);
			Interaction.SaveSetting(AppName, "DBSection", "DB_Password", PwdMySQL);

			Interaction.MsgBox("Database connection settings are saved.", MsgBoxStyle.Information);
		}
    }
}

