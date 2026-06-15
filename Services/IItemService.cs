using WebApplication2.Models;
namespace WebApplication2.Services;

public interface IItemService
{
    Task<IEnumerable<Item>> GetAllItemsAsync();
    Task<Item?> GetItemByIdAsync(int id);
    Task<Item> CreateItemAsync(Item item);
    Task<Item?> UpdateItemAsync(int id, Item item);
    Task<bool> DeleteItemAsync(int id);
}