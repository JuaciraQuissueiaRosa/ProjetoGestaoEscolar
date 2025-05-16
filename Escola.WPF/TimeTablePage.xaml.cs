using Escola.WPF.Models;
using Escola.WPF.Services;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
                if (!ValidateInputs()) return;
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
                if (!ValidateInputs()) return;
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

        private bool ValidateInputs()
        {
            try
            {
                ClearFieldBorders();

                if (txtClassId.SelectedItem == null)
                {
                    HighlightError(txtClassId, "Selecione uma turma.");
                    return false;
                }

                if (txtSubjectId.SelectedItem == null)
                {
                    HighlightError(txtSubjectId, "Selecione uma disciplina.");
                    return false;
                }

                if (txtTeacherId.SelectedItem == null)
                {
                    HighlightError(txtTeacherId, "Selecione um professor.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtDayOfWeek.Text))
                {
                    HighlightError(txtDayOfWeek, "Indique o dia da semana.");
                    return false;
                }

                if (!TimeSpan.TryParse(txtStartTime.Text, out var start))
                {
                    HighlightError(txtStartTime, "Hora de início inválida.");
                    return false;
                }

                if (!TimeSpan.TryParse(txtEndTime.Text, out var end))
                {
                    HighlightError(txtEndTime, "Hora de fim inválida.");
                    return false;
                }

                if (end <= start)
                {
                    HighlightError(txtEndTime, "Hora de fim deve ser posterior à hora de início.");
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
            txtClassId.ClearValue(Border.BorderBrushProperty);
            txtSubjectId.ClearValue(Border.BorderBrushProperty);
            txtTeacherId.ClearValue(Border.BorderBrushProperty);
            txtDayOfWeek.ClearValue(Border.BorderBrushProperty);
            txtStartTime.ClearValue(Border.BorderBrushProperty);
            txtEndTime.ClearValue(Border.BorderBrushProperty);

            txtClassId.ToolTip = null;
            txtSubjectId.ToolTip = null;
            txtTeacherId.ToolTip = null;
            txtDayOfWeek.ToolTip = null;
            txtStartTime.ToolTip = null;
            txtEndTime.ToolTip = null;
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

        private void dgTimeTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selected = dgTimeTable.SelectedItem as TimeTable;
                if (selected != null)
                {
                    txtClassId.SelectedValue = selected.ClassId;
                    txtSubjectId.SelectedValue = selected.SubjectId;
                    txtTeacherId.SelectedValue = selected.TeacherId;
                    txtDayOfWeek.Text = selected.DayOfWeek;
                    txtStartTime.Text = selected.StartTime.ToString(@"hh\:mm");
                    txtEndTime.Text = selected.EndTime.ToString(@"hh\:mm");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao selecionar horário: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
