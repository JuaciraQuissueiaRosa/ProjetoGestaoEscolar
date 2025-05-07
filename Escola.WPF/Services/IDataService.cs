using Escola.WPF.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Escola.WPF.Services
{
    public interface IDataService
    {
        // Teachers
        Task<List<Teacher>> GetTeachersAsync();
        Task<Teacher> GetTeacherByIdAsync(int id);
        Task AddTeacherAsync(Teacher teacher);
        Task UpdateTeacherAsync(Teacher teacher);
        Task<HttpResponseMessage> DeleteTeacherAsync(int id);

        // Students
        Task<List<Student>> GetStudentsAsync();
        Task<Student> GetStudentByIdAsync(int id);
        Task AddStudentAsync(Student student);
        Task UpdateStudentAsync(Student student);
        Task<HttpResponseMessage> DeleteStudentAsync(int id);
        Task<List<Student>> SearchStudentsAsync(string term);
        Task<JObject> GetStudentHistoryAsync(int studentId);

        // Subjects
        Task<List<Subject>> GetSubjectsAsync();
        Task<Subject> GetSubjectByIdAsync(int id);
        Task AddSubjectAsync(Subject subject);
        Task UpdateSubjectAsync(Subject subject);
        Task<HttpResponseMessage> DeleteSubjectAsync(int id);

        // Classes
        Task<List<Class>> GetClassesAsync();
        Task<Class> GetClassByIdAsync(int id);
        Task AddClassAsync(Class turma);
        Task UpdateClassAsync(Class turma);
        Task<bool> AssociateTeacherToClassAsync(int classId, int teacherId);
        Task<bool> AssociateSubjectToClassAsync(int classId, int subjectId);
        Task<bool> AssociateStudentToClassAsync(int classId, int studentId);
        Task<HttpResponseMessage> DeleteClassAsync(int id);

        // Marks
        Task<List<Mark>> GetMarksAsync();
        Task<Mark> GetMarkByIdAsync(int id);
        Task AddMarkAsync(Mark mark);
        Task UpdateMarkAsync(Mark mark);
        Task<HttpResponseMessage> DeleteMarkAsync(int id);

        // Reports (GradeSheet)
        Task<List<GradeSheet>> GetReportsAsync();
        Task<GradeSheet> GetReportByIdAsync(int id);
        Task AddReportAsync(GradeSheet report);
        Task UpdateReportAsync(GradeSheet report);
        Task<HttpResponseMessage> DeleteReportAsync(int id);

        // Events
        Task<List<Event>> GetEventsAsync();
        Task<Event> GetEventByIdAsync(int id);
        Task AddEventAsync(Event ev);
        Task UpdateEventAsync(Event ev);
        Task<HttpResponseMessage> DeleteEventAsync(int id);

        // TimeTables
        Task<List<TimeTable>> GetTimeTablesAsync();
        Task<TimeTable> GetTimeTableByIdAsync(int id);
        Task AddTimeTableAsync(TimeTable timeTable);
        Task UpdateTimeTableAsync(TimeTable timeTable);
        Task<HttpResponseMessage> DeleteTimeTableAsync(int id);
    }
}
