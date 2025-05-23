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

            return Ok(newTeacher); // Retorna com o Id gerado
        }

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

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            // Tenta encontrar o professor
            var teacher = db.Teachers.FirstOrDefault(t => t.Id == id);
            if (teacher == null)
            {
                // Retorna 404 se o professor não for encontrado
                return NotFound();
            }

            // Verifica se o professor tem disciplinas associadas
            bool hasSubjects = db.TeacherSubjects.Any(ts => ts.TeacherId == id);
            if (hasSubjects)
            {
                // Se o professor tem disciplinas, não pode ser excluído
                return BadRequest("Cannot delete teacher with assigned subjects.");
            }

            // Se não há disciplinas associadas, exclui o professor
            db.Teachers.DeleteOnSubmit(teacher);
            db.SubmitChanges();

            // Retorna sucesso
            return Ok("Teacher deleted successfully.");
        }
    }

}
