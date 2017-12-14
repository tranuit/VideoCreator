using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YoutubeNews
{
    public partial class Form1 : Form
    {
        private string strImagePath = "image.jpg";
        //private string strCreateCommand;
        //private string strCurentPath;
        public Form1()
        {
            InitializeComponent();
        }
        private void txtImage_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if(this.txtImage.Text != "")
            {
                try
                {
                    saveImage();
                    saveTextToFile();
                    createVideo();
                    insertTextToVideo();
                    MessageBox.Show("Complete Create video!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }     
            }
            else
            {
                MessageBox.Show("Please input url image!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void saveImage()
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(this.txtImage.Text), strImagePath);
            }
        }

        private void saveTextToFile()
        {
            using (var sw = new StreamWriter(File.Open("input.txt", FileMode.Create), Encoding.GetEncoding("UTF-8")))
            {
                for (int i = 0; i < txtText.Text.Length; i++)
                {
                    if( i % 14 == 0)
                    {
                        if(txtText.Text.Length > (i + 14) )
                        {
                            sw.WriteLine(txtText.Text.Substring(i , 14));
                        }
                        else
                        {
                            sw.WriteLine(txtText.Text.Substring(i, txtText.Text.Length - i));
                        }
                    }
                }
            }
        }
        private void createVideo()
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "ffmpeg.exe";
            // proc.StartInfo.Arguments = "-loop 1 -y -i image.jpg  -vf \"scale = trunc(iw / 2) * 2:trunc(ih / 2) * 2\" -c:v libx264 -t 30 -pix_fmt yuv420p out.mp4";
            proc.StartInfo.Arguments = "-loop 1 -y -i image.jpg  -vf \"scale = 640:480\" -c:v libx264 -t 180 -pix_fmt yuv420p out.mp4";
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            if (!proc.Start())
            {
                MessageBox.Show("Error starting");
                return;
            }
            StreamReader reader = proc.StandardError;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
            proc.Close();
        }
        private void insertTextToVideo()
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "ffmpeg.exe";
            proc.StartInfo.Arguments = "-y -i out.mp4 -vf \"[in]drawtext = textfile = 'input.txt':fontfile = ARIALUNI.TTF:x = 18:y = h - 30 * t:fontcolor = ffffff:fontsize = 36:shadowx = 2:shadowy = 2[out]\"  final.mp4";
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            if (!proc.Start())
            {
                MessageBox.Show("Error starting");
                return;
            }
            StreamReader reader = proc.StandardError;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
            proc.Close();
        }
    }
}
