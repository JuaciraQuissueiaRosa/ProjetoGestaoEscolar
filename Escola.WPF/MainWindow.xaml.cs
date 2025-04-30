using Escola.WPF.Services;
using System.Windows;

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


        private void BtnLoadSubjects_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SubjectsPage()); // Corrigir o namespace e o nome da página
        }

        private void BtnLoadTeachers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TeachersPage()); // Corrigir o namespace e o nome da página
        }

        private void BtnLoadStudents_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StudentsPage()); // Carrega a página de alunos
        }

        private void BtnLoadClasses_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClassesPage()); // Corrigir o namespace e o nome da página
        }

        private void BtnLoadMarks_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MarksPage()); // Corrigir o namespace e o nome da página
        }

        private void BtnLoadTimeTables_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TimeTablesPage()); // Corrigir o namespace e o nome da página
        }
        private void BtnLoadGradeSheets_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new GradeSheetsPage()); // Corrigir o namespace e o nome da página
        }

        private void BtnLoadEvents_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EventsPage()); // Corrigir o namespace e o nome da página
        }



    }
}