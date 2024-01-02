using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace POS_Management_System
{
    public partial class Signup : Form
    {
        public Signup()
        {
            InitializeComponent();
        }

        private void Signup_Load(object sender, EventArgs e)
        {
            RotateImages(pictureBox2);
        }//on load event

        private void backButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login login = new Login();
            login.ShowDialog();
            Application.Exit();
        }//back button click

        private void signupButton_Click(object sender, EventArgs e)
        {
            bool IsRadioChecked = string.IsNullOrEmpty(maleRadio.Text) || string.IsNullOrEmpty(femaleRadio.Text);

            if (string.IsNullOrEmpty(idNo.Text) || string.IsNullOrEmpty(nameTxt.Text) || IsRadioChecked || string.IsNullOrEmpty(roleCombo.Text) || string.IsNullOrEmpty(emailTxt.Text))
            {
                MessageBox.Show("Please fill up the boxes", "Notice");
            }
            else
            {
                MySqlConnection conn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=pos_system_db");
                using (conn)
                {
                    int rowsInserted = 0;

                    conn.Open();
                    //Insert profile info

                    string insertQry = "INSERT INTO profile_tbl(storeID, empName, empSex, empRole, empEmail) VALUES" +
                        "(@storeID, @empName, @empSex, @empRole, @empEmail);";
                    MySqlCommand profInsert = new MySqlCommand(insertQry, conn);
                    profInsert.Parameters.AddWithValue("@storeID", idNo.Text);
                    profInsert.Parameters.AddWithValue("@empName", nameTxt.Text);

                    //for radio button sex
                    if (maleRadio.Checked)
                    {
                        profInsert.Parameters.AddWithValue("@empSex", maleRadio.Text);
                    }
                    else
                    {
                        profInsert.Parameters.AddWithValue("@empSex", femaleRadio.Text);
                    }

                    profInsert.Parameters.AddWithValue("@empRole", roleCombo.Text);
                    profInsert.Parameters.AddWithValue("@empEmail", emailTxt.Text);
                    rowsInserted = profInsert.ExecuteNonQuery();

                    idNo.Clear();
                    nameTxt.Clear();
                    roleCombo.Text = "";
                    emailTxt.Clear();
                }
            }//checking if text is empty   
        }//sign up


        private void Signup_FormClosing(object sender, FormClosingEventArgs e)
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
        private static void RotateImages(PictureBox pic2)
        {
            //calls the rotate method--------------------------
            Bitmap bitmap2 = (Bitmap)pic2.Image;
            pic2.Image = (Image)(RotateImg(bitmap2, 390.0f));
        }//rotate image method

        
    }//Signup
}
