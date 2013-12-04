namespace OpenAA.Monazilla
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using NLog;
    using OpenAA.IO;
    using OpenAA.Extensions.String;

    public class MonaAgentSession : IDisposable
    {
        [JsonIgnore]
        public MonaAgent Agent { get; private set; }

        public string Id { get; set; }

        public string HAP { get; set; }

        public string PON { get; set; }

        private IDictionary<string,DateTime> _wait;
        public  IDictionary<string,DateTime> Wait 
        { 
            get 
            { 
                if (_wait == null )
                {
                    _wait = new Dictionary<string, DateTime>();
                }
                return _wait;
            }
            set { _wait = value; }
        }

        private IDictionary<string,int> _sambaLimit;
        public  IDictionary<string,int> SambaLimit 
        { 
            get 
            { 
                if (_sambaLimit == null )
                {
                    _sambaLimit = new Dictionary<string, int>();
                }
                return _sambaLimit;
            }
            set { _sambaLimit = value; }
        }

        private IDictionary<string,int> _sambaCount;
        public  IDictionary<string,int> SambaCount
        { 
            get 
            { 
                if (_sambaCount == null )
                {
                    _sambaCount = new Dictionary<string, int>();
                }
                return _sambaCount;
            }
            set { _sambaCount = value; }
        }

        private MonaAgentSession()
        {
            // 永続化のため、引数はなし。
            // 初期化はCreateメソッドで。
        }

        ~MonaAgentSession()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            this.Save();
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            var path = MonaAgentSession.GetSessionPath(this.Agent, this.Id);
            FileUtility.Write(path, json);
        }

        public static MonaAgentSession Create(MonaAgent agent, string id = null)
        {
            var instance = Load(agent, id);
            if (instance == null)
            {
                instance = new MonaAgentSession
                {
                    Id = id,
                    HAP = "",
                    PON = "",
                    Wait = new Dictionary<string,DateTime>(),
                    SambaLimit = new Dictionary<string,int>(),
                    SambaCount = new Dictionary<string,int>(),
                };
            }

            // これ重要
            instance.Agent = agent;

            return instance;
        }

        public static MonaAgentSession Load(MonaAgent agent, string id = null)
        {
            MonaAgentSession instance = null;
            var path = GetSessionPath(agent, id);
            var json = FileUtility.ReadToEnd(path);
            if (!string.IsNullOrEmpty(json))
            {
                instance = JsonConvert.DeserializeObject<MonaAgentSession>(json);
            }
            return instance;
        }

        public static string GetSessionPath(MonaAgent agent, string id = null)
        {
            var filename = "";
            if (string.IsNullOrEmpty(id))
            {
                filename = "_default.json";
            }
            else if (id.IsAsciiAlphabetAndNumeric())
            {
                filename = id + ".json";
            }
            else
            {
                throw new NotSupportedException("id='" + id + "'");
            }

            return agent.SessionDirectory + Path.DirectorySeparatorChar + filename;
        }
    }
}

