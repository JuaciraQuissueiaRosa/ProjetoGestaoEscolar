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
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString2"].ConnectionString);


        // GET:  api/timetable
        /// <summary>
        /// Retrieves all timetable entries.
        /// </summary>
        /// <returns>A list of timetable records with class, subject, and teacher information.</returns>
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

        /// <summary>
        /// Creates a new timetable entry.
        /// </summary>
        /// <param name="data">The timetable data to insert.</param>
        /// <returns>A success message or a conflict warning if the schedule overlaps.</returns>
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] Timetable data)
        {
            if (data == null)
                return BadRequest("Invalid timetable data.");

           
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


        /// <summary>
        /// Updates an existing timetable entry by ID.
        /// </summary>
        /// <param name="id">The ID of the timetable entry to update.</param>
        /// <param name="updated">The updated timetable data.</param>
        /// <returns>A success message or a conflict warning if the updated schedule overlaps.</returns>
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] Timetable updated)
        {
            var timetable = db.Timetables.FirstOrDefault(t => t.Id == id);
            if (timetable == null)
                return NotFound();

           
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


        /// <summary>
        /// Deletes a timetable entry by ID.
        /// </summary>
        /// <param name="id">The ID of the timetable entry to delete.</param>
        /// <returns>A success message if the entry was deleted.</returns>
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
