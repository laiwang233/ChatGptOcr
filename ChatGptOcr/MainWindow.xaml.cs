using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace ChatGptOcr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Window _overlayWindow;
        private System.Windows.Shapes.Rectangle _selectionRectangle;
        private Point _startPoint;
        private Point _endPoint;


        public MainWindow()
        {
            InitializeComponent();
        }

        /*/// <summary>
        /// 截图
        /// </summary>
        /// <param name="x">截图起始x轴</param>
        /// <param name="y">截取起始y轴</param>
        /// <param name="width">截图宽度</param>
        /// <param name="height">截图高度</param>
        /// <param name="fileName">保存路径加文件名</param>
        public void CapturePartialScreen(int x, int y, int width, int height, string fileName)
        {
            // 创建一个与截图区域大小相同的Bitmap对象
            using var bitmap = new Bitmap(width, height);
            // 使用Graphics.FromImage方法从Bitmap对象创建一个Graphics对象
            using var graphics = Graphics.FromImage(bitmap);
            
            // 使用CopyFromScreen方法从屏幕上截取指定区域
            graphics.CopyFromScreen(x, y, 0, 0, new Size(width, height));

            // 将Bitmap对象保存到文件
            bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            var x = 0;
            var y = 0;
            var width = 1920;
            var height = 1080;

            CapturePartialScreen(x, y, width, height, @"C:\Users\27991\Desktop\1.png");

        }*/

        private void CaptureScreen(object sender, RoutedEventArgs e)
        {
            _overlayWindow = CreateOverlayWindow();
            _overlayWindow.Show();
        }

        private Window CreateOverlayWindow()
        {
            var window = new Window
            {
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)),
                Topmost = true,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Width = SystemParameters.PrimaryScreenWidth,
                Height = SystemParameters.PrimaryScreenHeight
            };

            var canvas = new Canvas();
            window.Content = canvas;

            _selectionRectangle = new System.Windows.Shapes.Rectangle
            {
                Stroke = System.Windows.Media.Brushes.Red,
                StrokeThickness = 2
            };
            canvas.Children.Add(_selectionRectangle);

            window.PreviewMouseLeftButtonDown += (s, e) =>
            {
                _startPoint = e.GetPosition(canvas);
            };

            window.PreviewMouseMove += (s, e) =>
            {
                if (e.LeftButton != MouseButtonState.Pressed) return;
                var pos = e.GetPosition(canvas);
                var x = Math.Min(pos.X, _startPoint.X);
                var y = Math.Min(pos.Y, _startPoint.Y);
                var width = Math.Abs(pos.X - _startPoint.X);
                var height = Math.Abs(pos.Y - _startPoint.Y);

                Canvas.SetLeft(_selectionRectangle, x);
                Canvas.SetTop(_selectionRectangle, y);
                _selectionRectangle.Width = width;
                _selectionRectangle.Height = height;
            };

            window.PreviewMouseLeftButtonUp += (s, e) =>
            {
                _endPoint = e.GetPosition(canvas);
                TakeScreenShot();
                window.Close();
            };

            return window;
        }

        private async void TakeScreenShot()
        {
            // 获取选区的屏幕坐标
            var x = (int)Math.Min(_startPoint.X, _endPoint.X);
            var y = (int)Math.Min(_startPoint.Y, _endPoint.Y);
            var width = (int)Math.Abs(_endPoint.X - _startPoint.X);
            var height = (int)Math.Abs(_endPoint.Y - _startPoint.Y);

            // 延时以确保 UI 渲染正确
            await Task.Delay(100);

            // 截图
            using var bmp = new Bitmap(width, height);
            using var gfx = Graphics.FromImage(bmp);
            gfx.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height));

            // 将截图保存到剪贴板
            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            var img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.StreamSource = ms;
            img.EndInit();
            Clipboard.SetImage(img);

            // 将截图保存到文件（如果需要）
            // bmp.Save("screenshot.png", ImageFormat.Png);
        }
    }
}
