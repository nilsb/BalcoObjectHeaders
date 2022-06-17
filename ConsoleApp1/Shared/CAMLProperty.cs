using System.Reflection;

namespace SPLib.Shared
{
    public static class CAMLProperty<T>
    {
        public static string Get(string value, PropertyInfo property, bool isDateOnly)
        {
            string query = "";

            switch (property.PropertyType.Name)
            {
                case "SPUser":
                    query = GetUserProperty(value, property);

                    break;
                case "SPMultiUser":
                    query = GetUserListProperty(value, property);

                    break;
                case "DateTime":
                    if (isDateOnly)
                        query = GetDateProperty(value, property);
                    else
                        query = GetDateTimeProperty(value, property);

                    break;
                default:
                    query += "<FieldRef Name='" + property.Name + "' /><Value Type='Text'>" + value + "</Value>";

                    break;
            }

            return query;
        }

        public static string GetUserProperty(string value, PropertyInfo property)
        {
            string query = "";

            query += "<FieldRef Name='" + property.Name + "' /><Value Type='User'>" + value + "</Value>";

            return query;
        }

        public static string GetUserListProperty(string value, PropertyInfo property)
        {
            string query = "";

            query += "<FieldRef Name='" + property.Name + "' /><Value Type='LookupMulti'>" + value + "</Value>";

            return query;
        }

        public static string GetDateProperty(string value, PropertyInfo property)
        {
            string query = "";

            query += "<FieldRef Name='" + property.Name + "' /><Value Type='DateTime' IncludeTimeValue='FALSE'>" + value + "</Value>";

            return query;
        }

        public static string GetDateTimeProperty(string value, PropertyInfo property)
        {
            string query = "";

            query += "<FieldRef Name='" + property.Name + "' /><Value Type='DateTime'>" + value + "</Value>";

            return query;
        }
    }
}
