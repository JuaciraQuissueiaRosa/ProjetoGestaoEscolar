using Escola.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escola.WPF.Services
{
    public  class StudentDataService
    {
        private readonly ApiService _apiService;

        public StudentDataService()
        {
            // Usa o endereço da sua API
            _apiService = new ApiService("https://schoolapi.azurewebsites.net/");
        }
        public Task<List<Student>> GetAllAsync() =>
        _apiService.GetAsync<Student>("api/students");

        public Task AddAsync(Student student) =>
            _apiService.PostAsync("api/students", student);

        public Task UpdateAsync(Student student) =>
            _apiService.PutAsync($"api/students/{student.Id}", student);

        public Task DeleteAsync(int id) =>
            _apiService.DeleteAsync($"api/students/{id}");
    }
  
}
