using System;
using System.Security.Cryptography;
using System.Linq;

namespace PaymentSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Order currentOrder;
            PaymentSystem firstPaymentSystem;
            PaymentSystem secondPaymentSystem;
            PaymentSystem thirdPaymentSystem;

            currentOrder = new Order(1234, 12000);
            firstPaymentSystem = new PaymentSystem($"pay.system1.ru/order?amount={currentOrder.Amount}RUB&hash=");
            secondPaymentSystem = new PaymentSystem("order.system2.ru/pay?hash=");
            thirdPaymentSystem = new PaymentSystem($"system3.com/pay?amount={currentOrder.Amount}&curency=RUB&hash=");
            Console.WriteLine(firstPaymentSystem.GetPayingLink(currentOrder.Id));
            Console.WriteLine(secondPaymentSystem.GetPayingLink(currentOrder.Id, currentOrder.Amount));
            Console.WriteLine(thirdPaymentSystem.GetPayingLink(currentOrder.Amount, currentOrder.Id, currentOrder.SecureCode));
        }
    }

    class PaymentSystem : IPaymentSystem
    {
        private string _label;

        public PaymentSystem(string label) => (_label) = (label);

        public string GetPayingLink(int orderId)
        {
            return $"{_label}{GenerateCode(orderId)}";
        }

        public string GetPayingLink(int orderId, int orderAmount)
        {
            return $"{_label}{GenerateCode(orderId, orderAmount)}";
        }

        public string GetPayingLink(int orderAmount, int orderId, int secureCode)
        {
            return $"{_label}{GenerateCode(orderAmount, orderId, secureCode)}";
        }

        private string GenerateCode(int orderId)
        {
            MD5 hashCode;

            hashCode = MD5.Create();

            return string.Concat(hashCode.ComputeHash(BitConverter.GetBytes(orderId)).Select(x => x.ToString("X")));
        }

        private string GenerateCode(int orderId, int orderAmount)
        {
            MD5 hashCode;
            string convertedHashCode;
            string convertedAmount;

            hashCode = MD5.Create();
            convertedHashCode = string.Concat(hashCode.ComputeHash(BitConverter.GetBytes(orderId)).Select(x => x.ToString("X")));
            convertedAmount = string.Concat(BitConverter.GetBytes(orderAmount).Select(x => x.ToString("X")));

            return string.Concat(convertedHashCode, convertedAmount);
        }

        private string GenerateCode(int orderAmount, int orderId, int secureCode)
        {
            SHA1 hashCode;
            string convertedHashCode;
            string convertedId;
            string convertedCode;

            hashCode = SHA1.Create();
            convertedHashCode = string.Concat(hashCode.ComputeHash(BitConverter.GetBytes(orderAmount)).Select(x => x.ToString("X")));
            convertedId = string.Concat(BitConverter.GetBytes(orderId).Select(x => x.ToString("X")));
            convertedCode = string.Concat(BitConverter.GetBytes(secureCode).Select(x => x.ToString("X")));

            return string.Concat(convertedHashCode, convertedId, convertedCode);
        }
    }

    class Order
    {
        public readonly int Id;
        public readonly int Amount;
        public readonly int SecureCode;

        public Order(int id, int amount)
        {
            Id = id;
            Amount = amount;
            SecureCode = GenerateSecureCode();
        }

        private int GenerateSecureCode()
        {
            Random randomValue = new Random();

            return randomValue.Next();
        }
    }

    interface IPaymentSystem
    {
        string GetPayingLink(int initialValue);
    }
}
