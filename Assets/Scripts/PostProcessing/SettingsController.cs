using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro; // if using TMP Dropdown and InputField

public class SettingsController : MonoBehaviour
{
    public Volume globalVolume1;
    public Volume globalVolume2;
    public Volume globalVolume3;

    [Header("UI Elements")]
    public TMP_Dropdown presetDropdown;
    public TMP_InputField saveInputField;
    public Button saveButton;
    public Button addButton;    
    public Button deleteButton;

    [Header("Camera 1 Sliders")]
    public Slider brightnessSlider1;
    public Slider contrastSlider1;
    public Slider saturationSlider1;

    [Header("Camera 2 Sliders")]
    public Slider brightnessSlider2;
    public Slider contrastSlider2;
    public Slider saturationSlider2;

    [Header("Camera 3 Sliders")]
    public Slider brightnessSlider3;
    public Slider contrastSlider3;
    public Slider saturationSlider3;


    private ColorAdjustments colorAdjustments1;
    private ColorAdjustments colorAdjustments2;
    private ColorAdjustments colorAdjustments3;

    void Start()
    {
        globalVolume1.profile.TryGet(out colorAdjustments1);
        globalVolume2.profile.TryGet(out colorAdjustments2);
        globalVolume3.profile.TryGet(out colorAdjustments3);

        PopulateDropdown();
        presetDropdown.onValueChanged.AddListener(OnDropdownChanged);
        saveInputField.onEndEdit.AddListener(SubmitPresetNameWithEnter);
        addButton.onClick.AddListener(AddCurrentAsPreset);
        saveButton.onClick.AddListener(UpdateCurrentPreset);
        deleteButton.onClick.AddListener(DeleteCurrentPreset);

        LoadInitialPreset();
    }

    void PopulateDropdown()
    {
        var names = SettingsManager.GetPresetNames();
        presetDropdown.ClearOptions();
        presetDropdown.AddOptions(names);
    }

    void OnDropdownChanged(int index)
    {
        string presetName = presetDropdown.options[index].text;
        var settings = SettingsManager.LoadPreset(presetName);
        if (settings != null){
            ApplySettings(settings);
            UpdateUI(settings);
        }
    }

    public void ApplySettings(Settings settings)
    {
        if (colorAdjustments1 == null
           || colorAdjustments2 == null
           || colorAdjustments3 == null) return;

        colorAdjustments1.postExposure.value = settings.brightness1;
        colorAdjustments1.contrast.value = settings.contrast1;
        colorAdjustments1.saturation.value = settings.saturation1;
        
        colorAdjustments2.postExposure.value = settings.brightness2;
        colorAdjustments2.contrast.value = settings.contrast2;
        colorAdjustments2.saturation.value = settings.saturation2;

        colorAdjustments3.postExposure.value = settings.brightness3;
        colorAdjustments3.contrast.value = settings.contrast3;
        colorAdjustments3.saturation.value = settings.saturation3;
    }
    public void UpdateUI(Settings settings)
    {
        // Camera 1
        brightnessSlider1.SetValueWithoutNotify(settings.brightness1);
        contrastSlider1.SetValueWithoutNotify(settings.contrast1);
        saturationSlider1.SetValueWithoutNotify(settings.saturation1);

        // Camera 2
        brightnessSlider2.SetValueWithoutNotify(settings.brightness2);
        contrastSlider2.SetValueWithoutNotify(settings.contrast2);
        saturationSlider2.SetValueWithoutNotify(settings.saturation2);

        // Camera 3
        brightnessSlider3.SetValueWithoutNotify(settings.brightness3);
        contrastSlider3.SetValueWithoutNotify(settings.contrast3);
        saturationSlider3.SetValueWithoutNotify(settings.saturation3);
    }

    public void LoadInitialPreset()
    {
        if (presetDropdown.value < 0 || presetDropdown.options.Count == 0) return;

        OnDropdownChanged(presetDropdown.value);
    }

    public void SubmitPresetNameWithEnter(string presetName){AddCurrentAsPreset();}
    public void AddCurrentAsPreset()
    {
        string presetName = saveInputField.text.Trim();
        if (string.IsNullOrEmpty(presetName)) return;

        var current = new Settings(
            colorAdjustments1.postExposure.value,
            colorAdjustments1.contrast.value,
            colorAdjustments1.saturation.value,

            colorAdjustments2.postExposure.value,
            colorAdjustments2.contrast.value,
            colorAdjustments2.saturation.value,

            colorAdjustments3.postExposure.value,
            colorAdjustments3.contrast.value,
            colorAdjustments3.saturation.value
        );

        SettingsManager.SavePreset(presetName, current);
        PopulateDropdown(); // refresh UI

        saveInputField.text = "";

        Debug.Log("ADDED: " + presetName);
    }

    public void UpdateCurrentPreset()
    {
        // Ensure a valid selection exists
        if (presetDropdown.value < 0 || presetDropdown.options.Count == 0) return;

        string presetName = presetDropdown.options[presetDropdown.value].text;

        var current = new Settings(
            colorAdjustments1.postExposure.value,
            colorAdjustments1.contrast.value,
            colorAdjustments1.saturation.value,

            colorAdjustments2.postExposure.value,
            colorAdjustments2.contrast.value,
            colorAdjustments2.saturation.value,

            colorAdjustments3.postExposure.value,
            colorAdjustments3.contrast.value,
            colorAdjustments3.saturation.value
        );

        SettingsManager.SavePreset(presetName, current);
    }

    public void DeleteCurrentPreset()
    {
        int index = presetDropdown.value;
        if (index < 0 || index >= presetDropdown.options.Count) return;

        string presetName = presetDropdown.options[index].text;

        SettingsManager.DeletePreset(presetName);
        PopulateDropdown();

        // Optional: select first option after delete
        if (presetDropdown.options.Count > 0)
        {
            presetDropdown.value = 0;
            OnDropdownChanged(0); // apply new selection
        }
        Debug.Log("DELETED: " + presetName);
    }

}
