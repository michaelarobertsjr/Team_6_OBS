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
    public class BuyController : ControllerBase
    {
        [HttpPost]
        public async void BuyStock()
        {
            try
            {
                //get the query params
                string accnt;
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
                    accnt = acc[0];
                }
                else
                {
                    //headers not assigned properly so fail the buy operation
                    Response.StatusCode = 400;
                    await Response.Body.WriteAsync(Encoding.ASCII.GetBytes("Could not complete buy operation."));
                    TransactionLog l = new TransactionLog
                    {
                        Type = "Failed Buy",
                        Username = user.Username
                    };
                    HttpContext.Items["Log"] = l;
                    return;
                }


                //(Part 2)
                //TODO check if user has enough money for price * quantity in the listed account
                //find account current money and see if its less than cost of this buy
                //if it costs more than they have fail the buy else continue

                //make sure quantity is positive
                if(quantity <= 0)
                {
                    //fail the buy
                    Response.StatusCode = 400;
                    await Response.Body.WriteAsync(Encoding.ASCII.GetBytes("Could not complete buy operation."));
                    TransactionLog l = new TransactionLog
                    {
                        Type = "Failed Buy",
                        Account = accnt,
                        Quantity = quantity,
                        Username = user.Username,
                        Price = price
                    };
                    HttpContext.Items["Log"] = l; //save log to be logged in middleware
                    return;
                }

                //check if bank has enough stocks to cover this, if they dont purchase difference plus 5000
                int bankStocks = BuySell.BankStock();
                int stocksLeft = bankStocks - quantity;
                if(stocksLeft <= 0)
                {
                    //bank needs to purchase enough to cover purchase plus 5K
                    int purchase = stocksLeft * -1 + 5000;
                    bool success = BuySell.BankPurchase(purchase, price);
                    //fail the buy if the bank couldnt purchase stock
                    if (!success)
                    {
                        //fail the buy
                        Response.StatusCode = 400;
                        await Response.Body.WriteAsync(Encoding.ASCII.GetBytes("Could not complete buy operation."));
                        TransactionLog l = new TransactionLog
                        {
                            Type = "Failed Buy",
                            Account = accnt,
                            Quantity = quantity,
                            Username = user.Username,
                            Price = price
                        };
                        HttpContext.Items["Log"] = l; //save log to be logged in middleware
                        return;
                    }
                }

                //log the purchase in the buy table
                BuySell buy = new BuySell
                {
                    Username = user.Username,
                    Account = accnt,
                    Price = price,
                    Quantity = quantity
                };
                int id = buy.LogBuy();

                if(id == -1) {
                    Response.StatusCode = 400;
                    await Response.Body.WriteAsync(Encoding.ASCII.GetBytes("Could not complete buy operation."));
                    TransactionLog l = new TransactionLog
                    {
                        Type = "Failed Buy",
                        Account = accnt,
                        Quantity = quantity,
                        Username = user.Username,
                        Price = price
                    };
                    HttpContext.Items["Log"] = l; //save log to be logged in middleware
                    return;
                }
                else
                {
                    //respond with a json of the buy info
                    var cols = new
                    {
                        TransactionType = "BUY",
                        User = user.Username,
                        Account = accnt,
                        Price = price,
                        Quantity = quantity,
                        CostToUser = price * quantity
                    };
                    HttpContext.Response.ContentType = "application/json";
                    await HttpContext.Response.Body.WriteAsync(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(cols)));
                }

                //(Part 2)
                //TODO subtract the price from money in users account

                //create transaction_log object that holds full info about this transaction
                TransactionLog log = new TransactionLog
                {
                    Type = "Buy",
                    Account = accnt,
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
                await Response.Body.WriteAsync(Encoding.ASCII.GetBytes("Could not complete buy operation."));
                //create transaction_log object that holds full info about this transaction
                TransactionLog log = new TransactionLog
                {
                    Type = "Failed Buy",
                    Username = ((User)HttpContext.Items["User"]).Username
                };
                HttpContext.Items["Log"] = log; //save log to be logged in middleware
            }
        }

    }
}
