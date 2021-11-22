using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Backend.Models
{
    public class ItemNestable
    {
        public long id { get; set; }
        public List<ItemNestable> children { get; set; }
    }
}