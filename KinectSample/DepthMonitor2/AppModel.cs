using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectArchWpf;
using KLibrary.Labs.ObservableModel;
using Microsoft.Kinect;

namespace DepthMonitor2
{
    public class AppModel
    {
        const double Frequency = 30;

        static readonly Func<DepthImagePixel, Color> ToColor = p =>
            !p.IsKnownDepth ? Colors.Transparent
            : p.Depth <= 1000 ? Colors.Orange
            : p.Depth <= 2000 ? Colors.LightGreen
            : Colors.Transparent;

        Int32Rect _bitmapRect;
        int _bitmapStride;

        public ISettableProperty<WriteableBitmap> DepthBitmap { get; private set; }

        public AppModel()
        {
            DepthBitmap = ObservableProperty.CreateSettable<WriteableBitmap>(null);

            var kinect = new AsyncKinectManager();
            kinect.SensorConnected
                .Do(sensor =>
                {
                    sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);

                    _bitmapRect = new Int32Rect(0, 0, sensor.DepthStream.FrameWidth, sensor.DepthStream.FrameHeight);
                    _bitmapStride = 4 * sensor.DepthStream.FrameWidth;

                    try
                    {
                        sensor.Start();
                    }
                    catch (Exception ex)
                    {
                        // センサーが他のプロセスに既に使用されている場合に発生します。
                        Debug.WriteLine(ex);
                    }
                })
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(_ => DepthBitmap.Value = new WriteableBitmap(_bitmapRect.Width, _bitmapRect.Height, 96.0, 96.0, PixelFormats.Bgra32, null));
            kinect.SensorDisconnected
                .Do(sensor => sensor.Stop())
                .Subscribe(_ => DepthBitmap.Value = null);
            kinect.Initialize();

            Observable.Interval(TimeSpan.FromSeconds(1 / Frequency))
                .Select(_ => GetDepthData(kinect.Sensor.Value, (int)(1000 / Frequency)))
                .Where(d => d != null)
                .Select(ToBitmapData)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(d =>
                {
                    var b = DepthBitmap.Value;
                    if (b != null) b.WritePixels(_bitmapRect, d, _bitmapStride, 0);
                });
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
            catch (InvalidOperationException ex)
            {
                // センサーが稼働していないときにフレームを取得すると発生します。
                Debug.WriteLine(ex);
                return null;
            }
        }

        static byte[] ToBitmapData(DepthImagePixel[] depthData)
        {
            var bitmapData = new byte[4 * depthData.Length];

            var bitmapIndex = 0;
            foreach (var pixel in depthData)
            {
                var color = ToColor(pixel);

                bitmapData[bitmapIndex++] = color.B;
                bitmapData[bitmapIndex++] = color.G;
                bitmapData[bitmapIndex++] = color.R;
                bitmapData[bitmapIndex++] = color.A;
            }

            return bitmapData;
        }
    }
}
