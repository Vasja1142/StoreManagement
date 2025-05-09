﻿// Файл: StoreManagement.Domain/Toy.cs
using System;

namespace StoreManagement.Domain
{
    /// <summary>
    /// Представляет игрушку, наследуется от Товара.
    /// </summary>
    [Serializable]
    public class Toy : Goods
    {
 
        public int AgeRestriction { get; set; }
        public string Material { get; set; } = "Не указан";
        
        public Toy() : base() { }

        public Toy(string name, decimal price, string manufacturer, int ageRestriction, string material)
            : base(name, price, manufacturer)
        {
            AgeRestriction = ageRestriction;
            Material = material;
        }

        public Toy(Toy other) : base(other)
        {
            this.AgeRestriction = other.AgeRestriction;
            this.Material = other.Material;
        }

        // --- Переопределение методов ---

        public override void Show()
        {
            base.Show();
            Console.WriteLine($"  Возраст: {AgeRestriction}+");
            Console.WriteLine($"  Материал: {Material}");
        }

        public override void Init()
        {
            Name = ReadString("Введите название игрушки: ");
            Price = ReadDecimal("Введите цену игрушки: ");
            Manufacturer = ReadString("Введите производителя игрушки: ");
            AgeRestriction = ReadInt("Введите возрастное ограничение (лет): ", 0); // Не меньше 0
            Material = ReadString("Введите материал игрушки: ");
        }

        public override void RandomInit()
        {
            string[] toyNames = { "Машинка", "Кукла", "Конструктор", "Мяч", "Плюшевый мишка", "Пазл" };
            string[] toyManufacturers = { "Lego", "Hasbro", "Mattel", "Мир Детства", "Полесье" };
            string[] materials = { "Пластик", "Текстиль", "Дерево", "Резина", "Металл" };

            Name = toyNames[random.Next(toyNames.Length)];
            Price = (decimal)(random.NextDouble() * 2000 + 100); // Цена от 100 до 2100
            Manufacturer = toyManufacturers[random.Next(toyManufacturers.Length)];
            AgeRestriction = random.Next(0, 12); // Возраст от 0+ до 12+
            Material = materials[random.Next(materials.Length)];
        }

        public override bool Equals(object? obj)
        {
            if (!base.Equals(obj)) return false;
            Toy other = (Toy)obj;
            return AgeRestriction == other.AgeRestriction && Material == other.Material;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), AgeRestriction, Material);
        }

        public override string ToString()
        {
            return $"{base.ToString()}, Возраст: {AgeRestriction}+, Материал: {Material}";
        }

        public override object Clone()
        {
            return new Toy(this); // Используем конструктор копирования
        }
    }
}