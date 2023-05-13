using UnityEngine;
using UnityEngine.UI;

public class ResolutionButton : MonoBehaviour
{
    public SettingsManager settingsManager;
    public int width;
    public int height;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        settingsManager.SetResolution(width, height);
    }
}
