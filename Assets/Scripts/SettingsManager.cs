using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Toggle fullScreenToggle;

    public Slider mouseXSensitivitySlider;
    public Slider mouseYSensitivitySlider;
    public Slider zoomSensitivtySlider;
    public Slider volumeSlider;

    public AudioMixer mixer;

    public InputHandler inputHandler;


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
        float zoomSensitivityMultiplier = PlayerPrefs.GetFloat("ZoomSensitivityMultiplier", 1f);
        mouseXSensitivitySlider.value = mouseXSensitivity;
        mouseYSensitivitySlider.value = mouseYSensitivity;
        zoomSensitivtySlider.value = zoomSensitivityMultiplier;

    }

    public void OnVolumeSliderChanged(float sliderValue)
    {
        float mixerVolume = Mathf.Lerp(-60f, 0f, sliderValue);
        mixer.SetFloat("Volume", mixerVolume);
    }

    public void SetFullScreen()
    {
        if(Screen.fullScreen)
        {
            Screen.fullScreen = false;
        }
        else
        {
            Screen.fullScreen = true;
        }
    }

    public void SetMouseXSensitivity(float sensitivity)
    {
        // Save mouse sensitivity setting to player preferences
        PlayerPrefs.SetFloat("MouseXSensitivity", sensitivity);
        mouseXSensitivitySlider.value = sensitivity;
        UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "Game")
        {
            inputHandler.UpdateInputOptions();
        }
    }
    public void SetMouseYSensitivity(float sensitivity)
    {
        // Save mouse sensitivity setting to player preferences
        PlayerPrefs.SetFloat("MouseYSensitivity", sensitivity);
        mouseYSensitivitySlider.value = sensitivity;
        UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
        if(currentScene.name == "Game")
        {
            inputHandler.UpdateInputOptions();
        }
    }
    public void SetZoomSensitivityMultiplier(float multiplier)
    {
        // Save mouse sensitivity setting to player preferences
        PlayerPrefs.SetFloat("ZoomSensitivityMultiplier", multiplier);
        zoomSensitivtySlider.value = multiplier;
        UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "Game")
        {
            inputHandler.UpdateInputOptions();
        }
    }
    public void SetQuality(int qualityIndex)
    {
        PlayerPrefs.SetInt("Quality", qualityIndex);
        QualitySettings.SetQualityLevel(qualityIndex);
        SetAntiAliasing(8);
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
    public void Reset()
    {
        //SetFullScreen(true);
        SetMouseXSensitivity(1);
        SetMouseYSensitivity(1);
        SetZoomSensitivityMultiplier(1);
        SetAntiAliasing(8);
        SetQuality(5);
        SetShadowQuality(4);
        SetTextureQuality(0);
    }
}
