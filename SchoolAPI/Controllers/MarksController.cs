using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolAPI.Controllers
{
    [RoutePrefix("api/marks")]
    public class MarksController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext("GestaoEscolarDBConnectionString");


        // GET: api/marks
        [HttpGet]
        [Route("")]
        public IEnumerable<Nota> Get()
        {
            return db.Notas.ToList();
        }

        // POST: api/marks
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] Nota dados)
        {
            // Definir o ano letivo atual (2024/2025)
            int anoLetivoInicio = DateTime.Now.Year; // 2024
            int anoLetivoFim = anoLetivoInicio + 1; // 2025

            // O ano letivo começa em setembro (mês 9) do ano "anoLetivoInicio" e termina em junho (mês 6) do ano "anoLetivoFim"
            DateTime inicioAnoLetivo = new DateTime(anoLetivoInicio, 9, 1);
            DateTime fimAnoLetivo = new DateTime(anoLetivoFim, 6, 30);

            // Verificar se a data da avaliação está fora do período letivo (não permite adicionar ou alterar notas fora de 2024/2025)
            if (dados.DataAvaliacao < inicioAnoLetivo)
                return BadRequest($"O ano letivo {anoLetivoInicio}/{anoLetivoFim} ainda não começou. Não é permitido adicionar notas.");

            if (dados.DataAvaliacao > fimAnoLetivo)
                return BadRequest($"O ano letivo {anoLetivoInicio}/{anoLetivoFim} já terminou. Não é permitido adicionar notas.");

            // Adicionar a nova nota
            db.Notas.InsertOnSubmit(dados);
            db.SubmitChanges();

            // Atualizar a média
            UpdateMedia(dados.AlunoId.Value, dados.DisciplinaId.Value);
            return Ok("Nota adicionada.");
        }


        // PUT: api/marks/5
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] Nota dados)
        {
            var nota = db.Notas.FirstOrDefault(n => n.Id == id);
            if (nota == null)
                return NotFound();

            if (nota.AlunoId == null || nota.DisciplinaId == null)
                return BadRequest("AlunoId e DisciplinaId inválidos.");

            // Definir o ano letivo atual (2024/2025)
            int anoLetivoInicio = DateTime.Now.Year; // 2024
            int anoLetivoFim = anoLetivoInicio + 1; // 2025

            // O ano letivo começa em setembro (mês 9) do ano "anoLetivoInicio" e termina em junho (mês 6) do ano "anoLetivoFim"
            DateTime inicioAnoLetivo = new DateTime(anoLetivoInicio, 9, 1);
            DateTime fimAnoLetivo = new DateTime(anoLetivoFim, 6, 30);

            // Verificar se a data da avaliação está fora do período letivo (não permite adicionar ou alterar notas fora de 2024/2025)
            if (dados.DataAvaliacao < inicioAnoLetivo)
                return BadRequest($"O ano letivo {anoLetivoInicio}/{anoLetivoFim} ainda não começou. Não é permitido adicionar ou alterar notas.");

            if (dados.DataAvaliacao > fimAnoLetivo)
                return BadRequest($"O ano letivo {anoLetivoInicio}/{anoLetivoFim} já terminou. Não é permitido adicionar ou alterar notas.");

            // Atualizar a nota
            nota.Avaliacao = dados.Avaliacao;
            nota.Nota1 = dados.Nota1;
            nota.DataAvaliacao = dados.DataAvaliacao;

            db.SubmitChanges();

            // Atualizar a média
            UpdateMedia(nota.AlunoId.Value, nota.DisciplinaId.Value);
            return Ok("Nota atualizada.");
        }


        // DELETE: api/marks/5
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var nota = db.Notas.FirstOrDefault(n => n.Id == id);
            if (nota == null)
                return NotFound();

            db.Notas.DeleteOnSubmit(nota);
            db.SubmitChanges();
            return Ok("Nota removida.");
        }

        private void UpdateMedia(int alunoId, int disciplinaId)
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
