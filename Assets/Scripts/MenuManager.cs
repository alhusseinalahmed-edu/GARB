using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public AudioMixer audioMixer;
    [SerializeField] Menu[] menus;
    private void Awake()
    {
        instance = this;
    }
    public void OpenMenu(string menuName)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == menuName)
            {
                menus[i].Open();
            }
            else if (menus[i].isOpen)
            {
                CloseMenu(menus[i]);
            }
        }
    }
    public void OpenMenu(Menu menu)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].isOpen)
            {
                CloseMenu(menus[i]);
            }
        }
        menu.Open();
    }
    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
    public void ChangeGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }
    public void SetFullScreen(bool istrue)
    {
        Screen.fullScreen = istrue;
    }

}
