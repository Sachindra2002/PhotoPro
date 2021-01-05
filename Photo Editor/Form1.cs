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
            for (int i = 1; i < imgColor.Width; i++)
            {
                for (int j = 1; j < imgColor.Height; j++)
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
        Image flip(object obj)
        {
            Image imgColor = (Image)obj;
            Image imgFlip = null;
            Bitmap bmp1 = new Bitmap(imgColor);
            Bitmap bmp2 = new Bitmap(imgColor.Width, imgColor.Height);
            Color c1, c2;
            var w = imgColor.Width - 1;
            for (int i = 0; i < imgColor.Width; i++)
            {
                var h = imgColor.Height - 1;
                for (int j = 0; j < imgColor.Height; j++)
                {
                    c1 = bmp1.GetPixel(i, j);
                    bmp2.SetPixel(i, h, c1);
                    h--;
                }
                w--;
            }
            imgFlip = (Image)bmp2;
            return imgFlip;
        }
        Image flipV(object obj)
        {
            Image imgColor = (Image)obj;
            Image imgFlip = null;
            Bitmap bmp1 = new Bitmap(imgColor);
            Bitmap bmp2 = new Bitmap(imgColor.Width, imgColor.Height);
            Color c1, c2;
            var w = imgColor.Width - 1;
            for (int i = 0; i < imgColor.Width; i++)
            {
                //var h = imgOriginal.Height - 1;
                for (int j = 0; j < imgColor.Height; j++)
                {
                    c1 = bmp1.GetPixel(i, j);
                    bmp2.SetPixel(w, j, c1);
                    //h--;
                }
                w--;
            }
            imgFlip = (Image)bmp2;
            return imgFlip;
        }

        Image Invert(object obj)
        {
            Image imgColor = (Image)obj;
            Image imgInvert = null;
            Bitmap bmp1 = new Bitmap(imgColor);
            Bitmap bmp2 = new Bitmap(imgColor.Width, imgColor.Height);
            Color c1, c2;
            for (int y = 0; (y <= (imgColor.Height - 1)); y++)
            {
                for (int x = 0; (x <= (imgColor.Width - 1)); x++)
                {
                    Color inv = bmp1.GetPixel(x, y);
                    inv = Color.FromArgb(255, (255 - inv.R), (255 - inv.G), (255 - inv.B));
                    bmp2.SetPixel(x, y, inv);
                }
            }
            imgInvert = (Image)bmp2;
            return imgInvert;

        }

        Image Contrast(object obj, float value)
        {
            Image imgColor = (Image)obj;
            Image imgContrast = null;
            Bitmap bmp1 = new Bitmap(imgColor);
            Bitmap bmp2 = new Bitmap(imgColor.Width, imgColor.Height);

            var contrast = Math.Pow((100.0 + value) / 100.0, 2);
            for (int y = 0; y < imgColor.Height; y++)
            {
                for (int x = 0; x < imgColor.Width; x++)
                {
                    var oldColor = bmp1.GetPixel(x, y);
                    var red = ((((oldColor.R / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                    var green = ((((oldColor.G / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                    var blue = ((((oldColor.B / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                    if (red > 255) red = 255;
                    if (red < 0) red = 0;
                    if (green > 255) green = 255;
                    if (green < 0) green = 0;
                    if (blue > 255) blue = 255;
                    if (blue < 0) blue = 0;


                    var newColor = Color.FromArgb(oldColor.A, (int)red, (int)green, (int)blue);
                    bmp2.SetPixel(x, y, newColor);
                }
            }

            imgContrast = (Image)bmp2;
            return imgContrast;

        }

        Image Pixelate(object obj)
        {
            Image imgColor = (Image)obj;
            Image imgPixelated = null;
            Bitmap bmp1 = new Bitmap(imgColor);
            Bitmap bmp2 = new Bitmap(imgColor.Width, imgColor.Height);
            for (var yy = 0; yy < imgColor.Height && yy < imgColor.Height; yy += 4)
            {
                for (var xx = 0; xx < imgColor.Width && xx < imgColor.Width; xx += 4)
                {
                    var cellColors = new List<Color>();
                    // Store each color from the 4x4 cell into cellColors.
                    for (var y = yy; y < yy + 4 && y < imgColor.Height; y++)
                    {
                        for (var x = xx; x < xx + 4 && x < imgColor.Width; x++)
                        {
                            cellColors.Add(bmp1.GetPixel(x, y));
                        }
                    }

                    // Get the average red, green, and blue values.
                    var averageRed = cellColors.Aggregate(0, (current, color) => current + color.R) / cellColors.Count;
                    var averageGreen = cellColors.Aggregate(0, (current, color) => current + color.G) / cellColors.Count;
                    var averageBlue = cellColors.Aggregate(0, (current, color) => current + color.B) / cellColors.Count;
                    var averageColor = Color.FromArgb(averageRed, averageGreen, averageBlue);

                    // Go BACK over the 4x4 cell and set each pixel to the average color.
                    for (var y = yy; y < yy + 4 && y < imgColor.Height; y++)
                    {
                        for (var x = xx; x < xx + 4 && x < imgColor.Width; x++)
                        {
                            bmp2.SetPixel(x, y, averageColor);
                        }
                    }
                }
            }
            imgPixelated = (Image)bmp2;
            return imgPixelated;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Func<object, Image> func = new Func<object, Image>(convert2gray);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image);
            t.Start();
            t.ContinueWith((task) =>
            {
                pictureBox1.Image = task.Result;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Func<object, Image> func = new Func<object, Image>(flip);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image);
            t.Start();
            t.ContinueWith((task) =>
            {
                pictureBox1.Image = task.Result;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Func<object, Image> func = new Func<object, Image>(flipV);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image);
            t.Start();
            t.ContinueWith((task) =>
            {
                pictureBox1.Image = task.Result;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Func<object, Image> func = new Func<object, Image>(Invert);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image);
            t.Start();
            t.ContinueWith((task) =>
            {
                pictureBox1.Image = task.Result;

            }, TaskScheduler.FromCurrentSynchronizationContext());

        }
        private void button6_Click(object sender, EventArgs e)
        {  
            try
            {
                float val = float.Parse(textBox1.Text);
                Func<object, float, Image> func = new Func<object, float, Image>(Contrast);
                Task<Image> t = new Task<Image>(() => func(pictureBox1.Image, val));
                t.Start();
                t.ContinueWith((task) =>
                {
                    pictureBox1.Image = task.Result;

                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch(Exception ex)
            {
                MessageBox.Show("Please enter an value!");
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            Func<object, Image> func = new Func<object, Image>(Pixelate);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image);
            t.Start();
            t.ContinueWith((task) =>
            {
                pictureBox1.Image = task.Result;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }
    }
}
