using Escola.WPF.Models;
using Escola.WPF.Services;
using ScottPlot.TickGenerators.TimeUnits;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for MarksWindow.xaml
    /// </summary>
    public partial class MarksWindow : Window
    {
        private readonly IDataService _dataService;

        public MarksWindow()
        {
            InitializeComponent();
            _dataService = new ApiService();

            _ = InitializeAsync();
        }

        /// <summary>
        /// method to initialize the page
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync()
        {
            try
            {
                await LoadAssessmentTypes();
                await LoadStudents();
                await LoadSubjects();
                await LoadMarks();
                await LoadFinalAveragesAsync(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initialize page: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       /// <summary>
       /// method to load marks
       /// </summary>
       /// <returns></returns>
        private async Task LoadMarks()
        {

            dgMarks.ItemsSource = await _dataService.GetMarksAsync();
        }
        /// <summary>
        /// method to load students
        /// </summary>
        /// <returns></returns>
        private async Task LoadStudents()
        {
            cbStudents.ItemsSource = await _dataService.GetStudentsAsync();
        }
        /// <summary>
        /// method to load subjects
        /// </summary>
        /// <returns></returns>

        private async Task LoadSubjects()
        {
            cbSubjects.ItemsSource = await _dataService.GetSubjectsAsync();
        }

        /// <summary>
        /// method to load assessment types
        /// </summary>
        /// <returns></returns>

        private Task LoadAssessmentTypes()
        {
            var types = new List<string> { "Test", "Assignment", "Exam" };
            cbAssessmentType.ItemsSource = types;
            return Task.CompletedTask;
        }

        /// <summary>
        /// method to load final averages
        /// </summary>
        /// <returns></returns>
        private async Task LoadFinalAveragesAsync()
        {
            var response = await _dataService.GetFinalAveragesAsync(); // Preferencialmente via interface
            if (response != null)
            {
                dgFinalAverages.ItemsSource = response;
            }
            else
            {
                MessageBox.Show("Error loading final averages.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// method to create a new mark
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInputs()) return;

                if (cbStudents.SelectedValue == null || cbSubjects.SelectedValue == null)
                {
                    MessageBox.Show("Select a student and a subject.", "Required Fields", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var yearParts = txtAssessmentYear.Text.Split('/');
                if (yearParts.Length != 2 || !int.TryParse(yearParts[0], out _))
                {
                    MessageBox.Show("Enter a valid academic year in the format YYYY / YYYY.", "Invalid Format", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var mark = new Mark
                {
                    StudentId = Convert.ToInt32(cbStudents.SelectedValue),
                    SubjectId = Convert.ToInt32(cbSubjects.SelectedValue),
                    AssessmentType = cbAssessmentType.SelectedItem?.ToString(),
                    Grade = float.Parse(txtScore.Text),
                    AssessmentDate = txtAssessmentYear.Text.Trim()
                };

                var response = await _dataService.AddMarkAsync(mark);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Mark successfully added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // ✅ Recarrega tudo da API para garantir exibição correta
                    await LoadMarks();
                    await LoadFinalAveragesAsync(); // <- aqui é crucial
                    ClearInputs();
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error: {errorMessage}", "Error adding mark", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// method to edit a mark
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    await LoadFinalAveragesAsync(); // <- aqui é crucial
                    ClearInputs();
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error: {errorMessage}", "Error updating mark", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// method to delete a mark
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    await LoadFinalAveragesAsync(); // <- aqui é crucial
                    ClearInputs();
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error: {errorMessage}", "Error deleting mark", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting mark: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// method to reload final averages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnReloadFinalAverages_Click(object sender, RoutedEventArgs e)
        {
            await LoadFinalAveragesAsync();
        }

        /// <summary>
        /// method to clear inputs
        /// </summary>
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error to clean inputs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// method to validate inputs
        /// </summary>
        /// <returns></returns>
        private bool ValidateInputs()
        {
            try
            {
                ClearMarkFieldBorders();

                if (cbStudents.SelectedItem == null)
                {
                    HighlightError(cbStudents, "Select a subject.");
                    return false;
                }

                if (cbSubjects.SelectedItem == null)
                {
                    HighlightError(cbSubjects, "Select a subject.");
                    return false;
                }

                if (cbAssessmentType.SelectedItem == null)
                {
                    HighlightError(cbAssessmentType, "Select assessment type.");
                    return false;
                }

                if (!float.TryParse(txtScore.Text, out float score) || score < 0 || score > 20)
                {
                    HighlightError(txtScore, "Enter a score between 0 and 20.");
                    return false;
                }

                if (!Regex.IsMatch(txtAssessmentYear.Text, @"^\d{4}/\d{4}$"))
                {
                    HighlightError(txtAssessmentYear, "Insert the academic year in the format YYYY/YYYY.");
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
        /// <summary>
        /// method to highlight error
        /// </summary>
        /// <param name="control"></param>
        /// <param name="tooltip"></param>
        private void HighlightError(Control control, string tooltip)
        {
            control.BorderBrush = Brushes.Red;
            control.ToolTip = tooltip;
        }

        /// <summary>
        /// method to clear mark field borders
        /// </summary>
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

        /// <summary>
        /// method to handle selection change in marks data grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgMarks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedMark = dgMarks.SelectedItem as Mark;
                if (selectedMark != null)
                {
                    cbStudents.SelectedValue = selectedMark.StudentId;
                    cbSubjects.SelectedValue = selectedMark.SubjectId;
                    cbAssessmentType.SelectedItem = selectedMark.AssessmentType;
                    txtScore.Text = selectedMark.Grade.ToString("F2");

                   
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
                MessageBox.Show($"Error selecting mark: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// method to handle back to the main menu
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
    

