// Файл: StoreManagement.Collections/DNode.cs
namespace StoreManagement.Collections
{
    /// <summary>
    /// Представляет узел в двунаправленном списке DoublyLinkedList<T>.
    /// </summary>
    /// <typeparam name="T">Тип данных, хранящихся в узле.</typeparam>
    internal class DNode<T> // internal - т.к. используется только внутри сборки Collections
    {
        public T Data { get; set; }
        public DNode<T>? Previous { get; set; }
        public DNode<T>? Next { get; set; }

        public DNode(T data)
        {
            Data = data;
            Previous = null;
            Next = null;
        }
    }
}