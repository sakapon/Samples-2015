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
                .Do(m => Debug.WriteLine("Sending message. {0}", new[] { m }))
                .Select(m => new EventData(Encoding.UTF8.GetBytes(m)))
                .Subscribe(d => client.SendAsync(d));
        }
    }
}
