namespace SPLib.Shared
{
    public class SPKeyValue
    {
        public dynamic __metadata
        {
            get
            {
                return new
                {
                    type = "SP.KeyValue"
                };
            }
        }
        public dynamic Key { get; set; }
        public dynamic Value { get; set; }
        public dynamic ValueType { get; set; }
    }
}
