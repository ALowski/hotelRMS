using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotels.Core.Tools
{
    public static class HoltsCalculation
    {
        public static double[] Calculate(int length, double[] array)
        {
            double error=0;
            int count = array.Length;
            double[] l = new double[count];
            double[] r = new double[count];
            double aMin = 0;
            double gMin = 0;
            double? minValue=null;
            for(double a = 0; a <= 1; a += 0.01)
                for (double g = 0; g <= 1; g += 0.01)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (i == 0)
                        {
                            l[i] = array[i];
                            r[i] = ((array[i + 1] - array[i]) + (array[i + 2] - array[i + 1]) + (array[i + 3] - array[i + 2])) / 3;
                        }
                        else
                        {
                            l[i] = a * array[i] + (1 - a) * (l[i - 1] + r[i - 1]);
                            r[i] = g * (l[i] - l[i - 1]) + (1 - g) * r[i - 1];
                            error += (array[i] - l[i - 1] - r[i - 1]) * (array[i] - l[i - 1] - r[i - 1]);
                        }
                    }
                    error = error / count;
                    if (!minValue.HasValue ||error <= minValue )
                    {
                        aMin = a;
                        gMin = g;
                        minValue = error;
                    }
                }

            for (int i = 1; i < count; i++)
            {
                l[i] = aMin * array[i] + (1 - aMin) * (l[i - 1] + r[i - 1]);
                r[i] = gMin * (l[i] - l[i - 1]) + (1 - gMin) * r[i - 1];                
            }
            int m = 1;
            var result = new double[length];
            for (var i = 0; i < length; ++i, m++)
            {
                result[i] = l[count - 1] + m * r[count - 1];
                if (result[i] < 0)
                {
                    result[i] = 0;
                    if (r[count - 1] < 0)
                        break;
                }
            }
            return result;
        }
    }
}
