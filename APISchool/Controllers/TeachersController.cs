using APISchool;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;

namespace SchoolAPI.Controllers
{
    [RoutePrefix("api/teachers")]
    public class TeachersController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString2"].ConnectionString);


        /// <summary>
        /// Retrieves a list of all teachers.
        /// </summary>
        /// <returns>A list of teacher records.</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var teachers = db.Teachers.Select(t => new
            {
                t.Id,
                t.FullName,
                t.Email,
                t.Phone,
                t.TeachingArea
            }).ToList();

            if (!teachers.Any())
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "No teachers found."));

            return Ok(teachers);
        }

        /// <summary>
        /// Retrieves a specific teacher by their ID.
        /// </summary>
        /// <param name="id">The teacher ID.</param>
        /// <returns>The teacher's details if found.</returns>
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var teacher = db.Teachers
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    t.Email,
                    t.Phone,
                    t.TeachingArea
                })
                .FirstOrDefault();

            if (teacher == null)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, $"No teacher found with ID = {id}."));

            return Ok(teacher);
        }


        // POST: api/teachers
        /// <summary>
        /// Creates a new teacher record.
        /// </summary>
        /// <param name="data">The teacher data to be created.</param>
        /// <returns>The created teacher record with generated ID.</returns>
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] Teacher data)
        {
            if (data == null)
                return BadRequest("Invalid teacher data.");

            var newTeacher = new Teacher
            {
                FullName = data.FullName,
                Email = data.Email,
                Phone = data.Phone,
                TeachingArea = data.TeachingArea
            };

            db.Teachers.InsertOnSubmit(newTeacher);
            db.SubmitChanges();

            return Ok(newTeacher); 
        }


        /// <summary>
        /// Updates an existing teacher record.
        /// </summary>
        /// <param name="id">The teacher ID to update.</param>
        /// <param name="data">The updated teacher data.</param>
        /// <returns>A success message if the update is successful.</returns>
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] Teacher data)
        {
            var teacher = db.Teachers.FirstOrDefault(t => t.Id == id);
            if (teacher == null)
                return NotFound();

            teacher.FullName = data.FullName;
            teacher.Email = data.Email;
            teacher.Phone = data.Phone;
            teacher.TeachingArea = data.TeachingArea;

            db.SubmitChanges();
            return Ok("Teacher updated successfully.");
        }

        /// <summary>
        /// Deletes a teacher by ID if they are not assigned to any subject.
        /// </summary>
        /// <param name="id">The teacher ID.</param>
        /// <returns>A success message if the teacher was deleted, or an error if they are assigned to subjects.</returns>
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
        
            var teacher = db.Teachers.FirstOrDefault(t => t.Id == id);
            if (teacher == null)
            {
               
                return NotFound();
            }

            
            bool hasSubjects = db.TeacherSubjects.Any(ts => ts.TeacherId == id);
            if (hasSubjects)
            {
              
                return BadRequest("Cannot delete teacher with assigned subjects.");
            }

           
            db.Teachers.DeleteOnSubmit(teacher);
            db.SubmitChanges();

           
            return Ok("Teacher deleted successfully.");
        }
    }

}
