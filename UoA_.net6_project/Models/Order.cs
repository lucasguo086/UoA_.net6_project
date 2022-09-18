using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class Order
    {
        [Key]
        public string UserName { set; get; }
        public int ProductID { set; get; }
    }
}
