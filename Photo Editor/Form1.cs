using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Photo_Editor
{
    public partial class Form1 : Form
    {
        int lastCol = 0;
        Image imgOriginal;
        Image Picture;
        Boolean openedPicture = false;
        Boolean mouseClicked;
        Point startPoint = new Point();
        Point endPoint = new Point();
        Rectangle rectCropArea;
        Bitmap targetBitmap;
        public CancellationTokenSource cTokenSource;
        public CancellationToken cToken;
        public Form1()
        {
            InitializeComponent();
            trackBar5.Minimum = (int)0.5f;
            trackBar5.Maximum = 7;
            trackBar5.Value = 1;
            trackBar6.Minimum = -20;
            trackBar6.Maximum = 20;
            trackBar6.Value = 0;
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
                    cToken.ThrowIfCancellationRequested();
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
            Color c1;
            var w = imgColor.Width - 1;
            for (int i = 0; i < imgColor.Width; i++)
            {
                var h = imgColor.Height - 1;
                for (int j = 0; j < imgColor.Height; j++)
                {
                    cToken.ThrowIfCancellationRequested();
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
            Color c1;
            var w = imgColor.Width - 1;
            for (int i = 0; i < imgColor.Width; i++)
            {               
                //var h = imgOriginal.Height - 1;
                for (int j = 0; j < imgColor.Height; j++)
                {
                    cToken.ThrowIfCancellationRequested();
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
            for (int y = 0; (y <= (imgColor.Height - 1)); y++)
            {
                for (int x = 0; (x <= (imgColor.Width - 1)); x++)
                {
                    cToken.ThrowIfCancellationRequested();
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
            picture_refresh();
            Image imgColor = (Image)obj;
            Image imgContrast = null;
            Image pic;
            lock (lockObject)
            {
                Image img = pictureBox1.Image;
                pic = (Image)img.Clone();
            }
            Bitmap bmp1 = new Bitmap(pic);
            Bitmap bmp2 = new Bitmap(pic.Width, pic.Height);

            var contrast = Math.Pow((100.0 + value) / 100.0, 2);
            for (int y = 0; y < pic.Height; y++)
            {
                
                for (int x = 0; x < pic.Width; x++)
                {
                    cToken.ThrowIfCancellationRequested();
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
            for (var yy = 0; yy < imgColor.Height && yy < imgColor.Height; yy += 20)
            {
                cToken.ThrowIfCancellationRequested();
                for (var xx = 0; xx < imgColor.Width && xx < imgColor.Width; xx += 20)
                {
                    var cellColors = new List<Color>();
                    // Store each color from the 4x4 cell into cellColors.
                    for (var y = yy; y < yy + 20 && y < imgColor.Height; y++)
                    {
                        for (var x = xx; x < xx + 20 && x < imgColor.Width; x++)
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
                    for (var y = yy; y < yy + 20 && y < imgColor.Height; y++)
                    {
                        for (var x = xx; x < xx + 20 && x < imgColor.Width; x++)
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
            cToken.ThrowIfCancellationRequested();

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
            for (int y = 0; y < imgColor.Height; y++)
            {
                cToken.ThrowIfCancellationRequested();
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
            Graphics gfx = Graphics.FromImage(bmp2);
            gfx.TranslateTransform((float)bmp2.Width / 2, (float)bmp2.Height / 2);
            gfx.RotateTransform(rotationAngle);
            cToken.ThrowIfCancellationRequested();
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
            Graphics gfx = Graphics.FromImage(bmp2);
            gfx.TranslateTransform((float)bmp2.Width / 2, (float)bmp2.Height / 2);
            gfx.RotateTransform(rotationAngle);
            cToken.ThrowIfCancellationRequested();
            gfx.TranslateTransform(-(float)bmp2.Width / 2, -(float)bmp2.Height / 2);
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.DrawImage(imgColor, new Point(0, 0));
            gfx.Dispose();
            imgRotateLeft = (Image)bmp2;
            return imgRotateLeft;
        }
        private void picOriginalPicture_MouseUp(object sender, MouseEventArgs e)
        {
            mouseClicked = false;

            if (endPoint.X != -1)
            {
                Point currentPoint = new Point(e.X, e.Y);
                // Display coordinates
                textBox4.Text = e.X.ToString();
                textBox5.Text = e.Y.ToString();

            }
            endPoint.X = -1;
            endPoint.Y = -1;
            startPoint.X = -1;
            startPoint.Y = -1;
        }

        private void picOriginalPicture_MouseDown(object sender, MouseEventArgs e)
        {
            mouseClicked = true;

            startPoint.X = e.X;
            startPoint.Y = e.Y;
            // Display coordinates
            textBox2.Text = startPoint.X.ToString();
            textBox3.Text = startPoint.Y.ToString();

            endPoint.X = -1;
            endPoint.Y = -1;

            rectCropArea = new Rectangle(new Point(e.X, e.Y), new Size());
        }

        private void picOriginalPicture_MouseMove(object sender, MouseEventArgs e)
        {
            Point ptCurrent = new Point(e.X, e.Y);

            if (mouseClicked)
            {
                if (endPoint.X != -1)
                {
                    // Display Coordinates
                    textBox2.Text = startPoint.X.ToString();
                    textBox3.Text = startPoint.Y.ToString();
                    textBox4.Text = e.X.ToString();
                    textBox5.Text = e.Y.ToString();
                }

                endPoint = ptCurrent;

                if (e.X > startPoint.X && e.Y > startPoint.Y)
                {
                    rectCropArea.Width = e.X - startPoint.X;
                    rectCropArea.Height = e.Y - startPoint.Y;
                }
                else if (e.X < startPoint.X && e.Y > startPoint.Y)
                {
                    rectCropArea.Width = startPoint.X - e.X;
                    rectCropArea.Height = e.Y - startPoint.Y;
                    rectCropArea.X = e.X;
                    rectCropArea.Y = startPoint.Y;
                }
                else if (e.X > startPoint.X && e.Y < startPoint.Y)
                {
                    rectCropArea.Width = e.X - startPoint.X;
                    rectCropArea.Height = startPoint.Y - e.Y;
                    rectCropArea.X = startPoint.X;
                    rectCropArea.Y = e.Y;
                }
                else
                {
                    rectCropArea.Width = startPoint.X - e.X;
                    rectCropArea.Height = startPoint.Y - e.Y;
                    rectCropArea.X = e.X;
                    rectCropArea.Y = e.Y;
                }
                pictureBox1.Refresh();
            }
        }
        private void picOriginalPicture_Paint(object sender, PaintEventArgs e)
        {
            Pen drawLine = new Pen(Color.Red);
            drawLine.DashStyle = DashStyle.Dash;
            e.Graphics.DrawRectangle(drawLine, rectCropArea);
        }
        Image crop ()
        {
            //pictureBox1.Refresh();
            Image imgCropped = null;
            targetBitmap = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height);
            Bitmap sourceBitmap = new Bitmap(pictureBox1.Image, pictureBox1.Width, pictureBox1.Height);
            cToken.ThrowIfCancellationRequested();
            Graphics g = Graphics.FromImage(targetBitmap);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            g.DrawImage(sourceBitmap, new Rectangle(0, 0, pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height), rectCropArea, GraphicsUnit.Pixel);
            if (pictureBox1.Image != null)
                pictureBox1.Image.Dispose();
            //pictureBox1.Image = targetBitmap;
            imgCropped = (Image)targetBitmap;
            return imgCropped;

        }
        private void reset()
        {
            pictureBox1.Image = imgOriginal;
        }
        Image Gamma (object obj, float gamma)
        {
            picture_refresh();
            Image pic;
            lock (lockObject)
            {
                Image img = pictureBox1.Image;
                pic = (Image)img.Clone();
            }
            double c = 1d;
            Image imgColor = (Image)obj;
            Image imgGamma = null;
            Bitmap bmp1 = new Bitmap(pic);
            Bitmap bmp2 = new Bitmap(pic.Width, pic.Height);
            int width = pic.Width;
            int height = pic.Height;
            BitmapData srcData = bmp1.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int bytes = srcData.Stride * srcData.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            Marshal.Copy(srcData.Scan0, buffer, 0, bytes);
            bmp1.UnlockBits(srcData);
            int current = 0;
            int cChannels = 3;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    cToken.ThrowIfCancellationRequested();
                    current = y * srcData.Stride + x * 4;
                    for (int i = 0; i < cChannels; i++)
                    {
                        double range = (double)buffer[current + i] / 255;
                        double correction = c * Math.Pow(range, gamma);
                        result[current + i] = (byte)(correction * 255);
                    }
                    result[current + 3] = 255;
                }
            }
            Bitmap resImg = new Bitmap(width, height);
            BitmapData resData = resImg.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, resData.Scan0, bytes);
            resImg.UnlockBits(resData);
            //return resImg;
            imgGamma = (Image)resImg;
            return imgGamma;
        }
        Image Resize(object obj, int newWidth, int newHeight)
        {
            Image imgColor = (Image)obj;
            Image imgResize = null;

            Bitmap temp = new Bitmap(imgColor);
            Bitmap bmap = new Bitmap(newWidth, newHeight, temp.PixelFormat);
            double nWidthFactor = temp.Width / (double)newWidth;
            double nHeightFactor = temp.Height / (double)newHeight;
            double fx, fy, nx, ny;
            int cx, cy, fr_x, fr_y;
            Color color1 = new Color();
            Color color2 = new Color();
            Color color3 = new Color();
            Color color4 = new Color();
            byte nRed, nGreen, nBlue;
            byte bp1, bp2;

            for (int x = 0; x < bmap.Width; ++x)
            {
                for (int y = 0; y < bmap.Height; ++y)
                {
                    cToken.ThrowIfCancellationRequested();

                    fr_x = (int)Math.Floor(x * nWidthFactor);
                    fr_y = (int)Math.Floor(y * nHeightFactor);
                    cx = fr_x + 1;
                    if (cx >= temp.Width) cx = fr_x;
                    cy = fr_y + 1;
                    if (cy >= temp.Height) cy = fr_y;
                    fx = x * nWidthFactor - fr_x;
                    fy = y * nHeightFactor - fr_y;
                    nx = 1.0 - fx;
                    ny = 1.0 - fy;

                    color1 = temp.GetPixel(fr_x, fr_y);
                    color2 = temp.GetPixel(cx, fr_y);
                    color3 = temp.GetPixel(fr_x, cy);
                    color4 = temp.GetPixel(cx, cy);

                    // Blue
                    bp1 = (byte)(nx * color1.B + fx * color2.B);

                    bp2 = (byte)(nx * color3.B + fx * color4.B);

                    nBlue = (byte)(ny * bp1 + fy * bp2);

                    // Green
                    bp1 = (byte)(nx * color1.G + fx * color2.G);

                    bp2 = (byte)(nx * color3.G + fx * color4.G);

                    nGreen = (byte)(ny * bp1 + fy * bp2);

                    // Red
                    bp1 = (byte)(nx * color1.R + fx * color2.R);

                    bp2 = (byte)(nx * color3.R + fx * color4.R);

                    nRed = (byte)(ny * bp1 + fy * bp2);

                    bmap.SetPixel(x, y, Color.FromArgb
                    (255, nRed, nGreen, nBlue));
                }
            }
            //_currentBitmap = (Bitmap)bmap.Clone();
            imgResize = (Image)bmap;
            return imgResize;

        }
        Image Blur(object obj)
        {
            Image ImgColor = (Image)obj;
            Image imgBlur = null;
            Bitmap bmap;
            Int32 blurSize = 10;
            Bitmap blurred = new Bitmap(ImgColor.Width, ImgColor.Height);
            Rectangle rectangle = new Rectangle(0, 0, ImgColor.Width, ImgColor.Height);
            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(ImgColor, new Rectangle(0, 0, ImgColor.Width, ImgColor.Height),
                    new Rectangle(0, 0, ImgColor.Width, ImgColor.Height), GraphicsUnit.Pixel);

            // Lock the bitmap's bits
            BitmapData blurredData = blurred.LockBits(new Rectangle(0, 0, ImgColor.Width, ImgColor.Height), ImageLockMode.ReadWrite, blurred.PixelFormat);
            
            // Get bits per pixel for current PixelFormat
            int bitsPerPixel = Image.GetPixelFormatSize(blurred.PixelFormat);
            unsafe
            {
                byte* scan0 = (byte*)blurredData.Scan0.ToPointer();

                // look at every pixel in the blur rectangle
                for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
                {
                    for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                    {
                        int avgR = 0, avgG = 0, avgB = 0;
                        int blurPixelCount = 0;

                        // average the color of the red, green and blue for each pixel in the
                        // blur size while making sure you don't go outside the image bounds
                        for (int x = xx; (x < xx + blurSize && x < ImgColor.Width); x++)
                        {
                            for (int y = yy; (y < yy + blurSize && y < ImgColor.Height); y++)
                            {
                                // Get pointer to RGB
                                byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                                avgB += data[0]; // Blue
                                avgG += data[1]; // Green
                                avgR += data[2]; // Red

                                blurPixelCount++;
                            }
                        }

                        avgR = avgR / blurPixelCount;
                        avgG = avgG / blurPixelCount;
                        avgB = avgB / blurPixelCount;

                        // now that we know the average for the blur size, set each pixel to that color
                        for (int x = xx; x < xx + blurSize && x < ImgColor.Width && x < rectangle.Width; x++)
                        {
                            for (int y = yy; y < yy + blurSize && y < ImgColor.Height && y < rectangle.Height; y++)
                            {
                                // Get pointer to RGB
                                byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                                // Change values
                                data[0] = (byte)avgB;
                                data[1] = (byte)avgG;
                                data[2] = (byte)avgR;
                            }
                        }
                    }
                }

                // Unlock the bits
                blurred.UnlockBits(blurredData);

                //return blurred;
                imgBlur = (Image)blurred;
                return imgBlur;
            }            
            //return blurred;
          
        }
        Image Emboss(object obj)
        {
            Image imgColor = (Image)obj;
            Image imgEmboss = null;
            Bitmap bmp1 = new Bitmap(imgColor);
            Bitmap nB = new Bitmap(imgColor.Width, imgColor.Height);

            for (int x = 1; x <= imgColor.Width - 1; x++)
            {
                for (int y = 1; y <= imgColor.Height - 1; y++)
                {
                    nB.SetPixel(x, y, Color.DarkGray);
                }
            }

            for (int x = 1; x <= imgColor.Width - 1; x++)
            {
                for (int y = 1; y <= imgColor.Height - 1; y++)
                {
                    try
                    {
                        Color pixel = bmp1.GetPixel(x, y);

                        int colVal = (pixel.R + pixel.G + pixel.B);

                        if (lastCol == 0) lastCol = (pixel.R + pixel.G + pixel.B);

                        int diff;

                        if (colVal > lastCol) { diff = colVal - lastCol; } else { diff = lastCol - colVal; }

                        if (diff > 100)
                        {
                            nB.SetPixel(x, y, Color.Gray);
                            lastCol = colVal;
                        }


                    }
                    catch (Exception) { }
                }
            }

            for (int y = 1; y <= imgColor.Height - 1; y++)
            {

                for (int x = 1; x <= imgColor.Width - 1; x++)
                {
                    try
                    {
                        Color pixel = bmp1.GetPixel(x, y);

                        int colVal = (pixel.R + pixel.G + pixel.B);

                        if (lastCol == 0) lastCol = (pixel.R + pixel.G + pixel.B);

                        int diff;

                        if (colVal > lastCol) { diff = colVal - lastCol; } else { diff = lastCol - colVal; }

                        if (diff > 100)
                        {
                            nB.SetPixel(x, y, Color.Gray);
                            lastCol = colVal;
                        }

                    }
                    catch (Exception) { }
                }
                
            }
            imgEmboss = (Image)nB;
            return imgEmboss;
            //pictureBox1.Image = nB;

        }
        /******************************************************************************************************/
        /*                Buttons Below                                                                       */
        /******************************************************************************************************/
        private void button2_Click(object sender, EventArgs e)
        {
            cTokenSource = new CancellationTokenSource();
            cToken = cTokenSource.Token;
            Func<object, Image> func = new Func<object, Image>(convert2gray);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image, cToken);
            t.Start();
            t.ContinueWith((task) =>
            {              
                try
                {
                    if (task.IsFaulted)
                    {
                        throw t.Exception;
                    }
                    if (task.IsCompleted)
                    {
                        pictureBox1.Image = task.Result;
                        Picture = pictureBox1.Image;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Please Upload an Image!");
                }
            }, CancellationToken.None,
            TaskContinuationOptions.NotOnCanceled, 
            TaskScheduler.FromCurrentSynchronizationContext());

            t.ContinueWith((task) =>
            {
                MessageBox.Show("Task was Cancelled");
                t.Dispose();
            }, CancellationToken.None,
            TaskContinuationOptions.OnlyOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cTokenSource = new CancellationTokenSource();
            cToken = cTokenSource.Token;
            Func<object, Image> func = new Func<object, Image>(flip);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image, cToken);
            t.Start();
            t.ContinueWith((task) =>
            {                
                try
                {
                    if (task.IsFaulted)
                    {
                        throw t.Exception;
                    }
                    if (task.IsCompleted)
                    {
                        pictureBox1.Image = task.Result;
                        Picture = pictureBox1.Image;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Please Upload an Image!");
                }

            }, CancellationToken.None,
            TaskContinuationOptions.NotOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());
            t.ContinueWith((task) =>
            {
                MessageBox.Show("Task was Cancelled");
            }, CancellationToken.None,
            TaskContinuationOptions.OnlyOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cTokenSource = new CancellationTokenSource();
            cToken = cTokenSource.Token;
            Func<object, Image> func = new Func<object, Image>(flipV);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image, cToken);
            t.Start();
            t.ContinueWith((task) =>
            {
                try
                {
                    if (task.IsFaulted)
                    {
                        throw t.Exception;
                    }
                    if (task.IsCompleted)
                    {
                        pictureBox1.Image = task.Result;
                        Picture = pictureBox1.Image;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Please Upload an Image!");
                }

            }, CancellationToken.None,
            TaskContinuationOptions.NotOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());

            t.ContinueWith((task) =>
            {
                MessageBox.Show("Task was Cancelled");
            }, CancellationToken.None,
            TaskContinuationOptions.OnlyOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            cTokenSource = new CancellationTokenSource();
            cToken = cTokenSource.Token;
            Func<object, Image> func = new Func<object, Image>(Invert);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image, cToken);
            t.Start();
            t.ContinueWith((task) =>
            {
                try
                {
                    if (task.IsFaulted)
                    {
                        throw t.Exception;
                    }
                    if (task.IsCompleted)
                    {
                        pictureBox1.Image = task.Result;
                        Picture = pictureBox1.Image;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Please Upload an Image!");
                }

            }, CancellationToken.None,
            TaskContinuationOptions.NotOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());

            t.ContinueWith((task) =>
            {
                MessageBox.Show("Task was Cancelled");
            }, CancellationToken.None,
            TaskContinuationOptions.OnlyOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());

        }
        private void button6_Click(object sender, EventArgs e)
        {
            cTokenSource = new CancellationTokenSource();
            cToken = cTokenSource.Token;
            float val = 3;
            try
            {
               // float val = float.Parse(textBox1.Text);
                Func<object, float, Image> func = new Func<object, float, Image>(Contrast);
                Task<Image> t = new Task<Image>(() => func(pictureBox1.Image, val),cToken);
                t.Start();
                t.ContinueWith((task) =>
                {
                    try
                    {
                        if (task.IsFaulted)
                        {
                            throw t.Exception;
                        }
                        if (task.IsCompleted)
                        {
                            pictureBox1.Image = task.Result;
                            Picture = pictureBox1.Image;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Please Upload an Image!");
                    }

                }, CancellationToken.None,
            TaskContinuationOptions.NotOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());

                t.ContinueWith((task) =>
                {
                    MessageBox.Show("Task was Cancelled");
                }, CancellationToken.None,
            TaskContinuationOptions.OnlyOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please enter an value!");
            }

        }
        private void button8_Click(object sender, EventArgs e)
        {
            cTokenSource = new CancellationTokenSource();
            cToken = cTokenSource.Token;
            Func<object, Image> func = new Func<object, Image>(Pixelate);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image, cToken);
            t.Start();
            t.ContinueWith((task) =>
            {
                try
                {
                    if (task.IsFaulted)
                    {
                        throw t.Exception;
                    }
                    if (task.IsCompleted)
                    {
                        pictureBox1.Image = task.Result;
                        Picture = pictureBox1.Image;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Please Upload an Image!");
                }

            }, CancellationToken.None,
            TaskContinuationOptions.NotOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());

            t.ContinueWith((task) =>
            {
                MessageBox.Show("Task was Cancelled");
            }, CancellationToken.None,
            TaskContinuationOptions.OnlyOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());
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
            cTokenSource = new CancellationTokenSource();
            cToken = cTokenSource.Token;
            Func<object, Image> func = new Func<object, Image>(serpia);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image, cToken);
            t.Start();
            t.ContinueWith((task) =>
            {
                try
                {
                    if (task.IsFaulted)
                    {
                        throw t.Exception;
                    }
                    if (task.IsCompleted)
                    {
                        pictureBox1.Image = task.Result;
                        Picture = pictureBox1.Image;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Please Upload an Image!");
                }

            }, CancellationToken.None,
            TaskContinuationOptions.NotOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());

            t.ContinueWith((task) =>
            {
                MessageBox.Show("Task was Cancelled");
            }, CancellationToken.None,
            TaskContinuationOptions.OnlyOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button10_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            cTokenSource = new CancellationTokenSource();
            cToken = cTokenSource.Token;
            Func<object, Image> func = new Func<object, Image>(rotateNinety);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image, cToken);
            t.Start();
            t.ContinueWith((task) =>
            {
                try
                {
                    if (task.IsFaulted)
                    {
                        throw t.Exception;
                    }
                    if (task.IsCompleted)
                    {
                        pictureBox1.Image = task.Result;
                        Picture = pictureBox1.Image;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Please Upload an Image!");
                }

            }, CancellationToken.None,
            TaskContinuationOptions.NotOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());

            t.ContinueWith((task) =>
            {
                MessageBox.Show("Task was Cancelled");
            }, CancellationToken.None,
            TaskContinuationOptions.OnlyOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button12_Click(object sender, EventArgs e)
        {
            cTokenSource = new CancellationTokenSource();
            cToken = cTokenSource.Token;
            Func<object, Image> func = new Func<object, Image>(rotateLeft);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image, cToken);
            t.Start();
            t.ContinueWith((task) =>
            {
                try
                {
                    if (task.IsFaulted)
                    {
                        throw t.Exception;
                    }
                    if (task.IsCompleted)
                    {
                        pictureBox1.Image = task.Result;
                        Picture = pictureBox1.Image;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Please Upload an Image!");
                }

            }, CancellationToken.None,
            TaskContinuationOptions.NotOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());

            t.ContinueWith((task) =>
            {
                MessageBox.Show("Task was Cancelled");
            }, CancellationToken.None,
            TaskContinuationOptions.OnlyOnCanceled,
            TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void button13_Click(object sender, EventArgs e)
        {
            Func<Image> func = new Func< Image>(crop);
            Task<Image> t = new Task<Image>(func);
            t.Start();
            t.ContinueWith((task) =>
            {
                pictureBox1.Image = task.Result;
                Picture = pictureBox1.Image;

            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                float val = 3;
                if(val>5 || val<=0)
                {
                    MessageBox.Show("Please Enter a value between 0.9 - 4");
                }
                else
                {
                    Func<object, float, Image> func = new Func<object, float, Image>(Gamma);
                    Task<Image> t = new Task<Image>(() => func(pictureBox1.Image, val));
                    t.Start();
                    t.ContinueWith((task) =>
                    {
                        try
                        {
                            if (task.IsFaulted)
                            {
                                throw t.Exception;
                            }
                            if (task.IsCompleted)
                            {
                                pictureBox1.Image = task.Result;
                                Picture = pictureBox1.Image;
                            }
                        }catch (AggregateException ex)
                        {
                            MessageBox.Show("Please Upload an Image");
                        }
                           
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please enter an value!");
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            try
            {
                int val1 = int.Parse(textBox7.Text);
                int val2 = int.Parse(textBox8.Text);
                if(val1 <=0 && val2 <=0)
                {
                    MessageBox.Show("Values should be greater than 0");
                }
                else
                {
                    Func<object, int, int, Image> func = new Func<object, int, int, Image>(Resize);
                    Task<Image> t = new Task<Image>(() => func(pictureBox1.Image, val1, val2));
                    t.Start();
                    t.ContinueWith((task) =>
                    {
                        try
                        {
                            if (task.IsFaulted)
                            {
                                throw t.Exception;
                            }
                            if (task.IsCompleted)
                            {
                                pictureBox1.Image = task.Result;
                                Picture = pictureBox1.Image;
                            }
                        }catch(AggregateException ex)
                        {
                            MessageBox.Show("Please Upload an Image");
                        }
                        
                       
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }catch (Exception ex)
            {
                MessageBox.Show("Please Enter Both values");
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            cTokenSource.Cancel();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Func<object, Image> func = new Func<object, Image>(Blur);
            Task<Image> t = new Task<Image>(func, pictureBox1.Image);
            t.Start();
            t.ContinueWith((task) =>
            {
                pictureBox1.Image = task.Result;
                Picture = pictureBox1.Image;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            int val = trackBar5.Value;
            label28.Text = val.ToString();
            Func<object, float, Image> func = new Func<object, float, Image>(Gamma);
            Task<Image> t = new Task<Image>(() => func(pictureBox1.Image, val));
            t.Start();
            t.ContinueWith((task) =>
            {
                try
                {
                    if (task.IsFaulted)
                    {
                        throw t.Exception;
                    }
                    if (task.IsCompleted)
                    {
                        pictureBox1.Image = task.Result;
                        Picture = pictureBox1.Image;
                    }
                }
                catch (AggregateException ex)
                {
                    MessageBox.Show("Please Upload an Image");
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            int val = trackBar6.Value;
            label29.Text = val.ToString();
            Func<object, float, Image> func = new Func<object, float, Image>(Contrast);
            Task<Image> t = new Task<Image>(() => func(pictureBox1.Image, val), cToken);
            t.Start();
            t.ContinueWith((task) =>
            {
                try
                {
                    if (task.IsFaulted)
                    {
                        throw t.Exception;
                    }
                    if (task.IsCompleted)
                    {
                        pictureBox1.Image = task.Result;
                        Picture = pictureBox1.Image;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Please Upload an Image!");
                }

            }, CancellationToken.None,
                TaskContinuationOptions.NotOnCanceled,
                TaskScheduler.FromCurrentSynchronizationContext());

            t.ContinueWith((task) =>
            {
                MessageBox.Show("Task was Cancelled");
            }, CancellationToken.None,
                TaskContinuationOptions.OnlyOnCanceled,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            Func<object, Image> func = new Func<object, Image>(Emboss);
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
