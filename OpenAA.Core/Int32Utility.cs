namespace OpenAA
{
    using System;

    public static class Int32Utility
    {
        public static int ParseOrDefault(string s, int defaultValue = 0)
        {
            int result;
            if (!int.TryParse(s, out result))
            {
                result = defaultValue;
            }
            return result;
        }
    }
}
