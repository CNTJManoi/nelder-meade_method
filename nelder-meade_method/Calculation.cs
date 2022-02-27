using AngouriMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nelder_meade_method
{
    public static class Calculation
    {
        public static float? CalcFunc(string f, float x, float y)
        {
            float res;
            try
            {
                Entity expr = f;
                res = (float)expr.Substitute("x", x).Substitute("y", y).EvalNumerical();
            }
            catch (Exception)
            {
                return null;
            }

            return res;
        }
    }
}
