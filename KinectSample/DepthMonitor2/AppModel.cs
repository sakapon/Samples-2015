using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KLibrary.Labs.ObservableModel;
using Microsoft.Kinect;

namespace DepthMonitor2
{
    public class AppModel
    {
        const short DepthBound1 = 1000;
        const short DepthBound2 = 2000;
        const double Frequency = 30;

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

                    sensor.Start();
                })
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(_ => DepthBitmap.Value = new WriteableBitmap(_bitmapRect.Width, _bitmapRect.Height, 96.0, 96.0, PixelFormats.Bgra32, null));
            kinect.SensorDisconnected
                .ObserveOn(SynchronizationContext.Current)
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
                    if (b != null) DepthBitmap.Value.WritePixels(_bitmapRect, d, _bitmapStride, 0);
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
                var color
                    = !pixel.IsKnownDepth ? Colors.Transparent
                    : pixel.Depth <= DepthBound1 ? Colors.Orange
                    : pixel.Depth <= DepthBound2 ? Colors.LightGreen
                    : Colors.Transparent;

                bitmapData[bitmapIndex++] = color.B;
                bitmapData[bitmapIndex++] = color.G;
                bitmapData[bitmapIndex++] = color.R;
                bitmapData[bitmapIndex++] = color.A;
            }

            return bitmapData;
        }
    }
}
