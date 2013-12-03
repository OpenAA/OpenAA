namespace OpenAA.Monazilla
{
    using System;
    using System.IO;

    using OpenAA.IO;
    using OpenAA.Extensions.String;

    using Newtonsoft.Json;
    using NLog;

    public class MonaAgentSession : IDisposable
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public string Id { get; set; }

        public string HAP { get; set; }

        public string PON { get; set; }

        public DateTime Wait { get; set; }

        private MonaAgentSession()
        {
        }

        public void Save()
        {
            _logger.Trace("Save");
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            var path = GetSessionPath(this.Id);
            using (var writer = new StreamWriter(path))
            {
                writer.Write(json);
            }
        }

        public static MonaAgentSession Create(string id = null)
        {
            return new MonaAgentSession()
            {
                Id  = id,
                HAP  = "",
                PON  = "",
                Wait = DateTime.Now,
            };
        }

        public static MonaAgentSession LoadOrCreate(string id = null)
        {
            var instance = Load(id);
            if (instance == null)
            {
                instance = Create(id);
            }
            return instance;
        }

        public static MonaAgentSession Load(string id = null)
        {
            _logger.Trace("Load");
            MonaAgentSession instance = null;

            var path = GetSessionPath(id);
            if (File.Exists(path))
            {
                using (var reader = new StreamReader(path))
                {
                    var json = reader.ReadToEnd();
                    _logger.Trace(json);
                    instance = JsonConvert.DeserializeObject<MonaAgentSession>(json);
                }
            }
            return instance;
        }

        public static string GetSessionPath(string id)
        {
            // ディレクトリ
            var dir = PathUtility.GetDataPath()
                     + Path.DirectorySeparatorChar + "session";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // ファイル
            var fileName = "";
            if (string.IsNullOrEmpty(id))
            {
                fileName = "_default.json";
            }
            else if (id.IsAsciiAlphabetAndNumeric())
            {
                fileName = id + ".json";
            }
            else
            {
                throw new NotSupportedException("id");
            }

            var path = dir + Path.DirectorySeparatorChar + fileName;

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

                this.Save();

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

