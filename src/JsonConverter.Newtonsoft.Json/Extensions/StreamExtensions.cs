namespace System.IO;

internal static class StreamExtensions
{
    public static string ReadAsString(this Stream stream)
    {
        return new StreamReader(stream).ReadToEnd();
    }

#if !(NET35 || NET40)
    public static Task<string> ReadAsStringAsync(this Stream stream)
    {
        return new StreamReader(stream).ReadToEndAsync();
    }
#endif

#if NET45 || NET461 || NETSTANDARD1_0_OR_GREATER
    public static Task WriteAsync(this Stream stream, byte[] buffer, CancellationToken cancellationToken = default)
    {
        return stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
    }
#endif
}