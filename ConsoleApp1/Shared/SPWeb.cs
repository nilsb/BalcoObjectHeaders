using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SPLib.Shared
{
    public class SPWeb
    {
        private readonly Client _client;
        private readonly string baseUrl;
        private dynamic web { get; set; }

        public string Id { get; set; }
        public long Language { get; set; }
        public DateTime LastItemModifiedDate { get; set; }
        public string Title { get; set; }
        public string WebTemplate { get; set; }
        public string Description { get; set; }
        public string FullUrl { get; set; }

        public SPWeb(Client client, string Url)
        {
            baseUrl = Url;
            FullUrl = Url;
            _client = client;
        }

        public SPWeb(Client client, string Url, SPWeb input)
        {
            baseUrl = Url;
            this.Id = input.Id;
            this.Language = input.Language;
            this.LastItemModifiedDate = input.LastItemModifiedDate;
            this.Title = input.Title;
            this.Description = input.Description;
            this.WebTemplate = input.WebTemplate;
            this.FullUrl = input.FullUrl;
            _client = client;
        }

        public SPWeb(Client client, string Url, dynamic input)
        {
            baseUrl = Url;
            this.Id = input.Id;
            this.Language = input.Language;
            this.LastItemModifiedDate = input.LastItemModifiedDate;
            this.Title = input.Title;
            this.Description = input.Description;
            this.WebTemplate = input.WebTemplate;
            this.FullUrl = input.Url;
            _client = client;
        }

        public List<SPUser> Users(string q = "")
        {
            q += "*";

            var result = new List<SPUser>();
            var query = _client.Get(baseUrl, $"{baseUrl}/_api/search/query?querytext='" + q + "'&sourceid='B09A7990-05EA-4AF9-81EF-EDFAB16C4E31'&rowlimit=4999").query.PrimaryQueryResult;

            if (query != null)
            {
                SPSimpleDataTable table = new SPSimpleDataTable(query.RelevantResults.Table);
                table.RowCount = (int)query.RelevantResults.RowCount.Value;

                foreach (SPSimpleDataRow row in table.Rows)
                {
                    SPUser user = new SPUser(new {
                        Id = -1,
                        LoginName = (string)row.Cells["AccountName"].Value,
                        Title = (string)row.Cells["PreferredName"].Value,
                        JobTitle = (string)row.Cells["JobTitle"].Value,
                        Email = (string)row.Cells["WorkEmail"].Value
                    });
                    result.Add(user);
                }
            }

            return result;
        }

        public async Task<SPUser> EnsureUserAsync(string AccountName)
        {
            SPUser result = null;
            var user = await _client.PostAsync(baseUrl, $"{baseUrl}/_api/web/ensureuser(@v)?@v='{HttpUtility.UrlEncode(AccountName)}'", "");

            if (user != null)
            {
                result = new SPUser(user);
                var profile = await _client.GetAsync(baseUrl, $"{baseUrl}/_api/SP.UserProfiles.PeopleManager/GetPropertiesFor(@v)?@v='{HttpUtility.UrlEncode(result.LoginName)}'");

                if (profile != null && profile.UserProfileProperties != null)
                {
                    var profileProperties = (JObject)profile.UserProfileProperties;
                    var title = profileProperties.SelectToken(".results[?(@.Key == 'SPS-JobTitle')]");

                    if (title != null && title.SelectToken("Value") != null)
                    {
                        result.JobTitle = title.SelectToken("Value").ToString();
                    }
                }
            }

            return result;
        }

        public SPUser EnsureUser(string AccountName)
        {
            SPUser result = null;
            var user = _client.Post(baseUrl, $"{baseUrl}/_api/web/ensureuser(@v)?@v='{HttpUtility.UrlEncode(AccountName)}'", "");

            if (user != null)
            {
                result = new SPUser(user);
                var profile = _client.Get(baseUrl, $"{baseUrl}/_api/SP.UserProfiles.PeopleManager/GetPropertiesFor(@v)?@v='{HttpUtility.UrlEncode(result.LoginName)}'");

                if (profile != null && profile.UserProfileProperties != null)
                {
                    var profileProperties = (JObject)profile.UserProfileProperties;
                    var title = profileProperties.SelectToken(".results[?(@.Key == 'SPS-JobTitle')]");

                    if (title != null && title.SelectToken("Value") != null)
                    {
                        result.JobTitle = title.SelectToken("Value").ToString();
                    }
                }
            }

            return result;
        }

        public async Task<SPUser> EnsureUserAsync(long Id)
        {
            SPUser user = new SPUser();
            var result = await _client.GetAsync(baseUrl, $"{baseUrl}/_api/web/getuserbyid({Id})");

            if (result != null)
            {
                user.Id = result.Id;
                user.LoginName = result.LoginName;
                user.Title = result.Title;
                user.Email = result.Email;

                var profile = await _client.GetAsync(baseUrl, $"{baseUrl}/_api/SP.UserProfiles.PeopleManager/GetPropertiesFor(@v)?@v='{HttpUtility.UrlEncode(user.LoginName)}'");

                if (profile != null && profile.UserProfileProperties != null)
                {
                    var profileProperties = (JObject)profile.UserProfileProperties;
                    var title = profileProperties.SelectToken(".results[?(@.Key == 'SPS-JobTitle')]");

                    if (title != null && title.SelectToken("Value") != null)
                    {
                        user.JobTitle = title.SelectToken("Value").ToString();
                    }
                }
            }

            return user;
        }

        public SPUser EnsureUser(long Id)
        {
            SPUser user = new SPUser();
            var result = _client.Get(baseUrl, $"{baseUrl}/_api/web/getuserbyid({Id})");

            if (result != null)
            {
                user.Id = result.Id;
                user.LoginName = result.LoginName;
                user.Title = result.Title;
                user.Email = result.Email;

                var profile = _client.Get(baseUrl, $"{baseUrl}/_api/SP.UserProfiles.PeopleManager/GetPropertiesFor(@v)?@v='{HttpUtility.UrlEncode(user.LoginName)}'");

                if (profile != null && profile.UserProfileProperties != null)
                {
                    var profileProperties = (JObject)profile.UserProfileProperties;
                    var title = profileProperties.SelectToken(".results[?(@.Key == 'SPS-JobTitle')]");

                    if (title != null && title.SelectToken("Value") != null)
                    {
                        user.JobTitle = title.SelectToken("Value").ToString();
                    }
                }
            }

            return user;
        }

        public void Load()
        {
            var Web = _client.Get(baseUrl, $"{baseUrl}/_api/web");

            this.web = Web;
            this.Id = Web.Id;
            this.Language = Web.Language;
            this.LastItemModifiedDate = Web.LastItemModifiedDate;
            this.Title = Web.Title;
            this.Description = Web.Description;
            this.WebTemplate = Web.WebTemplate;
        }

        public void SendMail(List<string> to, string subject, string body)
        {
            string from = "sps@balco.se";

            if (to.Count <= 0 || subject.Length <= 0 || body.Length <= 0)
                return;

            List<SPKeyValue> headers = new List<SPKeyValue>
            {
                new SPKeyValue() { Key = "content-type", Value = "text/html", ValueType = "Edm.String" }
            };

            var jsonObject = new
            {
                properties = new
                {
                    __metadata = new
                    {
                        type = "SP.Utilities.EmailProperties"
                    },
                    From = from,
                    To = GetTo(to),
                    Body = body,
                    Subject = subject,
                    AdditonalHeaders = new 
                    { 
                        __metadata = new
                        {
                            type = "Collection(SP.KeyValue)"
                        },
                        results = headers
                    }
                }
            };

            string json = JsonConvert.SerializeObject(jsonObject);

            _client.Post(baseUrl, $"{baseUrl}/_api/SP.Utilities.Utility.SendEmail", json, "");
        }

        private dynamic GetTo(List<string> to)
        {
            if (to.Count > 1)
            {
                return new
                {
                    results = to
                };
            }
            else
            {
                return to.First();
            }
        }
    }
}
