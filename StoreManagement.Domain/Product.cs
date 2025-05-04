// Файл: StoreManagement.Domain/Product.cs
using System;

namespace StoreManagement.Domain
{
    /// <summary>
    /// Представляет продукт питания, наследуется от Товара.
    /// </summary>
    [Serializable]

    public class Product : Goods
    {
        protected DateTime _expirationDate;

        /// <summary>
        /// Срок годности продукта. Должен быть в будущем.
        /// </summary>
        public DateTime ExpirationDate
        {
            get => _expirationDate;
            set
            {
                // Можно добавить более строгую проверку, например, > даты производства,
                // но для простоты пока ограничимся будущей датой.
                if (value.Date < DateTime.Today) // Сравниваем только даты
                {
                    throw new ArgumentOutOfRangeException(nameof(ExpirationDate), "Срок годности не может быть в прошлом.");
                }
                _expirationDate = value;
            }
        }

        // --- Конструкторы ---

        public Product() : base() { } // Вызов конструктора базового класса

        public Product(string name, decimal price, string manufacturer, DateTime expirationDate)
            : base(name, price, manufacturer) // Вызов конструктора базового класса
        {
            ExpirationDate = expirationDate;
        }

        public Product(Product other) : base(other) // Вызов конструктора копирования базового класса
        {
            this.ExpirationDate = other.ExpirationDate;
        }

        // --- Переопределение методов ---

        /// <summary>
        /// Выводит полную информацию о продукте.
        /// </summary>
        public override void Show()
        {
            base.Show(); // Вызов метода базового класса
            Console.WriteLine($"  Срок годности: {ExpirationDate:yyyy-MM-dd}");
        }

        /// <summary>
        /// Инициализирует продукт с клавиатуры.
        /// </summary>
        public override void Init()
        {
            Name = ReadString("Введите название продукта: ");
            Price = ReadDecimal("Введите цену продукта: ");
            Manufacturer = ReadString("Введите производителя продукта: ");
            // Простая валидация даты при вводе
            while (true)
            {
                try
                {
                    ExpirationDate = ReadDateTime("Введите срок годности");
                    break; // Выход из цикла, если дата корректна
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Инициализирует продукт случайными данными.
        /// </summary>
        public override void RandomInit()
        {
            string[] productNames = { "Хлеб", "Масло", "Сыр", "Колбаса", "Йогурт", "Сок" };
            string[] manufacturers = { "Местный Хлебозавод", "Молочный Мир", "Мясной Дом", "Сады Придонья" };

            Name = productNames[random.Next(productNames.Length)];
            Price = (decimal)(random.NextDouble() * 500 + 50); // Цена от 50 до 550
            Manufacturer = manufacturers[random.Next(manufacturers.Length)];
            ExpirationDate = DateTime.Today.AddDays(random.Next(1, 90)); // Срок годности от 1 до 90 дней от сегодня
        }

        /// <summary>
        /// Проверяет эквивалентность продуктов.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (!base.Equals(obj)) // Проверка базовых полей
            {
                return false;
            }
            Product other = (Product)obj;
            return ExpirationDate.Date == other.ExpirationDate.Date; // Сравниваем только дату
        }

        /// <summary>
        /// Возвращает хеш-код продукта.
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), ExpirationDate.Date);
        }

        /// <summary>
        /// Возвращает строковое представление продукта.
        /// </summary>
        public override string ToString()
        {
            return $"{base.ToString()}, Срок годн.: {ExpirationDate:yyyy-MM-dd}";
        }

        /// <summary>
        /// Создает глубокую копию объекта Product.
        /// </summary>
        public override object Clone()
        {
            // Так как DateTime - структура, MemberwiseClone достаточно для глубокой копии
            // Если бы были ссылочные типы, потребовалось бы их клонировать отдельно.
            return new Product(this); // Используем конструктор копирования
            // Альтернатива: return MemberwiseClone(); - здесь даст тот же результат
        }
    }
}