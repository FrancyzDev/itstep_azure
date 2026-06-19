using Azure;
using Azure.Data.Tables;

using System.Text.Json.Serialization;
namespace WebApplication2.Models
{
    public class Item : ITableEntity
    {
        public string PartitionKey { get; set; } = "Item";
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public int Id
        {
            get => int.TryParse(RowKey, out var id) ? id : 0;
            set => RowKey = value.ToString();
        }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
