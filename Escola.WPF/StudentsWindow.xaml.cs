using Escola.WPF.Models;
using Escola.WPF.Services;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for StudentsWindow.xaml
    /// </summary>º
    

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
                        sb.AppendLine("📘 Student History");
                        sb.AppendLine("_______________________________\n");
                        sb.AppendLine("👤 Name: " + studentName);
                        sb.AppendLine("🏫 Class: " + className);

                        sb.AppendLine("\n🗂️ Final Averages:");
                        if (averages.Count == 0)
                        {
                            sb.AppendLine("• No average available.");
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

                        sb.AppendLine("\n📝 Assessments:");
                        if (marks.Count == 0)
                        {
                            sb.AppendLine("•  No assessments recorded.");
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
                                sb.AppendLine($"• {subject} - {type}");
                                sb.AppendLine($"  Grade: {grade}   Date: {date}");
                            }
                        }

                        MessageBox.Show(sb.ToString(), "Student History", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Please select a student.", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a student.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving history: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Clears all form input fields and deselects the DataGrid.
        /// </summary>
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
                MessageBox.Show($"Error clearing inputs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Fills form fields with the selected student data from the DataGrid.
        /// </summary>
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
                MessageBox.Show($"Error selecting student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Validates the form fields before saving or updating a student.
        /// </summary>
        private bool ValidateStudentInputs()
        {
            try
            {
                ClearStudentFieldBorders();

                if (string.IsNullOrWhiteSpace(txtFullName.Text))
                {
                    ShowValidationError(txtFullName, "Full name is required.");
                    return false;
                }

                if (dpBirthDate.SelectedDate == null)
                {
                    MessageBox.Show("Birth date is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpBirthDate.BorderBrush = Brushes.Red;
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtPhone.Text))
                {
                    ShowValidationError(txtPhone, "Phone number is required.");
                    return false;
                }
                else
                {
                    
                    Regex phoneRegex = new Regex(@"^(2\d{8}|3\d{8}|9\d{8})$");
                    if (!phoneRegex.IsMatch(txtPhone.Text))
                    {
                        ShowValidationError(txtPhone, "Invalid phone number. Must be 9 digits and start with 2, 3, or 9.");
                        return false;
                    }
                }

                if (string.IsNullOrWhiteSpace(txtAddress.Text))
                {
                    ShowValidationError(txtAddress, "Address is required.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    ShowValidationError(txtEmail, "Email is required.");
                    return false;
                }
                else if (!IsValidEmail(txtEmail.Text))
                {
                    ShowValidationError(txtEmail, "Invalid email format.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error validating fields: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// Highlights a TextBox in red and shows a tooltip with the validation message.
        /// </summary>

        private void ShowValidationError(TextBox field, string message)
        {
            MessageBox.Show(message, "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            field.BorderBrush = Brushes.Red;
            field.ToolTip = message;
        }

        /// <summary>
        /// Resets all input field borders and tooltips.
        /// </summary>
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



        /// <summary>
        /// Validates if the email address is correctly formatted.
        /// </summary>
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


        /// <summary>
        /// Navigates back to the main menu window.
        /// </summary>
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
