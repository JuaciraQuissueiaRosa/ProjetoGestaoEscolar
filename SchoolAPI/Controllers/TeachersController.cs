using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolAPI.Controllers
{
    [RoutePrefix("api/teachers")]
    public class TeachersController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext("GestaoEscolarDBConnectionString");

        // GET: api/teachers
        [HttpGet]
        [Route("")]
        public IEnumerable<Professore> Get()
        {
            return db.Professores.ToList();
        }

        // GET: api/teachers/5
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var professor = db.Professores.FirstOrDefault(p => p.Id == id);
            if (professor == null)
                return NotFound();

            return Ok(professor);
        }

        // POST: api/teachers
        [HttpPost]
        [Route(" ")]
        public IHttpActionResult Post([FromBody] Professore professor)
        {
            db.Professores.InsertOnSubmit(professor);
            db.SubmitChanges();
            return Ok(professor);
        }

        // PUT: api/teachers/5
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] Professore dados)
        {
            var professor = db.Professores.FirstOrDefault(p => p.Id == id);
            if (professor == null)
                return NotFound();

            professor.Nome = dados.Nome;
            professor.Email = dados.Email;
            professor.Contacto = dados.Contacto;
            professor.AreaEnsino = dados.AreaEnsino;

            db.SubmitChanges();
            return Ok(professor);
        }


        // DELETE: api/teachers/5
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var professor = db.Professores.FirstOrDefault(p => p.Id == id);
            if (professor == null)
                return NotFound();

            // Impedir eliminação se estiver associado a disciplinas
            bool temDisciplinas = db.ProfessorDisciplinas.Any(pd => pd.ProfessorId == id);
            if (temDisciplinas)
                return BadRequest("Não é possível apagar: professor associado a disciplinas.");

            db.Professores.DeleteOnSubmit(professor);
            db.SubmitChanges();
            return Ok("Professor apagado com sucesso.");
        }
    }
}
