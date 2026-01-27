using System.Text;
using Shared.Messaging;

namespace Consumer.Push.Messaging;

public static class PushMessageDecoder
{
    public static string Decode(PubSubPushMessage message)
    {
        var bytes = Convert.FromBase64String(message.Data);

        if (message.Attributes.TryGetValue(CompressionHelper.ContentEncodingAttribute, out var encoding)
            && string.Equals(encoding, CompressionHelper.GzipEncoding, StringComparison.OrdinalIgnoreCase))
        {
            return CompressionHelper.Decompress(bytes);
        }

        return Encoding.UTF8.GetString(bytes);
    }
}
