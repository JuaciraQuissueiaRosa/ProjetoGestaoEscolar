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
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString2"].ConnectionString);

        /// <summary>
        /// Retrieves all marks.
        /// </summary>

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

        /// <summary>
        /// Adds a new mark (grade).
        /// </summary>
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


        /// <summary>
        /// Updates an existing mark by ID.
        /// </summary>
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

        /// <summary>
        /// Retrieves the final averages per student and subject.
        /// </summary>
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

        /// <summary>
        /// Deletes a mark by ID.
        /// </summary>
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

            // Recalculate average after deletion
            UpdateAverage(mark.StudentId.Value, mark.SubjectId.Value);
            return Ok("Mark deleted successfully.");
        }

        /// <summary>
        /// Validates if the academic year is in the correct format: yyyy/yyyy.
        /// </summary>
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

        /// <summary>
        /// Checks if the academic year is already closed (past June 30 of the end year).
        /// </summary>
        private bool IsAcademicYearClosed(string yearRange)
        {
            var parts = yearRange.Split('/');
            if (!int.TryParse(parts[1], out int endYear)) return true;

            var endOfAcademicYear = new DateTime(endYear, 6, 30);
            return DateTime.Now > endOfAcademicYear;
        }

        /// <summary>
        /// Updates or recalculates the final average for a student in a subject.
        /// </summary>
        private void UpdateAverage(int studentId, int subjectId)
        {
            // Fetch all marks for the student in the given subject
            var marks = db.Marks.Where(m => m.StudentId == studentId && m.SubjectId == subjectId).ToList();

            if (marks.Count == 0)
            {
                // If there are no marks, delete the average if it exists
                var existingAverage = db.FinalAverages.FirstOrDefault(fa => fa.StudentId == studentId && fa.SubjectId == subjectId);
                if (existingAverage != null)
                {
                    db.FinalAverages.DeleteOnSubmit(existingAverage);
                    db.SubmitChanges();
                }
                return;
            }

            // Calculate the arithmetic average
            float average = (float)marks.Average(m => m.Grade);

            // Check if a final average already exists for the student and subject
            var finalAverage = db.FinalAverages.FirstOrDefault(fa => fa.StudentId == studentId && fa.SubjectId == subjectId);

            if (finalAverage == null)
            {
                // Insert a new final average
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
                // Update the existing average
                finalAverage.Average = average;
            }

            db.SubmitChanges();
        }


    }
}