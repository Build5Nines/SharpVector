using System;
using System.Collections.Concurrent;

namespace Build5Nines.SharpVector.VectorEncoding;

/// <summary>
/// Lookup for <see cref="IVectorEncoding"/> instances by id. Used by the
/// persistence layer to rehydrate encoded vectors when a database is loaded
/// from disk.
/// </summary>
public static class VectorEncodingRegistry
{
    private static readonly ConcurrentDictionary<string, IVectorEncoding> _encodings;

    static VectorEncodingRegistry()
    {
        _encodings = new ConcurrentDictionary<string, IVectorEncoding>(StringComparer.OrdinalIgnoreCase);
        Register(RawFloat32Encoding.Instance);
        Register(Int8ScalarQuantizationEncoding.Instance);
        Register(RaBitQEncoding.Instance);
        Register(TurboQuantEncoding.Instance);
    }

    /// <summary>
    /// Register a custom encoding. Re-registering an existing id replaces the entry.
    /// </summary>
    public static void Register(IVectorEncoding encoding)
    {
        if (encoding is null) throw new ArgumentNullException(nameof(encoding));
        if (string.IsNullOrEmpty(encoding.Id))
            throw new ArgumentException("Encoding must have a non-empty Id.", nameof(encoding));
        _encodings[encoding.Id] = encoding;
    }

    /// <summary>
    /// Resolve an encoding by id. Throws if the id is unknown.
    /// </summary>
    public static IVectorEncoding Get(string encodingId)
    {
        if (encodingId is null) throw new ArgumentNullException(nameof(encodingId));
        if (_encodings.TryGetValue(encodingId, out var enc)) return enc;
        throw new KeyNotFoundException($"No vector encoding registered with id '{encodingId}'.");
    }

    /// <summary>
    /// Resolve an encoding by id without throwing.
    /// </summary>
    public static bool TryGet(string encodingId, out IVectorEncoding encoding)
    {
        return _encodings.TryGetValue(encodingId, out encoding!);
    }
}
