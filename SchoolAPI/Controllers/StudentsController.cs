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
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString1"].ConnectionString);

        [HttpGet]
        public IHttpActionResult Get()
        {
            var students = db.Students.ToList();
            if (!students.Any())
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, $"No students found."));
            return Ok(students);
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

            if (!students.Any())
                return Ok("No students matched the search criteria.");

            return Ok(students);
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