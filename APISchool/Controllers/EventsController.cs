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
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var ev = db.Events.FirstOrDefault(e => e.Id == id);
            if (ev == null)
                return NotFound();
            return Ok(ev);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] Event data)
        {
            if (data == null)
                return BadRequest("Invalid event data.");

            if (data.EventTime == default(TimeSpan))
                return BadRequest("EventTime inválido ou ausente.");

            var newEvent = new Event
            {
                Name = data.Name,
                EventDate = data.EventDate,
                EventTime=data.EventTime,
                Location = data.Location,
                Description = data.Description
                // Não precisa atribuir ID manualmente (assume que o banco gera automaticamente)
            };

            db.Events.InsertOnSubmit(newEvent);
            db.SubmitChanges();

            return Ok(newEvent); // Retorna o evento já com o ID atribuído
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
            if (updatedEvent == null)
                return BadRequest("Dados inválidos.");

            if (updatedEvent.EventTime == default(TimeSpan))
                return BadRequest("Hora do evento (EventTime) é obrigatória e deve estar no formato HH:mm.");

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
                return BadRequest("Não é possível excluir este evento porque ainda há participantes vinculados a ele.");
            }

            db.Events.DeleteOnSubmit(ev);
            db.SubmitChanges();
            return Ok("Evento deletado com sucesso.");
        }
    }
}
