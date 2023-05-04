using System.Text.Json.Serialization;

namespace Backend.Models.Fitbit;

public partial class FitbitHeartDataResponse{
        [JsonPropertyName("activities-heart")]
        public List<ActivitiesHeart>? ActivitiesHeart { get; set; }

        [JsonPropertyName("activities-heart-intraday")]
        public ActivitiesHeartIntraday activitiesheartintraday { get; set; }

    }

public class ActivitiesHeart
    {
        [JsonPropertyName("customHeartRateZones")]
        public List<object> CustomHeartRateZones { get; set; }

        [JsonPropertyName("dateTime")]
        public string DateTime { get; set; }

        [JsonPropertyName("heartRateZones")]
        public List<HeartRateZone> HeartRateZones { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

public class ActivitiesHeartIntraday
    {
        [JsonPropertyName("dataset")]
        public List<Dataset> dataset { get; set; }

        [JsonPropertyName("datasetInterval")]
        public int datasetInterval { get; set; }

        [JsonPropertyName("datasetType")]
        public string datasetType { get; set; }
    }

public class Dataset
    {
        [JsonPropertyName("time")]
        public string time { get; set; }

        [JsonPropertyName("value")]
        public int value { get; set; }
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