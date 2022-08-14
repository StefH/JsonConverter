// ReSharper disable once CheckNamespace
namespace System;

internal static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string? value)
    {
#if NET35
        if (value == null)
        {
            return true;
        }

        foreach (var c in value)
        {
            if (!char.IsWhiteSpace(c))
            {
                return false;
            }
        }

        return true;
#else
        return string.IsNullOrWhiteSpace(value);
#endif
    }
}