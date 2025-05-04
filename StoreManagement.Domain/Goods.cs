// Файл: StoreManagement.Domain/Goods.cs
using System;
using System.Diagnostics.CodeAnalysis;
using StoreManagement.Domain.Interfaces; // Для [MaybeNullWhen]

namespace StoreManagement.Domain
{
    /// <summary>
    /// Абстрактный базовый класс для всех товаров.
    /// Реализует базовую функциональность, сравнение и клонирование.
    /// </summary>
    public abstract class Goods : IInit, IComparable<Goods>, ICloneable
    {
        // Статический генератор случайных чисел для RandomInit
        public static Random random = new Random();

        protected string _name = "Без имени";
        protected decimal _price;
        protected string _manufacturer = "Неизвестен";

        /// <summary>
        /// Название товара. Не может быть пустым.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Название товара не может быть пустым.", nameof(Name));
                }
                _name = value;
            }
        }

        /// <summary>
        /// Цена товара. Должна быть больше нуля.
        /// </summary>
        public decimal Price
        {
            get => _price;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Price), "Цена должна быть положительным числом.");
                }
                _price = value;
            }
        }

        /// <summary>
        /// Производитель товара. Не может быть пустым.
        /// </summary>
        public string Manufacturer
        {
            get => _manufacturer;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Производитель не может быть пустым.", nameof(Manufacturer));
                }
                _manufacturer = value;
            }
        }

        // --- Конструкторы ---

        /// <summary>
        /// Конструктор по умолчанию (для наследников).
        /// </summary>
        protected Goods() { }

        /// <summary>
        /// Конструктор с параметрами.
        /// </summary>
        protected Goods(string name, decimal price, string manufacturer)
        {
            Name = name; // Используем свойства для валидации
            Price = price;
            Manufacturer = manufacturer;
        }

        /// <summary>
        /// Конструктор копирования.
        /// </summary>
        protected Goods(Goods other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            this.Name = other.Name;
            this.Price = other.Price;
            this.Manufacturer = other.Manufacturer;
        }

        // --- Методы ---

        /// <summary>
        /// Выводит информацию об объекте в консоль (виртуальный метод).
        /// </summary>
        public virtual void Show()
        {
            Console.WriteLine($"Тип: {this.GetType().Name}");
            Console.WriteLine($"  Название: {Name}");
            Console.WriteLine($"  Цена: {Price:C}"); // Форматируем как валюту
            Console.WriteLine($"  Производитель: {Manufacturer}");
        }

        /// <summary>
        /// Абстрактный метод для инициализации с клавиатуры (реализуется в наследниках).
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// Абстрактный метод для случайной инициализации (реализуется в наследниках).
        /// </summary>
        public abstract void RandomInit();

        // --- Реализация IComparable<Goods> ---

        /// <summary>
        /// Сравнивает текущий товар с другим по ЦЕНЕ.
        /// </summary>
        /// <param name="other">Другой товар для сравнения.</param>
        /// <returns>-1, 0 или 1.</returns>
        public virtual int CompareTo(Goods? other)
        {
            if (other == null) return 1; // Считаем, что любой объект больше null
            return this.Price.CompareTo(other.Price);
        }

        // --- Переопределение стандартных методов ---

        /// <summary>
        /// Проверяет эквивалентность объектов. Сравнивает по всем полям.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Goods other = (Goods)obj;
            return Name == other.Name && Price == other.Price && Manufacturer == other.Manufacturer;
        }

        /// <summary>
        /// Возвращает хеш-код объекта.
        /// </summary>
        public override int GetHashCode()
        {
            // Используем HashCode.Combine для простого и эффективного вычисления хеш-кода
            return HashCode.Combine(Name, Price, Manufacturer, GetType()); // Включаем тип для точности Equals
        }

        /// <summary>
        /// Возвращает строковое представление объекта.
        /// </summary>
        public override string ToString()
        {
            return $"{GetType().Name}: {Name}, Цена: {Price:C}, Произв.: {Manufacturer}";
        }

        // --- Реализация ICloneable ---

        /// <summary>
        /// Создает глубокую копию объекта (в данном случае эквивалентно поверхностной).
        /// </summary>
        /// <returns>Клон объекта.</returns>
        public abstract object Clone(); // Делаем абстрактным, чтобы наследники реализовали

        /// <summary>
        /// Создает поверхностную копию объекта.
        /// </summary>
        /// <returns>Поверхностная копия.</returns>
        public virtual object ShallowCopy()
        {
            return this.MemberwiseClone();
        }

        // --- Вспомогательные методы для Init ---
        // Можно вынести в отдельный статический класс Helper, если их станет много

        /// <summary>
        /// Безопасно читает строку с консоли с приглашением.
        /// </summary>
        public static string ReadString(string prompt)
        {
            string? input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(input));
            return input;
        }

        /// <summary>
        /// Безопасно читает decimal с консоли с приглашением.
        /// </summary>
        public static decimal ReadDecimal(string prompt)
        {
            decimal value;
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), out value) && value > 0)
                {
                    return value;
                }
                Console.WriteLine("Ошибка: Введите положительное число.");
            }
        }

        /// <summary>
        /// Безопасно читает int с консоли с приглашением и проверкой диапазона.
        /// </summary>
        protected static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            int value;
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out value) && value >= min && value <= max)
                {
                    return value;
                }
                Console.WriteLine($"Ошибка: Введите целое число от {min} до {max}.");
            }
        }

        /// <summary>
        /// Безопасно читает double с консоли с приглашением и проверкой диапазона.
        /// </summary>
        protected static double ReadDouble(string prompt, double min = double.MinValue, double max = double.MaxValue)
        {
            double value;
            while (true)
            {
                Console.Write(prompt);
                // Учитываем региональные настройки для разделителя (точка или запятая)
                if (double.TryParse(Console.ReadLine()?.Replace('.', ','), out value) && value >= min && value <= max)
                {
                    return value;
                }
                Console.WriteLine($"Ошибка: Введите число от {min} до {max}.");
            }
        }

        /// <summary>
        /// Безопасно читает DateTime с консоли с приглашением.
        /// </summary>
        protected static DateTime ReadDateTime(string prompt)
        {
            DateTime value;
            while (true)
            {
                Console.Write(prompt + " (гггг-мм-дд): ");
                if (DateTime.TryParse(Console.ReadLine(), out value))
                {
                    return value;
                }
                Console.WriteLine("Ошибка: Введите дату в формате гггг-мм-дд.");
            }
        }
    }
}