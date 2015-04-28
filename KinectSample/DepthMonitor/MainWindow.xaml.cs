using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace DepthMonitor
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly TimeSpan FramesInterval = TimeSpan.FromSeconds(1 / 30.0);

        KinectSensor sensor;
        Int32Rect bitmapRect;
        int bitmapStride;
        WriteableBitmap depthBitmap;
        IDisposable framesSubscription;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count == 0) return;

            sensor = KinectSensor.KinectSensors[0];
            sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);

            bitmapRect = new Int32Rect(0, 0, sensor.DepthStream.FrameWidth, sensor.DepthStream.FrameHeight);
            bitmapStride = 4 * sensor.DepthStream.FrameWidth;
            depthBitmap = new WriteableBitmap(sensor.DepthStream.FrameWidth, sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgra32, null);

            TheImage.Source = depthBitmap;
            Task.Run(() => sensor.Start());

            framesSubscription = Observable.Interval(FramesInterval)
                .Select(_ => GetDepthData(sensor, (int)FramesInterval.TotalMilliseconds))
                .Where(d => d != null)
                .Select(ToBitmapData)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(d => depthBitmap.WritePixels(bitmapRect, d, bitmapStride, 0));
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            if (framesSubscription != null) framesSubscription.Dispose();
            if (sensor != null) sensor.Stop();
        }

        static DepthImagePixel[] GetDepthData(KinectSensor sensor, int millisecondsWait)
        {
            try
            {
                if (sensor == null || !sensor.IsRunning) return null;

                using (var frame = sensor.DepthStream.OpenNextFrame(millisecondsWait))
                {
                    if (frame == null) return null;

                    return frame.GetRawPixelData();
                }
            }
            catch (InvalidOperationException)
            {
                // センサーが稼働していないときにフレームを取得すると発生します。
                return null;
            }
        }

        static byte[] ToBitmapData(DepthImagePixel[] depthData)
        {
            var bitmapData = new byte[4 * depthData.Length];

            var i = 0;
            foreach (var pixel in depthData)
            {
                var color = ToColor(pixel);

                bitmapData[i++] = color.B;
                bitmapData[i++] = color.G;
                bitmapData[i++] = color.R;
                bitmapData[i++] = color.A;
            }

            return bitmapData;
        }

        static readonly Func<DepthImagePixel, Color> ToColor = p =>
            !p.IsKnownDepth ? Colors.Transparent
            : p.Depth <= 1000 ? Colors.Orange
            : p.Depth <= 2000 ? Colors.LightGreen
            : Colors.Transparent;
    }
}
