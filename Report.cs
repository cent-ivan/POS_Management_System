using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_Management_System
{
    public partial class Report : Form
    {
        public Report()
        {
            InitializeComponent();
        }

        private void Report_Load(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
            using (conn)
            {
                conn.Open();
                string query = "SELECT transID AS ID, Amount, Sale_Date, Sale_Time, Items_Sold FROM sales_tbl;";
                MySqlDataAdapter mysqlAdapter = new MySqlDataAdapter(query, conn);
                DataTable invtbl = new DataTable();
                mysqlAdapter.Fill(invtbl);

                //inserting data from the table
                reportGrid.DataSource = invtbl;
            }
        }//On load event

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Dashboard ds = new Dashboard();
            ds.ShowDialog();
            Application.Exit();
        }//back button

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
            using (conn)
            {
                conn.Open();
                MySqlDataAdapter mysqlAdaptor = new MySqlDataAdapter($"SELECT transID AS ID, Amount, Sale_Date, Sale_Time, Items_Sold " +
                    $"FROM sales_tbl WHERE CONCAT(Sale_Date, Sale_Time) LIKE '%{searchBox.Text}%'", conn);
                DataTable repTbl = new DataTable();
                repTbl.Clear();
                mysqlAdaptor.Fill(repTbl);

                //showing data in the table
                reportGrid.DataSource = repTbl;
            }
        }
    }
}
