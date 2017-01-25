using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace read_write_image
{
    class Measure
    {
        public static double log2(double numb)
        {

            return Math.Log(numb) / Math.Log(2);
        }    

        public static double sat(double[,] matrix, int iWidth, int iHeight)
        {
            double sum = 0;
            for (int y = 0; y < iHeight; y++)
            {
                for (int x = 0; x < iWidth; x++)
                {
                    sum += matrix[x, y];
                }
            }
            sum = 1.0 * sum / (iWidth * iHeight);
            return sum;
        }

        public static double colorfulness(double[,] iR, double[,] iG, double[,] iB, int iWidth,int iHeight)
        {
            double[,] deltaRG = new double[iWidth, iHeight];
            double[,] deltaYB = new double[iWidth, iHeight];

            //khai bao gia tri trung binh va do lech chuan
            double stdRG = 0, stdYB = 0, meanRG = 0, meanYB = 0, meanRGYB = 0, stdRGYB = 0;
            for (int y = 0; y < iHeight ; y++)
            {
                for (int x = 0; x < iWidth ; x++)
                {
                    deltaRG[x, y] = iR[x, y] - iG[x, y];
                    deltaYB[x, y] = 0.5 * (iR[x, y] + iG[x, y]) - iB[x, y];
                }
            }

            //tinh gia tri trung binh
            for (int y = 0; y < iHeight ; y++)
            {
                for (int x = 0; x < iWidth ; x++)
                {
                    meanRG += 1.0 * deltaRG[x, y];
                    meanYB += 1.0 * deltaYB[x, y];
                }
            }

            meanRG = 1.0 * meanRG / (iHeight * iWidth);
            meanYB = 1.0 * meanYB / (iHeight * iWidth);

            //tinh do lech chuan
            
            foreach(var element in deltaRG)
            {
                stdRG += 1.0 * Math.Pow((element - meanRG), 2) / (iHeight * iWidth - 1);
            }

            foreach (var element in deltaYB)
            {
                stdYB += 1.0 * Math.Pow((element - meanYB), 2) / (iHeight * iWidth - 1);
            }
            
            stdRG = Math.Sqrt(stdRG);
            stdYB = Math.Sqrt(stdYB);
            

            meanRGYB = 1.0 * Math.Sqrt(meanRG * meanRG + meanYB * meanYB);
            stdRGYB = 1.0 * Math.Sqrt(stdRG * stdRG + stdYB * stdYB);

            double temp = 1.0 * stdRGYB + 0.3 * meanRGYB;
            //return 1.0 * stdYB + 0.3 * meanRGYB;
            return 1.0 * stdRGYB + 0.3 * meanRGYB;
        }

        public static double sharpness(double[,] matrix, int iWidth,int iHeight)
        {
            double[,] xuv = new double[iWidth -1, iHeight -1];
            double[,] yuv = new double[iWidth -1, iHeight -1];
            double sum = 0;
            for (int y = 0; y < iHeight - 1; y++)
            {
                for (int x = 0; x < iWidth - 1; x++)
                {
                    xuv[x, y] = matrix[x, y] - matrix[x + 1, y];
                    yuv[x, y] = matrix[x, y] - matrix[x, y + 1];
                    sum += 1.0 * Math.Sqrt(xuv[x, y] * xuv[x, y] + yuv[x, y] * yuv[x, y]);
                }
            }

            sum = 1.0 * sum / ((iWidth) * (iHeight ));
            return sum;
        }

        public static double entropy(double[,] matrix)
        {
            double result = 0;
            Dictionary<double, int> table = new Dictionary<double, int>();

            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    //matrix[x, y] = (int)(matrix[x, y] * 255);
                    if (table.ContainsKey(matrix[x, y])) table[matrix[x, y]]++;
                    else table.Add(matrix[x, y], 1);
                }
            }

            double freq;

            foreach (var pair in table)
            {
                freq = 1.0 * pair.Value / (matrix.GetLength(0) * matrix.GetLength(1));
                result += freq * log2(freq);

            }

            result *= -1;
            return result;
        }
    }
}
