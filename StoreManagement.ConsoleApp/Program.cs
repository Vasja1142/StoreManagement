// Файл: StoreManagement.ConsoleApp/Program.cs
using StoreManagement.ConsoleApp.Demos;
using System;

namespace StoreManagement.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            //Console.WriteLine("Запуск демонстрации Лабораторной Работы №10...");
            //Lab10Demo.Run();
            //Console.WriteLine("\n===============================================\n");

            //Console.WriteLine("Запуск демонстрации Лабораторной Работы №12...");
            //Lab12Demo.Run();
            //Console.WriteLine("\n===============================================\n");

            Console.WriteLine("Запуск демонстрации Лабораторной Работы №13...");
            Lab13Demo.Run();

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}