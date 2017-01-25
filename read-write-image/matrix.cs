using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace read_write_image
{
    public static class Matrix
    {
        public static double[,] Gaussian3x3
        {
            get
            {
                return new double[,]
                { {-0.125,-0.125,-0.125 },
                   {-0.125,0.5,-0.125 },
                   {-0.125,-0.125,-0.125 } };
            }
        }

        public static double[,] Gaussian3x3thang
        {
            get
            {
                return new double[,]
                {
                    {0,-0.25,0 }, {-0.25,1,-0.25 }, {0,-0.25,0 }
                };
            }
        }
    }
}
