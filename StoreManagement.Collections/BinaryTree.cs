// Файл: StoreManagement.Collections/BinaryTree.cs
using System;
using System.Collections.Generic;
using System.Linq; // Для OrderBy в ConvertToSearchTree
using StoreManagement.Domain; // Для доступа к Goods

namespace StoreManagement.Collections
{
    /// <summary>
    /// Реализация бинарного дерева.
    /// Может быть построено как идеально сбалансированное
    /// и преобразовано в бинарное дерево поиска.
    /// </summary>
    /// <typeparam name="T">Тип хранимых данных.</typeparam>
    public class BinaryTree<T> where T : IComparable<T> // Добавляем ограничение для BST
    {
        private TreeNode<T>? _root;
        private int _count;
        public int Count => _count;

        // --- Построение идеально сбалансированного дерева ---

        /// <summary>
        /// Создает идеально сбалансированное бинарное дерево из заданного количества элементов,
        /// используя функцию генерации данных.
        /// </summary>
        /// <param name="count">Количество узлов в дереве.</param>
        /// <param name="dataFactory">Функция, создающая новый экземпляр данных типа T.</param>
        public void BuildBalancedTree(int count, Func<T> dataFactory)
        {
            if (count <= 0)
            {
                _root = null;
                _count = 0;
                return;
            }
            _count = count;
            _root = BuildBalancedRecursive(count, dataFactory);
        }

        /// <summary>
        /// Рекурсивный вспомогательный метод для построения сбалансированного дерева.
        /// </summary>
        private TreeNode<T> BuildBalancedRecursive(int n, Func<T> dataFactory)
        {
            if (n == 0)
            {
                return null!; // Базовый случай рекурсии
            }

            // Создаем корень текущего поддерева
            TreeNode<T> newNode = new TreeNode<T>(dataFactory());

            // Рассчитываем количество узлов в левом и правом поддеревьях
            int nl = n / 2;         // Количество узлов в левом поддереве
            int nr = n - nl - 1;    // Количество узлов в правом поддереве

            // Рекурсивно строим левое и правое поддеревья
            newNode.Left = BuildBalancedRecursive(nl, dataFactory);
            newNode.Right = BuildBalancedRecursive(nr, dataFactory);

            return newNode;
        }

        // --- Вывод дерева (In-order traversal) ---

        /// <summary>
        /// Выводит дерево в консоль с использованием симметричного обхода (in-order).
        /// </summary>
        public void PrintInOrder()
        {
            Console.WriteLine("Бинарное дерево (симметричный обход):");
            if (_root == null)
            {
                Console.WriteLine("Дерево пусто.");
                return;
            }
            PrintInOrderRecursive(_root, 0);
            Console.WriteLine($"Всего узлов: {_count}");
        }

        private void PrintInOrderRecursive(TreeNode<T>? node, int level)
        {
            if (node != null)
            {
                // 1. Рекурсивно обходим левое поддерево
                PrintInOrderRecursive(node.Left, level + 1);

                // 2. Посещаем (выводим) текущий узел
                Console.WriteLine($"{new string(' ', level * 3)}[{level}] {node.Data}");

                // 3. Рекурсивно обходим правое поддерево
                PrintInOrderRecursive(node.Right, level + 1);
            }
        }

        // --- Задание 7: Подсчет узлов по первой букве имени ---

        /// <summary>
        /// **(Вариант 7)** Находит количество элементов дерева (если T - Goods),
        /// у которых поле Name начинается с заданного символа (без учета регистра).
        /// </summary>
        /// <param name="startChar">Символ, с которого должно начинаться имя.</param>
        /// <returns>Количество найденных узлов.</returns>
        public int CountNodesWithNameStartingWith(char startChar)
        {
            if (_root == null) return 0;
            return CountNodesRecursive(_root, startChar);
        }

        private int CountNodesRecursive(TreeNode<T>? node, char startChar)
        {
            if (node == null)
            {
                return 0;
            }

            int count = 0;
            // Проверяем текущий узел
            if (node.Data is Goods goodsItem) // Безопасная проверка типа
            {
                if (!string.IsNullOrEmpty(goodsItem.Name) &&
                    goodsItem.Name.StartsWith(startChar.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    count = 1;
                }
            }

            // Рекурсивно подсчитываем в левом и правом поддеревьях
            count += CountNodesRecursive(node.Left, startChar);
            count += CountNodesRecursive(node.Right, startChar);

            return count;
        }

        // --- Преобразование в Бинарное Дерево Поиска (BST) ---

        /// <summary>
        /// Преобразует текущее дерево в Бинарное Дерево Поиска (BST).
        /// Данные извлекаются, сортируются (используя IComparable<T>),
        /// и дерево перестраивается.
        /// </summary>
        public void ConvertToSearchTree()
        {
            if (_root == null || _count == 0) return;

            // 1. Извлечь все данные из дерева в список
            List<T> dataList = new List<T>(_count);
            ExtractDataInOrder(_root, dataList);

            // 2. Отсортировать список (T должен реализовывать IComparable<T>)
            dataList.Sort(); // Использует стандартный Comparer<T>.Default

            // 3. Перестроить дерево из отсортированного списка как BST
            _root = BuildBstFromSortedListRecursive(dataList, 0, dataList.Count - 1);
            // _count остается тем же
        }

        /// <summary>
        /// Вспомогательный метод для извлечения данных (in-order).
        /// </summary>
        private void ExtractDataInOrder(TreeNode<T>? node, List<T> list)
        {
            if (node != null)
            {
                ExtractDataInOrder(node.Left, list);
                list.Add(node.Data);
                ExtractDataInOrder(node.Right, list);
            }
        }

        /// <summary>
        /// Рекурсивный вспомогательный метод для построения BST из отсортированного списка.
        /// </summary>
        private TreeNode<T>? BuildBstFromSortedListRecursive(List<T> sortedList, int start, int end)
        {
            if (start > end)
            {
                return null; // Базовый случай: подсписок пуст
            }

            // Выбираем средний элемент как корень поддерева
            int mid = start + (end - start) / 2;
            TreeNode<T> node = new TreeNode<T>(sortedList[mid]);

            // Рекурсивно строим левое поддерево из левой половины списка
            node.Left = BuildBstFromSortedListRecursive(sortedList, start, mid - 1);

            // Рекурсивно строим правое поддерево из правой половины списка
            node.Right = BuildBstFromSortedListRecursive(sortedList, mid + 1, end);

            return node;
        }


        // --- Очистка дерева ---

        /// <summary>
        /// Удаляет все узлы из дерева.
        /// </summary>
        public void Clear()
        {
            // Достаточно обнулить корень, сборщик мусора удалит остальные узлы,
            // так как на них не останется ссылок.
            _root = null;
            _count = 0;
        }
    }
}