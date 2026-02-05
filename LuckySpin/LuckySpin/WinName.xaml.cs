using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace LuckySpin
{
    /// <summary>
    /// Interaction logic for WinName.xaml
    /// </summary>
    public partial class WinName : Window
    {
        public WinName()
        {
            InitializeComponent();
        }

        public void ShowWinner(string winnerName, string imagePath = null)
        {
            WinnerNameText.Text = winnerName;
            WinnerNameText.FontSize = winnerName.Length > 15 ? 280 : 300;
            WinnerOverlay.Visibility = Visibility.Visible;

            // Hiển thị ảnh nếu có
            if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
            {
                WinnerImage.Source = new BitmapImage(new Uri(imagePath));
                WinnerImage.Visibility = Visibility.Visible;
            }
            else
            {
                WinnerImage.Visibility = Visibility.Collapsed;
            }

            var sb = new Storyboard();

            // Zoom
            var scaleX = new DoubleAnimation(0.5, 1, TimeSpan.FromMilliseconds(500))
            {
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(scaleX, WinnerScale);
            Storyboard.SetTargetProperty(scaleX, new PropertyPath("ScaleX"));

            var scaleY = scaleX.Clone();
            Storyboard.SetTargetProperty(scaleY, new PropertyPath("ScaleY"));

            sb.Children.Add(scaleX);
            sb.Children.Add(scaleY);

            sb.Begin();
        }

        private void WinnerOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }
    }
}
