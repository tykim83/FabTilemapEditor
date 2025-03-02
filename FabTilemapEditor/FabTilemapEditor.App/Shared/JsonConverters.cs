using FabTilemapEditor.App.Tilemap;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FabTilemapEditor.App.Shared;

[JsonSerializable(typeof(TilemapDto))]
public partial class MyJsonContext : JsonSerializerContext
{
}

public class TilemapDataConverter(int tilesWidth) : JsonConverter<int[]>
{
    public override void Write(Utf8JsonWriter writer, int[] value, JsonSerializerOptions options)
    {
        StringBuilder sb = new();
        sb.AppendLine("[");

        for (int i = 0; i < value.Length; i++)
        {
            if (i % tilesWidth == 0)
                sb.Append("\t\t\t\t");

            sb.Append(value[i]);

            if (i < value.Length - 1)
            {
                sb.Append(",");

                if ((i + 1) % tilesWidth == 0)
                    sb.AppendLine();
            }
        }
        sb.AppendLine();
        sb.Append("\t\t\t]");

        writer.WriteRawValue(sb.ToString(), skipInputValidation: true);
    }

    public override int[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var values = new List<int>();

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                values.Add(reader.GetInt32());
            }
        }

        return [.. values];
    }
}
