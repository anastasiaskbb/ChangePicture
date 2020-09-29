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
            public byte Alpha;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog files = new OpenFileDialog();
            files.Filter = "Image Files(*.JPG)| *.JPG;| All files(*.*) | *.* ";
            if (files.ShowDialog() == DialogResult.OK)
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
            if (pictureBox1.Image != null)
            {
                SaveFileDialog files = new SaveFileDialog();
                files.Filter = "Image Files(*.JPG)| *.JPG;| All files(*.*) | *.* ";

                if (files.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        //Получаем кодировщик
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                        //Создаем параметры компрессии
                        EncoderParameters myEncoderParameters = new EncoderParameters(1);
                        var myEncoderParameter = new EncoderParameter(myEncoder, 100L); //тут задаем уровень компрессии
                        myEncoderParameters.Param[0] = myEncoderParameter;
                        //Сохраняем передавая в функцию кодировщик и его параметры
                        pictureBox1.Image.Save(files.FileName, jpgEncoder, myEncoderParameters);
                    }
                    catch
                    {
                        MessageBox.Show("Invalid file format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            Bitmap Image = new Bitmap(pictureBox1.Image);
            unsafe
            {
                //Блокируем изображение
                var oneBits = Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height), ImageLockMode.ReadWrite, Image.PixelFormat);

                //Параллельным For циклом перебираем указатели
                Parallel.For(0, Image.Width * Image.Height, i => {
                    //Получаем указатель на текущий пиксель
                    Pixel* pxOne = (Pixel*)((byte*)oneBits.Scan0 + i * sizeof(Pixel));

                    //Меняем потоки
                    var tempRed = pxOne->Red;
                    pxOne->Red = pxOne->Green;
                    pxOne->Green = pxOne->Blue;
                    pxOne->Blue = tempRed;


                });

                //Разблокируем изображение
                Image.UnlockBits(oneBits);
                //Устанавливаем изображение в pictureBox
                pictureBox1.Image = Image;

            }
        }

        //Функция для получения кодировщика
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

    }
}
