namespace OpenAA.Monazilla
{
    using System;
    using System.IO;
    using OpenAA.IO;

    public class MonaAgentSession
    {
        public MonaAgentSession()
        {
        }

        public string HAP { get; set; }

        public string PON { get; set; }

        public static string GetSessionPath(int id)
        {
            // ディレクトリ
            var path = PathUtility.GetDataPath()
                     + Path.DirectorySeparatorChar + "session";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // ファイル
            path += Path.DirectorySeparatorChar + id;

            return path;
        }
    }
}

