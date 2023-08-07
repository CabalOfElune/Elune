using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public SettingsManager settingsManager;

    private string gameFilesPath = "";

    public void OnGameFilesPathChanged(string path) {
        print($"Path: {path}");
        gameFilesPath = path;
    }

    public void SaveSettings() {
        settingsManager.settings.gameFilesPath = gameFilesPath;
        settingsManager.Save();
    }
}
