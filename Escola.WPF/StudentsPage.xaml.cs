using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Windows;
using System.Windows.Controls;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for StudentsPage.xaml
    /// </summary>
    public partial class StudentsPage : Page
    {
        private readonly ApiService _apiService;
        private List<Student> _students;

        public StudentsPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LoadStudents();  // Load the students when the page loads
        }

        private async void LoadStudents()
        {
            try
            {
                // Fetch students from the API and bind to the DataGrid
                _students = await _apiService.GetAsync<Student>("students");
                dgStudents.ItemsSource = _students;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading students: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Create a new student
        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            // Gather user input (from textboxes or custom dialog)
            var newStudent = new Student
            {
                FullName = txtFullName.Text,  // Replace with user input
                Phone = txtPhone.Text,       // Replace with user input
                Email = txtEmail.Text  // Replace with user input
            };

            try
            {
                await _apiService.PostAsync("students", newStudent);
                MessageBox.Show("Student created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadStudents();  // Refresh the student list
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ClearForm();
        }

        // Edit an existing student
        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem is Student selectedStudent)
            {
                // Modify the selected student's information (e.g., with a dialog or form)
                selectedStudent.FullName = txtFullName.Text; // Replace with user input
                selectedStudent.Phone = txtPhone.Text;     // Replace with user input
                selectedStudent.Email = txtEmail.Text; /*/ Replace with user input*/

                try
                {
                    await _apiService.PutAsync("students/" + selectedStudent.Id, selectedStudent);
                    MessageBox.Show("Student updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadStudents();  // Refresh the student list
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a student to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            ClearForm();
        }

        // Delete an existing student
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem is Student selectedStudent)
            {
                var result = MessageBox.Show("Are you sure you want to delete this student?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _apiService.DeleteAsync("students/" + selectedStudent.Id);
                        MessageBox.Show("Student deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadStudents();  // Refresh the student list
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a student to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            ClearForm();


        }

        // Clear form fields after creating or editing a student
        private void ClearForm()
        {
            txtFullName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
        }


    }
}

