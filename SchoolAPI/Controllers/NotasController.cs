using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolAPI.Controllers
{
    public class NotasController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext("GestaoEscolarDBConnectionString");
        // GET: api/notas
        public IEnumerable<Nota> Get()
        {
            return db.Notas.ToList();
        }

        // POST: api/notas
        [HttpPost]
        [Route("")]
        public IHttpActionResult PostNota([FromBody] Nota nota)
        {
            if (nota.AlunoId == null || nota.DisciplinaId == null)
                return BadRequest("AlunoId e DisciplinaId são obrigatórios.");

            if (!db.Alunos.Any(a => a.Id == nota.AlunoId) || !db.Disciplinas.Any(d => d.Id == nota.DisciplinaId))
                return BadRequest("Aluno ou disciplina inválida.");

            // Verifica se o professor está associado à disciplina
            var autorizado = db.ProfessorDisciplinas.Any(pd =>
                pd.ProfessorId == nota.ProfessorId && pd.DisciplinaId == nota.DisciplinaId);
            if (!autorizado)
                return BadRequest("Este professor não está associado à disciplina.");

            db.Notas.InsertOnSubmit(nota);
            db.SubmitChanges();

            AtualizarMedia(nota.AlunoId.Value, nota.DisciplinaId.Value);
            return Ok("Nota lançada.");
        }



        // PUT: api/notas/5
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult PutNota(int id, [FromBody] Nota dados)
        {
            var nota = db.Notas.FirstOrDefault(n => n.Id == id);
            if (nota == null)
                return NotFound();

            if (nota.AlunoId == null || nota.DisciplinaId == null)
                return BadRequest("AlunoId e DisciplinaId inválidos.");

            // Bloquear se o período letivo já passou (ex: depois de 31-12-2024)
            if (nota.DataAvaliacao < new DateTime(2024, 12, 31))
                return BadRequest("Período letivo encerrado. Nota não pode ser alterada.");

            nota.Avaliacao = dados.Avaliacao;
            nota.Nota1 = dados.Nota1;
            nota.DataAvaliacao = dados.DataAvaliacao;

            db.SubmitChanges();

            AtualizarMedia(nota.AlunoId.Value, nota.DisciplinaId.Value);
            return Ok("Nota atualizada.");
        }
        // DELETE: api/notas/5
        public IHttpActionResult Delete(int id)
        {
            var nota = db.Notas.FirstOrDefault(n => n.Id == id);
            if (nota == null)
                return NotFound();

            db.Notas.DeleteOnSubmit(nota);
            db.SubmitChanges();
            return Ok("Nota removida.");
        }

        private void AtualizarMedia(int alunoId, int disciplinaId)
        {
            var notas = db.Notas
                .Where(n => n.AlunoId == alunoId && n.DisciplinaId == disciplinaId)
                .Select(n => n.Nota1)
                .ToList();

            if (!notas.Any()) return;

            float media = (float)notas.Average();

            var mediaFinal = db.MediasFinais.FirstOrDefault(m => m.AlunoId == alunoId && m.DisciplinaId == disciplinaId);
            if (mediaFinal == null)
            {
                db.MediasFinais.InsertOnSubmit(new MediasFinai
                {
                    AlunoId = alunoId,
                    DisciplinaId = disciplinaId,
                    Media = media
                });
            }
            else
            {
                mediaFinal.Media = media;
            }

            db.SubmitChanges();
        }
    }
}
