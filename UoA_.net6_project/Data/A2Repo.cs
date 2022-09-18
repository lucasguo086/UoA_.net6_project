using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using A2.Models;

namespace A2.Data
{
    public class A2Repo : IA2Repo
    {
        private readonly A2DBContext _dbContext;

        public A2Repo(A2DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public User Register(string UserName, string Password, string Address)
        {
            User user = new User();
            user.UserName = UserName;
            user.Password = Password;
            user.Address = Address;
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return user;
        }
        public bool IsRegistered(string UserName)
        {
            if (_dbContext.Users.Any(o => o.UserName == UserName))
            {
                return true;
            }
            else { return false; }
        }

        //该方法在MyAuthHandler使用
        public bool ValidLogin(string UserName, string Password)
        {
            if (_dbContext.Users.Any(o => (o.UserName == UserName && o.Password == Password)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Order PurchaseItem(string UserName, int ProductId)
        {
            Order order = new Order();
            order.UserName = UserName;
            order.ProductID = ProductId;
            //_dbContext.Orders.Add(order);
            //_dbContext.SaveChanges();
            return order;
        }


        public GameRecord FindWaitRecord()
        {
            GameRecord gameRecord = _dbContext.GameRecords.FirstOrDefault(o => o.State == "wait");
            return gameRecord;


            
        }

        public bool HaveWaitRecord()
        {
            if(_dbContext.GameRecords.Any(o => (o.State == "wait")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public GameRecord UpdateWaitRecord(string UserName)
        {
            GameRecord gameRecord = _dbContext.GameRecords.FirstOrDefault(o => o.State == "wait");
            gameRecord.State = "progress";
            gameRecord.Player2 = UserName;
            _dbContext.GameRecords.Update(gameRecord);
            _dbContext.SaveChanges();
            return gameRecord;
        }
        public GameRecord NewWaitRecord(string UserName)
        {
            GameRecord gameRecord = new GameRecord();
            gameRecord.GameId = System.Guid.NewGuid().ToString();
            gameRecord.State = "wait";
            gameRecord.Player1 = UserName;
            _dbContext.GameRecords.Add(gameRecord);
            _dbContext.SaveChanges();
            return gameRecord;
        }

        public GameRecord GetGameRecordByGameID(string GameID)
        {
            GameRecord gameRecord = _dbContext.GameRecords.FirstOrDefault(o => o.GameId == GameID);
            return gameRecord;
        }

        public GameRecord MakeAMove(string gameID, string move, string userName)
        {
            GameRecord gameRecord = _dbContext.GameRecords.FirstOrDefault(o => o.GameId == gameID);
            if (gameRecord.Player1 == userName)
            {
                gameRecord.LastMovePlayer1 = move;
                gameRecord.LastMovePlayer2 = null;
            }
            else
            {
                gameRecord.LastMovePlayer2 = move;
                gameRecord.LastMovePlayer1 = null;
            }
            _dbContext.GameRecords.Update(gameRecord);
            _dbContext.SaveChanges();
            return gameRecord;
        }

        public bool UserInGame(string username)
        {
            GameRecord gameRecord = _dbContext.GameRecords.FirstOrDefault(o => o.Player1 == username || o.Player2 == username);
            if(gameRecord == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void GameOver(string gameID)
        {
            GameRecord gameRecord = _dbContext.GameRecords.FirstOrDefault(o => o.GameId == gameID);
            _dbContext.GameRecords.Remove(gameRecord);
            _dbContext.SaveChanges();
        }
    }
}
