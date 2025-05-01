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
            InitializeComponent();
            _dataService = new ApiService();
            LoadMarks();  // Carrega as notas ao iniciar a página
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
                MessageBox.Show($"Erro ao carregar as notas: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    AssessmentDate = DateTime.Now,
                    TeacherId = 1 // Exemplo de ID do professor
                };

                await _dataService.AddMarkAsync(newMark);  // Chamada ao serviço para adicionar a nota
                LoadMarks();  // Atualiza a lista de notas
                ClearInputs();  // Limpa os campos
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar nota: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedMark = (Mark)dgMarks.SelectedItem;
            if (selectedMark == null)
            {
                MessageBox.Show("Selecione uma nota para editar.");
                return;
            }

            try
            {
                selectedMark.StudentId = int.Parse(txtStudentId.Text);
                selectedMark.SubjectId = int.Parse(txtSubjectId.Text);
                selectedMark.AssessmentType = txtAssessmentType.Text;
                selectedMark.Grade = float.Parse(txtScore.Text);
                selectedMark.AssessmentDate = DateTime.Now;
                selectedMark.TeacherId = 1;

                await _dataService.UpdateMarkAsync(selectedMark);  // Chamada ao serviço para atualizar a nota
                LoadMarks();  // Atualiza a lista de notas
                ClearInputs();  // Limpa os campos
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao editar nota: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedMark = (Mark)dgMarks.SelectedItem;
            if (selectedMark == null)
            {
                MessageBox.Show("Selecione uma nota para apagar.");
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
                MessageBox.Show($"Erro ao apagar nota: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearInputs()
        {
            txtStudentId.Clear();
            txtSubjectId.Clear();
            txtAssessmentType.Clear();
            txtScore.Clear();
        }
    }
}
