using System;
using System.Threading;

namespace MonitorBankAccountApp
{
    class BankAccountMonitor
    {
        private int balance = 0;
        private readonly object lockObj = new object();

        public void Deposit(int amount)
        {
            lock (lockObj)
            {
                balance += amount;
                Console.WriteLine($"[Monitor] Deposited {amount}, Balance: {balance}");
                Monitor.PulseAll(lockObj);
            }
        }

        public void Withdraw(int amount)
        {
            lock (lockObj)
            {
                while (balance < amount)
                {
                    Console.WriteLine($"[Monitor] Not enough funds for {amount}, waiting...");
                    Monitor.Wait(lockObj);
                }

                balance -= amount;
                Console.WriteLine($"[Monitor] Withdrew {amount}, Balance: {balance}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BankAccountMonitor account = new BankAccountMonitor();

            Thread t1 = new Thread(() => account.Deposit(100));
            Thread t2 = new Thread(() => account.Withdraw(50));
            Thread t3 = new Thread(() => account.Withdraw(70));
            Thread t4 = new Thread(() => account.Deposit(200));

            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();

            t1.Join();
            t2.Join();
            t3.Join();
            t4.Join();

            Console.WriteLine("All operations completed.");
        }
    }
}
