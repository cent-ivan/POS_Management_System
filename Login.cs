﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;

namespace POS_Management_System
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
        
        private void Login_Load(object sender, EventArgs e)
        {
            int row = 0;
            MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db"); //for table queries


            using (MySqlConnection connCreate = new MySqlConnection("datasource=localhost;port=3306;username=root;password=")) //temp connection
            {
                connCreate.Open();//temp connection open
                //Checking of existing database

                string checkQry = "SHOW DATABASES LIKE '%pos_system_db%';";
                MySqlCommand cmd = new MySqlCommand(checkQry, connCreate);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    row++;
                }
                reader.Close();//closes the current Reader

                if (row == 0)
                {
                    int rowsInserted = 0;

                    //Database
                    string createDBQry = "CREATE DATABASE pos_system_db;";
                    MySqlCommand createDB = new MySqlCommand(createDBQry, connCreate);
                    rowsInserted = createDB.ExecuteNonQuery();

                    using (conn)
                    {
                        conn.Open();
                        //Tables-------------------------------------------------------------

                        string profTBLQry = "CREATE TABLE profile_tbl(profID INT AUTO_INCREMENT PRIMARY KEY," +
                        "storeID INT NOT NULL, empName VARCHAR(30) NOT NULL, empSex VARCHAR(7) NOT NULL, empRole VARCHAR(15) NOT NULL, " +
                        "empEmail VARCHAR(50) NOT NULL, password VARCHAR(50) NOT NULL);";
                        MySqlCommand profTBL = new MySqlCommand(profTBLQry, conn);
                        rowsInserted = profTBL.ExecuteNonQuery(); //For the profile Table

                        //Creating index for the storeID for faster retrieval
                        string profINDEXQry = "CREATE INDEX profID_idx ON profile_tbl (storeID)";
                        MySqlCommand profINDEX = new MySqlCommand( profINDEXQry, conn);
                        rowsInserted = profINDEX.ExecuteNonQuery();

                        string sampleProfQry = "INSERT INTO profile_tbl(storeID, empName, empSex, empRole, empEmail, password) VALUES " +
                            "(214168, 'John Doe', 'MALE', 'ADMIN', 'sample@gmail.com', '12345');";
                        MySqlCommand sampleQry = new MySqlCommand(sampleProfQry, conn);
                        rowsInserted = sampleQry.ExecuteNonQuery(); //Insert sample profile

                        string invTBLQry = "CREATE TABLE inventory_tbl(invID INT AUTO_INCREMENT PRIMARY KEY, UPC VARCHAR(20) NOT NULL, " +
                        "Description VARCHAR(100) NOT NULL, Discount INT NOT NULL, Category VARCHAR(50) NOT NULL, unitPrice DECIMAL(18,2" +
                        ") NOT NULL, Quantity INT NOT NULL);";
                        MySqlCommand invTBL = new MySqlCommand(invTBLQry, conn);
                        rowsInserted = invTBL.ExecuteNonQuery(); //For the Inventory Table

                        string cartTBLQry = "CREATE TABLE cart_tbl(cartID INT AUTO_INCREMENT PRIMARY KEY, UPC VARCHAR(20) NOT NULL, Description VARCHAR(100) NOT NULL, " +
                        "unitPrice DECIMAL(18,2) NOT NULL, Quantity INT NOT NULL, Discount INT NOT NULL, Subtotal DECIMAL(18,2) NOT NULL, transDate VARCHAR(60) NOT NULL, transID INT NOT NULL);";
                        MySqlCommand cartTBL = new MySqlCommand(cartTBLQry, conn);
                        rowsInserted = cartTBL.ExecuteNonQuery(); //For the Cart table

                        string transTBLQry = "CREATE TABLE trans_tbl(cartID INT AUTO_INCREMENT PRIMARY KEY, UPC VARCHAR(20) NOT NULL, Description VARCHAR(100) NOT NULL, " +
                        "unitPrice DECIMAL(18,2) NOT NULL, Quantity INT NOT NULL, Discount INT NOT NULL, Subtotal DECIMAL(18,2) NOT NULL, transDate VARCHAR(60) NOT NULL, transID INT NOT NULL);";
                        MySqlCommand transTBL = new MySqlCommand(transTBLQry, conn);
                        rowsInserted = transTBL.ExecuteNonQuery(); //For the Transaction

                        string saleTBLQry = "CREATE TABLE sales_tbl(transID INT NOT NULL, Amount DECIMAL(18,2) NOT NULL, " +
                        "Sale_Date VARCHAR(50) NOT NULL, Sale_Time VARCHAR(50) NOT NULL, Items_Sold INT NOT NULL);";
                        MySqlCommand saleTBL = new MySqlCommand(saleTBLQry, conn);
                        rowsInserted = saleTBL.ExecuteNonQuery(); //For the Sale table

                    }//connection for creating table 
                }
                
            }
                
            RotateImages(pictureBox1, pictureBox2, pictureBox3, pictureBox4); //rotates the images passed
        }//on load event and creating of databases and tables

        private void loginButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(idNo.Text) || string.IsNullOrEmpty(password.Text))
                {
                    MessageBox.Show("Please fill up the boxes", "Notice");
                }
                else
                {
                    MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
                    using (conn)
                    {
                        conn.Open();
                        //Check for ID and password

                        string checkQry = "SELECT storeID, password FROM profile_tbl WHERE storeID = @idNo AND password = @password;";
                        MySqlCommand cmd = new MySqlCommand(checkQry, conn);
                        cmd.Parameters.AddWithValue("@idNo", idNo.Text);
                        cmd.Parameters.AddWithValue("@password", password.Text);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            reader.Close();
                            //Checking employee role
                            string checkRole = "SELECT empRole FROM profile_tbl WHERE storeID = @idNo AND password = @password;";
                            MySqlCommand comm = new MySqlCommand(checkRole, conn);
                            comm.Parameters.AddWithValue("@idNo", idNo.Text);
                            comm.Parameters.AddWithValue("@password", password.Text);
                            string result = comm.ExecuteScalar().ToString();

                            if (result.Equals("ADMIN"))
                            {
                                MessageBox.Show("Login Successfully!");
                                this.Hide();
                                Dashboard dsh = new Dashboard();
                                dsh.ShowDialog();
                                Application.Exit();
                            }
                            else
                            {
                                MessageBox.Show("Login Successfully!");
                                this.Hide();
                                POS pos = new POS();
                                pos.ShowDialog();
                                Application.Exit();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Incorrect ID Number or Password", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }//checks if form is empty
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error [{ex}] \nCheck all of the form for empty entry.", "Notice");
            }
        }//login button event

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }//formclosing event surely closes components (DO NOT REMOVE)



        //--Rotates an image----------------------------------------------------------------------------------
        public static Bitmap RotateImg(Bitmap bmp, float angle)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            Bitmap tempImg = new Bitmap(w, h);
            //Turn bitmap into graphics
            Graphics g = Graphics.FromImage(tempImg);

            g.DrawImageUnscaled(bmp, 1, 1);
            g.Dispose();

            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, w, h));

            Matrix mtrx = new Matrix();
            mtrx.Rotate(angle);

            RectangleF rct = path.GetBounds(mtrx);
            Bitmap newImg = new Bitmap(Convert.ToInt32(rct.Width), Convert.ToInt32(rct.Height));

            g = Graphics.FromImage(newImg);
            g.TranslateTransform(-rct.X, -rct.Y);
            g.RotateTransform(angle);
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(tempImg, 0, 0);
            g.Dispose();
            tempImg.Dispose();

            return newImg;
            //copy at https://www.c-sharpcorner.com/UploadFile/shubham0987/rotate-and-flip-images-in-windows-form/
        }
        private static void RotateImages(PictureBox pic1, PictureBox pic2, PictureBox pic3, PictureBox pic4)
        {
            //calls the rotate method--------------------------
            Bitmap bitmap1 = (Bitmap)pic1.Image;
            pic1.Image = (Image)(RotateImg(bitmap1, 330.0f));

            Bitmap bitmap2 = (Bitmap)pic2.Image;
            pic2.Image = (Image)(RotateImg(bitmap2, 390.0f));

            Bitmap bitmap3 = (Bitmap)pic3.Image;
            pic3.Image = (Image)(RotateImg(bitmap3, 160.0f));

            Bitmap bitmap4 = (Bitmap)pic4.Image;
            pic4.Image = (Image)(RotateImg(bitmap4, 30.0f));
        }//rotate image method

    }//Login

    //--Custom Components------------------------------------------------------------------------------------


    class round : TextBox
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // X-coordinate of upper-left corner or padding at start
            int nTopRect,// Y-coordinate of upper-left corner or padding at the top of the textbox
            int nRightRect, // X-coordinate of lower-right corner or Width of the object
            int nBottomRect,// Y-coordinate of lower-right corner or Height of the object
                            //RADIUS, how round do you want it to be?
            int nheightRect, //height of ellipse 
            int nweightRect //width of ellipse
        );
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 3, this.Width, this.Height, 10, 10));
        }
    }

    class RoundPanel : Panel
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // X-coordinate of upper-left corner or padding at start
            int nTopRect,// Y-coordinate of upper-left corner or padding at the top of the textbox
            int nRightRect, // X-coordinate of lower-right corner or Width of the object
            int nBottomRect,// Y-coordinate of lower-right corner or Height of the object
                            //RADIUS, how round do you want it to be?
            int nheightRect, //height of ellipse 
            int nweightRect //width of ellipse
        );
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 3, this.Width, this.Height, 15, 15));
        }
    }

}