namespace SPLib.Shared
{
    public class SPLookup
    {
        public long Id { get; set; }
        public string Value { get; set; }

        public SPLookup()
        {

        }

        public SPLookup(long id, string value)
        {
            Id = id;
            Value = value;
        }
    }
}
