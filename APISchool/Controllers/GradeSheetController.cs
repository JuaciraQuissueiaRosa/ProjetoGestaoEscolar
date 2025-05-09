using APISchool;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolAPI.Controllers
{
    [RoutePrefix("api/gradesheet")]
    public class GradeSheetController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString"].ConnectionString);
        // GET: api/gradesheet
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var sheets = db.GradeSheets.Select(p => new
            {
                p.Id,
                StudentName = p.Student.FullName,
                ClassName = p.Class.Name,
                p.CreatedDate,
                p.Comments,
                SubjectGrades = db.FinalAverages
                    .Where(f => f.StudentId == p.StudentId)
                    .Select(f => new
                    {
                        SubjectName = f.Subject.Name,
                        Average = f.Average
                    }).ToList()
            }).ToList();

            return Ok(sheets);
        }

        // GET: api/gradesheet/5
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                var pauta = db.GradeSheets.FirstOrDefault(p => p.Id == id);
                if (pauta == null)
                    return NotFound();

                var subjectGrades = db.FinalAverages
                    .Where(f => f.StudentId == pauta.StudentId)
                    .Select(f => new
                    {
                        SubjectName = f.Subject.Name,
                        Average = f.Average
                    }).ToList();

                return Ok(new
                {
                    GradeSheet = new
                    {
                        pauta.Id,
                        StudentName = pauta.Student.FullName,
                        ClassName = pauta.Class.Name,
                        pauta.CreatedDate,
                        pauta.Comments
                    },
                    Averages = subjectGrades
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("averages/student/{studentId}")]
        public IHttpActionResult GetFinalAveragesByStudent(int studentId)
        {
            try
            {
                var averages = db.FinalAverages
                    .Where(f => f.StudentId == studentId)
                    .Select(f => new
                    {
                        f.Id,
                        f.StudentId,
                        f.SubjectId,
                        SubjectName = f.Subject.Name,
                        f.Average
                    })
                    .ToList();

                if (!averages.Any())
                    return NotFound();

                return Ok(averages);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/gradesheet
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] GradeSheet data)
        {
            if (data == null)
                return BadRequest("Dados inválidos.");

            try
            {
                db.GradeSheets.InsertOnSubmit(data);
                db.SubmitChanges();
                return Ok("Ficha de notas criada com sucesso.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/gradesheet/5
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] GradeSheet data)
        {
            try
            {
                var existing = db.GradeSheets.FirstOrDefault(p => p.Id == id);
                if (existing == null)
                    return NotFound();

                existing.StudentId = data.StudentId;
                existing.ClassId = data.ClassId;
                existing.CreatedDate = data.CreatedDate;
                existing.Comments = data.Comments;

                db.SubmitChanges();
                return Ok("Ficha de notas atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE: api/gradesheet/5
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var pauta = db.GradeSheets.FirstOrDefault(p => p.Id == id);
                if (pauta == null)
                    return NotFound();

                db.GradeSheets.DeleteOnSubmit(pauta);
                db.SubmitChanges();

                return Ok("Ficha de notas removida com sucesso.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
