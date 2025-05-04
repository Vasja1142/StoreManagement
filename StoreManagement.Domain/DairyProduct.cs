// Файл: StoreManagement.Domain/DairyProduct.cs
using System;

namespace StoreManagement.Domain
{
    /// <summary>
    /// Представляет молочный продукт, наследуется от Продукта.
    /// </summary>
    [Serializable]
    public class DairyProduct : Product
    {
  
        public double FatContent { get; set; }
        public double Volume { get; set; }     

        // --- Конструкторы ---

        public DairyProduct() : base() { }

        public DairyProduct(string name, decimal price, string manufacturer, DateTime expirationDate, double fatContent, double volume)
            : base(name, price, manufacturer, expirationDate)
        {
            FatContent = fatContent;
            Volume = volume;
        }

        public DairyProduct(DairyProduct other) : base(other)
        {
            this.FatContent = other.FatContent;
            this.Volume = other.Volume;
        }

        // --- Переопределение методов ---

        public override void Show()
        {
            base.Show();
            Console.WriteLine($"  Жирность (%): {FatContent:F1}"); // Формат с 1 знаком после запятой
            Console.WriteLine($"  Объем (л): {Volume:F2}");     // Формат с 2 знаками после запятой
        }

        public override void Init()
        {
            base.Init(); // Инициализация базовых полей Product (включая Goods)
            FatContent = ReadDouble("Введите жирность (%): ", 0); // Не меньше 0
            Volume = ReadDouble("Введите объем (л): ", 0.001); // Больше 0
        }

        public override void RandomInit()
        {
            string[] dairyNames = { "Молоко", "Кефир", "Сметана", "Творог", "Ряженка" };
            string[] dairyManufacturers = { "Простоквашино", "Домик в деревне", "Веселый Молочник", "Савушкин Продукт" };

            base.RandomInit(); // Инициализация базовых полей Product (включая Goods)
            // Переопределим некоторые поля для большей релевантности
            Name = dairyNames[random.Next(dairyNames.Length)];
            Manufacturer = dairyManufacturers[random.Next(dairyManufacturers.Length)];
            FatContent = Math.Round(random.NextDouble() * 10, 1); // Жирность от 0.0 до 10.0 %
            Volume = Math.Round(random.NextDouble() * 1.5 + 0.2, 2); // Объем от 0.20 до 1.70 л
        }

        public override bool Equals(object? obj)
        {
            if (!base.Equals(obj)) return false;
            DairyProduct other = (DairyProduct)obj;
            // Используем допуск для сравнения double
            return Math.Abs(FatContent - other.FatContent) < 0.001 && Math.Abs(Volume - other.Volume) < 0.001;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), FatContent, Volume);
        }

        public override string ToString()
        {
            return $"{base.ToString()}, Жирн.: {FatContent:F1}%, Объем: {Volume:F2} л";
        }

        public override object Clone()
        {
            return new DairyProduct(this); // Используем конструктор копирования
        }
    }
}