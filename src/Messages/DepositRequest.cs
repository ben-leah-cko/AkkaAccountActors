using System;

namespace AkkaAccountActors.Messages
{
    public class DepositRequest : Message
    {
        public string Ledger { get; }
        public decimal Amount { get; }

        public DepositRequest(Guid correlationId, string ledger, decimal amount) : base(correlationId)
        {
            Ledger = ledger;
            Amount = amount;
        }
    }
}