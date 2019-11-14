using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Builder;
using NTDOY_Microservice;
using NTDOY_Microservice.Models;
using System.Data;

namespace NTDOY_MicroService.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        //checks for database connection
        //logs request to database once the token has been verified
        //and the request has gone to an endpoint
        public async Task InvokeAsync(HttpContext context)
        {
            //check if DB is connected and try to connect if it isnt
            if (DB_Connection.conn == null || DB_Connection.conn.State != ConnectionState.Open)
            {
                DB_Connection.Connect();
            }

            //check for db connection
            if(DB_Connection.conn == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.Body.WriteAsync(Encoding.ASCII.GetBytes("No database connection"));
                return;
            }

            await _next(context);   //wait for request to finish and come back with log information

            //log the results of this request
            var log = (TransactionLog)context.Items["Log"];
            log.RequestType = context.Request.Method;
            log.Origin = context.Request.Host.Value;
            log.LogToDatabase();
        }
    }

    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
