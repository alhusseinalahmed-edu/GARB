using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public Toggle fullScreenToggle;
    public Slider mouseXSensitivitySlider;
    public Slider mouseYSensitivitySlider;

    public Slider volumeSlider;
    public AudioMixer mixer;

    void Start()
    {
        // Set the initial value of the slider to match the current mixer volume 
        //ColorBlock colors = overallQualityButtons[0].transform.GetComponent<Button>().colors;
        //colors.normalColor = selectedColor;

        float mixerVolume;
        bool result = mixer.GetFloat("Volume", out mixerVolume);
        if (result)
        {
            float sliderValue = Mathf.InverseLerp(-60f, 0f, mixerVolume);
            volumeSlider.value = sliderValue;
        }

        // Load sensitivity setting from player preferences
        float mouseXSensitivity = PlayerPrefs.GetFloat("MouseXSensitivity", 1f);
        float mouseYSensitivity = PlayerPrefs.GetFloat("MouseYSensitivity", 1f);
        mouseXSensitivitySlider.value = mouseXSensitivity;
        mouseYSensitivitySlider.value = mouseYSensitivity;

    }

    public void OnVolumeSliderChanged(float sliderValue)
    {
        float mixerVolume = Mathf.Lerp(-60f, 0f, sliderValue);
        mixer.SetFloat("Volume", mixerVolume);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetMouseXSensitivity(float sensitivity)
    {
        // Save mouse sensitivity setting to player preferences
        PlayerPrefs.SetFloat("MouseXSensitivity", sensitivity);
    }
    public void SetMouseYSensitivity(float sensitivity)
    {
        // Save mouse sensitivity setting to player preferences
        PlayerPrefs.SetFloat("MouseYSensitivity", sensitivity);
    }

    public void SetQuality(int qualityIndex)
    {
        PlayerPrefs.SetInt("Quality", qualityIndex);
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetAntiAliasing(int antiAliasingIndex)
    {
        PlayerPrefs.SetInt("AntiAliasing", antiAliasingIndex);
        QualitySettings.antiAliasing = antiAliasingIndex;
    }

    public void SetTextureQuality(int textureQualityIndex)
    {
        PlayerPrefs.SetInt("TextureQuality", textureQualityIndex);
        QualitySettings.masterTextureLimit = textureQualityIndex;
    }

    public void SetShadowQuality(int shadowQualityIndex)
    {
        PlayerPrefs.SetInt("ShadowQuality", shadowQualityIndex);
        QualitySettings.shadows = (ShadowQuality)shadowQualityIndex;
    }
}
