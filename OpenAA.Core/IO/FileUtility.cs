namespace OpenAA.IO
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Text;
    using OpenAA.Text;

    public static class FileUtility
    {
        public static void Write(string path, string value, bool append = false, Encoding encoding = null)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (encoding == null)
            {
                encoding = Encoding.Default;
            }
            using (var sw = new StreamWriter(path, append, encoding))
            {
                sw.Write(value);
            }
        }

        public static string ReadToEnd(string path, Encoding encoding = null)
        {
            if (!File.Exists(path))
            {
                return string.Empty;
            }

            if (encoding == null)
            {
                encoding = Encoding.Default;
            }
            using (var sr = new StreamReader(path, encoding))
            {
                return sr.ReadToEnd();
            }
        }
    }
}

