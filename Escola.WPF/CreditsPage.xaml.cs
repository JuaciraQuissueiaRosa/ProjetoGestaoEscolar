using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for CreditsPage.xaml
    /// </summary>
    public partial class CreditsPage : Page
    {
        private readonly Random _random = new();
        private readonly DispatcherTimer _timer = new();

        public CreditsPage()
        {
            InitializeComponent();
            Loaded += CreditsPage_Loaded;
        }

        private void CreditsPage_Loaded(object sender, RoutedEventArgs e)
        {
            _timer.Interval = TimeSpan.FromMilliseconds(150);
            _timer.Tick += (s, args) => LaunchConfetti();
            _timer.Start();
        }

        private void LaunchConfetti()
        {
            var size = _random.Next(5, 15);
            var color = new SolidColorBrush(Color.FromRgb(
                (byte)_random.Next(256),
                (byte)_random.Next(256),
                (byte)_random.Next(256)));

            var rect = new Rectangle
            {
                Width = size,
                Height = size,
                Fill = color
            };

            double startX = _random.NextDouble() * ConfettiCanvas.ActualWidth;
            Canvas.SetLeft(rect, startX);
            Canvas.SetTop(rect, 0);

            ConfettiCanvas.Children.Add(rect);

            // Animação de queda
            var anim = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(30) };
            double y = 0;
            anim.Tick += (s, e) =>
            {
                y += 5;
                if (y > ConfettiCanvas.ActualHeight)
                {
                    ConfettiCanvas.Children.Remove(rect);
                    ((DispatcherTimer)s).Stop();
                }
                else
                {
                    Canvas.SetTop(rect, y);
                }
            };
            anim.Start();
        }
    }
}
