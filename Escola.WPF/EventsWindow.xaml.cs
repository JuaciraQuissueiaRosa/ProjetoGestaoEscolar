using Escola.WPF.Models;
using Escola.WPF.Services;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for EventsWindow.xaml
    /// </summary>
    public partial class EventsWindow : Window
    {
        private readonly IDataService _eventService;

        public EventsWindow()
        {
            InitializeComponent();
            _eventService = new ApiService();
            this.Loaded += EventsPage_Loaded;
        }

        private async void EventsPage_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadEvents();
            await LoadStudents();
            await LoadTeachers();
        }

        private async Task LoadEvents()
        {
            try
            {
                var events = await _eventService.GetEventsAsync();
                dgEvents.ItemsSource = events;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading events: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadStudents()
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

        private async Task LoadTeachers()
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

        private async Task RefreshEvents()
        {
            await LoadEvents();
            ClearInputs();
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateEventInputs()) return;

                var result = MessageBox.Show("Deseja realmente criar este evento?", "Confirmar Criação", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) return;

                var newEvent = new Event
                {
                    Name = txtName.Text,
                    EventDate = DateTime.Parse(dpEventDate.Text).Date, // Somente a data
                    EventTime = TimeSpan.Parse(txtEventTime.Text),     // Somente o horário
                    Location = txtLocation.Text,
                    Description = txtDescription.Text
                };

                await _eventService.AddEventAsync(newEvent);
                await RefreshEvents();
                MessageBox.Show("Evento criado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar evento: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateEventInputs()) return;

                var selectedEvent = (Event)dgEvents.SelectedItem;
                if (selectedEvent == null)
                {
                    MessageBox.Show("Selecione um evento para editar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show("Deseja realmente editar este evento?", "Confirmar Edição", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) return;

                selectedEvent.Name = txtName.Text;
                selectedEvent.EventDate = DateTime.Parse(dpEventDate.Text).Date;
                selectedEvent.EventTime = TimeSpan.Parse(txtEventTime.Text);
                selectedEvent.Location = txtLocation.Text;
                selectedEvent.Description = txtDescription.Text;

                await _eventService.UpdateEventAsync(selectedEvent);
                await RefreshEvents();
                MessageBox.Show("Evento editado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao editar evento: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        await RefreshEvents();
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
                        await RefreshEvents();
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
                    await RefreshEvents();
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

                    await RefreshEvents();
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
                if (selectedEvent == null)
                {
                    MessageBox.Show("Selecione um evento para deletar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show("Tem certeza que deseja excluir este evento?", "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes) return;

                var response = await _eventService.DeleteEventAsync(selectedEvent.Id);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao excluir evento:\n{error}", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                await RefreshEvents();
                MessageBox.Show("Evento deletado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao deletar evento: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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

                if (string.IsNullOrWhiteSpace(dpEventDate.Text) || !DateTime.TryParse(dpEventDate.Text, out _))
                {
                    ShowValidationError(dpEventDate, "Data do evento inválida ou vazia.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtEventTime.Text) || !TimeSpan.TryParse(txtEventTime.Text, out _))
                {
                    ShowValidationError(txtEventTime, "Hora do evento inválida ou vazia.");
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


        private void ClearInputs()
        {
            txtName.Clear();
            dpEventDate.Clear();
            txtEventTime.Clear();
            txtLocation.Clear();
            txtDescription.Clear();
            cbStudents.SelectedIndex = -1;
            cbTeachers.SelectedIndex = -1;
            dgEvents.SelectedIndex = -1;
            ClearEventFieldBorders();
        }
        private void ClearEventFieldBorders()
        {
            txtName.ClearValue(Border.BorderBrushProperty);
            txtLocation.ClearValue(Border.BorderBrushProperty);
            txtDescription.ClearValue(Border.BorderBrushProperty);
            dpEventDate.ClearValue(Border.BorderBrushProperty);
            dpEventDate.ClearValue(Border.BorderThicknessProperty);
            dpEventDate.ToolTip = null;
            txtEventTime.ToolTip = null;
            txtName.ToolTip = null;
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
                    dpEventDate.Text = selectedEvent.EventDate.ToString("yyyy-MM-dd"); // TextBox agora
                    txtEventTime.Text = selectedEvent.EventDate.ToString("HH:mm");     // Hora vem do EventDate
                    txtLocation.Text = selectedEvent.Location;
                    txtDescription.Text = selectedEvent.Description;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao selecionar evento: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow mainWindow)
                {
                    mainWindow.Show();
                    break;
                }
            }

            this.Close();
        }
    }
}
