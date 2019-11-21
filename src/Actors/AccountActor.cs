using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Util.Internal;
using AkkaAccountActors.Messages;

namespace AkkaAccountActors.Actors
{
    public class AccountActor : ReceiveActor
    {
        private readonly string _accountId;
        private readonly Dictionary<string, LedgerProperties> _ledgers;

        public AccountActor(string accountId)
        {
            _accountId = accountId;
            _ledgers = new Dictionary<string, LedgerProperties>()
            {
                {LedgerNames.Scheme, new LedgerProperties(false)} ,
                {LedgerNames.Available, new LedgerProperties(true)},
                {LedgerNames.Payable, new LedgerProperties(true)} ,
                {LedgerNames.Paid, new LedgerProperties(true)}
            };
                
            Receive<DepositRequest>(deposit =>
            {
                _ledgers[deposit.Ledger].Balance += deposit.Amount;

                Context.Self.Tell(new LogBalances());
                Context.Sender.Tell(new Accepted(deposit.CorrelationId));
            });
            
            Receive<WithdrawalRequest>(withdrawal =>
            {
                if (withdrawal.Amount <= 0)
                {
                   // Context.Sender.Tell(new Refused(withdrawal.CorrelationId, "Amount must be greater than zero."));
                   Console.WriteLine("Amount must be greater than zero.");
                    return;
                }
                
                if (!HasAvailableFunds(withdrawal.Ledger, withdrawal.Amount))
                {
                   // Context.Sender.Tell(new Refused(withdrawal.CorrelationId, "Insufficient funds."));
                   Console.WriteLine("Insufficient funds.");
                    return;
                }
                
                _ledgers[withdrawal.Ledger].Balance -= withdrawal.Amount;
                
                Context.Self.Tell(new LogBalances());

                Context.Sender.Tell(new Accepted(withdrawal.CorrelationId));
            });

            Receive<MovementRequest>(movementRequest =>
            {
                if (movementRequest.Amount <= 0)
                {
                   // Context.Sender.Tell(new Refused(movementRequest.CorrelationId, "Amount must be greater than zero."));
                   Console.WriteLine("Amount must be greater than zero.");
                    return;
                }

                if (!HasAvailableFunds(movementRequest.SourceLedger, movementRequest.Amount))
                {
                    Console.WriteLine("Insufficient funds.");
                    //Context.Sender.Tell(new Refused(movementRequest.CorrelationId, "Insufficient funds."));
                    return;
                }
                    
                _ledgers[movementRequest.SourceLedger].Balance -= movementRequest.Amount;
                _ledgers[movementRequest.DestinationLedger].Balance += movementRequest.Amount;

                Context.Self.Tell(new LogBalances());
                Context.Sender.Tell(new Accepted(movementRequest.CorrelationId));
            });

            Receive<LogBalances>(balances => { LogBalances(); });
        }
        
        private bool HasAvailableFunds(string ledgerName, decimal amount)
        {
            var ledger = _ledgers[ledgerName];

            if (!ledger.PreventNegative)
                return true;
                
            return ledger.Balance - amount >= 0;
        }
       
        private void LogBalances()
        {
            var s = "";
            _ledgers.ForEach((pair =>
            {
                var (key, value) = pair;
                s += $" - {key}: {value.Balance.ToString()}";
            }));
                
            Console.WriteLine($"Account: {_accountId} {s}");
        }
        
        private class LedgerProperties
        {
            public decimal Balance { get; set; } = 0;
            public bool PreventNegative { get; }

            public LedgerProperties(bool preventNegative)
            {
                PreventNegative = preventNegative;
            }
        }
    }
}