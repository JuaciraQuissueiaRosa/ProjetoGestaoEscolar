using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Windows;
using System.Windows.Controls;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for TimeTablePage.xaml
    /// </summary>
    public partial class TimeTablesPage : Page
    {
        private readonly IDataService _dataService;

        public TimeTablesPage()
        {
            InitializeComponent();
            _dataService = new ApiService();
            LoadTimeTable();  // Carrega as informações ao iniciar a página
        }

        private async void LoadTimeTable()
        {
            try
            {
                var records = await _dataService.GetTimeTablesAsync();  // Obtém todos os registros da tabela
                dgTimeTable.ItemsSource = records;  // Preenche o DataGrid
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar os horários: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newRecord = new TimeTable
                {
                    ClassId = int.Parse(txtClassId.Text),
                    SubjectId = int.Parse(txtSubjectId.Text),
                    TeacherId = int.Parse(txtTeacherId.Text),
                    DayOfWeek = txtDayOfWeek.Text,
                    StartTime = TimeSpan.Parse(txtStartTime.Text),
                    EndTime = TimeSpan.Parse(txtEndTime.Text)
                };
                await _dataService.AddTimeTableAsync(newRecord);  // Adiciona o novo horário
                LoadTimeTable();  // Atualiza a lista após a criação
                ClearInputs();  // Limpa os campos de entrada
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar o horário: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = (TimeTable)dgTimeTable.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("Selecione um horário para editar.");
                return;
            }

            try
            {
                selected.ClassId = int.Parse(txtClassId.Text);
                selected.SubjectId = int.Parse(txtSubjectId.Text);
                selected.TeacherId = int.Parse(txtTeacherId.Text);
                selected.DayOfWeek = txtDayOfWeek.Text;
                selected.StartTime = TimeSpan.Parse(txtStartTime.Text);
                selected.EndTime = TimeSpan.Parse(txtEndTime.Text);

                await _dataService.UpdateTimeTableAsync(selected);  // Atualiza o horário selecionado
                LoadTimeTable();  // Atualiza a lista após a edição
                ClearInputs();  // Limpa os campos de entrada
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao editar o horário: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = (TimeTable)dgTimeTable.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("Selecione um horário para excluir.");
                return;
            }

            try
            {
                await _dataService.DeleteTimeTableAsync(selected.Id);  // Deleta o horário selecionado
                LoadTimeTable();  // Atualiza a lista após a exclusão
                ClearInputs();  // Limpa os campos de entrada
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir o horário: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearInputs()
        {
            txtClassId.Clear();
            txtSubjectId.Clear();
            txtTeacherId.Clear();
            txtDayOfWeek.Clear();
            txtStartTime.Clear();
            txtEndTime.Clear();
        }
    }
}
