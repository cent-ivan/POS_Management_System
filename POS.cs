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
        public POS()
        {
            InitializeComponent();
        }
        private void POS_Load(object sender, EventArgs e)
        {
            string transCode = DateTime.Now.ToString("yyyy-MM"); //Complete
            transNumber.Text = transCode;
        }

        private void searchButton_Click(object sender, EventArgs e)
        {

        }//Search Button Event


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
  
    }
}
