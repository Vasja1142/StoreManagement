// Файл: StoreManagement.Collections/CollectionHandler.cs
namespace StoreManagement.Collections
{
    /// <summary>
    /// Представляет метод, который будет обрабатывать события CollectionCountChanged и CollectionReferenceChanged.
    /// </summary>
    /// <param name="source">Источник события (объект MyNewCollection).</param>
    /// <param name="args">Аргументы события, содержащие информацию об изменении.</param>
    public delegate void CollectionHandler(object source, CollectionHandlerEventArgs args);
}