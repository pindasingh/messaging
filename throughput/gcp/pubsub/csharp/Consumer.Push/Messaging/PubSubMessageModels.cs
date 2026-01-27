namespace Consumer.Push.Messaging;

public class PubSubPushRequest
{
    public PubSubPushMessage Message { get; set; } = new();
    public string Subscription { get; set; } = string.Empty;
}

public class PubSubPushMessage
{
    public string Data { get; set; } = string.Empty;
    public string MessageId { get; set; } = string.Empty;
    public DateTime PublishTime { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new();
}
