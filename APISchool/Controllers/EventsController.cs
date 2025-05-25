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
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString2"].ConnectionString);


        /// <summary>
        /// Retrieves a list of all events, including associated students and teachers.
        /// </summary>
        /// <returns>List of events with participants.</returns>
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
                e.EventTime,
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
        /// <summary>
        /// Retrieves an event by its ID.
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <returns>The specified event</returns>
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var ev = db.Events.FirstOrDefault(e => e.Id == id);
            if (ev == null)
                return NotFound();
            return Ok(ev);
        }

        /// <summary>
        /// Creates a new event with the provided data.
        /// </summary>
        /// <param name="data">Event object</param>
        /// <returns>Created event</returns>
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] Event data)
        {
            if (data == null)
                return BadRequest("Invalid event data.");

            if (data.EventTime == default(TimeSpan))
                return BadRequest("Invalid or missing EventTime.");

            var newEvent = new Event
            {
                Name = data.Name,
                EventDate = data.EventDate,
                EventTime=data.EventTime,
                Location = data.Location,
                Description = data.Description
              
            };

            db.Events.InsertOnSubmit(newEvent);
            db.SubmitChanges();

            return Ok(newEvent); 
        }


        /// <summary>
        /// Associates a student with a specific event.
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <param name="studentId">Student ID</param>
        /// <returns>Status message</returns>
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


        /// <summary>
        /// Associates a teacher with a specific event.
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <param name="teacherId">Teacher ID</param>
        /// <returns>Status message</returns>
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
        /// <summary>
        /// Updates an existing event.
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <param name="updatedEvent">Updated event data</param>
        /// <returns>Updated event</returns>
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, Event updatedEvent)
        {
            if (updatedEvent == null)
                return BadRequest("Invalid data.");

            if (updatedEvent.EventTime == default(TimeSpan))
                return BadRequest("Event time is required and must be in HH:mm format.");

            var existing = db.Events.FirstOrDefault(e => e.Id == id);
            if (existing == null)
                return NotFound();

            // Atualiza os dados
            existing.Name = updatedEvent.Name;
            existing.EventDate = updatedEvent.EventDate;
            existing.EventTime = updatedEvent.EventTime;
            existing.Location = updatedEvent.Location;
            existing.Description = updatedEvent.Description;

            db.SubmitChanges();
            return Ok(existing);
        }

        /// <summary>
        /// Removes a student from a specific event.
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <param name="studentId">Student ID</param>
        /// <returns>Status message</returns>
        [HttpDelete]
        [Route("{eventId}/remove-student/{studentId}")]
        public IHttpActionResult RemoveStudentFromEvent(int eventId, int studentId)
        {
            var participation = db.EventParticipations
                .FirstOrDefault(p => p.EventId == eventId && p.StudentId == studentId);

            if (participation == null)
                return NotFound();

            db.EventParticipations.DeleteOnSubmit(participation);
            db.SubmitChanges();

            return Ok("Student removed from event.");
        }


        /// <summary>
        /// Removes a teacher from a specific event.
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <param name="teacherId">Teacher ID</param>
        /// <returns>Status message</returns>
        [HttpDelete]
        [Route("{eventId}/remove-teacher/{teacherId}")]
        public IHttpActionResult RemoveTeacherFromEvent(int eventId, int teacherId)
        {
            var participation = db.EventParticipations
                .FirstOrDefault(p => p.EventId == eventId && p.TeacherId == teacherId);

            if (participation == null)
                return NotFound();

            db.EventParticipations.DeleteOnSubmit(participation);
            db.SubmitChanges();

            return Ok("Teacher removed from event.");
        }


        // DELETE: api/events/5
        /// <summary>
        /// Deletes an event if no participants are associated.
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <returns>Status message</returns>
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var ev = db.Events.FirstOrDefault(e => e.Id == id);
            if (ev == null)
                return NotFound();

            // Verifica se há participantes vinculados (aluno ou professor)
            var hasParticipants = db.EventParticipations.Any(p => p.EventId == id);
            if (hasParticipants)
            {
                return BadRequest("Cannot delete this event because there are participants still associated with it.");
            }

            db.Events.DeleteOnSubmit(ev);
            db.SubmitChanges();
            return Ok("Event deleted successfully.");
        }
    }
}
