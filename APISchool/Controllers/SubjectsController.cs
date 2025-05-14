using APISchool;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace SchoolAPI.Controllers
{
    [RoutePrefix("api/subjects")]
    public class SubjectsController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString"].ConnectionString);

        [HttpGet]
        public IHttpActionResult Get()
        {
            var subjects = db.Subjects
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.WeeklyHours,
                    TeacherNamesList = s.TeacherSubjects
                             .Where(p => p.TeacherId != null)
                             .Select(p => new { p.Teacher.Id, p.Teacher.FullName }).ToList()
                })
                .ToList();

            if (!subjects.Any())
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "No subject found."));

            return Ok(subjects);
        }


        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var subject = db.Subjects
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.WeeklyHours,
                    TeacherNamesList = s.TeacherSubjects.Select(ts => ts.Teacher.FullName).ToList()
                })
                .FirstOrDefault();

            if (subject == null)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, $"No subject found with ID = {id}."));

            return Ok(subject);
        }


        [HttpPost]
        public IHttpActionResult Post([FromBody] Subject subject)
        {
            db.Subjects.InsertOnSubmit(subject);
            db.SubmitChanges();
            return Ok("Subject created successfully.");
        }

        [HttpPost]
        [Route("{subjectId}/associate-teacher/{teacherId}")]
        public IHttpActionResult AssociateTeacher(int subjectId, int teacherId)
        {
            if (!db.Subjects.Any(s => s.Id == subjectId) || !db.Teachers.Any(t => t.Id == teacherId))
                return NotFound();

            bool alreadyAssociated = db.TeacherSubjects.Any(ts => ts.SubjectId == subjectId && ts.TeacherId == teacherId);
            if (alreadyAssociated)
                return BadRequest("This teacher is already associated with the subject.");

            var association = new TeacherSubject { SubjectId = subjectId, TeacherId = teacherId };
            db.TeacherSubjects.InsertOnSubmit(association);
            db.SubmitChanges();
            return Ok("Teacher associated with subject successfully.");
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] Subject data)
        {
            var subject = db.Subjects.FirstOrDefault(s => s.Id == id);
            if (subject == null)
                return NotFound();

            subject.Name = data.Name;
            subject.WeeklyHours = data.WeeklyHours;

            db.SubmitChanges();
            return Ok("Subject updated successfully.");
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var subject = db.Subjects.FirstOrDefault(s => s.Id == id);
            if (subject == null)
                return NotFound();

            bool hasTeachers = db.TeacherSubjects.Any(ts => ts.SubjectId == id);
            if (hasTeachers)
                return BadRequest("Cannot delete subject associated with teachers.");

            db.Subjects.DeleteOnSubmit(subject);
            db.SubmitChanges();
            return Ok("Subject deleted successfully.");
        }
    }

}