using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for MarksPage.xaml
    /// </summary>
    public partial class MarksPage : Page
    {
        private readonly IDataService _dataService;

        public MarksPage()
        {
            InitializeComponent();
            _dataService = new ApiService();

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                await LoadAssessmentTypes();
                await LoadMarks();
                await LoadStudents();
                await LoadSubjects();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initialize page: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadMarks()
        {
            var marks = await _dataService.GetMarksAsync();
            dgMarks.ItemsSource = marks;
        }

        private async Task LoadStudents()
        {
            cbStudents.ItemsSource = await _dataService.GetStudentsAsync();
        }

        private async Task LoadSubjects()
        {
            cbSubjects.ItemsSource = await _dataService.GetSubjectsAsync();
        }

        private Task LoadAssessmentTypes()
        {
            var types = new List<string> { "Teste", "Trabalho", "Exame" };
            cbAssessmentType.ItemsSource = types;
            return Task.CompletedTask;
        }
        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInputs()) return;

                var yearParts = txtAssessmentYear.Text.Split('/');
                if (yearParts.Length != 2 || !int.TryParse(yearParts[0], out int startYear))
                {
                    MessageBox.Show("Insert a valid academic year in format YYYY/YYYY.", "Invalid Format", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var mark = new Mark
                {
                    StudentId = (int)cbStudents.SelectedValue,
                    SubjectId = (int)cbSubjects.SelectedValue,
                    AssessmentType = cbAssessmentType.SelectedItem?.ToString(),
                    Grade = float.Parse(txtScore.Text),
                    AssessmentDate = txtAssessmentYear.Text.Trim()
                };

                var response = await _dataService.AddMarkAsync(mark);

                if (response.IsSuccessStatusCode)
                {


                    await LoadMarks();
                    await LoadSubjects();
                    await LoadStudents();
                    ClearInputs();
                    // ✅ NOVO BLOCO: Buscar média final atualizada
                    int studentId = mark.StudentId;
                    int subjectId = mark.SubjectId;

                    float? average = await _dataService.GetFinalAverageAsync(studentId, subjectId);
                    // Atualizar média na label
                

                    if (average != null)
                    {
                        MessageBox.Show($"Média final atual: {average:F2}", "Média Calculada", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Não foi possível calcular a média para este aluno nesta disciplina.", "Média Indisponível", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro: {errorMessage}", "Erro ao adicionar nota", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedMark = (Mark)dgMarks.SelectedItem;
                if (selectedMark == null)
                {
                    MessageBox.Show("You must select one mark to update.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!ValidateInputs()) return;

                var yearParts = txtAssessmentYear.Text.Split('/');
                if (yearParts.Length != 2 || !int.TryParse(yearParts[0], out int startYear))
                {
                    MessageBox.Show("Insert a valid academic year in format YYYY/YYYY.", "Invalid Format", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                selectedMark.StudentId = (int)cbStudents.SelectedValue;
                selectedMark.SubjectId = (int)cbSubjects.SelectedValue;
                selectedMark.AssessmentType = cbAssessmentType.SelectedItem?.ToString();
                selectedMark.Grade = float.Parse(txtScore.Text);
                selectedMark.AssessmentDate = txtAssessmentYear.Text.Trim();

                var response = await _dataService.UpdateMarkAsync(selectedMark);

                if (response.IsSuccessStatusCode)
                {
                    await LoadMarks();
                    dgMarks.ItemsSource = null; // força o refresh
                    dgMarks.ItemsSource = await _dataService.GetMarksAsync();
                    await LoadSubjects();
                    await LoadStudents();
                    ClearInputs();

                    // ✅ NOVO BLOCO: Buscar média final atualizada
                    int studentId = selectedMark.StudentId;
                    int subjectId = selectedMark.SubjectId;

                    float? average = await _dataService.GetFinalAverageAsync(studentId, subjectId);
                    // Atualizar média na label
               

                    if (average != null)
                    {
                        MessageBox.Show($"Média final atual: {average:F2}", "Média Calculada", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Não foi possível calcular a média para este aluno nesta disciplina.", "Média Indisponível", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedMark = (Mark)dgMarks.SelectedItem;
            if (selectedMark == null)
            {
                MessageBox.Show("Select one mark to delete.");
                return;
            }

            try
            {
                var result = MessageBox.Show("Are you sure you want to delete this mark?",
                                             "Confirm Delete",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                var response = await _dataService.DeleteMarkAsync(selectedMark.Id);

                if (response.IsSuccessStatusCode)
                {
                    await LoadMarks();
                    await LoadSubjects();
                    await LoadStudents();
                    ClearInputs();
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro: {errorMessage}", "Erro ao apagar nota", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting mark: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ClearInputs()
        {
            try
            {
                cbStudents.SelectedIndex = -1;
                cbSubjects.SelectedIndex = -1;
                cbAssessmentType.SelectedIndex = -1;
                txtScore.Clear();
                txtAssessmentYear.Clear();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error to clean inputs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool ValidateInputs()
        {
            try
            {
                ClearMarkFieldBorders();

                if (cbStudents.SelectedItem == null)
                {
                    HighlightError(cbStudents, "Selecione um aluno.");
                    return false;
                }

                if (cbSubjects.SelectedItem == null)
                {
                    HighlightError(cbSubjects, "Selecione uma disciplina.");
                    return false;
                }

                if (cbAssessmentType.SelectedItem == null)
                {
                    HighlightError(cbAssessmentType, "Selecione o tipo de avaliação.");
                    return false;
                }

                if (!float.TryParse(txtScore.Text, out float score) || score < 0 || score > 20)
                {
                    HighlightError(txtScore, "Insira uma nota entre 0 e 20.");
                    return false;
                }

                if (!Regex.IsMatch(txtAssessmentYear.Text, @"^\d{4}/\d{4}$"))
                {
                    HighlightError(txtAssessmentYear,"Insert the academic year in the format YYYY/YYYY.");
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
        private void HighlightError(Control control, string tooltip)
        {
            control.BorderBrush = Brushes.Red;
            control.ToolTip = tooltip;
        }

        private void ClearMarkFieldBorders()
        {
            cbStudents.ClearValue(Border.BorderBrushProperty);
            cbSubjects.ClearValue(Border.BorderBrushProperty);
            cbAssessmentType.ClearValue(Border.BorderBrushProperty);
            txtScore.ClearValue(Border.BorderBrushProperty);
            txtAssessmentYear.ClearValue(Border.BorderBrushProperty);

            cbStudents.ToolTip = null;
            cbSubjects.ToolTip = null;
            cbAssessmentType.ToolTip = null;
            txtScore.ToolTip = null;
            txtAssessmentYear.ToolTip = null;
        }


        private void dgMarks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedMark = dgMarks.SelectedItem as Mark;
                if (selectedMark != null)
                {
                    cbStudents.SelectedValue = selectedMark.StudentId;
                    cbSubjects.SelectedValue = selectedMark.SubjectId;
                    cbAssessmentType.SelectedItem  = selectedMark.AssessmentType;
                    txtScore.Text = selectedMark.Grade.ToString("F2");

                    // Garantir o formato correto "yyyy/yyyy"
                    if (!string.IsNullOrWhiteSpace(selectedMark.AssessmentDate))
                    {
                        var parts = selectedMark.AssessmentDate.Split('/');
                        if (parts.Length == 2 &&
                            int.TryParse(parts[0], out int startYear) &&
                            int.TryParse(parts[1], out int endYear) &&
                            endYear == startYear + 1)
                        {
                            txtAssessmentYear.Text = $"{startYear}/{endYear}";
                        }
                        else
                        {
                            // Se o formato não for válido, limpar ou definir padrão
                            txtAssessmentYear.Text = string.Empty;
                        }
                    }
                    else
                    {
                        txtAssessmentYear.Text = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao selecionar nota: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

     
    }
}
