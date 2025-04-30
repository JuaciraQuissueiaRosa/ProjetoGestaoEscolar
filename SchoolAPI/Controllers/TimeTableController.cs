using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolAPI.Controllers
{
    [RoutePrefix("api/timetable")]
    public class TimeTableController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString1"].ConnectionString);
        // GET:  api/timetable
        [HttpGet]
        public IHttpActionResult Get()
        {

            var timetables = db.Timetables.ToList();
            if (!timetables.Any())
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, $"No timetables found."));
            return Ok(timetables);
        }

        // GET:  api/timetable/5
        [HttpGet]
        [Route("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST:  api/timetable
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/timetable/5
        [HttpPut]
        [Route("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE:  api/timetable/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
        }
    }
}
