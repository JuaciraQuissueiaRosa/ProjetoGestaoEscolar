using APISchool;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolAPI.Controllers
{
    [RoutePrefix("api/marks")]
    public class MarksController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString"].ConnectionString);
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var marks = db.Marks.Select(m => new
            {
                m.Id,
                m.StudentId,
                StudentName = m.Student.FullName,
                m.SubjectId,
                SubjectName = m.Subject.Name,
                m.AssessmentType,
                m.Grade,
                m.AssessmentDate
             
            }).ToList();

            return Ok(marks);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] Mark data)
        {
            if (data.StudentId == 0 || data.SubjectId == 0)
                return BadRequest("StudentId and SubjectId are required.");

            if (string.IsNullOrWhiteSpace(data.AssessmentDate) || !IsValidAcademicYearFormat(data.AssessmentDate))
                return BadRequest("Invalid AssessmentDate format. Use yyyy/yyyy (e.g. 2024/2025).");

            if (IsAcademicYearClosed(data.AssessmentDate))
                return BadRequest("The academic year has ended. Adding marks is not allowed.");

            var newMark = new Mark
            {   Id= data.Id,
                StudentId = data.StudentId,
                SubjectId = data.SubjectId,
                Grade = data.Grade,
                AssessmentType = data.AssessmentType,
                AssessmentDate = data.AssessmentDate,
                
            };

            db.Marks.InsertOnSubmit(newMark);
            db.SubmitChanges();

            UpdateAverage(newMark.StudentId.Value, newMark.SubjectId.Value);
            return Ok(newMark);
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] Mark data)
        {
            var mark = db.Marks.FirstOrDefault(m => m.Id == id);
            if (mark == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(data.AssessmentDate) || !IsValidAcademicYearFormat(data.AssessmentDate))
                return BadRequest("Invalid AssessmentDate format. Use yyyy/yyyy (e.g. 2024/2025).");

            if (IsAcademicYearClosed(data.AssessmentDate))
                return BadRequest("The academic year has ended. Updating marks is not allowed.");

            // ✅ Atualiza também o aluno e disciplina
            mark.StudentId = data.StudentId;
            mark.SubjectId = data.SubjectId;
            mark.Grade = data.Grade;
            mark.AssessmentType = data.AssessmentType;
            mark.AssessmentDate = data.AssessmentDate;

            db.SubmitChanges();

            UpdateAverage(mark.StudentId.Value, mark.SubjectId.Value);
            return Ok("Mark updated successfully.");
        }


        [HttpGet]
        [Route("final-averages")]
        public IHttpActionResult GetFinalAverages()
        {
            var finalAverages = db.FinalAverages
                .Select(fa => new
                {
                    fa.StudentId,
                    fa.SubjectId,
                    fa.Average
                })
                .ToList();

            return Ok(finalAverages);
        }

        // DELETE: api/marks/5
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var mark = db.Marks.FirstOrDefault(m => m.Id == id);
            if (mark == null)
                return NotFound();

            if (IsAcademicYearClosed(mark.AssessmentDate))
                return BadRequest("The academic year has ended. Deleting marks is not allowed.");

            db.Marks.DeleteOnSubmit(mark);
            db.SubmitChanges();
            // ✅ ADICIONE ESTA LINHA se ainda não estiver lá:
            UpdateAverage(mark.StudentId.Value, mark.SubjectId.Value);
            return Ok("Mark deleted successfully.");
        }
        private bool IsValidAcademicYearFormat(string yearRange)
        {
            if (string.IsNullOrWhiteSpace(yearRange))
                return false;

            var parts = yearRange.Split('/');
            return parts.Length == 2 &&
                   int.TryParse(parts[0], out int startYear) &&
                   int.TryParse(parts[1], out int endYear) &&
                   endYear == startYear + 1;
        }

        private bool IsAcademicYearClosed(string yearRange)
        {
            var parts = yearRange.Split('/');
            if (!int.TryParse(parts[1], out int endYear)) return true;

            var endOfAcademicYear = new DateTime(endYear, 6, 30);
            return DateTime.Now > endOfAcademicYear;
        }

        private void UpdateAverage(int studentId, int subjectId)
        {
            // Buscar todas as notas do aluno na disciplina
            var marks = db.Marks.Where(m => m.StudentId == studentId && m.SubjectId == subjectId).ToList();

            if (marks.Count == 0)
            {
                // Se não existir nota, remover média caso exista
                var existingAverage = db.FinalAverages.FirstOrDefault(fa => fa.StudentId == studentId && fa.SubjectId == subjectId);
                if (existingAverage != null)
                {
                    db.FinalAverages.DeleteOnSubmit(existingAverage);
                    db.SubmitChanges();
                }
                return;
            }

            // Calcular média aritmética
            float average = (float)marks.Average(m => m.Grade);

            // Verificar se já existe média para o aluno e disciplina
            var finalAverage = db.FinalAverages.FirstOrDefault(fa => fa.StudentId == studentId && fa.SubjectId == subjectId);

            if (finalAverage == null)
            {
                // Inserir nova média
                finalAverage = new FinalAverage
                {
                    StudentId = studentId,
                    SubjectId = subjectId,
                    Average = average
                };
                db.FinalAverages.InsertOnSubmit(finalAverage);
            }
            else
            {
                // Atualizar média existente
                finalAverage.Average = average;
            }

            db.SubmitChanges();
        }


    }
}