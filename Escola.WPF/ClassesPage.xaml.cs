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
        private readonly IDataService _dataService;
        private List<Class> _classes;

        public ClassesPage()
        {
            InitializeComponent();
            _dataService = new ApiService();  // Serviço API para Classes
            LoadClasses();  // Carrega as classes ao iniciar a página
            LoadStudents();
            LoadTeachers();
            LoadSubjects();
        }

        private async void LoadClasses()
        {
            try
            {
                _classes = await _dataService.GetClassesAsync();  // Obtém as classes da API
                dgClasses.ItemsSource = _classes;  // Preenche o DataGrid
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading classes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadStudents()
        {
            try
            {
                var students = await _dataService.GetStudentsAsync();
                cbStudents.ItemsSource = students;
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
                var teachers = await _dataService.GetTeachersAsync();
                cbTeachers.ItemsSource = teachers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teachers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadSubjects()
        {
            try
            {
                var subjects = await _dataService.GetSubjectsAsync();
                cbSubjects.ItemsSource = subjects;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subjects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


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
                await _dataService.AddClassAsync(newClass);  // Cria uma nova classe via API
                MessageBox.Show("Class created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadClasses();  // Atualiza a lista de classes
                ClearForm();  // Limpa o formulário após criar a classe
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgClasses.SelectedItem is Class selectedClass)
            {
                selectedClass.Name = txtClassName.Text;
                selectedClass.Course = txtGroupName.Text;
                selectedClass.Shift = txtSchedule.Text;

                try
                {
                    await _dataService.UpdateClassAsync(selectedClass);  // Atualiza a classe via API
                    MessageBox.Show("Class updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadClasses();  // Atualiza a lista de classes
                    ClearForm();
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
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgClasses.SelectedItem is Class selectedClass)
            {
                var result = MessageBox.Show("Are you sure you want to delete this class?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _dataService.DeleteClassAsync(selectedClass.Id);  // Deleta a classe via API
                        MessageBox.Show("Class deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadClasses();  // Atualiza a lista de classes
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
        }

        private void ClearForm()
        {
            txtClassName.Clear();
            txtGroupName.Clear();
            txtSchedule.Clear();
        }

        private void dgClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void btnAssociate_Click(object sender, RoutedEventArgs e)
        {
            if (dgClasses.SelectedItem is Class selectedClass)
            {
                var classId = selectedClass.Id;
                var messages = new List<string>();

                // Associar Aluno
                if (cbStudents.SelectedValue is int studentId)
                {
                    bool result = await _dataService.AssociateStudentToClassAsync(classId, studentId);
                    messages.Add(result ? $"✓ Student associated." : "✗ Failed to associate student.");
                }

                // Associar Professor
                if (cbTeachers.SelectedValue is int teacherId)
                {
                    bool result = await _dataService.AssociateTeacherToClassAsync(classId, teacherId);
                    messages.Add(result ? $"✓ Teacher associated." : "✗ Failed to associate teacher.");
                }

                // Associar Disciplina
                if (cbSubjects.SelectedValue is int subjectId)
                {
                    bool result = await _dataService.AssociateSubjectToClassAsync(classId, subjectId);
                    messages.Add(result ? $"✓ Subject associated." : "✗ Failed to associate subject.");
                }

                MessageBox.Show(string.Join("\n", messages), "Association Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a class first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
