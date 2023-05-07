// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

namespace Backend.Models.Nest;

public class Device{
    [JsonPropertyName("name")]
    public string name { get; set; }

    [JsonPropertyName("type")]
    public string type { get; set; }

    [JsonPropertyName("assignee")]
    public string assignee { get; set; }

    [JsonPropertyName("traits")]
    public Traits traits { get; set; }

    [JsonPropertyName("parentRelations")]
    public List<ParentRelation> parentRelations { get; set; }
}

public class ParentRelation{
    [JsonPropertyName("parent")]
    public string parent { get; set; }

    [JsonPropertyName("displayName")]
    public string displayName { get; set; }
}

public class NestDevicesPacket{
    [JsonPropertyName("devices")]
    public List<Device> devices { get; set; }
}

public class SdmDevicesTraitsConnectivity{
    [JsonPropertyName("status")]
    public string status { get; set; }
}

public class SdmDevicesTraitsFan{
    [JsonPropertyName("timerMode")]
    public string timerMode { get; set; }
}

public class SdmDevicesTraitsHumidity{
    [JsonPropertyName("ambientHumidityPercent")]
    public int ambientHumidityPercent { get; set; }
}

public class SdmDevicesTraitsInfo{
    [JsonPropertyName("customName")]
    public string customName { get; set; }
}

public class SdmDevicesTraitsSettings{
    [JsonPropertyName("temperatureScale")]
    public string temperatureScale { get; set; }
}

public class SdmDevicesTraitsTemperature{
    [JsonPropertyName("ambientTemperatureCelsius")]
    public double ambientTemperatureCelsius { get; set; }
}

public class SdmDevicesTraitsThermostatEco{
    [JsonPropertyName("availableModes")]
    public List<string> availableModes { get; set; }

    [JsonPropertyName("mode")]
    public string mode { get; set; }

    [JsonPropertyName("heatCelsius")]
    public int heatCelsius { get; set; }

    [JsonPropertyName("coolCelsius")]
    public double coolCelsius { get; set; }
}

public class SdmDevicesTraitsThermostatHvac{
    [JsonPropertyName("status")]
    public string status { get; set; }
}

public class SdmDevicesTraitsThermostatMode{
    [JsonPropertyName("mode")]
    public string mode { get; set; }

    [JsonPropertyName("availableModes")]
    public List<string> availableModes { get; set; }
}

public class SdmDevicesTraitsThermostatTemperatureSetpoint{
    [JsonPropertyName("heatCelsius")]
    public double heatCelsius { get; set; }
}

public class Traits{
    [JsonPropertyName("sdm.devices.traits.Info")]
    public SdmDevicesTraitsInfo sdmdevicestraitsInfo { get; set; }

    [JsonPropertyName("sdm.devices.traits.Humidity")]
    public SdmDevicesTraitsHumidity sdmdevicestraitsHumidity { get; set; }

    [JsonPropertyName("sdm.devices.traits.Connectivity")]
    public SdmDevicesTraitsConnectivity sdmdevicestraitsConnectivity { get; set; }

    [JsonPropertyName("sdm.devices.traits.Fan")]
    public SdmDevicesTraitsFan sdmdevicestraitsFan { get; set; }

    [JsonPropertyName("sdm.devices.traits.ThermostatMode")]
    public SdmDevicesTraitsThermostatMode sdmdevicestraitsThermostatMode { get; set; }

    [JsonPropertyName("sdm.devices.traits.ThermostatEco")]
    public SdmDevicesTraitsThermostatEco sdmdevicestraitsThermostatEco { get; set; }

    [JsonPropertyName("sdm.devices.traits.ThermostatHvac")]
    public SdmDevicesTraitsThermostatHvac sdmdevicestraitsThermostatHvac { get; set; }

    [JsonPropertyName("sdm.devices.traits.Settings")]
    public SdmDevicesTraitsSettings sdmdevicestraitsSettings { get; set; }

    [JsonPropertyName("sdm.devices.traits.ThermostatTemperatureSetpoint")]
    public SdmDevicesTraitsThermostatTemperatureSetpoint sdmdevicestraitsThermostatTemperatureSetpoint { get; set; }

    [JsonPropertyName("sdm.devices.traits.Temperature")]
    public SdmDevicesTraitsTemperature sdmdevicestraitsTemperature { get; set; }
}

