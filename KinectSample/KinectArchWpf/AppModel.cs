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
        static readonly TimeSpan FramesInterval = TimeSpan.FromSeconds(1 / Frequency);

        public ISettableProperty<string> PositionText { get; private set; }

        public AppModel()
        {
            PositionText = ObservableProperty.CreateSettable("");

            var kinect = new AsyncKinectManager();
            kinect.SensorConnected
                .Subscribe(sensor =>
                {
                    sensor.SkeletonStream.EnableWithDefaultSmoothing();

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
                .Select(_ => kinect.Sensor.Value.GetSkeletonData(FramesInterval))
                .Select(GetPosition)
                .Select(p => p.HasValue ? SkeletonPointToString(p.Value) : "")
                .Subscribe(PositionText);
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
