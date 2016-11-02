﻿using System;
using System.Web.Http;
using DAL.DTO;
using DAL.Exceptions;
using DAL.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebUI.Auth;
using WebUI.Models;

namespace WebUI.Controllers
{
    [RoutePrefix("api/candidate")]
    public class CandidateController : ApiController
    {
        private CandidateService service;
        private static JsonSerializerSettings BOT_SERIALIZER_SETTINGS = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        public CandidateController(CandidateService service)
        {
            this.service = service;
        }

        [HttpGet, Authorize]
        public IHttpActionResult Get()
        {
            return this.Get(new CandidateSearchModel());
        }

        // GET api/<controller>
        [HttpPost, Authorize]
        [Route("search")]
        public IHttpActionResult Get([FromBody]CandidateSearchModel searchParameters)
        {
            if (ModelState.IsValid)
            {
                var tupleResult = service.Get(
                    searchParameters.FirstName,
                    searchParameters.LastName,
                    searchParameters.RelocationAgreement,
                    searchParameters.IsMale,
                    searchParameters.MinAge,
                    searchParameters.MaxAge,
                    searchParameters.StartExperience,
                    searchParameters.MinSalary,
                    searchParameters.MaxSalary,
                    searchParameters.CurrencyId,
                    searchParameters.IndustryId,
                    searchParameters.Position,
                    searchParameters.Technology,
                    searchParameters.LanguageSkills,
                    searchParameters.CitiesIds,
                    searchParameters.Current,
                    searchParameters.Size,
                    searchParameters.SortBy,
                    searchParameters.SortAsc,
                    searchParameters.SearchString
                    );

                var candidatesQuerryResult = tupleResult.Item1;
                var total = tupleResult.Item2;

                var ret = new
                {
                    Candidate = candidatesQuerryResult,
                    Current = searchParameters.Current,
                    Size = searchParameters.Size,
                    Total = total,
                    SortBy = searchParameters.SortBy,
                    SortAsc = searchParameters.SortAsc
                };
                return Json(ret, BOT_SERIALIZER_SETTINGS);
            }
            return BadRequest(ModelState);
        }

        // GET api/<controller>/5
        [HttpGet, Authorize]
        [Route("{id:int}")]
        public IHttpActionResult Get(int id)
        {
            var foundedEntity = service.Get(id);
            if (foundedEntity == null)
            {
                ModelState.AddModelError("Candidate", "Candidate with id " + id + " not founded.");
                return BadRequest(ModelState);
            }
            return Json(foundedEntity, BOT_SERIALIZER_SETTINGS);
        }

        // POST api/<controller>
        [HttpPost, Authorize]
        [Route("")]
        public IHttpActionResult Post([FromBody]CandidateDTO newCandidate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var id = PayloadDecoder.TryGetId(ActionContext.Request.Headers.Authorization.Parameter);
            var addedCandidate = service.Add(newCandidate, id);
            return Json(addedCandidate, BOT_SERIALIZER_SETTINGS);
        }

        // PUT api/<controller>/5
        [HttpPut, Authorize]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody]CandidateDTO changedCandidate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int userId = PayloadDecoder.TryGetId(ActionContext.Request.Headers.Authorization.Parameter);
            var updatedCandidate = service.Update(changedCandidate, userId);
            return Json(updatedCandidate, BOT_SERIALIZER_SETTINGS);
        }

        // DELETE api/<controller>/5
        [HttpDelete, Authorize]
        [Route("{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                service.Delete(id);
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpGet, Authorize]
        [Route("duplicates")]
        public IHttpActionResult Get([FromUri]CandidateDTO patternCandidate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var duplicates = service.GetDuplicates(patternCandidate);
            return Json(duplicates, BOT_SERIALIZER_SETTINGS);
        }
    }
}
