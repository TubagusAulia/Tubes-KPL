using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ManajemenPerpus.Core.Models;

public class BukuConverter : JsonConverter<FactoryBuku>
{
    public static FactoryBuku ConvertToDto(BukuDTO dto)
    {
        return dto.Kategori.ToLower() switch
        {
            "fiksi" => new BukuFiksiCreator(
                dto.IdBuku, dto.Judul, dto.Penulis,
                dto.Penerbit, dto.Kategori, dto.Sinopsis, dto.TanggalMasuk
            ),

            "non fiksi" => new BukuNonFiksiCreator(
                dto.IdBuku, dto.Judul, dto.Penulis,
                dto.Penerbit, dto.Kategori, dto.Sinopsis, dto.TanggalMasuk
            ),

            _ => throw new Exception("Kategori tidak valid: " + dto.Kategori)
        };
    }

    public override FactoryBuku Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var jsonDoc = JsonDocument.ParseValue(ref reader))
        {
            var root = jsonDoc.RootElement;
            if (!root.TryGetProperty("Kategori", out var kategoriProp))
                throw new JsonException("Property 'Kategori' not found in JSON.");

            var kategori = kategoriProp.GetString();

            if (kategori == "Fiksi")
            {
                return JsonSerializer.Deserialize<BukuFiksiCreator>(root.GetRawText(), options) ?? throw new JsonException("Deserialization returned null.");
            }
            else if (kategori == "Non Fiksi")
            {
                return JsonSerializer.Deserialize<BukuNonFiksiCreator>(root.GetRawText(), options) ?? throw new JsonException("Deserialization returned null.");
            }
            else
            {
                throw new NotSupportedException($"Kategori '{kategori}' is not supported.");
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, FactoryBuku value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
    }
}
