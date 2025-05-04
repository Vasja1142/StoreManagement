// Файл: StoreManagement.Domain/Comparers/SortByNameComparer.cs
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis; // Для [AllowNull]

namespace StoreManagement.Domain.Comparers
{
    /// <summary>
    /// Реализует сравнение объектов Goods по названию (Name).
    /// </summary>
    public class SortByNameComparer : IComparer<Goods>
    {
        /// <summary>
        /// Сравнивает два товара по их названиям.
        /// </summary>
        /// <param name="x">Первый товар.</param>
        /// <param name="y">Второй товар.</param>
        /// <returns>Результат сравнения строк названий.</returns>
        public int Compare(Goods? x, Goods? y)
        {
            // Обработка null для безопасности
            if (x == null && y == null) return 0;
            if (x == null) return -1; // null меньше любого объекта
            if (y == null) return 1;  // любой объект больше null

            // Используем стандартное сравнение строк
            return string.Compare(x.Name, y.Name, StringComparison.CurrentCulture);
        }
    }
}