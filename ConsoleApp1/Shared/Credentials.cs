using System;
using System.Net;

namespace SPLib.Shared
{
    public class Credentials
    {
        public NetworkCredential GetCurrent(string Url, string UserName, string Password, string Domain)
        {
            ICredentials credentials = CredentialCache.DefaultNetworkCredentials;
            var credential = credentials.GetCredential(new Uri(Url), "Basic");

            if (credential.UserName.Length <= 0)
            {
                credential = new NetworkCredential(UserName, Password, Domain);
            }

            return credential;
        }
    }
}
