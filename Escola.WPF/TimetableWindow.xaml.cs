using Escola.WPF.Models;
using Escola.WPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for TimetableWindow.xaml
    /// </summary>
    public partial class TimetableWindow : Window
    {
        private readonly IDataService _dataService;

        public TimetableWindow()
        {
            InitializeComponent();
            _dataService = new ApiService();
            LoadTimeTable();
            LoadComboBoxes(); // <- importante!
        }

        /// <summary>
        /// method to load the timetable
        /// </summary>
        /// <returns></returns>
        private async Task LoadTimeTable()
        {
            try
            {
                var records = await _dataService.GetTimeTablesAsync();
                dgTimeTable.ItemsSource = records;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error by loading the timetable: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// method to load the comboboxes with data
        /// </summary>
        /// <returns></returns>
        private async Task LoadComboBoxes()
        {
            try
            {
                txtClassId.ItemsSource = await _dataService.GetClassesAsync();
                txtSubjectId.ItemsSource = await _dataService.GetSubjectsAsync();
                txtTeacherId.ItemsSource = await _dataService.GetTeachersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error by loading the data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// method to create a new record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                var response = await _dataService.AddTimeTableAsync(newRecord);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error by creating the hour:\n{errorMessage}", "Hours conflict", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                LoadTimeTable();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error unexpected: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// method to edit a record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgTimeTable.SelectedItem is not TimeTable selected)
            {
                MessageBox.Show("Choose one hour to update.");
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

                var response = await _dataService.UpdateTimeTableAsync(selected);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error by update the hour:\n{errorMessage}", "Hours conflict", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                LoadTimeTable();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// method to delete a record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgTimeTable.SelectedItem is not TimeTable selected)
            {
                MessageBox.Show("Choose one hour to delete.");
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
                MessageBox.Show($"Error by deleting the hour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// method to validate the inputs
        /// </summary>
        /// <returns></returns>
        private bool ValidateInputs()
        {
            try
            {
                ClearFieldBorders();

                if (txtClassId.SelectedItem == null)
                {
                    HighlightError(txtClassId, "Choose one class.");
                    return false;
                }

                if (txtSubjectId.SelectedItem == null)
                {
                    HighlightError(txtSubjectId, "Choose one subject.");
                    return false;
                }

                if (txtTeacherId.SelectedItem == null)
                {
                    HighlightError(txtTeacherId, "Choose one teacher.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtDayOfWeek.Text))
                {
                    HighlightError(txtDayOfWeek, "Choose the day of the week.");
                    return false;
                }

                if (!TimeSpan.TryParse(txtStartTime.Text, out var start))
                {
                    HighlightError(txtStartTime, "Start time invalid.");
                    return false;
                }

                if (!TimeSpan.TryParse(txtEndTime.Text, out var end))
                {
                    HighlightError(txtEndTime, "End time invalid.");
                    return false;
                }

                if (end <= start)
                {
                    HighlightError(txtEndTime, "End time has to be after the start.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Validation error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// method to highlight the error
        /// </summary>
        /// <param name="control"></param>
        /// <param name="tooltip"></param>
        private void HighlightError(Control control, string tooltip)
        {
            control.BorderBrush = Brushes.Red;
            control.ToolTip = tooltip;
        }

        /// <summary>
        /// method to clear the field borders
        /// </summary>

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

        /// <summary>
        /// method to clear the inputs
        /// </summary>
        private void ClearInputs()
        {
            txtClassId.SelectedIndex = -1;
            txtSubjectId.SelectedIndex = -1;
            txtTeacherId.SelectedIndex = -1;
            txtDayOfWeek.Clear();
            txtStartTime.Clear();
            txtEndTime.Clear();
        }

        /// <summary>
        /// method to handle the selection changed event of the data grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                MessageBox.Show($"Selection error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// method to go back to the maiin menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow mainWindow)
                {
                    mainWindow.Show();
                    break;
                }
            }

            this.Close();
        }
    }
}
 
