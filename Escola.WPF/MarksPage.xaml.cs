using Escola.WPF.Models;
using Escola.WPF.Services;
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
            try
            {
                InitializeComponent();
                _dataService = new ApiService();
                LoadMarks();  // Carrega as notas ao iniciar a página
                LoadStudents();
                LoadSubjects();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initialize page: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadMarks()
        {
            try
            {
                var marks = await _dataService.GetMarksAsync();  // Corrigido para chamar o serviço sem tipo genérico
                dgMarks.ItemsSource = marks;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading marks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadStudents()
        {
            try
            {
                cbStudents.ItemsSource = await _dataService.GetStudentsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading students: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private async void LoadSubjects()
        {
            try
            {
                cbSubjects.ItemsSource = await _dataService.GetSubjectsAsync();
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
                if (!ValidateInputs()) return;

                var newMark = new Mark
                {
                    StudentId = (int)cbStudents.SelectedValue,
                    SubjectId = (int)cbSubjects.SelectedValue,
                    AssessmentType = txtAssessmentType.Text,
                    Grade = float.Parse(txtScore.Text),
                    AssessmentDate = dpAssessmentDate.SelectedDate ?? DateTime.Now,
                    TeacherId = 1 // Ou selecionar via ComboBox no futuro
                };

                await _dataService.AddMarkAsync(newMark);
                LoadMarks();
                LoadSubjects();
                LoadStudents();
                ClearInputs();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating mark: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedMark = (Mark)dgMarks.SelectedItem;
                if (selectedMark == null)
                {
                    MessageBox.Show("You must have to selection one mark to update.");
                    return;
                }

                try
                {
                    if (!ValidateInputs()) return;
                    selectedMark.StudentId = (int)cbStudents.SelectedValue;
                    selectedMark.SubjectId = (int)cbSubjects.SelectedValue;
                    selectedMark.AssessmentType = txtAssessmentType.Text;
                    selectedMark.Grade = float.Parse(txtScore.Text);
                    selectedMark.AssessmentDate = dpAssessmentDate.SelectedDate ?? DateTime.Now;
                    selectedMark.TeacherId = 1;

                    await _dataService.UpdateMarkAsync(selectedMark);
                    LoadMarks();
                    LoadSubjects();
                    LoadStudents();
                  
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error to update mark: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error to update mark: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedMark = (Mark)dgMarks.SelectedItem;
            if (selectedMark == null)
            {
                MessageBox.Show("Selection one mark to delete.");
                return;
            }

            try
            {
                await _dataService.DeleteMarkAsync(selectedMark.Id);  // Chamada ao serviço para excluir a nota
                LoadMarks();
                LoadSubjects();
                LoadStudents();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error to delete mark: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearInputs()
        {
            try
            {
                cbStudents.SelectedIndex = -1;
                cbSubjects.SelectedIndex = -1;
                txtAssessmentType.Clear();
                txtScore.Clear();
                dpAssessmentDate.SelectedDate = null;
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

                if (string.IsNullOrWhiteSpace(txtAssessmentType.Text))
                {
                    HighlightError(txtAssessmentType, "Insira o tipo de avaliação.");
                    return false;
                }

                if (!float.TryParse(txtScore.Text, out float score) || score < 0 || score > 20)
                {
                    HighlightError(txtScore, "Insira uma nota entre 0 e 20.");
                    return false;
                }

                if (dpAssessmentDate.SelectedDate == null)
                {
                    HighlightError(dpAssessmentDate, "Selecione uma data de avaliação.");
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
            txtAssessmentType.ClearValue(Border.BorderBrushProperty);
            txtScore.ClearValue(Border.BorderBrushProperty);
            dpAssessmentDate.ClearValue(Border.BorderBrushProperty);

            cbStudents.ToolTip = null;
            cbSubjects.ToolTip = null;
            txtAssessmentType.ToolTip = null;
            txtScore.ToolTip = null;
            dpAssessmentDate.ToolTip = null;
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
                    txtAssessmentType.Text = selectedMark.AssessmentType;
                    txtScore.Text = selectedMark.Grade.ToString("F2");
                    dpAssessmentDate.SelectedDate = selectedMark.AssessmentDate;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting mark: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
