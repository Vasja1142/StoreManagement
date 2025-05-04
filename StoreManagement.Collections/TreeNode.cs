// Файл: StoreManagement.Collections/TreeNode.cs
namespace StoreManagement.Collections
{
    /// <summary>
    /// Представляет узел в бинарном дереве BinaryTree<T>.
    /// </summary>
    /// <typeparam name="T">Тип данных, хранящихся в узле.</typeparam>
    internal class TreeNode<T> // internal - используется внутри сборки
    {
        public T Data { get; set; }
        public TreeNode<T>? Left { get; set; }
        public TreeNode<T>? Right { get; set; }

        public TreeNode(T data)
        {
            Data = data;
            Left = null;
            Right = null;
        }

        // Для удобства отладки и вывода
        public override string ToString()
        {
            return Data?.ToString() ?? "null";
        }
    }
}