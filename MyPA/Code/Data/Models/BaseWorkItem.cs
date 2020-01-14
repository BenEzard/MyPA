using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPA.Code.Data.Models
{
    public class BaseWorkItem : BaseDBObject
    {
        public int? WorkItemID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }


    }
}
