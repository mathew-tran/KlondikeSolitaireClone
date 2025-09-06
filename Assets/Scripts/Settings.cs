using UnityEngine;

public class Settings : MonoBehaviour
{
    private static Settings Instance;
    public static Settings GetInstance()
    {
        return Instance;
    }
    private void Awake()
    {
        Instance = this;
    }

    public void SetAutoDraw(bool autoDraw)
    {
        PlayerPrefs.SetInt("bAutoDraw", autoDraw ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool GetAutoDraw()
    {
        return PlayerPrefs.GetInt("bAutoDraw", 1) == 1;
    }

    public bool GetShowHints()
    {
        return PlayerPrefs.GetInt("bShowHints", 1) == 1;
    }
    public void SetShowHints(bool showHints)
    {
        PlayerPrefs.SetInt("bShowHints", showHints ? 1 : 0);
        PlayerPrefs.Save();
    }
}
