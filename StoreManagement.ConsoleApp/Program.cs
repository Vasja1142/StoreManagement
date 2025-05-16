// Файл: StoreManagement.ConsoleApp/Program.cs
using System;
using System.Text;
//using StoreManagement.ConsoleApp.Demos; // Убедитесь, что это using есть

namespace StoreManagement.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Запуск приложения управления коллекцией...");

            // Запускаем новый демо-класс для проверки рефакторинга
            //RefactoredCollectionDemo.Run();

            // Пока ApplicationMenu не запускаем, чтобы сфокусироваться на тестах ядра
            // ApplicationMenu menu = new ApplicationMenu();
            // menu.Run();

            Console.WriteLine("\nРабота приложения завершена. Нажмите любую клавишу для закрытия окна.");
            Console.ReadKey();
        }
    }
}