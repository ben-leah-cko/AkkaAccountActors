using System;
using System.Collections.Generic;
using System.Threading;
using Akka.Actor;
using Akka.Util.Internal;
using AkkaAccountActors.Actors;
using AkkaAccountActors.Messages;

namespace AkkaAccountActors
{
    class Program
    {
        static void Main(string[] args)
        {
            var accountSystem = ActorSystem.Create("accounts");

            var B100 = accountSystem.ActorOf(Props.Create(() => new AccountActor("B100")), "B100");
            var B200 = accountSystem.ActorOf(Props.Create(() => new AccountActor("B200")), "B200");
            
            B100.Tell(new MovementRequest(Guid.NewGuid(), LedgerNames.Scheme, LedgerNames.Available, 50));
            B200.Tell(new MovementRequest(Guid.NewGuid(), LedgerNames.Scheme, LedgerNames.Available, 50));
            //B100.Tell(new LogBalances());
            
            var transferActor = accountSystem.ActorOf(Props.Create(() => new TransferActor(B100, B200, 20, Guid.NewGuid())), $"transfer-{Guid.NewGuid()}");
            
            var transferActor2 = accountSystem.ActorOf(Props.Create(() => new TransferActor(B200, B100, 25, Guid.NewGuid())), $"transfer-{Guid.NewGuid()}");
            
            transferActor.Tell("start");

            transferActor2.Tell("start");

            Thread.Sleep(200);
            
            B100.Tell(new LogBalances());

            B200.Tell(new LogBalances());
            
            Console.ReadLine();
        }
        
//        private static IActorRef GetAccountActor(string accountId)
//        {
//            var account = Context.Child(accountId);
//            if (account.Equals(Nobody.Instance))
//            {
//                account = Context.ActorOf(Props.Create(() => new AccountActor(accountId)), accountId);
//            }
//
//            return account;
//        }


//        public class AccountSupervisor : UntypedActor
//        {
//            protected override void OnReceive(object message)
//            {
//                if (message is DepositRequest impact)
//                {
//                    var account = GetAccountActor(impact.AccountId);
//                    account.Tell(impact);
//                }
//
//                if (message is TransferRequest transfer)
//                {
//                    var source = GetAccountActor(transfer.SourceAccountId);
//                    var destination = GetAccountActor(transfer.DestinationAccountId);
//
//                    var transferActor = Context.ActorOf(Props.Create(() => new TransferActor(source, destination, transfer.Amount, transfer.CorrelationId)));
//                    transferActor.Tell("start");
//                }
//            }
//
//          
//        }


    }
}
