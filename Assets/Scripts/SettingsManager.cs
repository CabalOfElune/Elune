using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    const string SETTINGS_FILE = "config.yml";
    private string settingsFilePath;
    
    public GameSettings settings;


    void Awake()
    {
        settingsFilePath = Path.Combine(Application.persistentDataPath, SETTINGS_FILE);

        // Initialize with defaults
        if (!File.Exists(settingsFilePath)) {
            print($"No {SETTINGS_FILE} exists. Creating at {settingsFilePath}");
            settings = new GameSettings();
            Save();
            return;
        }

        print($"Found settings at {SETTINGS_FILE}");
        Load();
    }

    public void Save() {
        string yaml = settings.ToYaml();
        print($"Saving to {settingsFilePath}");
        File.WriteAllText(settingsFilePath, yaml);
    }

    public void Load() {
        // TODO Handle exceptions
        string yaml = File.ReadAllText(settingsFilePath);
        settings = GameSettings.FromYaml(yaml);
    }
}
