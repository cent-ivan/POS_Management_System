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
        public static string discountedTotal;

        public POS()
        {
            InitializeComponent();
        }
        private void POS_Load(object sender, EventArgs e)
        {
            discountLabel.Text = Discount.setValueDiscount;

            //Sets Transaction ID
            string transCode = DateTime.Now.ToString("yyyy-MM"); //Complete
            transNumber.Text = transCode + "-1";

            //Check the prices
            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
            using (conn)
            {
                conn.Open();
                string Query = "SELECT COUNT(UPC) FROM cart_tbl";
                MySqlCommand Command = new MySqlCommand(Query, conn);
                int rowsInserted = Convert.ToInt32(Command.ExecuteScalar().ToString());

                if (rowsInserted >= 1)
                { 
                    //Compute total price
                    using (conn)
                    {
                        try
                        {
                            string getTransactionTotal = "SELECT SUM(Subtotal) FROM cart_tbl;";
                            MySqlCommand cmd = new MySqlCommand(getTransactionTotal, conn);
                            totalLabel.Text = cmd.ExecuteScalar().ToString();
                        }
                        catch (NullReferenceException)
                        {
                            totalLabel.Text = "0.00";
                        }
                    }
                }
                else
                {
                    totalLabel.Text = "0.00";
                }
            }

            //Computes total price with discount
            if (String.IsNullOrEmpty(discountLabel.Text))
            {
                decimal totalPrice = Convert.ToDecimal(totalLabel.Text);
                discountLabel.Text = "0";
                finalTotal.Text = totalLabel.Text;
                discountedTotal  = finalTotal.Text;
            }
            else
            {
                decimal totalPrice = Convert.ToDecimal(totalLabel.Text);
                int discount = Convert.ToInt32(discountLabel.Text);
                decimal finalPrice = totalPrice - (totalPrice * discount / 100);
                finalTotal.Text = Convert.ToString(finalPrice);
                discountedTotal = finalTotal.Text;
            }

            LoadTable();
        }//On Load event

        private void dataGridCart_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow gridViewRow = dataGridCart.Rows[e.RowIndex];
                txtUPC.Text = gridViewRow.Cells[1].Value.ToString();
                txtDes.Text = gridViewRow.Cells[2].Value.ToString();
                txtUnit.Text = gridViewRow.Cells[3].Value.ToString();
                txtQuantity.Text = gridViewRow.Cells[4].Value.ToString();
                txtSubTotal.Text = gridViewRow.Cells[6].Value.ToString();
            }
        }//Cell click event for cart_tbl


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
                totalLabel.Text = "0.00";
                discountLabel.Text = "0";
                finalTotal.Text = "0.00";

                LoadTable();
            }
        }//NEW Transaction button event

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

        private void paymentButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Payment pay = new Payment();
            pay.ShowDialog();
            Application.Exit();
        }//Payment Button Event

        private void clearButton_Click(object sender, EventArgs e)
        {
            if (dataGridCart.Rows.Count >= 1)
            {
                ClearItems();
            }
            else
            {
                MessageBox.Show("Put something in the cart first.", "Notice");
            }  
        }//Clear Button

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

        private void panel2_Click(object sender, EventArgs e)
        {
            decimal totalPrice = Convert.ToDecimal(totalLabel.Text);
            int discount = Convert.ToInt32(discountLabel.Text);
            decimal finalPrice = totalPrice - (totalPrice * discount / 100);
            finalTotal.Text = Convert.ToString(finalPrice);
        }//Click the total container for price update

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
        }//Loads table

        private void ClearItems()
        {
            DialogResult result = MessageBox.Show("Clear Cart?", "Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
                using (conn)
                {
                    conn.Open();
                    //Creating a temp data table for the retrieval of UPC and Quantity of item in cart
                    var dataTbl1 = new DataTable();
                    dataTbl1.Load(new MySqlCommand("SELECT UPC, Quantity FROM cart_tbl;", conn).ExecuteReader());

                    //Creating a temp data table for the retrieval of UPC and Quantity of item in cart
                    var dataTbl2 = new DataTable();
                    dataTbl2.Load(new MySqlCommand("SELECT Quantity FROM inventory_tbl;", conn).ExecuteReader());

                    foreach (var row1 in dataTbl1.AsEnumerable())
                    {
                        string itemUPC = row1["UPC"].ToString();
                        int cartQty = Convert.ToInt32(row1["Quantity"]);

                        //Gets the total qty left in the inventory of said item
                        string Query = $"SELECT Quantity FROM inventory_tbl WHERE UPC = {itemUPC}";
                        MySqlCommand cmd = new MySqlCommand(Query, conn);
                        int invQty = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                        int restoreQty = cartQty + invQty;;
                        string updateQry = "UPDATE inventory_tbl SET Quantity = @qty WHERE UPC = @upc";
                        var updateInv = new MySqlCommand(updateQry, conn);
                        updateInv.Parameters.AddWithValue("@upc", itemUPC);
                        updateInv.Parameters.AddWithValue("@qty", restoreQty);
                        updateInv.ExecuteNonQuery();
                    }

                    //Clears the cart
                    MySqlDataAdapter mySqlAdapter = new MySqlDataAdapter("TRUNCATE TABLE cart_tbl;", conn);
                    DataTable cartTbl = new DataTable();
                    mySqlAdapter.Fill(cartTbl);

                    //showing data
                    dataGridCart.DataSource = cartTbl;
                }

                totalLabel.Text = "0.00";
                discountLabel.Text = "0";
                finalTotal.Text = "0.00";

                LoadTable();
            }
        } //Clear Item method

    }
}
