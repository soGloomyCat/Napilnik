using System;
using System.Collections.Generic;

namespace OnlineStore
{
    class OnlineStore
    {
        static void Main(string[] args)
        {
            Warehouse warehouse;
            Store store;

            warehouse = new Warehouse();
            store = new Store(warehouse);
            store.Work();
        }
    }

    class Good
    {
        private string _label;

        public string Label => _label;

        public Good(string label)
        {
            _label = label;
        }
    }

    class GoodCell : IReadOnlyCell
    {
        public Good Good { get; }
        public int Count { get; private set; }

        public GoodCell(string label, int count)
        {
            Good = new Good(label);
            Count = count;
        }

        public void IncreaseCount(int count)
        {
            Count += count;
        }

        public void DecreaseCount(int count)
        {
            Count -= count;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Название - {Good.Label}, количество - {Count};");
        }
    }

    class Cart
    {
        private List<GoodCell> _goods;

        public IReadOnlyList<IReadOnlyCell> GoodCells => _goods;

        public Cart()
        {
            _goods = new List<GoodCell>();
        }

        public void TakeGood(GoodCell goodCell, int count)
        {
            GoodCell tempCell;

            foreach (var cell in _goods)
            {
                if (cell.Good.Label == goodCell.Good.Label)
                {
                    cell.IncreaseCount(count);
                    return;
                }
            }

            tempCell = new GoodCell(goodCell.Good.Label, count);
            _goods.Add(tempCell);
        }

        public void ShowGoods()
        {
            int currentIndex;

            currentIndex = 1;

            if (!CheckCartFullness())
                return;

            foreach (var cell in _goods)
            {
                Console.Write($"{currentIndex++}. ");
                cell.ShowInfo();
            }
        }

        public void PlaceOrder(string paylink)
        {
            if (!CheckCartFullness())
                return;

            Console.Write($"Заказ оформлен. Номер заказа: {paylink}");
            _goods = new List<GoodCell>();
        }

        public bool CheckCartFullness()
        {
            if (_goods.Count == 0)
            {
                Console.WriteLine("Корзина пуста.");
                return false;
            }

            return true;
        }
    }

    class Warehouse
    {
        private List<GoodCell> _goodCells;

        public int Fulness => _goodCells.Count;
        public IReadOnlyList<IReadOnlyCell> GoodCells => _goodCells;

        public Warehouse()
        {
            _goodCells = new List<GoodCell>();
        }

        public void AddGood(string label, int count)
        {
            GoodCell tempCell;

            foreach (var cell in _goodCells)
            {
                if (cell.Good.Label.ToLower() == label.ToLower())
                {
                    cell.IncreaseCount(count);
                    return;
                }
            }

            tempCell = new GoodCell(label, count);
            _goodCells.Add(tempCell);
        }

        public void ShowAssortment()
        {
            if (!CheckWarehouseFullness())
                return;
        }

        public GoodCell TakeCell(int index)
        {
            return _goodCells[index];
        }

        public void ChangeCount(int cellIndex, int count)
        {
            _goodCells[cellIndex].DecreaseCount(count);
        }

        public bool CheckWarehouseFullness()
        {
            if (_goodCells.Count == 0)
            {
                Console.WriteLine("Склад пуст.");
                return false;
            }

            return true;
        }
    }

    class InformationOutputTerminal
    {
        public void ShowStock(IReadOnlyList<IReadOnlyCell> GoodCells)
        {
            int currentIndex;

            currentIndex = 1;

            foreach (var cell in GoodCells)
            {
                Console.Write($"{currentIndex++}. ");
                cell.ShowInfo();
            }
        }

        public string GeneratePaylink()
        {
            Random randomNumber;

            randomNumber = new Random();
            return randomNumber.Next(0, int.MaxValue).ToString();
        }
    }

    class Store
    {
        private Warehouse _warehouse;
        private Cart _cart;
        private InformationOutputTerminal _terminal;

        public Store(Warehouse warehouse)
        {
            _warehouse = warehouse;
            _cart = new Cart();
            _terminal = new InformationOutputTerminal();
        }

        public void Work()
        {
            int userInput;

            while (true)
            {
                Console.WriteLine("1. Заказать товар на склад;\n2. Просмотреть товар на складе;\n3. Добавить в корзину;\n4. Просмотреть товар в корзине;\n5. Оформить заказ;");
                Console.Write("Введите номер команды: ");

                if (int.TryParse(Console.ReadLine(), out userInput))
                {
                    switch (userInput)
                    {
                        case 1:
                            OrderGood();
                            break;
                        case 2:
                            if (_warehouse.CheckWarehouseFullness())
                                _terminal.ShowStock(_warehouse.GoodCells);

                            break;
                        case 3:
                            AddGoodToCart();
                            break;
                        case 4:
                            if (_cart.CheckCartFullness())
                                _terminal.ShowStock(_cart.GoodCells);

                            break;
                        case 5:
                            _cart.PlaceOrder(_terminal.GeneratePaylink());
                            break;
                        default:
                            Console.WriteLine("Номер команды не распознан.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Введены некорректные данные.");
                }

                Console.Write("Нажмите любую кнопку для продолжения.");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private void OrderGood()
        {
            string tempLabel;
            int tempCount;

            Console.Write("Название товара: ");
            tempLabel = Console.ReadLine();

            Console.Write("Количество товара: ");

            if (!TryParseUserInput(out tempCount))
            {
                return;
            }

            _warehouse.AddGood(tempLabel, tempCount);
        }

        private bool TryParseUserInput(out int currentValue)
        {
            int tempValue;

            if (int.TryParse(Console.ReadLine(), out tempValue))
            {
                if (tempValue <= 0)
                {
                    Console.WriteLine("Можно указывать только положительное значение.");
                    currentValue = 0;
                    return false;
                }

                currentValue = tempValue;
                return true;
            }

            currentValue = 0;
            return false;
        }

        private void AddGoodToCart()
        {
            int userInput;
            int goodsCount;

            if (!_warehouse.CheckWarehouseFullness())
                return;

            _terminal.ShowStock(_warehouse.GoodCells);
            Console.Write("Номер позиции: ");

            if (!int.TryParse(Console.ReadLine(), out userInput) || userInput > _warehouse.Fulness)
            {
                Console.WriteLine("Указано некорректное значение.");
                return;
            }

            Console.Write("Количество: ");

            if (!int.TryParse(Console.ReadLine(), out goodsCount))
            {
                Console.WriteLine("Указано некорректное значение.");
                return;
            }

            if (goodsCount > _warehouse.TakeCell(--userInput).Count)
            {
                Console.WriteLine("Нельзя взять больше, чем есть на складе.");
                return;
            }

            _cart.TakeGood(_warehouse.TakeCell(userInput), goodsCount);
            _warehouse.ChangeCount(userInput, goodsCount);
        }
    }

    interface IReadOnlyCell
    {
        Good Good { get; }
        int Count { get; }

        void ShowInfo();
    }
}
