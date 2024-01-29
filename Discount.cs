using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace POS_Management_System
{
    public partial class Discount : Form
    {
        public static string setValueDiscount;

        public Discount()
        {
            InitializeComponent();
        }

        private void Discount_Load(object sender, EventArgs e)
        {
            discountTxt.Text = "0";
        }//On load event

        private void comboDiscount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboDiscount.Text == "SENIOR")
            {
                discountTxt.Text = "20";
            }
            else if (comboDiscount.Text == "PWD")
            {
                discountTxt.Text = "20";
            }
            else if (comboDiscount.Text == "INDIGENOUS")
            {
                discountTxt.Text = "25";
            }
            else if (comboDiscount.Text == "VALUE CARD")
            {
                discountTxt.Text = "10";
            }
            else
            {
                discountTxt.Text = "0";
            }
        }//Choose dicount

        private void confirm_Click(object sender, EventArgs e)
        {
            setValueDiscount = discountTxt.Text;
            this.Hide();
            POS pos = new POS();
            pos.ShowDialog();
            Application.Exit();
        }//Confirm button

        private void backButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            var pos = new POS();
            pos.ShowDialog();
            Application.Exit();
        }//Back Button
        
    }
}
