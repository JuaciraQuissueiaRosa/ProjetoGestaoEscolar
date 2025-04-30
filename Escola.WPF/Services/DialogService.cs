using System.Windows;

namespace Escola.WPF.Services
{
    public class DialogService
    {
        public void ShowMessage(string title, string message)
        {
            MessageBox.Show(message, title);
        }
    }
}
