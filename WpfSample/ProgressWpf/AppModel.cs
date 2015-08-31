using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using KLibrary.Labs.ObservableModel;

namespace ProgressWpf
{
    public class AppModel
    {
        public IGetOnlyProperty<double> ProgressValue { get; private set; }

        public AppModel()
        {
            ProgressValue = Observable.Interval(TimeSpan.FromSeconds(0.03))
                .Select(n => n % 120)
                .Select(n => n / 100.0)
                .Select(v => v < 1.0 ? v : 1.0)
                .ToGetOnly(0.0);
        }
    }
}
