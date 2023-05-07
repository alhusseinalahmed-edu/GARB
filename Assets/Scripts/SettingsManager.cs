using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullScreenToggle;
    public Slider mouseSensitivitySlider;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown antiAliasingDropdown;
    public TMP_Dropdown textureQualityDropdown;
    public TMP_Dropdown shadowQualityDropdown;

    public string[] antiAliasingOptions = new string[] { "0", "2x", "4x", "8x" };
    public string[] textureOptions = new string[] { "0", "1", "2", "3" };
    public ShadowResolution[] shadowOptions;
    private Resolution[] resolutions;

    public AudioMixer mixer;
    public Slider volumeSlider;

    private const string volumeParameterName = "Volume";


    void Start()
    {
        // Set the initial value of the slider to match the current mixer volume
        float mixerVolume;
        bool result = mixer.GetFloat(volumeParameterName, out mixerVolume);
        if (result)
        {
            float sliderValue = Mathf.InverseLerp(-60f, 0f, mixerVolume);
            volumeSlider.value = sliderValue;
        }
        // Populate texture quality dropdown with available options
        textureQualityDropdown.ClearOptions();
        textureQualityDropdown.AddOptions(textureOptions.ToList());

        // Set texture quality dropdown to current value
        int currentTexture = QualitySettings.masterTextureLimit;
        textureQualityDropdown.value = Array.IndexOf(textureOptions, currentTexture.ToString());
        textureQualityDropdown.RefreshShownValue();

        // Populate shadow dropdown with available options
        shadowQualityDropdown.ClearOptions();
        List<string> shadowOptionsList = new List<string>();
        foreach (ShadowResolution option in shadowOptions)
        {
            shadowOptionsList.Add(option.ToString());
        }
        shadowQualityDropdown.AddOptions(shadowOptionsList);

        // Set shadow dropdown to current value
        ShadowResolution currentShadow = QualitySettings.shadowResolution;
        shadowQualityDropdown.value = Array.IndexOf(shadowOptions, currentShadow);
        shadowQualityDropdown.RefreshShownValue();

        // Get available screen resolutions
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        // Populate anti-aliasing dropdown with available options
        antiAliasingDropdown.ClearOptions();
        antiAliasingDropdown.AddOptions(antiAliasingOptions.ToList());

        // Set anti-aliasing dropdown to current value
        int currentAA = QualitySettings.antiAliasing;
        antiAliasingDropdown.value = Array.IndexOf(antiAliasingOptions, currentAA.ToString());
        antiAliasingDropdown.RefreshShownValue();

        // Populate quality dropdown with available options
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(QualitySettings.names.ToList());

        // Set quality dropdown to current value
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        // Populate resolution dropdown with available resolutions
        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            resolutionOptions.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Set full-screen toggle based on current screen mode
        fullScreenToggle.isOn = Screen.fullScreen;

        // Load sensitivity setting from player preferences
        float mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        mouseSensitivitySlider.value = mouseSensitivity;

        // Load quality settings from graphics settings
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        antiAliasingDropdown.value = QualitySettings.antiAliasing;
        textureQualityDropdown.value = QualitySettings.masterTextureLimit;
        shadowQualityDropdown.value = (int)QualitySettings.shadowResolution;
    }


    public void OnVolumeSliderChanged(float sliderValue)
    {
        // Convert the slider value to a mixer volume value and set it on the mixer
        float mixerVolume = Mathf.Lerp(-60f, 0f, sliderValue);
        mixer.SetFloat(volumeParameterName, mixerVolume);
    }

    public void SetResolution(int resolutionIndex)
    {
        // Set screen resolution based on selected option
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        // Set screen mode based on toggle state
        Screen.fullScreen = isFullScreen;
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        // Save mouse sensitivity setting to player preferences
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
    }

    public void SetQuality(int qualityIndex)
    {
        // Set quality level based on selected option
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetAntiAliasing(int antiAliasingIndex)
    {
        // Set anti-aliasing level based on selected option
        int selectedAA = int.Parse(antiAliasingOptions[antiAliasingIndex]);
        QualitySettings.antiAliasing = selectedAA;
        Debug.Log(QualitySettings.antiAliasing);
    }

    public void SetTextureQuality(int textureQualityIndex)
    {
        // Set texture quality based on selected option
        int selectedTexture = int.Parse(textureOptions[textureQualityIndex]);
        QualitySettings.masterTextureLimit = selectedTexture;
    }

    public void SetShadowQuality(int shadowQualityIndex)
    {
        // Set shadow quality based on selected option
        ShadowResolution selectedShadow = shadowOptions[shadowQualityIndex];
        QualitySettings.shadowResolution = selectedShadow;
    }
}
