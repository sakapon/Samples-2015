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
            ProgressValue = Observable.Interval(TimeSpan.FromSeconds(0.05))
                .Select(l => l % 101)
                .Select(l => l / 100.0)
                .ToGetOnly(0.0);
        }
    }
}
