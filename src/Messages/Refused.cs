using System;

namespace AkkaAccountActors.Messages
{
    public class Refused : Message
    {
        public string Reason { get; }

        public Refused(Guid correlationId, string reason) : base(correlationId)
        {
            Reason = reason;
        }
    }
}