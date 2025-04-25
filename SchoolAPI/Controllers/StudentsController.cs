using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolAPI.Controllers
{
    [RoutePrefix("api/students")]
    public class StudentsController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext("GestaoEscolarDBConnectionString");

        /// <summary>
        /// Return all the students in the database
        /// </summary>
        /// <returns></returns>
        // GET api/students
        [HttpGet]
        [Route(" ")]
        public IEnumerable<Aluno> Get()
        {
            return db.Alunos.ToList();
        }

        [HttpGet]
        [Route("{id}")]
        // GET api/students/5
        public Aluno Get(int id)
        {
            return db.Alunos.FirstOrDefault(a => a.Id == id);
        }

        [HttpGet]
        [Route("search")]
        public IHttpActionResult SearchStudents(string termo)
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


        // POST api/students
        [HttpPost]
        [Route(" ")]
        public IHttpActionResult Post([FromBody] Aluno aluno)
        {
            db.Alunos.InsertOnSubmit(aluno);
            db.SubmitChanges();
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, aluno));
        }

        // PUT api/students/5
        [HttpPut]
        [Route("{id}")]
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

        // DELETE api/students/5
        [HttpDelete]
        [Route("{id}")]
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
