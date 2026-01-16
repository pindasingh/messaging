using System.Text.Json;

namespace Shared.Messages;

public class SampleMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string ToJson() => JsonSerializer.Serialize(this);

    public static SampleMessage? FromJson(string json) =>
        JsonSerializer.Deserialize<SampleMessage>(json);
}
