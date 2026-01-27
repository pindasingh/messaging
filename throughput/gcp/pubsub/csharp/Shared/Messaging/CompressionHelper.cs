using System.IO.Compression;
using System.Text;

namespace Shared.Messaging;

public static class CompressionHelper
{
    public const string ContentEncodingAttribute = "content-encoding";
    public const string GzipEncoding = "gzip";

    public static byte[] Compress(string payload)
    {
        // Fastest compression reduces CPU cost for high-throughput workloads.
        var bytes = Encoding.UTF8.GetBytes(payload);
        using var outputStream = new MemoryStream();
        using (var gzipStream = new GZipStream(outputStream, CompressionLevel.Fastest, leaveOpen: true))
        {
            gzipStream.Write(bytes, 0, bytes.Length);
        }

        return outputStream.ToArray();
    }

    public static string Decompress(byte[] bytes)
    {
        using var inputStream = new MemoryStream(bytes);
        using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
        using var reader = new StreamReader(gzipStream, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}
