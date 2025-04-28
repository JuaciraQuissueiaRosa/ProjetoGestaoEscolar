using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ApiService apiService;
        private readonly string urlBase = "https://schoolapi.azurewebsites.net/";

        public MainWindow()
        {
            InitializeComponent();
            apiService = new ApiService();
        }

        // Carregar Students
        private async Task LoadStudents()
        {
            var response = await apiService.GetAsync<Student>(urlBase, "api/students");
            if (response.IsSucess)
            {
                var students = response.Result as List<Student>;
                // Atribuir ao DataGrid (exemplo)
                myDataGrid.ItemsSource = students;
            }
            else
            {
                var dialogService = new DialogService();
                dialogService.ShowMessage("Erro", response.Message);
            }
        }

        // Carregar Subjects
        private async Task LoadSubjects()
        {
            var response = await apiService.GetAsync<Subject>(urlBase, "api/subjects");
            if (response.IsSucess)
            {
                var subjects = response.Result as List<Subject>;
                myDataGrid.ItemsSource = subjects;
            }
            else
            {
                var dialogService = new DialogService();
                dialogService.ShowMessage("Erro", response.Message);
            }
        }

        // Carregar GradeSheets
        private async Task LoadGradeSheets()
        {
            var response = await apiService.GetAsync<GradeSheet>(urlBase, "api/gradesheets");
            if (response.IsSucess)
            {
                var gradeSheets = response.Result as List<GradeSheet>;
                myDataGrid.ItemsSource = gradeSheets;
            }
            else
            {
                var dialogService = new DialogService();
                dialogService.ShowMessage("Erro", response.Message);
            }
        }

        // Carregar Marks
        private async Task LoadMarks()
        {
            var response = await apiService.GetAsync<Mark>(urlBase, "api/marks");
            if (response.IsSucess)
            {
                var marks = response.Result as List<Mark>;
                myDataGrid.ItemsSource = marks;
            }
            else
            {
                var dialogService = new DialogService();
                dialogService.ShowMessage("Erro", response.Message);
            }
        }

        // Carregar Teachers
        private async Task LoadTeachers()
        {
            var response = await apiService.GetAsync<Teacher>(urlBase, "api/teachers");
            if (response.IsSucess)
            {
                var teachers = response.Result as List<Teacher>;
                myDataGrid.ItemsSource = teachers;
            }
            else
            {
                var dialogService = new DialogService();
                dialogService.ShowMessage("Erro", response.Message);
            }
        }

        // Carregar Classes
        private async Task LoadClasses()
        {
            var response = await apiService.GetAsync<Class>(urlBase, "api/classes");
            if (response.IsSucess)
            {
                var classes = response.Result as List<Class>;
                myDataGrid.ItemsSource = classes;
            }
            else
            {
                var dialogService = new DialogService();
                dialogService.ShowMessage("Erro", response.Message);
            }
        }

        // Carregar TimeTables
        private async Task LoadTimeTables()
        {
            var response = await apiService.GetAsync<TimeTable>(urlBase, "api/timetables");
            if (response.IsSucess)
            {
                var timeTables = response.Result as List<TimeTable>;
                myDataGrid.ItemsSource = timeTables;
            }
            else
            {
                var dialogService = new DialogService();
                dialogService.ShowMessage("Erro", response.Message);
            }
        }

        // Carregar Events
        private async Task LoadEvents()
        {
            var response = await apiService.GetAsync<Event>(urlBase, "api/events");
            if (response.IsSucess)
            {
                var events = response.Result as List<Event>;
                myDataGrid.ItemsSource = events;
            }
            else
            {
                var dialogService = new DialogService();
                dialogService.ShowMessage("Erro", response.Message);
            }
        }
        private async void BtnLoadSubjects_Click(object sender, RoutedEventArgs e)
        {
            await LoadSubjects();
        }

        private async void BtnLoadTeachers_Click(object sender, RoutedEventArgs e)
        {
            await LoadTeachers();
        }

        private async void BtnLoadStudents_Click(object sender, RoutedEventArgs e)
        {
            await LoadStudents();
        }

        private async void BtnLoadClasses_Click(object sender, RoutedEventArgs e)
        {
            await LoadClasses();
        }

        private async void BtnLoadMarks_Click(object sender, RoutedEventArgs e)
        {
            await LoadMarks();
        }

        private async void BtnLoadLoadTimeTables_Click(object sender, RoutedEventArgs e)
        {
            await LoadTimeTables();
        }

        private async void BtnLoadGradeSheets_Click(object sender, RoutedEventArgs e)
        {
            await LoadGradeSheets();
        }

        private async void BtnLoadEvents_Click(object sender, RoutedEventArgs e)
        {
            await LoadEvents();
        }



    }
}