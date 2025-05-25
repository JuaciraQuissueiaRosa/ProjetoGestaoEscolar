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
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString2"].ConnectionString);
        // GET: api/students

        /// <summary>
        /// Retrieves all students with their basic information and class name.
        /// </summary>
        /// <returns>A list of students.</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            try
            {
                var students = db.Students.Select(s => new
                {
                    s.Id,
                    s.FullName,
                    s.Email,
                    s.Phone,
                    s.Address,
                    s.ClassId,
                    s.BirthDate,
                    ClassName = s.Class != null ? s.Class.Name : null
                }).ToList();

                if (!students.Any())
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "No students found."));

                return Ok(students);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Erro ao obter estudantes: " + ex.Message));
            }
        }

        // GET: api/students/{id}

        /// <summary>
        /// Retrieves a specific student by ID.
        /// </summary>
        /// <param name="id">The student ID.</param>
        /// <returns>The student information if found.</returns>
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var student = db.Students
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.FullName,
                    s.Email,
                    s.Phone,
                    s.Address,
                    s.BirthDate,
                    s.ClassId,
                    ClassName = s.Class != null ? s.Class.Name : null
                })
                .FirstOrDefault();

            if (student == null)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, $"No student found with ID = {id}."));

            return Ok(student);
        }

        /// <summary>
        /// Searches for students based on a search term (name, ID, or class name).
        /// </summary>
        /// <param name="term">The search term.</param>
        /// <returns>A list of matching students.</returns>
        [HttpGet]
        [Route("search")]
        public IHttpActionResult SearchStudents(string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                    return BadRequest("Search term cannot be empty.");

                term = term.ToLower();

                var students = db.Students
                    .Where(s =>
                        s.FullName.ToLower().Contains(term) ||
                        s.Id.ToString().Contains(term) ||
                        (s.Class != null && s.Class.Name.ToLower().Contains(term))
                    )
                    .Select(s => new
                    {
                        s.Id,
                        s.FullName,
                        s.BirthDate,
                        s.Phone,
                        s.Address,
                        s.Email,
                        Class = s.Class == null ? null : new
                        {
                            s.Class.Id,
                            s.Class.Name
                        }
                    })
                    .ToList();

                if (!students.Any())
                    return Content(HttpStatusCode.NotFound, "No students matched the search criteria.");

                return Ok(students);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("An error occurred while searching for students: " + ex.Message));
            }
        }


        /// <summary>
        /// Retrieves the academic history of a student, including class, marks, and final averages.
        /// </summary>
        /// <param name="id">The student ID.</param>
        /// <returns>The student’s academic history.</returns>
        [HttpGet]
        [Route("{id}/history")]
        public IHttpActionResult GetStudentHistory(int id)
        {
            try
            {
                var student = db.Students.FirstOrDefault(s => s.Id == id);
                if (student == null || string.IsNullOrWhiteSpace(student.FullName))
                {
                    return Content(HttpStatusCode.NotFound, new { message = $"Student with ID {id} not found or incomplete data." });
                }

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

                if (classInfo == null && !marks.Any() && !averages.Any())
                {
                    return Content(HttpStatusCode.NotFound, new { message = $"No academic history found for student with ID {id}." });
                }

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
            catch (Exception ex)
            {
                return InternalServerError(new Exception("An error occurred while retrieving the student history: " + ex.Message));
            }
        }



        // POST: api/students
        /// <summary>
        /// Creates a new student record.
        /// </summary>
        /// <param name="data">The student data to insert.</param>
        /// <returns>The created student with its generated ID.</returns>
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] Student data)
        {
            if (data == null)
                return BadRequest("Invalid student data.");

            var newStudent = new Student
            {
                FullName = data.FullName,
                Email = data.Email,
                Phone = data.Phone,
                Address = data.Address,
                BirthDate=data.BirthDate,
                ClassId = data.ClassId
            };

            db.Students.InsertOnSubmit(newStudent);
            db.SubmitChanges();
            return Ok(newStudent); // Retorna com o Id gerado
        }

        /// <summary>
        /// Updates an existing student by ID.
        /// </summary>
        /// <param name="id">The ID of the student to update.</param>
        /// <param name="updated">The updated student data.</param>
        /// <returns>A confirmation message if the update was successful.</returns>
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
            student.BirthDate = updated.BirthDate;
            student.ClassId = updated.ClassId;

            db.SubmitChanges();
            return Ok("Student updated successfully.");
        }


        /// <summary>
        /// Deletes a student by ID if there are no associated marks.
        /// </summary>
        /// <param name="id">The ID of the student to delete.</param>
        /// <returns>A confirmation message if deletion was successful.</returns>
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