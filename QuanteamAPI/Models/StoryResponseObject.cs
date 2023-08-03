using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuanteamAPI.Models
{
    public class StoryResponseObject
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Uri { get; set; }
        public string? PostedBy { get; set; }

        [JsonPropertyName("time")]
        [JsonConverter(typeof(DateTimeOffsetConverter))]
        public DateTimeOffset Time { get; set; }
        public int Score { get; set; }
        public int CommentCount { get; set; }
    }



    public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                // If the value in JSON is a number (Unix timestamp), convert it to DateTimeOffset
                long timestamp = reader.GetInt64();
                return DateTimeOffset.FromUnixTimeSeconds(timestamp);
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                // If the value in JSON is a string, try to parse it to DateTimeOffset
                string dateString = reader.GetString()!;
                if (DateTimeOffset.TryParse(dateString, out DateTimeOffset dateTimeOffset))
                {
                    return dateTimeOffset;
                }
            }

            // Fallback value if the JSON data is not in the expected format
            return default;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.ToUnixTimeSeconds());
        }
    }
}

