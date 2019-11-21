using System;
using Akka.Actor;
using AkkaAccountActors.Messages;

namespace AkkaAccountActors.Actors
{
     public class TransferActor : ReceiveActor
        {
            private readonly IActorRef _sourceAccount;
            private readonly IActorRef _destinationAccount;
            private readonly decimal _amount;
            private readonly Guid _correlationId;

            public TransferActor(IActorRef sourceAccount, IActorRef destinationAccount, decimal amount, Guid correlationId)
            {
                _sourceAccount = sourceAccount ?? throw new ArgumentNullException(nameof(sourceAccount));
                _destinationAccount = destinationAccount ?? throw new ArgumentNullException(nameof(destinationAccount));
                _amount = amount;
                _correlationId = correlationId;

                Become(Starting);
            }

            private void Starting()
            {
                Receive<string>(s => s.Equals("start"), start =>
                {
                    Become(AwaitingDebitConfirmation);
                    
                    Console.WriteLine($"Withdrawing from account {_amount}");
                    //Withdrawal from source account
                    _sourceAccount.Tell(new WithdrawalRequest(_correlationId, LedgerNames.Available, _amount));
                });
            }

            private void AwaitingDebitConfirmation()
            {
                Receive<Accepted>(succeeded =>
                {
                    Become(AwaitingCreditConfirmation);
                    
                    Console.WriteLine($"Depositing in account {_amount}");
                    //Deposit in destination account 
                    _destinationAccount.Tell(new DepositRequest(_correlationId, LedgerNames.Available, _amount));
                });

                Receive<Refused>(funds =>
                {
                    //No need to compensate here, still in a consistent state
                   // Context.Parent.Tell(funds);
                });
            }

            private void AwaitingCreditConfirmation()
            {
                Receive<Accepted>((succeeded =>
                {
                    Console.WriteLine("Transfer Succeeded.");
                    //Context.Parent.Tell(new Status.Success("Transfer Succeeded"));
                }));

                Receive<Refused>((refused) =>
                {
                    Become(RollBackDebit);
                    Console.WriteLine("Rolling back transfer.");
                    //Roll back withdrawal from source account 
                    _sourceAccount.Tell(new DepositRequest(_correlationId, LedgerNames.Available, _amount));
                });
            }

            private void RollBackDebit()
            {
                Receive<Accepted>(succeeded => { Console.WriteLine("Rollback succeeded"); });
                
                //This is bad. 
                Receive<Refused>(refused => Console.WriteLine(("System in inconsistent state.")));
            }
        }
}