using System;
using System.Collections.Generic;
using System.Linq; // Используем стандартные операторы LINQ для реализации
using StoreManagement.Collections; // Доступ к MyNewCollection
using StoreManagement.Domain;     // Доступ к Goods (для примера с Average и ограничения)

namespace StoreManagement.Services
{
    /// <summary>
    /// Содержит методы расширения для класса MyNewCollection<T>.
    /// </summary>
    public static class MyCollectionExtensions
    {
        /// <summary>
        /// Фильтрует коллекцию на основе заданного предиката.
        /// </summary>
        /// <typeparam name="T">Тип элементов, должен быть наследником Goods.</typeparam>
        /// <param name="collection">Исходная коллекция.</param>
        /// <param name="predicate">Функция для проверки каждого элемента.</param>
        /// <returns>Новая коллекция MyNewCollection<T>, содержащая отфильтрованные элементы.</returns>
        public static MyNewCollection<T> Filter<T>(this MyNewCollection<T> collection, Func<T, bool> predicate)
            where T : Goods // <--- ДОБАВЛЕНО ОГРАНИЧЕНИЕ
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var filteredItems = collection.Where(predicate);
            return new MyNewCollection<T>($"{collection.Name}-Filtered", filteredItems);
        }

        /// <summary>
        /// Вычисляет среднее значение числового поля для элементов коллекции Goods.
        /// </summary>
        /// <param name="collection">Коллекция товаров.</param>
        /// <param name="selector">Функция для выбора числового значения (decimal) из элемента.</param>
        /// <returns>Среднее значение или 0, если коллекция пуста.</returns>
        public static decimal AggregateAverage(this MyNewCollection<Goods> collection, Func<Goods, decimal> selector)
        {
            // Этот метод уже специфичен для MyNewCollection<Goods>, поэтому для него 'T' уже Goods.
            // Ограничение здесь не нужно, так как 'T' не является параметром типа метода.
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            if (!collection.Any())
            {
                return 0m;
            }
            return collection.Average(selector);
        }

        /// <summary>
        /// Сортирует коллекцию по указанному ключу.
        /// </summary>
        /// <typeparam name="T">Тип элементов, должен быть наследником Goods.</typeparam>
        /// <typeparam name="TKey">Тип ключа сортировки.</typeparam>
        /// <param name="collection">Исходная коллекция.</param>
        /// <param name="keySelector">Функция для извлечения ключа сортировки.</param>
        /// <returns>Новая коллекция MyNewCollection<T>, содержащая отсортированные элементы.</returns>
        public static MyNewCollection<T> SortBy<T, TKey>(this MyNewCollection<T> collection, Func<T, TKey> keySelector)
            where T : Goods // <--- ДОБАВЛЕНО ОГРАНИЧЕНИЕ
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            var sortedItems = collection.OrderBy(keySelector);
            return new MyNewCollection<T>($"{collection.Name}-Sorted", sortedItems);
        }

        /// <summary>
        /// Группирует элементы коллекции по указанному ключу.
        /// </summary>
        /// <typeparam name="T">Тип элементов, должен быть наследником Goods.</typeparam>
        /// <typeparam name="TKey">Тип ключа группировки.</typeparam>
        /// <param name="collection">Исходная коллекция.</param>
        /// <param name="keySelector">Функция для извлечения ключа группировки.</param>
        /// <returns>Перечисление групп (IGrouping).</returns>
        public static IEnumerable<IGrouping<TKey, T>> GroupByCriteria<T, TKey>(this MyNewCollection<T> collection, Func<T, TKey> keySelector)
            where T : Goods // <--- ДОБАВЛЕНО ОГРАНИЧЕНИЕ
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            return collection.GroupBy(keySelector);
        }
    }
}