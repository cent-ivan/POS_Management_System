using System;
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
                        "empEmail VARCHAR(50) NOT NULL);";
                        MySqlCommand profTBL = new MySqlCommand(profTBLQry, conn);
                        rowsInserted = profTBL.ExecuteNonQuery();
                    }//connection for creating table 
                }
                else
                {
                    MessageBox.Show("Database already existed");
                }
            }
                
            RotateImages(pictureBox1, pictureBox2, pictureBox3, pictureBox4); //rotates the images passed
        }//on load event and creating of databases and tables

        private void loginButton_Click(object sender, EventArgs e)
        {

        }//login button event

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            Signup signup = new Signup();
            signup.ShowDialog();
            Application.Exit();
        }//link sign up event

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