using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;

namespace ColorMemoryWpf2
{
    public static class KinectHelper
    {
        public static void GetColorBgraData(this ColorFrameReader reader, byte[] data)
        {
            if (reader == null) return;

            using (var frame = reader.AcquireLatestFrame())
            {
                if (frame == null) return;

                frame.CopyConvertedFrameDataToArray(data, ColorImageFormat.Bgra);
            }
        }
    }

    public class BitmapInfo
    {
        public static BitmapInfo ForColorBgra(FrameDescription frameDescription)
        {
            return new BitmapInfo(frameDescription, PixelFormats.Bgra32, 4);
        }

        public static BitmapInfo ForColorBgra(int width, int height)
        {
            return new BitmapInfo(width, height, PixelFormats.Bgra32, 4);
        }

        public static BitmapInfo ForDepth(FrameDescription frameDescription)
        {
            return new BitmapInfo(frameDescription, PixelFormats.Bgra32, 4);
        }

        public FrameDescription FrameDescription { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public int BytesPerPixel { get; private set; }
        public int PixelsCount { get; private set; }
        public int PixelBytesLength { get; private set; }

        PixelFormat _pixelFormat;
        Int32Rect _rect;
        int _stride;

        public BitmapInfo(FrameDescription frameDescription, PixelFormat pixelFormat, int bytesPerPixel)
        {
            FrameDescription = frameDescription;
            Initialize(frameDescription.Width, frameDescription.Height, pixelFormat, bytesPerPixel);
        }

        public BitmapInfo(int width, int height, PixelFormat pixelFormat, int bytesPerPixel)
        {
            Initialize(width, height, pixelFormat, bytesPerPixel);
        }

        void Initialize(int width, int height, PixelFormat pixelFormat, int bytesPerPixel)
        {
            Width = width;
            Height = height;

            BytesPerPixel = bytesPerPixel;
            PixelsCount = width * height;
            PixelBytesLength = bytesPerPixel * PixelsCount;

            _pixelFormat = pixelFormat;
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
}
