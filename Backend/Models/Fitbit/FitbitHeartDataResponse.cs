using System.Text.Json.Serialization;

namespace Backend.Models.Fitbit;

public partial class FitbitHeartDataResponse{
        [JsonPropertyName("activities-heart")]
        public List<ActivitiesHeart>? ActivitiesHeart { get; set; }
    }

public partial class ActivitiesHeart{
        [JsonPropertyName("dateTime")]
        public DateTimeOffset DateTime { get; set; }

        [JsonPropertyName("value")]
        public Value? Value { get; set; }
    }

public partial class Value{
        [JsonPropertyName("customHeartRateZones")]
        public List<object>? CustomHeartRateZones { get; set; }

        [JsonPropertyName("heartRateZones")]
        public List<HeartRateZone>? HeartRateZones { get; set; }
    }

public partial class HeartRateZone{
        [JsonPropertyName("caloriesOut")]
        public double CaloriesOut { get; set; }

        [JsonPropertyName("max")]
        public long Max { get; set; }

        [JsonPropertyName("min")]
        public long Min { get; set; }

        [JsonPropertyName("minutes")]
        public long Minutes { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }