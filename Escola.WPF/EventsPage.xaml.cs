using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
                if (!ValidateEventInputs())
                    return;
                var newEvent = new Event
                {
                    Name = txtName.Text,
                    EventDate = DateTime.Parse(txtEventDate.Text),
                    Location = txtLocation.Text,
                    Description = txtDescription.Text
                };
                await _eventService.AddEventAsync(newEvent); // Chama o método correto AddEventAsync
                LoadEvents(); // Atualiza grid
                dgEvents.Items.Refresh(); // <-- esse força atualização visual
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
                if (!ValidateEventInputs())
                    return;
                var selectedEvent = (Event)dgEvents.SelectedItem;
                selectedEvent.Name = txtName.Text;
                selectedEvent.EventDate = DateTime.Parse(txtEventDate.Text);
                selectedEvent.Location = txtLocation.Text;
                selectedEvent.Description = txtDescription.Text;
                await _eventService.UpdateEventAsync(selectedEvent); // Chama o método correto UpdateEventAsync
                LoadEvents(); // Atualiza grid
                dgEvents.Items.Refresh(); // <-- esse força atualização visual
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
                        LoadEvents(); // Atualiza grid
                        dgEvents.Items.Refresh(); // <-- esse força atualização visual
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
                        LoadEvents(); // Atualiza grid
                        dgEvents.Items.Refresh(); // <-- esse força atualização visual
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
        private async void btnRemoveStudent_Click(object sender, RoutedEventArgs e)
        {
            if (dgEvents.SelectedItem is Event selectedEvent && cbStudents.SelectedItem is Student selectedStudent)
            {
                try
                {
                    await _eventService.RemoveStudentFromEventAsync(selectedEvent.Id, selectedStudent.Id);
                    MessageBox.Show("Student removed from event.");
                    LoadEvents(); // Atualiza grid
                    dgEvents.Items.Refresh(); // <-- esse força atualização visual
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error removing student: {ex.Message}");
                }
            }
        }

        private async void btnRemoveTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (dgEvents.SelectedItem is Event selectedEvent && cbTeachers.SelectedItem is Teacher selectedTeacher)
            {
                try
                {
                    await _eventService.RemoveTeacherFromEventAsync(selectedEvent.Id, selectedTeacher.Id);
                    MessageBox.Show("Teacher removed from event.");
                    LoadEvents(); // Atualiza grid
                    dgEvents.Items.Refresh(); // <-- esse força atualização visual
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error removing teacher: {ex.Message}");
                }
            }
        }


        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedEvent = (Event)dgEvents.SelectedItem;
                await _eventService.DeleteEventAsync(selectedEvent.Id); // Chama o método correto DeleteEventAsync
                LoadEvents();
                dgEvents.Items.Refresh(); // <-- esse força atualização visual
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error by delete event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateEventInputs()
        {
            try
            {
                ClearEventFieldBorders();

                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    ShowValidationError(txtName, "O nome do evento é obrigatório.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtEventDate.Text) || !DateTime.TryParse(txtEventDate.Text, out _))
                {
                    ShowValidationError(txtEventDate, "Data do evento inválida ou vazia.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtLocation.Text))
                {
                    ShowValidationError(txtLocation, "A localização é obrigatória.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    ShowValidationError(txtDescription, "A descrição é obrigatória.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao validar o evento: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

   

        private void ClearEventFieldBorders()
        {
            txtName.ClearValue(Border.BorderBrushProperty);
            txtEventDate.ClearValue(Border.BorderBrushProperty);
            txtLocation.ClearValue(Border.BorderBrushProperty);
            txtDescription.ClearValue(Border.BorderBrushProperty);

            txtName.ToolTip = null;
            txtEventDate.ToolTip = null;
            txtLocation.ToolTip = null;
            txtDescription.ToolTip = null;
        }

        private void ShowValidationError(TextBox field, string message)
        {
            MessageBox.Show(message, "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
            field.BorderBrush = Brushes.Red;
            field.ToolTip = message;
        }

        private void dgEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedEvent = dgEvents.SelectedItem as Event;
                if (selectedEvent != null)
                {
                    txtName.Text = selectedEvent.Name;
                    txtEventDate.Text = selectedEvent.EventDate.ToString("yyyy-MM-dd");
                    txtLocation.Text = selectedEvent.Location;
                    txtDescription.Text = selectedEvent.Description;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao selecionar evento: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
