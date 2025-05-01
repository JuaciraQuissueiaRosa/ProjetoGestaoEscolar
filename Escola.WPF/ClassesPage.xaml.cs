using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Windows;
using System.Windows.Controls;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for ClassesPage.xaml
    /// </summary>
    public partial class ClassesPage : Page
    {
        private readonly ApiService _apiService;
        private List<Class> _classes;

        public ClassesPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LoadClasses();  // Load the classes when the page loads
        }

        private async void LoadClasses()
        {
            try
            {
                // Fetch the classes from the API and bind them to the DataGrid
                _classes = await _apiService.GetAsync<Class>("classes");
                dgClasses.ItemsSource = _classes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading classes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Create a new class
        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            var newClass = new Class
            {
                Name = txtClassName.Text,
                Course = txtGroupName.Text,
                Shift = txtSchedule.Text
            };

            try
            {
                await _apiService.PostAsync("classes", newClass);
                MessageBox.Show("Class created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadClasses();  // Refresh the class list
                ClearForm();  // Clear the form after creating a class
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ClearForm();
        }

        // Edit an existing class
        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgClasses.SelectedItem is Class selectedClass)
            {
                // Modify the selected class information
                selectedClass.Name = txtClassName.Text;
                selectedClass.Course = txtGroupName.Text;
                selectedClass.Shift = txtSchedule.Text;

                try
                {
                    await _apiService.PutAsync("classes/" + selectedClass.Id, selectedClass);
                    MessageBox.Show("Class updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadClasses();  // Refresh the class list
                    ClearForm();  // Clear the form after editing a class
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a class to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            ClearForm();
        }

        // Delete an existing class
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgClasses.SelectedItem is Class selectedClass)
            {
                var result = MessageBox.Show("Are you sure you want to delete this class?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _apiService.DeleteAsync("classes/" + selectedClass.Id);
                        MessageBox.Show("Class deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadClasses();  // Refresh the class list
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a class to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            ClearForm();
        }

        // Clear form fields after creating or editing a class
        private void ClearForm()
        {
            txtClassName.Clear();
            txtGroupName.Clear();
            txtSchedule.Clear();
        }
    }
}

