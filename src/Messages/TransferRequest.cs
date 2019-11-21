using System;

namespace AkkaAccountActors.Messages
{
    public class TransferRequest : Message
    {
            public string SourceAccountId { get; set; }
            public string DestinationAccountId { get; set; }
            public decimal Amount { get; set; }

            public TransferRequest(Guid correlationId, string sourceAccountId, string destinationAccountId, decimal amount) : base(correlationId)
            {
                SourceAccountId = sourceAccountId;
                DestinationAccountId = destinationAccountId;
                Amount = amount;
            }
    }
}