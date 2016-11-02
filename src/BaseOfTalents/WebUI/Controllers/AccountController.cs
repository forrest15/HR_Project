using System;
using System.Web.Http;
using DAL.DTO;
using DAL.Services;
using Domain.Entities;
using Mailer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebUI.Auth.Infrastructure;
using WebUI.Extensions;
using WebUI.Models;
using WebUI.Results;
using WebUI.Services;

namespace WebUI.Controllers
{
    /// <summary>
    /// Controller of user actions, like registration, login/logout
    /// </summary>
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private IAccountService _userAccountService;
        private BaseService<MailContent, MailDTO> _mailService;
        private TemplateService _templateService;

        private static JsonSerializerSettings botSerializationSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public AccountController(IAccountService userAccountService, BaseService<MailContent, MailDTO> mailService,
            TemplateService templateService)
        {
            _userAccountService = userAuthService;
            _mailService = mailService;
            _templateService = templateService;
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
                return logedOut ? Unauthorized() : BadRequest() as IHttpActionResult;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        // POST api/<controller>
        [HttpPost, Authorize]
        [Route("invite")]
        public IHttpActionResult Register([FromBody]RegistrationModel newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mailContent = _mailService.Get(newUser.MailId);
            var template = _templateService.GetTemplate();

            newUser.GeneratePassword();
            var addedUser = _service.Add(newUser);

            var textAfterReplacing = MailBodyContentReplacer.Replace(mailContent.Body, addedUser.Login, addedUser.Password);
            var mail = MailTemplateGenerator.Generate(template, mailContent.Invitation, textAfterReplacing, mailContent.Farewell, mailContent.Subject,
                Globals.SettingsContext.Instance.GetImageUrl(), Globals.SettingsContext.Instance.GetOuterUrl());

            MailAgent.Send(addedUser.Email, mail.Subject, mail.Template);
            return Json(addedUser, botSerializationSettings);
        }

        /// <summary>
        /// Api for changing user password
        /// </summary>
        [HttpPost, Authorize]
        [Route("password")]
        public IHttpActionResult ChangePassword([FromBody]ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _userAccountService.ChangePassword(ActionContext.Request.Headers.Authorization.Parameter, model.OldPassword, model.NewPassword);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return new ForbiddenResult(e.Message);
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