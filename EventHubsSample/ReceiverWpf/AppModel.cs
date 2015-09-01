using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KLibrary.Labs.ObservableModel;
using Microsoft.ServiceBus.Messaging;

namespace ReceiverWpf
{
    public class AppModel
    {
        public IGetOnlyProperty<Point> Position { get; private set; }

        public AppModel()
        {
            Position = SimpleEventProcessor.Message
                .SelectToGetOnly(s => Point.Parse(s));

            var hostName = string.Format("Host-{0:yyyyMMdd-HHmmss}", DateTime.Now);
            var eventHubName = "sakapon-event-201508";
            var eventHubConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var storageConnectionString = ConfigurationManager.AppSettings["StorageConnection"];

            // Receives event once for one Consumer Group.
            var host = new EventProcessorHost(hostName, eventHubName, EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString);
            host.RegisterEventProcessorAsync<SimpleEventProcessor>();
        }
    }

    class SimpleEventProcessor : IEventProcessor
    {
        public static ISettableProperty<string> Message { get; private set; }

        static SimpleEventProcessor()
        {
            Message = ObservableProperty.CreateSettable("0,0");
        }

        async Task IEventProcessor.CloseAsync(PartitionContext context, CloseReason reason)
        {
            Debug.WriteLine("Processor Shutting Down. Partition '{0}', Reason: '{1}'.", context.Lease.PartitionId, reason);

            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        Task IEventProcessor.OpenAsync(PartitionContext context)
        {
            Debug.WriteLine("SimpleEventProcessor initialized. Partition: '{0}', Offset: '{1}'", context.Lease.PartitionId, context.Lease.Offset);

            return Task.FromResult<object>(null);
        }

        static DateTime enqueuedTime = DateTime.UtcNow;
        static readonly object enqueuedTimeLock = new object();

        async Task IEventProcessor.ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var data in messages)
            {
                lock (enqueuedTimeLock)
                {
                    if (data.EnqueuedTimeUtc <= enqueuedTime) continue;
                    enqueuedTime = data.EnqueuedTimeUtc;

                    var message = Encoding.UTF8.GetString(data.GetBytes());
                    Message.Value = message;

                    Debug.WriteLine("Message received. Partition: '{0}', Data: '{1}'", context.Lease.PartitionId, message);
                }
            }
            await context.CheckpointAsync();
        }
    }
}
