using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Escola.WPF
{
    /// <summary>
    /// Interaction logic for CreditsWindow.xaml
    /// </summary>
    public partial class CreditsWindow : Window
    {
        private readonly Random _random = new Random();

        public CreditsWindow()
        {
            InitializeComponent();
            StartConfettiAnimation();
        }

        // Starts a timer that periodically creates falling confetti
        private void StartConfettiAnimation()
        {
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200) // Frequency of confetti generation
            };
            timer.Tick += (s, e) => CreateConfetti();
            timer.Start();
        }

        // Creates a single confetti piece and animates it falling
        private void CreateConfetti()
        {
            // Create a small square with a random color
            Rectangle confetti = new Rectangle
            {
                Width = 6,
                Height = 6,
                Fill = new SolidColorBrush(Color.FromRgb(
                    (byte)_random.Next(256),
                    (byte)_random.Next(256),
                    (byte)_random.Next(256)))
            };

            // Random starting position from left side
            double startX = _random.Next((int)ActualWidth);
            double endY = ActualHeight + 10; // Fall below the window

            Canvas.SetLeft(confetti, startX);
            Canvas.SetTop(confetti, -10); // Start above the window

            ConfettiCanvas.Children.Add(confetti);

            // Create animation to move the confetti from top to bottom
            DoubleAnimation fallAnimation = new DoubleAnimation
            {
                From = -10,
                To = endY,
                Duration = TimeSpan.FromSeconds(3),
                AccelerationRatio = 0.2 // Speed up the fall a bit
            };

            // Remove confetti from the canvas after animation ends
            fallAnimation.Completed += (s, e) => ConfettiCanvas.Children.Remove(confetti);

            // Start the animation
            confetti.BeginAnimation(Canvas.TopProperty, fallAnimation);
        }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow mainWindow)
                {
                    mainWindow.Show();
                    break;
                }
            }

            this.Close();
        }
    }
}
