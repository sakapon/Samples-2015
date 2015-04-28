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
        static readonly TimeSpan FramesInterval = TimeSpan.FromSeconds(1 / 30.0);
        static readonly DepthBitmapInfo DepthBitmapInfo = BitmapInfo.ForDepth(DepthImageFormat.Resolution320x240Fps30);

        public IGetOnlyProperty<WriteableBitmap> DepthBitmap { get; private set; }

        public AppModel()
        {
            var kinect = new AsyncKinectManager();
            DepthBitmap = kinect.Sensor
                .ObserveOn(SynchronizationContext.Current)
                .Select(sensor => sensor != null ? DepthBitmapInfo.CreateBitmap() : null)
                .ToGetOnly(null);
            kinect.SensorConnected
                .Subscribe(sensor =>
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
                });
            kinect.SensorDisconnected
                .Subscribe(sensor => sensor.Stop());
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
