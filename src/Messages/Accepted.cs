using System;

namespace AkkaAccountActors.Messages
{
    public class Accepted : Message
    {
        public Accepted(Guid correlationId) : base(correlationId)
        {
        }
    }
}