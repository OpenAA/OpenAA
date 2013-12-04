namespace OpenAA.IO
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public static class FileUtility
    {
        public static void Write(string path, string value, bool append = false)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (var sw = new StreamWriter(path, append))
            {
                sw.Write(value);
            }
        }

        public static string ReadToEnd(string path)
        {
            if (!File.Exists(path))
            {
                return string.Empty;
            }
            using (var sr = new StreamReader(path))
            {
                return sr.ReadToEnd();
            }
        }
    }
}

