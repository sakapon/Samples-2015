using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

    public abstract class BitmapInfo
    {
        public static ColorBitmapInfo ForColor(ColorImageFormat format)
        {
            return new ColorBitmapInfo(format, PixelFormats.Bgr32, 4);
        }

        public static DepthBitmapInfo ForDepth(DepthImageFormat format)
        {
            return new DepthBitmapInfo(format, PixelFormats.Bgra32, 4);
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public int PixelsCount { get; private set; }
        public int PixelBytesLength { get; private set; }

        PixelFormat _pixelFormat;
        int _bytesPerPixel;

        Int32Rect _rect;
        int _stride;

        protected void Initialize(int width, int height, PixelFormat pixelFormat, int bytesPerPixel)
        {
            Width = width;
            Height = height;

            PixelsCount = width * height;
            PixelBytesLength = bytesPerPixel * PixelsCount;

            _pixelFormat = pixelFormat;
            _bytesPerPixel = bytesPerPixel;

            _rect = new Int32Rect(0, 0, width, height);
            _stride = bytesPerPixel * width;
        }

        // This method must be called on the UI thread.
        public WriteableBitmap CreateBitmap()
        {
            return new WriteableBitmap(Width, Height, 96.0, 96.0, _pixelFormat, null);
        }

        // This method must be called on the UI thread.
        public void WritePixels(WriteableBitmap bitmap, byte[] pixelBytes)
        {
            if (bitmap == null) return;

            bitmap.WritePixels(_rect, pixelBytes, _stride, 0);
        }
    }

    public class ColorBitmapInfo : BitmapInfo
    {
        public ColorImageFormat Format { get; private set; }

        public ColorBitmapInfo(ColorImageFormat format, PixelFormat pixelFormat, int bytesPerPixel)
        {
            Format = format;

            switch (format)
            {
                case ColorImageFormat.InfraredResolution640x480Fps30:
                case ColorImageFormat.RawBayerResolution640x480Fps30:
                case ColorImageFormat.RawYuvResolution640x480Fps15:
                case ColorImageFormat.RgbResolution640x480Fps30:
                case ColorImageFormat.YuvResolution640x480Fps15:
                    Initialize(640, 480, pixelFormat, bytesPerPixel);
                    break;
                case ColorImageFormat.RawBayerResolution1280x960Fps12:
                case ColorImageFormat.RgbResolution1280x960Fps12:
                    Initialize(1280, 960, pixelFormat, bytesPerPixel);
                    break;
                default:
                    Initialize(640, 480, pixelFormat, bytesPerPixel);
                    break;
            }
        }
    }

    public class DepthBitmapInfo : BitmapInfo
    {
        public DepthImageFormat Format { get; private set; }

        public DepthBitmapInfo(DepthImageFormat format, PixelFormat pixelFormat, int bytesPerPixel)
        {
            Format = format;

            switch (format)
            {
                case DepthImageFormat.Resolution320x240Fps30:
                    Initialize(320, 240, pixelFormat, bytesPerPixel);
                    break;
                case DepthImageFormat.Resolution640x480Fps30:
                    Initialize(640, 480, pixelFormat, bytesPerPixel);
                    break;
                case DepthImageFormat.Resolution80x60Fps30:
                    Initialize(80, 60, pixelFormat, bytesPerPixel);
                    break;
                default:
                    Initialize(640, 480, pixelFormat, bytesPerPixel);
                    break;
            }
        }
    }
}
