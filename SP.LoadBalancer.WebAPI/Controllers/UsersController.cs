using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SP.LoadBalancer.WebAPI.Models;
using System.Web.Http.Description;
using SP.LoadBalancer.WebAPI.Services;
namespace SP.LoadBalancer.WebAPI.Controllers
{

    [RoutePrefix("api/users")]
    [Authorize]
    public class UsersController : ApiController
    {
        [Route("users")]
        [HttpGet]
        [ResponseType(typeof(List<UserModel>))]
        public virtual IHttpActionResult GetLinksStates()
        {
            var userService = new UserService();
            var items = userService.GetUsers();

            return new ContentActionResult(HttpStatusCode.OK, "", items, Request);
        }
    }
}
