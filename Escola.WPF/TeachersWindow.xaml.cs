using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for TeachersWindow.xaml
    /// </summary>
    public partial class TeachersWindow : Window
    {
        private List<Teacher> _professors; 
        private readonly IDataService _dataService;  

        public TeachersWindow()
        {
            try
            {
                InitializeComponent();
                _dataService = new ApiService();
                LoadProfessors();  // Load the list of teachers on window initialization

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initialize page: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // Loads the list of teachers 
        private async void LoadProfessors()
        {
            try
            {
                _professors = await _dataService.GetTeachersAsync(); 
                dgProfessors.ItemsSource = _professors;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Teachers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event triggered when a teacher is selected in the DataGrid
        private void dgProfessors_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (dgProfessors.SelectedItem is Teacher selectedProfessor)
                {
                    // Populate the form fields with the selected teacher's data
                    txtProfessorName.Text = selectedProfessor.FullName;
                    txtProfessorPhone.Text = selectedProfessor.Phone;
                    txtProfessorEmail.Text = selectedProfessor.Email;
                    txtProfessorTeachingArea.Text = selectedProfessor.TeachingArea;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error on selection teachers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // Create a new teacher
        private async void btnCreateProfessor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateTeachersInputs()) return;
                var newProfessor = new Teacher
                {
                    FullName = txtProfessorName.Text,
                    Phone = txtProfessorPhone.Text,
                    Email = txtProfessorEmail.Text,
                    TeachingArea = txtProfessorTeachingArea.Text
                };

                try
                {
                   
                    await _dataService.AddTeacherAsync(newProfessor);
                    MessageBox.Show("Professor created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadProfessors();  
                    ClearProfessorForm();  
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating professor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating professor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // Edit an existing teacher
        private async void btnEditProfessor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateTeachersInputs()) return;
                if (dgProfessors.SelectedItem is Teacher selectedProfessor)
                {
                    selectedProfessor.FullName = txtProfessorName.Text;
                    selectedProfessor.Phone = txtProfessorPhone.Text;
                    selectedProfessor.Email = txtProfessorEmail.Text;
                    selectedProfessor.TeachingArea = txtProfessorTeachingArea.Text;

                    try
                    {
                     
                        await _dataService.UpdateTeacherAsync(selectedProfessor);
                        MessageBox.Show("Professor updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadProfessors(); 
                        ClearProfessorForm();  
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating professor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a professor to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating professor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // Delete a teacher
        private async void btnDeleteProfessor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgProfessors.SelectedItem is Teacher selectedProfessor)
                {
                    var result = MessageBox.Show("Are you sure you want to delete this professor?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                          
                            HttpResponseMessage response = await _dataService.DeleteTeacherAsync(selectedProfessor.Id);

                            if (response.IsSuccessStatusCode)
                            {
                                MessageBox.Show("Professor deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadProfessors(); 
                                ClearProfessorForm();
                            }
                            else
                            {
                                string errorMessage = await response.Content.ReadAsStringAsync();
                                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        catch (HttpRequestException ex)
                        {
                            MessageBox.Show($"Error deleting professor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select a professor to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // Validate teacher form inputs
        private bool ValidateTeachersInputs()
        {
            try
            {

                ClearFieldBorders();
                if (string.IsNullOrWhiteSpace(txtProfessorName.Text))
                {
                    MessageBox.Show("Teacher name is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    HighlightError(txtProfessorName);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtProfessorPhone.Text))
                {
                    MessageBox.Show("Teacher phone number is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    HighlightError(txtProfessorPhone);
                    return false;
                }
                else
                {
                    // Portuguese phone number validation (9 digits, starting with 2, 3 or 9)
                    Regex phoneRegex = new Regex(@"^(2\d{8}|3\d{8}|9\d{8})$");
                    if (!phoneRegex.IsMatch(txtProfessorPhone.Text))
                    {
                        MessageBox.Show("Invalid phone number. Must be 9 digits and start with 2, 3, or 9.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                        HighlightError(txtProfessorPhone);
                        return false;
                    }
                }

                if (string.IsNullOrWhiteSpace(txtProfessorEmail.Text))
                {
                    MessageBox.Show("Teacher email is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    HighlightError(txtProfessorEmail);
                    return false;
                }
                else if (!IsValidEmail(txtProfessorEmail.Text))
                {
                    MessageBox.Show("Invalid email format.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    HighlightError(txtProfessorEmail);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtProfessorTeachingArea.Text))
                {
                    MessageBox.Show("Teaching area is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    HighlightError(txtProfessorTeachingArea);
                    return false;
                }

                return true;
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }


        }

        // Clear the input form
        private void ClearProfessorForm()
        {
            try
            {
                txtProfessorName.Clear();
                txtProfessorPhone.Clear();
                txtProfessorEmail.Clear();
                txtProfessorTeachingArea.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // Highlight input with validation error
        private void HighlightError(Control control)
        {
            try
            {
                control.BorderBrush = Brushes.Red;
                control.BorderThickness = new Thickness(2);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error highlighting field.{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Reset field borders to default
        private void ClearFieldBorders()
        {
            try
            {
                txtProfessorName.ClearValue(System.Windows.Controls.Control.BorderBrushProperty);
                txtProfessorPhone.ClearValue(System.Windows.Controls.Control.BorderBrushProperty);
                txtProfessorEmail.ClearValue(System.Windows.Controls.Control.BorderBrushProperty);
                txtProfessorTeachingArea.ClearValue(System.Windows.Controls.Control.BorderBrushProperty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // Email format validation

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        // Navigate back to main menu
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
