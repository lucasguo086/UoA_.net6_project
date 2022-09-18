using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class GameMove
    {
        [Key]
        public string gameId { set; get; }
        public string move { set; get; }

    }
}
