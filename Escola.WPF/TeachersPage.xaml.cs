using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for TeachersPage.xaml
    /// </summary>
    public partial class TeachersPage : Page
    {
        private List<Teacher> _professors;  // Lista de professores
        private readonly IDataService _dataService;  // Serviço para dados (API + SQLite local)

        public TeachersPage()
        {
            try
            {
                InitializeComponent();
                _dataService = new ApiService();
                LoadProfessors();  // Carrega os professores ao iniciar a página

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initialize page: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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
            try
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
            catch(Exception ex)
            {
                MessageBox.Show($"Error on selection teachers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
        }

        // Criar um novo professor
        private async void btnCreateProfessor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateTeachersInputs()) return;
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
            catch(Exception ex)
            {
                MessageBox.Show($"Error creating professor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
         
        }

        // Editar um professor existente
        private async void btnEditProfessor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateTeachersInputs()) return;
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
            catch(Exception ex)
            {
                MessageBox.Show($"Error updating professor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
          
        }

        // Excluir um professor
        private async void btnDeleteProfessor_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private bool ValidateTeachersInputs()
        {
            try
            {

                ClearFieldBorders();
                if (string.IsNullOrWhiteSpace(txtProfessorName.Text))
                {
                    MessageBox.Show("O nome do professor é obrigatório.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    HighlightError(txtProfessorName);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtProfessorPhone.Text))
                {
                    MessageBox.Show("O telefone do professor é obrigatório.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    HighlightError(txtProfessorPhone);
                    return false;
                }
                else
                {
                    // Validação de número de telefone português (9 dígitos, começa com 2, 3 ou 9)
                    Regex phoneRegex = new Regex(@"^(2\d{8}|3\d{8}|9\d{8})$");
                    if (!phoneRegex.IsMatch(txtProfessorPhone.Text))
                    {
                        MessageBox.Show("Número de telefone do professor inválido. Deve conter 9 dígitos e começar com 2, 3 ou 9.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                        HighlightError(txtProfessorPhone);
                        return false;
                    }
                }

                if (string.IsNullOrWhiteSpace(txtProfessorEmail.Text))
                {
                    MessageBox.Show("O email do professor é obrigatório.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    HighlightError(txtProfessorEmail);
                    return false;
                }
                else if (!IsValidEmail(txtProfessorEmail.Text))
                {
                    MessageBox.Show("Formato de email inválido.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    HighlightError(txtProfessorEmail);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtProfessorTeachingArea.Text))
                {
                    MessageBox.Show("A área de ensino é obrigatória.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    HighlightError(txtProfessorTeachingArea);
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


        // Limpar o formulário de input
        private void ClearProfessorForm()
        {
            try
            {
                txtProfessorName.Clear();
                txtProfessorPhone.Clear();
                txtProfessorEmail.Clear();
                txtProfessorTeachingArea.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void HighlightError(Control control)
        {
            try
            {
                control.BorderBrush = Brushes.Red;
                control.BorderThickness = new Thickness(2);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error highlighting field.{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFieldBorders()
        {
            try
            {
                txtProfessorName.ClearValue(System.Windows.Controls.Control.BorderBrushProperty);
                txtProfessorPhone.ClearValue(System.Windows.Controls.Control.BorderBrushProperty);
                txtProfessorEmail.ClearValue(System.Windows.Controls.Control.BorderBrushProperty);
                txtProfessorTeachingArea.ClearValue(System.Windows.Controls.Control.BorderBrushProperty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}

