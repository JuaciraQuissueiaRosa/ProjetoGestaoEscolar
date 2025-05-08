using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Windows;
using System.Windows.Controls;

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

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newMark = new Mark
                {
                    StudentId = int.Parse(txtStudentId.Text),
                    SubjectId = int.Parse(txtSubjectId.Text),
                    AssessmentType = txtAssessmentType.Text,
                    Grade = float.Parse(txtScore.Text),
                    AssessmentDate = dpAssessmentDate.SelectedDate ?? DateTime.Now,
                    TeacherId = 1 // Ou selecionar via ComboBox no futuro
                };

                await _dataService.AddMarkAsync(newMark);
                LoadMarks();
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
                    selectedMark.StudentId = int.Parse(txtStudentId.Text);
                    selectedMark.SubjectId = int.Parse(txtSubjectId.Text);
                    selectedMark.AssessmentType = txtAssessmentType.Text;
                    selectedMark.Grade = float.Parse(txtScore.Text);
                    selectedMark.AssessmentDate = dpAssessmentDate.SelectedDate ?? DateTime.Now;
                    selectedMark.TeacherId = 1;

                    await _dataService.UpdateMarkAsync(selectedMark);
                    LoadMarks();
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
                LoadMarks();  // Atualiza a lista de notas
                ClearInputs();  // Limpa os campos
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
                txtStudentId.Clear();
                txtSubjectId.Clear();
                txtAssessmentType.Clear();
                txtScore.Clear();
                dpAssessmentDate.SelectedDate = null;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error to clean inputs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
