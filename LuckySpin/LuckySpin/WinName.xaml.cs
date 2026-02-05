using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace LuckySpin
{
    /// <summary>
    /// Interaction logic for WinName.xaml
    /// </summary>
    public partial class WinName : Window
    {
        MediaPlayer resultMusic = new MediaPlayer();

        public WinName()
        {
            InitializeComponent();
        }

        public void ShowWinner(string winnerName, string imagePath = null)
        {
            // Lấy kích thước màn hình thực tế
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double screenWidth = SystemParameters.PrimaryScreenWidth;

            // Tính toán FontSize động - VỪA PHẢI ĐỂ FIT MÀN HÌNH
            // Tiêu đề "Chúc mừng" = 18% chiều cao màn hình
            TitleText.FontSize = screenHeight * 0.18;

            // Tên người trúng = 15% hoặc 12% chiều cao màn hình (tùy độ dài tên)
            WinnerNameText.Text = winnerName;
            WinnerNameText.FontSize = winnerName.Length > 15 ? screenHeight * 0.12 : screenHeight * 0.15;
            WinnerNameText.MaxWidth = screenWidth * 0.95;

            WinnerOverlay.Visibility = Visibility.Visible;

            // Phát nhạc kết thúc
            try
            {
                string musicPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nhac_ket_thuc.mp3");
                if (System.IO.File.Exists(musicPath))
                {
                    resultMusic.Open(new Uri(musicPath));
                    resultMusic.Play();
                }
            }
            catch { }

            // Ảnh = 50% chiều cao màn hình - VỪA TO VỪA VỪA MÀN HÌNH!
            double imageSize = screenHeight * 0.50;

            // Hiển thị ảnh nếu có
            if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
            {
                WinnerImage.Source = new BitmapImage(new Uri(imagePath));
                WinnerImage.Width = imageSize;
                WinnerImage.Height = imageSize;
                WinnerImage.Visibility = Visibility.Visible;
            }
            else
            {
                WinnerImage.Visibility = Visibility.Collapsed;
            }

            // Animation zoom effect
            var sb = new Storyboard();

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
            resultMusic.Stop();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }
    }
}
