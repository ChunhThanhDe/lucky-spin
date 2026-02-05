using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IOPath = System.IO.Path;

namespace LuckySpin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> rewards = new List<string>();
        int currentSpinIndex = 0; // lần quay hiện tại (0-based)
        List<InputData> rewardData = new List<InputData>();
        int spinCount = 0; // số lần đã quay

        Random rd = new Random();
        public MainWindow()
        {
            InitializeComponent();
            string InputFilePath = AppDomain.CurrentDomain.BaseDirectory + "Config\\INPUT.xlsx";
            var list = ExcelHelper.GetData<InputData>(InputFilePath, "");
            rewardData = list;
            rewards = list.Select(x => x.NAME).ToList();
            DrawWheel();
            StartBgAnimation();
        }
        public void StartBgAnimation()
        {
            // Tạo hiệu ứng chạy từ -5 độ đến 5 độ
            DoubleAnimation tiltAnim = new DoubleAnimation
            {
                From = -5,             // Nghiêng trái 5 độ
                To = 5,               // Nghiêng phải 5 độ
                Duration = TimeSpan.FromSeconds(1), // Thời gian chạy (3 giây)
                AutoReverse = true,    // Chạy xong tự động quay ngược lại
                RepeatBehavior = RepeatBehavior.Forever, // Lặp lại vô tận

                // Giúp chuyển động mượt hơn, chậm dần ở hai đầu
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            // Áp dụng animation vào thuộc tính Angle của RotateTransform
            bgRotate.BeginAnimation(RotateTransform.AngleProperty, tiltAnim);
        }
        void DrawWheel()
        {
            WheelCanvas.Children.Clear();
            int n = rewards.Count;
            double anglePerSlice = 360.0 / n;
            double radius = 150;

            // ⭐ Góc bắt đầu (thường là -90° để slice đầu tiên ở trên)
            double startAngle = -90;

            // 1. Định nghĩa bộ màu bạn muốn (Ví dụ bộ màu hiện đại, nổi bật)
            Color[] palette = new Color[]
            {
                (Color)ColorConverter.ConvertFromString("#FF595E"), // Đỏ san hô
                (Color)ColorConverter.ConvertFromString("#FFCA3A"), // Vàng nghệ
                (Color)ColorConverter.ConvertFromString("#8AC926"), // Xanh lá táo
                (Color)ColorConverter.ConvertFromString("#1982C4"), // Xanh dương
                (Color)ColorConverter.ConvertFromString("#6A4C93")  // Tím đậm
            };

            for (int i = 0; i < n; i++)
            {
                double currentAngle = startAngle + i * anglePerSlice;

                // 2. Lấy màu từ danh sách dựa trên chỉ số i
                // Dùng dấu % để nếu số ô (n) nhiều hơn số màu trong palette, nó sẽ lặp lại từ đầu
                Color selectedColor = palette[i % palette.Length];

                Path slice = new Path
                {
                    Fill = new SolidColorBrush(selectedColor),
                    Data = CreateSliceGeometry(currentAngle, anglePerSlice, radius),

                    // Mẹo: Thêm viền trắng mỏng để các miếng tách biệt rõ trên nền sọc đen trắng
                    Stroke = Brushes.White,
                    StrokeThickness = 1
                };

                WheelCanvas.Children.Add(slice);

                // Thêm ảnh nền nếu có (vẽ ngay sau mỗi slice)
                if (!string.IsNullOrEmpty(rewardData[i].IMAGE))
                {
                    string imgPath = IOPath.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", rewardData[i].IMAGE);
                    if (System.IO.File.Exists(imgPath))
                    {
                        ImageBrush imgBrush = new ImageBrush
                        {
                            ImageSource = new BitmapImage(new Uri(imgPath)),
                            Opacity = 0.35,
                            Stretch = Stretch.UniformToFill
                        };

                        Path imageSlice = new Path
                        {
                            Fill = imgBrush,
                            Data = CreateSliceGeometry(currentAngle, anglePerSlice, radius)
                        };

                        WheelCanvas.Children.Add(imageSlice);
                    }
                }
            }

            // 2️⃣ Vẽ text SAU cùng (để text nằm trên cùng)
            for (int i = 0; i < n; i++)
            {
                double currentAngle = startAngle + i * anglePerSlice;
                double midAngle = currentAngle + anglePerSlice / 2;

                // Tạo TextBlock
                TextBlock txt = new TextBlock
                {
                    Text = Shorten(rewards[i], 10),
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    FontSize = 12,
                    TextAlignment = TextAlignment.Center
                };

                // Đo kích thước
                txt.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                double txtWidth = txt.DesiredSize.Width;
                double txtHeight = txt.DesiredSize.Height;

                // ⭐ Góc xoay text (tránh chữ ngược)
                double textRotation = midAngle;
                if (midAngle > 90 && midAngle < 270)
                {
                    textRotation += 180;
                }

                // Transform
                txt.RenderTransform = new RotateTransform(textRotation);
                txt.RenderTransformOrigin = new Point(0.5, 0.5);

                // ⭐ Vị trí text (đẩy ra 75% bán kính để không bị đè ảnh)
                double textRadius = radius * 0.75;
                double radians = midAngle * Math.PI / 180.0;

                double x = radius + textRadius * Math.Cos(radians) - txtWidth / 2;
                double y = radius + textRadius * Math.Sin(radians) - txtHeight / 2;

                Canvas.SetLeft(txt, x);
                Canvas.SetTop(txt, y);

                WheelCanvas.Children.Add(txt);
            }
        }
        string Shorten(string text, int maxLength = 10)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Length <= maxLength ? text : text.Substring(0, maxLength - 1) + "…";
        }
        PathGeometry CreateSliceGeometry(double startAngle, double sweepAngle, double radius)
        {
            Point center = new Point(radius, radius);

            double startRad = startAngle * Math.PI / 180.0;
            double endRad = (startAngle + sweepAngle) * Math.PI / 180.0;

            Point startPoint = new Point(
                center.X + radius * Math.Cos(startRad),
                center.Y + radius * Math.Sin(startRad)
            );

            Point endPoint = new Point(
                center.X + radius * Math.Cos(endRad),
                center.Y + radius * Math.Sin(endRad)
            );

            PathFigure figure = new PathFigure { StartPoint = center };
            figure.Segments.Add(new LineSegment(startPoint, true));
            figure.Segments.Add(new ArcSegment
            {
                Point = endPoint,
                Size = new Size(radius, radius),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = sweepAngle > 180
            });
            figure.Segments.Add(new LineSegment(center, true));

            return new PathGeometry { Figures = { figure } };
        }

        private void Spin_Click(object sender, RoutedEventArgs e)
        {
            if (rewardData.Count == 0)
            {
                MessageBox.Show("🎯 Đã quay hết!");
                return;
            }

            int targetIndex = GetTargetIndexForThisSpin();
            int rounds = rd.Next(6, 10);

            double stopAngle = CalculateStopAngleByIndex(targetIndex);

            WheelRotate.BeginAnimation(RotateTransform.AngleProperty, null);
            WheelRotate.Angle = 0;

            DoubleAnimation anim = new DoubleAnimation
            {
                From = 0,
                To = rounds * 360 + stopAngle,
                Duration = TimeSpan.FromSeconds(4),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                FillBehavior = FillBehavior.HoldEnd
            };

            SpinButton.IsEnabled = false;

            anim.Completed += (s, _) =>
            {
                OnSpinCompleted(targetIndex);
                SpinButton.IsEnabled = true;
            };

            WheelRotate.BeginAnimation(RotateTransform.AngleProperty, anim);
        }



        double CalculateStopAngleByIndex(int targetIndex)
        {
            int n = rewards.Count;
            double sliceAngle = 360.0 / n;

            double startAngle = -90;   // đúng như DrawWheel
            double pointerAngle = -90; // kim ở 12h

            // ⚠️ CLOCKWISE sweep → đảo dấu index
            double sliceCenter =
                startAngle + (targetIndex + 0.5) * sliceAngle;

            // 🔴 PHẢI ĐẢO CHIỀU QUAY
            double stopAngle = -(sliceCenter - pointerAngle);

            stopAngle %= 360;
            if (stopAngle < 0) stopAngle += 360;

            return stopAngle;
        }


        int GetTargetIndexForThisSpin()
        {
            int spinNumber = spinCount + 1;

            // 1️⃣ Ưu tiên STT
            var fixedItem = rewardData
                .FirstOrDefault(x => x.STT.HasValue && x.STT.Value == spinNumber);

            if (fixedItem != null)
                return rewardData.IndexOf(fixedItem);

            // 2️⃣ Không có STT → random theo RATE
            return GetIndexByRate();
        }



        void OnSpinCompleted(int index)
        {
            string result = rewards[index];
            string imagePath = null;

            // Lấy đường dẫn ảnh nếu có
            if (!string.IsNullOrEmpty(rewardData[index].IMAGE))
            {
                imagePath = IOPath.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", rewardData[index].IMAGE);
            }

            var winner = new WinName();
            winner.ShowWinner(result, imagePath);
            winner.Show();

            if (rewardData[index].ISCLEAR)
            {
                // xoá giải đã trúng
                rewardData.RemoveAt(index);
                rewards.RemoveAt(index);
            }

            spinCount++;

            //WheelRotate.Angle = 0;
            DrawWheel();

            if (rewardData.Count == 0)
            {
                MessageBox.Show("🎯 Đã quay hết phần thưởng!");
                SpinButton.IsEnabled = false;
            }
        }

        int GetIndexByRate()
        {
            var candidates = rewardData
                .Select((item, index) => new
                {
                    index,
                    rate =
                        item.RATE.HasValue
                            ? (item.RATE.Value > 0 ? item.RATE.Value : 0)   // =0 → loại
                            : 1                                              // null → default
                })
                .Where(x => x.rate > 0)
                .ToList();

            if (candidates.Count == 0)
                throw new Exception("Không có item nào đủ điều kiện random (RATE > 0)");

            double totalRate = candidates.Sum(x => x.rate);

            double r = rd.NextDouble() * totalRate;

            double acc = 0;
            foreach (var c in candidates)
            {
                acc += c.rate;
                if (r <= acc)
                    return c.index;
            }

            // fallback an toàn
            return candidates.Last().index;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }
    }
}
