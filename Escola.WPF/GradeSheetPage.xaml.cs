using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Windows;
using System.Windows.Controls;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for GradeSheet.xaml
    /// </summary>
    public partial class GradeSheetsPage : Page
    {
        private readonly IDataService _dataService;

        public GradeSheetsPage()
        {
            InitializeComponent();
            _dataService = new ApiService();
            LoadGradeSheets();  // Carregar as fichas de notas
        }

        private async void LoadGradeSheets()
        {
            try
            {
                var gradeSheets = await _dataService.GetReportsAsync();  // Carrega as fichas de notas
                dgGradeSheets.ItemsSource = gradeSheets;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar as fichas de notas: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newGradeSheet = new GradeSheet
                {
                    StudentId = int.Parse(txtStudentId.Text),
                    ClassId = int.Parse(txtClassId.Text),
                    CreatedDate = dpCreatedDate.SelectedDate ?? DateTime.Now,  // Verifica se a data foi selecionada
                    Comments = txtComments.Text
                };

                await _dataService.AddReportAsync(newGradeSheet);  // Adiciona uma nova ficha de nota
                LoadGradeSheets();  // Atualiza a lista de fichas de notas
                ClearInputs();  // Limpa os campos de entrada
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar ficha de nota: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedGradeSheet = (GradeSheet)dgGradeSheets.SelectedItem;
            if (selectedGradeSheet == null)
            {
                MessageBox.Show("Selecione uma ficha de nota para editar.");
                return;
            }

            try
            {
                selectedGradeSheet.StudentId = int.Parse(txtStudentId.Text);
                selectedGradeSheet.ClassId = int.Parse(txtClassId.Text);
                selectedGradeSheet.CreatedDate = dpCreatedDate.SelectedDate ?? DateTime.Now;  // Data da edição
                selectedGradeSheet.Comments = txtComments.Text;

                await _dataService.UpdateReportAsync(selectedGradeSheet);  // Atualiza a ficha de nota
                LoadGradeSheets();  // Atualiza a lista de fichas de notas
                ClearInputs();  // Limpa os campos de entrada
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao editar ficha de nota: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedGradeSheet = (GradeSheet)dgGradeSheets.SelectedItem;
            if (selectedGradeSheet == null)
            {
                MessageBox.Show("Selecione uma ficha de nota para apagar.");
                return;
            }

            try
            {
                await _dataService.DeleteReportAsync(selectedGradeSheet.Id);  // Apaga a ficha de nota
                LoadGradeSheets();  // Atualiza a lista de fichas de notas
                ClearInputs();  // Limpa os campos de entrada
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao apagar ficha de nota: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearInputs()
        {
            txtStudentId.Clear();
            txtClassId.Clear();
            dpCreatedDate.SelectedDate = null;
            txtComments.Clear();
        }
    }
}
