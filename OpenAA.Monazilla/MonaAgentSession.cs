namespace OpenAA.Monazilla
{
    using System;
    using System.IO;
    using OpenAA.IO;

    public class MonaAgentSession : IDisposable
    {
        public int Id { get; set; }

        public string HAP { get; set; }

        public string PON { get; set; }

        public DateTime Wait { get; set; }

        public MonaAgentSession(int id = 0)
        {
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

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

        #region IDisposable implementation

        private bool _disposed = false;

        public void Dispose()
        {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }

                _disposed = true;
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("[MonaAgentSession: Id={0}, HAP={1}, PON={2}, Wait={3}]", Id, HAP, PON, Wait);
        }
    }
}

