using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolAPI.Controllers
{
    public class DisciplinasController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext("GestaoEscolarDBConnectionString");
        // GET: api/Disciplinas
        public IEnumerable<Disciplina> Get()
        {
            return db.Disciplinas.ToList();
        }

        // GET: api/disciplinas/5
        public IHttpActionResult Get(int id)
        {
            var disciplina = db.Disciplinas.FirstOrDefault(d => d.Id == id);
            if (disciplina == null)
                return NotFound();

            return Ok(disciplina);
        }

        // POST: api/disciplinas
        public IHttpActionResult Post([FromBody] Disciplina disciplina)
        {
            db.Disciplinas.InsertOnSubmit(disciplina);
            db.SubmitChanges();
            return Ok(disciplina);
        }

        // PUT: api/disciplinas/5
        public IHttpActionResult Put(int id, [FromBody] Disciplina dados)
        {
            var disciplina = db.Disciplinas.FirstOrDefault(d => d.Id == id);
            if (disciplina == null)
                return NotFound();

            disciplina.Nome = dados.Nome;
            disciplina.CargaHorariaSemanal = dados.CargaHorariaSemanal;

            db.SubmitChanges();
            return Ok(disciplina);
        }

        // DELETE: api/disciplinas/5
        public IHttpActionResult Delete(int id)
        {
            var disciplina = db.Disciplinas.FirstOrDefault(d => d.Id == id);
            if (disciplina == null)
                return NotFound();

            // Impedir eliminação se estiver associada a professores
            bool temProfessores = db.ProfessorDisciplinas.Any(pd => pd.DisciplinaId == id);
            if (temProfessores)
                return BadRequest("Não é possível apagar: disciplina associada a professores.");

            db.Disciplinas.DeleteOnSubmit(disciplina);
            db.SubmitChanges();
            return Ok("Disciplina apagada com sucesso.");
        }
    }
}
