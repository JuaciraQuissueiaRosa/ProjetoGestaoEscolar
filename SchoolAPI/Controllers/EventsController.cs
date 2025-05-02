using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolAPI.Controllers
{
    [RoutePrefix("api/events")]
    public class EventsController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString1"].ConnectionString);
        // GET: api/Eventos
        // GET: api/events
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var events = db.Events.ToList();
            return Ok(events);
        }

        // GET: api/events/5
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var ev = db.Events.FirstOrDefault(e => e.Id == id);
            if (ev == null)
                return NotFound();
            return Ok(ev);
        }

        // POST: api/events
        [HttpPost]
        public IHttpActionResult Create(Event ev)
        {
            if (ev == null)
                return BadRequest("Invalid event data.");

            db.Events.InsertOnSubmit(ev);
            db.SubmitChanges();

            return Ok(ev);
        }

        // PUT: api/events/5
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, Event updatedEvent)
        {
            var existing = db.Events.FirstOrDefault(e => e.Id == id);
            if (existing == null)
                return NotFound();

            existing.Name = updatedEvent.Name;
            existing.EventDate = updatedEvent.EventDate;
            existing.Location = updatedEvent.Location;
            existing.Description = updatedEvent.Description;

            db.SubmitChanges();
            return Ok(existing);
        }

        // DELETE: api/events/5
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var ev = db.Events.FirstOrDefault(e => e.Id == id);
            if (ev == null)
                return NotFound();

            db.Events.DeleteOnSubmit(ev);
            db.SubmitChanges();
            return Ok();
        }
    }
}
