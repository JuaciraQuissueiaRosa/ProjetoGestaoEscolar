using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Windows;
using System.Windows.Controls;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for DisciplinasPage.xaml
    /// </summary>
    public partial class SubjectsPage : Page
    {
        private readonly ApiService _apiService;

        public SubjectsPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LoadSubjects();
        }

        private async void LoadSubjects()
        {
            var subjects = await _apiService.GetAsync<Subject>("subjects");
            dgSubjects.ItemsSource = subjects;
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            var subject = new Subject
            {
                Name = "New Subject", // Replace with user input
                WeeklyHours = 10      // Replace with user input
            };

            await _apiService.PostAsync("subjects", subject);
            LoadSubjects(); // Refresh the list
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgSubjects.SelectedItem is Subject selectedSubject)
            {
                selectedSubject.Name = "Updated Subject"; // Replace with user input
                selectedSubject.WeeklyHours = 12;         // Replace with user input
                await _apiService.PutAsync("subjects/" + selectedSubject.Id, selectedSubject);
                LoadSubjects(); // Refresh the list
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgSubjects.SelectedItem is Subject selectedSubject)
            {
                await _apiService.DeleteAsync("subjects/" + selectedSubject.Id);
                LoadSubjects(); // Refresh the list
            }
        }
    }
}
