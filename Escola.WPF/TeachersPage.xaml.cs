using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Windows;
using System.Windows.Controls;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for ProfessoresPage.xaml
    /// </summary>
    public partial class TeachersPage : Page
    {
        private readonly ApiService _apiService;
        private List<Teacher> _professors;
        private List<Subject> _subjects;
        public TeachersPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LoadProfessors();  // Load the professors when the page loads
            LoadSubjects();    // Load the subjects when the page loads
        }

        // Load Professors
        private async void LoadProfessors()
        {
            try
            {
                _professors = await _apiService.GetAsync<Teacher>("professors");
                dgProfessors.ItemsSource = _professors;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading professors: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Load Subjects
        private async void LoadSubjects()
        {
            try
            {
                _subjects = await _apiService.GetAsync<Subject>("subjects");
                dgSubjects.ItemsSource = _subjects;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subjects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Create a new professor
        private async void btnCreateProfessor_Click(object sender, RoutedEventArgs e)
        {
            var newProfessor = new Teacher
            {
                FullName = txtProfessorName.Text,
                Phone = txtProfessorPhone.Text,
                Email = txtProfessorEmail.Text
            };

            try
            {
                await _apiService.PostAsync("professors", newProfessor);
                MessageBox.Show("Professor created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadProfessors();  // Refresh the professor list
                ClearProfessorForm();  // Clear the form after creating a professor
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating professor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Edit an existing professor
        private async void btnEditProfessor_Click(object sender, RoutedEventArgs e)
        {
            if (dgProfessors.SelectedItem is Teacher selectedProfessor)
            {
                selectedProfessor.FullName = txtProfessorName.Text;
                selectedProfessor.Phone = txtProfessorPhone.Text;
                selectedProfessor.Email = txtProfessorEmail.Text;

                try
                {
                    await _apiService.PutAsync("professors/" + selectedProfessor.Id, selectedProfessor);
                    MessageBox.Show("Professor updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadProfessors();  // Refresh the professor list
                    ClearProfessorForm();  // Clear the form after editing a professor
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

        // Delete an existing professor
        private async void btnDeleteProfessor_Click(object sender, RoutedEventArgs e)
        {
            if (dgProfessors.SelectedItem is Teacher selectedProfessor)
            {
                var result = MessageBox.Show("Are you sure you want to delete this professor?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _apiService.DeleteAsync("professors/" + selectedProfessor.Id);
                        MessageBox.Show("Professor deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadProfessors();  // Refresh the professor list
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting professor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a professor to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Create a new subject
        private async void btnCreateSubject_Click(object sender, RoutedEventArgs e)
        {
            var newSubject = new Subject
            {
                Name = txtSubjectName.Text
            };

            try
            {
                await _apiService.PostAsync("subjects", newSubject);
                MessageBox.Show("Subject created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadSubjects();  // Refresh the subject list
                ClearSubjectForm();  // Clear the form after creating a subject
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Edit an existing subject
        private async void btnEditSubject_Click(object sender, RoutedEventArgs e)
        {
            if (dgSubjects.SelectedItem is Subject selectedSubject)
            {
                selectedSubject.Name = txtSubjectName.Text;

                try
                {
                    await _apiService.PutAsync("subjects/" + selectedSubject.Id, selectedSubject);
                    MessageBox.Show("Subject updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadSubjects();  // Refresh the subject list
                    ClearSubjectForm();  // Clear the form after editing a subject
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a subject to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Delete an existing subject
        private async void btnDeleteSubject_Click(object sender, RoutedEventArgs e)
        {
            if (dgSubjects.SelectedItem is Subject selectedSubject)
            {
                var result = MessageBox.Show("Are you sure you want to delete this subject?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _apiService.DeleteAsync("subjects/" + selectedSubject.Id);
                        MessageBox.Show("Subject deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadSubjects();  // Refresh the subject list
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a subject to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Clear Professor Form
        private void ClearProfessorForm()
        {
            txtProfessorName.Clear();
            txtProfessorPhone.Clear();
            txtProfessorEmail.Clear();
        }

        // Clear Subject Form
        private void ClearSubjectForm()
        {
            txtSubjectName.Clear();
        }
    }
}

