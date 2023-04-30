using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Windows.Forms;
using OpenCvSharp.CPlusPlus;
using System.Diagnostics;
using System.Drawing;

namespace MyCCTV
{
    class MyopencvClass : IDisposable
    {
        IplImage src;
        IplImage puttext;
        IplImage gamma;
        IplImage roi;
        Rect rect;

        public IplImage PutText(IplImage src)
        {
            puttext = new IplImage(src.Size, BitDepth.U8, 3);
            Cv.Copy(src, puttext);
            Cv.PutText(puttext, "CAM[1]", new CvPoint(550, 40), new CvFont(FontFace.HersheyComplexSmall, 1, 1), CvColor.Yellow);
            return puttext;
        }

        public IplImage GammaCorrect(IplImage src,double gam_level)
        {
            gamma = new IplImage(src.Size, BitDepth.U8, 3);
            double gamma_value = gam_level;
            byte[] lut = new byte[256];
            for (int i = 0; i < lut.Length; i++)
            {
                lut[i] = (byte)(Math.Pow(i / 255.0, 1.0 / gamma_value) * 255.0);
            }
            Cv.LUT(src, gamma, lut);
            return gamma;
        }

        // 줌을 위한 이미지 크기 인자들
        int roi_w = 640;
        int roi_h = 480;
        int x = 0;
        int y = 0;

        // 줌 이미지 생성, 뿌리기
        public IplImage RoiImage(IplImage src)
        {
            roi = new IplImage(src.Size, BitDepth.U8, 3);
            rect = new Rect(x, y, roi_w, roi_h);
            roi = src.Clone(rect);
            return roi;
        }
        // 줌 당기기
        public void SetRoi(int ctrl)
        {
            if (ctrl == 1)
            {
                roi_w = (roi_w * 9) / 10;
                roi_h = (roi_h * 9) / 10;
            }
            else
            {
                roi_w = 640; roi_h = 480;
            }
        }

        // 줌리모컨 가운데 클릭시 가운데로 줌당기게 하는 함수
        public void SetCenter()
        {
            x = 200;
            y = 140;
            roi_w = (640 * 5) / 10;
            roi_h = (480 * 5) / 10;
        }
        public void SetXPlus()
        {
            if (x +roi_w < 630)
                x += 15;
        }
        public void SetXMinus()
        {
            if (x > 0)
                x -= 15;
        }
        public void SetYPlus()
        {
            if (y+roi_h < 470)
                y += 15;
        }
        public void SetYMinus()
        {
            if (y > 0)
                y -= 15;
        }
        public void SetRectZero()
        {
            x = 0; y = 0;
            roi_w = 640;
            roi_h = 480;
        }
        public void Dispose()
        {
            if (src != null) Cv.ReleaseImage(src);
            if (puttext != null) Cv.ReleaseImage(puttext);
            if (gamma != null) Cv.ReleaseImage(gamma);
            if (roi != null) Cv.ReleaseImage(roi);
        }
    }
}
