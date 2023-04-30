using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyCCTV
{
    public partial class Form1 : Form
    {
        // 영상
        CvCapture capture;              // 프레임캡쳐 
        IplImage src;                   // 원본 소스
        MyopencvClass dst = new MyopencvClass();
        MovingClass opticalflow = new MovingClass();
        // 시간
        System.DateTime timenow;        // 현재시간
        // 컨트롤
        double gamma_control = 1;     // 감마(밝기) 조절필드
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 카메라 정상작동 텍스트(더미)
            tboxCamOK.Text = "cam[1]" + " : ok.." + "\r\ncam[2]" + " : ok..";
            try
            {
                capture = CvCapture.FromCamera(CaptureDevice.DShow, 0);
                capture.SetCaptureProperty(CaptureProperty.FrameWidth, 640);
                capture.SetCaptureProperty(CaptureProperty.FrameHeight, 480);
            }
            catch
            {
                timer1.Enabled= false;
            }
        }
        IplImage prev = new IplImage(new CvSize(640, 480), BitDepth.U8, 3);
        IplImage curr = new IplImage(new CvSize(640, 480), BitDepth.U8, 3);
        bool start = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            // 현재시각
            timenow = DateTime.Now;
            lblTime.Text = "현재시각 : " + timenow.ToString();
            // 카메라 캡쳐
            if(start==true) prev = src.Clone();
            src = capture.QueryFrame();
            curr = src.Clone();
            IplImage flowimg = new IplImage();
            flowimg= opticalflow.OpticalFlowLK(prev, curr);

            // 영상출력
            if (src != null)
            {
                if(checkBox1.Checked!=true)
                {
                    pictureBoxIpl1.ImageIpl = dst.RoiImage(dst.GammaCorrect(dst.PutText(src), gamma_control));
                }
                pictureBoxIpl2.ImageIpl = dst.RoiImage(dst.GammaCorrect(dst.PutText(src),gamma_control));
            }
            else
            {
                timer1.Enabled = false;
            }
            // smart mode

            if (start == true)
            {
                if(checkBox1.Checked)
                {
                    pictureBoxIpl1.ImageIpl = dst.PutText(flowimg);
                }
                pictureBoxIpl4.ImageIpl = flowimg;
            }
            tttttt.Text = opticalflow.moving_detected().ToString();
            start = true;
            dst.Dispose();
        }

        private void btnGUp_Click(object sender, EventArgs e)
        {
            gamma_control += 0.2;
        }

        private void btnGDown_Click(object sender, EventArgs e)
        {
            gamma_control -= 0.2;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            gamma_control = 1;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            dst.SetRoi(1);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            dst.SetRectZero();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dst.SetYMinus();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dst.SetXMinus();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            dst.SetXPlus();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            dst.SetYPlus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            string save_name = DateTime.Now.ToString("yyyy_MM_dd_hh시mm분ss초");
            Cv.SaveImage(tboxFilePath.Text + "\\" + save_name + ".jpg", dst.PutText(dst.GammaCorrect(src, gamma_control)));
            tboxLog.Text += DateTime.Now.ToString("g") + "_캡쳐되었습니다.\r\n";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            dst.SetCenter();
        }

        bool onoff = true;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(onoff==false)
            {
                tboxLog.Text += DateTime.Now.ToString("MM-d-HH:mm:ss ") + "AI보안모드 OFF\r\n";
                onoff = true;
            }
            else
            {
                tboxLog.Text += DateTime.Now.ToString("MM-d-HH:mm:ss ") + "AI보안모드 ON\r\n";
                onoff = false;
            }
            if(checkBox1.Checked)
            {
                checkBox1.Text = "      ON";
                checkBox1.BackColor = Color.SlateBlue;
            }
            else
            {
                checkBox1.Text = "      OFF";
                checkBox1.BackColor= Color.White;
            }
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                tboxFilePath.Text = fd.SelectedPath;
            }
        }
    }
}
