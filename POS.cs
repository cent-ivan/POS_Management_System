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
    public partial class POS : Form
    {
        public static string setValueDiscountPay;

        public POS()
        {
            InitializeComponent();
        }
        private void POS_Load(object sender, EventArgs e)
        {
            discountLabel.Text = Discount.setValueDiscount;
            setValueDiscountPay = discountLabel.Text;

            //Sets Transaction ID
            string transCode = DateTime.Now.ToString("yyyy-MM"); //Complete
            transNumber.Text = transCode;

            //Compute total price
            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
            using (conn)
            {
                try
                {
                    conn.Open();
                    string getTransactionTotal = "SELECT SUM(Subtotal) FROM cart_tbl;";
                    MySqlCommand cmd = new MySqlCommand(getTransactionTotal, conn);
                    totalLabel.Text = cmd.ExecuteScalar().ToString();
                }
                catch (NullReferenceException)
                {
                    totalLabel.Text = "0.00";
                }  
            }

            //Computes total price with discount
            decimal totalPrice = Convert.ToDecimal(totalLabel.Text);
            if (String.IsNullOrEmpty(discountLabel.Text))
            {
                discountLabel.Text = "0";
                finalTotal.Text = totalLabel.Text;
            }
            else
            {
                int discount = Convert.ToInt32(discountLabel.Text);
                decimal finalPrice = totalPrice - (totalPrice * discount / 100);
                finalTotal.Text = Convert.ToString(finalPrice);
            }

            LoadTable();

            //Create total label
        }//Transaction number generator


        private void transButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("New Transaction?", "New Transaction", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
                using (conn)
                {
                    //Clears the table
                    MySqlDataAdapter mySqlAdapter = new MySqlDataAdapter("TRUNCATE TABLE cart_tbl;", conn);
                    DataTable cartTbl = new DataTable();
                    mySqlAdapter.Fill(cartTbl);

                    //showing data
                    dataGridCart.DataSource = cartTbl;
                }
                totalLabel.Text = "0";
                discountLabel.Text = "0";
                finalTotal.Text = "0";

                LoadTable();
            }

        }//NEW Transaction button

        private void searchButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            searchProduct search = new searchProduct();
            search.ShowDialog();
            Application.Exit();
        }//Search Button Event

        private void discountButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Discount discountFrm = new Discount();
            discountFrm.ShowDialog();
        }//Discount Button Event

        private void backButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Dashboard ds = new Dashboard();
            ds.ShowDialog();
            Application.Exit();
        }//Back Button

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

        private void POS_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }//formclosing event surely closes components (DO NOT REMOVE)

        //--MySql Methods-------------------------------------------------------------------------

        private void LoadTable()
        {
            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
            using (conn)
            {
                conn.Open();
                string query = "SELECT cartID AS ID, UPC, Description, unitPrice AS Price, Quantity,  Discount, subtotal FROM cart_tbl;";
                MySqlDataAdapter mysqlAdapter = new MySqlDataAdapter(query, conn);
                DataTable invtbl = new DataTable();
                mysqlAdapter.Fill(invtbl);

                //inserting data from the table
                dataGridCart.DataSource = invtbl;
            }
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            decimal totalPrice = Convert.ToDecimal(totalLabel.Text);
            int discount = Convert.ToInt32(discountLabel.Text);
            decimal finalPrice = totalPrice - (totalPrice * discount / 100);
            finalTotal.Text = Convert.ToString(finalPrice);
        }
    }
}
