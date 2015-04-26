using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using KLibrary.Labs.ObservableModel;
using Microsoft.Kinect;

namespace DepthMonitor2
{
    public class AsyncKinectManager
    {
        ISettableProperty<KinectSensor> _Sensor;
        public IGetOnlyProperty<KinectSensor> Sensor { get; private set; }
        KinectSensor _sensorCache;

        // アプリケーション起動時に既にデバイスが接続されている場合も発生します。
        public IGetOnlyProperty<KinectSensor> SensorConnected { get; private set; }
        public IGetOnlyProperty<KinectSensor> SensorDisconnected { get; private set; }

        public AsyncKinectManager()
        {
            _Sensor = ObservableProperty.CreateSettable<KinectSensor>(null);
            Sensor = _Sensor.ToGetOnlyMask();

            SensorDisconnected = _Sensor
                .Select(s => _sensorCache)
                .Where(s => s != null)
                .ToGetOnly(null, true);
            SensorConnected = _Sensor
                .Do(s => _sensorCache = s)
                .Where(s => s != null)
                .ToGetOnly(null, true);
        }

        // SensorConnected を購読した後で呼び出してください。
        public void Initialize()
        {
            Task.Run(() =>
            {
                KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
                _Sensor.Value = FindSensor();
            });
        }

        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (Sensor.Value == null)
            {
                if (e.Status == KinectStatus.Connected)
                {
                    _Sensor.Value = e.Sensor;
                }
            }
            else if (Sensor.Value == e.Sensor)
            {
                if (e.Status != KinectStatus.Connected)
                {
                    _Sensor.Value = FindSensor();
                }
            }
        }

        static KinectSensor FindSensor()
        {
            return KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);
        }
    }
}
