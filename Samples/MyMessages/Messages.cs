using System;
using NServiceBus;

namespace MyMessages
{
    [Serializable]
    public class EventMessage : IMyEvent, IEvent
    {
        public Guid EventId { get; set; }
        public DateTime? Time { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public interface IMyEvent
    {
        Guid EventId { get; set; }
        DateTime? Time { get; set; }
        TimeSpan Duration { get; set; }
    }
}
