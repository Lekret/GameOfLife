using System.Collections.Generic;

public static class StringCache<T>
{
    private static readonly Dictionary<T, string> _cache = new();

    public static string Get(string format, T value)
    {
        if (!_cache.TryGetValue(value, out var cachedString))
        {
            cachedString = string.Format(format, value);
            _cache.Add(value, cachedString);
        }

        return cachedString;
    }
}