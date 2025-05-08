using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Windows;
using System.Windows.Controls;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for SubjectsPage.xaml
    /// </summary>
    public partial class SubjectsPage : Page
    {
        private readonly IDataService _dataService;  // Serviço para dados (API + SQLite local)

        public SubjectsPage()
        {
            try
            {
                InitializeComponent();
                _dataService = new ApiService();
                LoadSubjects();  // Carrega as disciplinas ao iniciar a página
                LoadTeachers(); // Carrega os professores no ComboBox
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initialize page: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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
                MessageBox.Show($"Error loading subjects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            dgSubjects.Items.Refresh();
        }

        private async void LoadTeachers()
        {
            try
            {
                var teachers = await _dataService.GetTeachersAsync();
                cbTeachers.ItemsSource = teachers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teachers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Error create subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedSubject = (Subject)dgSubjects.SelectedItem;
                if (selectedSubject == null)
                {
                    MessageBox.Show("You must have to selection one subject to update.");
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
                    MessageBox.Show($"Error loading subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void btnAssociateTeacher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgSubjects.SelectedItem is Subject selectedSubject && cbTeachers.SelectedItem is Teacher selectedTeacher)
                {
                    try
                    {
                        await _dataService.AssociateTeacherToSubjectAsync(selectedSubject.Id, selectedTeacher.Id);
                        MessageBox.Show("Teacher associated with success!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error associate teacher: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please, selection one subject and one teacher.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedSubject = (Subject)dgSubjects.SelectedItem;
                if (selectedSubject == null)
                {
                    MessageBox.Show("Selection one subject to delete.");
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
                    MessageBox.Show($"Error delete subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error delete subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void ClearInputs()
        {
            try
            {
                txtSubjectName.Clear();
                txtWeeklyHours.Clear();
                cbTeachers.SelectedIndex = -1;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error clean the inputs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
          
        }
    }
}

