using System;

namespace AkkaAccountActors.Messages
{
    public class MovementRequest : Message
    {
            public string SourceLedger { get; }
            public string DestinationLedger { get; }
            public decimal Amount { get; }

            public MovementRequest(Guid correlationId, string sourceLedger, string destinationLedger, decimal amount) : base(correlationId)
            {
                SourceLedger = sourceLedger;
                DestinationLedger = destinationLedger;
                Amount = amount;
            }
    }
}