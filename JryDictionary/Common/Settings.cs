using System.Collections.Generic;

namespace JryDictionary.Common
{
    public class Settings
    {
        public List<NameValuePair> EndPoints { get; set; }

        public string Proxy { get; set; }

        public class NameValuePair
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }
    }
}