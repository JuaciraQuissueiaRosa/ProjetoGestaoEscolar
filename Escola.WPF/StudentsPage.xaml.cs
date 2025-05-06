using Escola.WPF.Models;
using Escola.WPF.Services;
using Newtonsoft.Json;
using System.Net.Http;
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
                        LoadStudents();
                        ClearForm();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao excluir aluno: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void ClearForm()
        {
            txtFullName.Clear();
            dpBirthDate.SelectedDate = null;
            txtPhone.Clear();
            txtAddress.Clear();
            txtEmail.Clear();
        }

        private void dgStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
    }
}

