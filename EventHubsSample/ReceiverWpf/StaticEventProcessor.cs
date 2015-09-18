using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KLibrary.Labs.ObservableModel;
using Microsoft.ServiceBus.Messaging;

namespace ReceiverWpf
{
    public class StaticEventProcessor : IEventProcessor
    {
        public static ISettableProperty<string> Message { get; private set; }

        static readonly object messageLock = new object();

        static StaticEventProcessor()
        {
            Message = ObservableProperty.CreateSettable<string>(null);
        }

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Debug.WriteLine("Partition {0}: Processor closing. Reason: {1}", context.Lease.PartitionId, reason);

            if (reason == CloseReason.Shutdown)
                await context.CheckpointAsync();
        }

        public Task OpenAsync(PartitionContext context)
        {
            Debug.WriteLine("Partition {0}: Processor opening. Offset: {1}", context.Lease.PartitionId, context.Lease.Offset);

            return Task.FromResult<object>(null);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var data in messages)
            {
                lock (messageLock)
                {
                    var message = Encoding.UTF8.GetString(data.GetBytes());
                    Debug.WriteLine("Partition {0}: Message received. {1}", context.Lease.PartitionId, message);

                    Message.Value = message;
                }
            }

            await context.CheckpointAsync();
        }
    }
}
