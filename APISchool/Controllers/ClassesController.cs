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

        [HttpDelete]
        [Route("{classId}/remove-subject/{subjectId}")]
        public IHttpActionResult RemoveSubject(int classId, int subjectId)
        {
            var relation = db.SubjectClasses.FirstOrDefault(sc => sc.ClassId == classId && sc.SubjectId == subjectId);
            if (relation == null)
                return NotFound();

            db.SubjectClasses.DeleteOnSubmit(relation);
            db.SubmitChanges();
            return Ok("Disciplina removida da turma com sucesso.");
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

        [HttpDelete]
        [Route("{classId}/remove-teacher/{teacherId}")]
        public IHttpActionResult RemoveTeacher(int classId, int teacherId)
        {
            var relation = db.TeacherClasses.FirstOrDefault(tc => tc.ClassId == classId && tc.TeacherId == teacherId);
            if (relation == null)
                return NotFound();

            db.TeacherClasses.DeleteOnSubmit(relation);
            db.SubmitChanges();
            return Ok("Professor removido da turma com sucesso.");
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

        [HttpDelete]
        [Route("{classId}/remove-student/{studentId}")]
        public IHttpActionResult RemoveStudent(int classId, int studentId)
        {
            var student = db.Students.FirstOrDefault(s => s.Id == studentId && s.ClassId == classId);
            if (student == null)
                return NotFound();

            student.ClassId = null;
            db.SubmitChanges();
            return Ok("Aluno removido da turma com sucesso.");
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

            // Verifica se há alunos associados à turma
            var hasStudents = db.Students.Any(s => s.ClassId == id);
            if (hasStudents)
            {
                return BadRequest("Não é possível excluir esta turma porque há alunos vinculados a ela.");
            }

            // Verifica se há professores associados à turma
            var hasTeachers = db.TeacherClasses.Any(tc => tc.ClassId == id);
            if (hasTeachers)
            {
                return BadRequest("Não é possível excluir esta turma porque há professores vinculados a ela.");
            }

            db.Classes.DeleteOnSubmit(cls);
            db.SubmitChanges();

            return Ok("Turma excluída com sucesso.");
        }
    }
}
