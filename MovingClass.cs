using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCCTV
{
    class MovingClass : IDisposable
    {
        IplImage gray;
        public IplImage optical;
        bool detected = false;
        public IplImage GrayScale(IplImage src)
        {
            gray = new IplImage(src.Size, BitDepth.U8, 1);
            Cv.CvtColor(src, gray, ColorConversion.BgrToGray);
            return gray;
        }
        public IplImage OpticalFlowLK(IplImage previous, IplImage current)
        {
            IplImage prev = this.GrayScale(previous);
            IplImage currn = this.GrayScale(current);
            optical = current;

            int rows = optical.Height;
            int cols=optical.Width;

            CvMat velx = Cv.CreateMat(rows, cols, MatrixType.F32C1);
            CvMat vely = Cv.CreateMat(rows, cols, MatrixType.F32C1);

            velx.SetZero();
            vely.SetZero();

            Cv.CalcOpticalFlowLK(prev,currn,new CvSize(15,15),velx, vely);
            for(int i=0;i<cols;i+=15)
            {
                detected = false;
                for (int j=0; j<rows;j+=15)
                {
                    int dx = (int)Cv.GetReal2D(velx, j, i);
                    int dy = (int)Cv.GetReal2D(vely, j, i);

                    Cv.DrawCircle(optical, i, j, 1, CvColor.Red);
                    if(Math.Abs(dx)<30&&Math.Abs(dy)<30)
                    {
                        if (Math.Abs(dx) < 10 && Math.Abs(dy) < 10) continue;
                        detected = true;
                        Cv.DrawLine(optical, new CvPoint(i, j), new CvPoint(i + dx, j + dy), CvColor.Blue, 1, LineType.AntiAlias);
                        Cv.DrawCircle(optical, new CvPoint(i + dx, j + dy), 3, CvColor.Blue, -1);
                    }
                }
            }
            return optical;
        }
        public bool moving_detected()
        {
            return detected;
        }

        public void Dispose() { }
    }
}
