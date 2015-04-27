using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;

namespace KinectArchWpf
{
    public static class KinectHelper
    {
        public static Vector3D ToVector3D(this SkeletonPoint p)
        {
            return new Vector3D(p.X, p.Y, p.Z);
        }

        public static SkeletonPoint ToSkeletonPoint(this Vector3D v)
        {
            return new SkeletonPoint { X = (float)v.X, Y = (float)v.Y, Z = (float)v.Z };
        }

        public static void EnableWithDefaultSmoothing(this SkeletonStream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            stream.Enable(new TransformSmoothParameters
            {
                Correction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f,
                Prediction = 0.0f,
                Smoothing = 0.5f,
            });
        }

        public static Skeleton[] GetSkeletonData(this KinectSensor sensor, TimeSpan timeout)
        {
            if (sensor == null || !sensor.IsRunning) return null;

            try
            {
                using (var frame = sensor.SkeletonStream.OpenNextFrame((int)timeout.TotalMilliseconds))
                {
                    if (frame == null) return null;

                    var skeletonData = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletonData);
                    return skeletonData;
                }
            }
            catch (InvalidOperationException ex)
            {
                // センサーが稼働していない、またはストリームが有効になっていないときにフレームを取得すると発生します。
                Debug.WriteLine(ex);
                return null;
            }
        }

        public static byte[] GetColorData(this KinectSensor sensor, TimeSpan timeout)
        {
            if (sensor == null || !sensor.IsRunning) return null;

            try
            {
                using (var frame = sensor.ColorStream.OpenNextFrame((int)timeout.TotalMilliseconds))
                {
                    if (frame == null) return null;

                    return frame.GetRawPixelData();
                }
            }
            catch (InvalidOperationException ex)
            {
                // センサーが稼働していない、またはストリームが有効になっていないときにフレームを取得すると発生します。
                Debug.WriteLine(ex);
                return null;
            }
        }

        public static DepthImagePixel[] GetDepthData(this KinectSensor sensor, TimeSpan timeout)
        {
            if (sensor == null || !sensor.IsRunning) return null;

            try
            {
                using (var frame = sensor.DepthStream.OpenNextFrame((int)timeout.TotalMilliseconds))
                {
                    if (frame == null) return null;

                    return frame.GetRawPixelData();
                }
            }
            catch (InvalidOperationException ex)
            {
                // センサーが稼働していない、またはストリームが有効になっていないときにフレームを取得すると発生します。
                Debug.WriteLine(ex);
                return null;
            }
        }
    }
}
