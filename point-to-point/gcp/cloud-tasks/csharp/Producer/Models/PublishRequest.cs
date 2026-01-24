namespace Producer.Models;

public record PublishMessageRequest(string Message);

public record PublishMessageResponse(string TaskName);
