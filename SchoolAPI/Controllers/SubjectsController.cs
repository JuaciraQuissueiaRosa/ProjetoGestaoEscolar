using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolAPI.Controllers
{
    public class SubjectsController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext("GestaoEscolarDBConnectionString");
        // GET:  api/subjects
        public IEnumerable<Disciplina> Get()
        {
            return db.Disciplinas.ToList();
        }

        // GET:  api/subjects/
        public IHttpActionResult Get(int id)
        {
            var disciplina = db.Disciplinas.FirstOrDefault(d => d.Id == id);
            if (disciplina == null)
                return NotFound();

            return Ok(disciplina);
        }

        // POST:  api/subjects
        public IHttpActionResult Post([FromBody] Disciplina disciplina)
        {
            db.Disciplinas.InsertOnSubmit(disciplina);
            db.SubmitChanges();
            return Ok(disciplina);
        }
        [HttpPost]
        [Route("api/subjects/{subjectId}/associate-teacher/{TeacherId}")]
        public IHttpActionResult AssociateTeacher(int disciplinaId, int professorId)
        {
            var disciplina = db.Disciplinas.FirstOrDefault(d => d.Id == disciplinaId);
            var professor = db.Professores.FirstOrDefault(p => p.Id == professorId);

            if (disciplina == null || professor == null)
                return NotFound();

            // Verifica se já está associado
            bool jaAssociado = db.ProfessorDisciplinas.Any(pd =>
                pd.DisciplinaId == disciplinaId && pd.ProfessorId == professorId);
            if (jaAssociado)
                return BadRequest("Este professor já está associado à disciplina.");

            var associacao = new ProfessorDisciplina
            {
                DisciplinaId = disciplinaId,
                ProfessorId = professorId
            };

            db.ProfessorDisciplinas.InsertOnSubmit(associacao);
            db.SubmitChanges();

            return Ok("Professor associado à disciplina com sucesso.");
        }

        // PUT:  api/subjects/5
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

        // DELETE: api/subjects/5
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
