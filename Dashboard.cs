using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;

namespace POS_Management_System
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
            using (conn)
            {
                conn.Open();

                //Total Sales
                CountSales();

                //Total Inventory
                string TotalInvQuery = "SELECT COUNT(invID) FROM inventory_tbl";
                MySqlCommand Command2 = new MySqlCommand(TotalInvQuery, conn);
                string invResult = Command2.ExecuteScalar().ToString();

                totalInvLabel.Text = invResult;

                //Total Sold
                CountSold();

                //Total Users
                string TotalUserQuery = "SELECT COUNT(profID) FROM profile_tbl";
                MySqlCommand Command3 = new MySqlCommand(TotalUserQuery, conn);
                string userResult = Command3.ExecuteScalar().ToString();

                totalUser.Text = userResult;
            }
        }//On load event


        private void posButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            POS pos = new POS();
            pos.ShowDialog();
            Application.Exit();
        }//pos button

        private void invButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Inventory inv = new Inventory();
            inv.ShowDialog();
            Application.Exit();
        }//Inventory Button

        private void reportButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Report report = new Report();
            report.ShowDialog();
            Application.Exit();
        }//Report Button

        private void signinButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Signup signup = new Signup();
            signup.ShowDialog();
            Application.Exit();
        }//Adding a user

        private void logoutButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to logout?", "Log Out", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                this.Hide();
                Login login = new Login();
                login.ShowDialog();
                Application.Exit();
            }
        }//Log Out Button

        private void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }//formclosing event surely closes components (DO NOT REMOVE)

        
        //Total Sales Method
        private void CountSales()
        {
            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
            using (conn)
            {
                conn.Open();
                int rows = 0;

                string showQuery = "SELECT * FROM sales_tbl;";
                MySqlCommand cmd = new MySqlCommand(showQuery, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) //counts each rows shown by the query
                {
                    rows++;
                }

                if (rows == 0)
                {
                    totalSaleLabel.Text = "0.00";
                }
                else
                {
                    reader.Close();
                    string TotalSaleQuery = "SELECT SUM(Amount) FROM sales_tbl";
                    MySqlCommand Command1 = new MySqlCommand(TotalSaleQuery, conn);
                    decimal saleResult = Convert.ToDecimal(Command1.ExecuteScalar().ToString());

                    totalSaleLabel.Text = saleResult.ToString();
                }
                
                reader.Close();
            }
        }

        //Total Sold Method
        private void CountSold()
        {
            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
            using (conn)
            {
                conn.Open();
                int rows = 0;

                string showQuery = "SELECT * FROM sales_tbl;";
                MySqlCommand cmd = new MySqlCommand(showQuery, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) //counts each rows shown by the query
                {
                    rows++;
                }

                if (rows == 0)
                {
                    totalSoldLabel.Text = "0";
                }
                else
                {
                    reader.Close();
                    string TotalSoldQuery = "SELECT SUM(Items_Sold) FROM sales_tbl";
                    MySqlCommand Command1 = new MySqlCommand(TotalSoldQuery, conn);
                    decimal soldResult = Convert.ToDecimal(Command1.ExecuteScalar().ToString());

                    totalSoldLabel.Text = soldResult.ToString();
                }
            }
        }
    }
}
