using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using WebApplication2.Models;

namespace WebApplication2.Services;

public class ItemService : IItemService
{
    private const string connection_string = "DefaultEndpointsProtocol=https;AccountName=storagemaltsevp35;AccountKey=xsk+cJOY+2kHNpSBL2gOWSgNMTTLdXEsYLoQhUwYyT+a8J5/J2GTF9I80bbibQX9OumGkk0IyXb9+AStouaElg==;EndpointSuffix=core.windows.net";
    private readonly TableClient _tableClient;
    private readonly BlobContainerClient _imagesContainerClient;

    public ItemService()
    {
        var tableServiceClient = new TableServiceClient(connection_string);
        _tableClient = tableServiceClient.GetTableClient("items");
        _tableClient.CreateIfNotExists();

        var blobServiceClient = new BlobServiceClient(connection_string);
        _imagesContainerClient = blobServiceClient.GetBlobContainerClient("item-images");
        _imagesContainerClient.CreateIfNotExists();
    }

    public async Task<IEnumerable<Item>> GetAllItemsAsync()
    {
        var items = new List<Item>();

        var query = _tableClient.QueryAsync<Item>(filter: "PartitionKey eq 'Item'");

        await foreach (var entity in query)
        {
            items.Add(entity);
        }

        return items;
    }

    public async Task<Item?> GetItemByIdAsync(int id)
    {
        try
        {
            var response = await _tableClient.GetEntityAsync<Item>("Item", id.ToString());
            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<Item> CreateItemAsync(Item item, IFormFile? imageFile = null)
    {
        var allItems = await GetAllItemsAsync();
        item.Id = allItems.Any() ? allItems.Max(i => i.Id) + 1 : 1;
        item.PartitionKey = "Item";
        item.RowKey = item.Id.ToString();
        item.CreatedAt = DateTime.UtcNow;
        item.ImageUrl = await UploadImageToBlobAsync(imageFile, item.Id);


        await _tableClient.AddEntityAsync(item);

        return item;
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        try
        {
            var existing = await GetItemByIdAsync(id);
            if (existing == null)
                return false;

            await _tableClient.DeleteEntityAsync("Item", id.ToString());
            return true;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return false;
        }
    }

    private async Task<string> UploadImageToBlobAsync(IFormFile imageFile, int itemId)
    {
        var allowedTypes = new[] { "image/png", "image/jpeg", "image/jpg" };
        if (!allowedTypes.Contains(imageFile.ContentType.ToLower()))
        {
            throw new ArgumentException("Only PNG, JPG, JPEG images are allowed");
        }

        var extension = Path.GetExtension(imageFile.FileName);
        if (string.IsNullOrEmpty(extension))
        {
            extension = imageFile.ContentType switch
            {
                "image/png" => ".png",
                "image/jpeg" => ".jpg",
                "image/jpg" => ".jpg",
                _ => ".jpg"
            };
        }

        var fileName = $"item_{itemId}_{DateTime.UtcNow.Ticks}{extension}";
        var blobClient = _imagesContainerClient.GetBlobClient(fileName);

        using var stream = imageFile.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);

        return blobClient.Uri.ToString();
    }
}