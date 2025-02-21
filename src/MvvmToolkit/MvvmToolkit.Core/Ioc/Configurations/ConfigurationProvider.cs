using MvvmToolkit.Core.Ioc.Primitives;
using System;

namespace MvvmToolkit.Core.Ioc.Configurations
{
    public abstract class ConfigurationProvider : IConfigurationProvider
    {
        private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();
        protected IDictionary<string, string?> Data { get; set; }
        protected ConfigurationProvider()
        {
            Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        }
        /// <summary>
        /// Attempts to find a value with the given key, returns true if one is found, false otherwise.
        /// </summary>
        /// <param name="key">The key to lookup.</param>
        /// <param name="value">The value found at key if one is found.</param>
        /// <returns>True if key has a value, false otherwise.</returns>
        public virtual bool TryGet(string key, out string? value)
            => Data.TryGetValue(key, out value);

        /// <summary>
        /// Sets a value for a given key.
        /// </summary>
        /// <param name="key">The configuration key to set.</param>
        /// <param name="value">The value to set.</param>
        public virtual void Set(string key, string? value)
            => Data[key] = value;

        /// <summary>
        /// Loads (or reloads) the data for this provider.
        /// </summary>
        public virtual void Load()
        { }
        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath)
        {
            var results = new List<string>();

            if (parentPath is null)
            {
                foreach (var kv in Data)
                {
                    results.Add(Segment(kv.Key, 0));
                }
            }
            else
            {
                foreach (KeyValuePair<string, string?> kv in Data)
                {
                    if (kv.Key.Length > parentPath.Length &&
                        kv.Key.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase) &&
                        kv.Key[parentPath.Length] == ':')
                    {
                        results.Add(Segment(kv.Key, parentPath.Length + 1));
                    }
                }
            }

            results.AddRange(earlierKeys);
            results.Sort(ConfigurationKeyComparer.Comparison);

            return results;
        }
        private static string Segment(string key, int depth)
        {
            int indexOf = key.IndexOf(':',depth);
            return indexOf < 0 ? key.Substring(depth) : key.Substring(depth, indexOf - depth);
        }

        public IChangeToken GetReloadToken()
        {
            return _reloadToken;
        }
        public override string ToString()
        {
            return GetType().Name;
        }
    }

    /// <summary>
    /// IComparer implementation used to order configuration keys.
    /// </summary>
    public class ConfigurationKeyComparer : IComparer<string>
    {
        private const char KeyDelimiter = ':';

        /// <summary>
        /// The default instance.
        /// </summary>
        public static ConfigurationKeyComparer Instance { get; } = new ConfigurationKeyComparer();

        /// <summary>A comparer delegate with the default instance.</summary>
        internal static Comparison<string> Comparison { get; } = Instance.Compare;

        /// <summary>
        /// Compares two strings.
        /// </summary>
        /// <param name="x">First string.</param>
        /// <param name="y">Second string.</param>
        /// <returns>Less than 0 if x is less than y, 0 if x is equal to y and greater than 0 if x is greater than y.</returns>
        public int Compare(string? x, string? y)
        {
            ReadOnlySpan<char> xSpan = x.AsSpan();
            ReadOnlySpan<char> ySpan = y.AsSpan();

            xSpan = SkipAheadOnDelimiter(xSpan);
            ySpan = SkipAheadOnDelimiter(ySpan);

            // Compare each part until we get two parts that are not equal
            while (!xSpan.IsEmpty && !ySpan.IsEmpty)
            {
                int xDelimiterIndex = xSpan.IndexOf(KeyDelimiter);
                int yDelimiterIndex = ySpan.IndexOf(KeyDelimiter);

                int compareResult = Compare(
                    xDelimiterIndex == -1 ? xSpan : xSpan.Slice(0, xDelimiterIndex),
                    yDelimiterIndex == -1 ? ySpan : ySpan.Slice(0, yDelimiterIndex));

                if (compareResult != 0)
                {
                    return compareResult;
                }

                xSpan = xDelimiterIndex == -1 ? default :
                    SkipAheadOnDelimiter(xSpan.Slice(xDelimiterIndex + 1));
                ySpan = yDelimiterIndex == -1 ? default :
                    SkipAheadOnDelimiter(ySpan.Slice(yDelimiterIndex + 1));
            }

            return xSpan.IsEmpty ? (ySpan.IsEmpty ? 0 : -1) : 1;

            static ReadOnlySpan<char> SkipAheadOnDelimiter(ReadOnlySpan<char> a)
            {
                while (!a.IsEmpty && a[0] == KeyDelimiter)
                {
                    a = a.Slice(1);
                }
                return a;
            }

            static int Compare(ReadOnlySpan<char> a, ReadOnlySpan<char> b)
            {
#if NETCOREAPP
                bool aIsInt = int.TryParse(a, out int value1);
                bool bIsInt = int.TryParse(b, out int value2);
#else
                bool aIsInt = int.TryParse(a.ToString(), out int value1);
                bool bIsInt = int.TryParse(b.ToString(), out int value2);
#endif
                int result;

                if (!aIsInt && !bIsInt)
                {
                    // Both are strings
                    result = a.CompareTo(b, StringComparison.OrdinalIgnoreCase);
                }
                else if (aIsInt && bIsInt)
                {
                    // Both are int
                    result = value1 - value2;
                }
                else
                {
                    // Only one of them is int
                    result = aIsInt ? -1 : 1;
                }

                return result;
            }
        }
    }
}
