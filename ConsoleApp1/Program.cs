using SPLib.Models;
using SPLib.Shared;
using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    internal class Program
    {
        private static SPContext _context;
        private static string objectBaseUrl = "https://my.sp.url/site";
        private static string UserName = "";
        private static string Password = "";
        private static string Domain = "";
        private static Dictionary<string, string> lookupWeb;
        private static Dictionary<string, string> lookupList;
        private static SPList<ObjectNumber> objects;

        public static string GetObjectUrl(string number)
        {
            string returnValue = "";

            //try to get prospect
            List<ObjectNumber> prospects = objects.Find($"Prospectnumber eq '{number}'");

            if (prospects != null && prospects.Count > 0)
            {
                returnValue = objectBaseUrl + "/" + prospects[0].Title;
            }
            else
            {
                List<ObjectNumber> projects = objects.Find($"Projectnumber eq '{number}'");

                if (projects != null && projects.Count > 0)
                {
                    returnValue = objectBaseUrl + "/" + projects[0].Title;
                }
            }

            return returnValue;
        }

        public static ObjectInfo GetObjectHeaders(string number)
        {
            ObjectInfo returnValue = null;
            string objectUrl = GetObjectUrl(number);

            if (!string.IsNullOrEmpty(objectUrl))
            {
                SPContext objectContext = new SPContext(UserName, Password, Domain, objectUrl);
                SPList<ObjectInfo> objectInfo = new SPList<ObjectInfo>(objectContext, "Object Info", lookupWeb, lookupList);
                
                foreach(ObjectInfo info in objectInfo)
                {
                    returnValue = info;
                }
            }

            return returnValue;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            _context = new SPContext(UserName, Password, Domain, objectBaseUrl);
            lookupWeb = new Dictionary<string, string>();
            lookupList = new Dictionary<string, string>();
            lookupWeb.Add("Market", "https://lookup.base.url");
            lookupList.Add("Market", "Market");
            objects = new SPList<ObjectNumber>(_context, "ObjectNumbers", lookupWeb, lookupList);
            ObjectInfo objInfo = GetObjectHeaders("14111");
            Console.WriteLine(objInfo.Project_x0020_Adress);
        }
    }
}
