using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace read_write_image
{
    public partial class readImageForm : Form
    {
        public static Bitmap originalBitmap = null;
        private static Bitmap previewBitmap = null;
        public static Bitmap resultBitmap = null;
        public static double[,] I_enhance;

        public static double[,] redMatrix = null;
        public static double[,] greenMatrix = null;
        public static double[,] blueMatrix = null;

        public readImageForm()
        {
            InitializeComponent();
        }



        private void readImageForm_Load(object sender, EventArgs e)
        {
            //Bitmap bmp = new Bitmap("E:\\Bai giang\\Luan van\\test-img\\Image01.jpg");
            //pictureBox1.Image = bmp;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ;

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            //originalBitmap = null;
            //pictureBox1.Image = null;

            lblHeightIndex.Text = "";
            lblWidthIndex.Text = "";

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Chon hinh anh";
            ofd.Filter = "jpg Images|*.jpg";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //StreamReader str = new StreamReader(ofd.FileName);
                originalBitmap = new Bitmap(ofd.FileName);
                previewBitmap = originalBitmap;
                pictureBox1.Image = previewBitmap;
            }

            lblPath.Text = ofd.FileName.ToString();

        }

        private void btnGray_Click(object sender, EventArgs e)
        {
            if (originalBitmap == null)
            {
                MessageBox.Show("Chưa chọn anh!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {

                lblHeightIndex.Text = previewBitmap.Height.ToString();
                lblWidthIndex.Text = previewBitmap.Width.ToString();

                previewBitmap = originalBitmap;
                resultBitmap = null;

                // edgeBitmap = new Bitmap(previewBitmap.Width, previewBitmap.Height);
                int width = previewBitmap.Width;
                int height = previewBitmap.Height;

                //double test = Measure.sharpness(r_test, 3, 3);

                double[,] alpha = new double[width, height];
                double[,] blue = new double[width, height];
                double[,] red = new double[width, height];
                double[,] green = new double[width, height];

                Color p;
                //khai bao RGB
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        p = previewBitmap.GetPixel(x, y);
                        int a = p.A;
                        int b = p.B;
                        int r = p.R;
                        int g = p.G;

                        alpha[x, y] = Convert.ToDouble(a);
                        blue[x, y] = b;
                        red[x, y] = r;
                        green[x, y] = g;
                    }
                }

                //chuyen tu thang 255 ve thang 0-1

                double[,] red01 = new double[width, height];
                double[,] green01 = new double[width, height];
                double[,] blue01 = new double[width, height];

                double minRed = extBitmap.min2D(red, width, height);
                double minBlue = extBitmap.min2D(blue, width, height);
                double minGreen = extBitmap.min2D(green, width, height);

                double maxRed = extBitmap.max2D(red, width, height);
                double maxBlue = extBitmap.max2D(blue, width, height);
                double maxGreen = extBitmap.max2D(green, width, height);

                /* double testLEnght0 = red01.GetLength(0);
                 double testLEnght1 = red01.GetLength(1);*/

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        red01[x, y] = 1.0 * (red[x, y] - minRed) / (maxRed - minRed);
                        green01[x, y] = 1.0 * (green[x, y] - minGreen) / (maxGreen - minGreen);
                        blue01[x, y] = 1.0 * (blue[x, y] - minBlue) / (maxBlue - minBlue);
                    }
                }

                //khai bao HSI
                double[,] hue = new double[width, height];
                double[,] saturation = new double[width, height];
                double[,] intensity = new double[width, height];

                //chuyen anh RGB ve HSI

                double[,] tuSo = new double[width, height];
                double[,] mauSo = new double[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        intensity[x, y] = (red01[x, y] + green01[x, y] + blue01[x, y]) / 3;

                        tuSo[x, y] = 1.0 * ((red01[x, y] - green01[x, y]) + (red01[x, y] - blue01[x, y])) / 2;
                        mauSo[x, y] = 1.0 * Math.Sqrt(Math.Pow((red01[x, y] - green01[x, y]), 2) + 0.00001 + (red01[x, y] - blue01[x, y]) * ((green01[x, y] - blue01[x, y])));
                        hue[x, y] = 1.0 * Math.Acos(1.0 * tuSo[x, y] / mauSo[x, y]);
                        //hue[x, y] = (int)(hueBuffeR255[x, y] * 180.0 / Math.PI);

                        if (blue01[x, y] > green01[x, y])
                        {
                            hue[x, y] = 2 * Math.PI - hue[x, y];
                        }

                        //hue[x, y] = hue[x, y] * 180 / Math.PI;

                        double minRGB = Math.Min(red01[x, y], Math.Min(green01[x, y], blue01[x, y]));

                        saturation[x, y] = (1 - minRGB * 3.0 / (0.00001 + red01[x, y] + green01[x, y] + blue01[x, y]));
                    }
                }

                //nhan chap kenh i voi mat na K
                double[,] duv = extBitmap.conv2(intensity, width, height, Matrix.Gaussian3x3, Matrix.Gaussian3x3.GetLength(0), Matrix.Gaussian3x3.GetLength(0));

                //tinh lambdaDUV va lambdaIUV
                double[,] lambdaIUV = new double[width, height];
                double[,] lambdaDUV = new double[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        lambdaIUV[x, y] = 0.5 * (1 + Math.Tanh(3 - 12.0 * Math.Abs(intensity[x, y] - 0.5)));
                        lambdaDUV[x, y] = 0.5 * (1 + Math.Tanh(3 - 6.0 * (Math.Abs(duv[x, y]) - 0.5)));
                    }
                }

                //bat dau vong lap
                double kmin = 0.1, kmax = 0.25, ke = 0.05;
                //double kmin = 0.1, kmax = 0.25, ke = 0.01;
                //ke (k_epsilon) la dieu kien hoi tu

                double[,] w1 = new double[width, height];
                double[,] w2 = new double[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        w1[x, y] = intensity[x, y] + 1.0 * kmin * lambdaIUV[x, y] * lambdaDUV[x, y] * duv[x, y];
                        w2[x, y] = intensity[x, y] + 1.0 * kmax * lambdaIUV[x, y] * lambdaDUV[x, y] * duv[x, y];
                    }
                }

                //double n_test = extBitmap.overRange(w2);
                /*lblHeightIndex.Text += bufferB.GetLength(1);
                lblWidthIndex.Text += bufferB.GetLength(0);*/

                //tinh entropy hai anh
                double H1 = Measure.entropy(w1);
                double H2 = Measure.entropy(w2);


                int loop_count = 0;

                while (Math.Abs(kmin - kmax) > ke)
                {
                    loop_count++;
                    if (H1 > H2)
                    {
                        kmax = kmin + 0.618 * 1.0 * Math.Abs(kmin - kmax);

                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                w2[x, y] = intensity[x, y] + 1.0 * kmax * lambdaIUV[x, y] * lambdaDUV[x, y] * duv[x, y];

                            }
                        }
                        H2 = Measure.entropy(w2);

                        double nov2 = extBitmap.overRange(w2);
                        H2 = H2 * (1 - 1.0 * nov2 / (height * width));
                    }
                    else
                    {
                        kmin = kmin + (1 - 0.618) * Math.Abs(kmin - kmax);
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                w1[x, y] = intensity[x, y] + 1.0 * kmin * lambdaIUV[x, y] * lambdaDUV[x, y] * duv[x, y];
                            }
                        }
                        H1 = Measure.entropy(w1);
                        double nov1 = extBitmap.overRange(w1);
                        H1 = H1 * (1 - 1.0 * nov1 / (height * width));
                    }
                }

                //tinh k cuoi cung
                double k_opt = 0.5 * (kmin + kmax);
                I_enhance = new double[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        I_enhance[x, y] = intensity[x, y] + 1.0 * k_opt * lambdaIUV[x, y] * lambdaDUV[x, y] * duv[x, y];

                        //chuan hoa pixel < 0 va > 1
                        if ((I_enhance[x, y] > 1) || (I_enhance[x, y] < 0))
                        {
                            I_enhance[x, y] = intensity[x, y];
                        }
                    }
                }

                //n_test = extBitmap.overRange(w_opt);

                //chuyen ve RGB

                double[,] R255 = new double[width, height];
                double[,] G255 = new double[width, height];
                double[,] B255 = new double[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (hue[x, y] < Math.PI * 2 / 3)
                        {
                            B255[x, y] = 1.0 * I_enhance[x, y] * (1 - saturation[x, y]);
                            R255[x, y] = 1.0 * I_enhance[x, y] * (1 + saturation[x, y] * Math.Cos(hue[x, y]) / Math.Cos(Math.PI / 3 - hue[x, y]));
                            G255[x, y] = 3.0 * I_enhance[x, y] - R255[x, y] - B255[x, y];
                        }
                        else if (hue[x, y] < Math.PI * 4 / 3)
                        {
                            hue[x, y] = hue[x, y] - Math.PI * 2 / 3;
                            R255[x, y] = 1.0 * I_enhance[x, y] * (1 - saturation[x, y]);
                            G255[x, y] = 1.0 * I_enhance[x, y] * (1 + saturation[x, y] * Math.Cos(hue[x, y]) / Math.Cos(Math.PI / 3 - hue[x, y]));
                            B255[x, y] = 3.0 * I_enhance[x, y] - R255[x, y] - G255[x, y];
                        }
                        else
                        {
                            hue[x, y] = hue[x, y] - Math.PI * 4 / 3;
                            G255[x, y] = 1.0 * I_enhance[x, y] * (1 - saturation[x, y]);
                            B255[x, y] = 1.0 * I_enhance[x, y] * (1 + saturation[x, y] * Math.Cos(hue[x, y]) / Math.Cos(Math.PI / 3 - hue[x, y]));
                            R255[x, y] = 3.0 * I_enhance[x, y] - G255[x, y] - B255[x, y];
                        }
                    }
                }

                //chuyen ve 0-255
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        R255[x, y] = 1.0 * R255[x, y] * 255;
                        if (R255[x, y] > 255) R255[x, y] = 255;
                        else
                        {
                            if (R255[x, y] < 0) R255[x, y] = 0;
                        }

                        G255[x, y] = 1.0 * G255[x, y] * 255;
                        if (G255[x, y] > 255) G255[x, y] = 255;
                        else
                        {
                            if (G255[x, y] < 0) G255[x, y] = 0;
                        }

                        B255[x, y] = 1.0 * B255[x, y] * 255;
                        if (B255[x, y] > 255) B255[x, y] = 255;
                        else
                        {
                            if (B255[x, y] < 0) B255[x, y] = 0;
                        }

                    }
                }

                resultBitmap = new Bitmap(width, height);
                //hien anh ket qua
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        resultBitmap.SetPixel(x, y, Color.FromArgb(255, (int)R255[x, y], (int)G255[x, y], (int)B255[x, y]));
                    }
                }

                //tinh thuc nghiem

                //tinh saturation va I cua anh ban dau
                double[,] satBefore = new double[width, height];
                double[,] iBefore = new double[width, height];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        iBefore[x, y] = 1.0 * (red[x, y] + green[x, y] + blue[x, y]) / 3;
                        double minRGB = Math.Min(red[x, y], Math.Min(green[x, y], blue[x, y]));
                        satBefore[x, y] = (1 - minRGB * 3.0 / (0.00001 + red[x, y] + green[x, y] + blue[x, y]));
                    }
                }

                //tinh saturation va I cua anh ket qua
                double[,] satAfter = new double[width, height];
                double[,] iAfter = new double[width, height];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        iAfter[x, y] = 1.0 * (R255[x, y] + G255[x, y] + B255[x, y]) / 3;
                        double minRGB = Math.Min(R255[x, y], Math.Min(G255[x, y], B255[x, y]));
                        satAfter[x, y] = (1 - minRGB * 3.0 / (0.00001 + R255[x, y] + G255[x, y] + B255[x, y]));
                    }
                }

                picbxUnsharp.Image = resultBitmap;

                
                double[,] test_red = new double[,] { { 0, 10, 0 }, { 20, 250, 85 }, { 30, 15, 45 } };
                double[,] test_saturation_matrix = new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
                double[,] test_entropy_matrix = new double[,] { { 1, 1, 1 }, { 1, 1,1 }, { 0,1,0 } };
                double[,] test_green = new double[,] { { 255, 40, 9 }, { 55, 125, 10 }, { 5, 7, 23 } };
                double[,] test_blue = new double[,] { { 50, 51, 53 }, { 50, 52, 55 }, { 51, 54, 53 } };

                double test_color = Measure.colorfulness(test_green, test_blue, test_red, 3, 3);
                double test_sharp = Measure.sharpness(test_green, 3,3);
                double test_entropy = Measure.entropy(test_entropy_matrix);
                double test_sat = Measure.sat(test_saturation_matrix, 3,3);
                

                txtCTshaBefore.Text = Measure.sharpness(iBefore, width, height).ToString();
                txtCTshaAfter.Text = Measure.sharpness(iAfter, width, height).ToString();

                txtCTsatBefore.Text = Measure.sat(satBefore, width, height).ToString();
                txtCTsatAfter.Text = Measure.sat(satAfter, width, height).ToString();

                txtCTcoBefore.Text = Measure.colorfulness(red, green, blue, width, height).ToString();
                txtCTcoAfter.Text = Measure.colorfulness(R255, G255, B255, width, height).ToString();

                textBox1.Text = Measure.entropy(iBefore).ToString();
                txtCTenAfter.Text = Measure.entropy(iAfter).ToString();

                txtVDsatBe.Text = Measure.entropy(iAfter).ToString();
                tbxVDenBefore.Text = Measure.entropy(iAfter).ToString();

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((redMatrix == null) || (greenMatrix == null) || (blueMatrix == null))
            {
                MessageBox.Show("Chưa chọn đủ ma trận!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {


                tbxVDintensityBE.Text = "";
                tbxVDintensityAF.Text = "";

                tbxVDredAfter.Text = "";
                tbxVDgreenAfter.Text = "";
                tbxVDblueAfter.Text = "";

                tbxVDred.Text = "";
                tbxVDgreen.Text = "";
                tbxVDblue.Text = "";

                /*
                double[,] red = { { 0, 10, 0 }, { 20, 250, 85 }, { 30, 15, 45 } };
                double[,] green = { { 255, 40, 9 }, { 55, 125, 10 }, { 5, 7, 23 } };
                double[,] blue = { { 50, 51, 53 }, { 50, 52, 55 }, { 51, 54, 53 } };
                */

                int width = redMatrix.GetLength(0);
                int height = redMatrix.GetLength(1);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        tbxVDred.Text += redMatrix[x, y].ToString();
                        tbxVDred.Text += "\t";

                        tbxVDgreen.Text += greenMatrix[x, y].ToString();
                        tbxVDgreen.Text += "\t";

                        tbxVDblue.Text += blueMatrix[x, y].ToString();
                        tbxVDblue.Text += "\t";
                    }
                    tbxVDred.Text += Environment.NewLine;
                    tbxVDgreen.Text += Environment.NewLine;
                    tbxVDblue.Text += Environment.NewLine;
                }

                double[,] red01 = new double[width, height];
                double[,] green01 = new double[width, height];
                double[,] blue01 = new double[width, height];

                double minRed = extBitmap.min2D(redMatrix, width, height);
                double minBlue = extBitmap.min2D(blueMatrix, width, height);
                double minGreen = extBitmap.min2D(greenMatrix, width, height);

                double maxRed = extBitmap.max2D(redMatrix, width, height);
                double maxBlue = extBitmap.max2D(blueMatrix, width, height);
                double maxGreen = extBitmap.max2D(greenMatrix, width, height);

                /* double testLEnght0 = red01.GetLength(0);
                 double testLEnght1 = red01.GetLength(1);*/

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        red01[x, y] = 1.0 * (redMatrix[x, y] - minRed) / (maxRed - minRed);
                        green01[x, y] = 1.0 * (greenMatrix[x, y] - minGreen) / (maxGreen - minGreen);
                        blue01[x, y] = 1.0 * (blueMatrix[x, y] - minBlue) / (maxBlue - minBlue);
                    }
                }

                //khai bao HSI
                double[,] hue = new double[width, height];
                double[,] saturation = new double[width, height];
                double[,] intensity = new double[width, height];

                double[,] tuSo = new double[width, height];
                double[,] mauSo = new double[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        intensity[x, y] = (red01[x, y] + green01[x, y] + blue01[x, y]) / 3;

                        tuSo[x, y] = 1.0 * ((red01[x, y] - green01[x, y]) + (red01[x, y] - blue01[x, y])) / 2;
                        mauSo[x, y] = 1.0 * Math.Sqrt(Math.Pow((red01[x, y] - green01[x, y]), 2) + 0.00001 + (red01[x, y] - blue01[x, y]) * ((green01[x, y] - blue01[x, y])));
                        hue[x, y] = 1.0 * Math.Acos(1.0 * tuSo[x, y] / mauSo[x, y]);
                        //hue[x, y] = (int)(hueBuffeR255[x, y] * 180.0 / Math.PI);

                        if (blue01[x, y] > green01[x, y])
                        {
                            hue[x, y] = 2 * Math.PI - hue[x, y];
                        }

                        //hue[x, y] = hue[x, y] * 180 / Math.PI;

                        double minRGB = Math.Min(red01[x, y], Math.Min(green01[x, y], blue01[x, y]));

                        saturation[x, y] = (1 - minRGB * 3.0 / (0.00001 + red01[x, y] + green01[x, y] + blue01[x, y]));
                    }
                }

                //nhan chap kenh i voi mat na K
                double[,] duv = extBitmap.conv2(intensity, width, height, Matrix.Gaussian3x3thang, Matrix.Gaussian3x3thang.GetLength(0), Matrix.Gaussian3x3thang.GetLength(0));

                //tinh lambdaDUV va lambdaIUV
                double[,] lambdaIUV = new double[width, height];
                double[,] lambdaDUV = new double[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        lambdaIUV[x, y] = 0.5 * (1 + Math.Tanh(3 - 12.0 * Math.Abs(intensity[x, y] - 0.5)));
                        lambdaDUV[x, y] = 0.5 * (1 + Math.Tanh(3 - 6.0 * (Math.Abs(duv[x, y]) - 0.5)));
                    }
                }

                //bat dau vong lap
                double kmin = 0, kmax = 2, ke = 0.3;
                //ke (k_epsilon) la dieu kien hoi tu

                double[,] w1 = new double[width, height];
                double[,] w2 = new double[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        w1[x, y] = intensity[x, y] + 1.0 * kmin * lambdaIUV[x, y] * lambdaDUV[x, y] * duv[x, y];
                        w2[x, y] = intensity[x, y] + 1.0 * kmax * lambdaIUV[x, y] * lambdaDUV[x, y] * duv[x, y];
                    }
                }

                //tinh entropy hai anh
                double H1 = Measure.entropy(w1);
                double H2 = Measure.entropy(w2);

                //tinh over-range cua hai anh

                /*
                double nov1 = extBitmap.overRange(w1);
                double nov2 = extBitmap.overRange(w2);

                //chuan hoa entropy
                H1 = H1 * (1 - 1.0 * nov1 / (height * width));
                H2 = H2 * (1 - 1.0 * nov2 / (height * width));
                */
                int loop_count = 0;

                while (Math.Abs(kmin - kmax) > ke)
                {
                    loop_count++;
                    if (H1 > H2)
                    {
                        kmax = kmin + 0.618 * 1.0 * Math.Abs(kmin - kmax);

                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                w2[x, y] = intensity[x, y] + 1.0 * kmax * lambdaIUV[x, y] * lambdaDUV[x, y] * duv[x, y];

                            }
                        }
                        H2 = Measure.entropy(w2);

                        double nov2 = extBitmap.overRange(w2);
                        H2 = H2 * (1 - 1.0 * nov2 / (height * width));
                    }
                    else
                    {
                        kmin = kmin + (1 - 0.618) * Math.Abs(kmin - kmax);
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                w1[x, y] = intensity[x, y] + 1.0 * kmin * lambdaIUV[x, y] * lambdaDUV[x, y] * duv[x, y];
                            }
                        }
                        H1 = Measure.entropy(w1);
                        double nov1 = extBitmap.overRange(w1);
                        H1 = H1 * (1 - 1.0 * nov1 / (height * width));
                    }
                }

                //tinh k cuoi cung
                double k_opt = 0.5 * (kmin + kmax);
                I_enhance = new double[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        I_enhance[x, y] = intensity[x, y] + 1.0 * k_opt * lambdaIUV[x, y] * lambdaDUV[x, y] * duv[x, y];

                        //chuan hoa pixel < 0 va > 1
                        if ((I_enhance[x, y] > 1) || (I_enhance[x, y] < 0))
                        {
                            I_enhance[x, y] = intensity[x, y];
                        }
                    }
                }

                //n_test = extBitmap.overRange(w_opt);

                //chuyen ve RGB

                double[,] R255 = new double[width, height];
                double[,] G255 = new double[width, height];
                double[,] B255 = new double[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (hue[x, y] < Math.PI * 2 / 3)
                        {
                            B255[x, y] = 1.0 * I_enhance[x, y] * (1 - saturation[x, y]);
                            R255[x, y] = 1.0 * I_enhance[x, y] * (1 + saturation[x, y] * Math.Cos(hue[x, y]) / Math.Cos(Math.PI / 3 - hue[x, y]));
                            G255[x, y] = 3.0 * I_enhance[x, y] - R255[x, y] - B255[x, y];
                            //if(R255>)
                        }
                        else if (hue[x, y] < Math.PI * 4 / 3)
                        {
                            hue[x, y] = hue[x, y] - Math.PI * 2 / 3;
                            R255[x, y] = 1.0 * I_enhance[x, y] * (1 - saturation[x, y]);
                            G255[x, y] = 1.0 * I_enhance[x, y] * (1 + saturation[x, y] * Math.Cos(hue[x, y]) / Math.Cos(Math.PI / 3 - hue[x, y]));
                            B255[x, y] = 3.0 * I_enhance[x, y] - R255[x, y] - G255[x, y];
                        }
                        else
                        {
                            hue[x, y] = hue[x, y] - Math.PI * 4 / 3;
                            G255[x, y] = 1.0 * I_enhance[x, y] * (1 - saturation[x, y]);
                            B255[x, y] = 1.0 * I_enhance[x, y] * (1 + saturation[x, y] * Math.Cos(hue[x, y]) / Math.Cos(Math.PI / 3 - hue[x, y]));
                            R255[x, y] = 3.0 * I_enhance[x, y] - G255[x, y] - B255[x, y];
                        }
                    }
                }

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        //R255[x, y] = (int)(R255[x, y] * 255);
                        R255[x, y] = Math.Round(R255[x, y] * 255);
                        //Math.Round()
                        if (R255[x, y] > 255) R255[x, y] = 255;
                        else
                        {
                            if (R255[x, y] < 0) R255[x, y] = 0;
                        }

                        //G255[x, y] = (int)(G255[x, y] * 255);
                        G255[x, y] = Math.Round(G255[x, y] * 255);
                        if (G255[x, y] > 255) G255[x, y] = 255;
                        else
                        {
                            if (G255[x, y] < 0) G255[x, y] = 0;
                        }

                        B255[x, y] = Math.Round(B255[x, y] * 255);
                        //B255[x, y] = (int)(B255[x, y] * 255);
                        if (B255[x, y] > 255) B255[x, y] = 255;
                        else
                        {
                            if (B255[x, y] < 0) B255[x, y] = 0;
                        }

                    }
                }

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        String result = string.Format("{0:0.0000}", intensity[x, y]);
                        tbxVDintensityBE.Text += result;
                        tbxVDintensityBE.Text += "\t";

                        result = string.Format("{0:0.0000}", I_enhance[x, y]);
                        tbxVDintensityAF.Text += result;
                        tbxVDintensityAF.Text += "\t";

                        result = string.Format("{0:0}", R255[x, y]);
                        tbxVDredAfter.Text += result;
                        tbxVDredAfter.Text += "\t";

                        result = string.Format("{0:0}", G255[x, y]);
                        tbxVDgreenAfter.Text += result;
                        tbxVDgreenAfter.Text += "\t";

                        result = string.Format("{0:0}", B255[x, y]);
                        tbxVDblueAfter.Text += result;
                        tbxVDblueAfter.Text += "\t";
                    }

                    tbxVDintensityBE.Text += Environment.NewLine;
                    tbxVDintensityAF.Text += Environment.NewLine;

                    tbxVDredAfter.Text += Environment.NewLine;
                    tbxVDgreenAfter.Text += Environment.NewLine;
                    tbxVDblueAfter.Text += Environment.NewLine;
                }

                //tinh saturation va I cua anh ban dau
                double[,] satBefore = new double[width, height];
                double[,] iBefore = new double[width, height];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        iBefore[x, y] = 1.0 * (redMatrix[x, y] + greenMatrix[x, y] + blueMatrix[x, y]) / 3;
                        double minRGB = Math.Min(redMatrix[x, y], Math.Min(greenMatrix[x, y], blueMatrix[x, y]));
                        satBefore[x, y] = (1 - minRGB * 3.0 / (0.00001 + redMatrix[x, y] + greenMatrix[x, y] + blueMatrix[x, y]));
                    }
                }

                //tinh saturation va I cua anh ket qua
                double[,] satAfter = new double[width, height];
                double[,] iAfter = new double[width, height];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        iAfter[x, y] = 1.0 * (R255[x, y] + G255[x, y] + B255[x, y]) / 3;
                        double minRGB = Math.Min(R255[x, y], Math.Min(G255[x, y], B255[x, y]));
                        satAfter[x, y] = (1 - minRGB * 3.0 / (0.00001 + R255[x, y] + G255[x, y] + B255[x, y]));
                    }
                }



                tbxVDenBefore.Text = Measure.entropy(iBefore).ToString();
                tbxVDenAfter.Text = Measure.entropy(iAfter).ToString();

                tbxVDsharpBefore.Text = Measure.sharpness(iBefore, width, height).ToString();
                tbxVDsharpAfter.Text = Measure.sharpness(iAfter, width, height).ToString();

                tbxVDcorBefore.Text = Measure.colorfulness(redMatrix, greenMatrix, blueMatrix, width, height).ToString();
                tbxVDcorAfter.Text = Measure.colorfulness(R255, G255, B255, width, height).ToString();

                txtVDsatBe.Text = Measure.sat(satBefore, width, height).ToString();
                tbxVDsatAf.Text = Measure.sat(satAfter, width, height).ToString();
            }
        }

        public static double[,] openMatrix()
        {
            string path = null;
            double[,] result = null;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "File text|*.txt";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;//lay ra duong dan file ma tran vua mo
            }
            else
            {
                path = null;
            }

            if (path == null)
            {
                
                MessageBox.Show("Chưa chọn ma trận!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return result;
            }
            int width = 0, height = 0;
            string[] line = File.ReadAllLines(path);
            width = line.Length;
            height = line[0].Split(' ').Length;
            result = new double[width, height];
            for(int x=0;x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    result[x, y] = new double();
                    result[x, y] = double.Parse(line[x].Split(' ')[y]);
                }
            }
            return result;
        }

        private void btnOpenRed_Click(object sender, EventArgs e)
        {
            tbxVDred.Text = "";
            redMatrix = openMatrix();
            if (redMatrix == null)
            {
                return;     
            }
            else
            {
                int width = redMatrix.GetLength(0);
                int height = redMatrix.GetLength(1);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        tbxVDred.Text += redMatrix[x, y].ToString();
                        tbxVDred.Text += "\t";
                    }
                    tbxVDred.Text += Environment.NewLine;
                }
            }

        }

        private void btnOpenGreen_Click(object sender, EventArgs e)
        {
            tbxVDgreen.Text = "";
            greenMatrix = openMatrix();
            if (greenMatrix == null)
            {
                return;
            }
            else
            {
                int width = greenMatrix.GetLength(0);
                int height = greenMatrix.GetLength(1);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        tbxVDgreen.Text += greenMatrix[x, y].ToString();
                        tbxVDgreen.Text += "\t";
                    }
                    tbxVDgreen.Text += Environment.NewLine;
                }
            }
            
        }

        private void btnOpenBlue_Click(object sender, EventArgs e)
        {
            tbxVDblue.Text = "";
            blueMatrix = openMatrix();
            if (blueMatrix == null)
            {
                return;
            }
            else
            {
                int width = blueMatrix.GetLength(0);
                int height = blueMatrix.GetLength(1);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        tbxVDblue.Text += blueMatrix[x, y].ToString();
                        tbxVDblue.Text += "\t";
                    }
                    tbxVDblue.Text += Environment.NewLine;
                }
            }
            
        }
    }
}
