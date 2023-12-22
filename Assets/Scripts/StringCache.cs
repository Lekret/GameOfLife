using System.Collections.Generic;

public static class StringCache<T>
{
    private static readonly Dictionary<(string Format, T Value), string> _cache = new();

    public static string Get(string format, T value)
    {
        if (!_cache.TryGetValue((format, value), out var cachedString))
        {
            cachedString = string.Format(format, value);
            _cache.Add((format, value), cachedString);
        }

        return cachedString;
    }
}