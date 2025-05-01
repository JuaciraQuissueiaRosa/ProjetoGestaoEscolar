using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for ProfessoresPage.xaml
    /// </summary>
    public partial class TeachersPage : Page
    {
        private List<Teacher> _professors;  // Lista de professores
        private readonly IDataService _dataService;  // Serviço para dados (API + SQLite local)

        public TeachersPage()
        {
            InitializeComponent();
            _dataService = new ApiService();
            LoadProfessors();  // Carrega os professores ao iniciar a página
        }

        // Carrega a lista de professores (da API ou SQLite)
        private async void LoadProfessors()
        {
            try
            {
                _professors = await _dataService.GetTeachersAsync();  // Obtém os professores, primeiro da API, depois SQLite se necessário
                dgProfessors.ItemsSource = _professors;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading professors: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Evento de seleção de professor no DataGrid
        private void dgProfessors_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgProfessors.SelectedItem is Teacher selectedProfessor)
            {
                // Preenche os campos do formulário com os dados do professor selecionado
                txtProfessorName.Text = selectedProfessor.FullName;
                txtProfessorPhone.Text = selectedProfessor.Phone;
                txtProfessorEmail.Text = selectedProfessor.Email;
                txtProfessorTeachingArea.Text = selectedProfessor.TeachingArea;
            }
        }

        // Criar um novo professor
        private async void btnCreateProfessor_Click(object sender, RoutedEventArgs e)
        {
            var newProfessor = new Teacher
            {
                FullName = txtProfessorName.Text,
                Phone = txtProfessorPhone.Text,
                Email = txtProfessorEmail.Text,
                TeachingArea = txtProfessorTeachingArea.Text
            };

            try
            {
                // Adiciona professor na API e localmente
                await _dataService.AddTeacherAsync(newProfessor);
                MessageBox.Show("Professor created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadProfessors();  // Recarrega a lista de professores
                ClearProfessorForm();  // Limpa o formulário
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating professor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Editar um professor existente
        private async void btnEditProfessor_Click(object sender, RoutedEventArgs e)
        {
            if (dgProfessors.SelectedItem is Teacher selectedProfessor)
            {
                selectedProfessor.FullName = txtProfessorName.Text;
                selectedProfessor.Phone = txtProfessorPhone.Text;
                selectedProfessor.Email = txtProfessorEmail.Text;
                selectedProfessor.TeachingArea = txtProfessorTeachingArea.Text;

                try
                {
                    // Atualiza professor na API e localmente
                    await _dataService.UpdateTeacherAsync(selectedProfessor);
                    MessageBox.Show("Professor updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadProfessors();  // Recarrega a lista de professores
                    ClearProfessorForm();  // Limpa o formulário
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating professor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a professor to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Excluir um professor
        private async void btnDeleteProfessor_Click(object sender, RoutedEventArgs e)
        {
            if (dgProfessors.SelectedItem is Teacher selectedProfessor)
            {
                var result = MessageBox.Show("Are you sure you want to delete this professor?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Exclui professor da API e localmente
                        HttpResponseMessage response = await _dataService.DeleteTeacherAsync(selectedProfessor.Id);

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Professor deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadProfessors();  // Recarrega a lista de professores
                        }
                        else
                        {
                            string errorMessage = await response.Content.ReadAsStringAsync();
                            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        MessageBox.Show($"Error deleting professor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a professor to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Limpar o formulário de input
        private void ClearProfessorForm()
        {
            txtProfessorName.Clear();
            txtProfessorPhone.Clear();
            txtProfessorEmail.Clear();
            txtProfessorTeachingArea.Clear();
        }
    }
}

