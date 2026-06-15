using WebApplication2.Models;

namespace WebApplication2.Services;

public class ItemService : IItemService
{
    private readonly List<Item> _items = new();
    private int _nextId = 1;

    public ItemService()
    {
        _items.Add(new Item { Id = 1, Name = "Ноутбук", Price = 25000, CreatedAt = DateTime.UtcNow });
        _items.Add(new Item { Id = 2, Name = "Мишка", Price = 500, CreatedAt = DateTime.UtcNow });
        _items.Add(new Item { Id = 3, Name = "Клавіатура", Price = 2000, CreatedAt = DateTime.UtcNow });
        _nextId = 4;
    }

    public Task<IEnumerable<Item>> GetAllItemsAsync()
    {
        return Task.FromResult(_items.AsEnumerable());
    }

    public Task<Item?> GetItemByIdAsync(int id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        return Task.FromResult(item);
    }

    public Task<Item> CreateItemAsync(Item item)
    {
        item.Id = _nextId++;
        item.CreatedAt = DateTime.UtcNow;
        _items.Add(item);
        return Task.FromResult(item);
    }

    public Task<Item?> UpdateItemAsync(int id, Item item)
    {
        var existingItem = _items.FirstOrDefault(i => i.Id == id);
        if (existingItem == null)
            return Task.FromResult<Item?>(null);

        existingItem.Name = item.Name;
        existingItem.Price = item.Price;

        return Task.FromResult<Item?>(existingItem);
    }

    public Task<bool> DeleteItemAsync(int id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item == null)
            return Task.FromResult(false);

        return Task.FromResult(_items.Remove(item));
    }
}