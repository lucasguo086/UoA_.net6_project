using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using A2.Models;

namespace A2.Data
{
    public interface IA2Repo
    {
        User Register(string UserName, string Password, string Address);
        bool IsRegistered(string UserName);
        bool ValidLogin(string UserName, string Password);
        Order PurchaseItem(string UserName, int ProductId);
        GameRecord FindWaitRecord();
        bool HaveWaitRecord();
        GameRecord UpdateWaitRecord(string UserName);
        GameRecord NewWaitRecord(string UserName);
        GameRecord GetGameRecordByGameID(string GameID);
        GameRecord MakeAMove(string gameID, string move, string userName);
        bool UserInGame(string username);
        void GameOver(string gameID);
    }
}
