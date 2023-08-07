using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class GameSettings {
    // Serialized Fields
    public string GameFilesPath { get; set; }

    public GameSettings() {}

    public static GameSettings FromYaml(string yaml) {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        return deserializer.Deserialize<GameSettings>(yaml);
    }

    public string ToYaml() {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        return serializer.Serialize(this);
    } 
}