using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Builder;
using System.Net;
using NTDOY_Microservice.Models;

namespace NTDOY_MicroService.Middleware
{
    public class QuoteMiddleware
    {
        private readonly RequestDelegate _next;

        public QuoteMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        //attempts to get stock information about NTDOY
        //short circuits the pipeline if this fails since
        //almost all endpoints rely on this information
        public async Task InvokeAsync(HttpContext context)
        {
            const string url = "https://sandbox.tradier.com/v1/markets/quotes?symbols=NTDOY";
            string token = Environment.GetEnvironmentVariable("tradier");

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", "Bearer " + token);
                var response = request.GetResponse() as HttpWebResponse;

                //attach tradier response to headers
                context.Items["WebResponse"] = response;

                await _next(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                context.Response.StatusCode = 400;
                await context.Response.Body.WriteAsync(Encoding.ASCII.GetBytes("Could not get stock information from Tradier."));
                //create transaction_log object that holds full info about this transaction
                TransactionLog log = new TransactionLog
                {
                    Type = "Failed Stock Lookup",
                    Username = ((User)context.Items["User"]).Username
                };
                context.Items["Log"] = log; //save log to be logged in middleware
            }

        }
    }

    public static class QuoteMiddlewareExtensions
    {
        public static IApplicationBuilder UseQuoteMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<QuoteMiddleware>();
        }
    }
}
