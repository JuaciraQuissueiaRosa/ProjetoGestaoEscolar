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
    [RoutePrefix("api/students")]
    public class StudentsController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString"].ConnectionString);

        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var students = db.Students.ToList();
                if (!students.Any())
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "No students found."));
                return Ok(students);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Erro ao obter estudantes: " + ex.Message));
            }
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var student = db.Students.FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, $"No student found with ID = {id}."));

            }
            else
            {
                return Ok(student);
            }
               
        }

        [HttpGet]
        [Route("search")]
        public IHttpActionResult SearchStudents(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest("Search term cannot be empty.");

            term = term.ToLower();

            var students = db.Students
                .Where(s =>
                    s.FullName.ToLower().Contains(term) ||
                    s.Id.ToString().Contains(term) ||
                    s.ClassId.ToString().Contains(term))
                .ToList();

            return Ok(students); // Sempre retorna uma lista
        }

        [HttpGet]
        [Route("{id}/history")]
        public IHttpActionResult GetStudentHistory(int id)
        {
            var student = db.Students.FirstOrDefault(s => s.Id == id);
            if (student == null)
                return NotFound();

            var classInfo = db.Classes.FirstOrDefault(c => c.Id == student.ClassId);

            var marks = (from m in db.Marks
                         where m.StudentId == id
                         select new
                         {
                             m.Id,
                             m.StudentId,
                             m.SubjectId,
                             SubjectName = m.Subject.Name,
                             m.AssessmentType,
                             m.Grade,
                             m.AssessmentDate,
                             m.TeacherId,
                             TeacherName = m.Teacher.FullName
                         }).ToList();

            var averages = (from fa in db.FinalAverages
                            where fa.StudentId == id
                            select new
                            {
                                fa.SubjectId,
                                SubjectName = fa.Subject.Name,
                                fa.Average
                            }).ToList();

            // Resultado final agrupado como esperado
            var result = new
            {
                Student = new
                {
                    student.Id,
                    student.FullName
                },
                Class = classInfo == null ? null : new
                {
                    classInfo.Id,
                    classInfo.Name
                },
                Marks = marks,
                Averages = averages
            };

            return Ok(result);
        }


        [HttpPost]
        public IHttpActionResult Post([FromBody] Student student)
        {
            db.Students.InsertOnSubmit(student);
            db.SubmitChanges();
            return Ok("Student added successfully.");
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] Student updated)
        {
            var student = db.Students.FirstOrDefault(s => s.Id == id);
            if (student == null)
                return NotFound();

            student.FullName = updated.FullName;
            student.Email = updated.Email;
            student.Phone = updated.Phone;
            student.Address = updated.Address;
            student.ClassId = updated.ClassId;

            db.SubmitChanges();
            return Ok("Student updated successfully.");
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var student = db.Students.FirstOrDefault(s => s.Id == id);
            if (student == null)
                return NotFound();

            if (db.Marks.Any(m => m.StudentId == id))
                return BadRequest("Cannot delete student with existing marks.");

            db.Students.DeleteOnSubmit(student);
            db.SubmitChanges();
            return Ok("Student deleted successfully.");
        }
    }
}