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
        private bool _isActive;

        public IReadOnlyList<IReadOnlyCell> GoodCells => _goods;
        public int Fullness => _goods.Count;

        public Cart()
        {
            _goods = new List<GoodCell>();
            _isActive = false;
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

            if (!CheckFullness())
                return;

            foreach (var cell in _goods)
            {
                Console.Write($"{currentIndex++}. ");
                cell.ShowInfo();
            }
        }

        public void Order(string paylink)
        {
            if (!CheckFullness())
                return;

            Console.Write($"Заказ оформлен. Номер заказа: {paylink}\n");
            _goods = new List<GoodCell>();
        }

        public bool CheckFullness()
        {
            if (Fullness == 0)
                return false;

            return true;
        }

        public void Activate()
        {
            _isActive = true;
        }
    }

    class Warehouse
    {
        private List<GoodCell> _goodCells;

        public int Fullness => _goodCells.Count;
        public IReadOnlyList<IReadOnlyCell> GoodCells => _goodCells;

        public Warehouse()
        {
            _goodCells = new List<GoodCell>();
        }

        public void Delive(string label, int count)
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
            if (!CheckFullness())
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

        public bool CheckFullness()
        {
            if (Fullness == 0)
                return false;

            return true;
        }
    }

    class InformationOutputTerminal
    {
        private int _firstCartErrorIndex;
        private int _secondCartErrorIndex;
        private int _firstWarehouseErrorIndex;

        public int FirstCartErrorIndex => _firstCartErrorIndex;
        public int SecondCartErrorIndex => _secondCartErrorIndex;
        public int FirstWarehouseErrorIndex => _firstWarehouseErrorIndex;

        public InformationOutputTerminal()
        {
            _firstCartErrorIndex = 1;
            _secondCartErrorIndex = 2;
            _firstWarehouseErrorIndex = 3;
        }

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

        public void OutputErrorMessage(int indexError)
        {
            switch (indexError)
            {
                case 1:
                    Console.WriteLine("Для начала, вам нужно взять корзину.");
                    break;
                case 2:
                    Console.WriteLine("Корзина пуста.");
                    break;
                case 3:
                    Console.WriteLine("Склад пуст.");
                    break;
                default:
                    break;
            }
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
            _terminal = new InformationOutputTerminal();
        }

        public void Work()
        {
            int userInput;

            while (true)
            {
                Console.WriteLine("1. Заказать товар на склад;\n2. Просмотреть товар на складе;\n3. Добавить в корзину;\n4. Просмотреть товар в корзине;\n5. Оформить заказ;\n6. Взять корзину");
                Console.Write("Введите номер команды: ");

                if (int.TryParse(Console.ReadLine(), out userInput))
                {
                    switch (userInput)
                    {
                        case 1:
                            OrderGood();
                            break;
                        case 2:
                            if (CheckFulness(_warehouse))
                                _terminal.ShowStock(_warehouse.GoodCells);

                            break;
                        case 3:
                            if (CheckPresenceCart() && CheckFulness(_cart))
                                AddGoodToCart();

                            break;
                        case 4:
                            if (CheckPresenceCart() && CheckFulness(_cart))
                                _terminal.ShowStock(_cart.GoodCells);

                            break;
                        case 5:
                            if (CheckPresenceCart() && CheckFulness(_cart))
                                _cart.Order(_terminal.GeneratePaylink());
                            break;
                        case 6:
                            _cart = Cart();
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

        private Cart Cart()
        {
            Cart tempCart = new Cart();
            tempCart.Activate();
            Console.WriteLine("Вы взяли корзину.");
            return tempCart;
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

            _warehouse.Delive(tempLabel, tempCount);
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

            if (!_warehouse.CheckFullness())
                return;

            _terminal.ShowStock(_warehouse.GoodCells);
            Console.Write("Номер позиции: ");

            if (!int.TryParse(Console.ReadLine(), out userInput) || userInput > _warehouse.Fullness)
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

        private bool CheckFulness(Warehouse warehouse)
        {
            if (warehouse.CheckFullness())
                return true;
            else
                _terminal.OutputErrorMessage(_terminal.FirstWarehouseErrorIndex);

            return false;
        }

        private bool CheckFulness(Cart cart)
        {
            if (_cart.CheckFullness())
                return true;
            else
                _terminal.OutputErrorMessage(_terminal.SecondCartErrorIndex);

            return false;
        }

        private bool CheckPresenceCart()
        {
            if (_cart != null)
                return true;
            else
                _terminal.OutputErrorMessage(_terminal.FirstCartErrorIndex);

            return false;
        }
    }

    interface IReadOnlyCell
    {
        Good Good { get; }
        int Count { get; }

        void ShowInfo();
    }
}
