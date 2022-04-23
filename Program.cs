using System;
using System.IO;
using System.Collections.Generic;

namespace RefactoringBadDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            Terminal terminal = new Terminal(new Voter(), new DataBase(@"C:\*path*\VotersData.txt"));
            terminal.SimulateButtonClick();
        }
    }

    class Voter
    {
        private string _passportData;

        public string PassportData => _passportData;

        public Voter()
        {
            EnterData();
        }

        private void EnterData()
        {
            Console.Write("Введите серию и номер паспорта: ");
            _passportData = GetCorrectedEnteredData(Console.ReadLine());

            if (_passportData == null)
                EnterData();
        }

        private string GetCorrectedEnteredData(string inputData)
        {
            string tempInputData;
            ulong tempValue;

            tempInputData = inputData.Trim().Replace(" ", string.Empty);

            if (tempInputData.Length != 10 || !ulong.TryParse(tempInputData, out tempValue))
            {
                Console.WriteLine("Неверный формат паспортных данных. Попробуйте еще раз.");
                return null;
            }

            return tempInputData;
        }
    }

    class DataBase
    {
        private const int _confirmedStatus = 0;
        private const int _unconfirmedStatus = 1;

        private string _pathFile;
        private Dictionary<string, bool> _localRegisteredVotersBase;
        private bool _userDataFound;

        public bool UserDataFound => _userDataFound;

        public DataBase(string path)
        {
            if (!File.Exists(path))
                throw new InvalidOperationException("По указанному пути соответствующий файл не найден.");

            _pathFile = path;
            _localRegisteredVotersBase = GenerateLocalBaseRegisteredVoters(GenerateLocalData(_pathFile));
            _userDataFound = false;
        }

        public void VerifyAvailabilityData(string verifiedData)
        {
            foreach (var data in _localRegisteredVotersBase)
            {
                if (data.Key == verifiedData)
                {
                    if (data.Value)
                        Console.WriteLine($"По паспорту «{verifiedData}» доступ к бюллетеню на дистанционном электронном голосовании ПРЕДОСТАВЛЕН");
                    else
                        Console.WriteLine($"По паспорту «{verifiedData}» доступ к бюллетеню на дистанционном электронном голосовании НЕ ПРЕДОСТАВЛЕН");

                    _userDataFound = true;
                    break;
                }
            }

            if (!_userDataFound)
            {
                Console.WriteLine($"Паспорт «{verifiedData}» в списке участников дистанционного голосования НЕ НАЙДЕН");
            }
        }

        public void AddNewVoter(string data)
        {
            File.AppendAllText(_pathFile, Environment.NewLine + data);
            _localRegisteredVotersBase = GenerateLocalBaseRegisteredVoters(GenerateLocalData(_pathFile));
            Console.WriteLine("Новые данные зарегистрированы.");
        }

        private string[] GenerateLocalData(string pathFile)
        {
            string[] tempDatabase;
            string tempData;

            tempDatabase = File.ReadAllLines(pathFile);

            for (int i = 0; i < tempDatabase.Length; i++)
            {
                if (tempDatabase[i].Length > 10 || tempDatabase[i].Length < 10)
                    throw new InvalidOperationException("В файле находятся паспортные данные в некорректном формате. Отредактируйте файл и повторите попытку.");

                tempData = tempDatabase[i].Trim().Replace(" ", string.Empty);
                tempDatabase[i] = tempData;
            }

            return tempDatabase;
        }

        private Dictionary<string, bool> GenerateLocalBaseRegisteredVoters(string[] localData)
        {
            Dictionary<string, bool> tempDatabase;
            Random randomStatus;
            int currentStatus;

            tempDatabase = new Dictionary<string, bool>();
            randomStatus = new Random();

            foreach (var data in localData)
            {
                currentStatus = randomStatus.Next(0, 2);

                if (currentStatus == _confirmedStatus)
                    tempDatabase.Add(data, true);
                else if (currentStatus == _unconfirmedStatus)
                    tempDatabase.Add(data, false);
            }

            return tempDatabase;
        }
    }

    class Terminal
    {
        private Voter _voter;
        private DataBase _dataBase;

        public Terminal(Voter voter, DataBase dataBase)
        {
            _voter = voter;
            _dataBase = dataBase;
        }

        public void SimulateButtonClick()
        {
            _dataBase.VerifyAvailabilityData(_voter.PassportData);

            if (!_dataBase.UserDataFound)
                TryAddNewVoterData();
        }

        private void TryAddNewVoterData()
        {
            Console.WriteLine("Хотите зарегистрироваться в системе?");
            Console.Write("Введите ответ (Да или Нет): ");

            switch (Console.ReadLine().ToLower())
            {
                case "да":
                    _dataBase.AddNewVoter(_voter.PassportData);
                    break;
                case "нет":
                    Console.WriteLine("Закрытие программы. До свидания!");
                    break;
                default:
                    Console.WriteLine("Ответ не распознан. Закрытие программы.");
                    break;
            }
        }
    }
}
