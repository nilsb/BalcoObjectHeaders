using Newtonsoft.Json;

namespace SPLib.Shared
{
    public class SPModel<T>
    {
        [JsonIgnore]
        [JsonExtensionData]
        public SPList<T> List { get; set; }
        [JsonIgnore]
        public long Id { get; set; }
        [JsonIgnore]
        public long ID { get; set; }
        [JsonIgnore]
        public SPUser Author { get; set; }
        [JsonIgnore]
        public SPUser Editor { get; set; }
        [JsonIgnore]
        public SPDate Created { get; set; }
        [JsonIgnore]
        public SPDate Modified { get; set; }
        public string Title { get; set; }
    }
}
