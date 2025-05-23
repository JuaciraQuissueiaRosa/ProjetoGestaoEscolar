using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        public MainWindow()
        {
            InitializeComponent();
           
            
        }

    
        // Animação para mostrar fundo escuro suavemente
        private void AnimateDarkOverlayIn()
        {
            DarkOverlay.Visibility = Visibility.Visible;

            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 0.5,
                Duration = TimeSpan.FromMilliseconds(400),
                FillBehavior = FillBehavior.HoldEnd
            };

            DarkOverlay.BeginAnimation(OpacityProperty, fadeIn);
        }


        private void BtnLoadSubjects_Click(object sender, RoutedEventArgs e)
        {
            var subjectsWindow = new SubjectsWindow();
            subjectsWindow.Show();
        }

        private void BtnLoadTeachers_Click(object sender, RoutedEventArgs e)
        {
            var teachersWindow = new TeachersWindow();
            teachersWindow.Show();
        }

        private void BtnLoadStudents_Click(object sender, RoutedEventArgs e)
        {
            var studentsWindow = new StudentsWindow();
            studentsWindow.Show();
        }

        private void BtnLoadClasses_Click(object sender, RoutedEventArgs e)
        {
            var classesWindow = new ClassesWindow();
            classesWindow.Show();
        }

        private void BtnLoadMarks_Click(object sender, RoutedEventArgs e)
        {
            var marksWindow = new MarksWindow();
            marksWindow.Show();
        }

        private void BtnLoadTimeTables_Click(object sender, RoutedEventArgs e)
        {
            var timeTablesWindow = new TimetableWindow();
            timeTablesWindow.Show();
        }

        private void BtnLoadEvents_Click(object sender, RoutedEventArgs e)
        {
            var eventsWindow = new EventsWindow();
            eventsWindow.Show();
        }

        private void BtnLoadCredits_Click(object sender, RoutedEventArgs e)
        {
            var creditsWindow = new CreditsWindow();
            creditsWindow.Show();
        }

    }
}
