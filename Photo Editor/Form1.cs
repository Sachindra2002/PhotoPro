using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Photo_Editor
{
    public partial class Form1 : Form
    {
        Image imgOriginal;
        Image Picture;
        Boolean openedPicture = false;
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
                openedPicture = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void SavePhoto()
        {
            if (openedPicture)
            {

                SaveFileDialog save_pic = new SaveFileDialog();
                save_pic.Filter = "Images|*.png;*.jpg*;.bmp*";
                ImageFormat format = ImageFormat.Png;


                if (save_pic.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string extension = Path.GetExtension(save_pic.FileName);

                    switch (extension)
                    {

                        case ".jpg":
                            format = ImageFormat.Jpeg;
                            break;

                        case ".png":
                            format = ImageFormat.Png;
                            break;
                    }
                    pictureBox1.Image.Save(save_pic.FileName, format);
                }
            }
            else
            {
                MessageBox.Show("Please upload an image!");
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
        public void picture_refresh()
        {
            pictureBox1.Image = imgOriginal;
        }
        private object lockObject = new object();
        Image ChangeRGB(float red, float green, float blue, float brightness)
        {
            picture_refresh();
            Image pic;
            lock (lockObject)
            {
                Image img = pictureBox1.Image;
                pic = (Image)img.Clone();
            }
            
            Bitmap bitmap = new Bitmap(pic.Width, pic.Height);
            ImageAttributes imageAttributes = new ImageAttributes();
            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
            {
                new float[]{1+red,0 ,0 ,0 , 0 ,},
                new float[]{0 ,1+green, 0, 0, 0,},
                new float[]{0, 0,1+blue, 0, 0 ,},
                new float[]{0, 0, 0, 1, 0},
                new float[]{brightness,brightness,brightness, 0, 1}
            });

            imageAttributes.SetColorMatrix(colorMatrix);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.DrawImage(pic, new Rectangle(0, 0, pic.Width, pic.Height), 0, 0, pic.Width, pic.Height, GraphicsUnit.Pixel, imageAttributes);
            graphics.Dispose();
            return bitmap;
        }
        Image serpia(object obj)
        {
            Image imgColor = (Image)obj;
            Image imgSerpia = null;
            Bitmap bmp1 = new Bitmap(imgColor);
            Bitmap bmp2 = new Bitmap(imgColor.Width, imgColor.Height);
            Color c1, c2;
            for (int y = 0; y < imgColor.Height; y++)
            {
                for (int x = 0; x < imgColor.Width; x++)
                {
                    Color color = bmp1.GetPixel(x, y);
                    //c1 = bmp1.GetPixel(i, j);
                    int a = color.A;
                    int r = color.B;
                    int g = color.R;
                    int b = color.B;
                    int tr = (int)(0.393 * r + 0.769 * g + 0.189 * b);
                    int tg = (int)(0.349 * r + 0.686 * g + 0.168 * b);
                    int tb = (int)(0.271 * r + 0.534 * g + 0.131 * b);
                    if (tr > 255)
                    {
                        r = 255;
                    }
                    else
                    {
                        r = tr;
                    }
                    if (tg > 255)
                    {

                        g = 255;
                    }
                    else
                    {
                        g = tg;
                    }
                    if (tb > 255)
                    {
                        b = 255;
                    }
                    else
                    {
                        b = tb;
                    }

                    bmp2.SetPixel(x, y, Color.FromArgb(a, r, g, b));

                }
            }
            imgSerpia = (Image)bmp2;
            return imgSerpia;

        }
        Image rotateNinety(object obj)
        {
            int rotationAngle = 90;
            Image imgColor = (Image)obj;
            Image imgRotateNinety = null;
            Bitmap bmp1 = new Bitmap(imgColor);
            Bitmap bmp2 = new Bitmap(imgColor.Width, imgColor.Height);
            Color c1, c2;
            Graphics gfx = Graphics.FromImage(bmp2);
            gfx.TranslateTransform((float)bmp2.Width / 2, (float)bmp2.Height / 2);
            gfx.RotateTransform(rotationAngle);
            gfx.TranslateTransform(-(float)bmp2.Width / 2, -(float)bmp2.Height / 2);
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.DrawImage(imgColor, new Point(0, 0));
            gfx.Dispose();
            imgRotateNinety = (Image)bmp2;
            return imgRotateNinety;
        }
        Image rotateLeft(object obj)
        {
            int rotationAngle = -90;
            Image imgColor = (Image)obj;
            Image imgRotateLeft = null;
            Bitmap bmp1 = new Bitmap(imgColor);
            Bitmap bmp2 = new Bitmap(imgColor.Width, imgColor.Height);
            Color c1, c2;
            Graphics gfx = Graphics.FromImage(bmp2);
            gfx.TranslateTransform((float)bmp2.Width / 2, (float)bmp2.Height / 2);
            gfx.RotateTransform(rotationAngle);
            gfx.TranslateTransform(-(float)bmp2.Width / 2, -(float)bmp2.Height / 2);
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.DrawImage(imgColor, new Point(0, 0));
            gfx.Dispose();
            imgRotateLeft = (Image)bmp2;
            return imgRotateLeft;
        }
        private void reset()
        {
            pictureBox1.Image = imgOriginal;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Func<object, Image> func = new Func<object, Image>(convert2gray);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image);
            t.Start();
            t.ContinueWith((task) =>
            {
                pictureBox1.Image = task.Result;
                Picture = pictureBox1.Image;

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
                Picture = pictureBox1.Image;

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
                Picture = pictureBox1.Image;

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
                Picture = pictureBox1.Image;

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
                    Picture = pictureBox1.Image;

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
                Picture = pictureBox1.Image;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            float red = trackBar1.Value * 0.1f;
            float green = trackBar2.Value * 0.1f;
            float blue = trackBar3.Value * 0.1f;
            float brightness = trackBar4.Value * 0.1f;

            label8.Text = red.ToString();
            label9.Text = green.ToString();
            label10.Text = blue.ToString();
            label12.Text = brightness.ToString();
            Func<float, float, float, float, Image> func = new Func<float, float, float, float, Image>(ChangeRGB);
            Task<Image> t = new Task<Image>(() => func(red, green, blue, brightness));
            t.Start();
            t.ContinueWith((task) =>
            {
                pictureBox1.Image = task.Result;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            float red = trackBar1.Value * 0.1f;
            float green = trackBar2.Value * 0.1f;
            float blue = trackBar3.Value * 0.1f;
            float brightness = trackBar4.Value * 0.1f;

            label8.Text = red.ToString();
            label9.Text = green.ToString();
            label10.Text = blue.ToString();
            label12.Text = brightness.ToString();
            Func<float, float, float, float, Image> func = new Func<float, float, float, float, Image>(ChangeRGB);
            Task<Image> t = new Task<Image>(() => func(red, green, blue, brightness));
            t.Start();
            t.ContinueWith((task) =>
            {
                pictureBox1.Image = task.Result;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            float red = trackBar1.Value * 0.1f;
            float green = trackBar2.Value * 0.1f;
            float blue = trackBar3.Value * 0.1f;
            float brightness = trackBar4.Value * 0.1f;

            label8.Text = red.ToString();
            label9.Text = green.ToString();
            label10.Text = blue.ToString();
            label12.Text = brightness.ToString();
            Func<float, float, float, float, Image> func = new Func<float, float, float, float, Image>(ChangeRGB);
            Task<Image> t = new Task<Image>(() => func(red, green, blue, brightness));
            t.Start();
            t.ContinueWith((task) =>
            {
                pictureBox1.Image = task.Result;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            float red = trackBar1.Value * 0.1f;
            float green = trackBar2.Value * 0.1f;
            float blue = trackBar3.Value * 0.1f;
            float brightness = trackBar4.Value * 0.1f;

            label8.Text = red.ToString();
            label9.Text = green.ToString();
            label10.Text = blue.ToString();
            label12.Text = brightness.ToString();
            Func<float, float, float, float, Image> func = new Func<float, float, float, float, Image>(ChangeRGB);
            Task<Image> t = new Task<Image>(() => func(red, green, blue, brightness));
            t.Start();
            t.ContinueWith((task) =>
            {
                pictureBox1.Image = task.Result;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            SavePhoto();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Func<object, Image> func = new Func<object, Image>(serpia);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image);
            t.Start();
            t.ContinueWith((task) =>
            {
                pictureBox1.Image = task.Result;
                Picture = pictureBox1.Image;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button10_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Func<object, Image> func = new Func<object, Image>(rotateNinety);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image);
            t.Start();
            t.ContinueWith((task) =>
            {
                pictureBox1.Image = task.Result;
                Picture = pictureBox1.Image;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Func<object, Image> func = new Func<object, Image>(rotateLeft);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image);
            t.Start();
            t.ContinueWith((task) =>
            {
                pictureBox1.Image = task.Result;
                Picture = pictureBox1.Image;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
