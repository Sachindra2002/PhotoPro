using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Photo_Editor
{
    public partial class Form1 : Form
    {
        Image imgOriginal;
        public Form1()
        {
            InitializeComponent();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk_1(object sender, CancelEventArgs e)
        {
            try
            {
                string imgFileName = openFileDialog1.FileName;
                imgOriginal = Image.FromFile(imgFileName);
                pictureBox1.Image = imgOriginal;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        Image convert2gray(object obj)
        {
            Image imgColor = (Image)obj;
            Image imgGray = null;
            Bitmap bmp1 = new Bitmap(imgColor);
            Bitmap bmp2 = new Bitmap(imgColor.Width, imgColor.Height);
            Color c1, c2;
            for (int i = 1; i < imgOriginal.Width; i++)
            {
                for (int j = 1; j < imgOriginal.Height; j++)
                {
                    c1 = bmp1.GetPixel(i, j);
                    int c2A = (int)c1.A;
                    int c2Gray = (int)(
                    (Convert.ToDouble(c1.R) * 0.3) +
                    (Convert.ToDouble(c1.G) * 0.59) +
                    (Convert.ToDouble(c1.B) * 0.11)
                    );
                    c2 = Color.FromArgb(c2A, c2Gray, c2Gray, c2Gray);
                    bmp2.SetPixel(i, j, c2);
                }
            }
            imgGray = (Image)bmp2;
            return imgGray;
        }

        Image flip180(object obj)
        {
            Image imgColor = (Image)obj;
            Image imgFlip180 = null;
            Bitmap bmp1 = new Bitmap(imgColor);
            Bitmap bmp2 = new Bitmap(imgColor.Width, imgColor.Height);
            Color c1, c2;

            for (int i = 0; i < imgOriginal.Width; i++)
            {
                for (int j = 0; j < imgOriginal.Height; j++)
                {
                    c1 = bmp1.GetPixel(i, j);

                }
            }
            imgFlip180 = (Image)bmp2;
            return imgFlip180;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Func<object, Image> func = new Func<object, Image>(convert2gray);
            Task<Image> t = new Task<Image>(func, imgOriginal);
            t.Start();
            t.ContinueWith((task) =>
            {
                pictureBox2.Image = task.Result;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
