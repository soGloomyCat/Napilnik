using System;
using System.IO;

namespace Logging
{
    class Logging
    {
        static void Main(string[] args)
        {
            PathFinder ConsolePathfinder = new PathFinder(new ConsoleLogWritter());
            PathFinder FilePathfinder1 = new PathFinder(new FileLogWritter());
            PathFinder SecureConsolePathfinder = new PathFinder(new SecureConsoleLogWritter());
            PathFinder SecureFilePathfinder1 = new PathFinder(new SecureFileLogWritter());
            PathFinder SecureMultiPathfinder1 = new PathFinder(new SecureMultiLogWritter());

            ConsolePathfinder.Find();
            FilePathfinder1.Find();
            SecureConsolePathfinder.Find();
            SecureFilePathfinder1.Find();
            SecureMultiPathfinder1.Find();

        }
    }

    class PathFinder
    {
        private ILogger _logger;

        public PathFinder(ILogger logger)
        {
            _logger = logger;
        }

        public void Find()
        {
            _logger.WriteMessage();
        }
    }

    class LogWritter : ILogger
    {
        private string _message;

        public string Message => _message;
        public bool IsNeedDay => DateTime.Now.DayOfWeek == DayOfWeek.Friday;

        public LogWritter()
        {
            _message = GetMessage();
        }

        public virtual void WriteMessage()
        {
        }

        protected void OutputInformationalMessage()
        {
            Console.WriteLine("Лог не сгенерирован, т.к. генерация производится в другой день.");
        }

        private string GetMessage()
        {
            Console.Write("Установите сообщение для логгера: ");
            return _message = Console.ReadLine();
        }
    }

    class ConsoleLogWritter : LogWritter, IConsoleLogger
    {
        public override void WriteMessage()
        {
            WriteConsoleMessage(Message);
        }

        public void WriteConsoleMessage(string message)
        {
            Console.WriteLine(message);
        }
    }

    class FileLogWritter : LogWritter, IFileLogger
    {
        public override void WriteMessage()
        {
            WriteFileMessage(Message);
        }

        public void WriteFileMessage(string message)
        {
            File.WriteAllText("log0.txt", message);
            Console.WriteLine("Файл сгенерирован");
        }
    }

    class SecureConsoleLogWritter : ConsoleLogWritter
    {
        public override void WriteMessage()
        {
            if (IsNeedDay)
                WriteConsoleMessage(Message);
            else
                OutputInformationalMessage();
        }
    }

    class SecureFileLogWritter : LogWritter, IFileLogger
    {
        public override void WriteMessage()
        {
            if (IsNeedDay)
                WriteFileMessage(Message);
            else
                OutputInformationalMessage();
        }

        public void WriteFileMessage(string message)
        {
            File.WriteAllText("log1.txt", message);
            Console.WriteLine("Файл сгенерирован");
        }
    }

    class SecureMultiLogWritter : ConsoleLogWritter, IFileLogger
    {
        public override void WriteMessage()
        {
            WriteConsoleMessage(Message);

            if (IsNeedDay)
                WriteFileMessage(Message);
            else
                OutputInformationalMessage();
        }

        public void WriteFileMessage(string message)
        {
            File.WriteAllText("log2.txt", message);
            Console.WriteLine("Файл сгенерирован");
        }
    }

    interface ILogger
    {
        string Message { get; }
        void WriteMessage();
    }

    interface IConsoleLogger : ILogger
    {
        void WriteConsoleMessage(string message);
    }

    interface IFileLogger : ILogger
    {
        void WriteFileMessage(string message);
    }
}
