using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for SubjectsPage.xaml
    /// </summary>
    public partial class SubjectsPage : Page
    {
        private readonly IDataService _dataService;  // Serviço para dados (API + SQLite local)

        public SubjectsPage()
        {
            InitializeComponent();
            _dataService = new ApiService();

            Loaded += async (s, e) =>
            {
                await ReloadSubjectsAsync();
                await LoadTeachers();
            };

        }

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

                await _dataService.AddSubjectAsync(newSubject);  // Chamada correta para adicionar a disciplina
                ReloadSubjectsAsync();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error create subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
                    await _dataService.UpdateSubjectAsync(selectedSubject);  // Chamada correta para editar a disciplina
                    ReloadSubjectsAsync();
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
                        await ReloadSubjectsAsync(); // Atualiza a DataGrid automaticamente
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

        private async void btnRemoveTeacherFromSubject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgSubjects.SelectedItem is Subject selectedSubject &&
                    cbTeachers.SelectedItem is Teacher selectedTeacher)
                {
                    var confirm = MessageBox.Show(
                        $"Deseja remover o professor '{selectedTeacher.FullName}' da disciplina '{selectedSubject.Name}'?",
                        "Confirmar Remoção", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (confirm != MessageBoxResult.Yes) return;

                    bool success = await _dataService.RemoveTeacherFromSubjectAsync(selectedSubject.Id, selectedTeacher.Id);

                    if (success)
                    {
                        MessageBox.Show("Professor removido da disciplina com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        await ReloadSubjectsAsync();// Atualiza a lista, se você tiver esse método
                    }
                    else
                    {
                        MessageBox.Show("Falha ao remover o professor da disciplina.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Selecione uma disciplina e um professor.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao remover professor: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



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




        private void ClearInputs()
        {
            try
            {
                txtSubjectName.Clear();
                txtWeeklyHours.Clear();
                cbTeachers.SelectedIndex = -1;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error clean the inputs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
          
        }

        private bool ValidateSubjectInputs()
        {
            try
            {
                ClearFieldBorders();

                if (string.IsNullOrWhiteSpace(txtSubjectName.Text))
                {
                    HighlightError(txtSubjectName, "O nome da disciplina é obrigatório.");
                    return false;
                }

                if (!int.TryParse(txtWeeklyHours.Text, out int hours) || hours < 1)
                {
                    HighlightError(txtWeeklyHours, "Carga horária semanal inválida.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro de validação: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        private void HighlightError(Control control, string tooltip)
        {
            control.BorderBrush = Brushes.Red;
            control.ToolTip = tooltip;
        }

        private void ClearFieldBorders()
        {
            txtSubjectName.ClearValue(Border.BorderBrushProperty);
            txtWeeklyHours.ClearValue(Border.BorderBrushProperty);

            txtSubjectName.ToolTip = null;
            txtWeeklyHours.ToolTip = null;
        }

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
                MessageBox.Show($"Erro ao selecionar disciplina: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}

