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
                                    MessageBox.Show($"Não foi possível apagar a turma:\n{errorMessage}", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                                else
                                {
                                    MessageBox.Show("Não foi possível apagar a turma por um erro desconhecido.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }

                                return;
                            }

                            MessageBox.Show("Turma excluída com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                            await LoadClasses();
                            ClearForm();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erro ao apagar turma: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, selecione uma turma para excluir.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao apagar turma: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void ClearForm()
        {
            txtClassName.Clear();
            txtGroupName.Clear();
            txtSchedule.Clear();
            txtAcademicYear.Clear();
        }

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
                MessageBox.Show($"Erro ao selecionar turma: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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

        private async void btnRemoveFromClass_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgClasses.SelectedItem is not Class selectedClass)
                {
                    MessageBox.Show("Selecione uma turma.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var classId = selectedClass.Id;
                var messages = new List<string>();

                // Remover aluno
                if (cbStudents.SelectedValue is int studentId)
                {
                    bool result = await _dataService.RemoveStudentFromClassAsync(classId, studentId);
                    messages.Add(result ? "✓ Aluno removido." : "✗ Falha ao remover aluno.");
                }

                // Remover professor
                if (cbTeachers.SelectedValue is int teacherId)
                {
                    bool result = await _dataService.RemoveTeacherFromClassAsync(classId, teacherId);
                    messages.Add(result ? "✓ Professor removido." : "✗ Falha ao remover professor.");
                }

                // Remover disciplina
                if (cbSubjects.SelectedValue is int subjectId)
                {
                    bool result = await _dataService.RemoveSubjectFromClassAsync(classId, subjectId);
                    messages.Add(result ? "✓ Disciplina removida." : "✗ Falha ao remover disciplina.");
                }

                if (messages.Count == 0)
                {
                    MessageBox.Show("Nenhum item selecionado para remover.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                await LoadClasses(); // Atualiza a interface
                MessageBox.Show(string.Join("\n", messages), "Resultado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateClassInputs()
        {
            try
            {
                ClearClassFieldBorders();

                if (string.IsNullOrWhiteSpace(txtClassName.Text))
                {
                    HighlightError(txtClassName, "O nome da turma é obrigatório.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtGroupName.Text))
                {
                    HighlightError(txtGroupName, "O curso é obrigatório.");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(txtAcademicYear.Text))
                {
                    HighlightError(txtAcademicYear, "O ano letivo é obrigatório.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtSchedule.Text))
                {
                    HighlightError(txtSchedule, "O turno é obrigatório.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao validar turma: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        private void HighlightError(Control control, string tooltip)
        {
            control.BorderBrush = Brushes.Red;
            control.ToolTip = tooltip;
        }

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
                MessageBox.Show($"Erro ao validar turma: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);

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

    

