using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using KLibrary.Labs.ObservableModel;
using Microsoft.ServiceBus.Messaging;

namespace SenderWpf
{
    public class AppModel
    {
        public ISettableProperty<Point> Position { get; private set; }

        public AppModel()
        {
            Position = ObservableProperty.CreateSettable(new Point());

            var client = EventHubClient.Create("sakapon-event-201508");

            Position
                .Select(p => p.ToString())
                .Select(s => new EventData(Encoding.UTF8.GetBytes(s)))
                .Subscribe(d => client.SendAsync(d), ex => Debug.WriteLine(ex));
        }
    }
}
