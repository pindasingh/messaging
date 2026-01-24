using System.Text.Json;

namespace Shared.Events;

public class OrderCreatedV1
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string ToJson() => JsonSerializer.Serialize(this);

    public static OrderCreatedV1? FromJson(string json) =>
        JsonSerializer.Deserialize<OrderCreatedV1>(json);
}
