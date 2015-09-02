using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using KLibrary.Labs.ObservableModel;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace SenderWpf
{
    public class AppModel
    {
        public ISettableProperty<Point> Position { get; private set; }

        public AppModel()
        {
            Position = ObservableProperty.CreateSettable(new Point());

            var client = EventHubClient.Create("sakapon-event-201508");

            var index = 0;
            Position
                .Select(p => new { index = index++, position = p.ToString() })
                .Select(o => JsonConvert.SerializeObject(o))
                .Do(s => Debug.WriteLine("Sending message. Data: '{0}'", new[] { s }))
                .Select(s => new EventData(Encoding.UTF8.GetBytes(s)))
                .Subscribe(d => client.SendAsync(d), ex => Debug.WriteLine(ex));
        }
    }
}
