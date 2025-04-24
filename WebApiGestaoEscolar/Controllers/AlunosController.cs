using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WebApiGestaoEscolar.Controllers
{
    public class AlunosController : ApiController
    {
       EscolaDataContext db = new EscolaDataContext("GestaoEscolarDBConnectionString");
       

        // GET api/alunos
        public IEnumerable<Aluno> Get()
        {
            return db.Alunos.ToList();
        }

        // GET api/alunos/5
        public Aluno Get(int id)
        {
            return db.Alunos.FirstOrDefault(a => a.Id == id);
        }

        [HttpGet]
        [Route("api/alunos/pesquisar")]
        
        public IHttpActionResult PesquisarAlunos(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
                return BadRequest("O termo de pesquisa não pode ser vazio.");

            termo = termo.ToLower();

            var alunos = db.Alunos
          .Where(a =>
              a.Nome.ToLower().Contains(termo) ||
              a.Id.ToString().Contains(termo) ||
              a.TurmaId.ToString().Contains(termo) 
            
          )
          .ToList();

            if (!alunos.Any())
                ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Aluno não encontrado"));

            return Ok(alunos); // Retorna os alunos encontrados
        }


        // POST api/alunos
        public IHttpActionResult Post([FromBody] Aluno aluno)
        {
            db.Alunos.InsertOnSubmit(aluno);
            db.SubmitChanges();
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, aluno));
        }

        // PUT api/alunos/5
        [HttpPut]
        [Route("api/alunos/{id}")]
        public IHttpActionResult Put(int id, [FromBody] Aluno alunoAtualizado)
        {
            var aluno = db.Alunos.FirstOrDefault(a => a.Id == id);
            if (aluno == null)
                return NotFound();

            aluno.Nome = alunoAtualizado.Nome;
            aluno.Email = alunoAtualizado.Email;
            aluno.Contacto = alunoAtualizado.Contacto;
            aluno.Morada = alunoAtualizado.Morada;
            aluno.TurmaId = alunoAtualizado.TurmaId; // mudança de turma

            db.SubmitChanges();

            return Ok("Aluno atualizado com sucesso.");
        }

        // DELETE api/alunos/5
        [HttpDelete]
        [Route("api/alunos/{id}")]
        public IHttpActionResult Delete(int id)
        {
            var aluno = db.Alunos.FirstOrDefault(a => a.Id == id);
            if (aluno == null)
                return NotFound();

            bool temNotas = db.Notas.Any(n => n.AlunoId == id);
            if (temNotas)
                return BadRequest("Não é possível apagar o aluno porque existem notas associadas.");

            db.Alunos.DeleteOnSubmit(aluno);
            db.SubmitChanges();

            return Ok("Aluno apagado com sucesso.");
        }
    }
}
