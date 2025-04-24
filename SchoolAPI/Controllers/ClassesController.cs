using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace SchoolAPI.Controllers
{
    public class ClassesController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext("GestaoEscolarDBConnectionString");
        // GET: api/turmas
        [HttpGet]
        public IEnumerable<Turma> Get()
        {
            return db.Turmas.ToList();
        }

        [HttpGet]
        [Route("api/classes/{GetTurmas}")]
        // GET: api/turmas/5
        public IHttpActionResult Get(int id)
        {
            var turma = db.Turmas.FirstOrDefault(t => t.Id == id);
            if (turma == null)
                return NotFound();

            return Ok(turma);
        }

        [HttpPost]
        // POST: api/turmas
        public IHttpActionResult Post([FromBody] Turma novaTurma)
        {
            db.Turmas.InsertOnSubmit(novaTurma);
            db.SubmitChanges();
            return Ok(novaTurma);
        }

        [HttpPost]
        [Route("api/classes/{classesId}/associate-subject/{subjectId}")]
        public IHttpActionResult AssociateSubject(int turmaId, int disciplinaId)
        {
            if (!db.Turmas.Any(t => t.Id == turmaId) || !db.Disciplinas.Any(d => d.Id == disciplinaId))
                return NotFound();

            var relacao = new DisciplinaTurma
            {
                TurmaId = turmaId,
                DisciplinaId = disciplinaId
            };

            db.DisciplinaTurmas.InsertOnSubmit(relacao);
            db.SubmitChanges();

            return Ok("Disciplina associada à turma com sucesso.");
        }

        [HttpPost]
        [Route("api/classes/{classesId}/associate-professor/{professorId}")]
        public IHttpActionResult AssociateProfessor(int turmaId, int professorId)
        {
            if (!db.Turmas.Any(t => t.Id == turmaId) || !db.Professores.Any(p => p.Id == professorId))
                return NotFound();

            var relacao = new ProfessorTurma
            {
                TurmaId = turmaId,
                ProfessorId = professorId
            };

            db.ProfessorTurmas.InsertOnSubmit(relacao);
            db.SubmitChanges();

            return Ok("Professor associado à turma com sucesso.");
        }

        [HttpPost]
        [Route("api/classes/{classesId}/associate-student/{studentId}")]
        public IHttpActionResult AssociateStudent(int turmaId, int alunoId)
        {
            var turma = db.Turmas.FirstOrDefault(t => t.Id == turmaId);
            var aluno = db.Alunos.FirstOrDefault(a => a.Id == alunoId);

            if (turma == null || aluno == null)
                return NotFound();

            aluno.TurmaId = turmaId;
            db.SubmitChanges();

            return Ok("Aluno associado à turma com sucesso.");
        }

        [HttpPut]
        // PUT: api/turmas/5
        public IHttpActionResult Put(int id, [FromBody] Turma dados)
        {
            var turma = db.Turmas.FirstOrDefault(t => t.Id == id);
            if (turma == null)
                return NotFound();

            turma.Nome = dados.Nome;
            turma.AnoLetivo = dados.AnoLetivo;
            turma.Curso = dados.Curso;
            turma.Turno = dados.Turno;

            db.SubmitChanges();
            return Ok(turma);
        }

        [HttpDelete]
        // DELETE: api/turmas/5
        public IHttpActionResult Delete(int id)
        {
            var turma = db.Turmas.FirstOrDefault(t => t.Id == id);
            if (turma == null)
                return NotFound();

            db.Turmas.DeleteOnSubmit(turma);
            db.SubmitChanges();
            return Ok("Turma removida.");
        }
    }
}
