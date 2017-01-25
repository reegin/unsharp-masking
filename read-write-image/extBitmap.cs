using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace read_write_image
{
    class extBitmap
    {
        public static double min2D(double[,] input, int iWidth, int iHeight)
        {
            double result = input[0, 0];
            for (int y = 0; y < iHeight; y++)
            {
                for (int x = 0; x < iWidth; x++)
                {
                    if (input[x, y] < result) result = input[x, y];
                }
            }
            return result;
        }

        public static double max2D(double[,] input, int iWidth, int iHeight)
        {
            double result = input[0, 0];
            for (int y = 0; y < iHeight; y++)
            {
                for (int x = 0; x < iWidth; x++)
                {
                    if (input[x, y] > result) result = input[x, y];
                }
            }
            return result;
        }

        public static double[,] conv2(double[,] input, int iWidth, int iHeight, double[,] kernel, int kWidth, int kHeight)
        {
            double[,] resultMatrix = new double[iWidth, iHeight];
            int kMiddleWidth = kWidth / 2;
            int kMiddleHeight = kHeight / 2;
            double result;
            double sum;
            for (int y = 0; y < iHeight; ++y)
            {
                for (int x = 0; x < iWidth; ++x)
                {
                    sum = 0;
                    for (int i = 0; i <= kHeight - 1; i++)
                    {
                        for (int j = 0; j <= kWidth - 1; j++)
                        {
                            if ((y + (i - kMiddleHeight)) < 0 || (y + (i - kMiddleHeight)) >= iHeight || ((x + (j - kMiddleWidth)) < 0) || ((x + (j - kMiddleWidth)) >= iWidth))
                            {
                                result = 0;
                            }
                            else
                            {
                                result = 1.0*input[x + (j - kMiddleWidth), y + (i - kMiddleHeight)] * kernel[i, j];
                            }
                            sum += result;

                        }
                    }
                    resultMatrix[x, y] = sum;
                }
            }
            return resultMatrix;
        }

        public static double overRange(double[,] matrix)
        {
            double count = 0;
            for(int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    if ((matrix[x,y]<0) || (matrix[x, y] > 1))
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}
