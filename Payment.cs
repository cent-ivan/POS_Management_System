﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace POS_Management_System
{
    public partial class Payment : Form
    {
        public Payment()
        {
            InitializeComponent();
            this.ActiveControl = txtAmount;
        }

        private void Payment_Load(object sender, EventArgs e)
        {
            //Computes the total
            txtTotal.Text = POS.discountedTotal;

        }//On load event

        private void enter_Click(object sender, EventArgs e)
        {
            decimal total = Convert.ToDecimal(txtTotal.Text);
            decimal change = Convert.ToDecimal(txtAmount.Text) - total;
            txtChange.Text = change.ToString();
        }//Enter to view change

        private void Print_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show($"Confirm Transaction?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                InsertSale();
            }  
        }//Insert data to sales and print receipt


        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                decimal total = Convert.ToDecimal(txtTotal.Text);
                decimal change = Convert.ToDecimal(txtAmount.Text) - total;
                txtChange.Text = change.ToString();
            }
        }//if enter key pressed


        private void backButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            var pos = new POS();
            pos.ShowDialog();
            Application.Exit();
        }//Back button


        //Calculator---------------------------------------------------------------------------------------------------
        private void zero_Click(object sender, EventArgs e)
        {
            if(txtAmount.Text == "0" && txtAmount.Text != null)
            {
                txtAmount.Text = "0";
            }
            else
            {
                txtAmount.Text += "0";
            }
        }

        private void one_Click(object sender, EventArgs e)
        {
            if (txtAmount.Text == "0" && txtAmount.Text != null)
            {
                txtAmount.Text = "1";
            }
            else
            {
                txtAmount.Text += "1";
            }
        }

        private void two_Click(object sender, EventArgs e)
        {
            if (txtAmount.Text == "0" && txtAmount.Text != null)
            {
                txtAmount.Text = "2";
            }
            else
            {
                txtAmount.Text += "2";
            }
        }

        private void three_Click(object sender, EventArgs e)
        {
            if (txtAmount.Text == "0" && txtAmount.Text != null)
            {
                txtAmount.Text = "3";
            }
            else
            {
                txtAmount.Text += "3";
            }
        }

        private void four_Click(object sender, EventArgs e)
        {
            if (txtAmount.Text == "0" && txtAmount.Text != null)
            {
                txtAmount.Text = "4";
            }
            else
            {
                txtAmount.Text += "4";
            }
        }

        private void five_Click(object sender, EventArgs e)
        {
            if (txtAmount.Text == "0" && txtAmount.Text != null)
            {
                txtAmount.Text = "5";
            }
            else
            {
                txtAmount.Text += "5";
            }
        }

        private void six_Click(object sender, EventArgs e)
        {
            if (txtAmount.Text == "0" && txtAmount.Text != null)
            {
                txtAmount.Text = "6";
            }
            else
            {
                txtAmount.Text += "6";
            }
        }

        private void seven_Click(object sender, EventArgs e)
        {
            if (txtAmount.Text == "0" && txtAmount.Text != null)
            {
                txtAmount.Text = "7";
            }
            else
            {
                txtAmount.Text += "7";
            }
        }

        private void eight_Click(object sender, EventArgs e)
        {
            if (txtAmount.Text == "0" && txtAmount.Text != null)
            {
                txtAmount.Text = "8";
            }
            else
            {
                txtAmount.Text += "8";
            }
        }

        private void nine_Click(object sender, EventArgs e)
        {
            if (txtAmount.Text == "0" && txtAmount.Text != null)
            {
                txtAmount.Text = "9";
            }
            else
            {
                txtAmount.Text += "9";
            }
        }

        private void dot_Click(object sender, EventArgs e)
        {
            bool hasPeriod = false;
            foreach (var ele in txtAmount.Text)
            {
                if (ele.Equals("."))
                {
                    hasPeriod = true;
                }
            }

            if (txtAmount.Text == "0" && txtAmount.Text != null)
            {
                txtAmount.Text = "0.";
            }
            else
            {
                if (hasPeriod)
                {
                    txtAmount.Text += "";
                }
                else
                {
                    txtAmount.Text += ".";
                }
            }
        }

        private void thousand_Click(object sender, EventArgs e)
        {
            if (txtAmount.Text == "0" && txtAmount.Text != null)
            {
                txtAmount.Text = "1000";
            }
        }

        private void clear_Click(object sender, EventArgs e)
        {
            txtAmount.Text = "0";
        }


        //---SQL Methods--------------------------------------------------------------------------------------
        private void InsertSale()
        {
            string date_now = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("hh:mm tt");

            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
            using (conn)
            {
                conn.Open();
                string itemsQuery = $"SELECT SUM(Quantity) FROM cart_tbl";
                MySqlCommand cmd = new MySqlCommand(itemsQuery, conn);
                string total_sold = cmd.ExecuteScalar().ToString();

                string salesProduct = "INSERT INTO sales_tbl(transID, Amount, Sale_Date, Sale_Time, Items_Sold) VALUES " +
                    "(@transid, @amount, @today, @time, @items_sold);";
                MySqlCommand insertSales = new MySqlCommand(salesProduct, conn);
                insertSales.Parameters.AddWithValue("@transid", POS.TransId);
                insertSales.Parameters.AddWithValue("@amount", txtTotal.Text);
                insertSales.Parameters.AddWithValue("@today", date_now);
                insertSales.Parameters.AddWithValue("@time", time);
                insertSales.Parameters.AddWithValue("@items_sold", total_sold);

                int rowsInserted = insertSales.ExecuteNonQuery();
                MessageBox.Show("Transaction Recorded", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    }
}
