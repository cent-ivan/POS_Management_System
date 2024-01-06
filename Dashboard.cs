using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace POS_Management_System
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

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
            //this.Hide();
            //Report report = new Report();
            //report.ShowDialog();
            //Application.Exit();
        }//Report Button


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

    }
}
