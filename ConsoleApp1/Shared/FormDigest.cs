﻿using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SPLib.Shared
{
    public class FormDigest
    {
        public async Task<string> GetAsync(string Url, string UserName, string Password, string Domain)
        {
            string result = "";
            string response = "";

            using (HttpClientHandler handler = new HttpClientHandler()
            {
                Credentials = new Credentials().GetCurrent(Url, UserName, Password, Domain)
            })
            {
                using (HttpClient _client = new HttpClient(handler))
                {
                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Add("accept", "application/json;odata=verbose");

                    HttpContent content = new StringContent("");
                    content.Headers.ContentType = MediaTypeWithQualityHeaderValue.Parse("application/json;odata=verbose;charset=utf-8");
                    content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("odata", "verbose"));

                    HttpResponseMessage responseMessage = await _client.PostAsync($"{Url}/_api/contextinfo", content);

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        response = await responseMessage.Content.ReadAsStringAsync();

                        var val = JsonConvert.DeserializeObject<dynamic>(response);
                        result = val.d.GetContextWebInformation.FormDigestValue.ToString();
                    }
                }
            }

            return result;
        }

        public string Get(string Url, string UserName, string Password, string Domain)
        {
            return GetAsync(Url, UserName, Password, Domain).Result;
        }

        public string Get(string Url, ICredentials credential)
        {
            string baseUrl = Url;
            string response = "";
            HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create($"{baseUrl}/_api/contextinfo");
            endpointRequest.Method = "POST";
            endpointRequest.Credentials = credential;
            endpointRequest.Accept = "application/json;odata=verbose";
            endpointRequest.ContentLength = 0;
            HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();

            using (Stream dataStream = endpointResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                response = reader.ReadToEnd();
            }

            endpointResponse.Close();

            var val = JsonConvert.DeserializeObject<dynamic>(response);
            return val.d.GetContextWebInformation.FormDigestValue.ToString();
        }
    }
}
