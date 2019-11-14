using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NTDOY_MicroService.Models
{

    public class TokenManager
    {
        private static string Secret = Environment.GetEnvironmentVariable("secret");

        //return the user that made the request or null if not validated
        public static User ValidateToken(string token)
        {
            User user = new User();

            //validation setup
            var key = Encoding.ASCII.GetBytes(Secret);
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            try
            {
                //validate the claims and loop
                var claims = handler.ValidateToken(token, validations, out var tokenSecure);
                if (claims != null)
                {
                    foreach (Claim claim in claims.Claims)
                    {
                        //decode claim type into relevent user information
                        if (claim.Type == "Id")
                            user.Id = Int64.Parse(claim.Value);
                        else if (claim.Type == "Username")
                            user.Username = claim.Value;
                        else if (claim.Type == "Email")
                            user.Email = claim.Value;
                    }
                }

                return user;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }
    }

}
