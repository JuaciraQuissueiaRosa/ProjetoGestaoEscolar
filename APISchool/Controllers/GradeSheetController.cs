using APISchool;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolAPI.Controllers
{
    [RoutePrefix("api/gradesheet")]
    public class GradeSheetController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString"].ConnectionString);
        // GET: api/Pautas
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Pautas/5
        [HttpGet]
        [Route("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Pautas
        [HttpPost]
        [Route("")]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Pautas/5
        [HttpPut]
        [Route("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Pautas/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
        }
    }
}
