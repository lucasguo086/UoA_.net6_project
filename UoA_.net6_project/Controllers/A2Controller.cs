using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using A2.Data;
using A2.Models;
using A2.Dto;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace A2.Controllers
{
    [Route("api")]
    [ApiController]
    public class A2Controller : Controller
    {
        private readonly IA2Repo _repository;

        public A2Controller(IA2Repo repository)
        {
            _repository = repository;
        }

        [HttpPost("Register")]
        public ActionResult Register(User user)
        {
            if (_repository.IsRegistered(user.UserName))
            {
                return Ok("Username not available.");
            }
            else
            {
                _repository.Register(user.UserName, user.Password, user.Address);
                return Ok("User successfully registered.");
            }
        }

        [Authorize(AuthenticationSchemes = "MyAuthentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("GetVersionA")]
        public ActionResult GetVersionA()
        {
            //claimsidentity object consists of one or several claim objects
            //claimsIdentity: claim (key:value)
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            // User is the claim, set by handler, var claims = new[] { new Claim("user", username) };
            // program.cs also 
            Claim c = ci.FindFirst("User");
            string username = c.Value;

            Console.WriteLine(username);

            //return Ok("1.0.0 (auth)");
            return Ok(username);
        }

        [Authorize(AuthenticationSchemes = "MyAuthentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("PurchaseItem/{productID}")]
        public ActionResult PurchaseItem(int productID)
        {

            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("User");
            string username = c.Value;
            Order order = _repository.PurchaseItem(username, productID);
            return Ok(order);

        }

        [Authorize(AuthenticationSchemes = "MyAuthentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("PairMe")]
        public ActionResult PaireMe()
        {

            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("User");
            string username = c.Value;
            //return Ok(username);
            //System.Diagnostics.Debug.WriteLine("==========");
            //Console.WriteLine(username);
            //??????gamerecord, ??????????????????wait,???wait?????????
            //??????wait???player1????????????username, ???????????????return
            //??????wait???player1???????????????username, ????????????, return ???wait???????????????
            //????????????wait, ????????????gamerecord
            //????????????gamerecordout

            if (_repository.HaveWaitRecord()) //???wait
            {
                GameRecord gamerecord = _repository.FindWaitRecord();
                if (gamerecord.Player1 == username) //wait??????????????????
                {
                    GameRecordOut gout = new GameRecordOut
                    {
                        gameID = gamerecord.GameId,
                        state = gamerecord.State,
                        player1 = gamerecord.Player1
                    };
                    return Ok(gout);
                }
                else //wait?????????????????????
                {
                    GameRecord gameUpdate = _repository.UpdateWaitRecord(username);
                    GameRecordOut gout = new GameRecordOut
                    {
                        gameID = gameUpdate.GameId,
                        state = gameUpdate.State,
                        player1 = gameUpdate.Player1,
                        player2 = gameUpdate.Player2,
                        lastMovePlayer1 = gameUpdate.LastMovePlayer1,
                        lastMovePlayer2 = gameUpdate.LastMovePlayer2
                    };
                    return Ok(gout);
                }
            }
            else
            {
                GameRecord gamerecord = _repository.NewWaitRecord(username);
                GameRecordOut gout = new GameRecordOut
                {
                    gameID = gamerecord.GameId,
                    state = gamerecord.State,
                    player1 = gamerecord.Player1
                };
                return Ok(gout);
            }



        }

        [Authorize(AuthenticationSchemes = "MyAuthentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("TheirMove/{GameID}")]
        public ActionResult TheirMove(string GameID)
        {

            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("User");
            string username = c.Value;
            GameRecord gamerecord = _repository.GetGameRecordByGameID(GameID);
            if(gamerecord == null)
            {
                return Ok("no such gameId");
            }
            else
            {
                if((gamerecord.Player1 != username) &&(gamerecord.Player2 != username))
                {
                    return Ok("not your game id");
                }
                else if (gamerecord.Player1 == username && gamerecord.Player2 == null)
                {
                    return Ok("You do not have an opponent yet.");
                }
                else if (gamerecord.Player1 == username && gamerecord.LastMovePlayer2 == null)
                {
                    return Ok("Your opponent has not moved yet.");
                }
                else if (gamerecord.Player2 == username && gamerecord.LastMovePlayer1 == null)
                {
                    return Ok("Your opponent has not moved yet.");
                }
                else
                {
                    if(gamerecord.Player1 == username)
                    {
                        return Ok(gamerecord.LastMovePlayer2);
                    }
                    else
                    {
                        return Ok(gamerecord.LastMovePlayer1);
                    }
                }
            }
        }

        [Authorize(AuthenticationSchemes = "MyAuthentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpPost("MyMove")]
        public ActionResult MyMove(GameMove move)
        {

            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("User");
            string username = c.Value;
            GameRecord gamerecord = _repository.GetGameRecordByGameID(move.gameId);
            if (gamerecord == null)
            {
                return Ok("no such gameId");
            }
            else
            {
                if ((gamerecord.Player1 != username) && (gamerecord.Player2 != username))
                {
                    return Ok("not your game id");
                }
                else if (gamerecord.Player1 == username && gamerecord.Player2 == null)
                {
                    return Ok("You do not have an opponent yet.");
                }
                else if (gamerecord.Player1 == username && gamerecord.LastMovePlayer2 == null && gamerecord.LastMovePlayer1 != null)
                {
                    return Ok("It is not your turn.");
                }
                else if (gamerecord.Player2 == username && gamerecord.LastMovePlayer1 == null && gamerecord.LastMovePlayer2 != null)
                {
                    return Ok("It is not your turn.");
                }
                else
                {
                    _repository.MakeAMove(move.gameId, move.move, username);
                    return Ok("move registered");
                }
            }
                
            
        }

        [Authorize(AuthenticationSchemes = "MyAuthentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("QuitGame/{gameID}")]
        public ActionResult QuitGame(string gameID)
        {

            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("User");
            string username = c.Value;
            if(_repository.UserInGame(username)) //??????????????????
            {
                GameRecord gamerecord = _repository.GetGameRecordByGameID(gameID);
                if (gamerecord == null)  //????????????gameID
                {
                    return Ok("no such gameId");
                }
                else //?????????gameID
                {
                    if ((gamerecord.Player1 != username) && (gamerecord.Player2 != username))
                    {
                        return Ok("not your game id");
                    }
          
                    else
                    {
                        _repository.GameOver(gameID);
                        return Ok("game over");
                    }
                }

            }
            else //?????????????????????
            {
                return Ok("You have not started a game.");
            }


        }

    }
}
