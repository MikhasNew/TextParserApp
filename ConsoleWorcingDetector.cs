using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TextParserApp
{
    class ConsoleWorcingDetector
    {
        private int CurrentPosition { get; set; }
        private bool IsShowed { get; set; }

        public ConsoleWorcingDetector()
        {
            CurrentPosition = 0;
            IsShowed = false;
        }

        public void ShowWorkingProcessDetecter()
        {
            Thread.Sleep(1000);
            IsShowed = true;
            while (IsShowed)
            {

                switch (CurrentPosition)
                {
                    case 0:
                        Console.Write('\\');
                        CurrentPosition = 1;
                        break;
                    case 1:
                        Console.Write('|');
                        CurrentPosition = 2;
                        break;
                    case 2:
                        Console.Write('/');
                        CurrentPosition = 3;
                        break;
                    default:
                        Console.Write('-');
                        CurrentPosition = 0;
                        break;

                }

                var p = Console.GetCursorPosition();
                if (p.Left != 0)
                {
                    Thread.Sleep(1000);
                    Console.SetCursorPosition(p.Left - 1, p.Top);
                }
            }

        }
        public void ShowWorkingProcessDetecterAsync()
        {
            Thread t = new Thread(ShowWorkingProcessDetecter); //создает новый поток
            t.Name = "2Thred";
            t.Start(); //запускает новый поток

        }
        public void Stop()
        {
            IsShowed = false;
            Console.WriteLine();
        }

    }
}