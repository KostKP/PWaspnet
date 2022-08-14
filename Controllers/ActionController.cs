using System;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using aspnetapp.Helpers;
using aspnetapp.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace aspnetapp.Controllers
{
    [ApiController, Route("[controller]")]
    public class ActionController : ControllerBase
    {
        private IConfiguration configuration;
        private readonly DataContext context;

        public ActionController(IConfiguration config, DataContext context)
        {
            this.context = context;
            this.configuration = config;
        }

        /// <summary>
        /// Returns last action.
        /// </summary>
        /// <returns>Code 200/500</returns>
        /// <response code="200">Result</response>
        [Authorize, HttpGet, Route("last"), Produces("application/json")]
        public async Task<ActionResult> GetLast() {
            try {
                if (!TokenUtility.isTokenValid(context, User.FindFirst("jti").Value)) return BadRequest( new {
                    message = "Not valid token!"
                });
                string email = User.FindFirst(ClaimTypes.Email).Value;
                Entities.Action result = context.Actions.Where(a => a.UserEmail == email).AsEnumerable().LastOrDefault();
                return Ok( new {
                    date = result.Date,
                    action = result.ActionData
                });
            } catch {
                return Problem();
            }
        }

        /// <summary>
        /// Returns last week actions.
        /// </summary>
        /// <returns>Code 200/500</returns>
        /// <response code="200">Result</response>
        [Authorize, HttpGet, Route("week"), Produces("application/json")]
        public async Task<ActionResult> GetWeek() {
            try {
                if (!TokenUtility.isTokenValid(context, User.FindFirst("jti").Value)) return BadRequest( new {
                    message = "Not valid token!"
                });
                string email = User.FindFirst(ClaimTypes.Email).Value;

                DateTime fd = DateTime.Now.FirstDayOfWeek().ToUniversalTime();
                var result = context.Actions.Where(a => a.UserEmail == email && a.Date >= fd).AsEnumerable();
                List<Object> temp = new List<object>();
 
                foreach (var i in result) {
                    temp.Add(new {
                        date = i.Date,
                        weekday = i.Date.DayOfWeek,
                        action = i.ActionData
                    });
                }
                return Ok( new {
                    actions = temp
                });
            } catch { return Problem(); }
        }


        /// <summary>
        /// Store action.
        /// </summary>
        /// <returns>Code 200/500</returns>
        /// <response code="200">Can't save</response>
        [Authorize, HttpPost, Route("store"), Produces("application/json")]
        public async Task<ActionResult> StoreAction(Models.ActionRecordValidator data) {
            try {
                if (!TokenUtility.isTokenValid(context, User.FindFirst("jti").Value)) return BadRequest( new {
                    message = "Not valid token!"
                });
                string email = User.FindFirst(ClaimTypes.Email).Value;
                context.Actions.Add(new Entities.Action(Entities.User.find(context, email), DateTime.UtcNow, data.Action));
                context.SaveChanges();
                return Ok( new {
                    message="Action recorderd succesfuly!"
                });
            } catch (Exception e) {
                return Problem();
            }
        }
    }
}