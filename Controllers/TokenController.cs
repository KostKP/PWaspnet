using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using aspnetapp.Helpers;
using aspnetapp.Entities;
using aspnetapp.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace aspnetapp.Controllers
{
    [ApiController, Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private IConfiguration configuration;
        private readonly DataContext context;

        public TokenController(IConfiguration config, DataContext context)
        {
            this.context = context;
            this.configuration = config;
        }

        /// <summary>
        /// Create acces token.
        /// </summary>
        /// <returns>Code 202/401/500</returns>
        /// <response code="202">Token generated successfully</response>
        /// <response code="401">Can't create token with given credentials</response>   
        [HttpPost, Route("create"), Produces("application/json")]
        public async Task<IActionResult> Generate([FromBody] Models.TokenCreateValidator data)
        {
            try {
                var user = Entities.User.find(context, data.Email, PasswordUtility.getHash(data.Password));
                if (user != null) {
                        var newToken = TokenGen(user.Email);
                        string jti = newToken.Claims.First(claim => claim.Type == "jti").Value;
                        storeToken(user, Guid.Parse(jti));
                        return Accepted(
                            new {
                                    token = new JwtSecurityTokenHandler().WriteToken(newToken),
                                    id = jti
                                });
                    }
                else
                    {
                        return Unauthorized();
                    }
                } catch {
                    {
                        return Problem();
                    }
                }
        }

        /// <summary>
        /// ReCreate acces token.
        /// </summary>
        /// <returns>Code 202/400/500</returns>
        /// <response code="202">Token generated successfully</response>
        /// <response code="400">Can't create token with given id</response>   
        [HttpPost, Route("recreate"), Produces("application/json")]
        public async Task<IActionResult> Reissue([FromBody] Models.TokenReCreateValidator data)
        {
            try {
                Token? token = Entities.Token.find(context, Guid.Parse(data.Id), true);
                if (token != null) {
                    if  (token.IsBanned) return BadRequest(
                        new {
                            message = "This token is banned!"
                        }
                    );
                    if  (DateTime.UtcNow-token.CreatedAt > TimeSpan.FromDays(7)) return BadRequest(
                        new {
                            message = "Данный токен создал слишком давно чтобы быть пересозданным"
                        }
                    );
                    token.IsBanned = true;
                    context.SaveChanges();
                    var newToken = TokenGen(token.UserEmail);
                    string jti = newToken.Claims.First(claim => claim.Type == "jti").Value;
                    storeToken(token.User, Guid.Parse(jti));
                    return Accepted(
                        new {
                            token = new JwtSecurityTokenHandler().WriteToken(newToken),
                            id = jti
                        });
                    }
                else
                    {
                        return BadRequest();
                    }
                } catch {
                    {
                        return Problem();
                    }
                }
        }

        private JwtSecurityToken TokenGen(string email) {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.Email, email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddSeconds(configuration.GetValue<int>("Jwt:Lifetime")),
                signingCredentials: signIn);
            return token;
        }
        private void storeToken(User user, Guid tokenID) {
            context.Tokens.Add(new Token(user, tokenID));
            context.SaveChanges();
        }
    }
}
