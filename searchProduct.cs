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
    public partial class searchProduct : Form
    {
        protected decimal unitPrice;
        protected int discount;
        protected decimal subtotal = 0;
        private int prodQty;
        public searchProduct()
        {
            InitializeComponent();
        }

        private void searchProduct_Load(object sender, EventArgs e)
        {
            LoadTable();
        }//On load events

        private void invGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
            using (conn)
            {
                conn.Open();
                //checking the qunatity
                string quantity_query = "SELECT Quantity FROM inventory_tbl;";
                MySqlCommand cmd = new MySqlCommand(quantity_query, conn);
                int stock = Convert.ToInt32(cmd.ExecuteScalar());

                if (stock == 0)
                {
                    MessageBox.Show("Out of Stock");
                }
                else
                {
                    DataGridViewRow gridViewRow = invGrid.Rows[e.RowIndex];
                    if (e.RowIndex != -1)
                    {
                        txtID.Text = gridViewRow.Cells[0].Value.ToString();
                        txtUPC.Text = gridViewRow.Cells[1].Value.ToString();
                        txtDes.Text = gridViewRow.Cells[2].Value.ToString();
                        txtDiscount.Text = gridViewRow.Cells[3].Value.ToString();
                        txtUnit.Text = gridViewRow.Cells[5].Value.ToString();
                    }

                    //Assigning table data to variables with discount computation
                    discount = Convert.ToInt32(gridViewRow.Cells[3].Value.ToString());
                    unitPrice = Convert.ToDecimal(gridViewRow.Cells[5].Value.ToString());

                    //Auto assign the minimum and maximum in the number
                    productNum.Minimum = 1;
                    productNum.Maximum = Convert.ToInt32(gridViewRow.Cells[6].Value.ToString());
                    prodQty = Convert.ToInt32(gridViewRow.Cells[6].Value.ToString());

                }  
            }
            LoadTable();
            
        }//Click cell event

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
            using (conn)
            {
                conn.Open();
                MySqlDataAdapter mysqlAdaptor = new MySqlDataAdapter($"SELECT invID AS ID, UPC, Description, Discount, Category, unitPrice AS Price, Quantity " +
                $"FROM inventory_tbl WHERE CONCAT(UPC, Description) LIKE '%{searchBox.Text}%'", conn);
                DataTable invTbl = new DataTable();
                invTbl.Clear();
                mysqlAdaptor.Fill(invTbl);

                //inserting data from the table
                invGrid.DataSource = invTbl;
            }
        }//Search Box code


        private void addCartButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Add To Cart?", "Add To Cart", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                //computing the discount
                if (discount == 0)
                {
                    subtotal = unitPrice * Convert.ToInt32(productNum.Value);
                }
                else
                {
                    decimal discountTotal = unitPrice - (unitPrice * (discount / 100));
                    subtotal = discountTotal * Convert.ToInt32(productNum.Value);
                }
                MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
                using (conn)
                {
                    conn.Open();
                    string today = DateTime.Now.ToString("dd-MMM-yyyy, ddd hh:mm tt");
                    string insertCart = "INSERT INTO cart_tbl(UPC, Description, unitPrice, Quantity, Discount, Subtotal) VALUES " +
                        "(@upc, @description, @unitPrice, @quantity, @discount, @subtotal);";
                    MySqlCommand cartInsert = new MySqlCommand(insertCart, conn);
                    cartInsert.Parameters.AddWithValue("@upc", txtUPC.Text);
                    cartInsert.Parameters.AddWithValue("@description", txtDes.Text);
                    cartInsert.Parameters.AddWithValue("@unitPrice", txtUnit.Text);
                    cartInsert.Parameters.AddWithValue("@quantity", productNum.Value);
                    cartInsert.Parameters.AddWithValue("@discount", txtDiscount.Text);
                    cartInsert.Parameters.AddWithValue("@subtotal", subtotal);
                    int rowsInserted =  cartInsert.ExecuteNonQuery();

                    int updatedQty = prodQty -  Convert.ToInt32(productNum.Value);
                    string updateQuery = "UPDATE inventory_tbl SET Quantity = @updatedQty WHERE invID = @cartid";
                    MySqlCommand updateInv = new MySqlCommand(updateQuery, conn);
                    updateInv.Parameters.AddWithValue("@updatedQty", updatedQty);
                    updateInv.Parameters.AddWithValue("@cartid", txtID.Text);
                    rowsInserted = updateInv.ExecuteNonQuery();

                    LoadTable();
                }
            }

        }//Add to Cart Button


        private void backButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            var pos = new POS();
            pos.ShowDialog();
            Application.Exit();

        }//Back Button


        //--Mysql Methods-------------------------------------------------------------------------

        private void LoadTable()
        {
            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
            using (conn)
            {
                conn.Open();
                string query = "SELECT invID AS ID, UPC, Description, Discount, Category, unitPrice AS Price, Quantity FROM inventory_tbl;";
                MySqlDataAdapter mysqlAdapter = new MySqlDataAdapter(query, conn);
                DataTable invtbl = new DataTable();
                mysqlAdapter.Fill(invtbl);

                //inserting data from the table
                invGrid.DataSource = invtbl;
            }

        }//Loads or refreshes the table

    }
}
