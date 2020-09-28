using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab1MT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        struct Pixel
        {
            public byte Blue;
            public byte Green;
            public byte Red;
        }

            private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog files = new OpenFileDialog();
            files.Filter = "Image Files(*.BMP; *.JPG; *.GIF; *.PNG)| *.BMP; *.JPG; *.GIF; *.PNG | All files(*.*) | *.* ";
            if(files.ShowDialog()==DialogResult.OK)
            {
                try 
                {
                    pictureBox1.Image = new Bitmap(files.FileName);
                }
                catch
                {
                    MessageBox.Show("Invalid file format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image!=null)
            {
                SaveFileDialog files = new SaveFileDialog();
                files.Filter = "Image Files(*.BMP; *.JPG; *.GIF; *.PNG)| *.BMP; *.JPG; *.GIF; *.PNG | All files(*.*) | *.* ";

                if(files.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        pictureBox1.Image.Save(files.FileName);
                    }
                    catch
                    {
                        MessageBox.Show("Invalid file format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap Image = new Bitmap(pictureBox1.Image);
            Bitmap ResultImage = new Bitmap(Image.Width, Image.Height);
            unsafe
            {
                var oneBits = Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height), ImageLockMode.ReadOnly, Image.PixelFormat);
                var twoBits = ResultImage.LockBits(new Rectangle(0, 0, ResultImage.Width, ResultImage.Height), ImageLockMode.WriteOnly, ResultImage.PixelFormat);
                int padding = twoBits.Stride - (ResultImage.Width * sizeof(Pixel));
                int width = ResultImage.Width;
                int height = ResultImage.Height;
                Parallel.For(0, Image.Width * Image.Height, i =>
                {
                    Pixel* pxOne = (Pixel*)((byte*)oneBits.Scan0 + i * sizeof(Pixel));
                    byte* ptr = (byte*)twoBits.Scan0;
                    for (int j = 0; j < height; j++)
                    {
                        for(int k = 0; k < width; k++) 
                        {
                            
                            Pixel* pxTwo = (Pixel*)((byte*)twoBits.Scan0 + i * sizeof(Pixel));
                            pxOne->Red = pxTwo->Green;
                            pxOne->Green = pxTwo->Blue;
                            pxOne->Blue = pxTwo->Red;
                            ptr += sizeof(Pixel);
                        }
                        ptr += padding;
                    }
                });
                Image.UnlockBits(oneBits);
                ResultImage.UnlockBits(twoBits);
                pictureBox1.Image = ResultImage;

            }
        }
    }
}
