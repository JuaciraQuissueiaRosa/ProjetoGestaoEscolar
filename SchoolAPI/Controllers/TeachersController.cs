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
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString1"].ConnectionString);

        [HttpGet]
        public IHttpActionResult Get()
        {
            var teachers = db.Teachers.ToList();
            if (!teachers.Any())
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "No teachers found."));
            return Ok(teachers);
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var teacher = db.Teachers.FirstOrDefault(t => t.Id == id);
            if (teacher == null)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, $"No teacher found with ID = {id}."));
            return Ok(teacher);
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] Teacher teacher)
        {
            if (teacher == null)
                return BadRequest("Invalid teacher data.");

            // Garantir que o ID não seja definido manualmente
            teacher.Id = 0;

            db.Teachers.InsertOnSubmit(teacher);
            db.SubmitChanges();
            return Ok("Teacher added successfully.");
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
