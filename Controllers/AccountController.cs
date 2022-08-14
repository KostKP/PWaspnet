using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using aspnetapp.Helpers;
using aspnetapp.Entities;
using Microsoft.AspNetCore.Authorization;
using aspnetapp.Utilities;



namespace aspnetapp.Controllers
{
    [ApiController, Route("[controller]")]
    public class AccountController : ControllerBase
    {

        private IConfiguration configuration;
        private readonly DataContext context;

        public AccountController(IConfiguration config, DataContext context)
        {
            this.context = context;
            this.configuration = config;
        }


        /// <summary>
        /// Create new user account.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Code 202/400/500</returns>
        /// <response code="202">Account created successfully</response>
        /// <response code="400">Can't create account with given credentials</response>     
        [HttpPost, Route("register"), Produces("application/json")]
        public async Task<IActionResult> Register([FromBody] Models.AccountRegisterValidator data)
        {
            if (data.Email != null && data.Password != null)
            {
                try {
                    var user = new User(data.Email, "USER", PasswordUtility.getHash(data.Password), DateTime.UtcNow);
                    addNewUser(user);
                    return Accepted();
                } catch {} {
                    return BadRequest();
                }
            }
            return Problem();
        }


        /// <summary>
        /// Remove user account.
        /// </summary>
        /// <returns>Code 200/401/500</returns>
        /// <response code="200">Account removed successfully</response>
        /// <response code="401">Can't remove account with given credentials</response>   
        [Authorize, HttpPost, Route("remove"), Produces("application/json")]
        public async Task<IActionResult> Remove([FromBody] Models.AccountRemoveValidator data)
        {
            try {
                string email = User.FindFirst(ClaimTypes.Email).Value;
                var user =  Entities.User.find(context, email);
                if (user.Password == PasswordUtility.getHash(data.Password)) {
                    removeUser(user);
                    return Ok();
                }
                else {
                    return Unauthorized();
                }
            } catch (Exception e) {
                return Problem();
            }
        }

        private void addNewUser(User user) {
            context.Add(user);
            context.SaveChanges();
        }

        private void removeUser(User user) {
            context.Users.Attach(user);
            context.Users.Remove(user);
            context.SaveChanges();
        }
    }
}
