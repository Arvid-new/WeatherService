using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WeatherService.Entities;

namespace WeatherService.Security
{
    public class UserService
    {
        private readonly SecurityConfig SecurityConfig;

        public UserService(IOptions<SecurityConfig> securityConfig)
        {
            SecurityConfig = securityConfig.Value;
        }

        public Task<User> AuthenticateAsync(string username, string password)
        {
            return Task.Run(() =>
            {
                var user = SecurityConfig.Users.FirstOrDefault(x => x.Username == username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                    return null;

                // Authentication successful so generate jwt token.
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(SecurityConfig.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.Token = tokenHandler.WriteToken(token);

                return user;
            });
        }
    }
}
