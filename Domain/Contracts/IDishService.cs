using FoodDelivery.Domain.Models;
using FoodDelivery.DTO.DishDTO;

namespace FoodDelivery.Domain.Contracts
{
    /// <summary>
    /// Сервис для управления блюдами и их категориями
    /// </summary>
    public interface IDishService
    {
        /// <summary>
        /// Получает список всех блюд
        /// </summary>
        /// <returns>Коллекция всех блюд в меню</returns>
        Task<List<DishDTO>> GetAllAsync();

        /// <summary>
        /// Добавляет новое блюдо в меню
        /// </summary>
        /// <param name="dish">Данные нового блюда</param>
        Task<List<DishDTO>> AddDishAsync(DishDTO dish);

        /// <summary>
        /// Обновляет информацию о блюде
        /// </summary>
        /// <param name="id">Идентификатор блюда</param>
        /// <param name="dish">Новые данные блюда</param>
        Task<List<DishDTO>> UpdateDishAsync(int id, DishDTO dish);

        /// <summary>
        /// Добавляет новую категорию блюд
        /// </summary>
        /// <param name="name">Название категории</param>
        Task AddCategoryAsync(string name);

        /// <summary>
        /// Обновляет название категории блюд
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <param name="name">Новое название категории</param>
        Task UpdateCategoryAsync(int id, string name);
    }
}
