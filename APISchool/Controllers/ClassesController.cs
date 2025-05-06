using APISchool;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace SchoolAPI.Controllers
{
    [RoutePrefix("api/classes")]
    public class ClassesController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString"].ConnectionString);

        [HttpGet]
        public IHttpActionResult Get()
        {
            var classes = db.Classes.ToList();
            if (!classes.Any())
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "No classes found."));
            return Ok(classes);
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var cls = db.Classes.FirstOrDefault(c => c.Id == id);
            if (cls == null)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, $"No class found with ID = {id}."));
            return Ok(cls);
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] Class newClass)
        {
            db.Classes.InsertOnSubmit(newClass);
            db.SubmitChanges();
            return Ok("Class created successfully.");
        }

        [HttpPost]
        [Route("{classId}/associate-subject/{subjectId}")]
        public IHttpActionResult AssociateSubject(int classId, int subjectId)
        {
            if (!db.Classes.Any(c => c.Id == classId) || !db.Subjects.Any(s => s.Id == subjectId))
                return NotFound();

            var relation = new SubjectClass { ClassId = classId, SubjectId = subjectId };
            db.SubjectClasses.InsertOnSubmit(relation);
            db.SubmitChanges();
            return Ok("Subject successfully associated with the class.");
        }

        [HttpPost]
        [Route("{classId}/associate-teacher/{teacherId}")]
        public IHttpActionResult AssociateTeacher(int classId, int teacherId)
        {
            if (!db.Classes.Any(c => c.Id == classId) || !db.Teachers.Any(t => t.Id == teacherId))
                return NotFound();

            var relation = new TeacherClass { ClassId = classId, TeacherId = teacherId };
            db.TeacherClasses.InsertOnSubmit(relation);
            db.SubmitChanges();
            return Ok("Teacher successfully associated with the class.");
        }

        [HttpPost]
        [Route("{classId}/associate-student/{studentId}")]
        public IHttpActionResult AssociateStudent(int classId, int studentId)
        {
            var cls = db.Classes.FirstOrDefault(c => c.Id == classId);
            var student = db.Students.FirstOrDefault(s => s.Id == studentId);

            if (cls == null || student == null)
                return NotFound();

            student.ClassId = classId;
            db.SubmitChanges();
            return Ok("Student successfully assigned to the class.");
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] Class data)
        {
            var cls = db.Classes.FirstOrDefault(c => c.Id == id);
            if (cls == null)
                return NotFound();

            cls.Name = data.Name;
            cls.AcademicYear = data.AcademicYear;
            cls.Course = data.Course;
            cls.Shift = data.Shift;

            db.SubmitChanges();
            return Ok("Class updated successfully.");
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var cls = db.Classes.FirstOrDefault(c => c.Id == id);
            if (cls == null)
                return NotFound();

            db.Classes.DeleteOnSubmit(cls);
            db.SubmitChanges();
            return Ok("Class deleted successfully.");
        }
    }
}
