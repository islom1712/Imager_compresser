using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tasvirlarni_siqish_algoritmi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string inputImagePath = null;
        string outputImagePath = "output.jpg";
        string decomp = "output2.jpg";
        float firstsize = 0;
        float lastsize = 0;


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf=new OpenFileDialog();
            opf.Filter = "image files|*.jpg|png file|*.png";
            if(opf.ShowDialog()==DialogResult.OK )
            {
                inputImagePath= opf.FileName;
                pictureBox1.Image=Image.FromFile(inputImagePath);
                FileInfo fileInfo = new FileInfo(inputImagePath);
                label1.Text ="Ochilgan fayl hajmi:"+ Math.Round(fileInfo.Length / 1024.0,2).ToString() + " kb"; ;
                label1.Visible = true;
                firstsize= fileInfo.Length;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(inputImagePath!=null)
            {
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();

                int compressionLevel = 100 - trackBar1.Value; // Specify the compression level (0-100)

                //  ReduceImageSize2(inputImagePath, outputImagePath);
                if (radio1.Checked)
                {
                    ReduceImageMemorySize(inputImagePath, outputImagePath, compressionLevel);
                    pictureBox1.Image = Image.FromFile(inputImagePath);
                    pictureBox2.Image = Image.FromFile(outputImagePath);
                }
                else
                {

                    CompressImage(inputImagePath, outputImagePath);
                    DecompressImage(outputImagePath, decomp);
                    pictureBox1.Image = Image.FromFile(inputImagePath);
                    pictureBox2.Image = Image.FromFile(decomp);

                }


                FileInfo fileInfo = new FileInfo(outputImagePath);
                label2.Visible = true;
                label2.Text = "Siqilgan Fayl hajmi: " + Math.Round(fileInfo.Length / 1024.0, 2).ToString() + " kb";
                lastsize = fileInfo.Length;
                double foiz = Math.Round(100 - (lastsize / firstsize * 100.0), 2);
                label3.Visible = true;
                label3.Text = foiz.ToString() + "% Fayl hajmi qisqardi";
            }
            



        }
        void ReduceImageMemorySize(string inputImagePath, string outputImagePath, int compressionLevel)
        {
            using (Image image = Image.FromFile(inputImagePath))
            {
                // Create an EncoderParameters object to set the compression level
                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, compressionLevel);

                GC.Collect();
                GC.WaitForPendingFinalizers();
                // Get the JPEG codec
                ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

                // Save the image with the specified compression level
                image.Save(outputImagePath, jpegCodec, encoderParams);
               

            }
        }
        static void ReduceImageSize2(string inputImagePath, string outputImagePath)
        {
            using (Image image = Image.FromFile(inputImagePath))
            {
                // Create an EncoderParameters object to specify compression quality
                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (long)EncoderValue.CompressionNone);

                // Get the PNG codec
                ImageCodecInfo pngCodec = GetEncoderInfo(ImageFormat.Png);

                // Save the image in PNG format
                image.Save(outputImagePath, pngCodec, encoderParams);
            }
        }

        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
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

        ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType == mimeType)
                {
                    return codec;
                }
            }
            return null;
        }

        private void savefilebtn_Click(object sender, EventArgs e)
        {
            if(outputImagePath!= null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Jpg file|*.jpg";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if(radio1.Checked)
                        File.Copy(outputImagePath, saveFileDialog.FileName);
                    else
                        File.Copy(decomp, saveFileDialog.FileName);
                }
            }
            
        }

        public void CompressImage(string sourceFilePath, string compressedFilePath)
        {
            // Read the source image into a byte array
            byte[] sourceBytes = File.ReadAllBytes(sourceFilePath);

            // Create a compressed stream to write the compressed data
            using (FileStream compressedFileStream = File.Create(compressedFilePath))
            {
                // Create a DeflateStream to perform compression
                using (DeflateStream deflateStream = new DeflateStream(compressedFileStream, CompressionMode.Compress))
                {
                    // Write the source image data to the DeflateStream
                    deflateStream.Write(sourceBytes, 0, sourceBytes.Length);
                }
            }
             
        }
        public static void DecompressImage(string compressedFilePath, string decompressedFilePath)
        {
            // Read the compressed data from the file into a byte array
            byte[] compressedBytes = File.ReadAllBytes(compressedFilePath);

            // Create a decompressed stream to write the decompressed data
            using (FileStream decompressedFileStream = File.Create(decompressedFilePath))
            {
                // Create a DeflateStream to perform decompression
                using (DeflateStream deflateStream = new DeflateStream(new MemoryStream(compressedBytes), CompressionMode.Decompress))
                {
                    // Copy the decompressed data from the DeflateStream to the decompressedFileStream
                    deflateStream.CopyTo(decompressedFileStream);
                }
            }

            Console.WriteLine("Decompression completed successfully.");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GC.Collect();   
            GC.WaitForPendingFinalizers();  
            pictureBox1.Image = Image.FromFile(inputImagePath);
            pictureBox2.Image = Image.FromFile(decomp);
        }
    }
}
