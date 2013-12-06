using System;

namespace OpenAA.Monazilla.Models
{
    public class MonaResponse
    {
        public MonaThread Thread { get; set; }

        public string Raw { get; set; }

        public int No { get; set; }

        public string Name { get; set; }

        public string Mail { get; set; }

        public string Id { get; set; }

        public string Extra { get; set; }

        public string Message { get; set; }

        // ----

        //public bool IsAborn { get { throw new NotImplementedException(); } }

        //public bool IsAsciiArt { get { throw new NotImplementedException(); } }

        //public bool IsPopular { get { throw new NotImplementedException(); } }

        //public bool HasLink { get { throw new NotImplementedException(); } }

        //public bool HasMovie { get { throw new NotImplementedException(); } }

        //public bool HasImage { get { throw new NotImplementedException(); } }

    }
}

