namespace OpenAA.Math
{
    using System;

    public static class MathUtility
    {
        /// <summary>
        /// 数値を切り上げする。
        /// ExcelのROUNDUP関数と互換です。
        /// 
        /// 小数点以下第1位まで表示（小数点以下第2位を切り上げ）
        /// RoundUp( 23.45, 1 ) == 23.5
        /// 
        /// 小数点以下を表示しない（小数点以下第1位を切り上げ）
        /// RoundUp( 23.45, 0 ) == 24
        /// 
        /// 1の位を切り上げ
        /// RoundUp( 23.45, -1 ) == 30
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static double RoundUp(double value, int digits)
        {
            double coef = System.Math.Pow(10, digits);

            if (0 < value)
            {
                return System.Math.Ceiling(value * coef) / coef;
            }
            else
            {
                return System.Math.Floor(value * coef) / coef;
            }
        }
    }
}

