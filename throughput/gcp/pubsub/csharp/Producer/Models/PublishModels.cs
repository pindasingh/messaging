namespace Producer.Models;

public record PublishMessageRequest(string Message, bool? Compress = null);

public record PublishMessageResponse(string MessageId, bool Compressed);
