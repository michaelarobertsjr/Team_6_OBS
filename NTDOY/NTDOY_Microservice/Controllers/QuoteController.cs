using Microsoft.AspNetCore.Mvc;
using System.Net;
using System;
using System.IO;
using System.Text;
using NTDOY_Microservice.Models;

namespace NTDOY_MicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteController : ControllerBase
    {
        //gets NTDOY stock information from tradier
        [HttpGet]
        public async void GetStockQuote()
        {
            try
            {
                //send tradier response to user
                var response = (HttpWebResponse)HttpContext.Items["WebResponse"];
                var user = (User)HttpContext.Items["User"];

                var memoryStream = new MemoryStream();
                response.GetResponseStream().CopyTo(memoryStream);
                Response.ContentType = "application/json";
                byte[] r = memoryStream.ToArray();
                Response.StatusCode = 200;
                await Response.Body.WriteAsync(r);

                //create transaction_log object that holds full info about this transaction
                TransactionLog log = new TransactionLog
                {
                    Type = "Quote",
                    Username = user.Username
                };
                //get the price from the json response of stock data
                string json = Encoding.ASCII.GetString(r);  //turn json into string to be readable
                log.Price = TransactionLog.GetPriceFromJson(json);     //parse the price into a double to be saved
                HttpContext.Items["Log"] = log; //save log to be logged in middleware
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Response.StatusCode = 400;
                await Response.Body.WriteAsync(Encoding.ASCII.GetBytes("Could not get stock information from Tradier."));
                //create transaction_log object that holds full info about this transaction
                TransactionLog log = new TransactionLog
                {
                    Type = "Failed Stock Lookup",
                    Username = ((User)HttpContext.Items["User"]).Username
                };
                HttpContext.Items["Log"] = log; //save log to be logged in middleware
            }
        }

    }
}