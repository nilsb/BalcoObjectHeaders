using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPLib.Shared
{
    public class SPList<T> : IList<T>
    {
        private readonly Client _client;
        private readonly List<string> NonSettable = new List<string>() { "ID", "Id", "Modified", "Created", "Author", "Editor", "Web" };
        private readonly IList<T> list = new List<T>();
        private readonly string baseUrl;
        private readonly string listName;
        private readonly string apiUrl;
        public readonly string itemUrl;
        private readonly string query;
        private readonly SPWeb web;
        private readonly Dictionary<string, string> lookupWeb;
        private readonly Dictionary<string, string> lookupList;

        public SPWeb ParentWeb {
            get
            {
                return web;
            }
        }

        //public SPList(Client client)
        //{
        //    _client = client;
        //    var builder = new ConfigurationBuilder().AddJsonFile(@"appsettings.json");
        //    var section = builder.Build().GetSection("SP");
        //    baseUrl = section["BaseUrl"];
        //    listName = section["ListTitle"];
        //    web = new SPWeb(baseUrl);
        //}

        public SPList(SPContext context, string ListName, Dictionary<string, string> LookupWeb, Dictionary<string, string> LookupList)
        {
            _client = context._client;
            baseUrl = context.url;
            listName = ListName;
            apiUrl = $"{baseUrl}/_api/web/lists/getbytitle('{listName}')";
            itemUrl = $"{apiUrl}/items";
            web = context.web;
            lookupWeb = LookupWeb;
            lookupList = LookupList;
        }

        public SPList(Client client, string Url, string ListName, Dictionary<string, string> LookupWeb, Dictionary<string, string> LookupList)
        {
            _client = client;
            baseUrl = Url;
            listName = ListName;
            apiUrl = $"{baseUrl}/_api/web/lists/getbytitle('{listName}')";
            itemUrl = $"{apiUrl}/items";
            web = new SPWeb(client, baseUrl);
            lookupWeb = LookupWeb;
            lookupList = LookupList;
        }

        public SPList(Client client, string Url, string ListName, string Query, Dictionary<string, string> LookupWeb, Dictionary<string, string> LookupList)
        {
            _client = client;
            baseUrl = Url;
            listName = ListName;
            apiUrl = $"{baseUrl}/_api/web/lists/getbytitle('{listName}')";
            itemUrl = $"{apiUrl}/items";
            query = Query;
            web = new SPWeb(client, baseUrl);
            lookupWeb = LookupWeb;
            lookupList = LookupList;
        }

        public int Count => (int)_client.Get(baseUrl, $"{apiUrl}/ItemCount")["ItemCount"];

        public async Task<int> CountAsync() {
            return (int)((await _client.GetAsync(baseUrl, $"{apiUrl}/ItemCount"))["ItemCount"]);
        }

        public bool IsReadOnly => false;

        public async Task<T> GetAsync(long index)
        {
            T item = (T)Activator.CreateInstance(typeof(T));
            var d = await _client.GetAsync(baseUrl, $"{itemUrl}({index})");
            Type type = typeof(T);
            var props = type.GetProperties();

            foreach (var prop in props)
            {
                var lp = new ListProperty<T>(_client, lookupWeb, lookupList);
                await lp.SetAsync(this, item, prop, d);
            }

            return item;
        }

        public async Task SetAsync(long index, T value)
        {
            Type type = typeof(T);
            var eD = await _client.GetAsync(baseUrl, $"{apiUrl}?$select=ListItemEntityTypeFullName");
            string ListItemEntityTypeFullName = (string)eD["ListItemEntityTypeFullName"];
            var body = @"{ __metadata: { 'type': '" + ListItemEntityTypeFullName + "' }";

            var props = type.GetProperties();

            foreach (var prop in props)
            {
                if (NonSettable.Contains(prop.Name))
                    continue;

                var lp = new ListProperty<T>(_client, lookupWeb, lookupList);
                body += await lp.GetAsync(this, value, prop);
            }

            body += " }";

            await _client.PostAsync(baseUrl, $"{itemUrl}({index})", body, "MERGE");
        }

        public T this[int index]
        {
            get
            {
                T item = (T)Activator.CreateInstance(typeof(T));
                var d = _client.Get(baseUrl, $"{itemUrl}({index})");

                if(d != null)
                {
                    Type type = typeof(T);
                    var props = type.GetProperties();

                    foreach (var prop in props)
                    {
                        var lp = new ListProperty<T>(_client, lookupWeb, lookupList);
                        lp.Set(this, item, prop, d);
                    }
                }

                return item;
            }
            set
            {
                Type type = typeof(T);
                var eD = _client.Get(baseUrl, $"{apiUrl}?$select=ListItemEntityTypeFullName");

                if(eD != null)
                {
                    string ListItemEntityTypeFullName = (string)eD["ListItemEntityTypeFullName"];

                    var body = @"{ __metadata: { 'type': '" + ListItemEntityTypeFullName + "' }";

                    var props = type.GetProperties();

                    foreach (var prop in props)
                    {
                        if (NonSettable.Contains(prop.Name))
                            continue;

                        var lp = new ListProperty<T>(_client, lookupWeb, lookupList);
                        body += lp.Get(this, value, prop);
                    }

                    body += " }";

                    _client.Post(baseUrl, $"{itemUrl}({index})", body, "MERGE");
                }
            }
        }

        public async Task<List<T>> FindAsync(string filter)
        {
            List<T> found = new List<T>();
            Type type = typeof(T);
            var props = type.GetProperties();
            var d = await _client.GetAsync(baseUrl, itemUrl + "?$filter=" + filter);

            if(d != null)
            {
                foreach (var foundItem in d.results)
                {
                    T itm = (T)Activator.CreateInstance(typeof(T));

                    foreach (var prop in props)
                    {
                        var lp = new ListProperty<T>(_client, lookupWeb, lookupList);
                        await lp.SetAsync(this, itm, prop, foundItem);
                    }

                    found.Add(itm);
                }
            }

            return found;
        }


        public List<T> Find(string filter)
        {
            List<T> found = new List<T>();
            Type type = typeof(T);
            var props = type.GetProperties();
            var d = _client.Get(baseUrl, itemUrl + "?$filter=" + filter);

            if (d != null)
            {
                foreach (var foundItem in d.results)
                {
                    T itm = (T)Activator.CreateInstance(typeof(T));

                    foreach (var prop in props)
                    {
                        var lp = new ListProperty<T>(_client, lookupWeb, lookupList);
                        lp.Set(this, itm, prop, foundItem);
                    }

                    found.Add(itm);
                }
            }

            return found;
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAtAsync(int index)
        {
            await _client.PostAsync(baseUrl, $"{itemUrl}({index})", "DELETE");
        }

        public void RemoveAt(int index)
        {
            _client.Post(baseUrl, $"{itemUrl}({index})", "DELETE");
        }

        public async Task<T> AddAsync(T item)
        {
            var eD = await _client.GetAsync(baseUrl, $"{apiUrl}?$select=ListItemEntityTypeFullName");

            if(eD != null)
            {
                string ListItemEntityTypeFullName = (string)eD["ListItemEntityTypeFullName"];
                var body = "{ \"__metadata\": { \"type\": \"" + ListItemEntityTypeFullName + "\" }";

                Type type = typeof(T);
                var props = type.GetProperties();

                foreach (var prop in props)
                {
                    if (NonSettable.Contains(prop.Name))
                        continue;

                    var lp = new ListProperty<T>(_client, lookupWeb, lookupList);
                    body += lp.Get(this, item, prop);
                }

                body += " }";

                var d = await _client.PostAsync(baseUrl, $"{itemUrl}", body, "");

                if(d != null) 
                {
                    foreach (var prop in props)
                    {
                        var lp = new ListProperty<T>(_client, lookupWeb, lookupList);
                        await lp.SetAsync(this, item, prop, d);
                    }
                }

                return item;
            }

            return default(T);
        }

        public void Add(T item)
        {
            var eD = _client.Get(baseUrl, $"{apiUrl}?$select=ListItemEntityTypeFullName");

            if(eD != null)
            {
                string ListItemEntityTypeFullName = (string)eD["ListItemEntityTypeFullName"];
                var body = "{ \"__metadata\": { \"type\": \"" + ListItemEntityTypeFullName + "\" }";

                Type type = typeof(T);
                var props = type.GetProperties();

                foreach (var prop in props)
                {
                    if (NonSettable.Contains(prop.Name))
                        continue;

                    var lp = new ListProperty<T>(_client, lookupWeb, lookupList);
                    body += lp.Get(this, item, prop);
                }

                body += " }";

                _client.Post(baseUrl, $"{itemUrl}", body, "");
            }
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveAsync(T item)
        {
            Type type = typeof(T);
            var index = this.GetIndexById(item);
            var Id = index.Id.ToString();

            await _client.PostAsync(baseUrl, $"{itemUrl}({Id})", "DELETE");

            return true;
        }

        public bool Remove(T item)
        {
            Type type = typeof(T);
            var index = this.GetIndexById(item);
            var Id = index.Id.ToString();

            _client.Post(baseUrl, $"{itemUrl}({Id})", "DELETE");

            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var itemsCount = this.Count();
            var pageItems = 1000;
            var pID = 1;

            list.Clear();

            while(pID <= itemsCount)
            {
                var msg = _client.Get(baseUrl, $"{itemUrl}?$skiptoken=Paged=TRUE%26p_ID={pID - 1}&$top={pageItems}");
                pID += pageItems;

                foreach (var d in msg.results)
                {
                    T item = (T)Activator.CreateInstance(typeof(T));
                    Type type = typeof(T);
                    var props = type.GetProperties();

                    foreach (var prop in props)
                    {
                        var lp = new ListProperty<T>(_client, lookupWeb, lookupList);
                        lp.Set(this, item, prop, d);
                    }

                    list.Add(item);
                }
            }

            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var itemsCount = this.Count();
            var pageItems = 1000;
            var pID = 1;

            list.Clear();

            while (pID <= itemsCount)
            {
                var msg = _client.Get(baseUrl, $"{itemUrl}?$skiptoken=Paged=TRUE%26p_ID={pID - 1}&$top={pageItems}");
                pID += pageItems;

                foreach (var d in msg.results)
                {
                    T item = (T)Activator.CreateInstance(typeof(T));
                    Type type = typeof(T);
                    var props = type.GetProperties();

                    foreach (var prop in props)
                    {
                        var lp = new ListProperty<T>(_client, lookupWeb, lookupList);
                        lp.Set(this, item, prop, d);
                    }

                    list.Add(item);
                }
            }

            return list.GetEnumerator();
        }

        public List<dynamic> GetFields()
        {
            var d = _client.Post(baseUrl, $"{apiUrl}/Fields", "");
            return d.results;
        }

        private dynamic GetIndexById(T item)
        {
            Type type = typeof(T);
            var propID = type.GetProperty("Id");
            var valueId = (long)propID.GetValue(item);

            var listitem = list.SingleOrDefault(i =>
            {
                var Id = (long)propID.GetValue(i);

                return Id == valueId;
            });

            return new { Item = listitem, Index = list.IndexOf(listitem), Id = valueId };
        }

        private long GetIndexById(dynamic d)
        {
            if(d != null && d.Id != null)
            {
                return (long)d.Id;
            }

            return -1;
        }
    }
}
