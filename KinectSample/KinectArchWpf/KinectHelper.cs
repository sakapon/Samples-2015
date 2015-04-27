using System;
using System.Collections.Generic;
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
    }
}
