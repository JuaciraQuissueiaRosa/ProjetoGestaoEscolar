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
    [RoutePrefix("api/timetable")]
    public class TimeTableController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString"].ConnectionString);
        // GET:  api/timetable
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var timetables = db.Timetables.Select(t => new
            {
                t.Id,
                t.ClassId,
                ClassName = t.Class.Name,
                t.SubjectId,
                SubjectName = t.Subject.Name,
                t.TeacherId,
                TeacherName = t.Teacher.FullName,
                t.DayOfWeek,
                t.StartTime,
                t.EndTime
            }).ToList();

            if (!timetables.Any())
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "No timetables found."));

            return Ok(timetables);
        }


        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] Timetable data)
        {
            if (data == null)
                return BadRequest("Invalid timetable data.");

            // Verificar conflitos de horário
            var conflict = db.Timetables.Any(t =>
                t.ClassId == data.ClassId &&
                t.DayOfWeek == data.DayOfWeek &&
                ((data.StartTime >= t.StartTime && data.StartTime < t.EndTime) ||
                 (data.EndTime > t.StartTime && data.EndTime <= t.EndTime)));

            if (conflict)
                return BadRequest("Schedule conflict detected for the class.");

            db.Timetables.InsertOnSubmit(data);
            db.SubmitChanges();
            return Ok("Timetable entry created.");
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] Timetable updated)
        {
            var timetable = db.Timetables.FirstOrDefault(t => t.Id == id);
            if (timetable == null)
                return NotFound();

            // Verificar conflitos, ignorando o próprio horário
            var conflict = db.Timetables.Any(t =>
                t.Id != id &&
                t.ClassId == updated.ClassId &&
                t.DayOfWeek == updated.DayOfWeek &&
                ((updated.StartTime >= t.StartTime && updated.StartTime < t.EndTime) ||
                 (updated.EndTime > t.StartTime && updated.EndTime <= t.EndTime)));

            if (conflict)
                return BadRequest("Schedule conflict detected for the class.");

            timetable.ClassId = updated.ClassId;
            timetable.SubjectId = updated.SubjectId;
            timetable.TeacherId = updated.TeacherId;
            timetable.DayOfWeek = updated.DayOfWeek;
            timetable.StartTime = updated.StartTime;
            timetable.EndTime = updated.EndTime;

            db.SubmitChanges();
            return Ok("Timetable entry updated.");
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var timetable = db.Timetables.FirstOrDefault(t => t.Id == id);
            if (timetable == null)
                return NotFound();

            db.Timetables.DeleteOnSubmit(timetable);
            db.SubmitChanges();
            return Ok("Timetable entry deleted.");
        }
    }
}
