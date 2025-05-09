using Escola.WPF.Models;
using Escola.WPF.Services;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
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
            LoadTimeTable();
            LoadComboBoxes(); // <- importante!
        }

        private async void LoadTimeTable()
        {
            try
            {
                var records = await _dataService.GetTimeTablesAsync();
                dgTimeTable.ItemsSource = records;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar os horários: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadComboBoxes()
        {
            try
            {
                txtClassId.ItemsSource = await _dataService.GetClassesAsync();
                txtSubjectId.ItemsSource = await _dataService.GetSubjectsAsync();
                txtTeacherId.ItemsSource = await _dataService.GetTeachersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar comboboxes: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
      
        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newRecord = new TimeTable
                {
                    ClassId = (int)txtClassId.SelectedValue,
                    SubjectId = (int)txtSubjectId.SelectedValue,
                    TeacherId = (int)txtTeacherId.SelectedValue,
                    DayOfWeek = txtDayOfWeek.Text,
                    StartTime = TimeSpan.Parse(txtStartTime.Text),
                    EndTime = TimeSpan.Parse(txtEndTime.Text)
                };

                await _dataService.AddTimeTableAsync(newRecord);
                LoadTimeTable();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar o horário: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgTimeTable.SelectedItem is not TimeTable selected)
            {
                MessageBox.Show("Selecione um horário para editar.");
                return;
            }

            try
            {
                selected.ClassId = (int)txtClassId.SelectedValue;
                selected.SubjectId = (int)txtSubjectId.SelectedValue;
                selected.TeacherId = (int)txtTeacherId.SelectedValue;
                selected.DayOfWeek = txtDayOfWeek.Text;
                selected.StartTime = TimeSpan.Parse(txtStartTime.Text);
                selected.EndTime = TimeSpan.Parse(txtEndTime.Text);

                await _dataService.UpdateTimeTableAsync(selected);
                LoadTimeTable();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao editar o horário: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgTimeTable.SelectedItem is not TimeTable selected)
            {
                MessageBox.Show("Selecione um horário para excluir.");
                return;
            }

            try
            {
                await _dataService.DeleteTimeTableAsync(selected.Id);
                LoadTimeTable();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir o horário: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearInputs()
        {
            txtClassId.SelectedIndex = -1;
            txtSubjectId.SelectedIndex = -1;
            txtTeacherId.SelectedIndex = -1;
            txtDayOfWeek.Clear();
            txtStartTime.Clear();
            txtEndTime.Clear();
        }

    
    }
}
