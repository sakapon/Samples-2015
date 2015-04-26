using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KLibrary.Labs.ObservableModel;
using Microsoft.Kinect;

namespace DepthMonitor2
{
    public class AsyncKinectManager0
    {
        public ISettableProperty<KinectSensor> Sensor { get; private set; }

        // アプリケーション起動時に既にデバイスが接続されている場合には発生しません。
        public ISettableProperty<KinectSensor> SensorConnected { get; private set; }
        public ISettableProperty<KinectSensor> SensorDisconnected { get; private set; }

        public AsyncKinectManager0()
        {
            Sensor = ObservableProperty.CreateSettable<KinectSensor>(FindSensor());

            SensorConnected = ObservableProperty.CreateSettable<KinectSensor>(null, true);
            SensorDisconnected = ObservableProperty.CreateSettable<KinectSensor>(null, true);

            Task.Run(() =>
            {
                KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            });
        }

        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (Sensor.Value == null)
            {
                if (e.Status == KinectStatus.Connected)
                {
                    Sensor.Value = e.Sensor;
                    SensorConnected.OnNext(Sensor.Value);
                }
            }
            else
            {
                if (e.Sensor == Sensor.Value && e.Status != KinectStatus.Connected)
                {
                    SensorDisconnected.OnNext(Sensor.Value);
                    Sensor.Value = FindSensor();
                    if (Sensor.Value != null) SensorConnected.OnNext(Sensor.Value);
                }
            }
        }

        static KinectSensor FindSensor()
        {
            return KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);
        }
    }
}
