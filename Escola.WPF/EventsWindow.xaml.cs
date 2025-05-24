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

        // Constructor: Initializes components and binds the Loaded event

        public EventsWindow()
        {
            InitializeComponent();
            _eventService = new ApiService();
            this.Loaded += EventsPage_Loaded;
        }

        // Triggered when the window is loaded
        private async void EventsPage_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeAsync();
        }

        // Initializes the page by loading all data
        private async Task InitializeAsync()
        {
            await LoadEvents();
            await LoadStudents();
            await LoadTeachers();
        }


        // Loads all events into the DataGrid
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


        // Loads all students into the ComboBox
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

        // Loads all teachers into the ComboBox
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

        // Refreshes event data and clears input fields
        private async Task RefreshEvents()
        {
            await LoadEvents();
            ClearInputs();
        }

        // Handles creation of a new event
        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateEventInputs()) return;

                var result = MessageBox.Show("Do you really want to create this event?", "Confirm Creation", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                MessageBox.Show("Event successfully created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handles editing an existing event
        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateEventInputs()) return;

                var selectedEvent = (Event)dgEvents.SelectedItem;
                if (selectedEvent == null)
                {
                    MessageBox.Show("Please select an event to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show("Do you really want to edit this event?", "Confirm Edit", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) return;

                selectedEvent.Name = txtName.Text;
                selectedEvent.EventDate = DateTime.Parse(dpEventDate.Text).Date;
                selectedEvent.EventTime = TimeSpan.Parse(txtEventTime.Text);
                selectedEvent.Location = txtLocation.Text;
                selectedEvent.Description = txtDescription.Text;

                await _eventService.UpdateEventAsync(selectedEvent);
                await RefreshEvents();
                MessageBox.Show("Event successfully updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // Associates a student to the selected event
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

        // Associates a teacher to the selected event
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

        // Removes a student from the selected event
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
        // Removes a teacher from the selected event
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

        // Deletes the selected event after confirmation
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedEvent = (Event)dgEvents.SelectedItem;
                if (selectedEvent == null)
                {
                    MessageBox.Show("Please select an event to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this event?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes) return;

                var response = await _eventService.DeleteEventAsync(selectedEvent.Id);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error deleting event:\n{error}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                await RefreshEvents();
                MessageBox.Show("Event successfully deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Validates all required event input fields
        private bool ValidateEventInputs()
        {
            try
            {
                ClearEventFieldBorders();

                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    ShowValidationError(txtName, "Event name is required.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(dpEventDate.Text) || !DateTime.TryParse(dpEventDate.Text, out _))
                {
                    ShowValidationError(dpEventDate, "Invalid or empty event date.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtEventTime.Text) || !TimeSpan.TryParse(txtEventTime.Text, out _))
                {
                    ShowValidationError(txtEventTime, "Invalid or empty event time.");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(txtLocation.Text))
                {
                    ShowValidationError(txtLocation, "Location is required.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    ShowValidationError(txtDescription, "Description is required.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error validating event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Clears all form input fields
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
        // Clears error indicators from input fields
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

        // Shows an error on a specific field
        private void ShowValidationError(TextBox field, string message)
        {
            MessageBox.Show(message, "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            field.BorderBrush = Brushes.Red;
            field.ToolTip = message;
        }

        // Updates form fields when a row is selected
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
                MessageBox.Show($"Error selecting event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Returns to the main menu window
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
