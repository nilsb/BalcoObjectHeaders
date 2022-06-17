using System.Collections.Generic;

namespace SPLib.Shared
{
    public class SPContext
    {
        public readonly Client _client;
        public readonly SPWeb web;
        public readonly string url;
        private Dictionary<string, string> lookupWeb = new Dictionary<string, string>();
        private Dictionary<string, string> lookupList = new Dictionary<string, string>();

        public SPContext(string username, string password, string domain, string Url)
        {
            _client = new Client(username, password, domain);
            web = new SPWeb(_client, Url);
            url = Url;
        }
    }
}
