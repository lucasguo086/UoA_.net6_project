using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class GameRecord
    {
        [Key]
        public int ID { set; get; }
        public string GameId { set; get; }
        public string State { set; get; }
        public string Player1 { set; get; }
        public string? Player2 { set; get; }
        public string? LastMovePlayer1 { set; get; }
        public string? LastMovePlayer2 { set; get; }

    }
}
