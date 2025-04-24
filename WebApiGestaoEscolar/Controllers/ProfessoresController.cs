using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace WebApiGestaoEscolar.Controllers
{
    public class ProfessoresController : ApiController
    {
        EscolaDataContext db = new EscolaDataContext("GestaoEscolarDBConnectionString");
        
        // GET: api/professores
        public IEnumerable<Professore> Get()
        {
            return db.Professores.ToList();
        }

        // GET: api/professores/5
        public IHttpActionResult Get(int id)
        {
            var professor = db.Professores.FirstOrDefault(p => p.Id == id);
            if (professor == null)
                return NotFound();

            return Ok(professor);
        }

        // POST: api/professores
        public IHttpActionResult Post([FromBody] Professore professor)
        {
            db.Professores.InsertOnSubmit(professor);
            db.SubmitChanges();
            return Ok(professor);
        }

        // PUT: api/professores/5
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

        [HttpPost]
        [Route("api/professores/{professorId}/associar-disciplina/{disciplinaId}")]
        public IHttpActionResult AssociarDisciplina(int professorId, int disciplinaId)
        {
            if (!db.Professores.Any(p => p.Id == professorId) || !db.Disciplinas.Any(d => d.Id == disciplinaId))
                return NotFound();

            var relacao = new ProfessorDisciplina
            {
                ProfessorId = professorId,
                DisciplinaId = disciplinaId
            };

            db.ProfessorDisciplinas.InsertOnSubmit(relacao);
            db.SubmitChanges();

            return Ok("Associação realizada com sucesso.");
        }

        // DELETE: api/professores/5
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
