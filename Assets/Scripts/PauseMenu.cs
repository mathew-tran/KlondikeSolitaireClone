using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public TMP_Text Text;
    public Button ResumeButtonReference;

    public bool bIsComplete = false;

    public void OnWin()
    {
        SetupWinScreen();
        gameObject.SetActive(false);
        Toggle();
        bIsComplete = true;


    }
    public void Toggle()
    {
        if (bIsComplete)
        {
            return;
        }

        gameObject.SetActive(!isActiveAndEnabled);
        if (isActiveAndEnabled)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

    private void SetupWinScreen()
    {
        Text.text = "YOU WIN";
        ResumeButtonReference.gameObject.SetActive(false);

    }
}
