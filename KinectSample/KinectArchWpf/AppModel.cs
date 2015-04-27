using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using KLibrary.Labs.ObservableModel;
using Microsoft.Kinect;

namespace KinectArchWpf
{
    public class AppModel
    {
        const double Frequency = 30;

        public ISettableProperty<string> PositionText { get; private set; }

        public AppModel()
        {
            PositionText = ObservableProperty.CreateSettable("");

            var kinect = new AsyncKinectManager();
            kinect.SensorConnected
                .Subscribe(sensor =>
                {
                    sensor.SkeletonStream.Enable();

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

            Observable.Interval(TimeSpan.FromSeconds(1 / Frequency))
                .Select(_ => GetSkeletonData(kinect.Sensor.Value, (int)(1000 / Frequency)))
                .Select(GetPosition)
                .Select(p => p.HasValue ? SkeletonPointToString(p.Value) : "")
                .Subscribe(PositionText);
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
            catch (InvalidOperationException ex)
            {
                // センサーが稼働していないときにフレームを取得すると発生します。
                Debug.WriteLine(ex);
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
