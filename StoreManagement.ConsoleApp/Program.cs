// Файл: StoreManagement.ConsoleApp/Program.cs
// using StoreManagement.ConsoleApp.Demos; // Больше не нужны демо-классы напрямую
using System;
using System.Text; // Для Encoding

namespace StoreManagement.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Установка кодировки для корректного отображения
            Console.OutputEncoding = Encoding.UTF8;
            // Для корректного ввода с консоли тоже может понадобиться
            // Console.InputEncoding = Encoding.UTF8; // Раскомментировать при необходимости

            Console.WriteLine("Запуск приложения управления коллекцией...");

            // Создаем и запускаем главное меню приложения
            ApplicationMenu menu = new ApplicationMenu();
            menu.Run();

            // Сообщение после выхода из меню Run()
            Console.WriteLine("\nРабота приложения завершена. Нажмите любую клавишу для закрытия окна.");
            Console.ReadKey();
        }
    }
}