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
    /// Interaction logic for ClassesWindow.xaml
    /// </summary>
    public partial class ClassesWindow : Window
    {
        private readonly IDataService _dataService;
        private List<Class> _classes;

        public ClassesWindow()
        {
            try
            {
                InitializeComponent();
                _dataService = new ApiService();
                LoadClasses();  // Carrega as classes ao iniciar a página
                LoadStudents();
                LoadTeachers();
                LoadSubjects();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing page: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// method to load classes into the datagrid
        /// </summary>
        /// <returns></returns>

        private async Task LoadClasses()
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


        /// <summary>
        /// method to load students into the combobox
        /// </summary>
        /// <returns></returns>
        private async Task LoadStudents()
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


        /// <summary>
        /// method to load teachers into the combobox
        /// </summary>
        /// <returns></returns>
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
        /// method to load subjects into the combobox
        /// </summary>
        /// <returns></returns>
        private async Task LoadSubjects()
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

        /// <summary>
        /// method to create a new class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateClassInputs())
                    return;
                var newClass = new Class
                {
                    Name = txtClassName.Text,
                    Course = txtGroupName.Text,
                    AcademicYear = txtAcademicYear.Text,
                    Shift = txtSchedule.Text
                };

                try
                {
                    await _dataService.AddClassAsync(newClass);  // Cria uma nova classe via API
                    MessageBox.Show("Class created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadClasses();  // Atualiza a lista de classes
                    ClearForm();  // Limpa o formulário após criar a classe
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// method to edit a class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateClassInputs())
                    return;
                if (dgClasses.SelectedItem is Class selectedClass)
                {
                    selectedClass.Name = txtClassName.Text;
                    selectedClass.AcademicYear = txtAcademicYear.Text;
                    selectedClass.Course = txtGroupName.Text;
                    selectedClass.Shift = txtSchedule.Text;

                    try
                    {
                        await _dataService.UpdateClassAsync(selectedClass);  // Atualiza a classe via API
                        MessageBox.Show("Class updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadClasses();  // Atualiza a lista de classes
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// method to delete a class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgClasses.SelectedItem is Class selectedClass)
                {
                    var result = MessageBox.Show("Are you sure you want to delete this class?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            var response = await _dataService.DeleteClassAsync(selectedClass.Id);

                            if (!response.IsSuccessStatusCode)
                            {
                                var errorMessage = await response.Content.ReadAsStringAsync();

                                // Tenta extrair mensagem clara
                                if (!string.IsNullOrWhiteSpace(errorMessage))
                                {
                                    MessageBox.Show($"Was not possible to delete the class:\n{errorMessage}", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                                else
                                {
                                    MessageBox.Show("Was not possible to delete the class due to a unknown error.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }

                                return;
                            }

                            MessageBox.Show("Class deleted with success!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            await LoadClasses();
                            ClearForm();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error by delete the class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please, choose one class to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error by delete the class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// method to clear the form fields
        /// </summary>
        private void ClearForm()
        {
            txtClassName.Clear();
            txtGroupName.Clear();
            txtSchedule.Clear();
            txtAcademicYear.Clear();
        }


        /// <summary>
        /// method to handle the selection change in the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedClass = dgClasses.SelectedItem as Class;
                if (selectedClass != null)
                {
                    txtClassName.Text = selectedClass.Name;
                    txtGroupName.Text = selectedClass.Course;
                    txtAcademicYear.Text = selectedClass.AcademicYear;
                    txtSchedule.Text = selectedClass.Shift;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error by selecting the class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// method to associate a student, teacher or subject to a class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnAssociate_Click(object sender, RoutedEventArgs e)
        {
            try
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

                    await LoadClasses(); // Atualiza o DataGrid com os dados atualizados da turma
                    MessageBox.Show(string.Join("\n", messages), "Association Result", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Please select a class first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// method to remove a student, subject or teacher from a class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnRemoveFromClass_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgClasses.SelectedItem is not Class selectedClass)
                {
                    MessageBox.Show("Choose one class.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var classId = selectedClass.Id;
                var messages = new List<string>();

              
                if (cbStudents.SelectedValue is int studentId)
                {
                    bool result = await _dataService.RemoveStudentFromClassAsync(classId, studentId);
                    messages.Add(result ? "✓ Student removed." : "✗ Error by removing the student.");
                }

              
                if (cbTeachers.SelectedValue is int teacherId)
                {
                    bool result = await _dataService.RemoveTeacherFromClassAsync(classId, teacherId);
                    messages.Add(result ? "✓ Teacher remover." : "✗ Error by removing the teacher.");
                }

              
                if (cbSubjects.SelectedValue is int subjectId)
                {
                    bool result = await _dataService.RemoveSubjectFromClassAsync(classId, subjectId);
                    messages.Add(result ? "✓ Subject removed." : "✗ Error by removing the subject.");
                }

                if (messages.Count == 0)
                {
                    MessageBox.Show("Any selected item to remove.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                await LoadClasses(); // Atualiza a interface
                MessageBox.Show(string.Join("\n", messages), "Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// method to validate the inputs of the class
        /// </summary>
        /// <returns></returns>
        private bool ValidateClassInputs()
        {
            try
            {
                ClearClassFieldBorders();

                if (string.IsNullOrWhiteSpace(txtClassName.Text))
                {
                    HighlightError(txtClassName, "The class name is mandatory.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtGroupName.Text))
                {
                    HighlightError(txtGroupName, "The Group Name is mandatory.");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(txtAcademicYear.Text))
                {
                    HighlightError(txtAcademicYear, "The Academic Year is mandatory.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtSchedule.Text))
                {
                    HighlightError(txtSchedule, "The schedule is mandatory.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error by validating the class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// method to highlight the error in the class fields
        /// </summary>
        /// <param name="control"></param>
        /// <param name="tooltip"></param>
        private void HighlightError(Control control, string tooltip)
        {
            control.BorderBrush = Brushes.Red;
            control.ToolTip = tooltip;
        }

        /// <summary>
        /// Method to clear the borders of the class fields
        /// </summary>
        private void ClearClassFieldBorders()
        {
            try
            {
                txtClassName.ClearValue(Border.BorderBrushProperty);
                txtGroupName.ClearValue(Border.BorderBrushProperty);
                txtSchedule.ClearValue(Border.BorderBrushProperty);

                txtClassName.ToolTip = null;
                txtGroupName.ToolTip = null;
                txtSchedule.ToolTip = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        /// <summary>
        /// Button to come back to the main menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

    

