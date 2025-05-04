// Файл: StoreManagement.ConsoleApp/Program.cs
using StoreManagement.ConsoleApp.Demos; // Добавляем using для доступа к Lab10Demo


namespace StoreManagement.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Устанавливаем кодировку консоли для корректного отображения кириллицы
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Запуск демонстрации Лабораторной Работы №10...");

            // Вызываем статический метод Run из класса Lab10Demo
            Lab10Demo.Run();

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey(); // Пауза, чтобы пользователь успел увидеть вывод
        }
    }
}