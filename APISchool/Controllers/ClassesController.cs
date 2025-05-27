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
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString2"].ConnectionString);

        /// <summary>
        /// Retrieves a list of all classes with their associated students, teachers, and subjects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var classes = db.Classes.Select(c => new
            {
                c.Id,
                c.Name,
                c.AcademicYear,
                c.Course,
                c.Shift,

                Students = db.Students
          .Where(s => s.ClassId == c.Id)
          .Select(s => new { s.Id, s.FullName })
          .ToList(),

                Teachers = db.TeacherClasses
          .Where(tc => tc.ClassId == c.Id)
          .Select(tc => new { tc.Teacher.Id, tc.Teacher.FullName })
          .ToList(),

                Subjects = db.SubjectClasses
          .Where(sc => sc.ClassId == c.Id)
          .Select(sc => new { sc.Subject.Id, sc.Subject.Name })
          .ToList()
            }).ToList();

            return Ok(classes);
        }

        /// <summary>
        /// Retrieves a class by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var cls = db.Classes.FirstOrDefault(c => c.Id == id);
            if (cls == null)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, $"No class found with ID = {id}."));
            return Ok(cls);
        }

        /// <summary>
        /// Creates a new class with the provided details
        /// </summary>
        /// <param name="newClass"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] Class newClass)
        {
            db.Classes.InsertOnSubmit(newClass);
            db.SubmitChanges();

            return Ok(new
            {
                newClass.Name,
                newClass.AcademicYear,
                newClass.Course,
                newClass.Shift
            });
        }

        /// <summary>
        /// Associates a subject with a class by its ID
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Removes a subject from a class by its ID
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{classId}/remove-subject/{subjectId}")]
        public IHttpActionResult RemoveSubject(int classId, int subjectId)
        {
            var relation = db.SubjectClasses.FirstOrDefault(sc => sc.ClassId == classId && sc.SubjectId == subjectId);
            if (relation == null)
                return NotFound();

            db.SubjectClasses.DeleteOnSubmit(relation);
            db.SubmitChanges();
            return Ok("Subject successfully removed from the class.");
        }

        /// <summary>
        /// Associates a teacher with a class by its ID
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="teacherId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Removes a teacher from a class by its ID
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{classId}/remove-teacher/{teacherId}")]
        public IHttpActionResult RemoveTeacher(int classId, int teacherId)
        {
            var relation = db.TeacherClasses.FirstOrDefault(tc => tc.ClassId == classId && tc.TeacherId == teacherId);
            if (relation == null)
                return NotFound();

            db.TeacherClasses.DeleteOnSubmit(relation);
            db.SubmitChanges();
            return Ok("Teacher successfully removed from the class.");
        }

        /// <summary>
        /// Associates a student with a class by its ID
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Remove a student from a class by its ID
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{classId}/remove-student/{studentId}")]
        public IHttpActionResult RemoveStudent(int classId, int studentId)
        {
            var student = db.Students.FirstOrDefault(s => s.Id == studentId && s.ClassId == classId);
            if (student == null)
                return NotFound();

            student.ClassId = null;
            db.SubmitChanges();
            return Ok("Student successfully removed from the class.");
        }

        /// <summary>
        /// Edits an existing class by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>

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

        /// <summary>
        /// Delete and verify if there are students or teachers associated with the class
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var cls = db.Classes.FirstOrDefault(c => c.Id == id);
            if (cls == null)
                return NotFound();

          
            var hasStudents = db.Students.Any(s => s.ClassId == id);
            if (hasStudents)
            {
                return BadRequest("Cannot delete this class because there are students assigned to it.");
            }

           
            var hasTeachers = db.TeacherClasses.Any(tc => tc.ClassId == id);
            if (hasTeachers)
            {
                return BadRequest("Cannot delete this class because there are teachers assigned to it.");
            }

            db.Classes.DeleteOnSubmit(cls);
            db.SubmitChanges();

            return Ok("Class deleted successfully.");
        }
    }
}
