using System;

namespace AkkaAccountActors.Messages
{
    public class Message
    {
        public Guid CorrelationId { get;}

        public Message(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}