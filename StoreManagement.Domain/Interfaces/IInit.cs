// Файл: StoreManagement.Domain/IInit.cs
namespace StoreManagement.Domain.Interfaces
{
    /// <summary>
    /// Интерфейс для инициализации объектов вручную или случайными данными.
    /// </summary>
    public interface IInit
    {
        /// <summary>
        /// Инициализирует объект данными, введенными с клавиатуры.
        /// </summary>
        void Init();

        /// <summary>
        /// Инициализирует объект случайными данными.
        /// </summary>
        void RandomInit();
    }
}