using System;

namespace ATMDispenserExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var bills50s = new CurrencyBill(50, 1);
            var bills20s = new CurrencyBill(20, 2);
            var bills10s = new CurrencyBill(10, 5);

            bills50s.RegisterNext(bills20s)
                    .RegisterNext(bills10s);

            while (true)
            {
                Console.WriteLine("Please enter amount to dispense:");
                var isParsed = int.TryParse(Console.ReadLine(), out var amount);

                if (isParsed)
                {
                    var isDepensible = bills50s.DispenseRequest(amount);
                    if (isDepensible)
                    {
                        Console.WriteLine($"Your amount ${amount} is dispensible!");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to dispense ${amount}!");
                    }
                }
                else
                {
                    Console.WriteLine("Please enter a valid amount to dispense");
                }
            }
        }
    }

    public class CurrencyBill
    {
        private CurrencyBill next = CurrencyBill.Zero;
        private static readonly CurrencyBill Zero;

        static CurrencyBill()
        {
            Zero = new ZeroCurrencyBill();
        }

        public class ZeroCurrencyBill : CurrencyBill
        {
            public ZeroCurrencyBill() : base(0, 0)
            {
            }

            public override bool DispenseRequest(int amount)
            {
                return false;
            }
        }

        public CurrencyBill(int denomination, int quantity)
        {
            Denomination = denomination;
            Quantity = quantity;
        }

        public CurrencyBill RegisterNext(CurrencyBill currencyBill)
        {
            next = currencyBill;
            return next;
        }

        public int Denomination { get; }
        public int Quantity { get; }

        public virtual bool DispenseRequest(int amount)
        {
            if (amount >= Denomination)
            {
                var num = Quantity;
                var remainder = amount;
                while (remainder >= Denomination && num > 0)
                {
                    remainder -= Denomination;
                    num--;
                }

                if (remainder != 0)
                {
                    return next.DispenseRequest(remainder);
                }

                return true;
            }
            else
            {
                return next.DispenseRequest(amount);
            }

        }
    }
}
