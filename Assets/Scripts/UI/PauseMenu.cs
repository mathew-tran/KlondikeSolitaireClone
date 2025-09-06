using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public TMP_Text Text;
    public Button RetryButtonReference;
    public GamesWonText GamesWonText;

    public bool bIsComplete = false;
    public AudioSource WinSound;


    [ContextMenu("OnWin")]
    public void OnWin()
    {
        SetupWinScreen();
        gameObject.SetActive(false);
        Toggle();
        bIsComplete = true;

        if (Settings.GetInstance().GetPlaySFX())
        {
            WinSound.Play();
        }


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
        RetryButtonReference.gameObject.SetActive(false);
        GamesWonText.Show();

    }
}
