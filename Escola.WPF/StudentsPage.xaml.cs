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
                MessageBox.Show($"Erro ao carregar os alunos: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Erro ao criar aluno: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
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
                    MessageBox.Show($"Erro ao editar aluno: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Selecione um aluno para editar.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem is Student selectedStudent)
            {
                var result = MessageBox.Show("Tem certeza de que deseja excluir este aluno?", "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _dataService.DeleteStudentAsync(selectedStudent.Id);  // Chamada correta para excluir o aluno
                        LoadStudents();  // Recarrega a lista de alunos após a exclusão
                        ClearForm();  // Limpa o formulário de edição
                        MessageBox.Show("Aluno excluído com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information); // Confirmação de sucesso
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao excluir aluno: {ex.Message}\nDetalhes: {ex.StackTrace}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecione um aluno para excluir.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show($"Ocorreu um erro ao realizar a busca: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem is Student selectedStudent)
            {
                try
                {
                    JObject historyData = await _dataService.GetStudentHistoryAsync(selectedStudent.Id);
                    if (historyData == null)
                    {
                        MessageBox.Show("Erro ao obter o histórico do aluno.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                            sb.AppendLine($"  Nota: {grade}   Data: {date}   Professor: {teacher}");
                        }
                    }

                    MessageBox.Show(sb.ToString(), "Histórico do Aluno", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao buscar histórico: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Selecione um aluno.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void ClearForm()
        {
            txtFullName.Clear();
            dpBirthDate.SelectedDate = null;
            txtPhone.Clear();
            txtAddress.Clear();
            txtEmail.Clear();
            txtSearch.Clear();
            dgStudents.SelectedItem = null;
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
                MessageBox.Show($"Ocorreu um erro ao selecionar o aluno: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}

