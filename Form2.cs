using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoSlide
{
    public partial class Form2 : Form
    {
        string _currentPath = String.Empty;
        public Form1 Parent { get; set; }

        public Form2()
        {
            InitializeComponent();
        }

        public void SetImage(string path)
        {
            _currentPath = path;
            ResizeImage();
        }

        private void ResizeImage()
        {
            if (String.IsNullOrEmpty(_currentPath)) {
                return;
            }
            if (this.pictureBox1.Image != null)
            {
                this.pictureBox1.Image.Dispose();
                this.pictureBox1.Image = null;
                GC.Collect();
            }

            
            int width = this.pictureBox1.Width;
            int height = this.pictureBox1.Height;
            using (var image = Image.FromFile(_currentPath))
            {
                image.ExifRotate();
                var iW = image.Width;
                var iH = image.Height;
                double factor = 0;
                if ((double)iW / iH > Convert.ToDouble(width) / height)
                {
                    factor = Convert.ToDouble(width) / iW;
                }
                else
                {
                    factor = Convert.ToDouble(height) / iH;
                }
                factor = Math.Min(factor, 1D);
                var tgtWidth = Convert.ToInt32(factor * iW);
                var tgtHeight = Convert.ToInt32(factor * iH);
                var bgRect = new Rectangle(0, 0, width, height);
                var destImage = new Bitmap(width, height);

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.FillRectangle(Brushes.Black, bgRect);
                        var destRect = new Rectangle(Convert.ToInt32(Convert.ToDouble(width - tgtWidth) / 2), Convert.ToInt32(Convert.ToDouble(height - tgtHeight) / 2), tgtWidth, tgtHeight);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }

                this.pictureBox1.Image = destImage;
            }
            this.label1.Text = _currentPath;
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            ResizeImage();
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Parent.StopTimer();
                this.Close();
            }
            else if (e.KeyCode == Keys.Space)
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                    this.WindowState = FormWindowState.Normal;
                } else
                {
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.WindowState = FormWindowState.Maximized;
                }
            }
        }
    }
}
