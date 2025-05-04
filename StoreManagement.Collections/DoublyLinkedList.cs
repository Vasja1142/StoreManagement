// Файл: StoreManagement.Collections/DoublyLinkedList.cs
using System;
using System.Collections; // Для IEnumerable
using System.Collections.Generic; // Для IEnumerable<T>
using StoreManagement.Domain; // Для доступа к Goods и его Name

namespace StoreManagement.Collections
{
    /// <summary>
    /// Реализация двунаправленного списка.
    /// </summary>
    /// <typeparam name="T">Тип хранимых данных.</typeparam>
    public class DoublyLinkedList<T> : IEnumerable<T> // Реализуем IEnumerable<T> для возможности перебора foreach
    {
        private DNode<T>? _head;
        private DNode<T>? _tail;
        private int _count;

        public int Count => _count;
        public bool IsEmpty => _count == 0;

        /// <summary>
        /// Добавляет элемент в конец списка.
        /// </summary>
        public void Add(T data)
        {
            DNode<T> newNode = new DNode<T>(data);
            if (_head == null) // Список пуст
            {
                _head = newNode;
                _tail = newNode;
            }
            else // Список не пуст
            {
                _tail!.Next = newNode; // _tail не может быть null здесь
                newNode.Previous = _tail;
                _tail = newNode;
            }
            _count++;
        }

        /// <summary>
        /// Удаляет первое вхождение элемента с заданным значением.
        /// Сравнение происходит через Equals().
        /// </summary>
        /// <param name="data">Данные для удаления.</param>
        /// <returns>True, если элемент найден и удален, иначе false.</returns>
        public bool Remove(T data)
        {
            DNode<T>? current = _head;
            while (current != null)
            {
                // Используем Equals для сравнения объектов
                if (EqualityComparer<T>.Default.Equals(current.Data, data))
                {
                    RemoveNode(current);
                    return true;
                }
                current = current.Next;
            }
            return false;
        }

        /// <summary>
        /// **(Вариант 7)** Удаляет первый узел, данные которого (если это Goods)
        /// имеют указанное имя.
        /// </summary>
        /// <param name="name">Имя товара для поиска и удаления.</param>
        /// <returns>True, если элемент найден и удален, иначе false.</returns>
        public bool RemoveFirstByName(string name)
        {
            // Эта операция имеет смысл только если T - это Goods или его наследник.
            // Используем 'as' для безопасного приведения типа.
            DNode<T>? current = _head;
            while (current != null)
            {
                if (current.Data is Goods goodsItem && goodsItem.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    RemoveNode(current);
                    return true;
                }
                current = current.Next;
            }
            return false;
        }


        /// <summary>
        /// Вспомогательный приватный метод для удаления узла.
        /// </summary>
        private void RemoveNode(DNode<T> nodeToRemove)
        {
            if (nodeToRemove == null) return;

            // Обновляем ссылку Next у предыдущего узла
            if (nodeToRemove.Previous != null)
            {
                nodeToRemove.Previous.Next = nodeToRemove.Next;
            }
            else // Удаляется голова списка
            {
                _head = nodeToRemove.Next;
            }

            // Обновляем ссылку Previous у следующего узла
            if (nodeToRemove.Next != null)
            {
                nodeToRemove.Next.Previous = nodeToRemove.Previous;
            }
            else // Удаляется хвост списка
            {
                _tail = nodeToRemove.Previous;
            }

            _count--;

            // Опционально: очистить ссылки удаляемого узла (помогает сборщику мусора)
            nodeToRemove.Previous = null;
            nodeToRemove.Next = null;
        }

        /// <summary>
        /// Очищает список.
        /// </summary>
        public void Clear()
        {
            // Просто обнуляем ссылки на голову и хвост,
            // сборщик мусора позаботится об остальном.
            _head = null;
            _tail = null;
            _count = 0;
        }

        /// <summary>
        /// Выводит содержимое списка в консоль.
        /// </summary>
        public void Print()
        {
            if (IsEmpty)
            {
                Console.WriteLine("Список пуст.");
                return;
            }

            Console.WriteLine("Содержимое списка:");
            DNode<T>? current = _head;
            int index = 0;
            while (current != null)
            {
                Console.WriteLine($"  [{index++}]: {current.Data}"); // Используем ToString() элемента
                current = current.Next;
            }
            Console.WriteLine($"Всего элементов: {Count}");
        }

        // --- Реализация IEnumerable<T> ---

        public IEnumerator<T> GetEnumerator()
        {
            DNode<T>? current = _head;
            while (current != null)
            {
                yield return current.Data; // yield позволяет легко реализовать итератор
                current = current.Next;
            }
        }

        // --- Реализация IEnumerable (для совместимости) ---

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // Просто вызываем обобщенную версию
        }
    }
}