using System;
//using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using NTDOY_Microservice.Models;
using NTDOY_MicroService;

namespace NTDOY_Microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellController : ControllerBase
    {
        [HttpPost]
        public async void SellStock()
        {
            try
            {
                //get the query params
                string account;
                int quantity;
                Request.Query.TryGetValue("quantity", out StringValues quan);
                Request.Query.TryGetValue("account", out StringValues acc);

                //parse out the price
                var response = (HttpWebResponse)HttpContext.Items["WebResponse"];
                var user = (User)HttpContext.Items["User"];
                var memoryStream = new MemoryStream();
                response.GetResponseStream().CopyTo(memoryStream);
                string json = Encoding.ASCII.GetString(memoryStream.ToArray());  //turn json into string to be readable
                float price = TransactionLog.GetPriceFromJson(json);

                if (quan.Count == 1 && acc.Count == 1)
                {
                    quantity = Int32.Parse(quan[0]);
                    account = acc[0];
                }
                else
                {
                    //headers not assigned properly so fail the sell operation
                    Response.StatusCode = 400;
                    await Response.Body.WriteAsync(Encoding.ASCII.GetBytes("Could not complete sell operation."));
                    TransactionLog l = new TransactionLog
                    {
                        Type = "Failed Sell",
                        Username = user.Username
                    };
                    HttpContext.Items["Log"] = l;
                    return;
                }

                //create the sale object
                BuySell sell = new BuySell
                {
                    Username = user.Username,
                    Account = account,
                    Price = price,
                    Quantity = quantity
                };

                //check if user has enough stock in this account to sell the listed quantity
                //also checks if the quantity they want to sell is greater than 0
                int userStock = sell.UserAccountStock();
                if(userStock - sell.Quantity < 0 || sell.Quantity <= 0)
                {
                    //fail the sell
                    Response.StatusCode = 400;
                    await Response.Body.WriteAsync(Encoding.ASCII.GetBytes("Could not complete sell operation."));
                    TransactionLog l = new TransactionLog
                    {
                        Type = "Failed Sell",
                        Account = account,
                        Price = price,
                        Quantity = quantity,
                        Username = user.Username
                    };
                    HttpContext.Items["Log"] = l;
                    return;
                }

                //log the sale since they have enough to stocks to sell to the bank
                int id = sell.LogSell();

                if (id == -1)
                {
                    Response.StatusCode = 400;
                    await Response.Body.WriteAsync(Encoding.ASCII.GetBytes("Could not complete sell operation."));
                    TransactionLog l = new TransactionLog
                    {
                        Type = "Failed Sell",
                        Account = account,
                        Quantity = quantity,
                        Username = user.Username,
                        Price = price
                    };
                    HttpContext.Items["Log"] = l; //save log to be logged in middleware
                    return;
                }
                else
                {
                    //respond with a json of the sell info
                    var cols = new
                    {
                        TransactionType = "SELL",
                        User = user.Username,
                        Account = account,
                        Price = price,
                        Quantity = quantity,
                        EarningsForUser = price * quantity
                    };
                    HttpContext.Response.ContentType = "application/json";
                    await HttpContext.Response.Body.WriteAsync(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(cols)));
                }

                //(Part2)
                //TODO add the sell cost to the users account
                //if it fails undo the sell at id and also fail the sell else continue to log

                //create transaction_log object that holds full info about this transaction
                TransactionLog log = new TransactionLog
                {
                    Type = "Sell",
                    Account = account,
                    Quantity = quantity,
                    Username = user.Username,
                    Price = price
                };
                HttpContext.Items["Log"] = log; //save log to be logged in middleware
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Response.StatusCode = 400;
                await Response.Body.WriteAsync(Encoding.ASCII.GetBytes("Could not complete Sell operation."));
                //create transaction_log object that holds full info about this transaction
                TransactionLog log = new TransactionLog
                {
                    Type = "Failed Sell",
                    Username = ((User)HttpContext.Items["User"]).Username
                };
                HttpContext.Items["Log"] = log; //save log to be logged in middleware
            }
        }

    }
}
