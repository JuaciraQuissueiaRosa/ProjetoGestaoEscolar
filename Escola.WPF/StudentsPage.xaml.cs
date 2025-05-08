using Escola.WPF.Models;
using Escola.WPF.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for StudentsPage.xaml
    /// </summary>
    public partial class StudentsPage : Page
    {
        private readonly IDataService _dataService;  // Serviço para dados (API + SQLite local)

        public StudentsPage()
        {
            InitializeComponent();
            _dataService = new ApiService();
            LoadStudents();  // Carrega os alunos ao iniciar a página
        }

        private async void LoadStudents()
        {
            try
            {
                var students = await _dataService.GetStudentsAsync();  // Método correto para pegar os alunos
                dgStudents.ItemsSource = students;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newStudent = new Student
                {
                    FullName = txtFullName.Text,
                    BirthDate = dpBirthDate.SelectedDate ?? DateTime.Now,
                    Phone = txtPhone.Text,
                    Address = txtAddress.Text,
                    Email = txtEmail.Text
                };

                await _dataService.AddStudentAsync(newStudent);  // Chamada correta para adicionar um aluno
                LoadStudents();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error create the student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgStudents.SelectedItem is Student selectedStudent)
                {
                    try
                    {
                        selectedStudent.FullName = txtFullName.Text;
                        selectedStudent.BirthDate = dpBirthDate.SelectedDate ?? DateTime.Now;
                        selectedStudent.Phone = txtPhone.Text;
                        selectedStudent.Address = txtAddress.Text;
                        selectedStudent.Email = txtEmail.Text;

                        await _dataService.UpdateStudentAsync(selectedStudent);  // Chamada correta para editar um aluno
                        LoadStudents();
                        ClearForm();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error update the student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Choose one student to update", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error update the student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgStudents.SelectedItem is Student selectedStudent)
                {
                    var result = MessageBox.Show("Are you sure that you want to delete this student?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            await _dataService.DeleteStudentAsync(selectedStudent.Id);  // Chamada correta para excluir o aluno
                            LoadStudents();  // Recarrega a lista de alunos após a exclusão
                            ClearForm();  // Limpa o formulário de edição
                            MessageBox.Show("Student deleted with success.", "Information", MessageBoxButton.OK, MessageBoxImage.Information); // Confirmação de sucesso
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error by delete the student: {ex.Message}\nDetails: {ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Choose one student to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error by delete the student: {ex.Message}\nDetails: {ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();  // Pega o texto do campo de pesquisa
                if (string.IsNullOrEmpty(searchTerm))
                {
                    MessageBox.Show("Please enter a search term.");
                    return;
                }

                // Chama o serviço para buscar os alunos
                var students = await _dataService.SearchStudentsAsync(searchTerm);

                // Exibe os resultados na DataGrid ou de acordo com a sua UI
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
                MessageBox.Show($"Error by search the students: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
            catch(Exception ex)
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
    }
}

