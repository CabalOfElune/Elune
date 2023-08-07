public class GameSettings {
    public string gameFilesPath { get; set; }

    private GameSettings() {}

    public static GameSettings Defaults() {
        var settings = new GameSettings();
        return settings;
    }
}