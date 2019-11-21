using System;

namespace AkkaAccountActors.Messages
{
    public class WithdrawalRequest : Message
    {
        public string Ledger { get; }
        public decimal Amount { get; }

        public WithdrawalRequest(Guid correlationId, string ledger, decimal amount) : base(correlationId)
        {
            Ledger = ledger;
            Amount = amount;
        }
    }
}