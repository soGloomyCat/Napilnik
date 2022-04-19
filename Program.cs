using System;
using System.Collections.Generic;

namespace OCP
{
    class Program
    {
        static void Main(string[] args)
        {
            OrderForm orderForm;
            PaymentHandler paymentHandler;

            orderForm = new OrderForm(new List<PaymentSystem> { new PaymentSystem("QIWI", "Проверка платежа через QIWI..."),
                                                                new PaymentSystem("WebMoney", "Проверка платежа через WebMoney..."),
                                                                new PaymentSystem("Card", "Проверка платежа через Card...") });
            paymentHandler = new PaymentHandler();
            orderForm.ShowForm();
            orderForm.ChoosePaymentSystem();
            paymentHandler.TransferUser(orderForm.GetSelectedSystem(Console.ReadLine()));

        }
    }

    class PaymentSystem
    {
        private string _id;
        private string _verificationMessage;

        public PaymentSystem(string id, string verificationMessage)
        {
            _id = id;
            _verificationMessage = verificationMessage;
        }

        public string Id => _id;
        public string VerificationMessage => _verificationMessage;
    }

    class OrderForm
    {
        private List<PaymentSystem> _database;

        public OrderForm(List<PaymentSystem> database)
        {
            _database = database;
        }

        public void ShowForm()
        {
            Console.Write("Мы принимаем платежи через: ");
            ShowAvailableSystems();
        }

        public void ChoosePaymentSystem()
        {
            Console.WriteLine("Какой системой вы хотите воспользоваться?");
            Console.Write("Введите название идентификатор платежной системы: ");
        }

        public PaymentSystem GetSelectedSystem(string userInput)
        {
            PaymentSystem tempSystem;

            foreach (var paymentSystem in _database)
            {
                if (userInput.ToLower() == paymentSystem.Id.ToLower())
                {
                    tempSystem = paymentSystem;
                    return tempSystem;
                }
            }

            return null;
        }

        private void ShowAvailableSystems()
        {
            foreach (var paymentSystem in _database)
            {
                Console.Write($"{paymentSystem.Id}; ");
            }

            Console.WriteLine();
        }
    }

    class PaymentHandler
    {
        public void TransferUser(PaymentSystem currentSystem)
        {
            if (currentSystem == null)
            {
                Console.WriteLine("Платежная система не найдена. Повторите попытку позже.");
            }
            else
            {
                Console.WriteLine($"Переход на страницу платежной системы {currentSystem.Id}.");
                Console.WriteLine(currentSystem.VerificationMessage);
                Console.WriteLine("Оплата прошла успешно.");
            }
        }
    }
}
