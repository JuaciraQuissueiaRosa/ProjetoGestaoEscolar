using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Windows;
using System.Windows.Controls;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for DisciplinasPage.xaml
    /// </summary>
    public partial class SubjectsPage : Page
    {
        private readonly IDataService _dataService;  // Serviço para dados (API + SQLite local)

        public SubjectsPage()
        {
            InitializeComponent();
            _dataService = new ApiService();
            LoadSubjects();  // Carrega as disciplinas ao iniciar a página
        }

        private async void LoadSubjects()
        {
            try
            {
                var subjects = await _dataService.GetSubjectsAsync();  // Método correto para pegar as disciplinas
                dgSubjects.ItemsSource = subjects;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar as disciplinas: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newSubject = new Subject
                {
                    Name = txtSubjectName.Text,
                    WeeklyHours = int.Parse(txtWeeklyHours.Text)
                };

                await _dataService.AddSubjectAsync(newSubject);  // Chamada correta para adicionar a disciplina
                LoadSubjects();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar disciplina: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedSubject = (Subject)dgSubjects.SelectedItem;
            if (selectedSubject == null)
            {
                MessageBox.Show("Selecione uma disciplina para editar.");
                return;
            }

            try
            {
                selectedSubject.Name = txtSubjectName.Text;
                selectedSubject.WeeklyHours = int.Parse(txtWeeklyHours.Text);
                await _dataService.UpdateSubjectAsync(selectedSubject);  // Chamada correta para editar a disciplina
                LoadSubjects();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao editar disciplina: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedSubject = (Subject)dgSubjects.SelectedItem;
            if (selectedSubject == null)
            {
                MessageBox.Show("Selecione uma disciplina para apagar.");
                return;
            }

            try
            {
                await _dataService.DeleteSubjectAsync(selectedSubject.Id);  // Chamada correta para deletar a disciplina
                LoadSubjects();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao apagar disciplina: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearInputs()
        {
            txtSubjectName.Clear();
            txtWeeklyHours.Clear();
        }
    }
}

