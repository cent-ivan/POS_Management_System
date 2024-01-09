using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace POS_Management_System
{
    public partial class Inventory : Form
    {
        public Inventory()
        {
            InitializeComponent();
        }


        private void Inventory_Load(object sender, EventArgs e)
        {
            ClearTextBoxes();
            LoadTable();
        }//On load events

        private void invGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow gridViewRow = invGrid.Rows[e.RowIndex];
                txtID.Text = gridViewRow.Cells[0].Value.ToString();
                txtUPC.Text = gridViewRow.Cells[1].Value.ToString();
                txtDes.Text = gridViewRow.Cells[2].Value.ToString();
                txtDiscount.Text = gridViewRow.Cells[3].Value.ToString();
                combCategory.Text = gridViewRow.Cells[4].Value.ToString();
                txtUnit.Text = gridViewRow.Cells[5].Value.ToString();
                txtQuantity.Text = gridViewRow.Cells[6].Value.ToString();
            }
        }//Click cell event

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtID.Text))
                {
                    //check if text box is full
                    if (string.IsNullOrEmpty(txtUPC.Text) || string.IsNullOrEmpty(txtDes.Text) || string.IsNullOrEmpty(txtDiscount.Text) || string.IsNullOrEmpty(combCategory.Text) || string.IsNullOrEmpty(txtUnit.Text) || string.IsNullOrEmpty(txtQuantity.Text))
                    {
                        MessageBox.Show("Please fill up the boxes", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        InsertProduct();//Calls the Indsertion method
                        ClearTextBoxes();//Clears textboxes
                        LoadTable();//refereshes the table after inserting
                    }
                }
                else
                {
                    MessageBox.Show($"Item is already in the inventory", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }//Message Box Info if txtId has value
            }
            catch(MySqlException ex){
                MessageBox.Show($"Error on Mysql {ex}", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }//SAVE button

        private void newButton_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
        }//NEW Button

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show($"Do you want to Delete Product {txtID.Text}?", "DELETE", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                DeleteProduct();
            }
        }//DELETE Button

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show($"Please select an Item", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                UpdateProduct();
                LoadTable();
            }
        }//UPDATE Button

        private void clearButton_Click(object sender, EventArgs e)
        {
            txtUPC.Clear();
            txtDes.Clear();
            txtDiscount.Clear();
            combCategory.Text = " ";
            txtUnit.Clear();
            txtQuantity.Clear();
        }//CLEAR Button

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

                //showing data in the table
                invGrid.DataSource = invTbl;
            }
                
        }//For search text box

        private void comboCategory_TextChanged(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
            using (conn)
            {
                conn.Open();
                MySqlDataAdapter mysqlAdaptor = new MySqlDataAdapter($"SELECT invID AS ID, UPC, Description, Discount, Category, unitPrice AS Price, Quantity " +
                    $"FROM inventory_tbl WHERE  Category = '{comboCategory.Text}';", conn);
                DataTable invTbl = new DataTable();
                invTbl.Clear();
                mysqlAdaptor.Fill(invTbl);

                //showing data in the table
                invGrid.DataSource = invTbl;
            }

        }//For categorizing

        private void backButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Dashboard ds = new Dashboard();
            ds.ShowDialog();
            Application.Exit();
        }//BACK button

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



        private void ClearTextBoxes()
        {
            txtID.Clear(); 
            txtUPC.Clear();
            txtDes.Clear();
            txtDiscount.Clear();
            combCategory.Text = " ";
            txtUnit.Clear();
            txtQuantity.Clear();
        }//Text Clearing Method

        //--Mysql Commands-------------------------------------------------------------------------------------------------------
        public void LoadTable()
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

        private void InsertProduct()
        {
            int rowsInserted = 0;
            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
            using (conn)
            {
                conn.Open();
                string insertProduct = "INSERT INTO inventory_tbl(UPC, Description, Discount, Category, unitPrice, Quantity) VALUES " +
                    "(@upc, @description, @discount, @category, @unitPrice, @quantity);";
                MySqlCommand insertInv = new MySqlCommand(insertProduct, conn);
                insertInv.Parameters.AddWithValue("@upc", txtUPC.Text);
                insertInv.Parameters.AddWithValue("@description", txtDes.Text);
                insertInv.Parameters.AddWithValue("@discount", txtDiscount.Text);
                insertInv.Parameters.AddWithValue("@category", combCategory.Text.ToUpper());
                insertInv.Parameters.AddWithValue("@unitPrice", txtUnit.Text);
                insertInv.Parameters.AddWithValue("@quantity", txtQuantity.Text);
                rowsInserted = insertInv.ExecuteNonQuery();
                MessageBox.Show("Product Entered", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);  
            }

        }//Insert data in the table

        private void UpdateProduct()
        {
            int rowsInserted = 0;
            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
            using (conn)
            {
                conn.Open();
                string updateProduct = "UPDATE inventory_tbl SET UPC = @upc, Description = @description, Discount =  @discount, " +
                    "Category = @category, unitPrice = @unitPrice, Quantity = @quantity WHERE invID = @invID";
                MySqlCommand updateInv = new MySqlCommand(updateProduct, conn);
                updateInv.Parameters.AddWithValue("invID", txtID.Text);
                updateInv.Parameters.AddWithValue("@upc", txtUPC.Text);
                updateInv.Parameters.AddWithValue("@description", txtDes.Text);
                updateInv.Parameters.AddWithValue("@discount", txtDiscount.Text);
                updateInv.Parameters.AddWithValue("@category", combCategory.Text.ToUpper());
                updateInv.Parameters.AddWithValue("@unitPrice", txtUnit.Text);
                updateInv.Parameters.AddWithValue("@quantity", txtQuantity.Text);
                rowsInserted = updateInv.ExecuteNonQuery();
                MessageBox.Show("Product Updated", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }//Update data in database

        private void DeleteProduct()
        {
            int rowsInserted = 0;
            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
            using (conn)
            {
                conn.Open();
                string deleteProduct = "UPDATE inventory_tbl SET UPC = @upc, Description = @description, Discount =  @discount, " +
                    "Category = @category, unitPrice = @unitPrice, Quantity = @quantity WHERE invID = @invID";
                MySqlCommand deleteInv = new MySqlCommand(deleteProduct, conn);
                rowsInserted = deleteInv.ExecuteNonQuery();
                MessageBox.Show("Product Deleted", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }//Delete data in database

        
    }//Inventory Form
}
