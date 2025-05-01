using Escola.WPF.Models;
using Escola.WPF.Services;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for EventsPage.xaml
    /// </summary>
    public partial class EventsPage : Page
    {
        private readonly IDataService _eventService;

        public EventsPage()
        {
            InitializeComponent();
            _eventService = new ApiService();
            LoadEvents();
        }

        private async void LoadEvents()
        {
            var events = await _eventService.GetEventsAsync(); // Chama o método correto GetEventsAsync
            dgEvents.ItemsSource = events;
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            var newEvent = new Event
            {
                Name = txtName.Text,
                EventDate = DateTime.Parse(txtEventDate.Text),
                Location = txtLocation.Text,
                Description = txtDescription.Text
            };
            await _eventService.AddEventAsync(newEvent); // Chama o método correto AddEventAsync
            LoadEvents();
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedEvent = (Event)dgEvents.SelectedItem;
            selectedEvent.Name = txtName.Text;
            selectedEvent.EventDate = DateTime.Parse(txtEventDate.Text);
            selectedEvent.Location = txtLocation.Text;
            selectedEvent.Description = txtDescription.Text;
            await _eventService.UpdateEventAsync(selectedEvent); // Chama o método correto UpdateEventAsync
            LoadEvents();
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedEvent = (Event)dgEvents.SelectedItem;
            await _eventService.DeleteEventAsync(selectedEvent.Id); // Chama o método correto DeleteEventAsync
            LoadEvents();
        }
    }
}
