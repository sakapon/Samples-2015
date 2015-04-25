using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace DepthMonitor
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        const short DepthBound1 = 1000;
        const short DepthBound2 = 2000;

        KinectSensor sensor;
        DepthImagePixel[] depthData;
        byte[] bitmapData;
        Int32Rect bitmapRect;
        int bitmapStride;
        WriteableBitmap depthBitmap;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count == 0) return;

            sensor = KinectSensor.KinectSensors[0];
            sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);

            depthData = new DepthImagePixel[sensor.DepthStream.FramePixelDataLength];
            bitmapData = new byte[4 * sensor.DepthStream.FramePixelDataLength];
            bitmapRect = new Int32Rect(0, 0, sensor.DepthStream.FrameWidth, sensor.DepthStream.FrameHeight);
            bitmapStride = 4 * sensor.DepthStream.FrameWidth;
            depthBitmap = new WriteableBitmap(sensor.DepthStream.FrameWidth, sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgra32, null);

            TheImage.Source = depthBitmap;
            sensor.Start();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (sensor != null)
            {
                sensor.Stop();
            }
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            using (var frame = sensor.DepthStream.OpenNextFrame(1000 / 60))
            {
                if (frame == null) return;

                frame.CopyDepthImagePixelDataTo(depthData);
            }

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

            depthBitmap.WritePixels(bitmapRect, bitmapData, bitmapStride, 0);
        }
    }
}
