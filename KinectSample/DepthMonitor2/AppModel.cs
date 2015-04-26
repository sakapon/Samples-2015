using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using KLibrary.Labs.ObservableModel;

namespace DepthMonitor2
{
    public class AppModel
    {
        public ISettableProperty<WriteableBitmap> DepthBitmap { get; private set; }

        public AppModel()
        {
            DepthBitmap = ObservableProperty.CreateSettable<WriteableBitmap>(null);
        }
    }
}
