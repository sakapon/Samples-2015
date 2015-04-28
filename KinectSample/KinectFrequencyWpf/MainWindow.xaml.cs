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

namespace KinectFrequencyWpf
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly TimeSpan FramesInterval = TimeSpan.FromSeconds(1 / 30.0);

        KinectSensor sensor;
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
            sensor.SkeletonStream.Enable();

            Task.Run(() => sensor.Start());

            framesSubscription = Observable.Interval(FramesInterval)
                .Select(_ => GetSkeletonData(sensor, (int)FramesInterval.TotalMilliseconds))
                .Select(GetPosition)
                .Select(p => p.HasValue ? SkeletonPointToString(p.Value) : "")
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(t => PositionText.Text = t);
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            if (framesSubscription != null) framesSubscription.Dispose();
            if (sensor != null) sensor.Stop();
        }

        static Skeleton[] GetSkeletonData(KinectSensor sensor, int millisecondsWait)
        {
            try
            {
                if (sensor == null || !sensor.IsRunning) return null;

                using (var frame = sensor.SkeletonStream.OpenNextFrame(millisecondsWait))
                {
                    if (frame == null) return null;

                    var skeletonData = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletonData);
                    return skeletonData;
                }
            }
            catch (InvalidOperationException)
            {
                // センサーが稼働していないときにフレームを取得すると発生します。
                return null;
            }
        }

        static SkeletonPoint? GetPosition(Skeleton[] skeletonData)
        {
            if (skeletonData == null) return null;

            var skeleton = skeletonData.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
            if (skeleton == null) return null;

            return skeleton.Position;
        }

        static readonly Func<SkeletonPoint, string> SkeletonPointToString = p =>
            string.Format("({0:N3}, {1:N3}, {2:N3})", p.X, p.Y, p.Z);
    }
}
