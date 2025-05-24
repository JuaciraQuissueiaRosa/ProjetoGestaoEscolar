using Escola.WPF.Models;
using Escola.WPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    /// Interaction logic for SubjectsWindow.xaml
    /// </summary>
    public partial class SubjectsWindow : Window
    {
        private readonly IDataService _dataService;  // Serviço para dados (API + SQLite local)

        public SubjectsWindow()
        {
            InitializeComponent();
            _dataService = new ApiService();

            // Load data when the window is loaded
            Loaded += async (s, e) =>
            {
                await ReloadSubjectsAsync();
                await LoadTeachers();
            };

        }

        /// <summary>
        /// Loads the list of subjects from the data service and updates the DataGrid.
        /// </summary>

        private async Task ReloadSubjectsAsync()
        {
            try
            {
                var subjects = await _dataService.GetSubjectsAsync();
                dgSubjects.ItemsSource = subjects;
                dgSubjects.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reloading subjects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Loads the list of teachers into the ComboBox.
        /// </summary>
        private async Task LoadTeachers()
        {
            try
            {
                var teachers = await _dataService.GetTeachersAsync();
                cbTeachers.ItemsSource = teachers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teachers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Creates a new subject based on user input.
        /// </summary>
        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateSubjectInputs()) return;
                var newSubject = new Subject
                {
                    Name = txtSubjectName.Text,
                    WeeklyHours = int.Parse(txtWeeklyHours.Text)
                };

                await _dataService.AddSubjectAsync(newSubject);  
                ReloadSubjectsAsync(); // Not awaited intentionally
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error create subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Edits the selected subject with updated values from input fields.
        /// </summary>
        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateSubjectInputs()) return;
                var selectedSubject = (Subject)dgSubjects.SelectedItem;
                if (selectedSubject == null)
                {
                    MessageBox.Show("You must have to selection one subject to update.");
                    return;
                }

                try
                {
                    selectedSubject.Name = txtSubjectName.Text;
                    selectedSubject.WeeklyHours = int.Parse(txtWeeklyHours.Text);
                    await _dataService.UpdateSubjectAsync(selectedSubject); 
                    ReloadSubjectsAsync(); // Not awaited intentionally
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Associates the selected teacher to the selected subject.
        /// </summary>
        private async void btnAssociateTeacher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgSubjects.SelectedItem is Subject selectedSubject && cbTeachers.SelectedItem is Teacher selectedTeacher)
                {
                    var response = await _dataService.AssociateTeacherToSubjectAsync(selectedSubject.Id, selectedTeacher.Id);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Teacher associated with success!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        await ReloadSubjectsAsync(); 
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Failed to associate teacher: {errorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please, select one subject and one teacher.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Removes a teacher from the selected subject.
        /// </summary>
        private async void btnRemoveTeacherFromSubject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgSubjects.SelectedItem is Subject selectedSubject &&
                    cbTeachers.SelectedItem is Teacher selectedTeacher)
                {
                    var confirm = MessageBox.Show(
                        $"Do you want to remove the teacher '{selectedTeacher.FullName}' from the subject {selectedSubject.Name}'?",
                         "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (confirm != MessageBoxResult.Yes) return;

                    bool success = await _dataService.RemoveTeacherFromSubjectAsync(selectedSubject.Id, selectedTeacher.Id);

                    if (success)
                    {
                        MessageBox.Show("Teacher successfully removed from subject!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await ReloadSubjectsAsync();// Atualiza a lista, se você tiver esse método
                    }
                    else
                    {
                        MessageBox.Show("Failed to remove teacher from subject.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a subject and a teacher.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing teacher: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Deletes the selected subject.
        /// </summary>
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgSubjects.SelectedItem is not Subject selectedSubject)
            {
                MessageBox.Show("Please select a subject to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete the subject \"{selectedSubject.Name}\"?",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                await _dataService.DeleteSubjectAsync(selectedSubject.Id);
                MessageBox.Show("Subject deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                await ReloadSubjectsAsync(); // Atualiza a lista do DataGrid
                ClearInputs(); // Limpa os campos do formulário
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"HTTP error: {httpEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        /// <summary>
        /// Clears the input fields and ComboBox selection.
        /// </summary>
        private void ClearInputs()
        {
            try
            {
                txtSubjectName.Clear();
                txtWeeklyHours.Clear();
                cbTeachers.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clean the inputs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Validates the input fields for creating/editing a subject.
        /// </summary>
        private bool ValidateSubjectInputs()
        {
            try
            {
                ClearFieldBorders();

                if (string.IsNullOrWhiteSpace(txtSubjectName.Text))
                {
                    HighlightError(txtSubjectName, "Subject name is required.");
                    return false;
                }

                if (!int.TryParse(txtWeeklyHours.Text, out int hours) || hours < 1)
                {
                    HighlightError(txtWeeklyHours, "Invalid weekly hours.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Validation error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// Highlights a field with an error message.
        /// </summary>
        private void HighlightError(Control control, string tooltip)
        {
            control.BorderBrush = Brushes.Red;
            control.ToolTip = tooltip;
        }


        /// <summary>
        /// Clears visual error indicators from input fields.
        /// </summary>
        private void ClearFieldBorders()
        {
            txtSubjectName.ClearValue(Border.BorderBrushProperty);
            txtWeeklyHours.ClearValue(Border.BorderBrushProperty);

            txtSubjectName.ToolTip = null;
            txtWeeklyHours.ToolTip = null;
        }


        /// <summary>
        /// Updates input fields when a subject is selected from the DataGrid.
        /// </summary>
        private void dgSubjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedSubject = dgSubjects.SelectedItem as Subject;
                if (selectedSubject != null)
                {
                    txtSubjectName.Text = selectedSubject.Name;
                    txtWeeklyHours.Text = selectedSubject.WeeklyHours.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Returns to the main menu and closes the current window.
        /// </summary>
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
    

