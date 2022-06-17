using SPLib.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SPLib.Models
{
    public class ObjectNumber : SPModel<ObjectNumber>
    {
        [Display(Name="Project Number")]
        public string Projectnumber { get; set; }
        [Display(Name = "Prospect Number")]
        public string Prospectnumber { get; set; }
        public SPUser Seller { get; set; }
        [Display(Name = "Project Manager")]
        public SPUser Project_x0020_Manager { get; set; }
        public SPLookup Market { get; set; }
        [Display(Name = "Status")]
        public string ObjectStatus { get; set; }
    }
}
