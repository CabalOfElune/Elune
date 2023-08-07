using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public SettingsManager settingsManager;

    private string tempGameFilesPath = "";
    
    public TMP_InputField gameFilesPathField;

    public void Awake() {
        FillFields();
    }

    public void FillFields() {
        this.tempGameFilesPath = settingsManager.settings.GameFilesPath;
        gameFilesPathField.text = tempGameFilesPath;
        print($"Set path field to {gameFilesPathField.text}");
    }

    public void OnGameFilesPathChanged(string path) {
        print($"Path: {path}");
        tempGameFilesPath = path;
    }

    public void SaveSettings() {
        settingsManager.settings.GameFilesPath = this.tempGameFilesPath;
        settingsManager.Save();
    }
}
