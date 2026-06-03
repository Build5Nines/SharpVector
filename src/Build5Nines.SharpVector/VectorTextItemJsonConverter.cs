using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Build5Nines.SharpVector.VectorEncoding;

namespace Build5Nines.SharpVector;

/// <summary>
/// JsonConverter factory for <see cref="VectorTextItem{TDocument, TMetadata}"/>.
/// Handles both the legacy on-disk shape (where vectors are written as a
/// plain float array under "Vector") and the new shape that carries an
/// explicit encoding tag plus base64 bytes. Raw-encoded vectors continue to
/// be written in the legacy shape so previously-saved databases remain
/// byte-identical.
/// </summary>
internal sealed class VectorTextItemJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType) return false;
        var def = typeToConvert.GetGenericTypeDefinition();
        return def == typeof(VectorTextItem<,>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var args = typeToConvert.GetGenericArguments();
        var converterType = typeof(VectorTextItemJsonConverter<,>).MakeGenericType(args[0], args[1]);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}

internal sealed class VectorTextItemJsonConverter<TDocument, TMetadata>
    : JsonConverter<VectorTextItem<TDocument, TMetadata>>
{
    public override VectorTextItem<TDocument, TMetadata> Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object for VectorTextItem.");

        TDocument? text = default;
        TMetadata? metadata = default;
        float[]? legacyVector = null;
        string? encodingId = null;
        int? dimensions = null;
        byte[]? encodedBytes = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) break;
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected property name in VectorTextItem.");

            string? propName = reader.GetString();
            reader.Read();

            switch (propName)
            {
                case "Text":
                case "text":
                    text = JsonSerializer.Deserialize<TDocument>(ref reader, options);
                    break;
                case "Metadata":
                case "metadata":
                    metadata = JsonSerializer.Deserialize<TMetadata>(ref reader, options);
                    break;
                case "Vector":
                case "vector":
                    legacyVector = JsonSerializer.Deserialize<float[]>(ref reader, options);
                    break;
                case "EncodingId":
                case "encodingId":
                    encodingId = reader.GetString();
                    break;
                case "Dimensions":
                case "dimensions":
                    dimensions = reader.GetInt32();
                    break;
                case "EncodedBytes":
                case "encodedBytes":
                    encodedBytes = reader.GetBytesFromBase64();
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        IEncodedVector encoded;
        if (encodingId is not null && encodedBytes is not null && dimensions is not null)
        {
            var encoding = VectorEncodingRegistry.Get(encodingId);
            encoded = encoding.LoadFromBytes(encodedBytes, dimensions.Value);
        }
        else if (legacyVector is not null)
        {
            encoded = RawFloat32Encoding.Instance.Encode(legacyVector);
        }
        else
        {
            // Empty/missing vector — preserve null-ish behavior with a zero-length raw encoding.
            encoded = RawFloat32Encoding.Instance.Encode(Array.Empty<float>());
        }

        return new VectorTextItem<TDocument, TMetadata>(text!, metadata, encoded);
    }

    public override void Write(
        Utf8JsonWriter writer, VectorTextItem<TDocument, TMetadata> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Text");
        JsonSerializer.Serialize(writer, value.Text, options);

        writer.WritePropertyName("Metadata");
        JsonSerializer.Serialize(writer, value.Metadata, options);

        if (value.EncodedVector.EncodingId == RawFloat32Encoding.EncodingId)
        {
            // Preserve the legacy on-disk shape exactly: a plain float array
            // under "Vector". This means files written by raw-encoded databases
            // match what older versions produced, byte for byte.
            writer.WritePropertyName("Vector");
            JsonSerializer.Serialize(writer, value.Vector, options);
        }
        else
        {
            writer.WriteString("EncodingId", value.EncodedVector.EncodingId);
            writer.WriteNumber("Dimensions", value.EncodedVector.Dimensions);
            writer.WriteBase64String("EncodedBytes", value.EncodedVector.GetBytes());
        }

        writer.WriteEndObject();
    }
}
