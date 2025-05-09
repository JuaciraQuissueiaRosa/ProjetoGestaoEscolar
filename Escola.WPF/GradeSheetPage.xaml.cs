using Escola.WPF.Models;
using Escola.WPF.Services;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
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
        private async void LoadPerformanceChart(int studentId)
        {
            try
            {
                var averages = await _dataService.GetFinalAveragesByStudent(studentId);

                if (averages == null || !averages.Any())
                {
                    MessageBox.Show("Este aluno não possui médias registradas.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Prepara os dados para o gráfico
                string[] labels = averages.Select(a => a.SubjectName).ToArray();
                double[] values = averages.Select(a => (double)a.Average).ToArray();

                // Cria o modelo do gráfico
                var plotModel = new PlotModel { Title = "Média por Disciplina" };

                // Cria uma série de barras
                var barSeries = new BarSeries
                {
                    ItemsSource = values.Select((value, index) => new BarItem { Value = value }).ToArray(),
                };

                // Adiciona a série ao gráfico
                plotModel.Series.Add(barSeries);

                // Define o eixo X
                plotModel.Axes.Add(new OxyPlot.Axes.CategoryAxis
                {
                    Position = OxyPlot.Axes.AxisPosition.Bottom,
                    Key = "Subjects",
                    ItemsSource = labels
                });

                // Define o eixo Y
                plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
                {
                    Position = OxyPlot.Axes.AxisPosition.Left,
                    Minimum = 0,
                    Maximum = 20
                });

                // Atribui o gráfico ao controle
                plotView.Model = plotModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar gráfico: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void dgGradeSheets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgGradeSheets.SelectedItem is GradeSheet selected)
            {
                LoadPerformanceChart(selected.StudentId);
            }
        }
    }
}
