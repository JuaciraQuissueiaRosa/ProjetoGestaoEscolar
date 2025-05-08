using Escola.WPF.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Windows;

namespace Escola.WPF.Services
{
    public class ApiService:IDataService
    {
        private readonly HttpClient _client;

        public ApiService(string baseUrl = "https://schoolapi.azurewebsites.net/api/")
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public async Task<List<Teacher>> GetTeachersAsync()
        {
            var response = await _client.GetAsync("teachers");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Teacher>>();
        }

        public async Task<Teacher> GetTeacherByIdAsync(int id)
        {
            var response = await _client.GetAsync($"teachers/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Teacher>();
        }

        public async Task AddTeacherAsync(Teacher teacher)
        {
            var response = await _client.PostAsJsonAsync("teachers", teacher);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateTeacherAsync(Teacher teacher)
        {
            var response = await _client.PutAsJsonAsync($"teachers/{teacher.Id}", teacher);
            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> DeleteTeacherAsync(int id)
        {
            return await _client.DeleteAsync($"teachers/{id}");
        }
        public async Task<List<Student>> GetStudentsAsync()
        {
            var response = await _client.GetAsync("students");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Student>>();
        }

        public async Task<Student> GetStudentByIdAsync(int id)
        {
            var response = await _client.GetAsync($"students/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Student>();
        }

        public async Task AddStudentAsync(Student student)
        {
            var response = await _client.PostAsJsonAsync("students", student);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateStudentAsync(Student student)
        {
            var response = await _client.PutAsJsonAsync($"students/{student.Id}", student);
            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> DeleteStudentAsync(int id)
        {
            return await _client.DeleteAsync($"students/{id}");
        }

        public async Task<List<Subject>> GetSubjectsAsync()
        {
            var response = await _client.GetAsync("subjects");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Subject>>();
        }

        public async Task<Subject> GetSubjectByIdAsync(int id)
        {
            var response = await _client.GetAsync($"subjects/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Subject>();
        }

        public async Task AddSubjectAsync(Subject subject)
        {
            var response = await _client.PostAsJsonAsync("subjects", subject);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateSubjectAsync(Subject subject)
        {
            var response = await _client.PutAsJsonAsync($"subjects/{subject.Id}", subject);
            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> DeleteSubjectAsync(int id)
        {
            return await _client.DeleteAsync($"subjects/{id}");
        }

        public async Task<List<Class>> GetClassesAsync()
        {
            var response = await _client.GetAsync("classes");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Class>>();
        }

        public async Task<Class> GetClassByIdAsync(int id)
        {
            var response = await _client.GetAsync($"classes/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Class>();
        }

        public async Task AddClassAsync(Class turma)
        {
            var response = await _client.PostAsJsonAsync("classes", turma);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateClassAsync(Class turma)
        {
            var response = await _client.PutAsJsonAsync($"classes/{turma.Id}", turma);
            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> DeleteClassAsync(int id)
        {
            return await _client.DeleteAsync($"classes/{id}");
        }
        public async Task<List<Mark>> GetMarksAsync()
        {
            var response = await _client.GetAsync("marks");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Mark>>();
        }

        public async Task<Mark> GetMarkByIdAsync(int id)
        {
            var response = await _client.GetAsync($"marks/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Mark>();
        }

        public async Task AddMarkAsync(Mark mark)
        {
            var response = await _client.PostAsJsonAsync("marks", mark);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateMarkAsync(Mark mark)
        {
            var response = await _client.PutAsJsonAsync($"marks/{mark.Id}", mark);
            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> DeleteMarkAsync(int id)
        {
            return await _client.DeleteAsync($"marks/{id}");
        }

        public async Task<List<GradeSheet>> GetReportsAsync()
        {
            var response = await _client.GetAsync("reports");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<GradeSheet>>();
        }

        public async Task<GradeSheet> GetReportByIdAsync(int id)
        {
            var response = await _client.GetAsync($"reports/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GradeSheet>();
        }

        public async Task AddReportAsync(GradeSheet report)
        {
            var response = await _client.PostAsJsonAsync("reports", report);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateReportAsync(GradeSheet report)
        {
            var response = await _client.PutAsJsonAsync($"reports/{report.Id}", report);
            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> DeleteReportAsync(int id)
        {
            return await _client.DeleteAsync($"reports/{id}");
        }

        public async Task<List<Event>> GetEventsAsync()
        {
            var response = await _client.GetAsync("events");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Event>>();
        }

        public async Task<Event> GetEventByIdAsync(int id)
        {
            var response = await _client.GetAsync($"events/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Event>();
        }

        public async Task AddEventAsync(Event ev)
        {
            var response = await _client.PostAsJsonAsync("events", ev);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateEventAsync(Event ev)
        {
            var response = await _client.PutAsJsonAsync($"events/{ev.Id}", ev);
            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> DeleteEventAsync(int id)
        {
            return await _client.DeleteAsync($"events/{id}");
        }
        public async Task<List<TimeTable>> GetTimeTablesAsync()
        {
            var response = await _client.GetAsync("timetables");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<TimeTable>>();
        }

        public async Task<TimeTable> GetTimeTableByIdAsync(int id)
        {
            var response = await _client.GetAsync($"timetables/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TimeTable>();
        }

        public async Task AddTimeTableAsync(TimeTable timeTable)
        {
            var response = await _client.PostAsJsonAsync("timetables", timeTable);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateTimeTableAsync(TimeTable timeTable)
        {
            var response = await _client.PutAsJsonAsync($"timetables/{timeTable.Id}", timeTable);
            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> DeleteTimeTableAsync(int id)
        {
            return await _client.DeleteAsync($"timetables/{id}");
        }

        public async Task<List<Student>> SearchStudentsAsync(string term)
        {
            var response = await _client.GetAsync($"students/search?term={term}");

            if (response.IsSuccessStatusCode)
            {
                // Lê o conteúdo como string
                var jsonString = await response.Content.ReadAsStringAsync();

                // Desserializa a string JSON para uma lista de estudantes
                var students = JsonConvert.DeserializeObject<List<Student>>(jsonString);

                return students;
            }

            return new List<Student>();  // Retorna uma lista vazia caso não encontre nada
        }

        public async Task<JObject> GetStudentHistoryAsync(int studentId)
        {
            var response = await _client.GetAsync($"students/{studentId}/history");
            var json = await response.Content.ReadAsStringAsync();

            //MessageBox.Show(json);  // <-- isto ajuda a ver o conteúdo real

            if (response.IsSuccessStatusCode)
            {
                JObject result = JObject.Parse(json);
                return result;
            }

            return null;
        }

        public async Task<bool> AssociateTeacherToClassAsync(int classId, int teacherId)
        {
            var response = await _client.PostAsync($"classes/{classId}/associate-teacher/{teacherId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AssociateSubjectToClassAsync(int classId, int subjectId)
        {
            var response = await _client.PostAsync($"classes/{classId}/associate-subject/{subjectId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AssociateStudentToClassAsync(int classId, int studentId)
        {
            var response = await _client.PostAsync($"classes/{classId}/associate-student/{studentId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task AssociateTeacherToSubjectAsync(int subjectId, int teacherId)
        {
            var response = await _client.PostAsync($"subjects/{subjectId}/associate-teacher/{teacherId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task AssociateStudentToEventAsync(int eventId, int studentId)
        {
            var response = await _client.PostAsync($"events/{eventId}/associate-student/{studentId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task AssociateTeacherToEventAsync(int eventId, int teacherId)
        {
            var response = await _client.PostAsync($"events/{eventId}/associate-teacher/{teacherId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveStudentFromEventAsync(int eventId, int studentId)
        {
            var response = await _client.DeleteAsync($"events/{eventId}/remove-student/{studentId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveTeacherFromEventAsync(int eventId, int teacherId)
        {
            var response = await _client.DeleteAsync($"events/{eventId}/remove-teacher/{teacherId}");
            response.EnsureSuccessStatusCode();
        }
    }

}
