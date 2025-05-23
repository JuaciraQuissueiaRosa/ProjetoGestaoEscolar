using Escola.WPF.Services;
using System.Windows;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
          
        }


        private void BtnLoadSubjects_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SubjectsPage()); 
        }

        private void BtnLoadTeachers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TeachersPage()); 
        }

        private void BtnLoadStudents_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StudentsPage()); 
        }

        private void BtnLoadClasses_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClassesPage()); 
        }

        private void BtnLoadMarks_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MarksPage()); 
        }

        private void BtnLoadTimeTables_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TimeTablesPage()); 
        }
    
        private void BtnLoadEvents_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EventsPage()); 
        }

        private void BtnLoadCredits_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CreditsPage()); 
        }



    }
}