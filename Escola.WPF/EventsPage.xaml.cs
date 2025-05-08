using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for EventsPage.xaml
    /// </summary>
    public partial class EventsPage : Page
    {
        private readonly IDataService _eventService;

        public EventsPage()
        {
            try
            {
                InitializeComponent();
                _eventService = new ApiService();
                LoadEvents();
                LoadStudents();
                LoadTeachers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing page: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadEvents()
        {
            try
            {
                var events = await _eventService.GetEventsAsync(); // Chama o método correto GetEventsAsync
                dgEvents.ItemsSource = events;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading events: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
          
        }
        private async void LoadStudents()
        {
            try
            {
                cbStudents.ItemsSource = await _eventService.GetStudentsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading students: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
        }
        private async void LoadTeachers()
        {
            try
            {
                cbTeachers.ItemsSource = await _eventService.GetTeachersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teachers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newEvent = new Event
                {
                    Name = txtName.Text,
                    EventDate = DateTime.Parse(txtEventDate.Text),
                    Location = txtLocation.Text,
                    Description = txtDescription.Text
                };
                await _eventService.AddEventAsync(newEvent); // Chama o método correto AddEventAsync
                LoadEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
          
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedEvent = (Event)dgEvents.SelectedItem;
                selectedEvent.Name = txtName.Text;
                selectedEvent.EventDate = DateTime.Parse(txtEventDate.Text);
                selectedEvent.Location = txtLocation.Text;
                selectedEvent.Description = txtDescription.Text;
                await _eventService.UpdateEventAsync(selectedEvent); // Chama o método correto UpdateEventAsync
                LoadEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void btnAssociateStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgEvents.SelectedItem is Event selectedEvent && cbStudents.SelectedItem is Student selectedStudent)
                {
                    try
                    {
                        await _eventService.AssociateStudentToEventAsync(selectedEvent.Id, selectedStudent.Id);
                        MessageBox.Show("Student successfully associated.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void btnAssociateTeacher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgEvents.SelectedItem is Event selectedEvent && cbTeachers.SelectedItem is Teacher selectedTeacher)
                {
                    try
                    {
                        await _eventService.AssociateTeacherToEventAsync(selectedEvent.Id, selectedTeacher.Id);
                        MessageBox.Show("Teacher successfully associated.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedEvent = (Event)dgEvents.SelectedItem;
                await _eventService.DeleteEventAsync(selectedEvent.Id); // Chama o método correto DeleteEventAsync
                LoadEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error by delete event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
