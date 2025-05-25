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
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString2"].ConnectionString);


        /// <summary>
        /// Retrieves a list of all subjects along with their weekly hours and associated teachers.
        /// </summary>
        /// <returns>A list of subjects with teacher information.</returns>
        [HttpGet]
        [Route("")]
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

        /// <summary>
        /// Retrieves detailed information about a specific subject by ID.
        /// </summary>
        /// <param name="id">The subject ID.</param>
        /// <returns>The subject details, including associated teachers.</returns>
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


        // POST: api/subjects

        /// <summary>
        /// Creates a new subject.
        /// </summary>
        /// <param name="data">The subject data to create.</param>
        /// <returns>The newly created subject.</returns>
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] Subject data)
        {
            if (data == null)
                return BadRequest("Invalid subject data.");

            var newSubject = new Subject
            {
                Name = data.Name,
                WeeklyHours = data.WeeklyHours
            };

            db.Subjects.InsertOnSubmit(newSubject);
            db.SubmitChanges();

            return Ok(newSubject); 
        }



        /// <summary>
        /// Associates a teacher with a subject.
        /// </summary>
        /// <param name="subjectId">The subject ID.</param>
        /// <param name="teacherId">The teacher ID.</param>
        /// <returns>A confirmation message if association was successful.</returns>
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

        /// <summary>
        /// Removes a teacher from a subject.
        /// </summary>
        /// <param name="subjectId">The subject ID.</param>
        /// <param name="teacherId">The teacher ID.</param>
        /// <returns>A confirmation message if removal was successful.</returns>
        [HttpDelete]
        [Route("{subjectId}/remove-teacher/{teacherId}")]
        public IHttpActionResult RemoveTeacher(int subjectId, int teacherId)
        {
            var teacherSubject = db.TeacherSubjects.FirstOrDefault(ts => ts.SubjectId == subjectId && ts.TeacherId == teacherId);
            if (teacherSubject == null)
                return NotFound();

            db.TeacherSubjects.DeleteOnSubmit(teacherSubject);
            db.SubmitChanges();
            return Ok("Professor removido da disciplina com sucesso.");
        }



        /// <summary>
        /// Updates an existing subject.
        /// </summary>
        /// <param name="id">The subject ID.</param>
        /// <param name="data">The updated subject data.</param>
        /// <returns>A confirmation message if the update was successful.</returns>
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


        /// <summary>
        /// Deletes a subject by ID.
        /// </summary>
        /// <param name="id">The subject ID.</param>
        /// <returns>A confirmation message if deletion was successful, or an error if it is associated with other records.</returns>
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var subject = db.Subjects.FirstOrDefault(s => s.Id == id);
            if (subject == null)
                return NotFound();

            try
            {
                db.Subjects.DeleteOnSubmit(subject);
                db.SubmitChanges();
                return Ok("Subject deleted successfully.");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Cannot delete subject: it is associated with other records.");
            }
        }
    }

}