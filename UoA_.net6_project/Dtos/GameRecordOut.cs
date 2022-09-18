using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace A2.Dto
{
    public class GameRecordOut
    {
        public string gameID { get; set; }
        public string state { get; set; }
        public string player1 { get; set; }
        public string player2 { get; set; }
        public string lastMovePlayer1 { get; set; }
        public string lastMovePlayer2 { get; set; }
    }
}
