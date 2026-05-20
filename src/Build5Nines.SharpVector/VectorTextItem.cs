using System.Text.Json;
using System.Text.Json.Serialization;
using Build5Nines.SharpVector.VectorEncoding;

namespace Build5Nines.SharpVector;

/// <summary>
/// An interface for storing a text with its metadata and vector data.
/// </summary>
/// <typeparam name="TDocument"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
public interface IVectorTextItem<TDocument, TMetadata>
{
    TDocument Text { get; set; }
    TMetadata? Metadata { get; set; }

    /// <summary>
    /// The float vector representation. When the item is backed by a
    /// compressed encoding this returns a decoded approximation; assigning
    /// replaces the encoded vector with a fresh raw (lossless) encoding.
    /// </summary>
    float[] Vector { get; set; }

    /// <summary>
    /// The encoded form actually stored by the database. This is the
    /// authoritative representation: <see cref="Vector"/> is derived from it.
    /// The default implementation adapts to/from <see cref="Vector"/> via
    /// raw float32 encoding so existing external implementations of this
    /// interface keep compiling.
    /// </summary>
    IEncodedVector EncodedVector
    {
        get => RawFloat32Encoding.Instance.Encode(Vector);
        set => Vector = value.Decode();
    }
}

/// <summary>
/// An interface for storing a text with its metadata and vector.
/// </summary>
/// <typeparam name="TMetadata"></typeparam>
public interface IVectorTextItem<TMetadata> : IVectorTextItem<string, TMetadata>
{ }

/// <summary>
/// A class for storing a text with its metadata and vector.
/// </summary>
/// <typeparam name="TDocument"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
[JsonConverter(typeof(VectorTextItemJsonConverterFactory))]
public class VectorTextItem<TDocument, TMetadata> : IVectorTextItem<TDocument, TMetadata>
{
    public VectorTextItem(TDocument text, TMetadata? metadata, float[] vector)
    {
        Text = text;
        Metadata = metadata;
        EncodedVector = RawFloat32Encoding.Instance.Encode(vector);
    }

    public VectorTextItem(TDocument text, TMetadata? metadata, IEncodedVector encodedVector)
    {
        Text = text;
        Metadata = metadata;
        EncodedVector = encodedVector ?? throw new ArgumentNullException(nameof(encodedVector));
    }

    public TDocument Text { get; set; }
    public TMetadata? Metadata { get; set; }

    public IEncodedVector EncodedVector { get; set; }

    public float[] Vector
    {
        get => EncodedVector.Decode();
        set => EncodedVector = RawFloat32Encoding.Instance.Encode(value);
    }
}

/// <summary>
/// A class for storing a text with its metadata and vector data.
/// </summary>
/// <typeparam name="TMetadata"></typeparam>
public class VectorTextItem<TMetadata> : VectorTextItem<string, TMetadata>, IVectorTextItem<TMetadata>
{
    public VectorTextItem(string text, TMetadata? metadata, float[] vector)
        : base(text, metadata, vector)
    { }

    public VectorTextItem(string text, TMetadata? metadata, IEncodedVector encodedVector)
        : base(text, metadata, encodedVector)
    { }
}
