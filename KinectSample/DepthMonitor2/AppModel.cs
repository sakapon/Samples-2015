using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
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
        static readonly TimeSpan FramesInterval = TimeSpan.FromSeconds(1 / Frequency);

        static readonly Func<DepthImagePixel, Color> ToColor = p =>
            !p.IsKnownDepth ? Colors.Transparent
            : p.Depth <= 1000 ? Colors.Orange
            : p.Depth <= 2000 ? Colors.LightGreen
            : Colors.Transparent;

        static readonly DepthBitmapInfo DepthBitmapInfo = KinectHelper.GetDepthBitmapInfo(DepthImageFormat.Resolution320x240Fps30);

        public ISettableProperty<WriteableBitmap> DepthBitmap { get; private set; }

        public AppModel()
        {
            DepthBitmap = ObservableProperty.CreateSettable<WriteableBitmap>(null);

            var kinect = new AsyncKinectManager();
            kinect.SensorConnected
                .Do(sensor =>
                {
                    sensor.DepthStream.Enable(DepthBitmapInfo.Format);

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
                .Subscribe(_ => DepthBitmap.Value = DepthBitmapInfo.CreateBitmap());
            kinect.SensorDisconnected
                .Do(sensor => sensor.Stop())
                .Subscribe(_ => DepthBitmap.Value = null);
            kinect.Initialize();

            Observable.Interval(FramesInterval)
                .Select(_ => kinect.Sensor.Value.GetDepthData(FramesInterval))
                .Where(d => d != null)
                .Select(ToBitmapData)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(d => DepthBitmapInfo.WritePixels(DepthBitmap.Value, d));
        }

        static byte[] ToBitmapData(DepthImagePixel[] depthData)
        {
            var bitmapData = new byte[DepthBitmapInfo.PixelBytesLength];

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
