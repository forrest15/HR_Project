using System;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebUI.Infrastructure.Auth;
using WebUI.Models;

namespace WebUI.Controllers
{
    /// <summary>
    /// Controller of user actions, like registration, login/logout
    /// </summary>
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private IAccountService _userAccountService;
        private static JsonSerializerSettings botSerializationSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public AccountController(IAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        /// <summary>
        /// Logs user out of the application.
        /// Deletes session.
        /// </summary>
        /// <returns>Success or unsuccess</returns>
        [HttpPost, Authorize]
        [Route("logout")]
        public IHttpActionResult Logout()
        {
            try
            {
                bool logedOut = _userAccountService
                    .LogOut(ActionContext.Request.Headers.Authorization.Parameter);
                return Json(logedOut, botSerializationSettings);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        /// <summary>
        /// Api for getting corect user with session
        /// </summary>
        /// <param name="identity">the parameter for identifiing user</param>
        /// <returns>Full user info</returns>
        [HttpGet, Authorize]
        [Route("")]
        public IHttpActionResult Get()
        {
            try
            {
                var user = _userAccountService.GetUser(ActionContext.Request.Headers.Authorization.Parameter);
                var result = new
                {
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    RoleId = user.RoleId,
                    Photo = user.Photo,
                    BirthDate = user.BirthDate,
                    CreatedOn = user.CreatedOn,
                    Login = user.Login,
                    Email = user.Email,
                    Skype = user.Skype,
                    PhoneNumbers = user.PhoneNumbers,
                    IsMale = user.isMale,
                    CityId = user.CityId,
                    Id = user.Id
                };

                return Json(result, botSerializationSettings);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Api for changing user password
        /// </summary>
        [HttpPost, Authorize]
        [Route("password")]
        public HttpResponseMessage ChangePassword([FromBody]ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, ModelState);
            }
            try
            {
                _userAccountService.ChangePassword(ActionContext.Request.Headers.Authorization.Parameter, model.OldPassword, model.NewPassword);
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }
            catch (ArgumentException e)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.Forbidden, e.Message);
            }
        }

        /// <summary>
        /// Api for recovering accounts
        /// </summary>
        [HttpPost, AllowAnonymous]
        [Route("recover")]
        public HttpResponseMessage RecoverAccount([FromBody]string loginOrEmail)
        {
            try
            {
                _userAccountService.RecoverAccount(loginOrEmail);
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.Forbidden, e.Message);
            }
        }
    }
}