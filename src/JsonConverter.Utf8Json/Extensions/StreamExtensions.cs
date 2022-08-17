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
}