namespace OpenAA
{
    using System;

    public static class DoubleUtility
    {
        public static double ParseOrDefault(string s, double defaultValue = 0)
        {
            double result;
            if (!double.TryParse(s, out result))
            {
                result = defaultValue;
            }
            return result;
        }
    }
}
