using Escola.WPF.Models;
using Escola.WPF.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for StudentsWindow.xaml
    /// </summary>º
    /// 

    public partial class StudentsWindow : Window
    {
        private readonly IDataService _dataService;  // Serviço para dados (API + SQLite local)

        public StudentsWindow()
        {
            InitializeComponent();
            _dataService = new ApiService();
            LoadStudents();  // Load students when the window is initialized
        }

        /// <summary>
        /// Loads the list of students and populates the DataGrid.
        /// </summary>
        private async void LoadStudents()
        {
            try
            {
                var students = await _dataService.GetStudentsAsync();  
                dgStudents.ItemsSource = students;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        /// <summary>
        /// Creates a new student using input data.
        /// </summary>
        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateStudentInputs())
                    return;
                var newStudent = new Student
                {
                    FullName = txtFullName.Text,
                    BirthDate = dpBirthDate.SelectedDate ?? DateTime.MinValue,
                    Phone = txtPhone.Text,
                    Address = txtAddress.Text,
                    Email = txtEmail.Text
                };

                await _dataService.AddStudentAsync(newStudent); 
                LoadStudents();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating the student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Updates the selected student's information.
        /// </summary>
        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateStudentInputs())
                    return;
                if (dgStudents.SelectedItem is Student selectedStudent)
                {
                    try
                    {
                        selectedStudent.FullName = txtFullName.Text;
                        selectedStudent.BirthDate = dpBirthDate.SelectedDate ?? DateTime.MinValue;
                        selectedStudent.Phone = txtPhone.Text;
                        selectedStudent.Address = txtAddress.Text;
                        selectedStudent.Email = txtEmail.Text;

                        await _dataService.UpdateStudentAsync(selectedStudent); 
                        LoadStudents();
                        ClearForm();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Please select a student to update.", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating the student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Deletes the selected student after confirmation.
        /// </summary>
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgStudents.SelectedItem is Student selectedStudent)
                {
                    var result = MessageBox.Show("Are you sure that you want to delete this student?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        var response = await _dataService.DeleteStudentAsync(selectedStudent.Id);

                        if (!response.IsSuccessStatusCode)
                        {
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"Failed to delete student:\n{errorMessage}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        LoadStudents();  
                        ClearForm();   
                        MessageBox.Show("Student successfully deleted.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a student to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        /// <summary>
        /// Searches for students by name or other criteria.
        /// </summary>
        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();  
                if (string.IsNullOrEmpty(searchTerm))
                {
                    MessageBox.Show("Please enter a search term.");
                    return;
                }

              
                var students = await _dataService.SearchStudentsAsync(searchTerm);

              
                if (students.Any())
                {
                    dgStudents.ItemsSource = students;
                }
                else
                {
                    MessageBox.Show("No students matched the search criteria.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching students: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Displays the academic history of the selected student.
        /// </summary>
        private async void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgStudents.SelectedItem is Student selectedStudent)
                {
                    try
                    {
                        JObject historyData = await _dataService.GetStudentHistoryAsync(selectedStudent.Id);
                        if (historyData == null)
                        {
                            MessageBox.Show("Error by get the student history.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        string studentName = historyData["student"]?["fullName"]?.ToString() ?? "N/A";
                        string className = historyData["class"]?["name"]?.ToString() ?? "N/A";
                        JArray averages = (JArray)(historyData["averages"] ?? new JArray());
                        JArray marks = (JArray)(historyData["marks"] ?? new JArray());

                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("📘 Histórico do Aluno");
                        sb.AppendLine("_______________________________\n");
                        sb.AppendLine("👤 Nome: " + studentName);
                        sb.AppendLine("🏫 Turma: " + className);

                        sb.AppendLine("\n🗂️ Médias Finais:");
                        if (averages.Count == 0)
                        {
                            sb.AppendLine("• Nenhuma média disponível.");
                        }
                        else
                        {
                            foreach (var avg in averages)
                            {
                                string subject = avg["subjectName"]?.ToString() ?? "N/A";
                                string average = avg["average"]?.ToString() ?? "N/A";
                                sb.AppendLine($"• {subject}: {average}");
                            }
                        }

                        sb.AppendLine("\n📝 Avaliações Realizadas:");
                        if (marks.Count == 0)
                        {
                            sb.AppendLine("• Nenhuma avaliação registrada.");
                        }
                        else
                        {
                            foreach (var mark in marks)
                            {
                                string subject = mark["subjectName"]?.ToString() ?? "N/A";
                                string type = mark["assessmentType"]?.ToString() ?? "N/A";
                                string grade = mark["grade"]?.ToString() ?? "N/A";
                                string date = DateTime.TryParse(mark["assessmentDate"]?.ToString(), out DateTime d)
                                    ? d.ToString("dd/MM/yyyy")
                                    : "N/A";
                                string teacher = mark["teacherName"]?.ToString() ?? "N/A";

                                sb.AppendLine($"• {subject} - {type}");
                                sb.AppendLine($"  Grade: {grade}   Date: {date}   Teacher: {teacher}");
                            }
                        }

                        MessageBox.Show(sb.ToString(), "History of student", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error to see the history: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Selection one student.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error to see the history: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        private void ClearForm()
        {
            try
            {
                txtFullName.Clear();
                dpBirthDate.SelectedDate = null;
                txtPhone.Clear();
                txtAddress.Clear();
                txtEmail.Clear();
                txtSearch.Clear();
                dgStudents.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error to clean the inputs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void dgStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedStudent = dgStudents.SelectedItem as Student;
                if (selectedStudent != null)
                {
                    txtFullName.Text = selectedStudent.FullName;
                    dpBirthDate.SelectedDate = selectedStudent.BirthDate;
                    txtPhone.Text = selectedStudent.Phone;
                    txtAddress.Text = selectedStudent.Address;
                    txtEmail.Text = selectedStudent.Email;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error to selection student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private bool ValidateStudentInputs()
        {
            try
            {
                ClearStudentFieldBorders();

                if (string.IsNullOrWhiteSpace(txtFullName.Text))
                {
                    ShowValidationError(txtFullName, "O nome completo é obrigatório.");
                    return false;
                }

                if (dpBirthDate.SelectedDate == null)
                {
                    MessageBox.Show("A data de nascimento é obrigatória.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpBirthDate.BorderBrush = Brushes.Red;
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtPhone.Text))
                {
                    ShowValidationError(txtPhone, "O telefone é obrigatório.");
                    return false;
                }
                else
                {
                    // Validação de telefone português: deve ter 9 dígitos e começar com 2, 3 ou 9
                    Regex phoneRegex = new Regex(@"^(2\d{8}|3\d{8}|9\d{8})$");
                    if (!phoneRegex.IsMatch(txtPhone.Text))
                    {
                        ShowValidationError(txtPhone, "Número de telefone inválido para Portugal. Deve conter 9 dígitos e começar com 2, 3 ou 9.");
                        return false;
                    }
                }

                if (string.IsNullOrWhiteSpace(txtAddress.Text))
                {
                    ShowValidationError(txtAddress, "O endereço é obrigatório.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    ShowValidationError(txtEmail, "O email é obrigatório.");
                    return false;
                }
                else if (!IsValidEmail(txtEmail.Text))
                {
                    ShowValidationError(txtEmail, "Formato de email inválido.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao validar campos: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void ShowValidationError(TextBox field, string message)
        {
            MessageBox.Show(message, "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
            field.BorderBrush = Brushes.Red;
            field.ToolTip = message;
        }

        private void ClearStudentFieldBorders()
        {
            try
            {

                txtFullName.ClearValue(Border.BorderBrushProperty);
                txtPhone.ClearValue(Border.BorderBrushProperty);
                txtAddress.ClearValue(Border.BorderBrushProperty);
                txtEmail.ClearValue(Border.BorderBrushProperty);
                dpBirthDate.ClearValue(Border.BorderBrushProperty);

                txtFullName.ToolTip = null;
                txtPhone.ToolTip = null;
                txtAddress.ToolTip = null;
                txtEmail.ToolTip = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

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
