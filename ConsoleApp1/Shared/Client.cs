using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SPLib.Shared
{
    public class Client
    {
        private readonly string UserName;
        private readonly string Password;
        private readonly string Domain;

        public Client(string username, string password, string domain)
        {
            UserName = username;
            Password = password;
            Domain = domain;
        }

        public async Task<HttpResult> SendAsync(string digestUrl, string Url, string method, string body, string xmethod = "")
        {
            string response = "";
            string digest = await new FormDigest().GetAsync(digestUrl, UserName, Password, Domain);
            HttpResult result = new HttpResult(HttpStatusCode.NotFound, "");

            using (HttpClientHandler handler = new HttpClientHandler()
            {
                Credentials = new Credentials().GetCurrent(Url, UserName, Password, Domain)
            })
            {
                using (HttpClient _client = new HttpClient(handler))
                {
                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Add("accept", "application/json;odata=verbose;charset=utf-8");
                    _client.DefaultRequestHeaders.Add("X-RequestDigest", digest);

                    if (xmethod.Length > 0)
                    {
                        _client.DefaultRequestHeaders.Add("If-Match", "*");
                        _client.DefaultRequestHeaders.Add("X-HTTP-Method", xmethod);
                    }

                    HttpContent content = new StringContent(body);
                    content.Headers.ContentType = MediaTypeWithQualityHeaderValue.Parse("application/json;odata=verbose;charset=utf-8");
                    content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("odata", "verbose"));

                    try
                    {
                        HttpResponseMessage clientResponse = null;

                        if (method.ToLowerInvariant() == "post")
                            clientResponse = await _client.PostAsync(Url, content);
                        if (method.ToLowerInvariant() == "put")
                            clientResponse = await _client.PutAsync(Url, content);
                        if (method.ToLowerInvariant() == "get")
                            clientResponse = await _client.GetAsync(Url);
                        if (method.ToLowerInvariant() == "delete")
                            clientResponse = await _client.DeleteAsync(Url);

                        if (clientResponse.IsSuccessStatusCode)
                        {
                            response = await clientResponse.Content.ReadAsStringAsync();
                            result = new HttpResult(clientResponse.StatusCode, response);
                        }
                        else
                        {
                            result = new HttpResult(clientResponse.StatusCode, clientResponse.ReasonPhrase);
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        result = new HttpResult(HttpStatusCode.InternalServerError, ex.Message);
                    }
                }
            }

            return result;
        }


        public HttpResult Send(string digestUrl, string Url, string method, string body, string xmethod = "")
        {
            string response = "";
            string digest = new FormDigest().Get(digestUrl, UserName, Password, Domain);
            HttpResult result = new HttpResult(HttpStatusCode.NotFound, "");

            using (HttpClientHandler handler = new HttpClientHandler()
            {
                Credentials = new Credentials().GetCurrent(Url, UserName, Password, Domain)
            })
            {
                using (HttpClient _client = new HttpClient(handler))
                {
                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Add("accept", "application/json;odata=verbose;charset=utf-8");
                    _client.DefaultRequestHeaders.Add("X-RequestDigest", digest);

                    if (xmethod.Length > 0)
                    {
                        _client.DefaultRequestHeaders.Add("If-Match", "*");
                        _client.DefaultRequestHeaders.Add("X-HTTP-Method", xmethod);
                    }

                    HttpContent content = new StringContent(body);
                    content.Headers.ContentType = MediaTypeWithQualityHeaderValue.Parse("application/json;odata=verbose;charset=utf-8");
                    content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("odata", "verbose"));

                    try
                    {
                        HttpResponseMessage clientResponse = null;

                        if (method.ToLowerInvariant() == "post")
                            clientResponse = _client.PostAsync(Url, content).Result;
                        if (method.ToLowerInvariant() == "put")
                            clientResponse = _client.PutAsync(Url, content).Result;
                        if (method.ToLowerInvariant() == "get")
                            clientResponse = _client.GetAsync(Url).Result;
                        if (method.ToLowerInvariant() == "delete")
                            clientResponse = _client.DeleteAsync(Url).Result;

                        if (clientResponse.IsSuccessStatusCode)
                        {
                            response = clientResponse.Content.ReadAsStringAsync().Result;
                            result = new HttpResult(clientResponse.StatusCode, response);
                        }
                        else
                        {
                            response = clientResponse.Content.ReadAsStringAsync().Result;
                            result = new HttpResult(clientResponse.StatusCode, clientResponse.ReasonPhrase);
                        }

                    }
                    catch (HttpRequestException ex)
                    {
                        result = new HttpResult(HttpStatusCode.InternalServerError, ex.Message);
                    }
                }
            }

            return result;
        }

        public async Task<dynamic> GetAsync(string digestUrl, string Url)
        {
            dynamic d = null;

            var result = await SendAsync(digestUrl, Url, "GET", "");

            if (result.Status == HttpStatusCode.OK || result.Status == HttpStatusCode.Accepted || result.Status == HttpStatusCode.Created || result.Status == HttpStatusCode.NoContent)
            {
                var msg = JsonConvert.DeserializeObject<dynamic>(result.Message);

                if (msg != null)
                {
                    d = msg.d;
                }
            }

            return d;
        }

        public dynamic Get(string digestUrl, string Url)
        {
            dynamic d = null;

            var result = Send(digestUrl, Url, "GET", "");

            if (result.Status == HttpStatusCode.OK || result.Status == HttpStatusCode.Accepted || result.Status == HttpStatusCode.Created || result.Status == HttpStatusCode.NoContent)
            {
                var msg = JsonConvert.DeserializeObject<dynamic>(result.Message);

                if (msg != null)
                {
                    d = msg.d;
                }
            }

            return d;
        }

        public async Task<dynamic> PostAsync(string digestUrl, string Url, string body, string xmethod = "")
        {
            dynamic d = null;

            var result = await SendAsync(digestUrl, Url, "POST", body, xmethod);

            if (result.Status == HttpStatusCode.OK || result.Status == HttpStatusCode.Accepted || result.Status == HttpStatusCode.Created || result.Status == HttpStatusCode.NoContent)
            {
                var msg = JsonConvert.DeserializeObject<dynamic>(result.Message);

                if (msg != null)
                {
                    d = msg.d;
                }
            }

            return d;
        }

        public dynamic Post(string digestUrl, string Url, string body, string xmethod = "")
        {
            dynamic d = null;

            var result = Send(digestUrl, Url, "POST", body, xmethod);

            if (result.Status == HttpStatusCode.OK || result.Status == HttpStatusCode.Accepted || result.Status == HttpStatusCode.Created || result.Status == HttpStatusCode.NoContent)
            {
                var msg = JsonConvert.DeserializeObject<dynamic>(result.Message);

                if (msg != null)
                {
                    d = msg.d;
                }
            }

            return d;
        }

        public async Task<bool> PutAsync(string digestUrl, string Url, string body, string xmethod = "")
        {
            var result = await SendAsync(digestUrl, Url, "PUT", body, xmethod);

            if (result.Status == HttpStatusCode.OK || result.Status == HttpStatusCode.Accepted || result.Status == HttpStatusCode.Created || result.Status == HttpStatusCode.NoContent)
            {
                return false;
            }

            return true;
        }

        public bool Put(string digestUrl, string Url, string body, string xmethod = "")
        {
            var result = Send(digestUrl, Url, "PUT", body, xmethod);

            if (result.Status == HttpStatusCode.OK || result.Status == HttpStatusCode.Accepted || result.Status == HttpStatusCode.Created || result.Status == HttpStatusCode.NoContent)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteAsync(string digestUrl, string Url, string body, string xmethod = "")
        {
            var result = await SendAsync(digestUrl, Url, "DELETE", body, xmethod);

            if (result.Status == HttpStatusCode.OK || result.Status == HttpStatusCode.Accepted || result.Status == HttpStatusCode.Created || result.Status == HttpStatusCode.NoContent)
            {
                return false;
            }

            return true;
        }

        public bool Delete(string digestUrl, string Url, string body, string xmethod = "")
        {
            var result = Send(digestUrl, Url, "DELETE", body, xmethod);

            if (result.Status == HttpStatusCode.OK || result.Status == HttpStatusCode.Accepted || result.Status == HttpStatusCode.Created || result.Status == HttpStatusCode.NoContent)
            {
                return false;
            }

            return true;
        }
    }
}
