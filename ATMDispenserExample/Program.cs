using System;

namespace ATMDispenserExample
{
    class Program
    {
        static void Main(string[] args)
        {

            //create handlers
            var bills50s = new CurrencyBill(50, 1);
            var bills20s = new CurrencyBill(20, 2);
            var bills10s = new CurrencyBill(10, 5);

            //set handlers pipeline
            bills50s.RegisterNext(bills20s)
                    .RegisterNext(bills10s);

            //client code that uses handler
            while (true)
            {
                Console.WriteLine("Please enter amount to dispense:");
                var isParsed = int.TryParse(Console.ReadLine(), out var amount);

                if (isParsed)
                {

                    //sender pass the request to first handler in the pipeline
                    var isDepensible = bills50s.DispenseRequest(amount);
                    if (isDepensible)
                    {
                        Console.WriteLine($"Your amount ${amount} is dispensable!");
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
        private CurrencyBill next = CurrencyBill.Zero; //sets default handler instead of null object
        private static readonly CurrencyBill Zero;

        public int Denomination { get; }
        public int Quantity { get; }

        //A static contructor that initialize static Zero property
        //This property is used as default next handler instead of null object
        static CurrencyBill()
        {
            Zero = new ZeroCurrencyBill();
        }

        //Use to set static Zero property
        //Will always return false at it cannot process any give amount.
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

        //CurrencyBill contructor that set the denomination value and quantity
        public CurrencyBill(int denomination, int quantity)
        {
            Denomination = denomination;
            Quantity = quantity;
        }

        //Method that set next handler in the pipeline
        public CurrencyBill RegisterNext(CurrencyBill currencyBill)
        {
            next = currencyBill;
            return next;
        }

        //Method that process the request or pass is to the next handler
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
