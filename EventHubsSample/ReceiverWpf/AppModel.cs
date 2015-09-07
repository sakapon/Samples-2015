using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using KLibrary.Labs.ObservableModel;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace ReceiverWpf
{
    public class AppModel
    {
        public IGetOnlyProperty<Point> Position { get; private set; }

        public AppModel()
        {
            var index = -1;
            Position = StaticEventProcessor.Message
                .Select(m => (dynamic)JsonConvert.DeserializeObject(m))
                .Where(o => o.index > index)
                .Do(o => index = o.index)
                .Select(o => (string)o.position)
                .Select(Point.Parse)
                .ToGetOnly(default(Point));

            var hostName = string.Format("Host-{0:yyyyMMdd-HHmmss}", DateTime.Now);
            var eventHubName = ConfigurationManager.AppSettings["EventHubName"];
            var eventHubConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var storageConnectionString = ConfigurationManager.AppSettings["StorageConnection"];

            // Receives event once for one Consumer Group.
            var host = new EventProcessorHost(hostName, eventHubName, EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString);
            host.RegisterEventProcessorAsync<StaticEventProcessor>();
        }
    }
}
