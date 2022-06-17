using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SPLib.Shared
{
    public class ListProperty<T>
    {
        private readonly Client _client;
        private readonly Dictionary<string, string> _lookupWeb;
        private readonly Dictionary<string, string> _lookupList;

        public ListProperty(Client client, Dictionary<string, string> lookupWeb, Dictionary<string, string> lookupList) 
        {
            _client = client;
            _lookupWeb = lookupWeb;
            _lookupList = lookupList;
        }

        public string Get(SPList<T> list, T item, PropertyInfo property)
        {
            string body = "";

            if (!property.IsDefined(typeof(JsonIgnoreAttribute), false))
            {
                switch (property.PropertyType.Name)
                {
                    case "SPUser":
                        body = GetUserProperty(list, item, property);
                        break;
                    case "SPMultiUser":
                        body = GetUserListProperty(list, item, property);
                        break;
                    case "SPLookup":
                        body = GetLookupProperty(list, item, property);
                        break;
                    case "SPDate":
                        body = GetDateProperty(item, property);
                        break;
                    default:
                        var typeValue = property.GetValue(item);

                        if (typeValue != null)
                            body += ", \"" + property.Name + "\": \"" + typeValue.ToString() + "\"";
                        break;
                }
            }

            return body;
        }

        public async Task<string> GetAsync(SPList<T> list, T item, PropertyInfo property)
        {
            string body = "";

            if (!property.IsDefined(typeof(JsonIgnoreAttribute), false))
            {
                switch (property.PropertyType.Name)
                {
                    case "SPUser":
                        body = await GetUserPropertyAsync(list, item, property);
                        break;
                    case "SPMultiUser":
                        body = await GetUserListPropertyAsync(list, item, property);
                        break;
                    case "SPLookup":
                        body = GetLookupProperty(list, item, property);
                        break;
                    case "SPDate":
                        body = GetDateProperty(item, property);
                        break;
                    default:
                        var typeValue = property.GetValue(item);

                        if (typeValue != null)
                            body += ", \"" + property.Name + "\": \"" + typeValue.ToString() + "\"";
                        break;
                }
            }

            return body;
        }

        public string GetUserProperty(SPList<T> list, T item, PropertyInfo property)
        {
            string body = "";
            SPUser typeValue = (SPUser)property.GetValue(item);

            if (typeValue != null)
            {
                if(!string.IsNullOrEmpty(typeValue.LoginName) && typeValue.Id > 0)
                {
                    var ensuredUser = list.ParentWeb.EnsureUser(typeValue.LoginName);

                    if(ensuredUser != null)
                    {
                        typeValue = ensuredUser;

                        if (typeValue.Id > 0)
                            body += ", \"" + property.Name + "Id\": " + typeValue.Id.ToString();
                    }
                }
                else if(typeValue.Id > 0)
                {
                    body += ", \"" + property.Name + "Id\": " + typeValue.Id.ToString();
                }
            }

            return body;
        }

        public async Task<string> GetUserPropertyAsync(SPList<T> list, T item, PropertyInfo property)
        {
            string body = "";
            SPUser typeValue = (SPUser)property.GetValue(item);

            if (typeValue != null)
            {
                if (!string.IsNullOrEmpty(typeValue.LoginName) && typeValue.Id > 0)
                {
                    var ensuredUser = await list.ParentWeb.EnsureUserAsync(typeValue.LoginName);

                    if (ensuredUser != null)
                    {
                        typeValue = ensuredUser;

                        if (typeValue.Id > 0)
                            body += ", \"" + property.Name + "Id\": " + typeValue.Id.ToString();
                    }
                }
                else if(typeValue.Id > 0)
                {
                    body += ", \"" + property.Name + "Id\": " + typeValue.Id.ToString();
                }
            }

            return body;
        }

        public string GetUserListProperty(SPList<T> list, T item, PropertyInfo property)
        {
            string body = "";
            SPMultiUser typeValue = (SPMultiUser)property.GetValue(item);

            if (typeValue != null)
            {
                if(typeValue.SelectedUserLogins.Count > 0)
                {
                    typeValue.Users = new List<SPUser>();
                    
                    foreach(var loginName in typeValue.SelectedUserLogins)
                    {
                        var user = list.ParentWeb.EnsureUser(loginName);

                        if(user != null)
                            typeValue.Users.Add(user);
                    }

                    body += ", \"" + property.Name + "Id\": { \"results\": [" + string.Join(",", typeValue.Users.Select(u => u.Id)) + "] }";
                }
            }
            else
            {
                body += ", \"" + property.Name + "Id\": { \"results\": [] }";
            }

            return body;
        }

        public async Task<string> GetUserListPropertyAsync(SPList<T> list, T item, PropertyInfo property)
        {
            string body = "";
            SPMultiUser typeValue = (SPMultiUser)property.GetValue(item);

            if (typeValue != null)
            {
                if (typeValue.SelectedUserLogins.Count > 0)
                {
                    typeValue.Users = new List<SPUser>();

                    foreach (var loginName in typeValue.SelectedUserLogins)
                    {
                        var user = await list.ParentWeb.EnsureUserAsync(loginName);

                        if(user != null)
                            typeValue.Users.Add(user);
                    }

                    body += ", \"" + property.Name + "Id\": { \"results\": [" + string.Join(",", typeValue.Users.Select(u => u.Id)) + "] }";
                }
            }
            else
            {
                body += ", \"" + property.Name + "Id\": { \"results\": [] }";
            }

            return body;
        }

        public string GetLookupProperty(SPList<T> list, T item, PropertyInfo property)
        {
            string body = "";
            SPLookup typeValue = (SPLookup)property.GetValue(item);

            if (typeValue != null)
            {
                if (typeValue.Id > 0)
                    body += ", \"" + property.Name + "Id\": " + typeValue.Id.ToString();
            }

            return body;
        }

        public string GetDateProperty(T item, PropertyInfo property)
        {
            string body = "";
            SPDate typeValue = (SPDate)property.GetValue(item);

            if(typeValue != null)
                body += ", \"" + property.Name + "\": \"" + typeValue.DateValue.ToString("yyyy-MM-ddTHH:mm:ssZ") + "\"";

            return body;
        }

        public void Set(SPList<T> list, T item, PropertyInfo property, dynamic d)
        {
            if (!property.IsDefined(typeof(JsonExtensionDataAttribute), false))
            {
                switch (property.PropertyType.Name)
                {
                    case "SPUser":
                        SetUserProperty(list, item, property, d);
                        break;
                    case "SPMultiUser":
                        SetUserListProperty(list, item, property, d);
                        break;
                    case "SPLookup":
                        SetLookupProperty(list, item, property, d);
                        break;
                    case "SPDate":
                        SetDateProperty(item, property, d);
                        break;
                    default:
                        if (d.ContainsKey(property.Name) && d[property.Name].Value != null)
                            property.SetValue(item, d[property.Name].Value);
                        break;
                }
            }
        }

        public async Task SetAsync(SPList<T> list, T item, PropertyInfo property, dynamic d)
        {
            if (!property.IsDefined(typeof(JsonExtensionDataAttribute), false))
            {
                switch (property.PropertyType.Name)
                {
                    case "SPUser":
                        SetUserPropertyAsync(list, item, property, d);
                        break;
                    case "SPMultiUser":
                        SetUserListPropertyAsync(list, item, property, d);
                        break;
                    case "SPLookup":
                        await SetLookupPropertyAsync(list, item, property,d );
                        break;
                    case "SPDate":
                        SetDateProperty(item, property, d);
                        break;
                    default:
                        if (d.ContainsKey(property.Name) && d[property.Name].Value != null)
                            property.SetValue(item, d[property.Name].Value);
                        break;
                }
            }
        }

        public void SetUserProperty(SPList<T> list, T item, PropertyInfo property, dynamic d)
        {
            if (d.ContainsKey(property.Name + "Id") && d[property.Name + "Id"].Value != null)
            {
                var user = list.ParentWeb.EnsureUser((long)d[property.Name + "Id"].Value);

                if(user != null)
                    property.SetValue(item, user);
            }
        }

        public async Task SetUserPropertyAsync(SPList<T> list, T item, PropertyInfo property, dynamic d)
        {
            if (d.ContainsKey(property.Name + "Id") && d[property.Name + "Id"].Value != null)
            {
                var user = await list.ParentWeb.EnsureUserAsync((long)d[property.Name + "Id"].Value);

                if (user != null)
                    property.SetValue(item, user);
            }
        }

        public void SetUserListProperty(SPList<T> list, T item, PropertyInfo property, dynamic d)
        {
            if (d.ContainsKey(property.Name + "Id") && d[property.Name + "Id"] != null && d[property.Name + "Id"].results != null)
            {
                var mu = new SPMultiUser();

                foreach (long id in d[property.Name + "Id"].results)
                {
                    var user = list.ParentWeb.EnsureUser(id);

                    if (user != null)
                    {
                        mu.Users.Add(user);
                        mu.SelectedUserLogins.Add(user.LoginName);
                    }
                }

                property.SetValue(item, mu);
            }
        }

        public async Task SetUserListPropertyAsync(SPList<T> list, T item, PropertyInfo property, dynamic d)
        {
            if (d.ContainsKey(property.Name + "Id") && d[property.Name + "Id"] != null && d[property.Name + "Id"].results != null)
            {
                var mu = new SPMultiUser();

                foreach (long id in d[property.Name + "Id"].results)
                {
                    var user = await list.ParentWeb.EnsureUserAsync(id);

                    if (user != null)
                    {
                        mu.Users.Add(user);
                        mu.SelectedUserLogins.Add(user.LoginName);
                    }
                }

                property.SetValue(item, mu);
            }
        }

        public void SetLookupProperty(SPList<T> list, T item, PropertyInfo property, dynamic d)
        {
            string sourceWeb = "";
            string sourceList = "";

            if (_lookupWeb.TryGetValue(property.Name, out sourceWeb))
            {
                if (_lookupList.TryGetValue(property.Name, out sourceList))
                {
                    if (d.ContainsKey(property.Name + "Id") && d[property.Name + "Id"].Value != null)
                    {
                        var lookupId = (long)(d[property.Name + "Id"].Value);
                        var lookupd = _client.Get(sourceWeb, sourceWeb + "/_api/web/lists/getbytitle('" + sourceList + "')/items(" + lookupId + ")");

                        if (lookupd != null && lookupd.Id != null && lookupd.Title != null)
                        {
                            SPLookup value = new SPLookup((long)lookupd.Id.Value, lookupd.Title.Value);

                            property.SetValue(item, value);
                        }
                    }

                }
            }
        }

        public async Task SetLookupPropertyAsync(SPList<T> list, T item, PropertyInfo property, dynamic d)
        {
            string sourceWeb = "";
            string sourceList = "";

            if(_lookupWeb.TryGetValue(property.Name, out sourceWeb))
            {
                if(_lookupList.TryGetValue(property.Name, out sourceList))
                {
                    if (d.ContainsKey(property.Name + "Id") && d[property.Name + "Id"].Value != null)
                    {
                        var lookupId = (long)(d[property.Name + "Id"].Value);
                        var lookupd = await _client.GetAsync(sourceWeb, sourceWeb + "_api/web/lists/getbytitle('" + sourceList + "')/items(" + lookupId + ")");

                        if (lookupd != null && lookupd.Id != null && lookupd.Title != null)
                        {
                            SPLookup value = new SPLookup((long)lookupd.Id.Value, lookupd.Title.Value);

                            property.SetValue(item, value);
                        }
                    }
                }
            }
        }

        public void SetDateProperty(T item, PropertyInfo property, dynamic d)
        {
            if (d.ContainsKey(property.Name) && d[property.Name].Value != null)
            {
                property.SetValue(item, new SPDate((DateTime)d[property.Name].Value));
            }
        }

    }
}
