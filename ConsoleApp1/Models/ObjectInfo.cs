using SPLib.Shared;

namespace SPLib.Models
{
    public class ObjectInfo : SPModel<ObjectInfo>
    {
        public string Object_x0020_title { get; set; }
        public string Projectnumber { get; set; }
        public string Project_x0020_Adress { get; set; }
        public string City_x0020_Name { get; set; }
        public SPUser Seller { get; set; }
        public long Qty_x0020_Standard { get; set; }
        public long Qty_x0020_Balustrades { get; set; }
        public long Offer_x0020_sum { get; set; }
        public string ObjectStatus { get; set; }
        public string WorkZip { get; set; }
        public SPUser Project_x0020_Manager { get; set; }
    }
}
