using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class SettingsManager : MonoBehaviour
{
    const string SETTINGS_FILE = "config.yml";
    private string settingsFilePath;
    
    public GameSettings settings = GameSettings.Defaults();


    void Start()
    {
        settingsFilePath = Path.Combine(Application.persistentDataPath, SETTINGS_FILE);

        // Initialize with defaults
        if (!File.Exists(settingsFilePath)) {
            print($"No {SETTINGS_FILE} exists. Creating at {settingsFilePath}");
            Save();
            return;
        }

        print($"Found settings at {SETTINGS_FILE}");
    }

    public void Save() {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var yaml = serializer.Serialize(settings);
        print($"Saving to {settingsFilePath}");
        File.WriteAllText(settingsFilePath, yaml);
    }
}
