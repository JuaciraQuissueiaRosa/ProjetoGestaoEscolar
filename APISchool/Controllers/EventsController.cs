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
    [RoutePrefix("api/events")]
    public class EventsController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString"].ConnectionString);

        // GET: api/events
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var events = db.Events.Select(e => new
            {
                e.Id,
                e.Name,
                e.Description,
                e.EventDate,
                e.Location,
                Students = e.EventParticipations
                             .Where(p => p.StudentId != null)
                             .Select(p => new { p.Student.Id, p.Student.FullName }).ToList(),
                Teachers = e.EventParticipations
                             .Where(p => p.TeacherId != null)
                             .Select(p => new { p.Teacher.Id, p.Teacher.FullName }).ToList()
            }).ToList();

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

        [HttpPost]
        [Route("{eventId}/associate-student/{studentId}")]
        public IHttpActionResult AssociateStudent(int eventId, int studentId)
        {
            if (!db.Events.Any(e => e.Id == eventId) || !db.Students.Any(s => s.Id == studentId))
                return NotFound();

            var exists = db.EventParticipations.Any(p => p.EventId == eventId && p.StudentId == studentId);
            if (exists)
                return BadRequest("This student is already associated with the event.");

            db.EventParticipations.InsertOnSubmit(new EventParticipation
            {
                EventId = eventId,
                StudentId = studentId
            });
            db.SubmitChanges();
            return Ok("Student associated with event successfully.");
        }

        [HttpPost]
        [Route("{eventId}/associate-teacher/{teacherId}")]
        public IHttpActionResult AssociateTeacher(int eventId, int teacherId)
        {
            if (!db.Events.Any(e => e.Id == eventId) || !db.Teachers.Any(t => t.Id == teacherId))
                return NotFound();

            var exists = db.EventParticipations.Any(p => p.EventId == eventId && p.TeacherId == teacherId);
            if (exists)
                return BadRequest("This teacher is already associated with the event.");

            db.EventParticipations.InsertOnSubmit(new EventParticipation
            {
                EventId = eventId,
                TeacherId = teacherId
            });
            db.SubmitChanges();
            return Ok("Teacher associated with event successfully.");
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
