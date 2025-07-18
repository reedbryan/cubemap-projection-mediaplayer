using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Settings
{
    public float brightness1;
    public float contrast1;
    public float saturation1;

    public float brightness2;
    public float contrast2;
    public float saturation2;

    public float brightness3;
    public float contrast3;
    public float saturation3;

    public Settings(float brightness1, float contrast1, float saturation1
                   ,float brightness2, float contrast2, float saturation2
                   ,float brightness3, float contrast3, float saturation3 )
    {
        this.brightness1 = brightness1;
        this.contrast1 = contrast1;
        this.saturation1 = saturation1;

        this.brightness2 = brightness2;
        this.contrast2 = contrast2;
        this.saturation2 = saturation2;

        this.brightness3 = brightness3;
        this.contrast3 = contrast3;
        this.saturation3 = saturation3;
    }
}

public static class SettingsManager
{
    private const string PresetListKey = "VisualPresetsList";

    public static void SavePreset(string presetName, Settings settings)
    {
        string json = JsonUtility.ToJson(settings);
        PlayerPrefs.SetString(presetName, json);

        var names = GetPresetNames();
        if (!names.Contains(presetName))
        {
            names.Add(presetName);
            SavePresetNames(names);
        }

        PlayerPrefs.Save();
    }

    public static void DeletePreset(string presetName)
    {
        if (!PlayerPrefs.HasKey(presetName)) return;

        PlayerPrefs.DeleteKey(presetName);

        var names = GetPresetNames();
        names.Remove(presetName);
        SavePresetNames(names);

        PlayerPrefs.Save();
    }

    public static Settings LoadPreset(string presetName)
    {
        if (!PlayerPrefs.HasKey(presetName)) return null;
        string json = PlayerPrefs.GetString(presetName);
        return JsonUtility.FromJson<Settings>(json);
    }

    public static List<string> GetPresetNames()
    {
        string json = PlayerPrefs.GetString(PresetListKey, "{\"items\":[]}");
        return JsonUtility.FromJson<Wrapper>(json).items;
    }

    private static void SavePresetNames(List<string> names)
    {
        string json = JsonUtility.ToJson(new Wrapper { items = names });
        PlayerPrefs.SetString(PresetListKey, json);
    }


    [System.Serializable]
    private class Wrapper { public List<string> items; }
}
