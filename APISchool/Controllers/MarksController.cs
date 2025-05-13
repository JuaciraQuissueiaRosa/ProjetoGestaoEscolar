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
    [RoutePrefix("api/marks")]
    public class MarksController : ApiController
    {
        SchoolDataContext db = new SchoolDataContext(ConfigurationManager.ConnectionStrings["GestaoEscolarRGConnectionString"].ConnectionString);

        // GET: api/marks
        [HttpGet]
        public IHttpActionResult Get()
        {
            var marks = db.Marks.Select(m => new
            {
                m.Id,
                m.StudentId,
                m.SubjectId,
                m.AssessmentType,
                m.Grade,
                m.AssessmentDate,
                m.TeacherId,
                StudentName = m.Student.FullName,
                SubjectName = m.Subject.Name
            }).ToList();

            return Ok(marks);
        }

        // POST: api/marks
        [HttpPost]
        public IHttpActionResult Post([FromBody] Mark data)
        {
            int startYear = DateTime.Now.Year;
            int endYear = startYear + 1;

            DateTime academicYearStart = new DateTime(startYear, 9, 1);
            DateTime academicYearEnd = new DateTime(endYear, 6, 30);

            if (data.AssessmentDate < academicYearStart)
                return BadRequest($"The academic year {startYear}/{endYear} has not started. Adding marks is not allowed.");

            if (data.AssessmentDate > academicYearEnd)
                return BadRequest($"The academic year {startYear}/{endYear} has ended. Adding marks is not allowed.");

            db.Marks.InsertOnSubmit(data);
            db.SubmitChanges();

            UpdateAverage(data.StudentId.Value, data.SubjectId.Value);
            return Ok("Mark added successfully.");
        }

        // PUT: api/marks/5
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] Mark data)
        {
            var mark = db.Marks.FirstOrDefault(m => m.Id == id);
            if (mark == null)
                return NotFound();

            if (mark.StudentId == null || mark.SubjectId == null)
                return BadRequest("Invalid StudentId or SubjectId.");

            int startYear = DateTime.Now.Year;
            int endYear = startYear + 1;

            DateTime academicYearStart = new DateTime(startYear, 9, 1);
            DateTime academicYearEnd = new DateTime(endYear, 6, 30);

            if (data.AssessmentDate < academicYearStart)
                return BadRequest($"The academic year {startYear}/{endYear} has not started. Updating marks is not allowed.");

            if (data.AssessmentDate > academicYearEnd)
                return BadRequest($"The academic year {startYear}/{endYear} has ended. Updating marks is not allowed.");

            // Updating the mark
            mark.Grade = data.Grade;
            mark.AssessmentType = data.AssessmentType;
            mark.AssessmentDate = data.AssessmentDate;

            db.SubmitChanges();

            UpdateAverage(mark.StudentId.Value, mark.SubjectId.Value);
            return Ok("Mark updated successfully.");
        }

        // DELETE: api/marks/5
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var mark = db.Marks.FirstOrDefault(m => m.Id == id);
            if (mark == null)
                return NotFound();

            db.Marks.DeleteOnSubmit(mark);
            db.SubmitChanges();
            return Ok("Mark deleted successfully.");
        }

        private void UpdateAverage(int studentId, int subjectId)
        {
            var grades = db.Marks
                .Where(m => m.StudentId == studentId && m.SubjectId == subjectId)
                .Select(m => m.Grade)
                .ToList();

            if (!grades.Any()) return;

            float average = (float)grades.Average();

            var finalAverage = db.FinalAverages.FirstOrDefault(f => f.StudentId == studentId && f.SubjectId == subjectId);
            if (finalAverage == null)
            {
                db.FinalAverages.InsertOnSubmit(new FinalAverage
                {
                    StudentId = studentId,
                    SubjectId = subjectId,
                    Average = average
                });
            }
            else
            {
                finalAverage.Average = average;
            }

            db.SubmitChanges();
        }
    }
}